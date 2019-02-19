# SqlDataDiff

[![Build status](https://ci.appveyor.com/api/projects/status/x376j46kr9dancia/branch/master?svg=true)](https://ci.appveyor.com/project/MichaelSL/sqldatadiff/branch/master)

Utility to compare data in MS Sql Server tables and generate a migration script.
The script applied to source table updates data to be the same like in target table.

## Pre requisites
[.Net Core](https://dotnet.microsoft.com/download)

## Usage

```
Usage:  difftables [arguments] [options]

Arguments:
  sourceTable  Source table name
  targetTable  Target table name

Options:
  --src|-s          Source database connection string
  --target|-t       Target database connection string
  --output|-o       Output file
  --idempotent|-i   Generate idempotent script
  -? | -h | --help  Show help information
```

```
Usage:  dbdatacheck [options]

Options:
  --src|-s          Source database connection string
  --target|-t       Target database connection string
  --output|-o       Output file
  -? | -h | --help  Show help information
  ```