using NeoServer.Models;
using System.Text;
using System.Security.Cryptography;

//mysql
using MySql.Data.MySqlClient;
//redis
using StackExchange.Redis;
using NeoServer.Util;
namespace NeoServer.Services;

public static class CharacterService
{
    private static Sys.Manager.DBManager mMysqlConnection;

    static CharacterService()
    {
        mMysqlConnection = new Sys.Manager.DBManager();
    }

    public static async Task<bool> CreateCharacter(CharacterData data)
    {
        var id = await CheckUser(data.UserId);
        if (id == -1)
            return false;
        MySqlCommand createCommand = new MySqlCommand();
        var createQuery = "INSERT INTO user_character (user_id,char_name,channel_id,char_info_id,char_weapon_id) VALUES(@id,@name,@channel,@charID,@weaponID)";
        createCommand.CommandText = createQuery;
        createCommand.Parameters.Add(new MySqlParameter("@id", (int)id));
        createCommand.Parameters.Add(new MySqlParameter("@name", data.Name));
        createCommand.Parameters.Add(new MySqlParameter("@channel", data.ChannelId));
        createCommand.Parameters.Add(new MySqlParameter("@charID", data.CharId));
        createCommand.Parameters.Add(new MySqlParameter("@weaponID", data.WeaponId));
        var result = await mMysqlConnection.ExecuteQuery(createCommand);
        return true;
    }

    public static async Task<bool> DeleteCharacter(int id)
    {
        //
        MySqlCommand command = new MySqlCommand();
        var query = "DELETE FROM user_character WHERE id = @id";
        command.CommandText = query;
        command.Parameters.Add(new MySqlParameter("@id", id));
        var result = await mMysqlConnection.ExecuteQuery(command);
        Console.WriteLine($"Delete id:{id}");
        return true;
    }

    public static async Task<bool> UpdateCharacter(int userId, CharacterData data)
    {
        MySqlCommand command = new MySqlCommand();
        var query = "UPDATE user_character set (char_exp,char_level,char_hp,char_pos_x,char_pos_y) = (@exp,@level,@hp,@pos_x,@pos_y) WHERE id = @id";
        command.CommandText =query;
        command.Parameters.Add(new MySqlParameter("@id",userId));
        command.Parameters.Add(new MySqlParameter("@exp",data.EXP));
        command.Parameters.Add(new MySqlParameter("@level",data.Level));
        command.Parameters.Add(new MySqlParameter("@hp",data.HP));
        command.Parameters.Add(new MySqlParameter("@pos_x",data.PosX));
        command.Parameters.Add(new MySqlParameter("@pos_y",data.PosY));
        var result = await mMysqlConnection.ExecuteQuery(command);
        
        return true;
    }

    private static async Task<int> CheckUser(string id)
    {
        MySqlCommand command = new MySqlCommand();
        var userIdQuery = "SELECT id FROM user_info WHERE user_id=@id";
        command.CommandText = userIdQuery;
        command.Parameters.Add(new MySqlParameter("@id", id));
        var table = await mMysqlConnection.ExecuteQuery(command);
        if (table.Rows.Count <= 0)
            return -1;
        return (int)table.Rows[0]["id"];
    }


    public static async Task<RegisterCharacterData> GetCharacter(string userId, int channelId, string characterName)
    {
        var command = new MySqlCommand();
        var query = "select character_info.char_name as \"character_code\", weapon_info.weapon_name as \"weapon_code\" ,char_pos_x,char_pos_y, char_level from user_character " +
        " INNER JOIN character_info ON character_info.id = user_character.char_info_id " +
        " INNER JOIN  weapon_info  on user_character.char_weapon_id  = weapon_info.id " +
        " where  user_character.char_name =@name AND user_character.channel_id = @channel; ";
        command.CommandText = query;
        command.Parameters.Add(new MySqlParameter("@channel", channelId));
        command.Parameters.Add(new MySqlParameter("@name", characterName));
        var table = await mMysqlConnection.ExecuteQuery(command);
        if (table.Rows.Count <= 0)
        {
            Console.WriteLine("table is not found....");
            return null;
        }
        var character = table.Rows[0]["character_code"];
        var weapn = table.Rows[0]["weapon_code"];
        var level = table.Rows[0]["char_level"];
        var pos_x = table.Rows[0]["char_pos_x"];
        var pos_y = table.Rows[0]["char_pos_y"];

        RegisterCharacterData data = new RegisterCharacterData();
        data.CharId = character.ToString();
        data.WeaponId = weapn.ToString();
        data.Level = (int)level;
        data.PosX = (Single)pos_x;
        data.PosY = (Single)pos_y;

        return data;
    }

