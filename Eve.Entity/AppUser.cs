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
    
    public partial class AppUser
    {
        public AppUser()
        {
            this.CompanyRelations = new HashSet<CompanyRelation>();
            this.CompanyUsers = new HashSet<CompanyUser>();
            this.NotificationUsers = new HashSet<NotificationUser>();
            this.SaleOrders = new HashSet<SaleOrder>();
        }
    
        public int AppUserId { get; set; }
        public string AppUserName { get; set; }
        public string AppUserEmail { get; set; }
        public string AppUserPassword { get; set; }
        public Nullable<System.DateTime> AppUserDateOfBirth { get; set; }
        public bool AppUserIsActive { get; set; }
        public int AppUserTypeId { get; set; }
    
        public virtual ICollection<CompanyRelation> CompanyRelations { get; set; }
        public virtual ICollection<CompanyUser> CompanyUsers { get; set; }
        public virtual ICollection<NotificationUser> NotificationUsers { get; set; }
        public virtual ICollection<SaleOrder> SaleOrders { get; set; }
        public virtual UserType UserType { get; set; }
    }
}
