using Microsoft.AspNetCore.Mvc;
using NeoServer.Models;
using NeoServer.Services;
using System.Text.Json;
using MySql.Data.MySqlClient;
using System.Text.Json.Serialization;

namespace NeoServer.Controlers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    public LoginController()
    {
        db = new System.Manager.DBManager();
    }

    public static void PrintByteArray(byte[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Console.Write($"{array[i]:X2}");
            if ((i % 4) == 3) Console.Write(" ");
        }
        Console.WriteLine();
    }

    [HttpGet()]
    [Route("{id}/{hash}")]
    public IActionResult GetValue(string id, string hash)
    {
        Console.WriteLine(hash);
        return Ok(hash);
    }

    [HttpGet]
    public ActionResult<List<LoginData>> GetId()
    {
        if (LoginService.GetAll().Count <= 0)
            return NoContent();
        return LoginService.GetAll();
    }

    //USER DATA 생성
    [HttpPost]
    [Route("SignIn")]
    public async Task<IActionResult> SignIn(LoginData loginData)
    {
        //id check
        MySqlCommand findCommand=new MySqlCommand();
        var findQuery= "SELECT user_id FROM user_info WHERE user_id=@id";
        
        findCommand.CommandText=findQuery;
        MySqlParameter idCheck =new MySqlParameter("@id",MySqlDbType.VarChar,100);
        idCheck.Value=loginData.ID;

        findCommand.Parameters.Add(idCheck);

        var findCount= await db.ExecuteQuery(findCommand);
        Console.WriteLine(findCount);
        if(findCount.Rows.Count>0)
            return NotFound();
        else
        {
            //user id create Query
            MySqlCommand command=new MySqlCommand();
            var query ="INSERT INTO user_info (user_name,user_id,user_email,user_password) VALUES (@name,@id,@email,@password)";
            command.CommandText= query;

            MySqlParameter userName= new MySqlParameter("@name",MySqlDbType.VarChar,50);
            userName.Value= loginData.Name;
            command.Parameters.Add(userName);

            MySqlParameter userId= new MySqlParameter("@id",MySqlDbType.VarChar,100);
            userId.Value= loginData.ID;
            command.Parameters.Add(userId);

            MySqlParameter userEmail= new MySqlParameter("@email",MySqlDbType.VarChar,100);
            userEmail.Value=loginData.Email;
            command.Parameters.Add(userEmail);

            MySqlParameter userPassword= new MySqlParameter("@password",MySqlDbType.VarChar,255);
            userPassword.Value=loginData.Password;
            command.Parameters.Add(userPassword);
            
            int rows = await db.ExecuteNonQuery(command);
            
            Console.WriteLine(rows);
            return Ok();
        }
       
        // var login = new { ID = loginData.ID, Password = loginData.Password };
        // if (LoginService.GetAll().Count <= 0)
        // {
        //     Console.WriteLine(login);
        //     return NotFound();
        // }

        // //임시로 만든 로그인
        // foreach (var user in LoginService.GetAll())
        // {
        //     if (user.ID == loginData.ID && user.Password == loginData.Password)
        //     {
        //         string jsonString = JsonSerializer.Serialize<LoginData>(user);                
        //         //Console.WriteLine($"{user.ID} : {loginData.ID} // {user.Password} : {loginData.Password} // ");
        //         //PrintByteArray(user.HashID);
        //         Console.WriteLine(jsonString);
        //         return Ok(jsonString);
        //     }

        // }
    }

    [HttpPost]
    [Route("SignUp")]
    public IActionResult Create(LoginData loginData)
    {
        LoginService.Add(loginData);
        if (LoginService.GetAll().Count <= 0)
        {
            Console.WriteLine(loginData);
            return NotFound();
        }
        else
        {
            return CreatedAtAction(nameof(GetValue), new { id = loginData.ID }, "");
        }
        //return CreatedAtAction(nameof(GetValue),new {id=loginData.ID},loginData);
    }




    #region value
    NeoServer.System.Manager.DBManager db;
    #endregion
}