using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccess.Dapper
{
    public interface IDapperDataRepository
    {
        Task<int> AddAsync<TEntity>(TEntity entity, DbConnection dbConnection) where TEntity : class, new();
        Task<TEntity> FindByIDAsync<TEntity>(object id, DbConnection dbConnection) where TEntity : class, new();
        Task<IEnumerable<TEntity>> FindByKeyAsync<TEntity>(DbConnection dbConnection, Expression<Func<TEntity, bool>> keyFilter) where TEntity : class, new();
        Task<int> UpdateAsync<TEntity>(TEntity entity, object id, DbConnection dbConnection) where TEntity : class, new();
        // ...other methods as needed
    }
}
