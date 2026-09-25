using System;
using System.Collections.Generic;
using System.Text;

namespace masterdatalab.domain.Models.DTOs.Dashboard
{
    public class FaturamentoCliente
    {
        public int IdCliente { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public int QtdePedidos { get; set; }
        public decimal Total { get; set; }
    }
}
