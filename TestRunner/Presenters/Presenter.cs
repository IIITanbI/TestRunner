using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TestRunner.BL;

namespace TestRunner.Presenters
{
    public class Presenter
    {
        MainWindow mw;
        Model model;
        public string Folder { get; set; } = @"D:\Visual Studio 2015\Projects\BrowserTestAdapter\Tests\bin\Debug";

        public Presenter(MainWindow mainWindow)
        {
            this.mw = mainWindow;
            this.model = new Model();

            this.mw.OnLoaded += async (o, arg) =>
            {
                var tests = await model.LoadTests(Folder);
                mw.UpdateTreeView(tests);
            };

            this.mw.OnTestRunClick +=  (o, arg) =>
            {
                var tests = mw.SelectedTests;
                model.RunTests(tests);
            };
        }

    }
}
