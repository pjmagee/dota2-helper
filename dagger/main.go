package main

import (
	"context"
	"dagger/dota-2-helper/internal/dagger"
	"encoding/json"
	"fmt"
)

// Module with a set of functions to help with the build and release of the Dota 2 Helper project
type Dota2Helper struct {
	PublishSettings []PublishSetting
}

// Runtime Identifier for the target platform
// This cannot be an enum because of the Rid value containing a hyphen
// https://learn.microsoft.com/en-us/dotnet/core/rid-catalog#using-rids
type Rid = string

// Runtime Identifiers
const (
	WinX64     Rid = "win-x64"
	WinArm64   Rid = "win-arm64"
	OsxX64     Rid = "osx-x64"
	OsxArm64   Rid = "osx-arm64"
	LinuxX64   Rid = "linux-x64"
	LinuxArm64 Rid = "linux-arm64"
)

type Publish struct {
	Rid    Rid
	Suffix string
}

type PublishSetting struct {
	Platform string
	Outputs  []Publish
}

func New() *Dota2Helper {
	return &Dota2Helper{
		PublishSettings: []PublishSetting{
			{
				Platform: "windows",
				Outputs: []Publish{
					{Rid: WinX64, Suffix: "windows_amd64"},
					{Rid: WinArm64, Suffix: "windows_arm64"},
				},
			},
			{
				Platform: "osx",
				Outputs: []Publish{
					{Rid: OsxX64, Suffix: "osx_amd64"},
					{Rid: OsxArm64, Suffix: "osx_arm64"},
				},
			},
			{
				Platform: "linux",
				Outputs: []Publish{
					{Rid: LinuxX64, Suffix: "linux_amd64"},
					{Rid: LinuxArm64, Suffix: "linux_arm64"},
				},
			},
		},
	}
}

func (m *Dota2Helper) GetSuffix(rid Rid) string {
	for _, setting := range m.PublishSettings {
		for _, output := range setting.Outputs {
			if output.Rid == rid {
				return output.Suffix
			}
		}
	}
	return ""
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
	git *dagger.Directory,
) (string, error) {

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
	git *dagger.Directory,
) *dagger.File {

	return m.DotnetContainer().
		WithDirectory("/repo", git).
		WithWorkdir("/repo/src").
		WithExec([]string{"dotnet", "restore"}).
		WithExec([]string{"sh", "-c", "~/.dotnet/tools/nuget-license -i Dota2Helper.sln -o JsonPretty > ./packages.json || true"}).
		File("/repo/src/packages.json")
}

// Get the semver details of the current git repository
func (m *Dota2Helper) GetGitVersion(
// +defaultPath="."
// +ignore=["**/bin", "**/obj", "**/.idea", "**/docs", "**/.github", "**/.gitignore"]
	git *dagger.Directory,
) (GitVersion, error) {

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
// win-x64, win-arm64, osx-x64, osx-arm64, linux-x64, linux-arm64
	rid Rid,
// The version to publish e.g 1.0.0
	version string,
) *dagger.Container {

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
			fmt.Sprintf("%s", rid),
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
	git *dagger.Directory,
	rid Rid,
) *dagger.File {

	gitVersion, err := m.GetGitVersion(git)

	if err != nil {
		return nil
	}

	version := gitVersion.MajorMinorPatch
	published := m.DotnetPublish(git, rid, version)
	assets := published.Directory("/publish")
	suffix := m.GetSuffix(rid)

	zip := dag.
		Arc().
		ArchiveDirectory(fmt.Sprintf("Dota2Helper_v%s_%s.zip", version, suffix), assets).
		Zip()

	return zip
}

// Create a release on github
func (m *Dota2Helper) Release(
// +defaultPath="/"
	git *dagger.Directory,
// PAT with access
	token *dagger.Secret) error {

	zipFiles := []*dagger.File{
		m.PublishAsZip(git, WinX64),
		// m.PublishAsZip(git, WinArm64),
		//m.PublishAsZip(git, OsxX64),
		//m.PublishAsZip(git, OsxArm64),
		//m.PublishAsZip(git, LinuxX64),
		//m.PublishAsZip(git, LinuxArm64),
	}

	var gitVersion, _ = m.GetGitVersion(git)

	ghOpts := dagger.GhOpts{
		Token: token,
		Repo:  "github.com/pjmagee/dota2-helper", Source: git,
	}

	tag := fmt.Sprintf("v%s", gitVersion.MajorMinorPatch)
	title := fmt.Sprintf("Dota 2 Helper %s", gitVersion.MajorMinorPatch)

	releaseOptions := dagger.GhReleaseCreateOpts{
		Target:        "main",
		Files:         zipFiles,
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
// The OpenAI API key
	secret *dagger.Secret,
) (*dagger.Directory, error) {

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

	ctr, err := build.Sync(context.Background())

	if err != nil {
		return nil, err
	}

	return ctr.Directory("/audio"), nil
}
