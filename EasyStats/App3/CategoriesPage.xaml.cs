using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.UI.Input;
using System.Collections.ObjectModel;

namespace App3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CategoriesPage : Page
    {
        public CategoriesPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //This should be written here rather than the contructor
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            //This is where all your 'override backkey' code goes
            //You can put message dialog and/or cancel the back key using e.Handled = true;
            Frame.Navigate(typeof(MainPage));
            e.Handled = true;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            //remove the handler before you leave!
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private void Grid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);

            flyoutBase.ShowAt(senderElement);
            e.Handled = true;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var vm = Application.Current.Resources["ViewModel"] as ViewModel;

            
            vm.RemoveCategory((sender as MenuFlyoutItem).DataContext as Category);
            var cat=vm.Categories;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = Application.Current.Resources["ViewModel"] as ViewModel;
            var c = new Category();
            vm.AddCategory(c);
            
        }
    }
}
