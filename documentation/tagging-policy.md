# Image Tagging Policy

This document describes the tagging policy for the official .NET Docker images.

This policy aligns with the tagging practices utilized by the [Official Images on Docker Hub](https://hub.docker.com/search?q=&type=image&image_filter=official).

## Simple Tags
_Simple Tags_ reference an image for a single platform (e.g. `Windows amd64` or `Linux arm64v8`).

1. `<Major.Minor.Patch .NET Version>-<OS>-<Architecture>`

    **Examples**

    * `5.0.0-focal-amd64`
    * `5.0.0-focal-arm64v8`
    * `3.1.9-nanoserver-1809`
    * `2.1.23-alpine3.12`
    * `2.1.23-stretch-slim-arm32v7`

1. `<Major.Minor .NET Version>-<OS>-<Architecture>`

    **Examples**

    * `5.0-focal-arm64v8`
    * `5.0-focal-amd64`
    * `3.1-nanoserver-1809`
    * `3.1-alpine3.12`
    * `2.1-stretch-slim-arm32v7`

## Shared Tags

_Shared Tags_ [support multiple platforms](https://blog.docker.com/2017/09/docker-official-images-now-multi-platform/) and have the following characteristics:

* Include entries for all supported architectures.

* Include Linux entries based on Debian.

* Include Windows Nano Server entries for each supported version.

1. `<Major.Minor.Patch .NET Version>-<OS>`

    **Examples**

    * `5.0.2-focal`
    * `5.0.2-alpine3.12`
    * `5.0.2-windowservercore-ltsc2019`

1. `<Major.Minor.Patch .NET Version>`

    **Examples**

    * `5.0.0`
    * `3.1.9`
    * `2.1.23`

1. `<Major.Minor .NET Version>`

    **Examples**

    * `5.0`
    * `3.1`
    * `2.1`

1. `<Major.Minor .NET Version>-<OS>`

    **Examples**

    * `5.0-focal`
    * `5.0-alpine3.12`
    * `5.0-windowservercore-ltsc2019`

1. `latest`

    * dotnet - `latest` will reference the most recent non-prerelease `<Major.Minor.Patch Version>` image.
    * dotnet/nightly - `latest` will reference the most recent `<Major.Minor.Patch Version>` image. This implies `latest` will at times reference prerelease versions. In the event when there are multiple active prerelease versions (e.g. 3.1 preview 3 and 5.0 preview 1), `latest` will reference the lower prerelease version (e.g. 3.1 preview 3) until the point when the lower version (e.g. 3.1) is released. Once this happens, `latest` will reference the higher version (e.g. 5.0 preview 1).

## Tag Parts

1. `<Major.Minor.Patch .NET Version>` - The `Major.Minor.Patch` number of the .NET version included in the image.

    * Tags which use this version format are considered _fixed tags_. The .NET related contents of the references images are gauranteed to not change.
    * In the event serving of the .NET contents of the image is required outside of a .NET service release, a `-n` suffix will be added to the .NET version number (e.g. 5.0.1-1).

1. `<Major.Minor .NET Version>` - The `Major.Minor` number of the .NET version included in the image.

    * Tags which use this version format are considered _floating tags_. These tags are continuously updated to always reference the .NET images with the most recent patch available for the specified `Major.Minor` release.

1. `<OS>` - The name of the OS release and variant the image is based upon.

    * The image the tag references is automatically updated whenever a new OS patch is released. The OS release name doesn't support pinning to specific OS patches. If OS patch pinning is required then the image digest should be used (e.g. `mcr.microsoft.com/dotnet/runtime@sha256:4d3d5a5131a0621509ab8a75f52955f2d0150972b5c5fb918e2e59d4cb9a9823`).

1. `<Architecture>` - The architecture the image is based on.
    * For Windows, `amd64` is the only architecture supported and is excluded.
    * For .NET Core 2.1 and 3.1, `amd64` is the implied default if no architecture is specified.

## Tag Listing

Each [Docker Hub repository](https://hub.docker.com/_/microsoft-dotnet) contains a detailed listing of all supported tags. The listing is broken apart by OS platform (e.g. `Linux amd64` or `Nano Server, version 20H2 amd64 Tags`). Each row of the detailed listing represents a single image and contains all of the tags that reference it.

Tags | Dockerfile | OS Version
-----------| -------------| -------------
5.0.2-buster-slim-amd64, 5.0-buster-slim-amd64, 5.0.2-buster-slim, 5.0-buster-slim, 5.0.2, 5.0, latest | [Dockerfile](https://github.com/dotnet/dotnet-docker/blob/master/src/runtime/5.0/buster-slim/amd64/Dockerfile) | Debian 10

## Policy Changes

In the event that a change is needed to the tagging patterns used, all tags will continue to be supported for their original lifetime. As a contrived example
