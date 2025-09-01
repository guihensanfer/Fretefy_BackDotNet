using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces;
using Fretefy.Test.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fretefy.Test.WebApi.Controllers
{
    [Route("api/regiao")]
    [ApiController]
    public class RegiaoController : ControllerBase
    {
        private readonly IRegiaoService _regiaoService;

        public RegiaoController(IRegiaoService regiaoService)
        {
            _regiaoService = regiaoService;
        }

        /// <summary>
        /// Adiciona uma nova região
        /// </summary>
        /// <param name="regiaoDTO">Objeto contendo informações da região</param>
        /// <returns>Retorna a região criada</returns>
        [HttpPost("AddRegiao")]
        public async Task<IActionResult> AddRegiao([FromBody] RegiaoCreateDTO regiaoDTO)
        {
            if (regiaoDTO == null)
                return BadRequest("Objeto Região não pode ser nulo.");

            try
            {
                var regiao = await _regiaoService.AddRegiaoAsync(regiaoDTO);
                return Ok(regiao); // 200 OK com objeto criado
            }
            catch (InvalidOperationException ex)
            {
                // Regra de negócio violada (ex: região já existe)
                return BadRequest(ex.Message); // 400 Bad Request
            }
            catch (Exception ex)
            {
                // Erros inesperados
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza vínculos de cidades a uma região
        /// </summary>
        /// <param name="regiaoDTO">Objeto contendo região e lista de cidades</param>
        /// <returns>Ok se sucesso</returns>
        [HttpPut("UpdateRegiaoCidadesVinculos")]
        public async Task<IActionResult> UpdateRegiaoCidadesVinculos([FromBody] RegiaoUpdateDTO regiaoDTO)
        {
            if (regiaoDTO == null || regiaoDTO.CidadesIdsVinculadas == null)
                return BadRequest("Objeto Região não pode ser nulo.");            
           

            try
            {
                
                await _regiaoService.AddRegiaoCidadeVinculosAsync(
                    regiaoDTO.RegiaoId,
                    regiaoDTO.CidadesIdsVinculadas
                );

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                // Regra de negócio violada (ex: região já existe)
                return BadRequest(ex.Message); // 400 Bad Request
            }
            catch (Exception ex)
            {
                // Erros inesperados
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza uma região existente
        /// </summary>
        /// <param name="regiao">Objeto região atualizado</param>
        /// <returns>Retorna a região atualizada</returns>
        [HttpPut("UpdateRegiao")]
        public IActionResult UpdateRegiao([FromBody] Regiao regiao)
        {
            if (regiao == null)
                return BadRequest("Objeto Região não pode ser nulo.");

            try
            {
                _regiaoService.UpdateRegiao(regiao);
                return Ok(regiao);
            }
            catch (InvalidOperationException ex)
            {
                // Regra de negócio violada (ex: região já existe)
                return BadRequest(ex.Message); // 400 Bad Request
            }
            catch (Exception ex)
            {
                // Erros inesperados
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Remove uma região
        /// </summary>
        /// <param name="regiao">Objeto região a ser removido</param>
        /// <returns>Retorna a região removida</returns>
        [HttpDelete("DeleteRegiao")]
        public async Task<IActionResult> DeleteRegiao([FromQuery] string regiaoId)
        {
            if (string.IsNullOrWhiteSpace(regiaoId))
                return BadRequest("Informe o regionId");
                
            if(!Guid.TryParse(regiaoId, out Guid _regiaoId))
                return BadRequest("Não foi possível identificar o regionId informado.");

            try
            {
                var regiao = await _regiaoService.GetRegiaoByIdAsync(_regiaoId);

                if (regiao != null)
                {
                    _regiaoService.RemoveRegiao(regiao.Regiao);

                    return Ok(regiao);
                }
                else
                {
                    return NotFound();
                }           
            }
            catch (InvalidOperationException ex)
            {
                // Regra de negócio violada (ex: região já existe)
                return BadRequest(ex.Message); // 400 Bad Request
            }
            catch (Exception ex)
            {
                // Erros inesperados
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
        

        /// <summary>
        /// Lista todas as regiões
        /// </summary>
        /// <param name="ativo">Filtra apenas regiões ativas (opcional)</param>
        /// <param name="page">Número da página</param>
        /// <param name="itemsPerPage">Itens por página</param>
        /// <returns>Lista paginada de regiões</returns>
        [HttpGet]
        public async Task<IActionResult> ListRegioes([FromQuery] bool? ativo = true, [FromQuery] int page = 1, [FromQuery] int itemsPerPage = 15)
        {
            try
            {
                var result = await _regiaoService.ListRegioesAsync(new Paginacao(page, itemsPerPage), ativo);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                // Regra de negócio violada (ex: região já existe)
                return BadRequest(ex.Message); // 400 Bad Request
            }
            catch (Exception ex)
            {
                // Erros inesperados
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Lista regiões filtradas pelo nome
        /// </summary>
        /// <param name="nome">Nome da região</param>
        /// <param name="ativo">Filtra apenas regiões ativas (opcional)</param>
        /// <param name="page">Número da página</param>
        /// <param name="itemsPerPage">Itens por página</param>
        /// <returns>Lista paginada de regiões filtradas</returns>
        [HttpGet("ListRegioesByNome")]
        public async Task<IActionResult> ListRegioesByNome([FromQuery] string nome, [FromQuery] bool? ativo = true, [FromQuery] int page = 1, [FromQuery] int itemsPerPage = 15)
        {            
            try
            {
                var result = await _regiaoService.ListRegioesByNomeAsync(new Paginacao(page, itemsPerPage), nome, ativo);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                // Regra de negócio violada (ex: região já existe)
                return BadRequest(ex.Message); // 400 Bad Request
            }
            catch (Exception ex)
            {
                // Erros inesperados
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}
