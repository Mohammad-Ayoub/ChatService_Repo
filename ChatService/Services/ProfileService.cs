
using ChatService.Web.Dtos;
using ChatService.Web.Services;
using ChatService.Web.Storage;
namespace ChatService.Web;
public class ProfileService : IProfileService
{
    private readonly ICreateProfilePublisher _createProfilePublisher;
    private readonly IProfileStore _profileStore;

    public ProfileService(
        ICreateProfilePublisher createProfilePublisher,
        IProfileStore profileStore)
    {
        _createProfilePublisher = createProfilePublisher;
        _profileStore = profileStore;
    }

    public async Task EnqueueCreateProfile(Profile profile)
    {
        await _createProfilePublisher.Send(profile);
    }

    public Task CreateProfile(Profile profile)
    {
        return _profileStore.UpsertProfile(profile);
    }

    public Task<Profile?> GetProfile(string username)
    {
        return _profileStore.GetProfile(username);
    }

    public Task UpdateProfile(Profile profile)
    {
        return _profileStore.UpsertProfile(profile);
    }

   

    public Task<bool> isUserExist(string username)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DoesParticipantsExist(List<string> Participants)
    {
        throw new NotImplementedException();
    }
}