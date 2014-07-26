using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eve.Business.Services
{
    public class CompanyService : BaseService
    {
        public int GetCount(string like)
        {
            like = like ?? string.Empty;
            return context.Companies.Where(x => x.CompanyDescription.Contains(like) || x.CompanyName.Contains(like)).Count();
        }

        public List<Objects.Company> Get(Util.GridOptions options, string like)
        {
            options.OrderBy = options.OrderBy ?? "CompanyId";
            like = like ?? string.Empty;

            List<Objects.Company> lst = new List<Objects.Company>();
            foreach (var cc in context.Companies.Where(x => x.CompanyDescription.Contains(like) || x.CompanyName.Contains(like))
                .OrderBy(options.OrderBy, options.OrderDescending)
                .Skip(options.Skip)
                .Take(options.RowsPerPage))
                lst.Add(new Objects.Company(cc));
            return lst;
        }

        public Objects.Company Get(int companyId)
        {
            Entity.Company co = context.Companies.Find(companyId);
            if (co != null)
                return new Objects.Company(co);
            return null;
        }

        public int Save(Objects.Company company)
        {
            if (company.CompanyId == -1)
                context.Companies.Add(company.ToEntity());
            else
            {
                Entity.Company cc = context.Companies.Find(company.CompanyId);
                if (cc != null)
                {
                    cc.CompanyDescription = company.CompanyDescription;
                    cc.CompanyIsActive = company.CompanyIsActive;
                    cc.CompanyLogo = company.CompanyLogo;
                    cc.CompanyName = company.CompanyName;
                }
            }
            return context.SaveChanges();
        }
    }
}
