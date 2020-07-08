// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.DotNet.VersionTools.Dependencies;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Dotnet.Docker
{
    /// <summary>
    /// An IDependencyUpdater that regenerate the Dockerfiles based on the latest templates and variables.
    /// </summary>
    public class DockerfileUpdater : IDependencyUpdater
    {
        private string _repoRoot;

        public DockerfileUpdater(string repoRoot)
        {
            _repoRoot = repoRoot;
        }

        public IEnumerable<DependencyUpdateTask> GetUpdateTasks(IEnumerable<IDependencyInfo> dependencyInfos)
        {
            return new DependencyUpdateTask[] {
                new DependencyUpdateTask(
                    () => GenerateDockerfiles(),
                    Enumerable.Empty<IDependencyInfo>(),
                    Enumerable.Empty<string>()
                )
            };
        }

        private void GenerateDockerfiles()
        {
            Trace.TraceInformation($"GenerateDockerfiles");

            // Support both execution within Windows 10, Nano Server and Linux environments.
            string scriptPath = Path.Combine(_repoRoot, "eng", "dockerfile-templates", "Get-GeneratedDockerfiles.ps1");
            try
            {
                Process process = Process.Start("pwsh", scriptPath);
                process.WaitForExit();
            }
            catch (Win32Exception)
            {
                Process process = Process.Start("powershell", scriptPath);
                process.WaitForExit();
            }
        }
    }
}
