using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eve.Business.Objects
{
    public class ProductInventory
    {
        public int ProductId { get; set; }
        public int ProductMetaId { get; set; }
        public string ProductNameEs { get; set; }
        public string ProductNameEn { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductRetailPrice { get; set; }
        public decimal? ProductMetaPrice { get; set; }
        public decimal? ProductMetaRetailPrice { get; set; }
        public string ProductMetaTags { get; set; }
        public string ProductMetaTagsEn { get; set; }
        public string ProductMetaTagsEs { get; set; }
        public int ProductMetaInventory { get; set; }
        public int ProductMetaInventoryHold { get; set; }

        internal ProductInventory(Entity.usp_sel_ProductMetaInfo_Result u)
        {
            this.ProductId = u.ProductId.Value;
            this.ProductMetaId = u.ProductMetaId.Value;
            this.ProductMetaInventory = u.ProductMetaInventory.Value;
            this.ProductMetaInventoryHold = u.ProductMetaInventoryHold.Value;
            this.ProductMetaPrice = u.ProductMetaPrice;
            this.ProductMetaRetailPrice = u.ProductMetaRetailPrice;
            this.ProductMetaTags = u.ProductMetaTags;
            this.ProductMetaTagsEn = u.ProductMetaTagsEn;
            this.ProductMetaTagsEs = u.ProductMetaTagsEs;
            this.ProductNameEn = u.ProductNameEn;
            this.ProductNameEs = u.ProductNameEs;
            this.ProductPrice = u.ProductPrice.Value;
            this.ProductRetailPrice = u.ProductRetailPrice.Value;
        }
        public ProductInventory() { }
    }
}
