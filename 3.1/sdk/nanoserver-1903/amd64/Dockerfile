# escape=`

ARG REPO=mcr.microsoft.com/dotnet/core/aspnet

# Installer image
FROM mcr.microsoft.com/windows/servercore:1903 AS installer

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Retrieve .NET Core SDK
ENV DOTNET_SDK_VERSION 3.1.100-preview1-014459

RUN Invoke-WebRequest -OutFile dotnet.zip https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$Env:DOTNET_SDK_VERSION/dotnet-sdk-no-runtimes-$Env:DOTNET_SDK_VERSION-win-x64.zip; `
    $dotnet_sha512 = 'c2b2e5f856aeda280ce2d3dcf20d62530bb6e15b48762a35ffa6803701d68777c5841dbb9bc7cbab6f9806687223971e63be6d6bf88cd13f45a6cef3c38d41dd'; `
    if ((Get-FileHash dotnet.zip -Algorithm sha512).Hash -ne $dotnet_sha512) { `
        Write-Host 'CHECKSUM VERIFICATION FAILED!'; `
        exit 1; `
    }; `
    `
    Expand-Archive dotnet.zip -DestinationPath dotnet; `
    Remove-Item -Force dotnet.zip

# Retrieve WindowsDesktop Runtime
ENV DOTNET_VERSION 3.1.0-preview1.19506.1

RUN Invoke-WebRequest -OutFile desktop.zip https://dotnetcli.blob.core.windows.net/dotnet/WindowsDesktop/$Env:DOTNET_VERSION/dotnet-windows-desktop-runtime-no-core-$Env:DOTNET_VERSION-win-x64.zip; `
    $desktop_sha512 = '3652122944cba774130af8f878ab06a5b16a7bbe034156e14442b0b43a4344d087e8e19f69cf64cb8a3d4f9d3a3c07109746f39c4cd52fd74660b6119edc8517'; `
    if ((Get-FileHash desktop.zip -Algorithm sha512).Hash -ne $desktop_sha512) { `
        Write-Host 'CHECKSUM VERIFICATION FAILED!'; `
        exit 1; `
    }; `
    `
    Expand-Archive desktop.zip -DestinationPath dotnet/shared; `
    Remove-Item -Force desktop.zip

# Install PowerShell global tool
ENV POWERSHELL_VERSION=7.0.0-preview.4 `
    POWERSHELL_DISTRIBUTION_CHANNEL=PSDocker-DotnetCoreSDK-NanoServer-1903

RUN Invoke-WebRequest -OutFile PowerShell.Windows.x64.$ENV:POWERSHELL_VERSION.nupkg https://pwshtool.blob.core.windows.net/tool/$ENV:POWERSHELL_VERSION/PowerShell.Windows.x64.$ENV:POWERSHELL_VERSION.nupkg; `
    $powershell_sha512 = '1f0a57a210f2934fba68f710600c0b24a98dc0e2b291883b2f7cbb9801f59c831e0428267febe5b37b1fdb74af15a069ab28a95ab3d1760b6cf7026e9492b7d4'; `
    if ((Get-FileHash PowerShell.Windows.x64.$ENV:POWERSHELL_VERSION.nupkg -Algorithm sha512).Hash -ne $powershell_sha512) { `
        Write-Host 'CHECKSUM VERIFICATION FAILED!'; `
        exit 1; `
    }; `
    `
    \dotnet\dotnet tool install --add-source . --tool-path \powershell --version $ENV:POWERSHELL_VERSION PowerShell.Windows.x64; `
    Remove-Item -Force PowerShell.Windows.x64.$ENV:POWERSHELL_VERSION.nupkg; `
    Remove-Item -Path \powershell\.store\powershell.windows.x64\$ENV:POWERSHELL_VERSION\powershell.windows.x64\$ENV:POWERSHELL_VERSION\powershell.windows.x64.$ENV:POWERSHELL_VERSION.nupkg -Force

# SDK image
FROM $REPO:3.1-nanoserver-1903

# Add SDK and WindowsDescktop Runtime on top of base image
COPY --from=installer ["/dotnet", "/Program Files/dotnet"]

COPY --from=installer ["/powershell", "/Program Files/powershell"]

# In order to set system PATH, ContainerAdministrator must be used
USER ContainerAdministrator
RUN setx /M PATH "%PATH%;C:\Program Files\powershell"
USER ContainerUser

# Unset the value from the base image
ENV ASPNETCORE_URLS= `
    # Enable correct mode for dotnet watch (only mode supported in a container)
    DOTNET_USE_POLLING_FILE_WATCHER=true `
    # Skip extraction of XML docs - generally not useful within an image/container - helps performance
    NUGET_XMLDOC_MODE=skip

# Trigger first run experience by running arbitrary cmd
RUN dotnet help
