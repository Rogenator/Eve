using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eve.Business.Util
{
    public class GridOptions
    {
        public int CurrentPage { get; set; }
        public int RowsPerPage { get; set; }
        public int Skip
        {
            get
            {
                return this.CurrentPage * this.RowsPerPage;
            }
        }
        public string OrderBy { get; set; }
        public bool OrderDescending { get; set; }

        public GridOptions()
        {
            this.CurrentPage = 0;
            this.RowsPerPage = 25;
            this.OrderBy = null;
            this.OrderDescending = false;
        }
    }
}
