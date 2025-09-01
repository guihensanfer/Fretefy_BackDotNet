using System;
using System.Collections.Generic;
using System.Linq;
using Fretefy.Test.Domain.Entities;

namespace Fretefy.Test.Domain.Models
{
    public class RegiaoUpdateDTO
    {
        public Guid RegiaoId{ get; set; }

        public Guid[] CidadesIdsVinculadas { get; set; }

        /// <summary>
        /// Retorna o objeto original Região para ser utilizado para outras finalidades. Preferi padronizar aqui, caso no futuro seja adicionado novos atributos, fica fácil mapear para todo mundo.
        /// </summary>
        /// <returns>Retorna um objeto Região original.</returns>
        public Regiao GetRegiaoFromSelf()
        {
            return new Regiao()
            {
                Id = RegiaoId
            };
        }

        public List<RegiaoCidade> GetRegiaoCidadeFromSelf(Guid regiaoId)
        {
            if (CidadesIdsVinculadas == null || CidadesIdsVinculadas.Length <= 0)
                return null;

            return CidadesIdsVinculadas
                .Select(x => new RegiaoCidade(regiaoId, x))
                .ToList();
        }
    }    
    
}
