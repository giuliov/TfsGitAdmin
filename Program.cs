using System;

namespace TfsGitAdmin
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("TfsGitAdmin 0.1 (c) Giulio Vian");
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: TfsGitAdmin tfsUrl teamProjectCollection teamProject [additionalRepoName]");
                Console.WriteLine("Example: TfsGitAdmin http://localhost:8080/tfs/ DefaultCollection MyProject");
                return 1;
            }

            try
            {
                string tfsUrl = args[0];
                string teamProjectCollection = args[1];
                string teamProject = args[2];
                if (args.Length == 4)
                {
                    string additionalRepoName = args[3];
                    Console.WriteLine("Adding Git repository '{3}' to {0}/{1}/{2}", tfsUrl, teamProjectCollection, teamProject, additionalRepoName);
                    AddGitRepo(tfsUrl, teamProjectCollection, teamProject, additionalRepoName);
                }
                else
                {
                    Console.WriteLine("Creating default Git repository in {0}/{1}/{2}", tfsUrl, teamProjectCollection, teamProject);
                    CreateDefaultGitRepo(tfsUrl, teamProjectCollection, teamProject);
                }
                Console.WriteLine("Succeeded.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return -1;
            }
            return 0;
        }

        private static void CreateDefaultGitRepo(string tfsUrl, string teamProjectCollection, string teamProject)
        {
            string tpcUrl = tfsUrl + "/" + teamProjectCollection + "/";
            var tpc = Microsoft.TeamFoundation.Client.TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(tpcUrl));

            tpc.EnsureAuthenticated();

            var identityManagementService = tpc.GetService<Microsoft.TeamFoundation.Framework.Client.IIdentityManagementService>();

            //collection default groups
            var identityProjectCollectionAdministrators = identityManagementService.ReadIdentity(
                    Microsoft.TeamFoundation.Framework.Common.IdentitySearchFactor.DisplayName,
                    string.Format(@"[{0}]\Project Collection Administrators", teamProjectCollection),
                    Microsoft.TeamFoundation.Framework.Common.MembershipQuery.None,
                    Microsoft.TeamFoundation.Framework.Common.ReadIdentityOptions.None);
            var identityProjectCollectionBuildServiceAccounts = identityManagementService.ReadIdentity(
                    Microsoft.TeamFoundation.Framework.Common.IdentitySearchFactor.DisplayName,
                    string.Format(@"[{0}]\Project Collection Build Service Accounts", teamProjectCollection),
                    Microsoft.TeamFoundation.Framework.Common.MembershipQuery.None,
                    Microsoft.TeamFoundation.Framework.Common.ReadIdentityOptions.None);
            var identityProjectCollectionServiceAccounts = identityManagementService.ReadIdentity(
                    Microsoft.TeamFoundation.Framework.Common.IdentitySearchFactor.DisplayName,
                    string.Format(@"[{0}]\Project Collection Service Accounts", teamProjectCollection),
                    Microsoft.TeamFoundation.Framework.Common.MembershipQuery.None,
                    Microsoft.TeamFoundation.Framework.Common.ReadIdentityOptions.None);

            //project default groups
            var identityBuildAdministrators = identityManagementService.ReadIdentity(
                    Microsoft.TeamFoundation.Framework.Common.IdentitySearchFactor.DisplayName,
                    string.Format(@"[{0}]\Build Administrators", teamProject),
                    Microsoft.TeamFoundation.Framework.Common.MembershipQuery.None,
                    Microsoft.TeamFoundation.Framework.Common.ReadIdentityOptions.None);
            var identityContributors = identityManagementService.ReadIdentity(
                    Microsoft.TeamFoundation.Framework.Common.IdentitySearchFactor.DisplayName,
                    string.Format(@"[{0}]\Contributors", teamProject),
                    Microsoft.TeamFoundation.Framework.Common.MembershipQuery.None,
                    Microsoft.TeamFoundation.Framework.Common.ReadIdentityOptions.None);
            var identityProjectAdministrators = identityManagementService.ReadIdentity(
                    Microsoft.TeamFoundation.Framework.Common.IdentitySearchFactor.DisplayName,
                    string.Format(@"[{0}]\Project Administrators", teamProject),
                    Microsoft.TeamFoundation.Framework.Common.MembershipQuery.None,
                    Microsoft.TeamFoundation.Framework.Common.ReadIdentityOptions.None);
            var identityReaders = identityManagementService.ReadIdentity(
                    Microsoft.TeamFoundation.Framework.Common.IdentitySearchFactor.DisplayName,
                    string.Format(@"[{0}]\Readers", teamProject),
                    Microsoft.TeamFoundation.Framework.Common.MembershipQuery.None,
                    Microsoft.TeamFoundation.Framework.Common.ReadIdentityOptions.None);

            var aces = new Microsoft.TeamFoundation.Framework.Client.AccessControlEntry[] {
                new Microsoft.TeamFoundation.Framework.Client.AccessControlEntry(
                    identityProjectCollectionAdministrators.Descriptor,
                    Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.All, 0),
                new Microsoft.TeamFoundation.Framework.Client.AccessControlEntry(
                    identityProjectCollectionBuildServiceAccounts.Descriptor,
                    Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.GenericRead, 0),
                new Microsoft.TeamFoundation.Framework.Client.AccessControlEntry(
                    identityProjectCollectionServiceAccounts.Descriptor,
                    Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.All, 0),
                new Microsoft.TeamFoundation.Framework.Client.AccessControlEntry(
                    identityBuildAdministrators.Descriptor,
                    Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.CreateBranch
                    | Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.GenericContribute
                    | Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.ManageNote
                    | Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.GenericRead
                    | Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.CreateTag
                    , 0),
                new Microsoft.TeamFoundation.Framework.Client.AccessControlEntry(
                    identityContributors.Descriptor,
                    Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.CreateBranch
                    | Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.GenericContribute
                    | Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.ManageNote
                    | Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.GenericRead
                    | Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.CreateTag
                    , 0),
                new Microsoft.TeamFoundation.Framework.Client.AccessControlEntry(
                    identityProjectAdministrators.Descriptor,
                    Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.All, 0),
                new Microsoft.TeamFoundation.Framework.Client.AccessControlEntry(
                    identityReaders.Descriptor,
                    Microsoft.TeamFoundation.Git.Common.GitRepositoryPermissions.GenericRead, 0),
            };

            var gitSvc = tpc.GetService<Microsoft.TeamFoundation.Git.Client.GitRepositoryService>();

            gitSvc.CreateTeamProjectRepository(teamProject, aces);
        }

        private static void AddGitRepo(string tfsUrl, string teamProjectCollection, string teamProject, string additionalRepoName)
        {
            string tpcUrl = tfsUrl + "/" + teamProjectCollection + "/";
            var tpc = Microsoft.TeamFoundation.Client.TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(tpcUrl));

            tpc.EnsureAuthenticated();

            var gitSvc = tpc.GetService<Microsoft.TeamFoundation.Git.Client.GitRepositoryService>();
            var repos = gitSvc.QueryRepositories(teamProject);

            var defaultRepo = repos[0];

            var newRepo = new Microsoft.TeamFoundation.SourceControl.WebApi.GitRepository()
            {
                Name = additionalRepoName,
                ProjectReference = defaultRepo.ProjectReference
            };

            var cred = new Microsoft.VisualStudio.Services.Common.VssCredentials(true);
            var gitWebApiClient = new Microsoft.TeamFoundation.SourceControl.WebApi.GitHttpClient(new Uri(tpcUrl), cred);

            var createRepoTask = gitWebApiClient.CreateRepositoryAsync(newRepo);
            createRepoTask.Wait();
            var createdRepo = createRepoTask.Result;
        }
    }
}
