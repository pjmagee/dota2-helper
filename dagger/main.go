package main

import (
	"context"
	"dagger/dota-2-helper/internal/dagger"
	"fmt"
	"strings"
)

type Dota2Helper struct{}

func (m *Dota2Helper) Build(
	ctx context.Context,
	// +defaultPath="/src"
	src *dagger.Directory) (string, error) {

	cache := dag.CacheVolume("nuget-cache")

	return dag.Container().
		From("mcr.microsoft.com/dotnet/sdk:8.0").
		WithDirectory("/src", src, dagger.ContainerWithDirectoryOpts{
			Exclude: []string{"**/bin/**", "**/obj/**"},
		}).
		WithWorkdir("/src").
		WithMountedCache("/root/.nuget/packages", cache).
		WithExec([]string{
			"dotnet",
			"build"}).
		Stdout(ctx)
}

func (m *Dota2Helper) PublishWindows(
	ctx context.Context,
	// +defaultPath="/src"
	src *dagger.Directory,
	version string) *dagger.Container {

	cache := dag.CacheVolume("nuget-cache")

	return dag.Container().
		From("mcr.microsoft.com/dotnet/sdk:8.0").
		WithDirectory("/src", src, dagger.ContainerWithDirectoryOpts{
			Exclude: []string{"**/bin/**", "**/obj/**"},
		}).
		WithMountedCache("/root/.nuget/packages", cache).
		WithWorkdir("/src").
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

func (m *Dota2Helper) Release(
	ctx context.Context,
	// +defaultPath="/"
	git *dagger.Directory,
	tag string,
	token *dagger.Secret) error {

	version := strings.TrimPrefix(tag, "v")
	published := m.PublishWindows(ctx, git.Directory("src"), version)
	assets := published.Directory("/publish")

	zip := dag.Arc().ArchiveDirectory(fmt.Sprintf("Dota2Helper_%s_windows_amd64", tag), assets).Zip()
	ghOpts := dagger.GhOpts{Token: token, Repo: "github.com/pjmagee/dota2-helper", Source: git}
	err := dag.Gh(ghOpts).Release().Create(ctx, tag, "", dagger.GhReleaseCreateOpts{
		Target: "main",
		Files: []*dagger.File{
			zip,
		},
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
