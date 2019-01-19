# SqlDataDiff

Utility to compare data in MS Sql Server tables and generate a migration script.
The script applied to source table updates data to be the same like in target table.

## Pre requisites
[.Net Core](https://dotnet.microsoft.com/download)

## Usage

```dotnet SqlDataDiff.dll difftables <source table name> <target table name> -s "<source db connection string>" -t "<target db connection string>" -o diff.sql```