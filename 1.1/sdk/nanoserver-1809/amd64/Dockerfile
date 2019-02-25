# escape=`

# Installer image
FROM mcr.microsoft.com/windows/servercore:1809 AS installer

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Retrieve .NET Core SDK
ENV DOTNET_SDK_VERSION 1.1.12

RUN Invoke-WebRequest -OutFile dotnet.zip https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$Env:DOTNET_SDK_VERSION/dotnet-dev-win-x64.$Env:DOTNET_SDK_VERSION.zip; `
    $dotnet_sha512 = 'a7a19bba1b48f47e24da5daa3e73c9a3279613813843fa3513c01f4d1bb6026699df131cc624ee3fae49775282016e0dff62e3f62e670fe843ff6e66a3387b20'; `
    Expand-Archive dotnet.zip -DestinationPath dotnet; `
    Remove-Item -Force dotnet.zip


# SDK image
FROM mcr.microsoft.com/windows/nanoserver:1809

COPY --from=installer ["/dotnet", "/Program Files/dotnet"]

# In order to set system PATH, ContainerAdministrator must be used
USER ContainerAdministrator 
RUN setx /M PATH "%PATH%;C:\Program Files\dotnet"
USER ContainerUser

# Trigger first run experience by running arbitrary cmd to populate local package cache
RUN dotnet help
