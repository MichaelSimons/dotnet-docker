// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.DotNet.Docker.Tests
{
    public class RuntimeDepsImageTests : CommonRuntimeImageTests
    {
        protected override DotNetImageType ImageType => DotNetImageType.Runtime_Deps;

        public RuntimeDepsImageTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }
    }
}
