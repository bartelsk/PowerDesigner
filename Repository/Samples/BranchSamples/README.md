# Branch samples

These samples show how to work with branches.

## Summary
The Branch samples showcase examples of:

* Listing branches
* Checking branch existence
* Creating branches
* Getting and setting branch permissions

## Prerequisites
Before starting any of the samples, refer to the prerequisites in the [README](../../README.md) file in the root of this repository.

Configure the [App.config](App.config) file in this folder to point to the proper PowerDesigner repository as well. The configuration file contains information on the appropriate repository definition and user account which are used to connect to the repository.

## Running the samples
All samples are set up as tests that can be run by the built-in `Test Explorer` of Visual Studio. If the `Test Explorer` window is not visible, go to the `VIEW` menu and click `Test Explorer`.

Some samples assume the presence of certain users and/or groups, so do not run all tests at once! The samples are self-contained; you should first create the sample user before retrieving its properties, for example. There is no dependency on any existing user or group.

## Samples overview
This section lists the available branch samples.

### ListBranches

Lists available branches relative to a repository folder.

### ListBranchesForUser

Lists available branches relative to a repository folder based on access permissions of the specified user. The access permissions of the specified user are used to filter the results.

### BranchExists

Checks whether a branch exists.

### CreateBranch

Creates a new branch based on an existing branch. The sample creates a branch while applying the permissions of the currently connected account to the new branch.

### CreateBranchWithSpecificPermissions

Creates a new branch based on an existing branch and applies specific branch permissions to a user or group. The sample creates a branch and applies branch permission for a user.

### GetBranchPermissions

 Retrieves the permission on a branch for a specific user or group. The sample retrieves the permissions of the HR group on a branch.

### SetBranchPermission

 Grants permissions to a branch for a specific user or group. The sample grants the HR group `Read` permission to the specified branch. 

### DeleteBranchPermission

Deletes permissions from a branch for a specific user or group, e.g. remove the permissions of the HR group from a branch.

## Permissions

The following permissions are available:
- `Listable`: view the document, branch or folder in the repository browser and in search results. Without this permission, the object is hidden from the user.
- `Read`: read-only access to the document, branch or folder
- `Submit`: permission to check-in a document
- `Write`: permission to write to the document, branch or folder
- `Full`: full access on all repository objects

When there are no explicit permissions available, the permission `NotSet` is used.

These permissions can be found in the [PermissionTypeEnum](../../Common/Enums.cs) enum.

## Important notes about branch creation

Creating a branch can fail for several reasons:
- The currently connected account does not have `Write` permissions on the folder
- The currently connected account does not have the `Manage Branches` privilege
- The source branch folder already belongs to a branch (sub-branches are not supported).

Be aware that PowerDesigner does not throw an exception in any of these cases!


