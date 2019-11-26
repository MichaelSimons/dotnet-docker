// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;
using static Microsoft.DotNet.Docker.Tests.ImageVersion;

namespace Microsoft.DotNet.Docker.Tests
{
    public abstract class ImageTests
    {
        private static readonly ImageData[] s_linuxTestData =
        {
            new ImageData { Version = V2_1, OS = OS.StretchSlim,  Arch = Arch.Amd64 },
            new ImageData { Version = V2_1, OS = OS.Bionic,       Arch = Arch.Amd64 },
            new ImageData { Version = V2_1, OS = OS.Alpine39,     Arch = Arch.Amd64 },
            new ImageData { Version = V2_1, OS = OS.Alpine310,    Arch = Arch.Amd64 },
            new ImageData { Version = V2_1, OS = OS.StretchSlim,  Arch = Arch.Arm },
            new ImageData { Version = V2_1, OS = OS.Bionic,       Arch = Arch.Arm },
            new ImageData { Version = V2_2, OS = OS.StretchSlim,  Arch = Arch.Amd64 },
            new ImageData { Version = V2_2, OS = OS.Bionic,       Arch = Arch.Amd64 },
            new ImageData { Version = V2_2, OS = OS.Alpine39,     Arch = Arch.Amd64 },
            new ImageData { Version = V2_2, OS = OS.Alpine310,    Arch = Arch.Amd64 },
            new ImageData { Version = V2_2, OS = OS.StretchSlim,  Arch = Arch.Arm },
            new ImageData { Version = V2_2, OS = OS.Bionic,       Arch = Arch.Arm },
            new ImageData { Version = V3_0, OS = OS.BusterSlim,   Arch = Arch.Amd64 },
            new ImageData { Version = V3_0, OS = OS.Disco,        Arch = Arch.Amd64 },
            new ImageData { Version = V3_0, OS = OS.Bionic,       Arch = Arch.Amd64 },
            new ImageData { Version = V3_0, OS = OS.Alpine39,     Arch = Arch.Amd64 },
            new ImageData { Version = V3_0, OS = OS.Alpine310,    Arch = Arch.Amd64 },
            new ImageData { Version = V3_0, OS = OS.BusterSlim,   Arch = Arch.Arm },
            new ImageData { Version = V3_0, OS = OS.Disco,        Arch = Arch.Arm },
            new ImageData { Version = V3_0, OS = OS.Bionic,       Arch = Arch.Arm },
            new ImageData { Version = V3_0, OS = OS.BusterSlim,   Arch = Arch.Arm64 },
            new ImageData { Version = V3_0, OS = OS.Disco,        Arch = Arch.Arm64 },
            new ImageData { Version = V3_0, OS = OS.Bionic,       Arch = Arch.Arm64 },
            new ImageData { Version = V3_0, OS = OS.Alpine39,     Arch = Arch.Arm64,    SdkOS = OS.Buster },
            new ImageData { Version = V3_0, OS = OS.Alpine310,    Arch = Arch.Arm64,    SdkOS = OS.Buster },
            new ImageData { Version = V3_1, OS = OS.BusterSlim,   Arch = Arch.Amd64 },
            new ImageData { Version = V3_1, OS = OS.Bionic,       Arch = Arch.Amd64 },
            new ImageData { Version = V3_1, OS = OS.Focal,        Arch = Arch.Amd64 },
            new ImageData { Version = V3_1, OS = OS.Alpine310,    Arch = Arch.Amd64 },
            new ImageData { Version = V3_1, OS = OS.BusterSlim,   Arch = Arch.Arm },
            new ImageData { Version = V3_1, OS = OS.Bionic,       Arch = Arch.Arm },
            new ImageData { Version = V3_1, OS = OS.Focal,        Arch = Arch.Arm },
            new ImageData { Version = V3_1, OS = OS.BusterSlim,   Arch = Arch.Arm64 },
            new ImageData { Version = V3_1, OS = OS.Bionic,       Arch = Arch.Arm64 },
            new ImageData { Version = V3_1, OS = OS.Focal,        Arch = Arch.Arm64 },
            new ImageData { Version = V3_1, OS = OS.Alpine310,    Arch = Arch.Arm64,    SdkOS = OS.Buster },
        };
        private static readonly ImageData[] s_windowsTestData =
        {
            new ImageData { Version = V2_1, OS = OS.NanoServer1809, Arch = Arch.Amd64 },
            new ImageData { Version = V2_1, OS = OS.NanoServer1903, Arch = Arch.Amd64 },
            new ImageData { Version = V2_1, OS = OS.NanoServer1909, Arch = Arch.Amd64 },
            new ImageData { Version = V2_2, OS = OS.NanoServer1809, Arch = Arch.Amd64 },
            new ImageData { Version = V2_2, OS = OS.NanoServer1809, Arch = Arch.Arm },
            new ImageData { Version = V2_2, OS = OS.NanoServer1903, Arch = Arch.Amd64 },
            new ImageData { Version = V2_2, OS = OS.NanoServer1909, Arch = Arch.Amd64 },
            new ImageData { Version = V3_0, OS = OS.NanoServer1809, Arch = Arch.Amd64 },
            new ImageData { Version = V3_0, OS = OS.NanoServer1809, Arch = Arch.Arm },
            new ImageData { Version = V3_0, OS = OS.NanoServer1903, Arch = Arch.Amd64 },
            new ImageData { Version = V3_0, OS = OS.NanoServer1909, Arch = Arch.Amd64 },
            new ImageData { Version = V3_1, OS = OS.NanoServer1809, Arch = Arch.Amd64 },
            new ImageData { Version = V3_1, OS = OS.NanoServer1809, Arch = Arch.Arm },
            new ImageData { Version = V3_1, OS = OS.NanoServer1903, Arch = Arch.Amd64 },
            new ImageData { Version = V3_1, OS = OS.NanoServer1909, Arch = Arch.Amd64 },
        };

