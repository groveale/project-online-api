using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;

public class SqlHelper
{
    private readonly string _connectionString;

    public SqlHelper(string connectionString)
    {
        _connectionString = connectionString;
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
        string values = string.Join(", ", columnValues.Select(v => $"'{v}'")); // wrap values in single quotes

        //string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";


        // Merge query requires a primary key to be first in the column arrary
        string mergeQuery = $@"
            MERGE INTO {tableName} AS target
            USING (VALUES ({values})) AS source ({columns})
            ON target.{columnNames[0]} = source.{columnNames[0]}

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
