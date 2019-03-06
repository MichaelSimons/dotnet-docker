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
    /// An IDependencyUpdater that will append a sasToken to the urls used to download artifacts within the Dockerfiles.
    /// </summary>
    public class SasTokenUpdater : FileRegexUpdater
    {
        private const string SasTokenGroupName = "sasToken";

        private string _sasToken;

        public SasTokenUpdater(string path, string sasToken) : base()
        {
            _sasToken = sasToken;

            Trace.TraceInformation($"SasTokenUpdater is appending {_sasToken} download url in {path}");

            Path = path;
            Regex = new Regex($"https://{Program.ProductionStorageAccount}/[^;\\s]*(?<{SasTokenGroupName}>)[ |\\n|\\r|\\r\\n]");
            VersionGroupName = SasTokenGroupName;
        }

        protected override string TryGetDesiredValue(
            IEnumerable<IDependencyInfo> dependencyBuildInfos, out IEnumerable<IDependencyInfo> usedBuildInfos)
        {
            usedBuildInfos = Enumerable.Empty<IDependencyInfo>();

            return _sasToken;
        }
    }
}
