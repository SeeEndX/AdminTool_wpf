using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace AdminTool_wpf
{
    /// <summary>
    /// Логика взаимодействия для CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public static CustomMessageBox cmbWindow;
        public enum MessageBoxButton
        {
            OK,
            OKCancel
        }

        public enum MessageBoxType
        {
            Error,
            Warning,
            Success
        }

        public CustomMessageBox(string message, MessageBoxButton buttonType, MessageBoxType messageType)
        {
            cmbWindow = this;
            InitializeComponent();
            MessageTextBlock.Text = message;

            if (messageType == MessageBoxType.Error)
            {
                titleLbl.Content = "Ошибка";
                titleLbl.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d63a2f"));
            }
            else if (messageType == MessageBoxType.Warning)
            {
                titleLbl.Content = "Предупреждение";
                titleLbl.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d6802f"));
            }
            else if (messageType == MessageBoxType.Success)
            {
                titleLbl.Content = "Успех";
                titleLbl.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6fd62f"));
            }

            if (buttonType == MessageBoxButton.OK)
            {
                OkButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Collapsed;
                Grid.SetColumn(OkButton, 1);
            }
            else if (buttonType == MessageBoxButton.OKCancel)
            {
                OkButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Visible;
                Grid.SetColumn(OkButton, 0);
            }
        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) cmbWindow.DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.1));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(OpacityProperty, anim);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
