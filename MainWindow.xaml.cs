using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MyCalculator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            TabItem tabItem = leftTabControl.Items[Convert.ToInt32(menuItem.Tag)] as TabItem;
            tabItem.Visibility = Visibility.Visible;
            tabItem.IsSelected = true;
        }

        private void CollapseLeftTabItem(object sender, EventArgs e)
        {
            Button x = sender as Button;
            TabItem tabItem = leftTabControl.Items[Convert.ToInt32(x.Tag)] as TabItem;
            tabItem.Visibility = Visibility.Collapsed;

            WrapPanel content = tabItem.Content as WrapPanel;
            if (content != null)
                content.Visibility = Visibility.Collapsed;
        }

        private void ShowTabItemContent(object sender, MouseButtonEventArgs e)
        {
            TabItem tabItem = sender as TabItem;
            WrapPanel content = tabItem.Content as WrapPanel;
            if (content != null)
            {
                if (content.Visibility == Visibility.Collapsed)
                    content.Visibility = Visibility.Visible;
                else
                    content.Visibility = Visibility.Collapsed;
            }
        }

        private void NewWorksheet(object sender, EventArgs e)
        {
            TabItem worksheet = new TabItem();
            midTabControl.Items.Add(worksheet);

            worksheet.IsSelected = true;
            worksheet.MouseDoubleClick += RenameMidTabItem;

            {
                WrapPanel header = new WrapPanel();
                worksheet.Header = header;

                {
                    TextBox name = new TextBox();
                    header.Children.Add(name);

                    SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);
                    name.Background = transparent;
                    name.BorderThickness = new Thickness(0);
                    name.IsEnabled = false;
                    name.Text = "New worksheet";
                    name.KeyDown += SealRename;

                    Button x = new Button();
                    header.Children.Add(x);

                    x.Content = " X ";
                    x.Click += CloseMidTabItem;
                    x.Background = transparent;
                    x.BorderThickness = new Thickness(0);
                }
            }

            {
                WrapPanel content = new WrapPanel();
                worksheet.Content = content;
            }
        }

        private void CloseMidTabItem(object sender, EventArgs e)
        {
            Button x = sender as Button;
            WrapPanel header = x.Parent as WrapPanel;
            TabItem worksheet = header.Parent as TabItem;
            if(worksheet != null)
            {
                midTabControl.Items.Remove(worksheet);
            }
        }

        private void RenameMidTabItem(object sender, EventArgs e)
        {
            TabItem worksheet = sender as TabItem;
            WrapPanel header = worksheet.Header as WrapPanel;
            if (header.Children[0] != null)
            {
                TextBox name = header.Children[0] as TextBox;
                name.IsEnabled = true;
                name.BorderThickness = new Thickness(1);
            }
        }

        private void SealRename(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                TextBox name = sender as TextBox;
                name.BorderThickness = new Thickness(0);
                name.IsEnabled = false;
            }
        }
    }
}
