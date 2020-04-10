# TODO: define a shared base stage (build infra doesn't support)
ARG REPO=mcr.microsoft.com/dotnet/core/runtime


# Installer image
FROM $REPO:5.0-alpine3.11-composite as installer

# Install crossgen2
RUN wget -O crossgen2.tar.gz https://dotnetcli.azureedge.net/dotnet/Runtime/$DOTNET_VERSION/dotnet-crossgen2-$DOTNET_VERSION-linux-musl-x64.tar.gz \
    && crossgen2_sha512='6c2ca5d548cdf6c1afcd57ab1df4dcf6959b324294d031725c3fa848f760f9cb34db524c5be1f81a9c212170a8aebdec3a8570260ff9b312b9da8e3767bd4e61' \
    && echo "$crossgen2_sha512  crossgen2.tar.gz" | sha512sum -c - \
    && mkdir /crossgen2 \
    && tar -C /crossgen2 -oxzf crossgen2.tar.gz \
    && rm crossgen2.tar.gz

ENV ASPNET_VERSION 5.0.0-preview.4.20206.19

RUN \
    # Retrieve ASP.NET
    wget -O aspnetcore.tar.gz https://dotnetcli.azureedge.net/dotnet/aspnetcore/Runtime/$ASPNET_VERSION/aspnetcore-runtime-$ASPNET_VERSION-linux-musl-x64.tar.gz \
    &&aspnetcore_sha512='029ed32f8a7dfdb258438177b7fd40c187830db35b3d973dec296d8948f99d30a74269c45d31876c9b4533b8d7b6af65dea319299bd9a7081c6881cf026274b4' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && tar -ozxf aspnetcore.tar.gz -C /usr/share/dotnet ./shared/Microsoft.AspNetCore.App \
    && rm aspnetcore.tar.gz \
    \
    # Crossgen .NET Core
    && dotnet /crossgen2/crossgen2.dll \
        --composite --inputbubble -O --Os \
        -r /usr/share/dotnet/shared/Microsoft.NETCore.App/$DOTNET_VERSION/*.dll \
        -o /usr/share/dotnet/shared/Microsoft.AspNetCore.App/$ASPNET_VERSION/Microsoft.AspNetCore.App.SharedFramework.dll \
        /usr/share/dotnet/shared/Microsoft.AspNetCore.App/$ASPNET_VERSION/*.dll


# ASP.NET Core image
FROM $REPO:5.0-alpine3.11-composite

ENV ASPNET_VERSION 5.0.0-preview.4.20206.19

COPY --from=installer ["/usr/share/dotnet/shared/Microsoft.AspNetCore.App", "/usr/share/dotnet/shared/Microsoft.AspNetCore.App"]
