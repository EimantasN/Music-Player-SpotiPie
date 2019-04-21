using Mobile_Api.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mobile_Api
{
    public abstract class BaseService : SharedService
    {
        public async Task<List<T>> Search<T>(string query)
        {
            return await PostList<T>(Endpoints.Search, $"query={query}");
        }
    }
}
