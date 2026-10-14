# Municipal Services Portal

A Windows desktop application that lets residents submit municipal issue reports. It is built with C# and Windows Forms.

## Features

- Report an issue with a location, category, and description.
- Attach one or more images or documents to a report.
- See completion progress while filling in the form.
- Receive a unique reference number after submitting a valid report.
- Keep reports available for the duration of the current application session.

The **Local Events and Announcements** and **Service Request Status** menu options are placeholders for future development.

## Requirements

- Windows
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 (optional, with the .NET desktop development workload)

## Getting started

From the repository root, build and run the project:

```powershell
dotnet build
dotnet run --project .\AAPD_Practical\AAPD_Practical.csproj
```

Alternatively, open `AAPD_Practical.slnx` in Visual Studio and press `F5`.

## How to use the application

1. Select **Report an Issue** from the main menu.
2. Enter the required location, category, and description.
3. Optionally select **Attach images or documents** to add supporting files.
4. Select **Submit Report**.
5. Note the generated reference number shown in the confirmation dialog.

## Data storage

Reports are stored in memory in an `IssueRepository`, so they are cleared when the application closes. Attachments are stored only as references to their original file paths; the application does not copy or modify the files.

## Part 2: Local Events and Announcements

1. Select **Local Events and Announcements** from the main menu.
2. Select a category and optionally enable **Filter by date**.
3. Select **Search events** to view matching events and notices.
4. Review **Recommended for you**, which adapts to category searches.

The implementation uses a `SortedDictionary` for date ordering, `Dictionary` and `HashSet` for category lookup, a `PriorityQueue` for upcoming events, and a `Stack` and `Queue` for search history.

Run the catalogue checks with:

```powershell
dotnet run --project .\AAPD_Practical\AAPD_Practical.csproj -- --self-test
```

## Project structure

```text
AAPD_Practical/
├── Program.cs               Application entry point
├── MainMenuForm.cs          Main menu UI
├── ReportIssueForm.cs       Issue reporting UI and validation
├── IssueReport.cs           Issue report model
└── IssueRepository.cs       In-memory report storage
```
