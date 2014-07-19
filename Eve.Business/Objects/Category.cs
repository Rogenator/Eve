using System;
using System.Collections.Generic;

namespace Eve.Business.Objects
{
    public class Category
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string NameEs { get; set; }
        public string NameEn { get; set; }
        public bool IsActive { get; set; }

        internal Category(Eve.Entity.Category cat)
        {
            this.Id = cat.CategoryId;
            this.ParentId = cat.ParentCategoryId;
            this.NameEn = cat.CategoryNameEn;
            this.NameEs = cat.CategoryNameEs;
            this.IsActive = cat.CategoryIsActive;
        }
        public Category() { }

        internal Eve.Entity.Category ToEntity()
        {
            return new Entity.Category
            {
                CategoryId = Id,
                CategoryIsActive = IsActive,
                CategoryNameEn = NameEn,
                CategoryNameEs = NameEs,
                ParentCategoryId = ParentId
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
