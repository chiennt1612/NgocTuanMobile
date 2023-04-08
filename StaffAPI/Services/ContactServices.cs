using EntityFramework.API.Entities;
using EntityFramework.API.Entities.EntityBase;
using Microsoft.Extensions.Logging;
using StaffAPI.Repository.Interfaces;
using StaffAPI.Services.Interfaces;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StaffAPI.Services
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
                if (ilogger != null) ilogger.LogInformation($"Save object  Is OK");
                return a;
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"Save object Is Fail {ex.Message}");
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
                    if (ilogger != null) ilogger.LogInformation($"Get by id {Id.ToString()} Is {a.Fullname}");
                }
                catch (Exception ex)
                {
                    if (ilogger != null) ilogger.LogInformation($"Get by id {Id.ToString()} Is {ex.Message}");
                }
                return a;
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"Get by id {Id.ToString()} Is Fail {ex.Message}");
                return default;
            }
        }

        public async Task<Contact> Update(Contact order)
        {
            try
            {
                unitOfWork.contactRepository.Update(order);
                await unitOfWork.SaveAsync();
                if (ilogger != null) ilogger.LogInformation($"Update Contact is OK");
                return order;
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"Update Contact is error {ex.Message}");
                return default;
            }
        }

        public async Task<BaseEntityList<Contact>> GetListAsync(
            Expression<Func<Contact, bool>> expression,
            Func<Contact, object> sort, bool desc,
            int page, int pageSize)
        {
            try
            {
                var a = await unitOfWork.contactRepository.GetListAsync(expression, sort, desc, page, pageSize);
                if (ilogger != null) ilogger.LogInformation($"GetListAsync expression, sort {desc} {page} {pageSize}");
                return a;
            }
            catch (Exception ex)
            {
                if (ilogger != null) ilogger.LogError($"GetListAsync expression, sort {desc} {page} {pageSize} Is Fail {ex.Message}");
                return default;
            }
        }
    }
}
