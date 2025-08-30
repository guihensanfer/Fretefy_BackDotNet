using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Fretefy.Test.Infra.EntityFramework.Repositories
{
    public class RegiaoRepository : IRegiaoRepository
    {
        private DbSet<Regiao> _dbSet;

        public RegiaoRepository(DbContext dbContext)
        {
            _dbSet = dbContext.Set<Regiao>();
        }

        public IQueryable<Regiao> List()
        {
            return _dbSet.AsQueryable();
        }

        public IEnumerable<Regiao> ListByNome(string nome, bool? ativo = true)
        {
            IQueryable<Regiao> query = _dbSet.AsQueryable();

            if (ativo.HasValue)
            {
                query = query.Where(w => w.Ativo == ativo.Value);
            }

            if (!string.IsNullOrWhiteSpace(nome))
            {
                query = query.Where(w => EF.Functions.Like(w.Nome, $"%{nome}%"));
            }

            return query.ToList();
        }

        Regiao IRegiaoRepository.ListById(Guid regiaoId)
        {
            return _dbSet.FirstOrDefault(w => w.Id == regiaoId);
        }
    }
}
