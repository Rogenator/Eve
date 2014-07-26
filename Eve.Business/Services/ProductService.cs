using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eve.Business.Services
{
    public enum Comparisson
    {
        LowerThan,
        LowerOrEqualTo,
        EqualTo,
        DifferentOf,
        GreaterThan,
        GreaterOrEqualTo
    }

    public class ProductService : BaseService
    {
        public int GetCount(ProductFiltering filter)
        {
            return context.Products.Where(x => x.CompanyId.Value == filter.CompanyId)
                .Where(filter.Filter()).Count();
        }

        public List<Objects.Product> Get(Util.GridOptions options, ProductFiltering filter)
        {
            List<Objects.Product> lst = new List<Objects.Product>();
            foreach (var p in context.Products.Where(x => x.CompanyId.Value == filter.CompanyId)
                .Where(filter.Filter())
                .OrderBy(options.OrderBy, options.OrderDescending)
                .Skip(options.Skip)
                .Take(options.RowsPerPage))
                lst.Add(new Objects.Product(p));
            return lst;
        }

        public List<Objects.ProductImage> GetImages(int productId)
        {
            List<Objects.ProductImage> lst = new List<Objects.ProductImage>();
            foreach (var a in context.ProductImages.Where(x => x.ProductId == productId))
                lst.Add(new Objects.ProductImage(a));
            return lst;
        }

        public List<Objects.ProductMeta> GetMetas(int productId)
        {
            List<Objects.ProductMeta> lst = new List<Objects.ProductMeta>();
            foreach (var a in context.ProductMetas.Where(x => x.ProductId == productId))
                lst.Add(new Objects.ProductMeta(a));
            return lst;
        }

        public List<Objects.ProductInventory> GetInventory(int productId)
        {
            List<Objects.ProductInventory> lst = new List<Objects.ProductInventory>();
            foreach (Entity.usp_sel_ProductMetaInfo_Result pi in context.ProductInventory(productId))
                lst.Add(new Objects.ProductInventory(pi));
            return lst;
        }

        public List<string> Save(Objects.Product product, List<Objects.ProductImage> images, List<Objects.ProductMeta> metas)
        {
            if (product.ProductId == -1)
            {
                Entity.Product entity = product.ToEntity();
                context.Products.Add(entity);
                if (context.SaveChanges() > 0)
                {
                    if(images != null)
                        foreach (var i in images)
                        {
                            i.ProductId = entity.ProductId;
                            entity.ProductImages.Add(i.ToEntity());
                        }
                    if(metas != null)
                        foreach (var m in metas)
                        {
                            m.ProductId = entity.ProductId;
                            entity.ProductMetas.Add(m.ToEntity());
                        }
                    int rt = context.SaveChanges();
                    if (rt == 0)
                        return new List<string>();
                    else
                        return new List<string>(new string[] { "Failed" });
                }
            }
            else
            {
                Entity.Product entity = context.Products.Find(product.ProductId);
                if (entity != null)
                {
                    entity.CategoryId = product.CategoryId;
                    entity.CompanyId = product.CompanyId;
                    entity.ProductDescriptionEn = product.ProductDescriptionEn;
                    entity.ProductDescriptionEs = product.ProductDescriptionEs;
                    entity.ProductIsActive = product.ProductIsActive;
                    entity.ProductNameEn = product.ProductNameEn;
                    entity.ProductNameEs = product.ProductNameEs;
                    entity.ProductPrice = product.ProductPrice;
                    entity.ProductRetailPrice = product.ProductRetailPrice;
                    context.SaveChanges();
                    //Clear images
                    entity.ProductImages.Clear();
                    context.SaveChanges();
                    if (images != null)
                        foreach (var i in images)
                        {
                            i.ProductId = entity.ProductId;
                            entity.ProductImages.Add(i.ToEntity());
                        }
                    context.SaveChanges();
                    //Do Metas proc
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<root>");
                    foreach(var m in metas)
                    {
                        m.ProductId = entity.ProductId;
                        sb.Append(m.Xml);
                    }
                    sb.Append("</root>");
                    var value = context.Database.SqlQuery<string>("EXEC usp_upd_ProductMeta @x",
                        new System.Data.SqlClient.SqlParameter("@x", sb.ToString()));
                    return value.ToList();
                }
            }
            return new List<string>(new string[] { "Failed" });
        }

        public int SaveInventory(List<Objects.ProductMeta> metaList)
        {
            foreach (var m in metaList)
            {
                Entity.ProductMeta meta = context.ProductMetas.Find(m.ProductMetaId);
                if (meta != null)
                {
                    meta.ProductMetaInventory = m.ProductMetaInventory;
                    meta.ProductMetaPrice = m.ProductMetaPrice;
                    meta.ProductMetaRetailPrice = m.ProductMetaRetailPrice;
                }
            }
            return context.SaveChanges();
        }
    }

    public class ProductFiltering
    {
        public string Like { get; set; }
        public decimal? Price { get; set; }
        public Comparisson PriceComparisson { get; set; }
        public int Category { get; set; }
        public int CompanyId { get; set; }

        internal Func<Entity.Product, bool> Filter()
        {
            return (x) =>
            {
                bool pricePassed = true, likePassed = true, catsPassed = true;
                if (Price.HasValue)
                {
                    switch (PriceComparisson)
                    {
                        case Comparisson.DifferentOf: pricePassed = x.ProductPrice != Price.Value; break;
                        case Comparisson.EqualTo: pricePassed = x.ProductPrice == Price.Value; break;
                        case Comparisson.GreaterOrEqualTo: pricePassed = x.ProductPrice >= Price.Value; break;
                        case Comparisson.GreaterThan: pricePassed = x.ProductPrice > Price.Value; break;
                        case Comparisson.LowerOrEqualTo: pricePassed = x.ProductPrice <= Price.Value; break;
                        case Comparisson.LowerThan: pricePassed = x.ProductPrice < Price.Value; break;
                    }
                }
                if (Like != null)
                    likePassed = x.ProductNameEn.ToUpper().Contains(Like.ToUpper()) ||
                        x.ProductNameEs.ToUpper().Contains(Like.ToUpper()) ||
                        x.ProductDescriptionEn.ToUpper().Contains(Like.ToUpper()) ||
                        x.ProductDescriptionEs.ToUpper().Contains(Like.ToUpper());
                if (Category > -1)
                    catsPassed = Category == x.CategoryId.Value;
                return pricePassed && likePassed && catsPassed;
            };
        }
    }
}
