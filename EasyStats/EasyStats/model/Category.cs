using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStats.model
{
    class Category
    {
        public string Name { get; set; }
        public IList<Data> DataList { get; set; }
        public string Unit { get; set; }
    }
}
