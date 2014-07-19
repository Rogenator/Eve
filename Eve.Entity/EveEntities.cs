using System;
using System.Data.Entity;

namespace Eve.Entity
{
    public partial class EveEntities : DbContext
    {
        public EveEntities(string connectionString) : base(connectionString) { }
    }
}