        protected DockerHelper DockerHelper { get; private set; }
        protected abstract DotNetImageType ImageType { get; }
        protected ITestOutputHelper OutputHelper { get; private set; }

        protected ImageTests(ITestOutputHelper outputHelper)
        {
            DockerHelper = new DockerHelper(outputHelper);
            OutputHelper = outputHelper;
        }

        public static IEnumerable<object[]> GetImageData()
        {
            string archFilterPattern = GetFilterRegexPattern("IMAGE_ARCH_FILTER");
            string osFilterPattern = GetFilterRegexPattern("IMAGE_OS_FILTER");
            string versionFilterPattern = GetFilterRegexPattern("IMAGE_VERSION_FILTER");

            // Filter out test data that does not match the active architecture, os and version filters.
            return (DockerHelper.IsLinuxContainerModeEnabled ? s_linuxTestData : s_windowsTestData)
                .Where(imageData => archFilterPattern == null
                    || Regex.IsMatch(Enum.GetName(typeof(Arch), imageData.Arch), archFilterPattern, RegexOptions.IgnoreCase))
                .Where(imageData => osFilterPattern == null
                    || Regex.IsMatch(imageData.OS, osFilterPattern, RegexOptions.IgnoreCase))
                .Where(imageData => versionFilterPattern == null
                    || Regex.IsMatch(imageData.VersionString, versionFilterPattern, RegexOptions.IgnoreCase))
                .Select(imageData => new object[] { imageData });
        }

        private static string GetFilterRegexPattern(string filterEnvName)
        {
            string filter = Environment.GetEnvironmentVariable(filterEnvName);
            return filter != null ? $"^{Regex.Escape(filter).Replace(@"\*", ".*").Replace(@"\?", ".")}$" : null;
        }

        [Theory]
        [MemberData(nameof(GetImageData))]
        public void VerifyNoInsecureFiles(ImageData imageData)
        {
            if (imageData.Version < new Version("3.1") || !DockerHelper.IsLinuxContainerModeEnabled ||
                (imageData.OS.Contains("alpine") && imageData.IsArm))
            {
                return;
            }

            string worldWritableDirectoriesWithoutStickyBitCmd = @"find / -xdev -type d \( -perm -0002 -a ! -perm -1000 \)";
            string worldWritableFilesCmd = "find / -xdev -type f -perm -o+w";
            string noUserOrGroupFilesCmd;
            if (imageData.OS.Contains("alpine"))
            {
                // BusyBox in Alpine doesn't support the more convenient -nouser and -nogroup options for the find command
                noUserOrGroupFilesCmd = @"find / -xdev -exec stat -c %U-%n {} \+ | { grep ^UNKNOWN || true; }";
            }
            else
            {
                noUserOrGroupFilesCmd = @"find / -xdev \( -nouser -o -nogroup \)";
            }

            string command = $"/bin/sh -c \"{worldWritableDirectoriesWithoutStickyBitCmd} && {worldWritableFilesCmd} && {noUserOrGroupFilesCmd}\"";

            string output = DockerHelper.Run(
                image: imageData.GetImage(ImageType, DockerHelper),
                name: imageData.GetIdentifier($"InsecureFiles-{ImageType}"),
                command: command
            );

            Assert.Empty(output);
        }

        [Theory]
        [MemberData(nameof(GetImageData))]
        public void VerifyEnvironmentVariables(ImageData imageData)
        {
            EnvironmentVariableInfo.Validate(GetEnvironmentVariables(imageData), ImageType, imageData, DockerHelper);
        }

        protected abstract IEnumerable<EnvironmentVariableInfo> GetEnvironmentVariables(ImageData imageData);

        protected IEnumerable<EnvironmentVariableInfo> GetCommonEnvironmentVariables()
        {
            yield return new EnvironmentVariableInfo("DOTNET_RUNNING_IN_CONTAINER", "true");
        }

        protected IEnumerable<EnvironmentVariableInfo> GetRuntimeEnvironmentVariables(ImageData imageData)
        {
            List<EnvironmentVariableInfo> variables = new List<EnvironmentVariableInfo>();
            variables.AddRange(GetCommonEnvironmentVariables());
            variables.Add(new EnvironmentVariableInfo("ASPNETCORE_URLS", "http://+:80"));

            if (imageData.OS.StartsWith(Tests.OS.AlpinePrefix))
            {
                variables.Add(new EnvironmentVariableInfo("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "true"));
            }

            return variables;
        }
    }
}
