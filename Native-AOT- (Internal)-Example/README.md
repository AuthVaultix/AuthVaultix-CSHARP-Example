# AuthVaultix Native AOT Example Project

This repository contains a full, modern C# console application showing how to implement the **AuthVaultix SDK** in a **Native AOT (Ahead-Of-Time)** compilation workflow using .NET 8.0.

## Project Structure

- **`AuthVaultixNativeAotExample.csproj`**: The project file configured with Native AOT publish options.
- **`AuthVaultix.cs`**: The core AuthVaultix SDK. Optimized for modern .NET 8.0.
- **`Program.cs`**: The console interface coordinating SDK functions and user inputs.

---

## How to Build & Publish

To build this project as a single, standalone native binary, follow these steps:

### Prerequisites
- Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).
- For Windows Native compilation, install the **C++ build tools** via Visual Studio Installer (check *"Desktop development with C++"* workload).

### Steps

1. Open PowerShell or Command Prompt in the project folder:
   ```cmd
   cd AuthVaultixNativeAotExample
   ```

2. Restore NuGet dependencies:
   ```cmd
   dotnet restore
   ```

3. Publish a standalone, native executable optimized for Windows:
   ```cmd
   dotnet publish -r win-x64 -c Release
   ```

### Output
The compiled standalone native binary will be generated at:
`bin/Release/net8.0/win-x64/publish/AuthVaultixNativeAotExample.exe`
This file can be run on any 64-bit Windows machine without requiring any other files or .NET runtimes.
