# PowerDesigner Repository Client Library

This library enables managed access from any .NET language to the PowerDesigner Repository through COM Interop.

## Prerequisites

You will need to have the following installed on your machine:

* .NET Framework 4.8
* PowerDesigner 16.5 or higher
* Visual Studio 2019 or higher (for build purposes)

## Building

In order to build the library, you will have to add references to the following PowerDesigner assemblies, supplied by SAP:
* Interop.PdCommon
* Interop.PdRMG (Repository Management)

Optional:
* Interop.PdPDM (Physical Data Models)
* Interop.PdWSP (Workspace)

Most of the time you can find the assemblies in `C:\Program Files\SAP\PowerDesigner 16`. Make sure the assembly versions match the version of PowerDesigner you are using.

## Usage

Check out the Repository.Samples folder detailed usage samples.