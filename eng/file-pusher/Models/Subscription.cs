// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace FilePusher.Models
{
    public class Subscription
    {
        [JsonProperty(Required = Required.Always)]
        public GitRepo RepoInfo { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string sourcePath { get; set; }
    }
}
