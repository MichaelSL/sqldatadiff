using SqlDataDiff.DataDiff.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SqlDataDiff.DataDiff.Implementations.TableSchemaValidators
{
    public class SamePrimaryKeysValidator : ITableSchemaValidator
    {
        public (bool, IEnumerable<string>) Validate(DataTable srcTable, DataTable dstTable)
        {
            var srcPrimaryKeys = srcTable.PrimaryKey;
            var dstPrimaryKeys = dstTable.PrimaryKey;

            if (!srcPrimaryKeys.SequenceEqual(dstPrimaryKeys, new DataColumnEqualityComparer()))
            {
                return (false, new [] { "Primary keys doesn't match" });
            }
            return (true, null);
        }

        class DataColumnEqualityComparer : IEqualityComparer<DataColumn>
        {
            public bool Equals(DataColumn x, DataColumn y)
            {
                return (x.ColumnName == y.ColumnName) && (x.DataType == y.DataType);
            }

            public int GetHashCode(DataColumn obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
