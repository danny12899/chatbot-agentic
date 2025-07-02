using System.Data;
using System.Text;
using chatbot_agentic.Util;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace chatbot_agentic.Agents
{
    public class SqlAgent
    {
        private readonly AppSettings _appSettings;
        private readonly SqlConnection _connection;
        private readonly StringBuilder _instructions =  new StringBuilder();

        public SqlAgent(IOptions<AppSettings> appSettings) {
            _connection = new SqlConnection(_appSettings?.ConnectionString);
        }

        public ChatCompletionAgent GetAgent(Kernel kernel, string llmName)
        {
            _instructions.AppendLine($"I have a database with these tables and columns");
            _instructions.AppendLine(GetDatabaseInfo());
            _instructions.AppendLine();

            return new()
            {
                Name = "SqlAgent",
                Description = "Agent to invoke to give a sql query to answer the user's question about the database",
                Kernel = kernel
            };
        }

        private string GetDatabaseInfo()
        {
            var sb = new StringBuilder();
            var tableNames = new List<string>();

            _connection.Open();
            using var getTableNamesCmd = _connection.CreateCommand();
            getTableNamesCmd.CommandText = $"SELECT name FROM sys.tables";
            using var reader = getTableNamesCmd.ExecuteReader();
            while (reader.Read())
                tableNames.Add(reader.GetString(0));
            reader.Close();

            foreach (var tableName in tableNames)
            {
                using var getSchemaCmd = _connection.CreateCommand();
                getSchemaCmd.CommandText = $"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
                using var schemaReader = getSchemaCmd.ExecuteReader();
                List<string> schemas = [];
                while (schemaReader.Read())
                    schemas.Add($"{schemaReader.GetString(0)} [{schemaReader.GetString(1)}]");
                sb.AppendLine($"Table {tableName} has Schema: Columns: {string.Join(", ", schemas)}");
            }

            // add table data

            return sb.ToString();
        }

        private IEnumerable<string> GetTableData(string column, string table)
        {
            using var getTableCmd = _connection.CreateCommand();
            getTableCmd.CommandText = $"SELECT DISTINCT [{column}] FROM {table}";
            using var reader = getTableCmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            return dt.Rows.Cast<DataRow>().Select(r => string.Join(", ", r.ItemArray));
        }
    }
}
