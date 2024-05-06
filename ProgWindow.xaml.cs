using AdminService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
using System.Windows.Shapes;
using Label = System.Windows.Controls.Label;

namespace AdminTool_wpf
{
    /// <summary>
    /// Логика взаимодействия для ProgWindow.xaml
    /// </summary>
    public partial class ProgWindow : Window
    {
        IAdminService serviceClient;
        IISManager manager = new IISManager();
        private string currentUser;
        ProgWindow progWin;

        public ProgWindow(IAdminService serviceClient, string user)
        {
            currentUser = user;
            this.serviceClient = serviceClient;
            InitializeComponent();
            progWin = this;
            InitializeTabs();
        }

        private void InitializeTabs()
        {
            List<IISManager.ActionItem> userFunctions = manager.GetFunctionsForUser(currentUser);

            foreach (TabItem tab in functionTabs.Items)
            {
                if (userFunctions.Any(f => f.Name == (string)tab.Header))
                {
                    tab.Visibility = Visibility.Visible;
                    functionTabs.SelectedItem = tab;
                }
                else
                {
                    tab.Visibility = Visibility.Collapsed;
                }
            }

            if (userFunctions.Count == 0)
            {
                functionTabs.Visibility = Visibility.Collapsed;
                lblInfo.Visibility = Visibility.Visible;
            }
            else
            {
                lblInfo.Visibility = Visibility.Hidden;
            }
        }

        public void DataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var row = ItemsControl.ContainerFromElement((DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;
            if (row != null && !row.IsEditing)
            {
                if (row.IsSelected)
                {
                    row.IsSelected = false;
                }
                else
                {
                    row.IsSelected = true;
                }
            }
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.3));
            anim.Completed += (s, _) => Application.Current.Shutdown();
            this.BeginAnimation(OpacityProperty, anim);
        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) progWin.DragMove();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.3));
            anim.Completed += (s, _) =>
            {
                this.Close();
                if (Tag is Window authWindow)
                {
                    authWindow.Show();
                }
            };
            this.BeginAnimation(OpacityProperty, anim);
        }
    }
}
