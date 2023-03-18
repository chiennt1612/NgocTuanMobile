using StaffAPI.Models;
using System.Threading.Tasks;
using Utils.Models;

namespace StaffAPI.Services.Interfaces
{
    public interface IProfile
    {
        Task<ResponseOK> LinkInvoice(InvoiceInput inv);
        Task<ResponseOK> RemoveInvoice(InvoiceInput inv);
        Task<ResponseOK> SetProfile(ProfileInputModel inv);
        Task<ResponseOK> GetProfile(int profileType = 1);
        Task<ResponseOK> SetCompanyInfo(CompanyInfoInput inv);
        Task<ResponseOK> GetCompanyList();
        Task<ResponseOK> GetCompanyInfo(int profileType = 1);
        Task<ResponseOK> GetContractAllList(ContractInput inv);
        Task<ContractResult> GetContractList(ContractInput inv);
    }
}
