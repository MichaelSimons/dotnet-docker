// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.DotNet.Docker.Tests
{
    public class SdkImageTests : ImageTests
    {
        protected override DotNetImageType ImageType => DotNetImageType.SDK;

        public SdkImageTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Theory]
        [MemberData(nameof(GetImageData))]
        public void VerifyPackageCache(ImageData imageData)
        {
            string verifyCacheCommand = null;
            if (imageData.Version.Major == 2)
            {
                if (DockerHelper.IsLinuxContainerModeEnabled)
                {
                    verifyCacheCommand = "test -d /usr/share/dotnet/sdk/NuGetFallbackFolder";
                }
                else
                {
                    verifyCacheCommand = "CMD /S /C PUSHD \"C:\\Program Files\\dotnet\\sdk\\NuGetFallbackFolder\"";
                }
            }
            else
            {
                OutputHelper.WriteLine(".NET Core SDK images >= 3.0 don't include a package cache.");
            }

            if (verifyCacheCommand != null)
            {
                // Simple check to verify the NuGet package cache was created
                DockerHelper.Run(
                    image: imageData.GetImage(DotNetImageType.SDK, DockerHelper),
                    command: verifyCacheCommand,
                    name: imageData.GetIdentifier("PackageCache"));
            }
        }

        [Theory]
        [MemberData(nameof(GetImageData))]
        public void VerifyPowerShellScenario_DefaultUser(ImageData imageData)
        {
            ExecutePowerShellScenario(imageData, null);
        }

        [Theory]
        [MemberData(nameof(GetImageData))]
        public void VerifyPowerShellScenario_NonDefaultUser(ImageData imageData)
        {
            var optRunArgs = "-u 12345:12345"; // Linux containers test as non-root user
            if (imageData.OS.Contains("nanoserver", StringComparison.OrdinalIgnoreCase))
            {
                // windows containers test as Admin, default execution is as ContainerUser
                optRunArgs = "-u ContainerAdministrator ";
            }

            ExecutePowerShellScenario(imageData, optRunArgs);
        }

        private void ExecutePowerShellScenario(ImageData imageData, string optionalArgs)
        {
            if (imageData.Version.Major < 3)
            {
                OutputHelper.WriteLine("PowerShell does not exist in pre-3.0 images, skip testing");
                return;
            }

            // A basic test which executes an arbitrary command to validate PS is functional
            string output = DockerHelper.Run(
                image: imageData.GetImage(DotNetImageType.SDK, DockerHelper),
                name: imageData.GetIdentifier($"pwsh"),
                optionalRunArgs: optionalArgs,
                command: $"pwsh -c (Get-Childitem env:DOTNET_RUNNING_IN_CONTAINER).Value"
            );

            Assert.Equal(output, bool.TrueString, ignoreCase: true);
        }

        protected override IEnumerable<EnvironmentVariableInfo> GetEnvironmentVariables(ImageData imageData)
        {
            List<EnvironmentVariableInfo> variables = new List<EnvironmentVariableInfo>();
            variables.AddRange(GetCommonEnvironmentVariables());

            string aspnetUrlsValue = imageData.Version.Major < 3 ? "http://+:80" : string.Empty;
            variables.Add(new EnvironmentVariableInfo("ASPNETCORE_URLS", aspnetUrlsValue));
            variables.Add(new EnvironmentVariableInfo("DOTNET_USE_POLLING_FILE_WATCHER", "true"));
            variables.Add(new EnvironmentVariableInfo("NUGET_XMLDOC_MODE", "skip"));

            if (imageData.Version.Major >= 3
                && (DockerHelper.IsLinuxContainerModeEnabled || imageData.Version >= new Version("3.1")))
            {
                variables.Add(new EnvironmentVariableInfo("POWERSHELL_DISTRIBUTION_CHANNEL", allowAnyValue: true));
            }

            if (imageData.SdkOS.StartsWith(Tests.OS.AlpinePrefix))
            {
                variables.Add(new EnvironmentVariableInfo("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "false"));
                variables.Add(new EnvironmentVariableInfo("LC_ALL", "en_US.UTF-8"));
                variables.Add(new EnvironmentVariableInfo("LANG", "en_US.UTF-8"));
            }

            return variables;
        }
    }
}