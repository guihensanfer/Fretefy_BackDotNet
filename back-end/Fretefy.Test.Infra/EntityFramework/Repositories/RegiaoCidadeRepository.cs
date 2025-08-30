using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fretefy.Test.Infra.EntityFramework.Repositories
{
    public class RegiaoCidadeRepository : IRegiaoCidadeRepository
    {
        private DbSet<RegiaoCidade> _dbSet;

        public RegiaoCidadeRepository(DbContext dbContext)
        {
            _dbSet = dbContext.Set<RegiaoCidade>();
        }

        public IQueryable<RegiaoCidade> List()
        {
            return _dbSet.AsQueryable();
        }

        public IEnumerable<RegiaoCidade> ListByCidadeId(Guid cidadeId)
        {
            return _dbSet.Where(w => w.CidadeID == cidadeId);
        }
        
        public IEnumerable<RegiaoCidade> ListByRegiaoId(Guid regiaoId)
        {
            return _dbSet.Where(w => w.RegiaoId == regiaoId);
        }

    }
}
