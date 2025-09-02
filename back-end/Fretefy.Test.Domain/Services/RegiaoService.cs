using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces;
using Fretefy.Test.Domain.Interfaces.Repositories;
using Fretefy.Test.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fretefy.Test.Domain.Services
{
    public class RegiaoService : IRegiaoService
    {
        private readonly IRegiaoRepository _regiaoRepository;
        private readonly IRegiaoCidadeRepository _regiaoCidadeRepository;
        private readonly ICidadeRepository _cidadeRepository;
        private readonly HttpClient _httpClient;


        public RegiaoService(IRegiaoRepository regiaoRepository, IRegiaoCidadeRepository regiaoCidadeRepository, ICidadeRepository cidadeRepository, HttpClient httpClient)
        {
            _regiaoRepository = regiaoRepository;
            _regiaoCidadeRepository = regiaoCidadeRepository;
            _cidadeRepository = cidadeRepository;
            _httpClient = httpClient;
        }

        public async Task<RegiaoDTO> AddRegiaoAsync(RegiaoCreateDTO regiao)
        {
            if (regiao == null)
                throw new InvalidOperationException("Objeto Região inválido.");

            if (string.IsNullOrWhiteSpace(regiao.Nome))
                throw new InvalidOperationException("Região sem nome.");

            if (regiao.CidadesIdsVinculadas == null || regiao.CidadesIdsVinculadas.Length <= 0)
                throw new InvalidOperationException("Nenhuma cidade vinculada. Informe ao menos uma cidade a esta região.");

            var todasCidadesExistem = await _cidadeRepository.AllExists(regiao.CidadesIdsVinculadas);

            if (!todasCidadesExistem)
                throw new InvalidOperationException("Uma ou mais cidades não estão disponíveis.");

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

        public async Task<RegiaoDTO> AddRegiaoByCEPAsync(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                throw new ArgumentNullException("CEP deve ser fornecido.");

            // Remove caracteres inválidos (apenas números no CEP)
            cep = new string(cep.Where(char.IsDigit).ToArray());

            if (cep.Length != 8)
                throw new ArgumentException("CEP deve conter 8 dígitos numéricos.", nameof(cep));

            var url = $"https://viacep.com.br/ws/{cep}/json/";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Erro ao consultar o CEP {cep}. Status: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();

            // Deserializa o JSON do ViaCEP
            var viaCep = JsonConvert.DeserializeObject<ViaCepResponse>(content);

            if (viaCep == null || !string.IsNullOrEmpty(viaCep.Erro))
                throw new Exception($"CEP {cep} não encontrado.");

            var cidadesRelacionadas = _cidadeRepository.ListByUf(viaCep.Uf);

            if(cidadesRelacionadas == null)
                throw new Exception($"Não encontrado cidades com UF {viaCep.Uf}");

            var cidadeIdealId = cidadesRelacionadas
                .Where(c => string.Equals(c.Nome, viaCep.Localidade, StringComparison.OrdinalIgnoreCase))
                ?.Select(x => x.Id);

            if (cidadeIdealId == null || !cidadeIdealId.Any())
                throw new Exception($"Não encontrado cidade {viaCep.Localidade}");

            // Monta o DTO
                var regiaoDto = new RegiaoCreateDTO
            {
                Nome = viaCep.regiao ?? ($"{viaCep.Localidade} - {viaCep.Uf}"),
                Ativo = true,
                CidadesIdsVinculadas = cidadeIdealId.ToArray(),
            };
        
            // Adiciona e retorna a região com as cidades encontradas
            return await AddRegiaoAsync(regiaoDto);
        }

        public async Task AddRegiaoCidadeVinculosAsync(Guid regiaoId, Guid[] cidadeIds)
        {            
            if (cidadeIds == null || cidadeIds.Length <= 0)
                throw new InvalidOperationException("Informe ao menos uma cidade.");

            var idsRepetidos = cidadeIds.GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            if (idsRepetidos.Any())
                throw new InvalidOperationException($"Id cidade {idsRepetidos.First()} foi definido mais de uma vez.");

            var todasCidadesExistem = await _cidadeRepository.AllExists(cidadeIds);

            if (!todasCidadesExistem)
                throw new InvalidOperationException("Uma ou mais cidades não estão disponíveis.");            

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

        public async Task<RegioesDTO> ListRegioesAsync(Paginacao paginacao, string nome = null, bool? ativo = true)
        {
            var regioes = await _regiaoRepository.ListAsync(paginacao, nome, ativo);
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
