using ChatService.Web.Dtos;


namespace ChatService.Web.Services;

public interface ICreateProfilePublisher
{
    Task Send(Profile profile);
}
