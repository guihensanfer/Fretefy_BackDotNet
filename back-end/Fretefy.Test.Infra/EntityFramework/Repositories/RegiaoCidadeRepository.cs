using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Fretefy.Test.Infra.EntityFramework.Repositories
{
    public class RegiaoCidadeRepository : IRegiaoCidadeRepository
    {
        private readonly DbSet<RegiaoCidade> _dbSet;
        private readonly DbContext _dbContext;

        public RegiaoCidadeRepository(DbContext dbContext)
        {
            _dbSet = dbContext.Set<RegiaoCidade>();
            _dbContext = dbContext;
        }

        public async Task AddRangeAsync(List<RegiaoCidade> regioesCidades)
        {
            await _dbSet.AddRangeAsync(regioesCidades);
        }

        public void Remove(Guid regiaoId)
        {
            var regioes = _dbSet.Where(x => x.RegiaoId == regiaoId).ToList();

            if(regioes.Any())            
                _dbSet.RemoveRange(regioes);
        }

        async Task<List<RegiaoCidade>> IRegiaoCidadeRepository.ListByRegiaoIdAsync(Guid regiaoId)
        {
            return await _dbSet.Where(x => x.RegiaoId == regiaoId).ToListAsync();
        }        

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public Task<bool> CheckExistsAsync(Guid regiaoId, Guid cidadeId)
        {
            return _dbSet.AnyAsync(x => x.RegiaoId == regiaoId && x.CidadeID == cidadeId);
        }

        public Task<bool> ValidCidade(Guid cidadeId)
        {
            return _dbSet.AnyAsync(x => x.CidadeID == cidadeId);
        }
    }
}
