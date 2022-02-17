using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAnyConnect_test1
{
    public class MainController
    {

        //private bool reconnectAfterSleep = true;

        private int selectedVpn = -1;
        private int selectedVpnSleep = -1;
        //string location = "";


        private List<Account> accounts = new List<Account>();
        private MainWindow window;

        private void LoadData()
        {
            accounts = Account.GetAccounts();
        }


        public void AdaptConnectionButtons(MainWindow mainWindow)
        {
            MainWindow.ButtonState state;
            foreach (int i in new int[] { 0, 1 })
            {
                state = MainWindow.ButtonState.Disabled; // Set up variable with disabled and change to enabled if necessary
                if (accounts.Count > i && !selectedVpn.Equals(accounts[i].Id)) {
                    state = MainWindow.ButtonState.EnabledUnselected;
                }
                else if (accounts.Count > i && selectedVpn.Equals(accounts[i].Id))
                {
                    state = MainWindow.ButtonState.EnabledSelected;
                }
                mainWindow.SetUpConnectButton(i, (accounts.Count > i ? accounts[i].DisplayName : "empty"), state);
            }
        }

        public void AdaptReconnectAfterSleep() {
            window.checkbox_reconnectAfterSleep.IsChecked = Options.ReconnectAfterSleep;
        }

        public void RecognizeChangeReconnectAfterSleep() {
            Options.ReconnectAfterSleep = window.checkbox_reconnectAfterSleep.IsChecked ?? true;
        }


        public MainController(MainWindow window)
        {
            LoadData();
            this.window = window;
        }

        public void SetUpMain() {
            AdaptReconnectAfterSleep();
            AdaptConnectionButtons(window);
        }





        public void SetUpEditWindow(EditWindow ew, int slot) {
            if (accounts.Count <= slot)
            {
                ew.SetScriptTextbox("connect _enterVpnUrlHere_\n_enterYourUsernameHere_\n<password>");
                ew.SetDisplaynameBox("My new connection");
                ew.SetPasswordBox("");
            }
            else
            {
                ew.SetDisplaynameBox(accounts[slot].DisplayName);
                ew.SetScriptTextbox(accounts[slot].Script);
                ew.SetPasswordBox(PasswordManager.GetPassword(accounts[slot].Id));
            }
        }

        public void saveChanges(int slot, string displayName, string script, ref string password) {

            if (slot >= accounts.Count)
            {

                int id = Account.createNewAccount(displayName, script);
                PasswordManager.SetPassword(id, ref password);

            }
            else {

                Account.updateAccount(accounts[slot].Id, displayName, script);
                if (password != "") {
                    PasswordManager.SetPassword(accounts[slot].Id, ref password);
                }

            }

            AdaptConnectionButtons(window);
            if (selectedVpn.Equals(accounts[slot].Id)) {
                VpnRestart();
            }
        }






        public void VpnStart(int slot)
        {
            if (slot >= accounts.Count)
                return;

            if (!selectedVpn.Equals(-1)) {
                VpnEnd();
            }

            string pwd = PasswordManager.GetPassword(accounts[slot].Id);

            string script = accounts[slot].Script;
            script = script.Replace("<password>", pwd);

            VpnCmdConnector.Connect(Options.VpnExecutableLocation, script);

            selectedVpn = accounts[slot].Id;
            AdaptConnectionButtons(window);
        }
        public void VpnStartWithId(int id)
        {
            VpnStart(  getSlotFromId(id)  );
        }

        private int getSlotFromId(int id) {
            int slot = accounts.FindIndex(a => a.Id == id);
            return (slot < 2 ? slot : -1);
        }

        private int getIdFromSlot(int slot) {
            if (slot >= accounts.Count) {
                return -1;
            }
            return accounts[slot].Id;
        }


        public void VpnEnd()
        {
            VpnCmdConnector.Disconnect(Options.VpnExecutableLocation);

            selectedVpn = -1;
            AdaptConnectionButtons(window);
        }


        public void VpnRestart()
        {
            int runningId = selectedVpn; // Copy ValueType
            VpnEnd();
            VpnStart(runningId);
        }


        public void VpnSleep() {
            selectedVpnSleep = selectedVpn;
            selectedVpn = -1;
            VpnEnd();
        }

        public void VpnWakeup() {
            if (Options.ReconnectAfterSleep && !selectedVpnSleep.Equals(-1)) {
                VpnStartWithId(selectedVpnSleep);
                selectedVpn = selectedVpnSleep;
                selectedVpnSleep = -1;
            }
        }



    }
}
