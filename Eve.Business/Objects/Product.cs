using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eve.Business.Objects
{
    public class Product
    {
        public int ProductId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProductNameEs { get; set; }
        public string ProductNameEn { get; set; }
        public string ProductDescriptionEs { get; set; }
        public string ProductDescriptionEn { get; set; }
        public bool ProductIsActive { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductRetailPrice { get; set; }

        internal Product(Entity.Product p)
        {
            this.CategoryId = p.CategoryId.Value;
            this.CategoryName = p.Category.CategoryNameEn;
            this.CompanyId = p.CompanyId.Value;
            this.CompanyName = p.Company.CompanyName;
            this.ProductDescriptionEn = p.ProductDescriptionEn;
            this.ProductDescriptionEs = p.ProductDescriptionEs;
            this.ProductId = p.ProductId;
            this.ProductIsActive = p.ProductIsActive;
            this.ProductNameEn = p.ProductNameEn;
            this.ProductNameEs = p.ProductNameEs;
            this.ProductPrice = p.ProductPrice;
            this.ProductRetailPrice = p.ProductRetailPrice;
        }
        public Product() { }

        internal Entity.Product ToEntity()
        {
            return new Entity.Product
            {
                CategoryId = this.CategoryId,
                CompanyId = this.CompanyId,
                ProductDescriptionEn = this.ProductDescriptionEn,
                ProductDescriptionEs = this.ProductDescriptionEs,
                ProductId = this.ProductId,
                ProductIsActive = this.ProductIsActive,
                ProductNameEn = this.ProductNameEn,
                ProductNameEs = this.ProductNameEs,
                ProductPrice = this.ProductPrice,
                ProductRetailPrice = this.ProductRetailPrice
            };
        }
    }

    public class ProductImage
    {
        public int ProductImageId { get; set; }
        public int ProductId { get; set; }
        public string ProductImageUrl { get; set; }
        public string ProductImageThumbnail { get; set; }

        internal ProductImage(Entity.ProductImage pi)
        {
            this.ProductId = pi.ProductId.Value;
            this.ProductImageId = pi.ProductImageId;
            this.ProductImageThumbnail = pi.ProductImageThumbnail;
            this.ProductImageUrl = pi.ProductImageUrl;
        }
        public ProductImage() { }

        internal Entity.ProductImage ToEntity()
        {
            return new Entity.ProductImage
            {
                ProductId = this.ProductId,
                ProductImageId = this.ProductImageId,
                ProductImageThumbnail = this.ProductImageThumbnail,
                ProductImageUrl = this.ProductImageUrl
            };
        }
    }

    public class ProductMeta
    {
        public int ProductId { get; set; }
        public int ProductMetaId { get; set; }
        public string ProductMetaTags { get; set; }
        public decimal? ProductMetaPrice { get; set; }
        public decimal? ProductMetaRetailPrice { get; set; }
        public int ProductMetaInventory { get; set; }
        public int ProductMetaInventoryHold { get; set; }
        internal string Xml
        {
            get
            {
                return string.Format("<m pid='{0}' tags='{1}' />", ProductId, ProductMetaTags);
            }
        }

        internal ProductMeta(Entity.ProductMeta m)
        {
            this.ProductId = m.ProductId.Value;
            this.ProductMetaId = m.ProductMetaId;
            this.ProductMetaInventory = m.ProductMetaInventory;
            this.ProductMetaInventoryHold = m.ProductMetaInventoryHold;
            this.ProductMetaPrice = m.ProductMetaPrice;
            this.ProductMetaRetailPrice = m.ProductMetaRetailPrice;
            this.ProductMetaTags = m.ProductMetaTags;
        }
        public ProductMeta() { }

        internal Entity.ProductMeta ToEntity()
        {
            return new Entity.ProductMeta
            {
                ProductId = this.ProductId,
                ProductMetaId = this.ProductMetaId,
                ProductMetaInventory = this.ProductMetaInventory,
                ProductMetaInventoryHold = this.ProductMetaInventoryHold,
                ProductMetaPrice = this.ProductMetaPrice,
                ProductMetaRetailPrice = this.ProductMetaRetailPrice,
                ProductMetaTags = this.ProductMetaTags
            };
        }
    }
}