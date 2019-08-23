// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.CommandLine;

namespace FilePusher
{
    public class Options
    {
        public string GitEmail { get; private set; }
        public string GitAuthToken { get; private set; }
        public string GitUser { get; private set; }
        public string SubscriptionsPath { get; private set; }

        public void Parse(string[] args)
        {
            ArgumentSyntax argSyntax = ArgumentSyntax.Parse(args, syntax =>
            {
                string subscriptionsPath = null;
                syntax.DefineParameter(
                    "subscriptions",
                    ref subscriptionsPath,
                    "Path to the subscritions json file (defaults to subscriptions.json)");
                SubscriptionsPath = subscriptionsPath;

                string gitUser = null;
                syntax.DefineParameter(
                    "git-user",
                    ref gitUser,
                    "GitHub user used to make PR");
                GitUser = gitUser;

                string gitEmail = null;
                syntax.DefineParameter(
                    "git-email",
                    ref gitEmail,
                    "GitHub email used to make PR");
                GitEmail = gitEmail;

                string gitAuthToken = null;
                syntax.DefineParameter(
                    "git-auth-token",
                    ref gitAuthToken,
                    "GitHub authorization token used to make PR");
                GitAuthToken = gitAuthToken;
            });

            // Workaround for https://github.com/dotnet/corefxlab/issues/1689
            foreach (Argument arg in argSyntax.GetActiveArguments())
            {
                if (arg.IsParameter && !arg.IsSpecified)
                {
                    Console.WriteLine($"error: `{arg.Name}` must be specified.");
                    Environment.Exit(1);
                }
            }
        }
    }
}
