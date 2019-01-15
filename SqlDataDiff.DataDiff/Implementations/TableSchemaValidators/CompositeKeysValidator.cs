using SqlDataDiff.DataDiff.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SqlDataDiff.DataDiff.Implementations.TableSchemaValidators
{
    public class CompositeKeysValidator : ITableSchemaValidator
    {
        public (bool, IEnumerable<string>) Validate(DataTable srcTable, DataTable dstTable)
        {
            var srcPrimaryKeys = srcTable.PrimaryKey;
            var dstPrimaryKeys = dstTable.PrimaryKey;

            if (srcPrimaryKeys.Length > 1 || dstPrimaryKeys.Length > 1)
            {
                return (false, new [] { "Composite keys not supported" });
            }

            return (true, null);
        }
    }
}
