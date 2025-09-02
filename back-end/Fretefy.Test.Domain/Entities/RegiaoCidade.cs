using System;

namespace Fretefy.Test.Domain.Entities
{
    public class RegiaoCidade : IEntity
    {
        public RegiaoCidade(){}

        public RegiaoCidade(Guid regiaoId, Guid cidadeId)
        {
            RegiaoId = regiaoId;
            CidadeID = cidadeId;
        }

        public Guid RegiaoId { get; set; }                 
        public Guid CidadeID { get; set; }             
        
        public Guid Id { get; set; }
    }
}
