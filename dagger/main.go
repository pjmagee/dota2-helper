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
// +defaultPath="."
// +ignore=["**/bin", "**/obj", "**/.idea", "**/docs", "**/.github", "**/.gitignore"]
	git *dagger.Directory) (string, error) {

	return m.DotnetContainer().
		WithDirectory("/repo", git).
		WithWorkdir("/repo/src").
		// https://github.com/dotnet/sdk/issues/40913
		WithExec([]string{"dotnet", "build", "Dota2Helper"}).
		Stdout(context.Background())
}

// Get a dotnet container with the required tools
func (m *Dota2Helper) DotnetContainer() *dagger.Container {

	cache := dag.CacheVolume("nuget-cache")

	return dag.Container().
		From("mcr.microsoft.com/dotnet/sdk:8.0-alpine").
		WithMountedCache("/root/.nuget/packages", cache).
		WithExec([]string{"dotnet", "tool", "install", "--global", "nuget-license"}).
		WithExec([]string{"dotnet", "tool", "install", "--global", "GitVersion.Tool"}).
		WithExec([]string{"sh", "-c", "export PATH=\"$PATH:/root/.dotnet/tools\""})
}

// Scan the project for used packages and bundle licenses and metadata into a single file
func (m *Dota2Helper) GetPackagesFile(
    // +defaultPath="."
    // +ignore=["**/bin", "**/obj", "**/.idea", "**/docs", "**/.github", "**/.gitignore"]
	git *dagger.Directory) *dagger.File {

	return m.DotnetContainer().
		WithDirectory("/repo", git).
		WithWorkdir("/repo/src").
		WithExec([]string{"dotnet", "restore"}).
		WithExec([]string{"sh", "-c", "~/.dotnet/tools/nuget-license -i Dota2Helper.sln -o JsonPretty > ./packages.json || true"}).
		File("/repo/src/packages.json")
}

// Get the semver details of the current git repository
func (m *Dota2Helper) GitVersion(
// +defaultPath="."
// +ignore=["**/bin", "**/obj", "**/.idea", "**/docs", "**/.github", "**/.gitignore"]
	git *dagger.Directory) (GitVersion, error) {

	version, err := m.DotnetContainer().
		WithDirectory("/repo", git).
		WithWorkdir("/repo").
		WithExec([]string{"sh", "-c", "~/.dotnet/tools/dotnet-gitversion"}).
		Stdout(context.Background())

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
func (m *Dota2Helper) DotnetPublish(
// +defaultPath="."
// +ignore=["**/bin", "**/obj", "**/.idea", "**/docs", "**/.github", "**/.gitignore"]
	git *dagger.Directory,
	version string) *dagger.Container {

	return m.DotnetContainer().
		WithDirectory("/repo", git).
		WithWorkdir("/repo/src").
		WithFile("./Dota2Helper.Desktop/packages.json", m.GetPackagesFile(git)).
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

// Publish the project in release mode and create a zip file
func (m *Dota2Helper) PublishAsZip(
// +defaultPath="."
// +ignore=["**/bin", "**/obj", "**/.idea", "**/docs", "**/.github", "**/.gitignore"]
	git *dagger.Directory) (*dagger.File, error) {

	gitVersion, err := m.GitVersion(git)

	if err != nil {
		return nil, err
	}

	version := gitVersion.MajorMinorPatch
	published := m.DotnetPublish(git, version)
	assets := published.Directory("/publish")

	zip := dag.
		Arc().
		ArchiveDirectory(fmt.Sprintf("Dota2Helper_v%s_windows_amd64", version), assets).
		Zip()

	return zip, nil
}

// Create a release on github
func (m *Dota2Helper) Release(
// +defaultPath="/"
	git *dagger.Directory,
	token *dagger.Secret) error {

	var zip, _ = m.PublishAsZip(git)
	var gitVersion, _ = m.GitVersion(git)

	ghOpts := dagger.GhOpts{
		Token: token,
		Repo:  "github.com/pjmagee/dota2-helper", Source: git,
	}

	tag := fmt.Sprintf("v%s", gitVersion.MajorMinorPatch)
	title := fmt.Sprintf("Dota 2 Helper %s", gitVersion.MajorMinorPatch)
	releaseOptions := dagger.GhReleaseCreateOpts{
		Target:        "main",
		Files:         []*dagger.File{zip},
		Latest:        dagger.GhLatestLatestAuto,
		VerifyTag:     true,
		Draft:         true,
		GenerateNotes: true,
	}

	err := dag.Gh(ghOpts).Release().Create(context.Background(), tag, title, releaseOptions)
	return err
}

// Create audio assets using OpenAI TTS
func (m *Dota2Helper) CreateAudioAssets(
	ctx context.Context,
// The OpenAI API key
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

	build := dag.Container().
		From("curlimages/curl:latest").
		WithSecretVariable("OPEN_AI_API_KEY", secret).
		WithoutEntrypoint().
		WithWorkdir("/audio")

	var template = `{
		"model": "tts-1-hd",
		"input": "%s",
		"voice": "nova"
	}`

	for _, text := range assets {
		cmd := fmt.Sprintf(`curl -v https://api.openai.com/v1/audio/speech \
            			-H "Authorization: Bearer $OPEN_AI_API_KEY" \
            			-H "Content-Type: application/json" \
            			-d '%s' \
            			--output "%s"`, fmt.Sprintf(template, text), fmt.Sprintf("%s.mp3", text))
		build = build.WithExec([]string{"sh", "-c", cmd})
	}

	ctr, err := build.Sync(ctx)

	if err != nil {
		return nil, err
	}

	return ctr.Directory("/audio"), nil
}
