using Auth.Models;
using Auth.Repository.Interfaces;
using Auth.Services.Interfaces;
using EntityFramework.API.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Services
{
    public class InvoiceSaveServices : IInvoiceSaveServices
    {
        private IUnitOfWork unitOfWork;
        private ILogger<InvoiceSaveServices> ilogger;
        public InvoiceSaveServices( ILogger<InvoiceSaveServices> ilogger, IUnitOfWork unitOfWork)
        {
            this.ilogger = ilogger;
            this.unitOfWork = unitOfWork;
        }
        public async Task<InvoiceSave> AddAsync(InvoiceSave contact)
        {
            try
            {
                var a = await unitOfWork.invoiceSaveRepository.AddAsync(contact);
                await unitOfWork.SaveAsync();
                ilogger.LogInformation($"Save object  Is OK");
                return a;
            }
            catch (Exception ex)
            {
                ilogger.LogError($"Save object Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<InvoiceSave> GetByIdAsync(long Id)
        {
            try
            {
                var a = await unitOfWork.invoiceSaveRepository.GetByIdAsync(Id);
                //try
                //{
                //    ilogger.LogInformation($"Get by id {Id.ToString()} Is {a.Fullname}");
                //}
                //catch (Exception ex)
                //{
                //    ilogger.LogInformation($"Get by id {Id.ToString()} Is {ex.Message}");
                //}
                return a;
            }
            catch (Exception ex)
            {
                ilogger.LogError($"Get by id {Id.ToString()} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<IList<InvoiceSave>> InvoceSaveGetListAsync(int? Page, int? PageSize, SearchDateModel exp)
        {
            var a = await unitOfWork.invoiceSaveRepository.InvoceSaveGetListAsync(Page, PageSize, exp);
            return a;
        }
    }
}
