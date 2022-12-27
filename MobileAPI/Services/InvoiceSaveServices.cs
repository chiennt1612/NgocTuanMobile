using MobileAPI.Repository.Interfaces;
using MobileAPI.Services.Interfaces;
using EntityFramework.API.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils.Models;

namespace MobileAPI.Services
{
    public class InvoiceSaveServices : IInvoiceSaveServices
    {
        private IUnitOfWork unitOfWork;
        private ILogger<InvoiceSaveServices> ilogger;
        public InvoiceSaveServices(ILogger<InvoiceSaveServices> ilogger, IUnitOfWork unitOfWork)
        {
            this.ilogger = ilogger;
            this.unitOfWork = unitOfWork;
        }
        public async Task<bool> DeleteAsync(long id)
        {
            try
            {
                await unitOfWork.invoiceSaveRepository.DeleteAsync(id);
                await unitOfWork.SaveAsync();
                if (ilogger != null) ilogger.LogInformation($"Save object  Is OK");
                return true;
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"Save object Is Fail {ex.Message}");
                return false;
            }
        }

        public async Task<InvoiceSave> AddAsync(InvoiceSave contact)
        {
            try
            {
                var a = await unitOfWork.invoiceSaveRepository.AddAsync(contact);
                await unitOfWork.SaveAsync();
                if (ilogger != null) ilogger.LogInformation($"Save object  Is OK");
                return a;
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"Save object Is Fail {ex.Message}");
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
                if (ilogger != null) ilogger.LogInformation($"Get by id {Id.ToString()}");
                //}
                //catch (Exception ex)
                //{
                //    ilogger.LogInformation($"Get by id {Id.ToString()} Is {ex.Message}");
                //}
                return a;
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"Get by id {Id.ToString()} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<IList<InvoiceSave>> InvoceSaveGetListAsync(int? Page, int? PageSize, InvoiceSaveSearchModelA exp)
        {
            var a = await unitOfWork.invoiceSaveRepository.InvoceSaveGetListAsync(Page, PageSize, exp);
            return a;
        }

        public async Task<int> GetCountAsync(InvoiceSaveSearchModelA exp)
        {
            var a = await unitOfWork.invoiceSaveRepository.GetCountAsync(exp);
            return a;
        }
    }
}
