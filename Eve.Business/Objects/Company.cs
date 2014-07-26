using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eve.Business.Objects
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDescription { get; set; }
        public string CompanyLogo { get; set; }
        public bool CompanyIsActive { get; set; }

        internal Company(Eve.Entity.Company c)
        {
            this.CompanyDescription = c.CompanyDescription;
            this.CompanyId = c.CompanyId;
            this.CompanyIsActive = c.CompanyIsActive;
            this.CompanyLogo = c.CompanyLogo;
            this.CompanyName = c.CompanyName;
        }
        public Company() { }

        internal Eve.Entity.Company ToEntity()
        {
            return new Entity.Company
            {
                CompanyDescription = this.CompanyDescription,
                CompanyId = this.CompanyId,
                CompanyIsActive = this.CompanyIsActive,
                CompanyLogo = this.CompanyLogo,
                CompanyName = this.CompanyName
            };
        }
    }
}
