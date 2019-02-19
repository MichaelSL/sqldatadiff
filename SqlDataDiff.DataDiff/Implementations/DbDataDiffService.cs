using SqlDataDiff.DataDiff.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SqlDataDiff.DataDiff.Implementations
{
    public class DbDataDiffService : IDbDataDiffService
    {
        private const string SqlServerGetTablesQuery = @"SELECT [name] FROM sys.Tables WHERE [name] <> '__EFMigrationsHistory'";
        private const string ReadTableQueryTemplate = @"SELECT * FROM {0}";
        private readonly ITableDataDiffService tableDataDiffService;

        public DbDataDiffService(ITableDataDiffService tableDataDiffService)
        {
            this.tableDataDiffService = tableDataDiffService ?? throw new ArgumentNullException(nameof(tableDataDiffService));
        }

        public (bool, bool, string) IsDatabasesDataIdentical(IDbConnection dbSourceConnection, IDbConnection dbTargetConnection)
        {
            var srcConnectionType = dbSourceConnection.GetType();
            var targetConnectionType = dbTargetConnection.GetType();

            if (srcConnectionType != targetConnectionType)
            {
                return (false, false, $"Database connection types doesn't match: {srcConnectionType.Name}, {targetConnectionType.Name}");
            }

            IDbCommand getSrcTablesCommand = null;
            IDbCommand readSrcTableCommand = null;
            if (srcConnectionType == typeof(SqlConnection))
            {
                getSrcTablesCommand = new SqlCommand(SqlServerGetTablesQuery, dbSourceConnection as SqlConnection);
                readSrcTableCommand = new SqlCommand
                {
                    Connection = dbSourceConnection as SqlConnection
                };
            }

            IDbCommand getTargetTablesCommand = null;
            IDbCommand readTargetTableCommand = null;
            if (targetConnectionType == typeof(SqlConnection))
            {
                getTargetTablesCommand = new SqlCommand(SqlServerGetTablesQuery, dbTargetConnection as SqlConnection);
                readTargetTableCommand = new SqlCommand
                {
                    Connection = dbTargetConnection as SqlConnection
                };
            }

            if (getSrcTablesCommand == null)
            {
                return (false, false, $"Database connection provider not supported: {srcConnectionType.Name}");
            }
            if (getTargetTablesCommand == null)
            {
                return (false, false, $"Database connection provider not supported: {targetConnectionType.Name}");
            }

            var didWeOpenTheSrcConnection = false;
            var didWeOpenTheTargetConnection = false;
            if (dbSourceConnection.State != ConnectionState.Open)
            {
                dbSourceConnection.Open();
                didWeOpenTheSrcConnection = true;
            }
            if (dbTargetConnection.State != ConnectionState.Open)
            {
                dbTargetConnection.Open();
                didWeOpenTheTargetConnection = true;
            }

            var sourceTableNames = ReadNames(getSrcTablesCommand).OrderBy(_ => _);
            var targetTableNames = ReadNames(getTargetTablesCommand).OrderBy(_ => _);

            if (!sourceTableNames.SequenceEqual(targetTableNames))
            {
                //different tables in a databases
                return (true, false, null);
            }

            foreach (var tableName in sourceTableNames)
            {
                DataTable srcTable, targetTable;
                var query = string.Format(ReadTableQueryTemplate, tableName);

                readSrcTableCommand.CommandText = query;
                //srcTable = LoadTable(readSrcTableCommand);
                using (var reader = readSrcTableCommand.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    srcTable = LoadTable(reader);
                }
                readTargetTableCommand.CommandText = query;
                //targetTable = LoadTable(readTargetTableCommand);
                using (var reader = readTargetTableCommand.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    targetTable = LoadTable(reader);
                }

                var (success, res, error) = tableDataDiffService.TablesDataIdentical(srcTable, targetTable);
                if (!success)
                {
                    return (success, false, error);
                }
                if (!res)
                {
                    return (true, res, null);
                }
            }

            if (didWeOpenTheSrcConnection)
            {
                dbSourceConnection.Close();
            }
            if (didWeOpenTheTargetConnection)
            {
                dbTargetConnection.Close();
            }

            return (true, true, null);
        }

        private List<string> ReadNames(IDbCommand dbCommand)
        {
            var names = new List<string>();
            using (var reader = dbCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    names.Add(reader.GetString(0));
                }
            }
            return names;
        }

        private DataTable LoadTable(IDataReader dataReader)
        {
            var dataTable = new DataTable();
            dataTable.Load(dataReader);
            return dataTable;
        }

        private DataTable LoadTable(IDbCommand dbCommand)
        {
            var dataTable = new DataTable();
            using (var da = new SqlDataAdapter(dbCommand as SqlCommand))
            {
                da.Fill(dataTable);
            }
            return dataTable;
        }
    }
}
