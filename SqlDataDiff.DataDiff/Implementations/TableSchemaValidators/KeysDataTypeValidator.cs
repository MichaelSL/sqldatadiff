using SqlDataDiff.DataDiff.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SqlDataDiff.DataDiff.Implementations.TableSchemaValidators
{
    public class KeysDataTypeValidator : ITableSchemaValidator
    {
        public (bool, IEnumerable<string>) Validate(DataTable srcTable, DataTable dstTable)
        {
            var srcPrimaryKeys = srcTable.PrimaryKey;
            var dstPrimaryKeys = dstTable.PrimaryKey;

            if (srcPrimaryKeys.Any(k => !TypeSupported(k.DataType)) || dstPrimaryKeys.Any(k => !TypeSupported(k.DataType)))
            {
                return (false, new[] { $"Primary keys data type is not supported. Supported data types: {string.Join(", ", supportedTypes.Values)}" });
            }

            return (true, null);
        }

        private readonly IDictionary<Type, string> supportedTypes = new Dictionary<Type, string>
        {
            { typeof(int), typeof(int).Name },
            { typeof(long), typeof(long).Name }
        };

        private bool TypeSupported(Type type)
        {
            if (supportedTypes.ContainsKey(type))
                return true;

            return false;
        }
    }
}
