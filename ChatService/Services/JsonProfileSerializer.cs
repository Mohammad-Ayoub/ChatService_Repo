using Newtonsoft.Json;
using ChatService.Web.Dtos;

namespace ChatService.Web.Services;

public class JsonProfileSerializer : IProfileSerializer
{
    public string SerializeProfile(Profile profile)
    {
        return JsonConvert.SerializeObject(profile);
    }

    public Profile DeserializeProfile(string serialized)
    {
        return JsonConvert.DeserializeObject<Profile>(serialized);
    }
}