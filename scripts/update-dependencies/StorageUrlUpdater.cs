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
    /// An IDependencyUpdater that will update the urls used to download artifacts within the Dockerfiles.
    /// </summary>
    public class StorageUrlUpdater : FileRegexUpdater
    {
        private const string StorageGroupName = "storage";

        private string _replacement;

        public StorageUrlUpdater(string path, string source, string replacement) : base()
        {
            _replacement = replacement;

            Trace.TraceInformation($"StorageUrlUpdater is replacing {source} with {replacement} in {path}");

            Path = path;
            Regex = new Regex($"(?<{StorageGroupName}>{Regex.Escape(source)})");
            VersionGroupName = StorageGroupName;
        }

        protected override string TryGetDesiredValue(
            IEnumerable<IDependencyInfo> dependencyBuildInfos, out IEnumerable<IDependencyInfo> usedBuildInfos)
        {
            usedBuildInfos = Enumerable.Empty<IDependencyInfo>();

            return _replacement;
        }
    }
}
