﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.DotNet.Docker.Tests
{
    public class AspnetImageTests : CommonRuntimeImageTests
    {
        protected override DotNetImageType ImageType => DotNetImageType.Aspnet;

        public AspnetImageTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Theory]
        [MemberData(nameof(GetImageData))]
        public async Task VerifyAppScenario(ImageData imageData)
        {
            ImageScenarioVerifier verifier = new ImageScenarioVerifier(imageData, DockerHelper, OutputHelper, isWeb: true);
            await verifier.Execute();
        }
    }
}
