ARG REPO=mcr.microsoft.com/dotnet/core/runtime

# Installer image
FROM buildpack-deps:focal-curl as installer

# Retrieve ASP.NET Core
RUN aspnetcore_version=5.0.0-preview.4.20206.19 \
    && curl -SL --output aspnetcore.tar.gz https://dotnetcli.azureedge.net/dotnet/aspnetcore/Runtime/$aspnetcore_version/aspnetcore-runtime-$aspnetcore_version-linux-x64.tar.gz \
    && aspnetcore_sha512='02a0c0a6477186d7768afe87947c7fcbb16f1047fd4838a11617609b28425b4e64fc64947bce4fb219e8055b39f7225f632ac15e5178c1b6c9c0e267e7ff95d8' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && tar -ozxf aspnetcore.tar.gz ./shared/Microsoft.AspNetCore.App \
    && rm aspnetcore.tar.gz


# ASP.NET Core image
FROM $REPO:5.0-focal

COPY --from=installer ["/shared/Microsoft.AspNetCore.App", "/usr/share/dotnet/shared/Microsoft.AspNetCore.App"]
