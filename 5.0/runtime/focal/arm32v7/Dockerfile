ARG REPO=mcr.microsoft.com/dotnet/core/runtime-deps

# Installer image
FROM arm32v7/buildpack-deps:focal-curl as installer

# Retrieve .NET Core
RUN dotnet_version=5.0.0-preview.4.20209.4 \
    && curl -SL --output dotnet.tar.gz https://dotnetcli.azureedge.net/dotnet/Runtime/$dotnet_version/dotnet-runtime-$dotnet_version-linux-arm.tar.gz \
    && dotnet_sha512='3346558b0df757b3caafb5f78d198f6ad27c444da1fa26f8b2ba9eaaed49c6ffd631114f5b7f16672f24861bf4f80ab0ecc4cc7ee0e4a2bed1317cc6feec8715' \
    && echo "$dotnet_sha512 dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /dotnet \
    && tar -ozxf dotnet.tar.gz -C /dotnet \
    && rm dotnet.tar.gz


# .NET Core runtime image
FROM $REPO:5.0-focal-arm32v7

COPY --from=installer ["/dotnet", "/usr/share/dotnet"]

RUN ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