    public static async Task<List<CharacterData>> GetCharacters(string userId, int channelId)
    {
        var id = await CheckUser(userId);
        if (id == -1)
        {
            Console.WriteLine("id is not found....");
            return null;
        }
        var command = new MySqlCommand();
        var query = "select * from user_character where user_id = @id AND channel_id = @channel";
        command.CommandText = query;
        command.Parameters.Add(new MySqlParameter("@id", id));
        command.Parameters.Add(new MySqlParameter("@channel", channelId));
        var table = await mMysqlConnection.ExecuteQuery(command);
        if (table.Rows.Count <= 0)
        {
            Console.WriteLine("id table is not found....");
            return null;
        }

        List<CharacterData> characters = new List<CharacterData>();
        foreach (System.Data.DataRow row in table.Rows)
        {
            var characterID = row["id"];
            var user = row["user_id"];
            var name = row["char_name"];
            var level = row["char_level"];
            var pos_x = row["char_pos_x"];
            var pos_y = row["char_pos_y"];
            var channel = row["channel_id"];
            var char_id = row["char_info_id"];
            var weapon_id = row["char_weapon_id"];
            var exp =row["char_exp"];
            var hp = row["char_hp"];


            CharacterData data = new CharacterData();
            data.Id = (int)characterID;
            data.UserId = user.ToString();
            data.Name = name.ToString();
            data.Level = (int)level;
            data.PosX = (Single)pos_x;
            data.PosY = (Single)pos_y;
            data.ChannelId = (int)channel;
            data.CharId = (int)char_id;
            data.WeaponId = (int)weapon_id;
            data.EXP= (int)exp;
            data.HP= (int)hp;


            characters.Add(data);
        }

        return characters;
    }

    public static async Task<List<CharacterData>> GetAllCharacter(string userId)
    {
        var id = await CheckUser(userId);
        if (id == -1)
            return null;
        var command = new MySqlCommand();
        var query = "SELECT * FROM user_character WHERE user_id = @id";
        command.CommandText = query;
        command.Parameters.Add(new MySqlParameter("@id", id));
        var table = await mMysqlConnection.ExecuteQuery(command);
        if (table.Rows.Count <= 0)
        {
            Console.WriteLine("table row count zero");
            return null;
        }

        List<CharacterData> characters = new List<CharacterData>();
        foreach (System.Data.DataRow row in table.Rows)
        {
            var characterID = row["id"];
            var user = row["user_id"];
            var name = row["char_name"];
            var level = row["char_level"];
            var pos_x = row["char_pos_x"];
            var pos_y = row["char_pos_y"];
            var channel = row["channel_id"];
            var char_id = row["char_info_id"];
            var weapon_id = row["char_weapon_id"];
            var exp =row["char_exp"];
            var hp = row["char_hp"];
            CharacterData data = new CharacterData();
            data.Id = (int)characterID;
            data.UserId = user.ToString();
            data.Name = name.ToString();
            data.Level = (int)level;
            data.PosX = (Single)pos_x;
            data.PosY = (Single)pos_y;
            data.ChannelId = (int)channel;
            data.CharId = (int)char_id;
            data.WeaponId = (int)weapon_id;

            data.EXP= (int)exp;
            data.HP= (int)hp;

            characters.Add(data);
        }
        return characters;
    }
}
