# Document Samples

These samples show how to work with repository documents. 

The `RepositoryDocument` class manages the storage of any file (except the file storing design model) in the repository.
The `Document` object does not own any sub-object. The whole file content is stored in a binary format.


## Summary
The Document samples showcase examples of:

* Listing documents
* Checking document existence
* Create and delete document folders
* Get document information
* Checking-in and checking-out documents
* Freezing and unfreezing documents
* Locking and unlocking documents
* Deleting document(version)s
* Managing document permissions

## Prerequisites
Before starting any of the samples, refer to the prerequisites in the [README](../../README.md) file in the root of this repository.

Configure the [App.config](App.config) file in this folder to point to the proper PowerDesigner repository as well. The configuration file contains information on the appropriate repository definition and user account which are used to connect to the repository.

## Running the samples
All samples are set up as tests that can be run by the built-in `Test Explorer` of Visual Studio. If the `Test Explorer` window is not visible, go to the `VIEW` menu and click `Test Explorer`.

Some samples assume the presence of certain documents, so do not run all tests at once! The samples are self-contained; you should first run the `CheckIn` sample before running the `CheckOutDocument` sample, for example. There is no dependency on any existing document.

## Samples overview
This section lists the available samples.

### ListDocuments

List all documents in a folder. Returns [Document](#document-information) objects.

### DocumentExists

Determines whether a document exists in a particular repository folder.

### GetDocumentInfo

Retrieves document information. Returns a [Document](#document-information) object.

### CreateDocumentFolder

Creates a repository document folder.

### DeleteDocumentFolder

Deletes a repository document folder.

### CheckInFile

Adds a file to the repository. Also demonstrates the use of the `DocumentCheckedIn` event handler to display check-in progress.

### CheckInFiles

Adds multiple files in one go to the repository. Also demonstrates the use of the `DocumentCheckedIn` event handler to display check-in progress.

### CheckOutDocument

Checks out a document with its default name and an alternative name. Also demonstrates the use of the `DocumentCheckedOut` event handler to display check-out progress.

### CheckOutDocumentOtherVersion

Checks out a specific version of a document with its default name and an alternative name. Also demonstrates the use of the `DocumentCheckedOut` event handler to display check-out progress.

### CheckOutDocuments

Checks out all documents in a folder and saves them to disc. Also demonstrates the use of the `DocumentCheckedOut` event handler to display check-out progress.

### CheckOutDocumentsRecursively

Checks out all documents in a folder (and any sub folder of that folder) into a single folder on disc. Also demonstrates the use of the `DocumentCheckedOut` event handler to display check-out progress.

### CheckOutDocumentsRecursivelyMimicingRepoStructure

Checks out all documents in a folder (and any sub folder of that folder) while preserving the repository folder structure on disc. Also demonstrates the use of the `DocumentCheckedOut` event handler to display check-out progress.

### FreezeDocument

Freezes a document, i.e. creates a new version of the document. 

### UnfreezeDocument

Unfreezes a document. Unfreezing a document version allows you to make further changes to the existing version, but means that you will no longer be able to go back and view the previously frozen state.

### LockDocument

Locks a document. Prevents other users from making changes to it.

### UnlockDocument

Unlocks a document. Allows other users to make changes to it.

### DeleteDocument

Completely removes a document.

### DeleteDocumentVersion

Removes the current version of a document, reverting it to the previous version.

### GetDocumentPermissions

Retrieves the permission of a document for a specific user or group.

### SetDocumentPermission

Grants permissions to a document for a specific user or group.

### DeleteDocumentPermission

Deletes permissions from a document for a specific user or group.

## Permissions

The following document permissions are available:
- `Listable`: view the document, branch or folder in the repository browser and in search results. Without this permission, the object is hidden from the user.
- `Read`: read-only access to the document, branch or folder
- `Submit`: permission to check-in a document
- `Write`: permission to write to the document, branch or folder
- `Full`: full access on all repository objects

When there are no explicit permissions available, the permission `NotSet` is used.

These permissions can be found in the [PermissionTypeEnum](../../Common/Enums.cs) enum. The `GetDocumentPermissions` and `SetDocumentPermissions` samples showcase its usage.

## Document information

Information on a repository document is returned as a [Document](../../Common/Document.cs) object with following properties:

- `Name`: the name of the document
- `ClassName`: the PowerDesigner class name of the document
- `ExtractionFileName`: the file name of the document when checked-out.
- `IsFrozen`: determines whether the document is frozen or not.
- `IsLocked`: determines whether the document is locked or not.
- `Location`: the location of the document in the repository
- `ObjectType`: the object type of the document
- `Version`: the document version
- `VersionComment`: the document version comment

The samples `ListDocuments` and `GetDocumentInfo` show an implementation of this class.


