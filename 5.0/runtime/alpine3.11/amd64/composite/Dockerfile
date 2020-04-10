# TODO: define a shared base stage (build infra doesn't support)
ARG REPO=mcr.microsoft.com/dotnet/core/runtime-deps


# Installer image
FROM $REPO:5.0-alpine3.11 as installer

ENV DOTNET_VERSION 5.0.0-preview.4.20209.4

# Install crossgen2
RUN wget -O crossgen2.tar.gz https://dotnetcli.azureedge.net/dotnet/Runtime/5.0.0-preview.4.20209.4/dotnet-crossgen2-5.0.0-preview.4.20209.4-linux-musl-x64.tar.gz \
    && crossgen2_sha512='6c2ca5d548cdf6c1afcd57ab1df4dcf6959b324294d031725c3fa848f760f9cb34db524c5be1f81a9c212170a8aebdec3a8570260ff9b312b9da8e3767bd4e61' \
    && echo "$crossgen2_sha512  crossgen2.tar.gz" | sha512sum -c - \
    && mkdir /crossgen2 \
    && tar -C /crossgen2 -oxzf crossgen2.tar.gz \
    && rm crossgen2.tar.gz


RUN \
    # Retrieve .NET
    wget -O dotnet.tar.gz https://dotnetcli.azureedge.net/dotnet/Runtime/$DOTNET_VERSION/dotnet-runtime-$DOTNET_VERSION-linux-musl-x64.tar.gz \
    && dotnet_sha512='844ea2c6b4df947fdd2ca1340cb8824834a363905e7ffaa7e8a674625878ef027630b123aae21f4f9c9e52d4acb1d0b158face3e8c52e0e4b95e7ce28e1386ed' \
    && echo "$dotnet_sha512  dotnet.tar.gz" | sha512sum -c - \
    && mkdir /dotnet \
    && tar -C /dotnet -oxzf dotnet.tar.gz \
    && rm dotnet.tar.gz \
    \
    # Crossgen .NET
    && /dotnet/dotnet /dotnet/crossgen2/crossgen2.dll \
        --composite --inputbubble -O --Os \
        -o /dotnet/shared/Microsoft.NETCore.App/$DOTNET_VERSION/Microsoft.NETCore.App.SharedFramework.dll \
        /dotnet/shared/Microsoft.NETCore.App/$DOTNET_VERSION/*.dll


# .NET runtime image
FROM $REPO:5.0-alpine3.11

ENV DOTNET_VERSION 5.0.0-preview.4.20209.4

COPY --from=installer ["/dotnet", "/usr/share/dotnet"]
RUN ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
