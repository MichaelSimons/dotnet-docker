// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.DotNet.VersionTools.Dependencies;
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
    public class ProductVersionUpdater : FileRegexUpdater
    {
        public static string GetProductVersion(string productName, string dockerfileVersion, string variables)
        {
            string versionVariableName = $"{productName}-{dockerfileVersion}-version";
            string versionValueGroupName = "versionValue";
            Match match = Regex.Match(variables, $"\"{versionVariableName}\": \"(?<{versionValueGroupName}>[\\d]+.[\\d]+.[\\d]+(-[\\w]+(.[\\d]+)*)?)\"");
            return match.Groups[versionValueGroupName].Value;
        }
        private string _productName;

        public ProductVersionUpdater(string productName, string dockerfileVersion, string repoRoot) : base()
        {
            _productName = productName;
            string versionVariableName = $"{productName}-{dockerfileVersion}-version";
            string versionValueGroupName = "versionValue";

            Trace.TraceInformation($"Updating {versionVariableName}");

            Path = System.IO.Path.Combine(repoRoot, "manifest.versions.json");
            Regex = new Regex($"\"{versionVariableName}\": \"(?<{versionValueGroupName}>[\\d]+.[\\d]+.[\\d]+(-[\\w]+(.[\\d]+)*)?)\"");
            VersionGroupName = versionValueGroupName;
        }

        protected override string TryGetDesiredValue(
            IEnumerable<IDependencyInfo> dependencyBuildInfos, out IEnumerable<IDependencyInfo> usedBuildInfos)
        {
            IDependencyInfo productInfo = dependencyBuildInfos.First(info => info.SimpleName == _productName);

            usedBuildInfos = new IDependencyInfo[] { productInfo };

            return productInfo.SimpleVersion;
        }
    }
}
