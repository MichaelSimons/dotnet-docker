ARG REPO=mcr.microsoft.com/dotnet/core/runtime-deps

# Installer image
FROM buildpack-deps:buster-curl as installer

# Retrieve .NET Core
RUN dotnet_version=5.0.0-preview.4.20209.4 \
    && curl -SL --output dotnet.tar.gz https://dotnetcli.azureedge.net/dotnet/Runtime/$dotnet_version/dotnet-runtime-$dotnet_version-linux-x64.tar.gz \
    && dotnet_sha512='bf958791d2c8fdee5e5f51b7bc3c2d10b239b4e7cb565ecd9eb8b4fc06da92e21ab6f358624499c809fb3fdd96e4125b4056f8f8ee6204cb2242178df22f7a6e' \
    && echo "$dotnet_sha512 dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /dotnet \
    && tar -ozxf dotnet.tar.gz -C /dotnet \
    && rm dotnet.tar.gz


# .NET Core runtime image
FROM $REPO:5.0-buster-slim

COPY --from=installer ["/dotnet", "/usr/share/dotnet"]

RUN ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
