namespace Oficina.Common.Application.IServices
{
    public interface IAuthAppService
    {
        Task<string> Authenticate(string username, string password);
    }
}
