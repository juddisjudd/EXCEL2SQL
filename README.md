# Excel to MariaDB SQL Converter

![](https://i.imgur.com/NlUzvgZ.png)

Excel to MariaDB Converter is a simple desktop application built with C# WinForms and .NET 6.0. It allows users to convert Excel files (.xlsx) to MariaDB .SQL files by leveraging the [RebaseData API](https://www.rebasedata.com/).

## Features

- Drag and drop an Excel file (.xlsx) onto the application
- Convert the Excel file to a MariaDB .SQL file by clicking the "Convert" button
- Download the resulting .SQL file as a .zip archive
- Display progress using a progress bar during file upload and download

## Installation

### Prerequisites

- Install the latest version of [Visual Studio](https://visualstudio.microsoft.com/downloads/) with the ".NET Desktop Development" workload
- Install [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) if not already installed with Visual Studio

### Steps

1. Clone the repository or download the source code
2. Open the solution file `ExcelToMariaDBConverter.sln` in Visual Studio
3. Right-click on the solution in the Solution Explorer, and click "Restore NuGet Packages"
4. Press F5 or click "Start" to run the application in Debug mode

## Usage

1. Drag and drop an Excel file (.xlsx) onto the application window
2. Click the "Convert" button
3. Choose a location to save the output .zip file containing the .SQL file
4. Once the conversion is complete, a success message will appear

## License

[MIT](https://choosealicense.com/licenses/mit/)
