using App3.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

namespace App3
{
    
    public class Category
    {
        
        public string Name { get; set; } = "Sample Name";
        
        public string Unit { get; set; } = "Unit";
        
        public Color Color { get; set; } = Colors.Red;
        
        public bool Visible { get; set; } = true;
    }
    public class Item
    {
        
        public Category Category { get; set; }
        
        public int Day { get; set; } = 2;
        
        public double Value { get; set; }
        
        public String InputValue { get; set; }
    }

    

    public class ViewModel
    {
        public ViewModel()
        {

            var c1 = new Category() { Name = "Category1", Unit = "kg", Color = Color.FromArgb(255, 255, 0, 0) };
            var c2 = new Category() { Name = "Category2", Unit = "mm", Color = Color.FromArgb(255, 0, 255, 0) };
            var c3 = new Category() { Name = "Category3", Unit = "ehh", Color = Color.FromArgb(255, 0, 0, 255) };
            var l1 = new ObservableCollection<Item>(); var l2 = new ObservableCollection<Item>(); var l3 = new ObservableCollection<Item>();

            l1.Add(new Item() { Category = c1, Day = 2, Value = 1 - 10 });
            l1.Add(new Item() { Category = c1, Day = 5, Value = 107 });
            l1.Add(new Item() { Category = c1, Day = 10, Value = 103 });
            l1 = new ObservableCollection<Item>(l1.OrderBy(i => i.Day).ToList());

            l2.Add(new Item() { Category = c2, Day = 2, Value = 0 });
            l2.Add(new Item() { Category = c2, Day = 12, Value = 66 });
            l2.Add(new Item() { Category = c2, Day = 7, Value = 55 });
            l2 = new ObservableCollection<Item>(l2.OrderBy(i => i.Day).ToList());

            l3.Add(new Item() { Category = c3, Day = 5, Value = 26 });
            l3.Add(new Item() { Category = c3, Day = 7, Value = 56 });
            l3.Add(new Item() { Category = c3, Day = 8, Value = 103 });
            l3 = new ObservableCollection<Item>(l3.OrderBy(i => i.Day).ToList());


            Items.Add(c1, l1);
            Items.Add(c2, l2);
            Items.Add(c3, l3);



        }

        private int _day=0;
        public int Day
        {
            get { return _day; }
            set
            {
                _day = value;

                if (TodayLine == null) TodayLine = new ObservableCollection<Pair>();
                TodayLine.Clear();
                TodayLine.Add(new Pair() { A=0, B=Day });
                TodayLine.Add(new Pair() { A = 100, B = Day });
            }
        }

        public class Pair
        {
            public int A { get; set; }
            public int B { get; set; }
        }

        public ObservableCollection<Pair> TodayLine { get; set; } 


        ObservableCollection<Item> _dailyItems = new ObservableCollection<Item>();
        public ObservableCollection<Item> DailyItems
        {
            get
            {
                return _dailyItems;
            }
        }

        ObservableCollection<Category> _categories = new ObservableCollection<Category>();
        public ObservableCollection<Category> Categories
        {
            get {
                foreach(var c in Items.Keys)
                {
                    if (!_categories.Contains(c))
                    {
                        _categories.Add(c);
                    }
                }
                return _categories;
            }
        }
        public void AddCategory(Category c)
        {
            _categories.Add(c);
            Items.Add(c, new ObservableCollection<Item>());
        }
        public void RemoveCategory(Category c)
        {
            Items.Remove(c);
            _categories.Remove(c);
            
        }


        public int Today { get { return 13; } }

        private List<Item> _newItems = null;

        public List<Item> NewItems
        {
            get
            {
                if (_newItems == null)
                {
                    _newItems = new List<Item>();
                    foreach (var c in Items.Keys)
                    {
                        
                        var n = (Items[c].Count!=0&&Items[c].Last().Day == Today )? Items[c].Last() : new Item() { Category = c, Day = Today };
                        _newItems.Add(n);
                    }
                }

                return _newItems;
            }
            set
            {
                _newItems = value;
            }
        }

        public Collection<ISeries> LineSeries { get; internal set; }
        private Dictionary<Category, LineSeries> _series = new Dictionary<Category, LineSeries>();
        public void RefreshLineSeries()
        {
            //Forget deleted categories
            var categories = _series.Keys.ToList();
            foreach (var c in categories)
            {
                if (!c.Visible || !Items.ContainsKey(c))
                {
                    LineSeries.Remove(_series[c]);
                    _series.Remove(c);
                }
            }
            //Display new categories
            foreach (var c in Items.Keys)
            {
                if (c.Visible && !_series.ContainsKey(c))
                {
                    var s = new LineSeries()
                    {
                        ItemsSource = Items[c],
                        IndependentValuePath = "Day",
                        DependentValuePath = "Value",
                        IsSelectionEnabled = false,
                        // = new SolidColorBrush() { Color=Colors.Red},
                        Foreground=new SolidColorBrush() { Color=Colors.Red },
                        
                        DependentRangeAxis = new LinearAxis()
                        {
                            Orientation = AxisOrientation.Y,
                            ShowGridLines = false
                        },
                        Title = c.Name

                    };
                    
                    
                    Style style = new Style(typeof(Control));
                    style.Setters.Add(new Setter(Control.VisibilityProperty, Visibility.Collapsed));                    
                    style.Setters.Add(new Setter(Control.WidthProperty, 0));
                    style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(c.Color)));
                    
                    s.DataPointStyle = style;

                    LineSeries.Add(s);
                    s.SelectionChanged += S_SelectionChanged;
                    _series.Add(c, s);
                }
            }

            Day = 8;


        }

        private void S_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            Day = (e.AddedItems.First() as Item).Day;
            DailyItems.Clear();
            foreach (var items in Items)
            {
                Item item = items.Value.ToList().Find(i => i.Day == Day);
                if (item != null) DailyItems.Add(item);
            }

        }

        
        public  Dictionary<Category, ObservableCollection<Item>> Items { get; set; } = new Dictionary<Category, ObservableCollection<Item>>();



        public void SaveNewItems()
        {

            foreach (var n in NewItems)
            {
                if (n.InputValue != null && n.InputValue.Length != 0)
                {
                    try
                    {
                        n.Value = Double.Parse(n.InputValue);
                        if (Items[n.Category].Count!=0&&Items[n.Category].Last().Day == n.Day)
                        {
                            Items[n.Category].Remove(Items[n.Category].Last());
                        }
                        Items[n.Category].Add(n);

                    }
                    catch (Exception e)
                    {
                        //TODO inform user?
                    }
                }
            }

            NewItems = null;

            Save();

        }

        public void Save()
        {
            Persistence p = new Persistence();
            p.Items = Items;
            p.Save();
        }
        public void Load()
        {
            Persistence p = new Persistence();
            p.Load();
            Items = p.Items;
            //Categories = p.Items.Keys.ToList();
        }

    }
}
