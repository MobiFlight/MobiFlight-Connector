# License #
MIT License

Copyright (c) 2016 MobiFlight

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

# Readme #

### What is this repository for? ###
This repository contains the code for the Mobiflight Project

### How do I get set up? ###

#### Summary of set up ####
Checkout the code and open the MobiFlightConnector.sln project.

#### Configuration ####
Compile with DEBUG-MOBIFLIGHT option for development and debugging

Compile with RELEASE-MOBIFLIGHT option for release

Compile with DEBUG-MOBIFLIGHT (EN) option to force English UI - actually for testing purpose  

Compile with DEBUG-MOBIFLIGHT (Offline) option to not depend on FSUIPC runing - actually for testing purpose  

#### Dependencies ####
All Dependecies are currently contained in the repository and not referenced dynamically from their repository

* AutoUpdater.NET - The auto update functionality - some minor local modifications
* CommandMessenger - Library for communication back and forth between PC and Arduino
* MobiflightConnector - The PC application for configuration and communication between Flightsim and Arduino
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

### Who do I talk to? ###
Sebastian