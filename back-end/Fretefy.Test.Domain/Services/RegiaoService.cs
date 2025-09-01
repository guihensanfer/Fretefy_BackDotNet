using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces;
using Fretefy.Test.Domain.Interfaces.Repositories;
using Fretefy.Test.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fretefy.Test.Domain.Services
{
    public class RegiaoService : IRegiaoService
    {
        private readonly IRegiaoRepository _regiaoRepository;
        private readonly IRegiaoCidadeRepository _regiaoCidadeRepository;

        public RegiaoService()
        {
        }

        public RegiaoService(IRegiaoRepository regiaoRepository, IRegiaoCidadeRepository regiaoCidadeRepository)
        {
            _regiaoRepository = regiaoRepository;
            _regiaoCidadeRepository = regiaoCidadeRepository;
        }

        public async Task<RegiaoDTO> AddRegiaoAsync(RegiaoCreateDTO regiao)
        {
            if (regiao == null)
                throw new InvalidOperationException("Objeto Região inválido.");

            if (string.IsNullOrWhiteSpace(regiao.Nome))
                throw new InvalidOperationException("Região sem nome.");

            if (regiao.CidadesIdsVinculadas == null || regiao.CidadesIdsVinculadas.Length <= 0)
                throw new InvalidOperationException("Nenhuma cidade vinculada. Informe ao menos uma cidade a esta região.");

            regiao.Nome = regiao.Nome.Trim(); // Limpa os espaços para evitar erros de digitação

            var exists = await _regiaoRepository.ExistsByNomeAsync(regiao.Nome);

            if (exists)
                throw new InvalidOperationException("Região já existente.");

            // Regiao

            var regiaoObj = regiao.GetRegiaoFromSelf();

            await _regiaoRepository.AddAsync(regiaoObj);
            // Salva a região para receber os ids correspondente do banco
            await _regiaoRepository.SaveChangesAsync();

            // Id salvo no banco na transação
            Guid regiaoScopeId = regiaoObj.Id;

            // Região cidade

            // Percorre os ids para criar um insert em massa            
            await _regiaoCidadeRepository.AddRangeAsync(regiao.GetRegiaoCidadeFromSelf(regiaoScopeId));
            await _regiaoCidadeRepository.SaveChangesAsync();

            return await GetRegiaoByIdAsync(regiaoScopeId);
        }

        public async Task AddRegiaoCidadeVinculosAsync(Guid regiaoId, Guid[] cidadeIds)
        {
            if (cidadeIds == null || cidadeIds.Length <= 0)
                throw new InvalidOperationException("Informe ao menos uma cidade.");

            // Primeiro remove os vinculos existentes
                RemoveRegiaoCidadeVinculos(regiaoId);

            // Percorre os ids para criar um insert em massa
            List<RegiaoCidade> vinculos = new List<RegiaoCidade>();
            vinculos.AddRange(cidadeIds.Select(c => new RegiaoCidade(regiaoId, c)));
            await _regiaoCidadeRepository.AddRangeAsync(vinculos);

            await _regiaoCidadeRepository.SaveChangesAsync();
        }

        public async Task<RegiaoDTO> GetRegiaoByIdAsync(Guid regiaoId)
        {
            var regiao = await _regiaoRepository.GetByIdAsync(regiaoId);

            if(regiao == null)
                throw new InvalidOperationException("Região não encontrada");            

            RegiaoDTO regiaoDTO = new RegiaoDTO();
            regiaoDTO.Regiao = regiao;
            regiaoDTO.CidadesVinculadas = await _regiaoCidadeRepository.ListByRegiaoIdAsync(regiaoId);

            return regiaoDTO;
        }

        public async Task<RegioesDTO> ListRegioesAsync(Paginacao paginacao, bool? ativo = true)
        {
            var regioes = await _regiaoRepository.ListAsync(paginacao, ativo);
            RegioesDTO regioesDTO = new RegioesDTO();            

            foreach (var regiao in regioes)
            {
                RegiaoDTO regiaoDTO = new RegiaoDTO();
                regiaoDTO.Regiao = regiao;
                regiaoDTO.CidadesVinculadas = await _regiaoCidadeRepository.ListByRegiaoIdAsync(regiao.Id);

                regioesDTO.Data.Add(regiaoDTO);
            }                        

            return regioesDTO;
        }

        public async Task<RegioesDTO> ListRegioesByNomeAsync(Paginacao paginacao, string nome, bool? ativo = true)
        {
            var regioes = await _regiaoRepository.ListByNomeAsync(paginacao, nome, ativo);
            RegioesDTO regioesDTO = new RegioesDTO();

            foreach (var regiao in regioes)
            {
                RegiaoDTO regiaoDTO = new RegiaoDTO();
                regiaoDTO.Regiao = regiao;
                regiaoDTO.CidadesVinculadas = await _regiaoCidadeRepository.ListByRegiaoIdAsync(regiao.Id);

                regioesDTO.Data.Add(regiaoDTO);
            }                        

            return regioesDTO;
        }

        public void RemoveRegiao(Regiao regiao)
        {
            // Remove os vinculos
            _regiaoCidadeRepository.Remove(regiao.Id);
            // Remove a região em si
            _regiaoRepository.Remove(regiao);

            _regiaoCidadeRepository.SaveChangesAsync();
            _regiaoRepository.SaveChangesAsync();
        }

        public void RemoveRegiaoCidadeVinculos(Guid regiaoId)
        {
            _regiaoCidadeRepository.Remove(regiaoId);
            _regiaoCidadeRepository.SaveChangesAsync();
        }

        public void UpdateRegiao(Regiao regiao)
        {
            _regiaoRepository.Update(regiao);
            _regiaoRepository.SaveChangesAsync();
        }
    }
}
