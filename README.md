[![RunTests](https://github.com/MobiFlight/MobiFlight-Connector/actions/workflows/ci.yml/badge.svg)](https://github.com/MobiFlight/MobiFlight-Connector/actions/workflows/ci.yml)

# Readme #

### What is this repository for? ###
This repository contains the code for the Mobiflight Project

### How do I get set up? ###

#### Prerequisites ####

You will need the following development tools:

*  Visual Studio
*  Node.js and npm \
   One option for installing these is [nvm-windows](https://github.com/coreybutler/nvm-windows). Once you've downloaded nvm-windows itself, run
   ```
   nvm install latest
   ```
   to install the latest version of Node.js and npm, then
   ```
   nvm use $VERSION
   ```
   where `$VERSION` is the version you just installed.

#### Summary of set up ####
Checkout the code and open the MobiFlightConnector.sln project.

#### Configuration ####
Compile with DEBUG option for development and debugging.

Compile with RELEASE option for release - this will also execute the release build scripts after compiling that package MobiFlight nicely.

Before running MobiFlightConnector from Visual Studio, you'll need to start a Node development server. To do this, open a terminal window, change to the
src\MobiFlightConnector\frontend directory, then run

```
npm install && npm run dev
```

#### Dependencies ####
All Dependencies are currently contained in the repository and not referenced dynamically from their repository

* CommandMessenger - Library for communication back and forth between PC and Arduino
* MobiFlightConnector - The PC application for configuration and communication between Flightsim and Arduino
* MobiFlightUnitTests - The test suite for the MobiFlight Connector
* VersionInfo - A helper tool to detect the current release version, used during creation of Release Package

#### How to run tests ####
Run the MobiFlightConnectorTest project for unit tests
#### Build release package instructions ####
The release package is generated automatically using a RELEASE-build POST BUILD STEP. The necessary program to "sniff" the current version is part of the solution.

### Contribution guidelines ###

To be defined...

* Writing tests
* Code review
* Other guidelines

### Get more information ###
Look at the wiki to [check for further information ](https://github.com/Mobiflight/MobiFlight-Connector/wiki)

### Who do I talk to? ###
Sebastian
