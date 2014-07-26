using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eve.Business.Services
{
    public class MetaService : BaseService
    {
        public int GetCount(string like)
        {
            like = like ?? string.Empty;
            return context.Metas.Where(x => x.MetaNameEn.Contains(like) || x.MetaNameEs.Contains(like)).Count();
        }

        public List<Objects.Meta> Get(Util.GridOptions options, string like)
        {
            List<Objects.Meta> lst = new List<Objects.Meta>();
            foreach (var m in context.Metas.Where(x => x.MetaNameEn.Contains(like) || x.MetaNameEs.Contains(like))
                .OrderBy(options.OrderBy, options.OrderDescending)
                .Skip(options.Skip)
                .Take(options.RowsPerPage))
                lst.Add(new Objects.Meta(m));
            return lst;
        }

        public List<Objects.Meta> GetAll()
        {
            List<Objects.Meta> lst = new List<Objects.Meta>();
            foreach (var m in context.Metas.OrderBy(x => x.MetaNameEn))
                lst.Add(new Objects.Meta(m));
            return lst;
        }

        public Objects.Meta Get(int metaId)
        {
            Entity.Meta meta = context.Metas.Find(metaId);
            if (meta != null)
                return new Objects.Meta(meta);
            return null;
        }

        public int Save(Objects.Meta meta)
        {
            if (meta.MetaId == -1)
            {
                Entity.Meta entity = meta.ToEntity();
                context.Metas.Add(entity);
                if (context.SaveChanges() > 0)
                {
                    foreach (Objects.MetaOption o in meta.Options)
                        entity.MetaOptions.Add(o.ToEntity());
                    return context.SaveChanges();
                }
                return -1;
            }
            else
            {
                Entity.Meta entity = context.Metas.Find(meta.MetaId);
                if (entity != null)
                {
                    entity.MetaNameEn = meta.MetaNameEn;
                    entity.MetaNameEs = meta.MetaNameEs;
                    context.SaveChanges();

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<root>");
                    foreach (var op in meta.Options)
                    {
                        op.MetaId = entity.MetaId;
                        sb.Append(op.Xml);
                    }
                    sb.Append("</root>");
                    context.Database.ExecuteSqlCommand("EXEC dbo.usp_upd_MetaOptions @x",
                        new System.Data.SqlClient.SqlParameter("@x", sb.ToString()));
                    return 1;
                }
                return -1;
            }
        }

        public int Delete(int metaId)
        {
            try
            {
                Entity.Meta m = context.Metas.Find(metaId);
                m.MetaOptions.Clear();
                context.Metas.Remove(m);
                return context.SaveChanges();
            }
            catch
            {
                return -1;
            }
        }

        public int SaveOption(Objects.MetaOption option)
        {
            if (option.MetaOptionId == -1)
            {
                context.MetaOptions.Add(option.ToEntity());
                return context.SaveChanges();
            }
            else
            {
                Entity.MetaOption entity = context.MetaOptions.Find(option.MetaOptionId);
                if (entity != null)
                {
                    entity.MetaOptionNameEn = option.MetaOptionNameEn;
                    entity.MetaOptionNameEs = option.MetaOptionNameEs;
                    entity.MetaOptionValue = option.MetaOptionValue;
                    return context.SaveChanges();
                }
                return -1;
            }
        }

        private void ClearMeta(int metaId)
        {
            context.Database.ExecuteSqlCommand(String.Format("DELETE FROM MetaOption WHERE MetaId = {0}", metaId));
        }
    }
}
