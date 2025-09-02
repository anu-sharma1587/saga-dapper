using System.Data.Common;
using System.Threading.Tasks;

namespace DataAccess.DbConnectionProvider
{
    public interface IDbConnectionFactory
    {
        Task<DbConnection> CreateAsync(string name = null);
    }
}
