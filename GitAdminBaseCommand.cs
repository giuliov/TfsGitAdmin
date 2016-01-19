using ManyConsole;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Git.Client;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;
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
        internal string Username { get; set; }
        internal string Password { get; set; }

        public GitAdminBaseCommand()
        {
            this.HasOption("no|nologo", "Hide initial banner",
              value => this.NoLogo = value != null);
            this.HasOption("h|help",  "Shows this message and exit",
              value => this.ShowHelp = value != null);
            this.HasRequiredOption("s|tfsUrl=",  "TFS Url, e.g. http://localhost:8080/tfs/",
              value => this.TfsUrl = value);
            this.HasRequiredOption("c|teamProjectCollection=",  "Team Project Collection",
              value => this.TeamProjectCollection = value);
            this.HasRequiredOption("p|teamProject=", "Source Project Name",
              value => this.TeamProject = value);
            this.HasOption("u|username=", "Username",
              value => this.Username = value);
            this.HasOption("w|password=", "Password",
              value => this.Password = value);
        }

        protected TfsTeamProjectCollection tpc;
        protected GitRepositoryService gitSvc;
        protected GitHttpClient gitWebApiClient;

        protected void Connect()
        {
            string tpcUrl = TfsUrl + "/" + TeamProjectCollection + "/";

            if (!string.IsNullOrWhiteSpace(this.Username))
            {
                Console.WriteLine("Authenticating as '{0}'", this.Username);

                // source http://blogs.msdn.com/b/buckh/archive/2013/01/07/how-to-connect-to-tf-service-without-a-prompt-for-liveid-credentials.aspx
                NetworkCredential netCred = new NetworkCredential(this.Username, this.Password);
                BasicAuthCredential basicCred = new BasicAuthCredential(netCred);
                TfsClientCredentials tfsCred = new TfsClientCredentials(basicCred);
                tfsCred.AllowInteractive = false;

                tpc = new TfsTeamProjectCollection(new Uri(tpcUrl), tfsCred);
            }
            else
            {
                tpc = new TfsTeamProjectCollection(new Uri(tpcUrl));
            }

            tpc.EnsureAuthenticated();

            gitSvc = tpc.GetService<GitRepositoryService>();

            if (!string.IsNullOrWhiteSpace(this.Username))
            {
                NetworkCredential netCred = new NetworkCredential(this.Username, this.Password);
                var cred = new VssCredentials(new Microsoft.VisualStudio.Services.Common.WindowsCredential(netCred));
                gitWebApiClient = new GitHttpClient(new Uri(tpcUrl), cred);
            }
            else
            {
                var cred = new VssCredentials(true);
                gitWebApiClient = new GitHttpClient(new Uri(tpcUrl), cred);
            }

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
