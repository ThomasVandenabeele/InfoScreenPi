# InfoScreen Pi

## Description

Open source ASP.NET Core 2.1 application providing digital signage software targeting KU Leuven - Diepenbeek.
This repo is still under construction.


### Prerequisites
To run this software on Ubuntu linux install following software: 
 - Install .NET Core SDK.
 - Install libraries for System.Drawing


Before installing .NET, you'll need to register the Microsoft key, register the product repository, and install required dependencies. This only needs to be done once per machine.

Open a command prompt and run the following commands:
```console
wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
```

Update the products available for installation, then install the .NET SDK.

In your command prompt, run the following commands: 
```console
sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-sdk-2.1
```

Install the necessary libraries for System.Drawing on Linux:

```console
sudo apt install libc6-dev 
sudo apt install libgdiplus
```

## Run the software
```console
dotnet restore
dotnet run
```