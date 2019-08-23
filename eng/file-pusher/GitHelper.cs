// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FilePusher.Models;
using Microsoft.DotNet.VersionTools.Automation;
using Microsoft.DotNet.VersionTools.Automation.GitHubApi;

namespace FilePusher
{
    public static class GitHelper
    {
        private const int DefaultMaxTries = 10;
        private const int DefaultRetryMillisecondsDelay = 5000;

        public static GitHubClient GetClient(Options options)
        {
            GitHubAuth githubAuth = new GitHubAuth(options.GitAuthToken, options.GitUser, options.GitEmail);
            return new GitHubClient(githubAuth);
        }

        public static async Task CreatePRAsync(GitHubClient client, GitRepo gitRepo, string commitMessage, Func<GitHubBranch, Task<IEnumerable<GitObject>>> getChanges)
        {
            GitHubProject project = new GitHubProject(gitRepo.Name, gitRepo.Owner);
            GitHubBranch branch = new GitHubBranch(gitRepo.Branch, project);

            IEnumerable<GitObject> changes = await getChanges(branch);

            if (!changes.Any())
            {
                return;
            }

            string masterRef = $"heads/{gitRepo.Branch}";
            GitReference currentMaster = await client.GetReferenceAsync(project, masterRef);
            string masterSha = currentMaster.Object.Sha;
            GitTree tree = await client.PostTreeAsync(project, masterSha, changes.ToArray());
            GitCommit commit = await client.PostCommitAsync(
                project, commitMessage, tree.Sha, new[] { masterSha });


            // TODO: Create PR instead of pushing...
            // GitHubAuth gitHubAuth = new GitHubAuth(Options.GitHubPassword, Options.GitHubUser, Options.GitHubEmail);
            PullRequestCreator prCreator = new PullRequestCreator(client.Auth, client.Auth.User);
            PullRequestOptions prOptions = new PullRequestOptions()
            {
                BranchNamingStrategy = new SingleBranchNamingStrategy($"UpdateDependencies-{gitRepo.Branch}")
            };

            await prCreator.CreateOrUpdateAsync(
                commitMessage,
                commitMessage,
                string.Empty,
                branch,
                project,
                prOptions);

            // Only fast-forward. Don't overwrite other changes: throw exception instead.
            //return await client.PatchReferenceAsync(project, masterRef, commit.Sha, force: false);
        }

        public static async Task ExecuteGitOperationsWithRetryAsync(Options options, Func<GitHubClient, Task> execute,
            int maxTries = DefaultMaxTries, int retryMillisecondsDelay = DefaultRetryMillisecondsDelay)
        {
            using (GitHubClient client = GitHelper.GetClient(options))
            {
                for (int i = 0; i < maxTries; i++)
                {
                    try
                    {
                        await execute(client);

                        break;
                    }
                    catch (HttpRequestException ex) when (i < (maxTries - 1))
                    {
                        Console.WriteLine($"Encountered exception interacting with GitHub: {ex.Message}");
                        Console.WriteLine($"Trying again in {retryMillisecondsDelay}ms. {maxTries - i - 1} tries left.");
                        await Task.Delay(retryMillisecondsDelay);
                    }
                }
            }
        }
    }
}
