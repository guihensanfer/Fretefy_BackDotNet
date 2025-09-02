using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fretefy.Test.Domain.Interfaces.Repositories
{
    public interface IRegiaoRepository
    {
        // Insert
        Task AddAsync(Regiao regiao);

        // Update
        void Update(Regiao regiao);

        // Delete
        void Remove(Regiao regiao);

        // GET
        Task<bool> ExistsByNomeAsync(string nome);        
        Task<Regiao> GetByIdAsync(Guid regiaoId);
        Task<List<Regiao>> ListAsync(Paginacao paginacao, string nome = null, bool? ativo = true);

        // Atualiza a transação no banco
        Task SaveChangesAsync();
    }
}
