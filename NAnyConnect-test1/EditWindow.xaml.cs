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
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {

        private int slot = -1;
        private MainController controller;

        public EditWindow(int slot, MainController controller) : base()
        {
            InitializeComponent();

            this.slot = slot;
            this.controller = controller;

            this.controller.SetUpEditWindow(this, this.slot);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var displayName = form_displayName.Text;
            var script = form_script.Text;
            var password = form_password.Password;

            controller.saveChanges(slot, displayName, script, ref password);

            Close();
        }




        public void SetScriptTextbox(string s)
        { 
            form_script.Text = s;
        }

        public void SetDisplaynameBox(string s)
        {
            form_displayName.Text = s;
        }

        public void SetPasswordBox(string s)
        {
            form_password.Password = s; // Unnötiges Risiko hier Passwort reinzuladen. Besser: Passwort leer lassen auch wenn existiert und nur zum Ändern des Passworts nützlich sein
        }


    }
}
