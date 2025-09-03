using System;

namespace DataAccess.Mediator.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SqlParameterAttribute : Attribute
    {
        public string Name { get; }
        public SqlParameterAttribute(string name) => Name = name;
    }
}
