using JJMasterData.Commons.Data;
using masterdatalab.domain.Models.DTOs.Dashboard;
using masterdatalab.domain.Models.Enums;
using System.Data;

namespace masterdatalab.domain.Repositories
{
    public class DashboardRepository(DataAccess dataAccess) : BaseRepository(dataAccess)
    {

        private static readonly string StatusFaturados = $"{(int)SituacaoPedido.PagamentoAprovado}, {(int)SituacaoPedido.Faturado}";

        public Task<List<ResumoSituacao>> ResumoPorSituacaoAsync(DateTime? de, DateTime? ate)
        {
            var cmd = new DataAccessCommand
            {
                Sql = @"SELECT s.id            AS Id,
                               s.descricao     AS Situacao,
                               s.cor           AS Cor,
                               COUNT(*)        AS Qtd,
                               ISNULL(SUM(p.valor_total), 0) AS Total
                        FROM Pedidos p
                        INNER JOIN situacao s ON s.id = p.status
                        WHERE (@De IS NULL OR p.data_emissao >= @De)
                          AND (@Ate IS NULL OR p.data_emissao < DATEADD(DAY, 1, @Ate))
                        GROUP BY s.id, s.descricao, s.cor
                        ORDER BY s.id"
            };

            cmd.Parameters.Add(new DataAccessParameter("@De", (object?)de ?? DBNull.Value, DbType.DateTime2));
            cmd.Parameters.Add(new DataAccessParameter("@Ate", (object?)ate ?? DBNull.Value, DbType.DateTime2));

            return GetListAsync<ResumoSituacao>(cmd);
        }

        public Task<List<FaturamentoCliente>> FaturamentoPorClienteAsync(DateTime? de, DateTime? ate)
        {
            var cmd = new DataAccessCommand
            {
                Sql = $@"SELECT c.id       AS IdCliente,
                        c.nome     AS Cliente,
                        COUNT(*)   AS QtdPedidos,
                        ISNULL(SUM(p.valor_total), 0) AS Total
                 FROM Pedidos p
                 INNER JOIN Cliente c ON c.id = p.id_cliente
                 WHERE p.status IN ({StatusFaturados})
                   AND (@De IS NULL OR p.data_emissao >= @De)
                   AND (@Ate IS NULL OR p.data_emissao < DATEADD(DAY, 1, @Ate))
                 GROUP BY c.id, c.nome
                 ORDER BY Total DESC"
            };

            cmd.Parameters.Add(new DataAccessParameter("@De", (object?)de ?? DBNull.Value, DbType.DateTime2));
            cmd.Parameters.Add(new DataAccessParameter("@Ate", (object?)ate ?? DBNull.Value, DbType.DateTime2));

            return GetListAsync<FaturamentoCliente>(cmd);
        }

        public Task<List<TopProduto>> TopProdutosAsync(int top, DateTime? de, DateTime? ate)
        {
            var cmd = new DataAccessCommand
            {
                Sql = $@"SELECT TOP (@Top)
                        pr.codigo            AS Codigo,
                        pr.nome              AS Produto,
                        SUM(i.quantidade)    AS QtdUnidades,
                        SUM(i.quantidade * i.valor_unit) AS Total
                 FROM ItemPedidos i
                 INNER JOIN Pedidos  p  ON p.id = i.pedido_id
                 INNER JOIN Produtos pr ON pr.codigo = i.item_id
                 WHERE p.status IN ({StatusFaturados})
                   AND (@De IS NULL OR p.data_emissao >= @De)
                   AND (@Ate IS NULL OR p.data_emissao < DATEADD(DAY, 1, @Ate))
                 GROUP BY pr.codigo, pr.nome
                 ORDER BY QtdUnidades DESC"
            };

            cmd.Parameters.Add(new DataAccessParameter("@Top", top, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@De", (object?)de ?? DBNull.Value, DbType.DateTime2));
            cmd.Parameters.Add(new DataAccessParameter("@Ate", (object?)ate ?? DBNull.Value, DbType.DateTime2));

            return GetListAsync<TopProduto>(cmd);
        }
    }
}
