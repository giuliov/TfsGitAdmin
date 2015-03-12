using System;
using System.Linq;

namespace TfsGitAdmin
{
    class ListReposCommand : GitAdminBaseCommand
    {
        public ListReposCommand()
        {
            this.IsCommand("list", "List all Git repositories in a Team Project");
        }

        public override int Run(string[] remainingArguments)
        {
            Connect();

            var repos = gitSvc.QueryRepositories(TeamProject);
            if (repos.Any())
            {
                // nice formatting code
                const int maxColWidth = 20;
                const string col1Title = "Name";
                const string col2Title = "RemoteUrl";
                int longest = repos.Max(r => r.Name.Length);
                int colWidth = Math.Min(Math.Max(col1Title.Length, longest), maxColWidth);

                // title
                Console.WriteLine("{0,-" + maxColWidth.ToString() + "} {1}", col1Title, col2Title);
                Console.WriteLine(string.Empty.PadRight(maxColWidth + col2Title.Length + 1, '-'));
                // rows
                foreach (var repo in repos)
                {
                    Console.WriteLine("{0,-" + maxColWidth.ToString() + "} {1}", repo.Name, repo.RemoteUrl);
                }//for
            }
            else
            {
                Console.WriteLine("There are no Git Repositories in '{0}'", this.TeamProject);
            }//if
            return 0;
        }
    }
}
