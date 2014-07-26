using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Eve.Entity
{
    public partial class EveEntities : DbContext
    {
        public EveEntities(string connectionString) : base(connectionString) { }

        public List<usp_sel_ProductMetaInfo_Result> ProductInventory(int productId)
        {
            List<usp_sel_ProductMetaInfo_Result> lst = new List<usp_sel_ProductMetaInfo_Result>();
            foreach (var s in this.usp_sel_ProductMetaInfo(productId))
                lst.Add(s);
            return lst;
        }
    }
}