using NeoServer.Models;
using System.Text;
using System.Security.Cryptography;

namespace NeoServer.Services;

public static class LoginService
{
    static LoginService()
    {
        users =new List<LoginData>();
    }

    public static List<LoginData> GetAll()
    {
        return users;
    }

    public static void Add(LoginData user) 
    {
        if(user.ID==null)
            return;
        var byteID = Encoding.UTF8.GetBytes(user.ID);
        SHA256 hash= SHA256.Create();
        user.HashID= hash.ComputeHash(byteID);
        users.Add(user);
    }


    private static List<LoginData> users;
}