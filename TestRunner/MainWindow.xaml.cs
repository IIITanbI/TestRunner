using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
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

namespace TestRunner
{
    public interface IMainWindow
    {

    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        public event EventHandler<RoutedEventArgs> OnLoaded;
        public event EventHandler<RoutedEventArgs> OnTestRunClick;

        public MainWindow()
        {
            var a = this.TestTreeView1;
            InitializeComponent();
            new Presenters.Presenter(this);
        }

        public List<TestCase> SelectedTests => this.TestTreeView1.SelectedTests;


        public void UpdateTreeView(List<TestCase> tests)
        {
            this.TestTreeView1.UpdateTreeView(tests);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.OnLoaded?.Invoke(sender, e);
        }
        private void Button_RunTests_Click(object sender, RoutedEventArgs e)
        {
            this.OnTestRunClick?.Invoke(sender, e);
        }
    }
}
