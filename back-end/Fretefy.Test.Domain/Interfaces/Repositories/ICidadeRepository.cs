using Fretefy.Test.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fretefy.Test.Domain.Interfaces.Repositories
{
    public interface ICidadeRepository
    {
        IQueryable<Cidade> List();
        IEnumerable<Cidade> ListByUf(string uf);
        IEnumerable<Cidade> Query(string terms);

        /// <summary>
        /// Verifica a existencia das cidades pelo id.
        /// </summary>
        /// <param name="cidadeIds"></param>
        /// <returns></returns>
        Task<bool> AllExists(Guid[] cidadeIds);
    }
}
