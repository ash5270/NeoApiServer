using System;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Text.Json;

namespace NeoServer.Sys.Manager;

public class DBManager : IDisposable
{
    //mysql 초기화
    public DBManager()
    {
        mDbStr = "Server=192.168.123.104; Port=3306;Database=neo_server;Uid=rupso;Pwd=vmfkek5270";
    }

    public DBManager(string database)
    {
        mDbStr = String.Format("Server=localhost; Port=3306;Database={0};Uid=rupso;Pwd=vmfkek5270", database);
    }

    //Dispose로 connect clear
    public void Dispose()
    {
 
    }

    public async Task<DataTable> ExecuteQuery(string query)
    {
        DataTable table = new DataTable();
        using (var connection = new MySqlConnection(mDbStr))
        {
            await connection.OpenAsync();
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
            {
                await adapter.FillAsync(table);
            }
        }
        return table;
    }

    public async Task<DataTable> ExecuteQuery(MySqlCommand command)
    {
        DataTable table = new DataTable();
        using (var connection = new MySqlConnection(mDbStr))
        {
            await connection.OpenAsync();

            command.Connection = connection;
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
            {
                await adapter.FillAsync(table);
            }
        }
        command.Dispose();
        return table;
    }

    public async Task<int> ExecuteNonQuery(string query)
    {
        int rows = 0;
        using (var connection = new MySqlConnection(mDbStr))
        {
            await connection.OpenAsync();
            using (var command = new MySqlCommand(query, connection))
            {
                rows = await command.ExecuteNonQueryAsync();
            }
        }

        return rows;
    }

    public async Task<int> ExecuteNonQuery(MySqlCommand command)
    {
        int rows = 0;
        using (var connection = new MySqlConnection(mDbStr))
        {
            connection.Open();
            command.Connection = connection;
            rows = await command.ExecuteNonQueryAsync();
        }

        return rows;
    }


    private string mDbStr;
}
