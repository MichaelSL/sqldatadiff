using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SqlDataDiff.DataDiff.Extensions
{
    public static class DataStringExtensions
    {
        public static IDbConnection GetConnection(this string connectionString, DbProvider dbProvider)
        {
            switch (dbProvider)
            {
                case DbProvider.MsSql:
                    return new System.Data.SqlClient.SqlConnection(connectionString);
                default:
                    throw new NotImplementedException("Provider not implemented");
            }
        }
    }
}
