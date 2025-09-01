using Fretefy.Test.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fretefy.Test.Domain.Interfaces.Repositories
{
    public interface IRegiaoCidadeRepository
    {
        // Insert
        Task AddRangeAsync(List<RegiaoCidade> regioesCidades);

        // Delete
        void Remove(Guid regiaoId);

        Task<List<RegiaoCidade>> ListByRegiaoIdAsync(Guid regiaoId);        

        Task SaveChangesAsync();    
    }
}
