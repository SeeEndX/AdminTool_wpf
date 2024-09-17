using AdminService;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace AdminTool_wpf
{
    /// <summary>
    /// Логика взаимодействия для ViewReportWindow.xaml
    /// </summary>
    public partial class ViewReportWindow : Window
    {
        ViewReportWindow viewReportWin;
        private int user;
        IAdminService serviceClient;

        public ViewReportWindow(IAdminService serviceClient, int userId)
        {
            viewReportWin = this;
            InitializeComponent();
            user = userId;
            this.serviceClient = serviceClient;
            GetReports(user);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.3));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(OpacityProperty, anim);
        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) viewReportWin.DragMove();
        }

        private void GetReports(int user)
        {
            (List<string> reports, string username) = serviceClient.GetReportsForUser(user);
            if (reports.Count > 0)
            {
                userInfoTextBlock.Text = $"Действия пользователя {username}";
                foreach (string reportEntry in reports)
                {
                    reportTextBlock.AppendText(reportEntry);
                }
            }
            else
            {
                userInfoTextBlock.Text = $"{username} не выполнял действий";
                reportTextBlock.Visibility = Visibility.Collapsed;
                viewReportWin.Height = 100;
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.3));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(OpacityProperty, anim);
        }
    }
}
