using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces.Repositories;
using Fretefy.Test.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fretefy.Test.Infra.EntityFramework.Repositories
{
    public class RegiaoRepository : IRegiaoRepository
    {
        private readonly DbSet<Regiao> _dbSet;
        private readonly DbContext _dbContext;

        public RegiaoRepository(DbContext dbContext)
        {
            _dbSet = dbContext.Set<Regiao>();
        }

        public async Task AddAsync(Regiao regiao)
        {
            await _dbSet.AddAsync(regiao);
        }

        public Task<bool> ExistsByNomeAsync(string nome)
        {
            return _dbSet.AnyAsync(r => r.Nome == nome);
        }

        public async Task<List<Regiao>> ListAsync(Paginacao paginacao, bool? ativo = true)
        {
            IQueryable<Regiao> query = _dbSet.AsQueryable();

            // Filtro por ativo, se null então trás todos results
            if (ativo.HasValue)
                query = query.Where(r => r.Ativo == ativo.Value);

            // Retorno paginado (performance)
            return await query
                .OrderBy(r => r.Nome) // Ordena pelo nome da região
                .Skip((paginacao.Pagina - 1) * paginacao.TotalItensPorPagina)
                .Take(paginacao.TotalItensPorPagina)
                .ToListAsync();
        }

        public Task<Regiao> GetByIdAsync(Guid regiaoId)
        {
            return _dbSet.FirstOrDefaultAsync(w => w.Id == regiaoId);
        }        

        public async Task<List<Regiao>> ListByNomeAsync(Paginacao paginacao, string nome, bool? ativo = true)
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

            return await query
                .OrderBy(r => r.Nome) // Ordena pelo nome da região
                .Skip((paginacao.Pagina - 1) * paginacao.TotalItensPorPagina)
                .Take(paginacao.TotalItensPorPagina)
                .ToListAsync();
        }

        public void Remove(Regiao regiao)
        {
            _dbSet.Remove(regiao);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Update(Regiao regiao)
        {
            _dbSet.UpdateRange(regiao);
        }
    }
}
