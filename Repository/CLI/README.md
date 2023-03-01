<h1 align="center">PowerDesigner Repository CLI</h1>

<p align="center">
  <br>
  <img src="PDR-CLI.png" alt="PowerDesigner Repository CLI logo" width="140px" height="140px"/>
  <br><br>
  <i>The PowerDesigner Repository CLI is a command-line interface tool that allows you <br>to interact with the PowerDesigner repository directly from a command shell.</i>
  <br>
</p>

<p align="center">  
  <a href="https://github.com/bartelsk/PowerDesigner/blob/main/LICENSE">
    <img src="https://img.shields.io/github/license/bartelsk/powerdesigner" alt="Apache 2 license" />
  </a>
</p>

<hr>

## Introduction

This PowerDesigner Repository CLI enables interaction with the PowerDesigner Repository from the command line. It can:
- List and check out documents (models, extensions, et cetera) 
- Manage users and groups
- Create and list branches

## Prerequisites
Please refer to the prerequisites and build instructions in the [README](../README.md#prerequisites) file in the root of this repository.

## Quickstart

Build the main solution in `Debug` or `Release` mode and copy the files in the respective `bin\x64\Debug` or `bin\x64\Release` folder to a location convenient to you.
Open a command prompt for that location and type:

```
pdr --help
```

to view the available commands and their options. 

The following section lists the available commands in more detail.
<br>

## Commands

Work seamlessly with the PowerDesigner repository from the command line.
 
:point_right: Currently, the CLI supports commands related to:
- [Authentication]()
- [Documents](#document-commands)
- [Users](#user-commands)
- [Branches](#branch-commands)

More commands are expected to be added in the near future.
<br>

## Authentication
In order to run CLI commands, you must first set up a connection profile. The connection profile stores the login details of the account that is used to connect to the repository.
This information is stored securely in the `UserProfile` folder on the machine you run the command on.

The connection profile cannot be used on a different machine or transferred to another user, as it is encrypted with a private key that considers machine and user details.

Under the hood, the [PowerDesigner Repository Client Library](../README.md) uses an active database connection to the repository. 
The actual 'authentication' (database login) is performed by the CLI when you run a command. The CLI does not maintain the database connection as it is stateless.

The following authentication commands are available:
- [LogIn](#login)
- [LogOut](#logout)

### LogIn

Persists repository connection information.

```bash
pdr auth login [options]
``` 

**Options** 

- ``-rd``, ``--repo-definition``
     - Specifies the repository definition used to connect to the repository (optional).
- ``-ru``, ``--repo-user``
    - The login name of the account that is used to connect to the repository (required).
- ``-rp``, ``--repo-password``
    - The password of the account used to connect to the repository (optional). If omitted, a secure prompt will appear to enter the password.

**Examples**

```bash
# Create connection profile for user 'Admin' and prompt for the password
$ pdr auth login --repo-user Admin

# Create connection profile for user 'Admin' and repository definition 'MyRepoDefinition' while using the single-dash convention
$ pdr auth login -rd MyRepoDefinition -ru Admin -rp P@ssw0rd
``` 

### LogOut

Removes persisted repository connection information.

```bash
pdr auth logout
``` 

**Examples**

```bash
# Delete connection profile
$ pdr auth logout
``` 

## Document commands
The following document commands are available:
- [Info](#info)
- [List](#list)
- [Checkout](#checkout)

### Info

Returns repository document information.

```bash
pdr document info [options]
``` 

**Options** 

- ``-fp``, ``--folder-path``
    - The repository folder that contains the document (required).
- ``-dn``, ``--document-name``
    - The name of the document (required).

**Examples**

```bash
# Get details of model 'MyModel' in folder 'MyFolder'
$ pdr document info --folder-path MyFolder --document-name MyModel

# Get details of model 'MyModel' in folder 'MyFolder' while using the single-dash convention
$ pdr document info -fp MyFolder -dn MyModel
``` 

### List

Enumerates repository documents.

```bash
pdr document list [options]
``` 

**Options** 

- ``-fp``, ``--folder-path``
    - The repository folder to query (required).
- ``-r``, ``--recursive``
    - Indicates whether to list documents in any sub-folder of the specified repository folder (optional).

**Examples**

```bash
# List all documents in folder 'MyFolder'
$ pdr document list --folder-path MyFolder

# List all documents in folder 'MyFolder' and its subfolders while using the single-dash convention
$ pdr document list -fp MyFolder -r true 
``` 

### Checkout

Contains sub-commands related to checking out repository documents. The following sub-commands are available:
- [File](#file)
- [Folder](#folder)

#### File

Checks out a single document in a repository folder.

```bash
pdr document checkout file [options]
``` 

**Options** 

- ``-fp``, ``--folder-path``
    - The repository folder from which to retrieve the document (required).
- ``-tf``, ``--target-folder``
    - The folder on disc to use as the check-out location for the document (required).
- ``-dn``, ``--document-name``
    - The name of the document to check out (required). 
- ``-dv``, ``--document-version``
    - The document version. The latest version of the document will be checked out if the specified document version does not exist. The version must also belong to the same branch as the current object (optional).

**Examples**

```bash
# Check out the latest version of model 'MyModel' in folder 'MyFolder' and save it to C:\Temp
$ pdr document checkout file --folder-path MyFolder --document-name MyModel --target-folder C:\Temp

# # Check out the version 2 of model 'MyModel' in folder 'MyFolder' and save it to C:\Temp while using the single-dash convention
$ pdr document checkout file -fp MyFolder -dn MyModel -tf C:\Temp
``` 

#### Folder

Checks out all documents in a repository folder.

```bash
pdr document checkout folder [options]
``` 

**Options** 

- ``-fp``, ``--folder-path``
    - The repository folder from which to retrieve the documents (required).
- ``-tf``, ``--target-folder``
    - The folder on disc to use as the check-out location for the documents (required).
- ``-r``, ``--recursive``
     - Indicates whether to retrieve documents in any sub-folder of the specified repository folder (optional).
- ``-ps``, ``--preserve-structure``
     - Indicates whether to mimic the repository folder structure on the local disc when checking out. Applies to recursive check-outs only. (optional).

**Examples**

```bash
# Check out all documents in folder 'MyFolder' and save it to C:\Temp
$ pdr document checkout folder --folder-path MyFolder --target-folder C:\Temp

# Check out all documents in folder 'MyFolder' and its subfolder and save it to C:\Temp while preserving the repository folder structure and using the single-dash convention
$ pdr document checkout folder -fp MyFolder -tf C:\Temp -r true -ps true
``` 

## User commands

The following user commands are available:
- [Create](#create)
- [Password](#password)
- [Status](#status)
- [Unblock](#unblock)

### Create

Creates a user and and optionally adds the user account to an existing group.

```bash
pdr user create [options]
``` 

**Options** 

- ``-ln``, ``--login-name``
    - Specifies the login name for the new user (required).
- ``-fn``, ``--full-name``
    - The real name of the new user (required).
- ``-ue``, ``--email-address``
    - The email address of the new user (required).
- ``-ur``, ``--user-right``
    - The rights for the new user (optional). Can be specified multiple times for compound rights.
    - Allowed values are: 
      - None
      - Connect
      - FreezeVersions
      - LockVersions
      - ManageBranches
      - ManageConfigurations
      - ManageAllDocuments
      - ManageUsers
      - ManageRepository
      - EditPortalObjects
      - EditPortalExtensions
- ``-ug``, ``--group``
    - The name of the group to which to add the user (optional). Group rights will be inherited by the new user.

**Examples**

```bash
# Create a user with 'Connect' rights
$ pdr user create --login-name JWilliams --full-name "John Williams" --email-address john@williams.com --user-right Connect

# Create a user with 'Connect' and 'ManageUsers' rights using the single-dash convention
$ pdr user create -ln JWilliams -fn "John Williams" -ue john@williams.com -ur Connect -ur ManageUsers

# Create a user with 'Connect' and 'ManageUsers' rights and add him to the 'Developers' groups while using the single-dash convention
$ pdr user create -ln JWilliams -fn "John Williams" -ue john@williams.com -ur Connect -ur ManageUsers -ug Developers
``` 

### Password

Contains sub-commands related to repository user passwords. The following sub-commands are available:
- [Reset](#reset)

#### Reset

Resets a user password.

```bash
pdr user password reset [options]
```

**Options** 

- ``-ln``, ``--login-name``
    - Specifies the login name of the user for which to reset the password (required).

**Examples**

```bash
# Resets the password of user 'UserA'
$ pdr user password reset --login-name UserA

# Resets the password of user 'UserA' while using the single-dash convention
$ pdr user password reset -ln UserA
``` 

### Status

Returns the user account status.

```bash
pdr user status [options]
``` 

**Options** 

- ``-ln``, ``--login-name``
    - Specifies the login name of the user for which to get its status (required).

**Examples**

```bash
# Retrieves the status for user 'UserA'
$ pdr user status --login-name UserA

# Retrieves the user status while using the single-dash convention
$ pdr user status -ln UserA
``` 

### Unblock

Unblocks a user account.

```bash
pdr user unblock [options]
``` 

**Options** 

- ``-ln``, ``--login-name``
    - Specifies the login name of the user to unblock (required).

**Examples**

```bash
# Unblocks user 'UserA'
$ pdr user unblock --login-name UserA

# Unblock a user while using the single-dash convention
$ pdr user unblock -ln UserA
``` 

## Branch commands

The following branch commands are available:
- [Create](#create)
- [List](#list)

### Create

Creates a branch based on a base branch and assigns access rights.

```bash
pdr branch create [options]
``` 

**Optional** 
- ``-bb``, ``--base-branch``
    - The path of the base branch folder in the repository (required). The contents of the base branch will be copied into the new branch.
- ``-bn``, ``--branch-name``
    - The name for the new branch (required).
- ``-ug``, ``--user-group`` 
    - Either the login name of a user or a group name that is assigned branch permissions via the `--user-group-permission` option (optional). 
    - Branch permissions for the new branch will be inherited from the base branch. This option allows overriding existing permissions or adding new ones.
- ``-ugp``, ``--user-group-permission``
    - Permissions for the user or group for the new branch (optional). Only valid in combination with the `--user-group` option.
    - Allowed values are: 
      - NotSet
      - Listable
      - Read
      - Submit
      - Write
      - Full

**Examples**

```bash
# Creates branch 'NewDevBranch' based on branch 'MyFolder\DevBranch' 
$ pdr branch create --base-branch MyFolder\DevBranch --branch-name NewDevBranch

# Creates branch 'NewDevBranch' and assigns the DEV group Write access while the single-dash convention
$ pdr branch create --bb MyFolder\DevBranch -bn NewDevBranch -ug DEV -ugp Write
``` 

### List

Enumerates existing branches.

```bash
pdr branch list [options]
``` 

**Optional** 

- ``-rf``, ``--root-folder``
    - The repository folder from which to start the enumeration (required).
- ``-ug``, ``--filter``
    - A user login or group name used to filter branches based on access permission (optional).

**Examples**

```bash
# Lists all branches in folder 'MyFolder'
$ pdr branch list --root-folder MyFolder

# Lists the branches in folder 'MyFolder' that the DEV group has access to while using the single-dash convention
$ pdr branch list -rf MyFolder -ug DEV
``` 