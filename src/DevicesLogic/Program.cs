// See https://aka.ms/new-console-template for more information
using Microsoft.Data.SqlClient;

string connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True";

using var connection = new SqlConnection(connectionString);
connection.Open();

var command = new SqlCommand("SELECT * FROM EmbeddedDevice", connection);
var reader = command.ExecuteReader();

while (reader.Read())
{
    Console.WriteLine($"{reader["Id"]}, {reader["Name"]}, {reader["IpAddress"]}");
}