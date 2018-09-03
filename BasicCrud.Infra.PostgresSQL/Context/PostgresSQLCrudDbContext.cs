using BasicCrud.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Tnf.Runtime.Session;

namespace BasicCrud.Infra.SqlServer.Context
{
    public class PostgresSQLCrudDbContext : CrudDbContext
    {
        public PostgresSQLCrudDbContext(DbContextOptions<CrudDbContext> options, ITnfSession session) 
            : base(options, session)
        {
        }
    }
}