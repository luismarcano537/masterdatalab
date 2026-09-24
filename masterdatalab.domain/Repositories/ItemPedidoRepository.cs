using JJMasterData.Commons.Data;
using masterdatalab.domain.Models.DTOs.ItemPedido;
using System.Data;

namespace masterdatalab.domain.Repositories
{
    public class ItemPedidoRepository(DataAccess dataAccess) : BaseRepository(dataAccess)
    {
        private const string Colunas = "id AS Id, item_id AS ItemId, pedido_id AS PedidoId, quantidade AS Quantidade, valor_unit AS ValorUnit";

        public Task<List<ItemPedidoResponse>> GetByPedidoAsync(int pedidoId)
        {
            var cmd = new DataAccessCommand
            {
                Sql = $"SELECT {Colunas} FROM ItemPedidos WHERE pedido_id = @PedidoId ORDER BY id DESC"
            };
            cmd.Parameters.Add(new DataAccessParameter("@PedidoId", pedidoId, DbType.Int32));

            return GetListAsync<ItemPedidoResponse>(cmd);
        }

        public Task<ItemPedidoResponse?> GetByIdAsync(int id)
        {
            var cmd = new DataAccessCommand
            {
                Sql = $"SELECT {Colunas} FROM ItemPedidos WHERE id = @Id"
            };
            cmd.Parameters.Add(new DataAccessParameter("@Id", id, DbType.Int32));

            return GetAsync<ItemPedidoResponse>(cmd);
        }

        public async Task<bool> ItemJaExisteAsync(int pedidoId, int codProduto)
        {
            var cmd = new DataAccessCommand { Sql = "SELECT COUNT(1) FROM ItemPedidos WHERE pedido_id = @PedidoId AND item_id = @CodProduto" };

            cmd.Parameters.Add(new DataAccessParameter("@PedidoId", pedidoId, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@CodProduto", codProduto, DbType.Int32));
            return await CountAsync(cmd) > 0;
        }

        public Task<int> InsertAsync(ItemPedidoRequest itemPedido, decimal valorUnit)
        {
            var cmd = new DataAccessCommand
            {
                Sql = "INSERT INTO ItemPedidos (pedido_id, item_id, quantidade, valor_unit) OUTPUT INSERTED.id VALUES(@PedidoId, @ItemId, @Quantidade, @ValorUnit)"
            };

            cmd.Parameters.Add(new DataAccessParameter("@PedidoId", itemPedido.PedidoId, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@ItemId", itemPedido.ItemId, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@Quantidade", itemPedido.Quantidade, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@ValorUnit", valorUnit, DbType.Decimal));
            return InsertAsync(cmd);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cmd = new DataAccessCommand
            {
                Sql = "DELETE FROM ItemPedidos WHERE id = @Id"
            };

            cmd.Parameters.Add(new DataAccessParameter("@Id", id, DbType.Int32));
            return await ExecuteAsync(cmd) > 0;
        }
    }
}
