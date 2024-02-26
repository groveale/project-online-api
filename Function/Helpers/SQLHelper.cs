using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;

public class SqlHelper
{
    private readonly string _connectionString;
    private readonly Dictionary<string, string[]> _compositeKeys;

    public SqlHelper(string connectionString, Dictionary<string, string[]> compositeKeys)
    {
        _connectionString = connectionString;
        _compositeKeys = compositeKeys;
    }

    public void AddObjectToTable(JObject JSONdata, string tableName)
    {
        List<string> columnNames = new List<string>();
        List<string> columnValues = new List<string>();

        foreach (var property in JSONdata.Properties())
        {
            columnNames.Add(property.Name);
            if (property.Value.Type == JTokenType.Date)
            {
                columnValues.Add(((DateTime)property.Value).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                columnValues.Add(property.Value.ToString());
            }
        }

        // Generate the SQL query dynamically
        // string columns = string.Join(", ", JSONdata.GetType().GetProperties().Select(p => p.Name));
        // string values = string.Join(", ", JSONdata.GetType().GetProperties().Select(p => $"@{p.Name}"));
        string columns = string.Join(", ", columnNames);
        string values = string.Join(", ", columnValues.Select(v => $"'{v.Replace("'", "''")}'")); // wrap values in single quotes and escape any apostrophes

        // Insert is the better approach, as data added to the prestaging table will be merged into the main tables anyway
        //string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";


        // Merge query will overwrite existing data if the primary key already exists
        //string[] keyColumnNames = { "column1", "column2" }; // Example composite key columns
        string[] keyColumnNames = _compositeKeys[tableName];

        string mergeQuery = $@"
            MERGE INTO {tableName} AS target
            USING (VALUES ({values})) AS source ({columns})
            ON {string.Join(" AND ", keyColumnNames.Select(name => $"target.{name} = source.{name}"))}

            WHEN MATCHED THEN
                UPDATE SET {string.Join(", ", columnNames.Select((name, index) => $"target.{name} = source.{name}"))}

            WHEN NOT MATCHED THEN
                INSERT ({columns}) VALUES ({values});
        ";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(mergeQuery, connection))
            {
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} rows affected.");
            }
        }
    }
}
