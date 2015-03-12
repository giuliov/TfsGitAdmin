using System;
using System.Linq;

namespace TfsGitAdmin
{
    class RenameRepoCommand : GitAdminBaseCommand
    {
        internal string OldName { get; set; }
        internal string NewName { get; set; }

        public RenameRepoCommand()
        {
            this.IsCommand("rename", "Renames an existing Git repository");

            this.HasRequiredOption("o|old|oldName=", "Old name of Git Repository",
              value => this.OldName = value);
            this.HasRequiredOption("n|new|newName=", "New name of Git Repository",
              value => this.NewName = value);
        }

        public override int Run(string[] remainingArguments)
        {
            Console.WriteLine("Renaming Git repository '{3}' to '{4}' in {0}/{1}/{2}"
                , TfsUrl, TeamProjectCollection, TeamProject, OldName, NewName);

            Connect();

            var repos = gitSvc.QueryRepositories(TeamProject);

            var oldRepo = repos.Where(r => string.Compare(r.Name, OldName, true) == 0).FirstOrDefault();
            if (oldRepo == null)
            {
                Console.WriteLine("Error: Repository '{0}' not found.", OldName);
                return 1;
            }

            var renameRepoTask = gitWebApiClient.RenameRepositoryAsync(oldRepo, NewName);

            return WaitAsyncTask(renameRepoTask);
        }
    }
}
