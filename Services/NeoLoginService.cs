using NeoServer.Models;
using System.Text;
using System.Security.Cryptography;

//mysql
using MySql.Data.MySqlClient;
//redis
using StackExchange.Redis;

namespace NeoServer.Services;

public static class LoginService
{
    #region value
    private static NeoServer.System.Manager.DBManager mMysqlConnection;
    private static ConnectionMultiplexer mReadisConnection;
    private static IDatabase? mReadisDB;
    #endregion
    static LoginService()
    {
        mMysqlConnection = new System.Manager.DBManager();
        mReadisConnection = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        if (mReadisConnection.IsConnected)
            mReadisDB = mReadisConnection.GetDatabase();
    }



    public static async Task<bool> SignUp(SignUpData signUpData)
    {
        //id check
        MySqlCommand findCommand = new MySqlCommand();
        var findQuery = "SELECT user_id FROM user_info WHERE user_id=@id";

        findCommand.CommandText = findQuery;
        MySqlParameter idCheck = new MySqlParameter("@id", MySqlDbType.VarChar, 100);
        idCheck.Value = signUpData.ID;

        findCommand.Parameters.Add(idCheck);

        var findCount = await mMysqlConnection.ExecuteQuery(findCommand);
        Console.WriteLine(findCount);
        if (findCount.Rows.Count > 0)
            return false;
        else
        {
            //user id create Query
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

            int rows = await mMysqlConnection.ExecuteNonQuery(command);

            Console.WriteLine(rows);
            return true;
        }
    }

    public static async Task<Tuple<bool,Guid>> SignIn(LoginData loginData)
    {
        //ID Check Command
        MySqlCommand findCommand = new MySqlCommand();
        var findQuery = "SELECT user_id,user_password FROM user_info WHERE user_id=@id";
        //Command Set in Query
        findCommand.CommandText = findQuery;
        //Set ID Parameter
        MySqlParameter userID = new MySqlParameter("@id", MySqlDbType.VarChar, 100);
        userID.Value = loginData.ID;
        findCommand.Parameters.Add(userID);

        //Send Query       
        var result = await mMysqlConnection.ExecuteQuery(findCommand);
        if (result.Rows.Count <= 0)
            return new Tuple<bool, Guid>(false,Guid.Empty);

        if (result.Rows.Count >= 2)
        {
            return new Tuple<bool, Guid>(false,Guid.Empty);
        }

        if ((string)result.Rows[0]["user_password"] != loginData.Password)
            return new Tuple<bool, Guid>(false,Guid.Empty);

        //MYSQL처리 완료
        //Redis에서 이미 접속 중인 ID/Session인지 확인
        if (mReadisDB == null)
        {
            Console.WriteLine("Readis Connection Error");
            return new Tuple<bool, Guid>(false,Guid.Empty);
        }

        var id = (string)result.Rows[0]["user_id"];
        string str = await mReadisDB.StringGetAsync(id);
        if(str!=null)
        {
            //아이디가 존재하면 세션 끊어버리고 새로 만듬
            await mReadisDB.KeyDeleteAsync(id);
        }

        //여기서 이제 redis에 유저 세션 정보 저장후 클라한테 보냄
        Guid guid = Guid.NewGuid();
        Console.WriteLine(guid.ToString()+"   size: "+ guid.ToByteArray().Length);

        await mReadisDB.StringSetAsync(id,guid.ToByteArray());

        return new Tuple<bool, Guid>(true,guid);
    }



}