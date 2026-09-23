using System;
using System.Collections.Generic;
using System.Text;

namespace masterdatalab.domain.Models.DTOs.Produtos
{
    public class ProdutoRequest
    {
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }
}
