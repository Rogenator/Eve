//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Eve.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductMeta
    {
        public ProductMeta()
        {
            this.SaleOrderDetails = new HashSet<SaleOrderDetail>();
        }
    
        public int ProductMetaId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string ProductMetaTags { get; set; }
        public Nullable<decimal> ProductMetaPrice { get; set; }
        public Nullable<decimal> ProductMetaRetailPrice { get; set; }
        public int ProductMetaInventory { get; set; }
        public int ProductMetaInventoryHold { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual ICollection<SaleOrderDetail> SaleOrderDetails { get; set; }
    }
}