using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;

namespace DataAccess.Dapper
{
    public class DapperDataRepository : IDapperDataRepository
    {
        public async Task<int> AddAsync<TEntity>(TEntity entity, DbConnection dbConnection) where TEntity : class, new()
        {
            // Example: Insert using Dapper
            // You may want to use a library like Dapper.Contrib or custom SQL
            var tableName = typeof(TEntity).Name;
            var sql = $"INSERT INTO {tableName} VALUES (@entity)";
            return await dbConnection.ExecuteAsync(sql, entity);
        }

        public async Task<TEntity> FindByIDAsync<TEntity>(object id, DbConnection dbConnection) where TEntity : class, new()
        {
            var tableName = typeof(TEntity).Name;
            var sql = $"SELECT * FROM {tableName} WHERE Id = @id";
            return await dbConnection.QueryFirstOrDefaultAsync<TEntity>(sql, new { id });
        }

        public async Task<IEnumerable<TEntity>> FindByKeyAsync<TEntity>(DbConnection dbConnection, Expression<Func<TEntity, bool>> keyFilter) where TEntity : class, new()
        {
            // For demo: you may want to use a library to convert Expression to SQL
            throw new NotImplementedException("Expression to SQL conversion required");
        }

        public async Task<int> UpdateAsync<TEntity>(TEntity entity, object id, DbConnection dbConnection) where TEntity : class, new()
        {
            var tableName = typeof(TEntity).Name;
            var sql = $"UPDATE {tableName} SET /*...*/ WHERE Id = @id";
            // You need to build the SET clause dynamically
            throw new NotImplementedException("Dynamic update SQL required");
        }
    }
}
