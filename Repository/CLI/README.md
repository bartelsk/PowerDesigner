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
Please refer to the prerequisites in the [README](../README.md) file in the root of this repository.

## Quickstart

TODO
<br>

## Commands

Work seamlessly with the PowerDesigner repository from the command line.
<br>

## User commands

The following user commands are available:
- [Create](#create)
- [Status](#status)
- [Unblock](#unblock)

### Create

Creates a user and adds the user account to a group.

```bash
pdr user create [options]
``` 

**Options** 

- ``-rd``, ``--repo-definition``
     - Specifies the repository definition used to connect to the repository (optional).
- ``-ru``, ``--repo-user``
    - The login name of the account that is used to connect to the repository.
- ``-rp``, ``--repo-password``
    - The password of the account used to connect to the repository.

**Examples**

```bash
# TODO
``` 

### Status

Returns the user account status.

```bash
pdr user status [options]
``` 

**Options** 

- ``-rd``, ``--repo-definition``
     - Specifies the repository definition used to connect to the repository (optional).
- ``-ru``, ``--repo-user``
    - The login name of the account that is used to connect to the repository.
- ``-rp``, ``--repo-password``
    - The password of the account used to connect to the repository.
- ``-l``, ``--login-name``
    - Specifies the login name of the user for which to get its status.

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

- ``-rd``, ``--repo-definition``
     - Specifies the repository definition used to connect to the repository (optional).
- ``-ru``, ``--repo-user``
    - The login name of the account that is used to connect to the repository.
- ``-rp``, ``--repo-password``
    - The password of the account used to connect to the repository.
- ``-l``, ``--login-name``
    - Specifies the login name of the user to unblock.

**Examples**

```bash
# Unblocks user 'UserA'
$ pdr user unblock --login-name UserA --repo-user Admin --repo-password P@ssw0rd

# Unblock a user while using a repository definition and the single-dash convention
$ pdr user unblock -l UserA -rd MyRepoDefinition -ru Admin -rp P@ssw0rd
``` 
