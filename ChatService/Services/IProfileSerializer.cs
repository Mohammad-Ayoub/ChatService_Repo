using ChatService.Web.Dtos;

namespace ChatService.Web.Services;

public interface IProfileSerializer
{
    string SerializeProfile(Profile profile);
    Profile DeserializeProfile(string serialized);
}