// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FilePusher.Models;
using Microsoft.DotNet.VersionTools.Automation;
using Microsoft.DotNet.VersionTools.Automation.GitHubApi;
using Newtonsoft.Json;

namespace FilePusher
{
    public class Program
    {
        private static Options Options { get; } = new Options();

        public static async Task Main(string[] args)
        {
            // TODO:  Add Dockerfile to build
            // TODO:  Add support for filtering subscriptions

            // Hookup a TraceListener to capture details from Microsoft.DotNet.VersionTools
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            Options.Parse(args);

            string subscriptionsJson = File.ReadAllText(Options.SubscriptionsPath);
            Subscription[] subscriptions = JsonConvert.DeserializeObject<Subscription[]>(subscriptionsJson);

            foreach (Subscription subscription in subscriptions)
            {
                Console.WriteLine($"Processing {subscription.RepoInfo.Name}/{subscription.RepoInfo.Branch}");

                await GitHelper.ExecuteGitOperationsWithRetryAsync(Options, async client =>
                {
                    await GitHelper.CreatePRAsync(client, subscription.RepoInfo, $"Updating {subscription.sourcePath}", async branch =>
                    {
                        return await GetUpdatedFiles(subscription.sourcePath, client, branch);
                    });
                });
            }
        }

        private async static Task AddUpdatedFile(
            List<GitObject> updatedFiles,
            GitHubClient client,
            GitHubBranch branch,
            string filePath,
            string updatedContent)
        {
            string currentContent = await client.GetGitHubFileContentsAsync(filePath, branch);
            
            // TODO:  Handle File Deletes
            if (currentContent == updatedContent)
            {
                Console.WriteLine($"File '{filePath}' has not changed.");
            }
            else
            {
                Console.WriteLine($"File '{filePath}' has changed.");
                // TODO:  File path is not the git file path \ instead of /
                updatedFiles.Add(new GitObject
                {
                    Path = filePath,
                    Type = GitObject.TypeBlob,
                    Mode = GitObject.ModeFile,
                    Content = updatedContent
                });
            }
        }

        private static IEnumerable<string> GetFiles(string targetDirectory) =>
            Directory.GetDirectories(targetDirectory)
                .SelectMany(dir => GetFiles(dir))
                .Concat(Directory.GetFiles(targetDirectory));

        private async static Task<GitObject[]> GetUpdatedFiles(string sourcePath, GitHubClient client, GitHubBranch branch)
        {
            List<GitObject> updatedFiles = new List<GitObject>();

            foreach (string file in GetFiles(sourcePath))
            {
                await AddUpdatedFile(updatedFiles, client, branch, file, File.ReadAllText(file));
            }

            return updatedFiles.ToArray();
        }
    }
}
