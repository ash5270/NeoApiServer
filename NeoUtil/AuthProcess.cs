//자체 auth 인증
using System.Web;
using System.Collections.Specialized;
using StackExchange.Redis;
using System.Text.Encodings;

namespace NeoServer.Util;

public class AuthProcess
{
    //이 친구가 해야할일 
    //header에 uuid와 id 데이터를 가지고 
    //레디즈 서버에 있는지 확인

    private ConnectionMultiplexer mReadisConnection;
    private IDatabase? mReadisDB;
    private IHeaderDictionary mHeader;

    public AuthProcess(IHeaderDictionary coll )
    {
        mReadisConnection = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        if (mReadisConnection.IsConnected)
            mReadisDB = mReadisConnection.GetDatabase();

       mHeader= coll;
    }   

    public async Task<bool> Check()
    {
        if(!mHeader.TryGetValue("Auth-ID", out var id))
            return false;
        if(!mHeader.TryGetValue("Auth-UUID",out var uuid))
            return false;

        byte[] bytes= await mReadisDB.StringGetAsync(id.ToString());
        if(bytes==null||bytes.Length<=0)
            return false;
        var bytesStr= Convert.ToBase64String(bytes);
        if(bytesStr==uuid.ToString())
            return true;
        return false;
    }

    
}


