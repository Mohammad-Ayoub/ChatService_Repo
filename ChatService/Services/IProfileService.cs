
using ChatService.Web.Dtos;

namespace ChatService.Web.Services;
public interface IProfileService
{
    Task EnqueueCreateProfile(Profile profile);
    Task CreateProfile(Profile profile);
    Task<Profile?> GetProfile(string username);
    Task UpdateProfile(Profile profile);
    Task<Boolean>isUserExist(string username);
    Task<bool> DoesParticipantsExist(List<string> Participants);

}