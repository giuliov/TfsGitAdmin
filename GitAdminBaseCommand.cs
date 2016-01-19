using ManyConsole;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Git.Client;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Threading.Tasks;

namespace TfsGitAdmin
{
    abstract class GitAdminBaseCommand : ConsoleCommand
    {
        internal bool ShowHelp { get; set; }
        internal bool NoLogo { get; set; }

        internal string TfsUrl { get; set; }
        internal string TeamProjectCollection { get; set; }
        internal string TeamProject { get; set; }

        public GitAdminBaseCommand()
        {
            this.HasOption("no|nologo", "Hide initial banner",
              value => this.NoLogo = value != null);
            this.HasOption("h|help",  "Shows this message and exit",
              value => this.ShowHelp = value != null);
            this.HasRequiredOption("s|tfsUrl=",  "TFS Url, e.g. http://localhost:8080/tfs/",
              value => this.TfsUrl = value);
            this.HasRequiredOption("c|teamProjectCollection=",  "Password for Source user",
              value => this.TeamProjectCollection = value);
            this.HasRequiredOption("p|teamProject=",  "Source Project Name",
              value => this.TeamProject = value);
        }

        protected TfsTeamProjectCollection tpc;
        protected GitRepositoryService gitSvc;
        protected GitHttpClient gitWebApiClient;

        protected void Connect()
        {
            string tpcUrl = TfsUrl + "/" + TeamProjectCollection + "/";
            tpc = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(tpcUrl));

            tpc.EnsureAuthenticated();

            gitSvc = tpc.GetService<GitRepositoryService>();

            var cred = new VssCredentials(true);
            gitWebApiClient = new GitHttpClient(new Uri(tpcUrl), cred);
        }

        protected int WaitAsyncTask(Task task)
        {
            task.Wait();
            if (task.IsFaulted)
            {
                task.Exception.Dump(Console.Out);
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
