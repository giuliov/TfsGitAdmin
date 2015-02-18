# TfsGitAdmin
Team Foundation Server 2013 utility
to create an additional Git repository in a Project set using Git verions control.
May also create the default Git repository in a Project with **no** version control.

More information is in my blog post [Automating TeamProject creation](http://blog.casavian.eu/blog/2014/10/30/automating-project-creation/).

## Usage

Usage is simple:

```
TfsGitAdmin <TFSurl> <TeamCollectionName> <TeamProjectName> [<GitRepositoryName>]
```

When invoked with three arguments, is tries to add the default Git repository to a Project without version control; if you add the fourth parameter, it tries to add a Repository with the specified name, to a project with Git version control.

Example

```
TfsGitAdmin http://localhost:8080/tfs DefaultCollection MySampleProject MyAdditionalGitRepository
```

## Build
To compile the project, you must copy in the `References` folder, the proper version of the following TFS assemblies:

- Microsoft.TeamFoundation.Client
- Microsoft.TeamFoundation.Common
- Microsoft.TeamFoundation.Git.Client
- Microsoft.TeamFoundation.Git.Common
- Microsoft.TeamFoundation.SourceControl.WebApi
- Microsoft.VisualStudio.Services.Common
- Microsoft.VisualStudio.Services.WebApi

Build with Visual Studio 2013 Update 4 for TFS 2013 Update 4, YMMV.