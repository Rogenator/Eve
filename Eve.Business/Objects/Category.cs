using System;
using System.Collections.Generic;

namespace Eve.Business.Objects
{
    public class Category
    {
        public int CategoryId { get; set; }
        public int? ParentCategoryId { get; set; }
        public string CategoryNameEs { get; set; }
        public string CategoryNameEn { get; set; }
        public bool CategoryIsActive { get; set; }

        internal Category(Eve.Entity.Category cat)
        {
            this.CategoryId = cat.CategoryId;
            this.ParentCategoryId = cat.ParentCategoryId;
            this.CategoryNameEn = cat.CategoryNameEn;
            this.CategoryNameEs = cat.CategoryNameEs;
            this.CategoryIsActive = cat.CategoryIsActive;
        }
        public Category() { }

        internal Eve.Entity.Category ToEntity()
        {
            return new Entity.Category
            {
                CategoryId = CategoryId,
                CategoryIsActive = CategoryIsActive,
                CategoryNameEn = CategoryNameEn,
                CategoryNameEs = CategoryNameEs,
                ParentCategoryId = ParentCategoryId
            };
        }
    }

    public class CategoryTree : Category
    {
        public List<CategoryTree> Childs { get; set; }

        internal CategoryTree(Eve.Entity.Category cat) : base(cat)
        {
            this.Childs = new List<CategoryTree>();
            foreach (var c in cat.Category1)
                this.Childs.Add(new CategoryTree(c));
        }
        public CategoryTree() : base()
        {
            this.Childs = new List<CategoryTree>();
        }
    }
}
