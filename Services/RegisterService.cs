using NeoServer.Models;
using System.Text;
using System.Security.Cryptography;

//mysql
using MySql.Data.MySqlClient;
//redis
using StackExchange.Redis;
using NeoServer.Util;
namespace NeoServer.Services;


public static class RegisterService
{
    #region value
    private static Util.DataValidator mValidator;
    private static Sys.Manager.DBManager mMysqlConnection;

    #endregion


    static RegisterService()
    {
        mMysqlConnection = new Sys.Manager.DBManager();
        mValidator=new DataValidator();
    }


    public static async Task<bool> SignUp(SignUpData data)
    {
        if(!mValidator.RegisterUserValidator(data))
            return false;
        var idCheck = await IDCheck(data);
        if (!idCheck)
            return false;
        var inserCheck = await InsertData(data);
        if(!inserCheck)
            return false;
        return true;
    }

    private static async Task<bool> IDCheck(SignUpData signUpData)
    {
        MySqlCommand findCommand = new MySqlCommand();
        var findQuery = "SELECT COUNT(user_id) AS COUNT FROM user_info WHERE user_id = @id";
        findCommand.CommandText = findQuery;
        MySqlParameter idCheck = new MySqlParameter("@id", MySqlDbType.VarChar, 100);
        idCheck.Value = signUpData.ID;

        findCommand.Parameters.Add(idCheck);

        var findRows = await mMysqlConnection.ExecuteQuery(findCommand);
        var count = (Int64)findRows.Rows[0][0];
        if (count >= 1)
            return false;
        else
            return true;
    }

    private static async Task<bool> InsertData(SignUpData signUpData)
    {
        MySqlCommand command = new MySqlCommand();
        var query = "INSERT INTO user_info (user_name,user_id,user_email,user_password) VALUES (@name,@id,@email,@password)";
        command.CommandText = query;

        MySqlParameter userName = new MySqlParameter("@name", MySqlDbType.VarChar, 50);
        userName.Value = signUpData.Name;
        command.Parameters.Add(userName);

        MySqlParameter userId = new MySqlParameter("@id", MySqlDbType.VarChar, 100);
        userId.Value = signUpData.ID;
        command.Parameters.Add(userId);

        MySqlParameter userEmail = new MySqlParameter("@email", MySqlDbType.VarChar, 100);
        userEmail.Value = signUpData.Email;
        command.Parameters.Add(userEmail);

        MySqlParameter userPassword = new MySqlParameter("@password", MySqlDbType.VarChar, 255);
        userPassword.Value = signUpData.Password;
        command.Parameters.Add(userPassword);

        var rows = await mMysqlConnection.ExecuteNonQuery(command);

        return true;
    }
 






}