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

        public CategoryTree GetCategory(int categoryId)
        {
            var c = context.Categories.Where(x => x.CategoryId == categoryId).FirstOrDefault();
            if (c != null)
                return new CategoryTree(c);
            return null;
        }
        
        public List<CategoryTree> GetCategories(string nameLike)
        {
            List<CategoryTree> lst = new List<CategoryTree>();
            foreach (var c in context.Categories.Where(x =>
                (x.CategoryNameEn.Contains(nameLike) || x.CategoryNameEs.Contains(nameLike)) && x.ParentCategoryId == null
                ).OrderBy(c => c.CategoryNameEn))
                lst.Add(new CategoryTree(c));
            return lst;
        }

        public List<Category> GetCategoriesList()
        {
            List<Category> lst = new List<Category>();
            foreach (var c in context.Categories.Where(x => x.CategoryIsActive).OrderBy(x => x.CategoryNameEn))
                lst.Add(new Category(c));
            return lst;
        }

        public Category SaveCategory(Category cat)
        {
            if (cat.CategoryId > 0)
            {
                var entity = context.Categories.Where(x => x.CategoryId == cat.CategoryId).FirstOrDefault();
                if (entity != null)
                {
                    entity.CategoryIsActive = cat.CategoryIsActive;
                    entity.CategoryNameEn = cat.CategoryNameEn;
                    entity.CategoryNameEs = cat.CategoryNameEs;
                    entity.ParentCategoryId = cat.ParentCategoryId;
                    if (context.SaveChanges() > 0)
                        return GetCategory(cat.CategoryId);
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
