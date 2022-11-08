<h1 align="center">PowerDesigner Repository Client Library</h1>

<p align="center">
  <br>
  <img src="PDR.png" alt="PowerDesigner Repository Client Library logo" width="140px" height="140px"/>
  <br><br>
  <i>The PowerDesigner Repository Client Library enables managed access <br>from any .NET language to the PowerDesigner Repository through COM Interop.</i>
  <br>
</p>

<p align="center">  
  <a href="https://github.com/bartelsk/PowerDesigner/blob/main/LICENSE">
    <img src="" alt="Apache 2 license" />
  </a>
</p>

<hr>

## Introduction

The PowerDesigner Repository Client Library allows you to:
- Work with documents (models, extensions, et cetera)
- Manage users and groups
- Create and delete branches

all from the comfort of .NET! :heart_eyes:

## Usage

- Check out the [Samples](/Repository/Samples) folder for detailed usage samples demonstrating common activities.

## Prerequisites

You will need to have the following installed on your machine:

- .NET Framework 4.8
- PowerDesigner 16.5 or higher
- Visual Studio 2019 or higher (for build purposes)

**Note:** The PowerDesigner Repository Client Library uses an active database connection to the repository through COM Interop, which means PowerDesigner needs to be installed on the machine you intend to use the Client Library.

## Building

In order to build the library, you will have to add references to the following PowerDesigner assemblies, supplied by SAP:
- Interop.PdCommon
- Interop.PdRMG (Repository Management)

These assemblies will be available in the installation folder: `C:\Program Files\SAP\PowerDesigner 16`. Make sure the assembly versions match the version of PowerDesigner you are using.

## NuGet package?

There is no NuGet package for the PowerDesigner Repository Client Library as it needs to be built with the commercially available PowerDesigner assemblies (from your local PowerDesigner installation). 
Also, the necessary assemblies need to match the version of PowerDesigner you are using. 

## Changelog

[Learn about the latest improvements](CHANGELOG.md)

## Questions

- Do you have questions? Please [join our Discussions Forum](https://github.com/bartelsk/PowerDesigner/discussions).
- Do you want to report a bug? Please [create a new issue](https://github.com/bartelsk/PowerDesigner/issues).



