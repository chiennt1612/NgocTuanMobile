using Auth.Repository.Interfaces;
using Auth.Services.Interfaces;
using EntityFramework.API.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Auth.Services
{
    public class ContactServices : IContactServices
    {
        private IUnitOfWork unitOfWork;
        private ILogger<ContactServices> ilogger;
        public ContactServices(IUnitOfWork unitOfWork, ILogger<ContactServices> ilogger)
        {
            this.unitOfWork = unitOfWork;
            this.ilogger = ilogger;
        }
        public async Task<Contact> AddAsync(Contact contact)
        {
            try
            {
                var a = await unitOfWork.contactRepository.AddAsync(contact);
                await unitOfWork.SaveAsync();
                ilogger.LogInformation($"Save object  Is OK");//{JsonConvert.SerializeObject(article)}
                return a;
            }
            catch (Exception ex)
            {
                ilogger.LogError($"Save object Is Fail {ex.Message}");//{JsonConvert.SerializeObject(article)}
                return default;
            }
        }

        public async Task<Contact> GetByIdAsync(long Id)
        {
            try
            {
                var a = await unitOfWork.contactRepository.GetByIdAsync(Id);
                try
                {
                    ilogger.LogInformation($"Get by id {Id.ToString()} Is {JsonConvert.SerializeObject(a)}");
                }
                catch (Exception ex)
                {
                    ilogger.LogInformation($"Get by id {Id.ToString()} Is {ex.Message}");
                }
                return a;
            }
            catch (Exception ex)
            {
                ilogger.LogError($"Get by id {Id.ToString()} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<Contact> Update(Contact order)
        {
            try
            {
                unitOfWork.contactRepository.Update(order);
                await unitOfWork.SaveAsync();
                ilogger.LogInformation($"Update Contact is OK");
                return order;
            }
            catch (Exception ex)
            {
                ilogger.LogError($"Update Contact is error {ex.Message}");
                return default;
            }
        }
    }
}
