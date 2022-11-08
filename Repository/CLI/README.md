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
    <img src="" alt="Apache 2 license" />
  </a>
</p>

<hr>

## Introduction

This PowerDesigner Repository CLI enables interaction with the PowerDesigner Repository from the command line. It can:
- Work with documents (models, extensions, et cetera)
- Manage users and groups
- Create and delete branches

## Prerequisites
Please refer to the prerequisites in the [README](../README.md#prerequisites) file in the root of this repository.

## Quickstart

TODO
<br>

## Commands

Work seamlessly with the PowerDesigner repository from the command line.
 
All commands require at least two parameters: the login name of the account that is used to connect to the repository and its password.
This is necessary because the [PowerDesigner Repository Client Library](../README.md) uses an active database connection to the repository. 
The CLI does not maintain this connection as it is stateless.

Currently, the CLI supports the following commands related to:
- [Users](#user-commands)
- [Branches](#branch-commands)

Below you will find a detailed description of the available commands and their options.
<br>

## User commands

The following user commands are available:
- [Create](#create)
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
- ``-rd``, ``--repo-definition``
     - Specifies the repository definition used to connect to the repository (optional).
- ``-ru``, ``--repo-user``
    - The login name of the account that is used to connect to the repository (required).
- ``-rp``, ``--repo-password``
    - The password of the account used to connect to the repository (required).

**Examples**

```bash
# Create a user with 'Connect' rights
$ pdr user create --login-name JWilliams --full-name "John Williams" --email-address john@williams.com --user-right Connect --repo-user Admin --repo-password P@ssw0rd

# Create a user with 'Connect' and 'ManageUsers' rights using the single-dash convention
$ pdr user create -ln JWilliams -fn "John Williams" -ue john@williams.com -ur Connect -ur ManageUsers -ru Admin -rp P@ssw0rd

# Create a user with 'Connect' and 'ManageUsers' rights and add him to the 'Developers' groups while using a repository definition and the single-dash convention
$ pdr user create -ln JWilliams -fn "John Williams" -ue john@williams.com -ur Connect -ur ManageUsers -ug Developers -rd MyRepoDefinition -ru Admin -rp P@ssw0rd
``` 

### Status

Returns the user account status.

```bash
pdr user status [options]
``` 

**Options** 

- ``-l``, ``--login-name``
    - Specifies the login name of the user for which to get its status (required).
- ``-rd``, ``--repo-definition``
     - Specifies the repository definition used to connect to the repository (optional).
- ``-ru``, ``--repo-user``
    - The login name of the account that is used to connect to the repository (required).
- ``-rp``, ``--repo-password``
    - The password of the account used to connect to the repository (required).

**Examples**

```bash
# Retrieves the status for user 'UserA'
$ pdr user status --login-name UserA --repo-user Admin --repo-password P@ssw0rd

# Retrieves the user status while using a repository definition and the single-dash convention
$ pdr user status -l UserA -rd MyRepoDefinition -ru Admin -rp P@ssw0rd
``` 

### Unblock

Unblocks a user account.

```bash
pdr user unblock [options]
``` 

**Options** 

- ``-l``, ``--login-name``
    - Specifies the login name of the user to unblock (required).
- ``-rd``, ``--repo-definition``
     - Specifies the repository definition used to connect to the repository (optional).
- ``-ru``, ``--repo-user``
    - The login name of the account that is used to connect to the repository (required).
- ``-rp``, ``--repo-password``
    - The password of the account used to connect to the repository (required).

**Examples**

```bash
# Unblocks user 'UserA'
$ pdr user unblock --login-name UserA --repo-user Admin --repo-password P@ssw0rd

# Unblock a user while using a repository definition and the single-dash convention
$ pdr user unblock -l UserA -rd MyRepoDefinition -ru Admin -rp P@ssw0rd
``` 

## Branch commands

The following branch commands are available:
- [Create](#create)
- [List](#list)

### Create

Creates a branch and and assigns access rights.

```bash
pdr branch create [options]
``` 

**Optional** 

- ``-rd``, ``--repo-definition``
     - Specifies the repository definition used to connect to the repository (optional).
- ``-ru``, ``--repo-user``
    - The login name of the account that is used to connect to the repository (required).
- ``-rp``, ``--repo-password``
    - The password of the account used to connect to the repository (required).

**Examples**

```bash
# TODO
``` 

### List

Enumerates existing branches.

```bash
pdr branch list [options]
``` 

**Optional** 

- ``-rd``, ``--repo-definition``
     - Specifies the repository definition used to connect to the repository (optional).
- ``-ru``, ``--repo-user``
    - The login name of the account that is used to connect to the repository (required).
- ``-rp``, ``--repo-password``
    - The password of the account used to connect to the repository (required).

**Examples**

```bash
# TODO
``` 