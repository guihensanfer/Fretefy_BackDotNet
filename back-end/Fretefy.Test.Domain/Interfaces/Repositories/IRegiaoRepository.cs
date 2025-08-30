using Fretefy.Test.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fretefy.Test.Domain.Interfaces.Repositories
{
    public interface IRegiaoRepository
    {        
        IQueryable<Regiao> List();
        Regiao ListById(Guid regiaoId);

        /// <summary>
        /// Obtém Regiões a partir do Like em nome, por padrão trás somente ativas.
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="ativo">Status da região, informe null para trazer ambos os resultados.</param>
        /// <returns></returns>
        IEnumerable<Regiao> ListByNome(string nome, bool? ativo = true);
    }
}
