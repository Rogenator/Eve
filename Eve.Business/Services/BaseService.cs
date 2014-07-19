using System;
using Eve.Entity;

namespace Eve.Business.Services
{
    public abstract class BaseService : IDisposable
    {
        protected readonly EveEntities context;

        public BaseService()
        {
            this.context = new EveEntities(DataBase.ConnectionString);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (context != null)
                    context.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
