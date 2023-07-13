using Microsoft.AspNetCore.Mvc;
using NeoServer.Models;
using NeoServer.Services;

//json
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

using NeoServer.Util;

namespace NeoServer.Controlers;

[ApiController]
[Route("[controller]")]
public class CharacterController : ControllerBase
{
    private AuthProcess mAuth;
    [HttpPost]
    public async Task<IActionResult> CreateCharacter(CharacterData data)
    {
        mAuth= new AuthProcess(Request.Headers);
        if(!await mAuth.Check())
            return NotFound();
        //캐릭터 생성 
        var result = await CharacterService.CreateCharacter(data);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCharacter(int id)
    {
        Console.WriteLine("Delete Start");
        mAuth= new AuthProcess(Request.Headers);
        if(!await mAuth.Check())
            return NotFound();
        //캐릭터정보삭제 
        Console.WriteLine("Delete Success");
        var result  = await CharacterService.DeleteCharacter(id);
        return Ok();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateCharacter(int id, CharacterData data)
    {
        mAuth= new AuthProcess(Request.Headers);
        if(!await mAuth.Check())
            return NotFound();
        Console.WriteLine("Update Success");
        var result = await CharacterService.UpdateCharacter(id,data);
        if(result)
            return Ok();
        else
            return NotFound();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAllChracter(string id)
    {
        mAuth= new AuthProcess(Request.Headers);
        if(!await mAuth.Check())
            return NotFound();
        //캐릭터 정보 가져옴
        var data = await CharacterService.GetAllCharacter(id);
        //json으로 파싱
        string charactersJson =JsonSerializer.Serialize(data);
        JsonNode charactersNode = JsonNode.Parse(charactersJson);
        JsonNode resultNode =new JsonObject
        {
            ["array"]= charactersNode
        };      
        return Ok(resultNode);
    }

    [HttpGet("{id}/{channel}")]
    public async Task<IActionResult> GetChracters(string id,int channel)
    {
        Console.WriteLine($"URL : /Character/{id}/{channel}    HttpMethod : Get ");
        mAuth= new AuthProcess(Request.Headers);
        if(!await mAuth.Check())
            return NotFound();
        //캐릭터 정보 가져옴
        var data = await CharacterService.GetCharacters(id,channel);
        if(data==null)
        {
            Console.WriteLine("Characters Data not found ...");
            return NotFound();
        }
        //json으로 파싱
        string charactersJson =JsonSerializer.Serialize(data);
        JsonNode charactersNode = JsonNode.Parse(charactersJson);
        JsonNode resultNode =new JsonObject
        {
            ["array"]= charactersNode
        };      
        Console.WriteLine("Get Character data size"+ data.Count);
        return Ok(resultNode);
    }

    [HttpGet("{id}/{channel}/{name}")]
    public async Task<IActionResult> GetCharacter(string id,int channel, string name)
    {
        Console.WriteLine($"URL : /Character/{id}/{channel}/{name}    HttpMethod : Get ");
        mAuth= new AuthProcess(Request.Headers);
        if(!await mAuth.Check())
            return NotFound();
        //캐릭터 정보 가져옴
        var data = await CharacterService.GetCharacter(id,channel,name);
        
        if(data==null)
        {
            Console.WriteLine("Character Data not found ...");
            return NotFound();
        }

        string charactersJson =JsonSerializer.Serialize(data);
        JsonNode charactersNode = JsonNode.Parse(charactersJson);
        Console.WriteLine(charactersNode.ToJsonString());
        return Ok(charactersNode);
    }
}