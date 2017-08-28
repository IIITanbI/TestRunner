using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace TestRunner
{
    /// <summary>
    /// Interaction logic for TestsTreeView.xaml
    /// </summary>
    public partial class TestsTreeView : UserControl
    {
        public TestsTreeView()
        {
            InitializeComponent();
            this.TreeView.Items.Add(new TreeViewItem() { Header = 3333 });
            this.TreeView.Items.Add(new TreeViewItem() { Header = 4444 });
            this.TreeView.Items.Add(new TreeViewItem() { Header = 5555 });
            this.TreeView.Items.Add(new TreeViewItem() { Header = 6666 });
            this.TreeView.Items.Add(new TreeViewItem() { Header = 7777 });
            this.TreeView.Items.Add(new TreeViewItem() { Header = 8888 });

            this.TreeView.SelectedItemChanged += OnTreeViewItemSelected;
        }


        private static readonly PropertyInfo IsSelectionChangeActiveProperty = typeof(TreeView).GetProperty("IsSelectionChangeActive", BindingFlags.NonPublic | BindingFlags.Instance);
        private List<TreeViewItem> selectedItems = new List<TreeViewItem>();
        public List<TestCase> SelectedTests => this.selectedItems.Select(si => (TestCase)si.Tag).ToList();

        public void UpdateTreeView(List<TestCase> tests)
        {
            this.TreeView.Items.Clear();
            foreach (var test in tests)
            {
                this.TreeView.Items.Add(new TreeViewItem() { Header = test.FullyQualifiedName, Tag = test });
            }
        }

        public void OnTreeViewItemSelected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeViewItem = this.TreeView.SelectedItem as TreeViewItem;
            if (treeViewItem == null)
            {
                this.selectedItems = new List<TreeViewItem>();
                return;
            }

            // suppress selection change notification
            // select all selected items
            // then restore selection change notifications
            var isSelectionChangeActive = IsSelectionChangeActiveProperty.GetValue(this.TreeView, null);
            IsSelectionChangeActiveProperty.SetValue(this.TreeView, true, null);

            // allow multiple selection
            // when control key is pressed
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                // select previous items
                selectedItems.ForEach(item => item.IsSelected = true);
                //if (treeViewItem.IsSelected)
                //{
                //    treeViewItem.IsSelected = false;
                //}
            }
            else
            {
                // unselect all another selected items
                selectedItems.ForEach(item => item.IsSelected = false);
                // clear selected items list
                this.selectedItems = new List<TreeViewItem>();
            }

            IsSelectionChangeActiveProperty.SetValue(this.TreeView, isSelectionChangeActive, null);
            selectedItems.Add(treeViewItem);
        }
    }
}
