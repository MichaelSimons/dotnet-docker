ARG REPO=mcr.microsoft.com/dotnet/core/runtime

# Installer image
FROM arm32v7/buildpack-deps:buster-curl as installer

# Retrieve ASP.NET Core
RUN aspnetcore_version=5.0.0-preview.4.20206.19 \
    && curl -SL --output aspnetcore.tar.gz https://dotnetcli.azureedge.net/dotnet/aspnetcore/Runtime/$aspnetcore_version/aspnetcore-runtime-$aspnetcore_version-linux-arm.tar.gz \
    && aspnetcore_sha512='a00b63e27f4ff2c1b3f5bca8f82de6e461586d590715b209144bd1d3ddab65668b8b1ce1f5aa5da351fc381eec8b18cc2c574228b0dd9fc337cbe7c4a61f0cb8' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && tar -ozxf aspnetcore.tar.gz ./shared/Microsoft.AspNetCore.App \
    && rm aspnetcore.tar.gz


# ASP.NET Core image
FROM $REPO:5.0-buster-slim-arm32v7

COPY --from=installer ["/shared/Microsoft.AspNetCore.App", "/usr/share/dotnet/shared/Microsoft.AspNetCore.App"]
