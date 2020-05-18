using System;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Windows;
using System.Windows.Input;

namespace ResetPassword
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Inir
            InitializeComponent();

            // Load Dir Info
            GetDirectoryInfo();

            // Add Instructions
            Instructions();
        }

        // Get Dir Info
        private void GetDirectoryInfo()
        {
            try
            {
                var Forest = System.DirectoryServices.ActiveDirectory.Forest.GetCurrentForest();
                foreach (Domain domain in Forest.Domains)
                {
                    _domain.Items.Add(domain);
                }
            }
            catch (Exception Ex)
            {
                _status.Text = String.Format("Error getting directory info: {0}", Ex.Message);
                _status.Foreground = System.Windows.Media.Brushes.DarkRed;
            }
        }

        private void Instructions()
        {
            _status.Text += String.Format("{0}  Requirements:{0}  Passwords can only be changed once per day.{0}  You may not reuse any of your last 5 passwords.{0}  Dash Accounts must use passwords that are at least 15 characters long.{0}  Password may not contain any part of your name.{0}  You must have at least one of each: Uppercase,Lowercase,Number,Special Character.", Environment.NewLine);
        }

        private void _password_new_reveal_Checked(object sender, RoutedEventArgs e)
        {
            _password_new_revealed.Visibility = Visibility.Visible;
        }

        private void _password_new_reveal_Unchecked(object sender, RoutedEventArgs e)
        {
            _password_new_revealed.Visibility = Visibility.Hidden;
        }

        private void _password_new_Key(object sender, KeyEventArgs e)
        {
            _password_new_revealed.Content = _password_new.Password.ToString();
        }

        private void _submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Reset the password
                using (var context = new PrincipalContext(ContextType.Domain, _domain.SelectedItem.ToString()))
                {
                    using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, _username.Text))
                    {
                        user.ChangePassword(_password_current.Password, _password_new.Password);
                        user.Save();
                    }
                }

                _status.Text = "Password has been set";
                _status.Foreground = System.Windows.Media.Brushes.DarkGreen;
            }
            catch (Exception Ex)
            {
                _status.Text = String.Format("{0}", Ex.Message);
                _status.Foreground = System.Windows.Media.Brushes.DarkRed;
                Instructions();
            }
        }
    }
}
