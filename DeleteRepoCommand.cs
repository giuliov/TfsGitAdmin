using System;
using System.Linq;

namespace TfsGitAdmin
{
    class DeleteRepoCommand : GitAdminBaseCommand
    {
        internal string RepoName { get; set; }

        public DeleteRepoCommand()
        {
            this.IsCommand("delete", "Deletes an existing Git repository");

            this.HasRequiredOption("r|repoName=", "Name of Git Repository to delete",
              value => this.RepoName = value);
        }

        public override int Run(string[] remainingArguments)
        {
            Console.WriteLine("Deleting Git repository '{3}' from {0}/{1}/{2}", TfsUrl, TeamProjectCollection, TeamProject, RepoName);

            Connect();

            var repos = gitSvc.QueryRepositories(TeamProject);

            var repoToDelete = repos.Where(r => string.Compare(r.Name, RepoName, true) == 0).FirstOrDefault();
            if (repoToDelete == null)
            {
                Console.WriteLine("Error: Repository '{0}' not found.", RepoName);
                return 1;
            }

            var deleteRepoTask = gitWebApiClient.DeleteRepositoryAsync(repoToDelete.Id);

            return WaitAsyncTask(deleteRepoTask);
        }
    }
}
