ARG REPO=mcr.microsoft.com/dotnet/core/runtime-deps

# Installer image
FROM arm64v8/buildpack-deps:buster-curl as installer

# Retrieve .NET Core
RUN dotnet_version=5.0.0-preview.4.20209.4 \
    && curl -SL --output dotnet.tar.gz https://dotnetcli.azureedge.net/dotnet/Runtime/$dotnet_version/dotnet-runtime-$dotnet_version-linux-arm64.tar.gz \
    && dotnet_sha512='96d43914d4ac3009496deefe4bf660e90eec033b3fe11660b5064e51ef761949b512f9e96cc2724bb2fbe2564d66993b52a9257b73747bf6e08623beb48b98c7' \
    && echo "$dotnet_sha512 dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /dotnet \
    && tar -ozxf dotnet.tar.gz -C /dotnet \
    && rm dotnet.tar.gz


# .NET Core runtime image
FROM $REPO:5.0-buster-slim-arm64v8

COPY --from=installer ["/dotnet", "/usr/share/dotnet"]

RUN ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
