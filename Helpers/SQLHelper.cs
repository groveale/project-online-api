using System;
using System.Linq;
using Microsoft.Data.SqlClient;

public class SqlHelper
{
    private readonly string _connectionString;

    public SqlHelper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void AddObjectToTable<T>(T dynamicData, string tableName)
    {
        // Generate the SQL query dynamically
        string columns = string.Join(", ", dynamicData.GetType().GetProperties().Select(p => p.Name));
        string values = string.Join(", ", dynamicData.GetType().GetProperties().Select(p => $"@{p.Name}"));
        string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Add parameters dynamically
                foreach (var property in dynamicData.GetType().GetProperties())
                {
                    object value = property.GetValue(dynamicData) ?? DBNull.Value;
                    command.Parameters.AddWithValue($"@{property.Name}", value);
                }

                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} rows affected.");
            }
        }
    }
}
