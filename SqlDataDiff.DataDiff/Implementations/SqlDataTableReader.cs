using SqlDataDiff.DataDiff.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace SqlDataDiff.DataDiff.Implementations
{
    public class SqlDataTableReader : IDataTableReader
    {
        public DataTable GetDataTable(string connectionString, string tableName)
        {
            DataTable data = new DataTable();
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                IDbCommand command = new SqlCommand();
                command.Connection = dbConnection;
                command.CommandText = "SELECT * FROM " + tableName;

                dbConnection.Open();
                IDataReader reader = command.ExecuteReader(CommandBehavior.KeyInfo);
                data.Load(reader);

                dbConnection.Close();
            }
            return data;
        }
    }
}
