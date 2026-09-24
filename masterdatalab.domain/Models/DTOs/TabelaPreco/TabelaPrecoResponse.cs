using System;
using System.Collections.Generic;
using System.Text;

namespace masterdatalab.domain.Models.DTOs.TabelaPreco
{
    public class TabelaPrecoResponse
    {
        public int Id { get; set; }
        public int CodCliente { get; set; }
        public int CodProduto { get; set; }
        public decimal ValorUnit { get; set; }
        public DateTime DataInclusao { get; set; }
    }
}
