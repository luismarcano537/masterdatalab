using System;
using System.Collections.Generic;
using System.Text;

namespace masterdatalab.domain.Models.DTOs.TabelaPreco
{
    public class TabelaPrecoRequest
    {
        public int CodCliente { get; set; }
        public int CodProduto { get; set; }
        public decimal ValorUnit { get; set; }
    }
}
