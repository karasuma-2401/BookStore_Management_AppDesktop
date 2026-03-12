using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookStore_Management_AppDesktop.Views.UserControls
{
    public partial class ForgotPasswordForm : UserControl
    {
        public ForgotPasswordForm()
        {
            InitializeComponent();
        }

        private bool _isPasting = false;
        private void Otp_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isPasting && sender is System.Windows.Controls.TextBox textBox && textBox.Text.Length > 0)
            {
                var request = new TraversalRequest(FocusNavigationDirection.Next);
                textBox.MoveFocus(request);
            }
        }

        private void MoveFocusOnBackSpace_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                if (sender is System.Windows.Controls.TextBox textBox)
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        e.Handled = true;
                        var request = new TraversalRequest(FocusNavigationDirection.Previous);
                        var elemnentWithFocus = Keyboard.FocusedElement as UIElement;
                        elemnentWithFocus?.MoveFocus(request);
                    }
                }
            }
        }
        private void OnOtpPaste (object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string pastedOtp = (string)e.DataObject.GetData(typeof(string));
                e.CancelCommand();

                if (!string.IsNullOrEmpty(pastedOtp) &&  pastedOtp.Length == 6 && int.TryParse(pastedOtp, out _))
                {
                    _isPasting = true;

                    Otp1.Text = pastedOtp[0].ToString();
                    Otp2.Text = pastedOtp[1].ToString();
                    Otp3.Text = pastedOtp[2].ToString();
                    Otp4.Text = pastedOtp[3].ToString();
                    Otp5.Text = pastedOtp[4].ToString();
                    Otp6.Text = pastedOtp[5].ToString();

                    Otp6.Focus();
                }
            }
        }
        private void MoveFocusOnEnter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                var request = new TraversalRequest(FocusNavigationDirection.Next);
                var elementWithFocus = Keyboard.FocusedElement as UIElement;
                elementWithFocus?.MoveFocus(request);
            }
        }
    }
}
