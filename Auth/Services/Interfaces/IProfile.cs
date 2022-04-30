using Auth.Models;
using System.Threading.Tasks;
using Utils.Models;

namespace Auth.Services.Interfaces
{
    public interface IProfile
    {
        Task<ResponseOK> SetProfile(ProfileInputModel inv);
        Task<ResponseOK> GetProfile();
    }
}
