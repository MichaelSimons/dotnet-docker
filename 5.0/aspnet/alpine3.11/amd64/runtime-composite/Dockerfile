ARG REPO=mcr.microsoft.com/dotnet/core/runtime
FROM $REPO:5.0-alpine3.11-composite

# Install ASP.NET Core
ENV ASPNET_VERSION 5.0.0-preview.4.20206.19

RUN wget -O aspnetcore.tar.gz https://dotnetcli.azureedge.net/dotnet/aspnetcore/Runtime/$ASPNET_VERSION/aspnetcore-runtime-$ASPNET_VERSION-linux-musl-x64.tar.gz \
    && aspnetcore_sha512='b0a9f7bca456fb5f469ccbfca4ce63ac477df84b73936568d789f62b9867e2a567dcf53faaf56cb0996578d4d1a81d1995fa076317089626a363c89ffbb85a2a' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && tar -ozxf aspnetcore.tar.gz -C /usr/share/dotnet ./shared/Microsoft.AspNetCore.App \
    && rm aspnetcore.tar.gz
