﻿using Auth.Models;
using Auth.Repository.Interfaces;
using EntityFramework.API.DBContext;
using EntityFramework.API.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Auth.Repository
{
    public class InvoiceSaveRepository : GenericRepository<InvoiceSave, long>, IInvoiceSaveRepository
    {
        private readonly AppDbContext _context;
        public InvoiceSaveRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
            : base(dbContext, contextAccessor)
        {
            _context = dbContext;
        }

        public async Task<IList<InvoiceSave>> InvoceSaveGetListAsync(int? Page, int? PageSize, SearchDateModel exp)
        {
            await Task.Delay(0);
            int _page = Page.HasValue ? Page.Value : 1;
            int _pagesize = PageSize.HasValue ? PageSize.Value : 10;

            if (!exp.FromDate.HasValue) exp.FromDate = new DateTime(1900, 1, 1);
            if (!exp.ToDate.HasValue) exp.ToDate = new DateTime(2900, 12, 31);
            Expression<Func<InvoiceSave, bool>> expression = u => (
                    (u.InvDate >= exp.FromDate.Value) && 
                    (u.InvDate <= exp.ToDate.Value));
            return _context.InvoiceSaves
                            .Where(expression)
                            .OrderByDescending(u => u.Id)
                            .Skip((_page - 1) * _pagesize)
                            .Take(_pagesize)
                            .ToList();
        }
    }
}
