using System;

namespace DataAccess.Mediator.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbProcedureAttribute : Attribute
    {
        public string Name { get; }
        public DbProcedureAttribute(string name) => Name = name;
    }
}
