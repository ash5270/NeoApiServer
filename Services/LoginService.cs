using NeoServer.Models;
using System.Text;
using System.Security.Cryptography;

//mysql
using MySql.Data.MySqlClient;
//redis
using StackExchange.Redis;
using NeoServer.Util;
namespace NeoServer.Services;

public static class LoginService
{
    #region value
    private static Sys.Manager.DBManager mMysqlConnection;
    private static ConnectionMultiplexer mReadisConnection;
    private static IDatabase? mReadisDB;

    private static DataValidator mValidator;
    #endregion
    static LoginService()
    {
        mMysqlConnection = new Sys.Manager.DBManager();
        mReadisConnection = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        if (mReadisConnection.IsConnected)
            mReadisDB = mReadisConnection.GetDatabase();

        mValidator=new DataValidator();
    }

    public static async Task<Tuple<bool,Guid>> SignIn(LoginData loginData)
    {
        if(!mValidator.LoginUserValidator(loginData))
            return new Tuple<bool, Guid>(false,Guid.Empty);
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

        if ((string)result.Rows[0][columnName: "user_password"] != loginData.Password)
            return new Tuple<bool, Guid>(false,Guid.Empty);

        var id = (string)result.Rows[0]["user_id"];
        var guid = await CreateSession(id);

        return new Tuple<bool, Guid>(true,guid);
    }
    

    public static async Task<Guid> CreateSession(string id)
    {
        //MYSQL처리 완료
        //Redis에서 이미 접속 중인 ID/Session인지 확인
        if (mReadisDB == null)
        {
            Console.WriteLine("Readis Connection Error");
            return Guid.Empty;
        }

        string? str = await mReadisDB.StringGetAsync(id);
        if(str!=null)
        {
            //아이디가 존재하면 세션 끊어버리고 새로 만듬
            await mReadisDB.KeyDeleteAsync(id);
        }

        //여기서 이제 redis에 유저 세션 정보 저장후 클라한테 보냄
        Guid guid = Guid.NewGuid();
        Console.WriteLine(guid.ToString()+"   size: "+ guid.ToByteArray().Length);

        await mReadisDB.StringSetAsync(id,guid.ToByteArray());

        return guid;
    }  
} 