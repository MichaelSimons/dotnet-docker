// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.DotNet.VersionTools.Dependencies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dotnet.Docker
{
    /// <summary>
    /// An IDependencyUpdater that will update the full versioned tags within the manifest to align with the
    /// current product version.
    /// </summary>
    public class ProductVersionTagUpdater : FileRegexUpdater
    {
        private string _productName;
        private readonly static string[] ExcludedMonikers = { "servicing", "rtm" };

        public ProductVersionTagUpdater(string productName, string dockerfileVersion, string repoRoot) : base()
        {
            _productName = productName;
            string versionVariableName = $"{productName}-{dockerfileVersion}-version-tag";
            string versionValueGroupName = "versionValue";

            Trace.TraceInformation($"Updating {versionVariableName}");

            Path = System.IO.Path.Combine(repoRoot, "versions.json");
            Regex = new Regex($"\"{versionVariableName}\": \"(?<{versionValueGroupName}>[\\d]+.[\\d]+.[\\d]+(-[\\w]+(.[\\d]+)*)?)\"");
            VersionGroupName = versionValueGroupName;
        }

        protected override string TryGetDesiredValue(
            IEnumerable<IDependencyInfo> dependencyBuildInfos, out IEnumerable<IDependencyInfo> usedBuildInfos)
        {
            IDependencyInfo productInfo = dependencyBuildInfos.First(info => info.SimpleName == _productName);

            usedBuildInfos = new IDependencyInfo[] { productInfo };

            // Derive the Docker tag version from the product build version.
            // 5.0.0-preview.2.19530.9 => 5.0.0-preview.2
            string versionRegexPattern = "[\\d]+.[\\d]+.[\\d]+(-[\\w]+(.[\\d]+)?)?";
            Match versionMatch = Regex.Match(productInfo.SimpleVersion, versionRegexPattern);
            string tagVersion = versionMatch.Success ? versionMatch.Value : productInfo.SimpleVersion;

            foreach (string excludedMoniker in ExcludedMonikers)
            {
                int monikerIndex = tagVersion.IndexOf($"-{excludedMoniker}", StringComparison.OrdinalIgnoreCase);
                if (monikerIndex != -1)
                {
                    tagVersion = tagVersion.Substring(0, monikerIndex);
                }
            }

            return tagVersion;
        }
    }
}
