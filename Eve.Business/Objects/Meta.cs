using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eve.Business.Objects
{
    public class Meta
    {
        public int MetaId { get; set; }
        public string MetaNameEs { get; set; }
        public string MetaNameEn { get; set; }
        public List<MetaOption> Options { get; set; }

        internal Meta(Entity.Meta m)
        {
            this.MetaId = m.MetaId;
            this.MetaNameEn = m.MetaNameEn;
            this.MetaNameEs = m.MetaNameEs;
            this.Options = new List<MetaOption>();
            foreach (var mo in m.MetaOptions)
                this.Options.Add(new MetaOption(mo));
        }
        public Meta()
        {
            this.Options = new List<MetaOption>();
        }

        internal Entity.Meta ToEntity()
        {
            return new Entity.Meta
            {
                MetaId = this.MetaId,
                MetaNameEn = this.MetaNameEn,
                MetaNameEs = this.MetaNameEs
            };
        }
    }

    public class MetaOption
    {
        public int MetaOptionId { get; set; }
        public int MetaId { get; set; }
        public string MetaOptionNameEs { get; set; }
        public string MetaOptionNameEn { get; set; }
        public string MetaOptionValue { get; set; }
        internal string Xml
        {
            get
            {
                return string.Format("<m mid='{0}' oid='{1}' es='{2}' en='{3}' val='{4}' />",
                    MetaId, MetaOptionId, MetaOptionNameEs, MetaOptionNameEn, MetaOptionValue);
            }
        }

        internal MetaOption(Entity.MetaOption o)
        {
            this.MetaId = o.MetaId;
            this.MetaOptionId = o.MetaOptionId;
            this.MetaOptionNameEn = o.MetaOptionNameEn;
            this.MetaOptionNameEs = o.MetaOptionNameEs;
            this.MetaOptionValue = o.MetaOptionValue;
        }
        public MetaOption() { }

        internal Entity.MetaOption ToEntity()
        {
            return new Entity.MetaOption
            {
                MetaId = this.MetaId,
                MetaOptionId = this.MetaOptionId,
                MetaOptionNameEn = this.MetaOptionNameEn,
                MetaOptionNameEs = this.MetaOptionNameEs,
                MetaOptionValue = this.MetaOptionValue
            };
        }
    }
}
