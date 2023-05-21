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
        /*
         * This should be am app that lets you calculate using more advanced methods, stuff you're not able to do with a regular calculator:
                - create variables and use them by their custom names
                - create functions, calculate functions outputs for given, custom inputs
                - represent functions using customizable plot panels
        */
        public MainWindow()
        {
            InitializeComponent();
        }

        //Show left TabItem
        //This one is for the items that appear on the left toolbar (it's a TabControl actually). Initially, there are some TabItems are not visible (collapsed), so you have to access the View menu and click on the specific item to make its tab appear
        private void ViewMenuItem_Click(object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            TabItem tabItem = leftTabControl.Items[Convert.ToInt32(menuItem.Tag)] as TabItem;
            tabItem.Visibility = Visibility.Visible;
            tabItem.IsSelected = true;
        }

        //Hide left TabItem
        //This one is for collapsing the TabItems on the left toolbar (TabControl) when you click on the 'X' button inside the TabItem's header.
        private void CollapseLeftTabItem(object sender, EventArgs e)
        {
            Button x = sender as Button;
            TabItem tabItem = leftTabControl.Items[Convert.ToInt32(x.Tag)] as TabItem;
            tabItem.Visibility = Visibility.Collapsed;

            WrapPanel content = tabItem.Content as WrapPanel;
            if (content != null)
                content.Visibility = Visibility.Collapsed;
        }

        //Show content on left TabItem
        //This one is so that you can see the content of a TabItem when you double click its header, as initially, the content is also collapsed.
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

        //Add TabItem to middle TabControl
        //It creates a new worksheet (TabItem) in the middle (main) section. Hierarchy = TabItem( Header( WrapPanel( TextBox, Button)), Content( WrapPanel))
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

                content.Orientation = Orientation.Vertical;
            }
        }

        //Remove TabItem from middle TabControl
        //It closes (remove from TabControl) the TabItem from the middle TabControl when 'X' button is press on the header of the TabItem.
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

        //Rename header of middle TabItem - part 1 (this event is only a half, the other half is on the next event)
        //It enables the TextBox from the header of the TabItem when double clicked on the TabItem. I wanna change this in the future, because it not only enables the TextBox when clicked on the header, but also on the content and i don't like it.
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

        //Rename header of middle TabItem - part 2
        //It disables the TextBox (so it will look like a TextBlock again) when, while the text is selected, 'Enter' key is pressed.
        private void SealRename(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                TextBox name = sender as TextBox;
                name.BorderThickness = new Thickness(0);
                name.IsEnabled = false;
            }
        }

        //Create a space for variables
        /*
            When requested (by Click on the 'Variable box' button located in the Content of the 'Toolbox' item on the left toolbar (TabControl)),
        it creates a WrapPanle with a Button inside, that creates 'Variables' inside the WrapPanel when clicked.
            !If requested while no worksheet (TabItem on middle TabControl) is selected (it also has to be opened, obv), it will show a MessageBox
        with instructions.
        */
        private void CreateVariableBox(object sender, RoutedEventArgs e)
        {
            if (midTabControl.SelectedContent != null)
            {
                WrapPanel variableBox = new WrapPanel();
                WrapPanel content = midTabControl.SelectedContent as WrapPanel;
                content.Children.Add(variableBox);

                variableBox.Orientation = Orientation.Vertical;

                {
                    Button addVariable = new Button();
                    variableBox.Children.Add(addVariable);

                    addVariable.Content = "Add variable";
                    addVariable.Click += AddVariable;
                }
            }
            else
                MessageBox.Show("You must first CREATE and SELECT a workspace sheet to create a item! Try File->New->Worksheet");
        }

        //Add 'Variable' to the 'Variable box'
        //Adds a 'Variable' inside the 'Variable box' when clicked on the 'Add variable' button. Hierarchy = WrapPanel( TextBox, TextBlock, TextBox, Button)
        //Btw, the button after the last TextBox is for enabling the TextBoxes.
        private void AddVariable(object sender, EventArgs e)
        {
            WrapPanel variable = new WrapPanel();
            Button butt = sender as Button;
            WrapPanel content = butt.Parent as WrapPanel;
            content.Children.Add(variable);

            variable.Orientation = Orientation.Horizontal;
            variable.Margin = new Thickness(0, 5, 0, 5);

            {
                TextBox name = new TextBox();
                variable.Children.Add(name);

                name.KeyDown += ValidateRenameVariable;
                name.Margin = new System.Windows.Thickness(0, 0, 5, 0);
                name.MinWidth = 30;
                

                TextBlock equalSign = new TextBlock();
                variable.Children.Add(equalSign);

                equalSign.Text = "=";

                TextBox value = new TextBox();
                variable.Children.Add(value);

                value.KeyDown += ValidateRenameVariable;
                value.Margin = new System.Windows.Thickness(5, 0, 5, 0);
                value.MinWidth = 30;

                Button edit = new Button();
                variable.Children.Add(edit);

                edit.Content = "Edit";
                edit.Click += EditVariable;
                edit.Padding = new Thickness(2, 0, 2, 0);
                //edit.BorderThickness = new Thickness(0);
                //edit.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        //Edit the name and value of a 'Variable' - part 1 (again, this is only a half of the editing process)
        //When 'Edit' button is clicked, it enables the TextBoxes of a 'Variable'.
        private void EditVariable(object sender, EventArgs e)
        {
            Button edit = sender as Button;
            WrapPanel variable = edit.Parent as WrapPanel;
            TextBox name = variable.Children[0] as TextBox;
            TextBox value = variable.Children[2] as TextBox;

            name.IsEnabled = true;
            value.IsEnabled = true;
        }

        //Edit the name and value of a 'Variable' - part 2
        //If 'Enter' key is press while the TextBox is selected, it disables the TextBox.
        private void ValidateRenameVariable(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                TextBox textBox = sender as TextBox;
                textBox.IsEnabled = false;
            }
        }
    }
}
