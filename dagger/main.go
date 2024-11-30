package main

import (
	"context"
	"dagger/dota-2-helper/internal/dagger"
	"encoding/json"
	"fmt"
)

type Dota2Helper struct {
}

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

// Build the project
func (m *Dota2Helper) Build(
	ctx context.Context,
// +defaultPath="."
	git *dagger.Directory) (string, error) {

	cache := dag.CacheVolume("nuget-cache")
	opts := dagger.ContainerWithDirectoryOpts{Exclude: []string{"**/bin/**", "**/obj/**"}}

	return m.Dotnet().
		WithDirectory("/repo", git, opts).
		WithWorkdir("/repo/src").
		WithMountedCache("/root/.nuget/packages", cache).
		// https://github.com/dotnet/sdk/issues/40913
		WithExec([]string{"dotnet", "build", "Dota2Helper"}).
		Stdout(ctx)
}

// Get a dotnet container with the required tools
func (m *Dota2Helper) Dotnet() *dagger.Container {
	return dag.Container().
		From("mcr.microsoft.com/dotnet/sdk:9.0").
		WithExec([]string{"dotnet", "tool", "install", "--global", "nuget-license"}).
		WithExec([]string{"dotnet", "tool", "install", "--global", "GitVersion.Tool"}).
		WithExec([]string{"sh", "-c", "export PATH=\"$PATH:/root/.dotnet/tools\""})
}

func (m *Dota2Helper) GetPackages(
	ctx context.Context,
// +defaultPath="."
	git *dagger.Directory) (string, error) {

	return m.Dotnet().
		WithDirectory("/repo", git).
		WithWorkdir("/repo/src").
		WithExec([]string{"sh", "-c", "~/.dotnet/tools/nuget-license -i Dota2Helper.sln -o JsonPretty || true"}).
		Stdout(ctx)
}

// Get the semver details of the current git repository
func (m *Dota2Helper) GitVersion(
	ctx context.Context,
// +defaultPath="."
	git *dagger.Directory) (GitVersion, error) {

	version, err := m.Dotnet().
		WithDirectory("/repo", git).
		WithWorkdir("/repo").
		WithExec([]string{"sh", "-c", "~/.dotnet/tools/dotnet-gitversion"}).
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

// Publish the project in release mode
func (m *Dota2Helper) PublishWindows(
	ctx context.Context,
// +defaultPath="."
	git *dagger.Directory,
	version string) *dagger.Container {

	cache := dag.CacheVolume("nuget-cache")

	packages, _ := m.GetPackages(ctx, git)

	return m.Dotnet().
		WithDirectory("/repo", git, dagger.ContainerWithDirectoryOpts{
			Exclude: []string{},
		}).
		WithMountedCache("/root/.nuget/packages", cache).
		WithWorkdir("/repo/src").
		WithNewFile("./Dota2Helper.Desktop/packages.json", packages).
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

// Zip the published files
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

// Create a release on github
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
		Latest:        dagger.GhLatestLatestAuto,
		VerifyTag:     true,
		Draft:         true,
		GenerateNotes: true,
	})

	if err != nil {
		return err
	}

	return nil
}

// Create audio assets using OpenAI TTS
func (m *Dota2Helper) CreateAudioAssets(
	ctx context.Context,
	secret *dagger.Secret) (*dagger.Directory, error) {

	assets := []string{
		"Pull",
		"Stack",
		"Bounty",
		"Power rune",
		"Lotus Pool",
		"Wisdom rune",
		"Radiant Tormentor",
		"Dire Tormentor",
		"Roshan",
		"Tier 1s",
		"Tier 2s",
		"Tier 3s",
		"Tier 4s",
		"Tier 5s",
	}

	var template = `{
		"model": "tts-1-hd",
		"input": "%s",
		"voice": "nova"
	}`

	build := dag.Container().
		From("curlimages/curl:latest").
		WithSecretVariable("OPEN_AI_API_KEY", secret).
		WithoutEntrypoint().
		WithWorkdir("/audio")

	for _, text := range assets {
		payload := fmt.Sprintf(template, text)
		cmd := fmt.Sprintf(`curl -v https://api.openai.com/v1/audio/speech \
            			-H "Authorization: Bearer $OPEN_AI_API_KEY" \
            			-H "Content-Type: application/json" \
            			-d '%s' \
            			--output "%s"`, payload, fmt.Sprintf("%s.mp3", text))
		build = build.WithExec([]string{"sh", "-c", cmd})
	}

	ctr, err := build.Sync(ctx)

	if err != nil {
		return nil, err
	}

	return ctr.Directory("/audio"), nil
}
