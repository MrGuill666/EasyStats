using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyStats.model;

namespace EasyStats.viewmodel
{
    class MainViewModel
    {
        public MainViewModel()
        {
            Day = 1;
            Categories = new List<Category>();
            var c1 = new Category(){ Unit="kg", CategoryName = "Category1"
            };
            var c2 = new Category() {  CategoryName = "Category2" };
            Categories.Add(c1);
            Categories.Add(c2);
            var d1 = new Data() { Day = 1, Category = c1, Value = 6.0 };
            var d2 = new Data() { Day = 1, Category = c2, Value = 7.0 };
            c1.DataList.Add(d1);
            c2.DataList.Add(d2);


        }

        public int Day { get; set; }
        public List<Data> DailyDataList {
            get
            {
                var datas = new List<Data>();
                foreach(var c in Categories)
                {
                    foreach(var d in c.DataList)
                    {
                        if (d.Day == Day)
                        {
                            datas.Add(d);
                        }
                    }
                }
                return datas;
            }
        }
        public List<Category> Categories { get; set; }
    }

}
