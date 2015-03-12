using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Git.Common;
using System;

namespace TfsGitAdmin
{
    class CreateDefaultRepoCommand : GitAdminBaseCommand
    {
        internal string AdditionalRepoName { get; set; }

        public CreateDefaultRepoCommand()
        {
            this.IsCommand("create", "Creates the default Git repository on a Team Project without version control");
        }

        public override int Run(string[] remainingArguments)
        {
            Console.WriteLine("Creating default Git repository in {0}/{1}/{2}", TfsUrl, TeamProjectCollection, TeamProject);

            Connect();

            var identityManagementService = tpc.GetService<IIdentityManagementService>();

            //collection default groups
            var identityProjectCollectionAdministrators = identityManagementService.ReadIdentity(
                    IdentitySearchFactor.DisplayName,
                    string.Format(@"[{0}]\Project Collection Administrators", TeamProjectCollection),
                    MembershipQuery.None,
                    ReadIdentityOptions.None);
            var identityProjectCollectionBuildServiceAccounts = identityManagementService.ReadIdentity(
                    IdentitySearchFactor.DisplayName,
                    string.Format(@"[{0}]\Project Collection Build Service Accounts", TeamProjectCollection),
                    MembershipQuery.None,
                    ReadIdentityOptions.None);
            var identityProjectCollectionServiceAccounts = identityManagementService.ReadIdentity(
                    IdentitySearchFactor.DisplayName,
                    string.Format(@"[{0}]\Project Collection Service Accounts", TeamProjectCollection),
                    MembershipQuery.None,
                    ReadIdentityOptions.None);

            //project default groups
            var identityBuildAdministrators = identityManagementService.ReadIdentity(
                    IdentitySearchFactor.DisplayName,
                    string.Format(@"[{0}]\Build Administrators", TeamProject),
                    MembershipQuery.None,
                    ReadIdentityOptions.None);
            var identityContributors = identityManagementService.ReadIdentity(
                    IdentitySearchFactor.DisplayName,
                    string.Format(@"[{0}]\Contributors", TeamProject),
                    MembershipQuery.None,
                    ReadIdentityOptions.None);
            var identityProjectAdministrators = identityManagementService.ReadIdentity(
                    IdentitySearchFactor.DisplayName,
                    string.Format(@"[{0}]\Project Administrators", TeamProject),
                    MembershipQuery.None,
                    ReadIdentityOptions.None);
            var identityReaders = identityManagementService.ReadIdentity(
                    IdentitySearchFactor.DisplayName,
                    string.Format(@"[{0}]\Readers", TeamProject),
                    MembershipQuery.None,
                    ReadIdentityOptions.None);

            var aces = new AccessControlEntry[] {
                new AccessControlEntry(
                    identityProjectCollectionAdministrators.Descriptor,
                    GitRepositoryPermissions.All, 0),
                new AccessControlEntry(
                    identityProjectCollectionBuildServiceAccounts.Descriptor,
                    GitRepositoryPermissions.GenericRead, 0),
                new AccessControlEntry(
                    identityProjectCollectionServiceAccounts.Descriptor,
                    GitRepositoryPermissions.All, 0),
                new AccessControlEntry(
                    identityBuildAdministrators.Descriptor,
                    GitRepositoryPermissions.CreateBranch
                    | GitRepositoryPermissions.GenericContribute
                    | GitRepositoryPermissions.ManageNote
                    | GitRepositoryPermissions.GenericRead
                    | GitRepositoryPermissions.CreateTag
                    , 0),
                new AccessControlEntry(
                    identityContributors.Descriptor,
                    GitRepositoryPermissions.CreateBranch
                    | GitRepositoryPermissions.GenericContribute
                    | GitRepositoryPermissions.ManageNote
                    | GitRepositoryPermissions.GenericRead
                    | GitRepositoryPermissions.CreateTag
                    , 0),
                new AccessControlEntry(
                    identityProjectAdministrators.Descriptor,
                    GitRepositoryPermissions.All, 0),
                new AccessControlEntry(
                    identityReaders.Descriptor,
                    GitRepositoryPermissions.GenericRead, 0),
            };

            gitSvc.CreateTeamProjectRepository(TeamProject, aces);

            return 0;
        }
    }
}
