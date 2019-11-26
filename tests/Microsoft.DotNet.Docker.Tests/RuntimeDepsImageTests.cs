// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.DotNet.Docker.Tests
{
    public class RuntimeDepsImageTests : ImageTests
    {
        protected override DotNetImageType ImageType => DotNetImageType.Runtime_Deps;

        public RuntimeDepsImageTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        protected override IEnumerable<EnvironmentVariableInfo> GetEnvironmentVariables(ImageData imageData) => 
            GetRuntimeEnvironmentVariables(imageData);
    }

public class LinuxImageTheoryAttribute : TheoryAttribute
{
    public LinuxImageTheoryAttribute()
    {
        if (!DockerHelper.IsLinuxContainerModeEnabled)
        {
            Skip = "Running in Windows Container mode";
        }
    }
}
}