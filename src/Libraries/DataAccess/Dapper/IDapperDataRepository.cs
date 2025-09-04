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

    /// <summary>
    /// Executes a stored procedure and returns a collection of results.
    /// </summary>
    /// <typeparam name="T">The type of the result object.</typeparam>
    /// <typeparam name="TParams">The type of the parameter object.</typeparam>
    /// <param name="parameters">The parameters for the stored procedure.</param>
    /// <param name="dbConnection">The database connection.</param>
    /// <returns>A collection of result objects.</returns>
    Task<IEnumerable<T>> ExecuteSpQueryAsync<T, TParams>(TParams parameters, DbConnection dbConnection) where T : class, new();
    }
}
