using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Linq;

namespace TfsGitAdmin
{
    class AddRepoCommand : GitAdminBaseCommand
    {
        internal string AdditionalRepoName { get; set; }

        public AddRepoCommand()
        {
            this.IsCommand("add", "Adds an additional Git repository");

            this.HasRequiredOption("r|additionalRepoName=", "Name of Git Repository to add",
              value => this.AdditionalRepoName = value);
        }

        public override int Run(string[] remainingArguments)
        {
            Console.WriteLine("Adding Git repository '{3}' to {0}/{1}/{2}", TfsUrl, TeamProjectCollection, TeamProject, AdditionalRepoName);

            Connect();

            var repos = gitSvc.QueryRepositories(TeamProject);
            if (repos.Any())
            {

                var defaultRepo = repos[0]; //HACK

                var newRepo = new GitRepository()
                {
                    Name = AdditionalRepoName,
                    ProjectReference = defaultRepo.ProjectReference
                };

                var createRepoTask = gitWebApiClient.CreateRepositoryAsync(newRepo);

                return WaitAsyncTask(createRepoTask);
            }
            else
            {
                Console.WriteLine("Error: Team Project '{0}' has no Git repositories.", TeamProject);
                return 1;
            }
        }
    }
}
