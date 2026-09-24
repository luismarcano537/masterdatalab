using JJMasterData.Commons.Data;
using masterdatalab.domain.Extensions;

namespace masterdatalab.domain.Repositories
{
    public class BaseRepository(DataAccess dataAccess)
    {
        protected async Task<T?> GetAsync<T>(DataAccessCommand cmd)
        {
            var dt = await dataAccess.GetDataTableAsync(cmd);
            return dt.ToModel<T>();
        }

        protected async Task<List<T>> GetListAsync<T>(DataAccessCommand cmd)
        {
            var dt = await dataAccess.GetDataTableAsync(cmd);
            return dt.ToModelList<T>() ?? [];
        }

        protected async Task<int> CountAsync(DataAccessCommand cmd)
        {
            var result = await dataAccess.GetResultAsync(cmd);
            return result is null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        protected async Task<int> InsertAsync(DataAccessCommand cmd)
        {
            var id = await dataAccess.GetResultAsync(cmd);
            return Convert.ToInt32(id);
        }

        protected Task<int> ExecuteAsync(DataAccessCommand cmd)
            => dataAccess.SetCommandAsync(cmd);

        protected async Task<T?> ScalarAsync<T>(DataAccessCommand cmd) where T : struct
        {
            var result = await dataAccess.GetResultAsync(cmd);
            return result is null || result == DBNull.Value ? null : (T)Convert.ChangeType(result, typeof(T));
        }
    }
}