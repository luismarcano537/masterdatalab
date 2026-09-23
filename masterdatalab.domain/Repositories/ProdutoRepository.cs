using masterdatalab.domain.Models.DTOs.Cliente;
using JJMasterData.Commons.Data;
using System.Data;
using System;
using System.Collections.Generic;
using System.Text;
using masterdatalab.domain.Models.DTOs.Produtos;

namespace masterdatalab.domain.Repositories
{
    public class ProdutoRepository(DataAccess dataAccess) : BaseRepository(dataAccess)
    {
        private const string Colunas = "id AS Id, codigo AS Codigo, nome AS Nome, descricao AS Descricao, data_cadastro AS DataCadastro";

        public Task<ProdutoResponse?> GetByCodigoAsync(int codigo)
        {
            var cmd = new DataAccessCommand
            {
                Sql = $"SELECT {Colunas} FROM Produtos WHERE codigo = @Codigo"
            };
            cmd.Parameters.Add(new DataAccessParameter("@Codigo", codigo, DbType.Int32));
            return GetAsync<ProdutoResponse>(cmd);
        }

        public Task<int> CountAsync()
        {
            var cmd = new DataAccessCommand
            {
                Sql = "SELECT COUNT(1) FROM Produtos"
            };

            return CountAsync(cmd);
        }

        public Task<List<ProdutoResponse>> GetAllAsync(int page, int pageSize)
        {
            var offset = (page - 1) * pageSize;

            var cmd = new DataAccessCommand
            {
                Sql = $"SELECT {Colunas} FROM Produtos ORDER BY nome OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY"
            };
            cmd.Parameters.Add(new DataAccessParameter("@Offset", offset, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@PageSize", pageSize, DbType.Int32));
            return GetListAsync<ProdutoResponse>(cmd);
        }

        public Task<int> InsertAsync(ProdutoRequest produto)
        {
            var cmd = new DataAccessCommand
            {
                Sql = "INSERT INTO Produtos (codigo, nome, descricao, data_cadastro) OUTPUT INSERTED.codigo VALUES(@Codigo, @Nome, @Descricao, CAST(SYSDATETIME() AS date))"
            };
            cmd.Parameters.Add(new DataAccessParameter("@Codigo", produto.Codigo, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@Nome", produto.Nome, DbType.String));
            cmd.Parameters.Add(new DataAccessParameter("@Descricao", produto.Descricao, DbType.String));
            return InsertAsync(cmd);
        }

        public async Task<bool> UpdateAsync(int codigo, ProdutoRequest produto)
        {
            var cmd = new DataAccessCommand
            {
                Sql = "UPDATE Produtos SET nome = @Nome, descricao = @Descricao WHERE codigo = @Codigo"
            };

            cmd.Parameters.Add(new DataAccessParameter("@Nome", produto.Nome, DbType.String));
            cmd.Parameters.Add(new DataAccessParameter("@Descricao", produto.Descricao, DbType.String));
            cmd.Parameters.Add(new DataAccessParameter("@Codigo", codigo, DbType.Int32));

            return await ExecuteAsync(cmd) > 0;
        }
    }
}
