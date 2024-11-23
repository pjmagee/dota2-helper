package main

import (
	"context"
	"dagger/dota-2-helper/internal/dagger"
	"encoding/json"
	"fmt"
)

type Dota2Helper struct{}

type GitVersion struct {
	AssemblySemFileVer        string `json:"AssemblySemFileVer"`
	AssemblySemVer            string `json:"AssemblySemVer"`
	BranchName                string `json:"BranchName"`
	BuildMetaData             int    `json:"BuildMetaData"`
	CommitDate                string `json:"CommitDate"`
	CommitsSinceVersionSource int    `json:"CommitsSinceVersionSource"`
	EscapedBranchName         string `json:"EscapedBranchName"`
	FullBuildMetaData         string `json:"FullBuildMetaData"`
	FullSemVer                string `json:"FullSemVer"`
	InformationalVersion      string `json:"InformationalVersion"`
	Major                     int    `json:"Major"`
	MajorMinorPatch           string `json:"MajorMinorPatch"`
	Minor                     int    `json:"Minor"`
	Patch                     int    `json:"Patch"`
	PreReleaseLabel           string `json:"PreReleaseLabel"`
	PreReleaseLabelWithDash   string `json:"PreReleaseLabelWithDash"`
	PreReleaseNumber          int    `json:"PreReleaseNumber"`
	PreReleaseTag             string `json:"PreReleaseTag"`
	PreReleaseTagWithDash     string `json:"PreReleaseTagWithDash"`
	SemVer                    string `json:"SemVer"`
	Sha                       string `json:"Sha"`
	ShortSha                  string `json:"ShortSha"`
	UncommittedChanges        int    `json:"UncommittedChanges"`
	VersionSourceSha          string `json:"VersionSourceSha"`
	WeightedPreReleaseNumber  int    `json:"WeightedPreReleaseNumber"`
}

func (m *Dota2Helper) Build(
	ctx context.Context,
// +defaultPath="."
	git *dagger.Directory) (string, error) {

	cache := dag.CacheVolume("nuget-cache")
	opts := dagger.ContainerWithDirectoryOpts{Exclude: []string{"**/bin/**", "**/obj/**"}}

	return dag.Container().
		From("mcr.microsoft.com/dotnet/sdk:8.0").
		WithDirectory("/repo", git, opts).
		WithWorkdir("/repo/src").
		WithMountedCache("/root/.nuget/packages", cache).
		WithExec([]string{"dotnet", "build"}).
		Stdout(ctx)
}

func (m *Dota2Helper) GitVersion(
	ctx context.Context,
// +defaultPath="."
	git *dagger.Directory) (GitVersion, error) {

	version, err := dag.Container().
		From("gittools/gitversion:latest").
		WithDirectory("/repo", git).
		WithExec([]string{"/tools/dotnet-gitversion", "/repo"}).
		Stdout(ctx)

	if err != nil {
		return GitVersion{}, err
	}

	var gitVersion GitVersion
	err = json.Unmarshal([]byte(version), &gitVersion)

	if err != nil {
		return GitVersion{}, err
	}

	return gitVersion, nil
}

func (m *Dota2Helper) PublishWindows(
	ctx context.Context,
// +defaultPath="."
	git *dagger.Directory,
	version string) *dagger.Container {

	cache := dag.CacheVolume("nuget-cache")

	return dag.Container().
		From("mcr.microsoft.com/dotnet/sdk:8.0").
		WithDirectory("/repo", git, dagger.ContainerWithDirectoryOpts{
			Exclude: []string{"**/bin/**", "**/obj/**"},
		}).
		WithMountedCache("/root/.nuget/packages", cache).
		WithWorkdir("/repo/src").
		WithExec([]string{
			"dotnet",
			"publish",
			"Dota2Helper.Desktop",
			"-c",
			"Release",
			"-r",
			"win-x64",
			"--self-contained",
			"true",
			fmt.Sprintf("/p:Version=%s", version),
			"/p:PublishSingleFile=true",
			"-o",
			"/publish",
			"--verbosity", "normal",
		})
}

func (m *Dota2Helper) Zip(
	ctx context.Context,
// +defaultPath="."
	git *dagger.Directory) (*dagger.File, error) {

	gitVersion, err := m.GitVersion(ctx, git)

	if err != nil {
		return nil, err
	}

	version := gitVersion.MajorMinorPatch
	published := m.PublishWindows(ctx, git, version)
	assets := published.Directory("/publish")

	zip := dag.
		Arc().
		ArchiveDirectory(fmt.Sprintf("Dota2Helper_v%s_windows_amd64", version), assets).
		Zip()

	return zip, nil
}

func (m *Dota2Helper) Release(
	ctx context.Context,
// +defaultPath="/"
	git *dagger.Directory,
	token *dagger.Secret) error {

	gitVersion, err := m.GitVersion(ctx, git)

	if err != nil {
		return err
	}

	zip, err := m.Zip(ctx, git)

	ghOpts := dagger.GhOpts{
		Token: token,
		Repo:  "github.com/pjmagee/dota2-helper", Source: git,
	}

	tag := fmt.Sprintf("v%s", gitVersion.MajorMinorPatch)
	title := fmt.Sprintf("Dota 2 Helper %s", gitVersion.MajorMinorPatch)

	err = dag.Gh(ghOpts).Release().Create(ctx, tag, title, dagger.GhReleaseCreateOpts{
		Target:        "main",
		Files:         []*dagger.File{zip},
		Latest:        true,
		VerifyTag:     true,
		Draft:         true,
		GenerateNotes: true,
	})

	if err != nil {
		return err
	}

	return nil
}
