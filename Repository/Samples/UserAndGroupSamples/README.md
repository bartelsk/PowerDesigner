# User and Group Samples

These samples show how to work with users and groups.

## Summary
The User and Group samples showcase examples of:

* Listing users and groups
* Checking user or group existence
* Retrieving information of a user or group
* Creating and deleting a user or group
* Handling user group membership
* Getting and setting user and group rights

## Prerequisites
Before starting any of the samples, refer to the prerequisites in the [README](../../README.md) file in the root of this repository.

Configure the [App.config](App.config) file in this folder to point to the proper PowerDesigner repository as well. The configuration file contains information on the appropriate repository definition and user account which are used to connect to the repository.

## Running the samples
All samples are set up as tests that can be run by the built-in `Test Explorer` of Visual Studio. If the `Test Explorer` window is not visible, go to the `VIEW` menu and click `Test Explorer`.

Some samples assume the presence of certain users and/or groups, so do not run all tests at once! The samples are self-contained; you should first create the sample user before retrieving its properties, for example. There is no dependency on any existing user or group.

## Samples overview
This section lists the available samples.

### ListUsers

Retrieves all users, but only displays the first 25 users.

### UserExists

Determines whether a user exists.

### GetUserInformation

Displays information on the users' status, last login date, rights and group membership.

### CreateUser

Creates a user.

### CreateUserAndAddToGroup

Creates a user and adds it to a group.

### DeleteUser

Deletes a user.

### ListGroups

Retrieve all groups, but only display the first 10 groups.

### GroupExists

Determines whether a group exists.

### GetGroupInformation

Displays group information.

### CreateGroup

Creates a group.

### GetGroupRights

Retrieves group rights.

### AddGroupRights

Assigns additional group rights.

### ReplaceGroupRights

Clears the available group rights and assigns new ones.

### DeleteGroup

Deletes a group.

### GetUserGroups

Retrieves the groups of which the user is a member.

### GetUserRights

Retrieves user rights.

### AddUserRights

Assigns additional user rights.

### ReplaceUserRights

Clears the available user rights and assigns new ones.

## User and group rights

The following user and group rights (privileges) are available:
- `Connect`: connect to the repository
- `FreezeVersions`: freeze and unfreeze document versions
- `LockVersions`: lock and unlock documents to prevent other users from making changes to them
- `ManageBranches`: create repository branches
- `ManageConfigurations`: create sets of repository documents
- `ManageAllDocuments`: perform any action on any document version. Implicitly includes `Full` permission on all repository documents.
- `ManageUsers`: create, modify, and delete repository users and groups, grant them rights, and add them to groups. Users with this right can list all repository documents and set permissions on them without needing explicit `Full` permission.
- `ManageRepository`: create, upgrade, and delete the repository database
- `EditPortalObjects`: create and edit diagrams in PowerDesigner Web        
- `EditPortalExtensions`: create and edit custom properties in PowerDesigner Web

When there are no explicit rights available, the right `None` is used.

These permissions can be found in the [UserOrGroupRightsEnum](../../Common/Enums.cs) enum.







