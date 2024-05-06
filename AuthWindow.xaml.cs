using AdminService;
using System;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using static AdminTool_wpf.CustomMessageBox;

namespace AdminTool_wpf
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public static AuthWindow authWindow;
        IAdminService serviceClient;

        public AuthWindow()
        {
            InitializeComponent();
            InitializeServiceClient();
            authWindow = this;
        }

        private void InitializeServiceClient()
        {
            var binding = new NetTcpBinding();
            binding.SendTimeout = TimeSpan.FromSeconds(120);
            ChannelFactory<IAdminService> channelFactory =
                new ChannelFactory<IAdminService>(binding,
                new EndpointAddress("net.tcp://localhost:8000/AdminService"));
            serviceClient = channelFactory.CreateChannel();
        }

        private void Auth()
        {
            string login = tbLogin.Text;
            string password = tbPass.Text;

            User user = serviceClient.Authenticate(login, password);

            if (user != null)
            {
                if (user.Group == "Admin")
                {
                    AdminWindow adminWin = new AdminWindow(serviceClient);
                    adminWin.Tag = this;
                    adminWin.Show();
                    Hide();
                }
                else if (user.Group == "Dev")
                {
                    ProgWindow progWin = new ProgWindow(serviceClient, user.Login);
                    progWin.Tag = this;
                    progWin.Show();
                    Hide();
                }
            }
            else
            {
                var cmb = new CustomMessageBox("Неверные данные!",CustomMessageBox.MessageBoxButton.OK, MessageBoxType.Error);
                
                this.Effect = new BlurEffect { Radius = 10 };
                cmb.ShowDialog();
                this.Effect = null;
            }
        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) authWindow.DragMove();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Auth();
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
