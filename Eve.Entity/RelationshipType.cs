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
    
    public partial class RelationshipType
    {
        public RelationshipType()
        {
            this.CompanyRelations = new HashSet<CompanyRelation>();
        }
    
        public int RelationshipTypeId { get; set; }
        public string RelationshipTypeNameEs { get; set; }
        public string RelationshipTypeNameEn { get; set; }
        public bool RelationshipTypeIsActive { get; set; }
    
        public virtual ICollection<CompanyRelation> CompanyRelations { get; set; }
    }
}
