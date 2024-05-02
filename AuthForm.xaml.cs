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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace AdminTool_wpf
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public static AuthWindow authWindow;
        public AuthWindow()
        {
            InitializeComponent();
            authWindow = this;
        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) authWindow.DragMove();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = tbLogin.Text;
            var password = tbPass.Text;
            MessageBox.Show($"Вход выполнен! {login} {password}");
            
            // Код для обработки нажатия кнопки "Войти"
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.3));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(OpacityProperty, anim);
        }
    }
}
