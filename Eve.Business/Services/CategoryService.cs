using System;
using System.Collections.Generic;
using System.Linq;
using Eve.Business.Objects;

namespace Eve.Business.Services
{
    public class CategoryService : BaseService
    {
        public List<CategoryTree> CategoriesTree
        {
            get
            {
                List<CategoryTree> tree = new List<CategoryTree>();
                foreach (var c in context.Categories.Where(x => x.ParentCategoryId == null && x.CategoryIsActive).OrderBy(y => y.CategoryNameEs))
                    tree.Add(new CategoryTree(c));
                return tree;
            }
        }

        public Category GetCategory(int categoryId)
        {
            var c = context.Categories.Where(x => x.CategoryId == categoryId).FirstOrDefault();
            if (c != null)
                return new Category(c);
            return null;
        }
        
        public List<Category> GetCategories(string nameLike)
        {
            List<Category> lst = new List<Category>();
            foreach (var c in context.Categories.Where(x => x.CategoryNameEn.Contains(nameLike) || x.CategoryNameEs.Contains(nameLike)))
                lst.Add(new Category(c));
            return lst;
        }

        public Category SaveCategory(Category cat)
        {
            if (cat.Id > 0)
            {
                var entity = context.Categories.Where(x => x.CategoryId == cat.Id).FirstOrDefault();
                if (entity != null)
                {
                    entity.CategoryIsActive = cat.IsActive;
                    entity.CategoryNameEn = cat.NameEn;
                    entity.CategoryNameEs = cat.NameEs;
                    entity.ParentCategoryId = cat.ParentId;
                    if (context.SaveChanges() > 0)
                        return GetCategory(cat.Id);
                }
                return null;
            }
            else
            {
                var entity = cat.ToEntity();
                context.Categories.Add(entity);
                if (context.SaveChanges() > 0)
                    return GetCategory(entity.CategoryId);
                return null;
            }
        }
    }
}
