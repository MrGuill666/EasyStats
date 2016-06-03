using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI;
using System.Collections.ObjectModel;
using Windows.Storage;
using System.IO;
using Windows.Storage.Streams;

namespace App3
{
    class Persistence
    {
        private const string filename = "data.json";
        public async Task
Save()
        {
            Task<string> task = Task.Factory.StartNew(() => { return JsonConvert.SerializeObject(datas); });
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (var fs = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (var outStream = fs.GetOutputStreamAt(0))
                {
                    using (var dw = new DataWriter(outStream))
                    {
                        dw.WriteString(task.Result);
                        await dw.StoreAsync();
                        dw.DetachStream();
                    }
                    await outStream.FlushAsync();
                }

            }


        }

        public async Task Load()
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
            using (var fs = await file.OpenAsync(FileAccessMode.Read))
            {
                using (var inStream = fs.GetInputStreamAt(0))
                {
                    using (var dr = new DataReader(inStream))
                    {
                        await dr.LoadAsync((uint)fs.Size);
                        var json = dr.ReadString((uint)fs.Size);
                        dr.DetachStream();
                        datas = JsonConvert.DeserializeObject<List<DataPersist>>(json);
                        System.Diagnostics.Debug.WriteLine(datas);
                    }
                }

            }

        }

        private class ItemPersist
        {
            public int Day { get; set; }
            public double Value { get; set; }
        }
        private class DataPersist
        {
            public Category Category { get; set; }
            public List<ItemPersist> Datas { get; set; }
        }

        private List<DataPersist> datas { get; set; } = new List<DataPersist>();

        public Dictionary<Category, ObservableCollection<Item>> Items
        {
            set
            {
                datas = new List<DataPersist>();
                foreach (var d in value)
                {
                    var list = new List<ItemPersist>();
                    datas.Add(new DataPersist() { Category = d.Key, Datas = list });
                    foreach (var i in d.Value)
                    {
                        list.Add(new ItemPersist() { Day = i.Day, Value = i.Value });
                    }
                }
            }


            get
            {
                var ret = new Dictionary<Category, ObservableCollection<Item>>();

                foreach (var d in datas)
                {
                    var list = new ObservableCollection<Item>();
                    ret.Add(d.Category, list);

                    foreach (var i in d.Datas)
                    {
                        list.Add(new Item() { Category = d.Category, Day = i.Day, Value = i.Value });
                    }
                }

                return ret;
            }
        }


    }
}
