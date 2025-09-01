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

        [HttpPost]
        public async Task<IActionResult> AddRegiao([FromBody] RegiaoDTO regiaoDTO)
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

        [HttpPost]
        public async Task<IActionResult> AddRegiaoCidadesVinculos([FromBody] RegiaoDTO regiaoDTO)
        {
            if (regiaoDTO == null || regiaoDTO.Regiao == null || regiaoDTO.CidadesVinculadas == null)
                return BadRequest("Objeto Região não pode ser nulo.");            

            try
            {
                await _regiaoService.AddRegiaoCidadeVinculosAsync(regiaoDTO.Regiao.Id,
                    regiaoDTO.CidadesVinculadas.Select(x=> x.CidadeID)?.ToArray()
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

        [HttpPut]
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

        [HttpDelete]
        public IActionResult DeleteRegiao([FromBody] Regiao regiao)
        {
            if (regiao == null)
                return BadRequest("Objeto Região não pode ser nulo.");

            try
            {
                _regiaoService.RemoveRegiao(regiao);
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

        [HttpDelete]
        public IActionResult RemoveRegiaoCidadeTodosVinculos([FromBody] Regiao regiao)
        {
            if (regiao == null)
                return BadRequest("Objeto Região não pode ser nulo.");

            try
            {
                _regiaoService.RemoveRegiaoCidadeVinculos(regiao.Id);
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
