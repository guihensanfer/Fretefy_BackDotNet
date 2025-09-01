using System.Collections.Generic;
using Fretefy.Test.Domain.Entities;

namespace Fretefy.Test.Domain.Models
{
    public class RegioesDTO
    {
        public RegioesDTO()
        {
            Data = new List<RegiaoDTO>();
        }
        public List<RegiaoDTO> Data { get; set; }
    }

    public class RegiaoDTO
    {
        public Regiao Regiao { get; set; }

        public List<RegiaoCidade> CidadesVinculadas { get; set; }        
    }    
}
