using System;

namespace DataAccess.Mediator.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SqlDbTypeAttribute : Attribute
    {
        public System.Data.DbType DbType { get; }
        public SqlDbTypeAttribute(System.Data.DbType dbType) => DbType = dbType;
    }
}
