using Microsoft.Extensions.CommandLineUtils;
using SqlDataDiff.DataDiff.Implementations;
using SqlDataDiff.DataDiff.Implementations.TableSchemaValidators;
using SqlDataDiff.DataDiff.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace SqlDataDiff.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication { Description = "SqlDataDiff" };

            app.OnExecute(() =>
            {
                app.ShowHint();
                return 0;
            });

            var create = app.Command("difftables", config =>
            {
                config.Description = "create data diff for tables";

                var srcTableArgument = config.Argument("sourceTable", "Source table name");
                var targetTableArgument = config.Argument("targetTable", "Target table name");

                bool validateArguments(CommandArgument srcTbl, CommandArgument targetTbl) => !string.IsNullOrEmpty(srcTbl.Value) && !string.IsNullOrEmpty(targetTbl.Value);

                var sourceConnectionString = config.Option("--src|-s", "Source database connection string", CommandOptionType.SingleValue);
                var targetConnectionString = config.Option("--target|-t", "Target database connection string", CommandOptionType.SingleValue);
                var outputFile = config.Option("--output|-o", "Output file", CommandOptionType.SingleValue);
                var idempotentScript = config.Option("--idempotent|-i", "Generate idempotent script", CommandOptionType.NoValue);

                bool validateOptions(CommandOption srcConnectionString, CommandOption trgtConnectionString, CommandOption outFile) => srcConnectionString.HasValue() && trgtConnectionString.HasValue() && outFile.HasValue();

                config.OnExecute(async () =>
                {
                    if (!validateOptions(sourceConnectionString, targetConnectionString, outputFile) || !validateArguments(srcTableArgument, targetTableArgument))
                    {
                        config.ShowHelp(); //show help
                        return 0;
                    }

                    IDataTableReader dataTableReader = new SqlDataTableReader();

                    var srcTable = dataTableReader.GetDataTable(sourceConnectionString.Value(), srcTableArgument.Value);
                    var targetTable = dataTableReader.GetDataTable(targetConnectionString.Value(), targetTableArgument.Value);

                    var validators = new List<ITableSchemaValidator> { new SamePrimaryKeysValidator(), new KeysDataTypeValidator(), new CompositeKeysValidator() };
                    var service = new TableDataDiffService(new QueryFormatter(), new TableSchemaValidatorsComposite(validators));

                    var (success, diffScript, error) = service.GetDataDiffSql(srcTable, targetTable, idempotentScript.Value() != null);

                    if (!success)
                    {
                        Console.WriteLine(error);
                        return 1;
                    }

                    try
                    {
                        await File.WriteAllTextAsync(outputFile.Value(), diffScript);
                    }
                    catch (IOException ioException)
                    {
                        Console.Error.WriteLine(ioException.Message);
                        return 1;
                    }

                    return 0;
                });
                config.HelpOption("-? | -h | --help");
            });

            app.HelpOption("-? | -h | --help");
            var result = app.Execute(args);
            Environment.Exit(result);
        }
    }
}
