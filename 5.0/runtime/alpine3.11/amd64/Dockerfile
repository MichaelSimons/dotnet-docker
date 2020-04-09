ARG REPO=mcr.microsoft.com/dotnet/core/runtime-deps
FROM $REPO:5.0-alpine3.11

# Install .NET Core
ENV DOTNET_VERSION 5.0.0-preview.4.20209.4

RUN wget -O dotnet.tar.gz https://dotnetcli.azureedge.net/dotnet/Runtime/$DOTNET_VERSION/dotnet-runtime-$DOTNET_VERSION-linux-musl-x64.tar.gz \
    && dotnet_sha512='844ea2c6b4df947fdd2ca1340cb8824834a363905e7ffaa7e8a674625878ef027630b123aae21f4f9c9e52d4acb1d0b158face3e8c52e0e4b95e7ce28e1386ed' \
    && echo "$dotnet_sha512  dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -C /usr/share/dotnet -oxzf dotnet.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet \
    && rm dotnet.tar.gz
