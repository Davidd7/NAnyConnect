using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NAnyConnect_test1
{
    public class MainController
    {

        private int selectedVpn = -1;
        private int selectedVpnSleep = -1;

        private IReadOnlyList<Account> accounts;
        private MainWindow window;

        private void LoadData()
        {
            accounts = Account.GetReadOnlyAccounts();
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
            mainWindow.SetUpNoConnectionButton(  (selectedVpn != -1 ? MainWindow.ButtonState.EnabledUnselected : MainWindow.ButtonState.EnabledSelected)  );
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


        public void DeleteSlot(int slot)
        {
            if (GetIdFromSlot(slot).Equals(selectedVpn))
                VpnEnd(false); // ui will be updated at the end of this method
            Account.DeleteAccount(  GetIdFromSlot(slot)  );
            accounts = Account.GetReadOnlyAccounts();
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




        public void SetUpSettingsWindow(SettingsWindow sw)
        {
            sw.SetVpnExecutableLocationTextbox(Options.VpnExecutableLocation);
            sw.SetshowCommandPromptWindowsCheckbox(Options.ShowCommandPromptWindows);
        }

        public void SaveSettingsWindowChanges(string vpnExecutableLocation, bool showCommandPromptWindows) {
            Options.VpnExecutableLocation = vpnExecutableLocation;
            Options.ShowCommandPromptWindows = showCommandPromptWindows;
        }

        public void MainWindowShowLoading(bool b, MainController c) {
            System.Windows.Application.Current?.Dispatcher.BeginInvoke(() =>
            {
                window.SetLoading(b);
                Mouse.OverrideCursor = b ? Cursors.AppStarting : Cursors.Arrow;
            });
        }


        public void SaveChanges(int slot, string displayName, string script, ref string password) {

            if (slot >= accounts.Count)
            {
                int id = Account.CreateNewAccount(displayName, script);
                PasswordManager.SetPassword(id, ref password);
            }
            else // Update existing account
            {
                Account.UpdateAccount(accounts[slot].Id, displayName, script);
                if (password != "") {
                    PasswordManager.SetPassword(accounts[slot].Id, ref password);
                }
                if (selectedVpn.Equals(accounts[slot].Id))
                {
                    VpnRestart();
                }

            }

            AdaptConnectionButtons(window);
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

            VpnCmdConnector.Connect(Options.VpnExecutableLocation, script, (b) => MainWindowShowLoading(b, this));

            selectedVpn = accounts[slot].Id;
            AdaptConnectionButtons(window);
        }
        public void VpnStartWithId(int id)
        {
            VpnStart(  GetSlotFromId(id)  );
        }

        private int GetSlotFromId(int id) {
            return Account.GetPositionFromId(id);
        }

        private int GetIdFromSlot(int slot) {
            if (slot >= accounts.Count) {
                return -1;
            }
            return accounts[slot].Id;
        }


        public void VpnEnd(bool updateUI = true)
        {
            VpnCmdConnector.Disconnect(Options.VpnExecutableLocation, (b) => MainWindowShowLoading(b, this));

            selectedVpn = -1;
            if (updateUI)
                AdaptConnectionButtons(window);
        }


        public void VpnRestart()
        {
            int runningId = selectedVpn; // Copy ValueType
            VpnEnd();
            VpnStartWithId(runningId);
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
