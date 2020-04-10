# TODO: define a shared base stage (build infra doesn't support)
ARG REPO=mcr.microsoft.com/dotnet/core/runtime-deps


# Installer image
FROM $REPO:5.0-alpine3.11 as installer

ENV DOTNET_VERSION 5.0.0-preview.4.20209.4

# Install crossgen2
RUN wget -O crossgen2.tar.gz https://dotnetcli.azureedge.net/dotnet/Runtime/$DOTNET_VERSION/dotnet-crossgen2-$DOTNET_VERSION-linux-musl-x64.tar.gz \
    && crossgen2_sha512='6c2ca5d548cdf6c1afcd57ab1df4dcf6959b324294d031725c3fa848f760f9cb34db524c5be1f81a9c212170a8aebdec3a8570260ff9b312b9da8e3767bd4e61' \
    && echo "$crossgen2_sha512  crossgen2.tar.gz" | sha512sum -c - \
    && mkdir /crossgen2 \
    && tar -C /crossgen2 -oxzf crossgen2.tar.gz \
    && rm crossgen2.tar.gz

ENV ASPNET_VERSION 5.0.0-preview.4.20206.19

RUN \
    # Retrieve .NET and ASP.NET
    wget -O aspnetcore.tar.gz https://dotnetcli.azureedge.net/dotnet/aspnetcore/Runtime/$ASPNET_VERSION/aspnetcore-runtime-$ASPNET_VERSION-linux-musl-x64.tar.gz \
    && aspnetcore_sha512='029ed32f8a7dfdb258438177b7fd40c187830db35b3d973dec296d8948f99d30a74269c45d31876c9b4533b8d7b6af65dea319299bd9a7081c6881cf026274b4' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && mkdir /dotnet \
    && tar -C /dotnet -ozxf aspnetcore.tar.gz \
    && rm aspnetcore.tar.gz \
    \
    # Crossgen .NET Core
    && mkdir -p /work/input \
    && cp /dotnet/shared/Microsoft.NETCore.App/$DOTNET_VERSION/*.dll /work/input \
    && cp /dotnet/shared/Microsoft.AspNetCore.App/$ASPNET_VERSION/*.dll /work/input \
    && mkdir -p /work/output \
    && /dotnet/dotnet /dotnet/crossgen2/crossgen2.dll \
        --composite --inputbubble -O --Os \
        -o /work/output/Microsoft.AspNetCore.App.SharedFramework.dll \
        /work/input/*.dll \
    && for b in /dotnet/shared/Microsoft.NETCore.App/$DOTNET_VERSION/*.dll; \
        do \
            a=/work/output/`basename ${b}`; \
            test -e ${a} && cp ${a} /dotnet/shared/Microsoft.NETCore.App/$DOTNET_VERSION; \
        done \
    && for b in /dotnet/shared/Microsoft.AspNetCore.App/$ASPNET_VERSION/*.dll; \
        do \
            a=/work/output/`basename ${b}`; \
            test -e ${a} && cp ${a} /dotnet/shared/Microsoft.AspNetCore.App/$ASPNET_VERSION; \
        done \
    && cp /work/output/Microsoft.AspNetCore.App.SharedFramework.dll /dotnet/shared/Microsoft.AspNetCore.App/$ASPNET_VERSION \
    && rm -r /work


# ASP.NET image
FROM $REPO:5.0-alpine3.11

ENV DOTNET_VERSION=5.0.0-preview.4.20209.4
    ASPNET_VERSION=5.0.0-preview.4.20206.19

COPY --from=installer ["/dotnet", "/usr/share/dotnet"]
RUN ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
