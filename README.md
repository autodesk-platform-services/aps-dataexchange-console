# Sample console connector

[![oAuth2](https://img.shields.io/badge/oAuth2-v2-green.svg)](http://developer.autodesk.com/)
![.NET](https://img.shields.io/badge/.NET%20Framework-4.8-blue.svg)
![Intermediary](https://img.shields.io/badge/Level-Intermediary-lightblue.svg)
[![License](http://img.shields.io/:license-MIT-blue.svg)](http://opensource.org/licenses/MIT)

# Description
This small application serves as a demo for setting up and using the DX SDK as a Nuget package. The sample demonstrates how one can Create or Update Data Exchanges using DX SDK.

# Thumbnail
![image](https://github.com/autodesk-platform-services/DataExchange-sample-consoleapp/assets/143083177/00fee44d-6522-4612-b868-cab5d4dd185d)


# Setup
**DX SDK** is installed into this project as a package reference. All required packages are a part of packages.config and will be restored automatically on first build.

## Prerequisites
1. [Register an app](https://aps.autodesk.com/myapps/), and select the Data Management and the Data Exchange APIs. Note down the values of **Client ID, Client Secret and Auth Callback**. For more information on different types of apps, refer [Application Types](https://aps.autodesk.com/en/docs/oauth/v2/developers_guide/App-types/) page.
2. Verify that you have access to the [Autodesk Construction Cloud](https://acc.autodesk.com/) (ACC).
3. Visual Studio.
4. Dot NET Framework 4.8 with basic knowledge of C#.

## Running locally
1. Clone this repository using *git clone*.
2. Follow [these](https://aps.autodesk.com/en/docs/dx-sdk/v1/developers_guide/installing_the_sdk/#procedure) instructions for installing the Data Exchange .Net SDK NuGet package in Visual Studio.[Note: Autodesk.DataExchange.UI is not required for console application.]
3. Restore the DX SDK packages by one of the following approaches:
    * Building the solution using Visual Studio IDE, or 
    * Building the solution using *BuildSolution.bat*
4. Add values for Client Id, Client Secret and Auth callback in the App.config file in the sample console connector.

Once you build and run the sample console connector, it will open the URL for authentication in a web browser. 
You can enter your credentials in the authentication page and on successful authententication, you will see the Console connector screen as seen in the Thumbnail above. 

## Further Reading
### Documentation:
* [DX SDK](https://aps.autodesk.com/en/docs/fdxsdk/v1/developers_guide/overview/) 
* [CLI developer guide](https://aps.autodesk.com) 
<!--ToDo: Update links to new Prod SDK documentation-->

# License
This sample is licensed under the terms of the MIT License. Please see the LICENSE file for full details.

# Written by
Dhiraj Lotake, PSET , Autodesk
