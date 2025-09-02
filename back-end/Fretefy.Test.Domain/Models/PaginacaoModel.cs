namespace Fretefy.Test.Domain.Models
{
    /// <summary>
    /// Obriga o dev a trabalhar pensando em performance para exibição de dados que hoje podem ser minimos, mas no futuro pode conter milhares de registros e o banco não pode ser sobrecarregado.
    /// </summary>
    public class Paginacao
    {        
        public Paginacao(int pagina, int totalItensPorPagina)
        {
            Pagina = pagina;
            TotalItensPorPagina = totalItensPorPagina;            
        }            

        /// <summary>
        /// Representa a página atual a ser processada.
        /// </summary>
        public int Pagina { get; private set; }

        /// <summary>
        /// Indique quantos itens mostrar por página.
        /// </summary>
        public int TotalItensPorPagina { get; set; }
    }
}
