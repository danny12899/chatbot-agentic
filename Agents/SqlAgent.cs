using System.Text;
using chatbot_agentic.Util;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace chatbot_agentic.Agents
{
    public class SqlAgent
    {
        private readonly AppSettings _appSettings;
        private readonly SqlConnection _connection;
        public SqlAgent(IOptions<AppSettings> appSettings) {
            _connection = new SqlConnection(_appSettings?.ConnectionString);
        }

        private async Task<string> GetDatabaseInfoAsync()
        {
            var sb = new StringBuilder();
            var tableNames = new List<string>();

            await _connection.OpenAsync();
            using var getTableNamesCmd = _connection.CreateCommand();
            getTableNamesCmd.CommandText = $"SELECT name FROM sys.tables";
            using var reader = await getTableNamesCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                tableNames.Add(reader.GetString(0));
            reader.Close();

            foreach (var tableName in tableNames)
            {
                using var getSchemaCmd = _connection.CreateCommand();
                getSchemaCmd.CommandText = $"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
                using var schemaReader = await getSchemaCmd.ExecuteReaderAsync();
                List<string> schemas = [];
                while (await schemaReader.ReadAsync())
                    schemas.Add($"{schemaReader.GetString(0)} [{schemaReader.GetString(1)}]");
                sb.AppendLine($"Table {tableName} has Schema: Columns: {string.Join(", ", schemas)}");
            }

            return sb.ToString();
        }
    }
}
