using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces;
using Fretefy.Test.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using ClosedXML.Excel;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Fretefy.Test.WebApi.Controllers
{
    [Route("api/regiao")]
    [ApiController]
    public class RegiaoController : ControllerBase
    {
        private readonly IRegiaoService _regiaoService;
        private readonly ICidadeService _cidadeService;

        public RegiaoController(IRegiaoService regiaoService, ICidadeService cidadeService)
        {
            _regiaoService = regiaoService;
            _cidadeService = cidadeService;
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
        /// Adiciona uma nova região a partir de um CEP fornecido.
        /// </summary>
        /// <param name="regiaoDTO">Objeto contendo informações da região</param>
        /// <returns>Retorna a região criada</returns>
        [HttpPost("AddRegiaoByCEPUsandoViaCEPAPI")]
        public async Task<IActionResult> AddRegiaoByCEPUsandoViaCEPAPI([FromQuery] string cep)
        {            
            try
            {
                var regiao = await _regiaoService.AddRegiaoByCEPAsync(cep);

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
        /// Lista regiões.
        /// </summary>
        /// <param name="nome">Nome da região</param>
        /// <param name="ativo">Filtra apenas regiões ativas (opcional)</param>
        /// <param name="page">Número da página</param>
        /// <param name="itemsPerPage">Itens por página</param>
        /// <returns>Lista paginada de regiões filtradas</returns>
        [HttpGet("ListRegioes")]
        public async Task<IActionResult> ListRegioes([FromQuery] string nome = null, [FromQuery] bool? ativo = true, [FromQuery] int page = 1, [FromQuery] int itemsPerPage = 15)
        {            
            try
            {
                var result = await _regiaoService.ListRegioesAsync(new Paginacao(page, itemsPerPage), nome, ativo);

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
        /// Exporta lista de regiões para Excel
        /// </summary>
        /// <param name="ativo">Filtra apenas regiões ativas (opcional)</param>
        /// <param name="nome">Filtro por nome da região (opcional)</param>
        /// <returns>Arquivo Excel com lista de regiões</returns>
        [HttpGet("ListRegioesExportXLS")]
        public async Task<IActionResult> ListRegioesExportXLS([FromQuery] bool? ativo = true, [FromQuery] string nome = null)
        {
            try
            {
                var regioes = await _regiaoService.ListRegioesAsync(null, nome, ativo);

                if (regioes?.Data == null || !regioes.Data.Any())
                {
                    return NotFound("Nenhuma região encontrada.");
                }

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Regiões");

                // Cabeçalhos
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "Nome";
                worksheet.Cell(1, 3).Value = "Ativo";
                worksheet.Cell(1, 4).Value = "Cidades Vinculadas";

                // Estilo dos cabeçalhos
                var headerRange = worksheet.Range(1, 1, 1, 4);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Dados
                int row = 2;
                foreach (var regiao in regioes.Data)
                {
                    worksheet.Cell(row, 1).Value = regiao.Regiao.Id;
                    worksheet.Cell(row, 2).Value = regiao.Regiao.Nome;
                    worksheet.Cell(row, 3).Value = regiao.Regiao.Ativo ? "Sim" : "Não";

                    List<string> cidadeNomes = new List<string>();

                    worksheet.Cell(row, 4).Value = string.Join(", ", regiao.CidadesVinculadas.Select(x => _cidadeService.Get(x.CidadeID).Nome));
                    row++;
                }

                // Autoajustar colunas
                worksheet.Columns().AdjustToContents();

                // Salvar em memória
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                // Retornar o arquivo para download
                return File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Regioes.xlsx"
                );
            }
            catch (InvalidOperationException ex)
            {
                // Regra de negócio violada (ex: região já existe)
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                // Erros inesperados
                return StatusCode(500, new { erro = $"Erro interno: {ex.Message}" });
            }
        }

        
        
    }
}
