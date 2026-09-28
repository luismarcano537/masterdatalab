using masterdatalab.domain.Models.Enums;
using masterdatalab.domain.Models.ViewModels;
using masterdatalab.domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace masterdatalab.domain.Services
{
    public class DashboardService(DashboardRepository repo)
    {
        public async Task<DashboardViewModel> MontarAsync(DateTime? de, DateTime? ate)
        {
            var porSituacao = await repo.ResumoPorSituacaoAsync(de, ate);
            var porCliente = await repo.FaturamentoPorClienteAsync(de, ate);
            var topProdutos = await repo.TopProdutosAsync(5, de, ate);

            var faturados = new[] { (int)SituacaoPedido.PagamentoAprovado, (int)SituacaoPedido.Faturado };

            return new DashboardViewModel
            {
                De = de,
                Ate = ate,
                TotalPedidos = porSituacao.Sum(s => s.Qtd),
                TotalPendentes = porSituacao
                    .FirstOrDefault(s => s.Id == (int)SituacaoPedido.PendenteConfirmacao)?.Qtd ?? 0,
                Faturamento = porSituacao
                    .Where(s => faturados.Contains(s.Id))
                    .Sum(s => s.Total),
                PorSituacao = porSituacao,
                PorCliente = porCliente,
                TopProdutos = topProdutos
            };
        }
    }
}
