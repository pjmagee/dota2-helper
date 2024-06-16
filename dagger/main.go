// A generated module for Dota2Helper functions
//
// This module has been generated via dagger init and serves as a reference to
// basic module structure as you get started with Dagger.
//
// Two functions have been pre-created. You can modify, delete, or add to them,
// as needed. They demonstrate usage of arguments and return types using simple
// echo and grep commands. The functions can be called from the dagger CLI or
// from one of the SDKs.
//
// The first line in this comment block is a short description line and the
// rest is a long description with more detail on the module's purpose or usage,
// if appropriate. All modules should have a short description.

package main

import (
	"context"
	"dagger/dota-2-helper/internal/dagger"
	"fmt"
)

type Dota2Helper struct{}

func (m *Dota2Helper) Build(ctx context.Context, directory *Directory) (string, error) {
	return dag.Container().
		From("mcr.microsoft.com/dotnet/sdk:8.0").
		WithDirectory("/src", directory, dagger.ContainerWithDirectoryOpts{
			Exclude: []string{"**/bin/**", "**/obj/**"},
		}).
		WithWorkdir("/src").
		WithExec([]string{
			"dotnet",
			"build"}).
		Stdout(ctx)
}

func (m *Dota2Helper) PublishWindows(ctx context.Context, directory *Directory, version string) *dagger.Container {

	return dag.Container().
		From("mcr.microsoft.com/dotnet/sdk:8.0").
		WithDirectory("/src", directory, dagger.ContainerWithDirectoryOpts{
			Exclude: []string{"**/bin/**", "**/obj/**"},
		}).
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
			"/publish"})
}

func (m *Dota2Helper) Release(
	ctx context.Context,
	directory *Directory,
	tag string,
	token *dagger.Secret) (dagger.Void, error) {

	var title string = ""
	published := m.PublishWindows(ctx, directory, tag)
	assets := published.Directory("/publish")

	gh := dag.Gh(dagger.GhOpts{
		Token: token,
		Repo:  "github.com/pjmagee/dota2-helper",
	})

	zip := dag.Arc(dagger.ArcOpts{}).ArchiveDirectory(fmt.Sprintf("Dota2Helper_%s_windows_amd64", tag), assets).Zip()

	ghRelease := gh.Release()

	return ghRelease.Create(ctx, tag, title, dagger.GhReleaseCreateOpts{
		Target:        "main",
		Files:         []*dagger.File{zip},
		Latest:        true,
		VerifyTag:     true,
		Draft:         true,
		GenerateNotes: true,
	})
}
