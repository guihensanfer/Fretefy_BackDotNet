using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fretefy.Test.Domain.Interfaces
{
    public interface IRegiaoService
    {
        // Insert
        Task<RegiaoDTO> AddRegiaoAsync(RegiaoCreateDTO regiao);

        // Update
        void UpdateRegiao(Regiao regiao);

        // Delete
        void RemoveRegiao(Regiao regiao);

        // GET        
        Task<RegioesDTO> ListRegioesAsync(Paginacao paginacao, bool? ativo = true);
        Task<RegioesDTO> ListRegioesByNomeAsync(Paginacao paginacao, string nome, bool? ativo = true);
        Task<RegiaoDTO> GetRegiaoByIdAsync(Guid regiaoId);

        // Regiões cidades

        Task AddRegiaoCidadeVinculosAsync(Guid regiaoId, Guid[] cidadeIds);
        /// <summary>
        /// Remove todos os vinculos.
        /// </summary>
        /// <param name="regiaoId"></param>
        void RemoveRegiaoCidadeVinculos(Guid regiaoId);        
    }
}
