using masterdatalab.domain.Models.DTOs.Cliente;
using JJMasterData.Commons.Data;
using System.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace masterdatalab.domain.Repositories
{
    public class ClienteRepository(DataAccess dataAccess) : BaseRepository(dataAccess)
    {
        private const string colunas = "id AS Id, nome AS Nome, documento AS Documento, razao_social AS RazaoSocial";

        public Task<ClienteResponse?> GetByIdAsync(int id)
        {
            var cmd = new DataAccessCommand
            {
                Sql = $"SELECT {colunas} FROM Cliente WHERE id = @id"
            };
            cmd.Parameters.Add(new DataAccessParameter("@id", id, DbType.Int32));
            return GetAsync<ClienteResponse>(cmd);
        }

        public Task<int> CountAsync()
        {
            var cmd = new DataAccessCommand
            {
                Sql = "SELECT COUNT(1) FROM Cliente"
            };

            return CountAsync(cmd);
        }

        public Task<List<ClienteResponse>> GetAllAsync(int page, int pageSize)
        {
            var offSet = (page - 1) * pageSize;

            var cmd = new DataAccessCommand
            {
                Sql = $"SELECT {colunas} FROM Cliente ORDER BY nome OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY"
            };
            cmd.Parameters.Add(new DataAccessParameter("@Offset", offSet, DbType.Int32));
            cmd.Parameters.Add(new DataAccessParameter("@PageSize", pageSize, DbType.Int32));
            return GetListAsync<ClienteResponse>(cmd);
        }

        public Task<int> InsertAsync(ClienteRequest cliente)
        {
            var cmd = new DataAccessCommand
            {
                Sql = $"INSERT INTO Cliente (nome, documento, razao_social) OUTPUT INSERTED.id VALUES(@Nome, @Documento, @RazaoSocial)"
            };
            cmd.Parameters.Add(new DataAccessParameter("@Nome", (object)cliente.Nome, DbType.String));
            cmd.Parameters.Add(new DataAccessParameter("@Documento", (object)cliente.Documento, DbType.String));
            cmd.Parameters.Add(new DataAccessParameter("@RazaoSocial", (object?)cliente.RazaoSocial ?? DBNull.Value, DbType.String));
            return InsertAsync(cmd);
        }

        public async Task<bool> UpdateAsync(int id, ClienteRequest cliente)
        {
            var cmd = new DataAccessCommand
            {
                Sql = "UPDATE Cliente SET nome = @Nome, documento = @Documento, razao_social = @RazaoSocial WHERE id = @id"
            };

            cmd.Parameters.Add(new DataAccessParameter("@Nome", (object)cliente.Nome, DbType.String));
            cmd.Parameters.Add(new DataAccessParameter("@Documento", (object)cliente.Documento, DbType.String));
            cmd.Parameters.Add(new DataAccessParameter("@RazaoSocial", (object?)cliente.RazaoSocial ?? DBNull.Value, DbType.String));
            cmd.Parameters.Add(new DataAccessParameter("@id", id, DbType.Int32));

            return await ExecuteAsync(cmd) > 0;
        }
    }
}