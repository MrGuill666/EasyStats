using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStats.model
{
    class Category
    {
        public Category()
        {
            DataList = new List<Data>();
            CategoryName = "DefaultCategoryName";
        }
        public string CategoryName { get; set; }
        public IList<Data> DataList { get; set; }
        public string Unit { get; set; }
    }
}
