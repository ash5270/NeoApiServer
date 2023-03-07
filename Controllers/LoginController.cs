using Microsoft.AspNetCore.Mvc;
using NeoServer.Models;
using NeoServer.Services;

//json
using System.Text.Json;
using System.Text.Json.Serialization;


namespace NeoServer.Controlers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    [HttpGet()]
    [Route("{id}/{hash}")]
    public IActionResult GetValue(string id, string hash)
    {
        Console.WriteLine(hash);
        return Ok(hash);
    }


    //USER DATA 생성
    [HttpPost]
    [Route("SignUp")]
    public async Task<IActionResult> SignUp(SignUpData signUpData)
    {
        var result = await LoginService.SignUp(signUpData);
        if (!result)
            return NoContent();
        else
            return Ok();
    }

    [HttpPost]
    [Route("SignIn")]
    public async Task<ActionResult<byte[]>> SignIn(LoginData loginData)
    {
        var result = await LoginService.SignIn(loginData);
        if (!result.Item1)
        {
            Console.WriteLine("login failed");
            return NoContent();
        }
        //Convert.ToBase64String를 안해도 데이터는 성공적으로 보내짐
        var data = new Dictionary<string, object>{
        {"uuid",Convert.ToBase64String(result.Item2.ToByteArray())}
    };
        string json = JsonSerializer.Serialize(data);
        return Ok(json);
    }
}