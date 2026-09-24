using JJMasterData.Commons.Data;
using masterdatalab.domain.Models.DTOs.Pedido;
using masterdatalab.domain.Models.Enums;
using System.Data;

namespace masterdatalab.domain.Repositories
{
    public class PedidoRepository(DataAccess dataAccess) : BaseRepository(dataAccess)
    {
        private const string Colunas = "id AS Id, id_cliente AS IdCliente, status AS Status, data_emissao AS DataEmissao, valor_total AS ValorTotal, qtd_produtos AS QtdProdutos, qtd_unidades AS QtdUnidades";

        public Task<int> CountAsync(int? status)
        {
            var cmd = new DataAccessCommand
            {
                Sql = "SELECT COUNT(1) FROM Pedidos WHERE (@Status IS NULL OR status = @Status)"
            };

            cmd.Parameters.Add(new DataAccessParameter("@Status", (object?)status ?? DBNull.Value, DbType.Int32));
            return CountAsync(cmd);
        }

        public Task<List<PedidoResponse>> GetAllAsync(int page, int pageSize, int? status)
        {
            var offset = (page - 1) * pageSize;

            var cmd = new DataAccessCommand
            {
                Sql = $@"SELECT {Colunas} FROM Pedidos WHERE (@Status IS NULL OR status = @Status) ORDER BY id DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY"
            };
            cmd.Parameters.Add(new DataAccessParameter("@Status", (object?)status ?? DBNull.Value, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@Offset", offset, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@PageSize", pageSize, DbType.Int32));
            return GetListAsync<PedidoResponse>(cmd);
        }

        public Task<PedidoResponse?> GetByIdAsync(int id)
        {
            var cmd = new DataAccessCommand
            {
                Sql = $"SELECT {Colunas} FROM Pedidos WHERE id = @Id"
            };
            cmd.Parameters.Add(new DataAccessParameter("@Id", id, DbType.Int32));

            return GetAsync<PedidoResponse>(cmd);
        }

        public Task<int> InsertAsync(PedidoRequest pedido)
        {
            var cmd = new DataAccessCommand
            {
                Sql = "INSERT INTO Pedidos (id_cliente, status) OUTPUT INSERTED.id VALUES(@IdCliente, @Status)"
            };

            cmd.Parameters.Add(new DataAccessParameter("@IdCliente", pedido.IdCliente, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@Status", (int)SituacaoPedido.PendenteConfirmacao, DbType.Int32));
            return InsertAsync(cmd);
        }

        public async Task<bool> AtualizarStatusAsync(int id, int status)
        {
            var cmd = new DataAccessCommand
            {
                Sql = "UPDATE Pedidos SET status = @Status WHERE id = @Id"
            };

            cmd.Parameters.Add(new DataAccessParameter("@Status", status, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@id", id, DbType.Int32));
            return await ExecuteAsync(cmd) > 0;
        }

        public Task<int?> GetStatusAsync(int id)
        {
            var cmd = new DataAccessCommand { Sql = "SELECT status FROM Pedidos WHERE id = @Id" };
            cmd.Parameters.Add(new DataAccessParameter("@Id", id, DbType.Int32));
            return ScalarAsync<int>(cmd);
        }

        public async Task<bool> PossuiItensAsync(int id)
        {
            var cmd = new DataAccessCommand { Sql = "SELECT COUNT(1) FROM ItemPedidos WHERE pedido_id = @Id" };
            cmd.Parameters.Add(new DataAccessParameter("@Id", id, DbType.Int32));
            return await CountAsync(cmd) > 0;
        }

        public Task<int?> GetClienteAsync(int id)
        {
            var cmd = new DataAccessCommand { Sql = "SELECT id_cliente FROM Pedidos WHERE id = @Id" };
            cmd.Parameters.Add(new DataAccessParameter("@Id", id, DbType.Int32));
            return ScalarAsync<int>(cmd);
        }
    }
}
