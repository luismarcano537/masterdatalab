using masterdatalab.domain.Models.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace masterdatalab.domain.Models.ViewModels
{
    public class DashboardViewModel
    {
        public DateTime? De { get; set; }
        public DateTime? Ate { get; set; }
        public int TotalPedidos { get; set; }
        public int TotalPendentes { get; set; }
        public decimal Faturamento { get; set; }

        public List<ResumoSituacao> PorSituacao { get; set; } = [];
        public List<FaturamentoCliente> PorCliente { get; set; } = [];
        public List<TopProduto> TopProdutos { get; set; } = [];
    }
}
