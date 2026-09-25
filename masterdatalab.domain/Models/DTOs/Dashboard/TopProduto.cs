using System;
using System.Collections.Generic;
using System.Text;

namespace masterdatalab.domain.Models.DTOs.Dashboard
{
    public class TopProduto
    {
        public int Codigo { get; set; }
        public string Produto { get; set; } = string.Empty;
        public int QtdeUnidades { get; set; }
        public decimal Total { get; set; }
    }
}
