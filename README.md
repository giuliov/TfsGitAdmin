# TfsGitAdmin
Team Foundation Server 2013 utility to manage Git repositories in a Team Project.

Some information on API used can be found in my blog post [Automating TeamProject creation](http://blog.casavian.eu/blog/2014/10/30/automating-project-creation/).

## Usage

Available commands are:

 * add         - Adds an additional Git repository
 * create      - Creates the default Git repository on a Team Project without version control
 * delete      - Deletes an existing Git repository
 * list        - List all Git repositories in a Team Project
 * rename      - Renames an existing Git repository
 * help <name> - For help with one of the above commands

## Examples

```
TfsGitAdmin add -s http://localhost:8080/tfs -c DefaultCollection -p MySampleProject -r MyAdditionalGitRepository
```

Adds an additional Git repository to Team Project `MySampleProject`.

```
TfsGitAdmin rename -s http://localhost:8080/tfs -c DefaultCollection -p MySampleProject -o MyOldGitRepository -n MyNewGitRepository
```

Renames the existing `MyOldGitRepository` Git repository to `MyNewGitRepository`.

```
TfsGitAdmin list -s http://localhost:8080/tfs -c DefaultCollection -p MySampleProject
```

Lists all Git repositories in `MySampleProject`.


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
