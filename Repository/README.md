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

## Building

In order to build the library, you will have to add references to the following PowerDesigner assemblies, supplied by SAP:
- Interop.PdCommon
- Interop.PdRMG (Repository Management)

Most of the time you can find these assemblies in the installation folder `C:\Program Files\SAP\PowerDesigner 16`. Make sure the assembly versions match the version of PowerDesigner you are using.

