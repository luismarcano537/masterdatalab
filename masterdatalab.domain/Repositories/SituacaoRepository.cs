using System.Data;
using JJMasterData.Commons.Data;
using masterdatalab.domain.Models.DTOs.Situacao;

namespace masterdatalab.domain.Repositories
{
    public class SituacaoRepository(DataAccess dataAccess) : BaseRepository(dataAccess)
    {
        private const string Colunas = "id AS Id, descricao AS Descricao, icone AS Icone, cor AS Cor";

        public Task<List<SituacaoResponse>> GetAllAsync()
        {
            var cmd = new DataAccessCommand
            {
                Sql = $"SELECT {Colunas} FROM situacao ORDER BY id"
            };
            return GetListAsync<SituacaoResponse>(cmd);
        }
    }
}