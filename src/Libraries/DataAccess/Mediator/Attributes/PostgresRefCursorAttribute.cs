using System;

namespace DataAccess.Mediator.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PostgresRefCursorAttribute : Attribute
    {
        public string Name { get; }
        public PostgresRefCursorAttribute(string name) => Name = name;
    }
}
