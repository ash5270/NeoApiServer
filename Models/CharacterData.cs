namespace NeoServer.Models;

//캐릭터 정보(등록,수정,삭제)
public class CharacterData
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string? Name { get; set; }
    public int Level { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public int ChannelId { get; set; }
    public int CharId { get; set; }
    public int WeaponId { get; set; }
    public int EXP {get;set;}
    public int HP {get;set;}
}

//캐릭터 검색시 사용
public class RegisterCharacterData
{
    public int Level { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public string CharId { get; set; }
    public string WeaponId { get; set; }
}