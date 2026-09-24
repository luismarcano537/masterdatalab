using JJMasterData.Commons.Data;
using masterdatalab.domain.Models.DTOs.Produtos;
using masterdatalab.domain.Models.DTOs.TabelaPreco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace masterdatalab.domain.Repositories
{
    public class TabelaPrecoRepository(DataAccess dataAccess) : BaseRepository(dataAccess)
    {
        private const string Colunas = "id AS Id, cod_cliente AS CodCliente, cod_produto AS CodProduto, valor_unit AS ValorUnit, data_inclusao AS DataInclusao";

        public Task<int> CountAsync()
        {
            var cmd = new DataAccessCommand
            {
                Sql = "SELECT COUNT(1) FROM tabela_precos"
            };

            return CountAsync(cmd);
        }

        public Task<List<TabelaPrecoResponse>> GetAllAsync(int page, int pageSize)
        {
            var offset = (page - 1) * pageSize;

            var cmd = new DataAccessCommand
            {
                Sql = $"SELECT {Colunas} FROM tabela_precos ORDER BY cod_cliente OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY"
            };
            cmd.Parameters.Add(new DataAccessParameter("@Offset", offset, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@PageSize", pageSize, DbType.Int32));
            return GetListAsync<TabelaPrecoResponse>(cmd);
        }

        public Task<List<TabelaPrecoResponse>> GetByClienteAsync(int codCliente)
        {
            var cmd = new DataAccessCommand
            {
                Sql = $"SELECT {Colunas} FROM tabela_precos WHERE cod_cliente = @codCliente"
            };
            cmd.Parameters.Add(new DataAccessParameter("@codCliente", codCliente, DbType.Int32));
            return GetListAsync<TabelaPrecoResponse>(cmd);
        }

        public Task<decimal?> GetPrecoAsync(int codCliente, int codProduto)
        {
            var cmd = new DataAccessCommand
            {
                Sql = "SELECT valor_unit FROM tabela_precos WHERE cod_cliente = @codCliente AND cod_produto = @codProduto"
            };
            cmd.Parameters.Add(new DataAccessParameter("@codCliente", codCliente, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@codProduto", codProduto, DbType.Int32));
            return ScalarAsync<decimal>(cmd);
        }

        public Task<int> InsertAsync(TabelaPrecoRequest preco)
        {
            var cmd = new DataAccessCommand
            {
                Sql = "INSERT INTO tabela_precos (cod_cliente, cod_produto, valor_unit, data_inclusao) OUTPUT INSERTED.id VALUES(@codCliente, @codProduto, @ValorUnit, SYSDATETIME())"
            };
            cmd.Parameters.Add(new DataAccessParameter("@codCliente", preco.CodCliente, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@codProduto", preco.CodProduto, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@ValorUnit", preco.ValorUnit, DbType.Decimal));
            return InsertAsync(cmd);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cmd = new DataAccessCommand
            {
                Sql = "DELETE FROM tabela_precos WHERE id = @Id"
            };

            cmd.Parameters.Add(new DataAccessParameter("@Id", id, DbType.Int32));
            return await ExecuteAsync(cmd) > 0;
        }
    }
}
