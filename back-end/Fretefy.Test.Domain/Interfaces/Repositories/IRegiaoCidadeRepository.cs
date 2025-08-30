using Fretefy.Test.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fretefy.Test.Domain.Interfaces.Repositories
{
    public interface IRegiaoCidadeRepository
    {
        IQueryable<RegiaoCidade> List();

        /// <summary>
        /// Listará regiões para uma cidade especifica, ideal para filtro em tela.
        /// </summary>
        /// <param name="cidadeId"></param>
        /// <returns></returns>
        IEnumerable<RegiaoCidade> ListByCidadeId(Guid cidadeId);        
        
        IEnumerable<RegiaoCidade> ListByRegiaoId(Guid regiaoId);        
    }
}
