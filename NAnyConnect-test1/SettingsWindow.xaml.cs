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
using System.Windows.Shapes;

namespace NAnyConnect_test1
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {

        private MainController controller;

        public SettingsWindow(MainController controller) : base()
        {
            InitializeComponent();

            this.controller = controller;

            this.controller.SetUpSettingsWindow(this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            controller.SaveSettingsWindowChanges(form_vpnExecutableLocation.Text);
            Close();
        }




        public void SetVpnExecutableLocationTextbox(string s)
        {
            form_vpnExecutableLocation.Text = s;
        }



    }
}
