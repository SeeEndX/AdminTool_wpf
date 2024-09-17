using AdminService;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace AdminTool_wpf
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        AdminWindow adminWindow;
        IAdminService serviceClient;
        public AdminWindow(IAdminService serviceClient)
        {
            adminWindow = this;
            InitializeComponent();
            this.serviceClient = serviceClient;
            GetData();
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
            if (Mouse.LeftButton == MouseButtonState.Pressed) adminWindow.DragMove();
        }

        private void GetData()
        {
            try
            {
                dgvUsers.ItemsSource = serviceClient.GetUserData();
            }
            catch (Exception ex)
            {
                this.Effect = new BlurEffect { Radius = 10 };
                CustomMessageBox cmb = new CustomMessageBox(ex.Message, CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
            }
        }

        private void DeleteData(object sender, EventArgs e)
        {
            var messageBox = new CustomMessageBox("Вы уверены, " +
                "что хотите удалить выбранных пользователей?", CustomMessageBox.MessageBoxButton.OKCancel, CustomMessageBox.MessageBoxType.Warning);
            this.Effect = new BlurEffect { Radius = 10 };
            bool? result = messageBox.ShowDialog();
            if (result == true)
            {
                if (dgvUsers.SelectedItems.Count>0)
                {
                    foreach (User user in dgvUsers.SelectedItems)
                    {
                        serviceClient.DeleteUser(user.Id);
                    }
                    GetData();
                    this.Effect = null;
                }
                else
                {
                    messageBox = new CustomMessageBox("Вы не выбрали пользователей для удаления ", 
                        CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                    messageBox.ShowDialog();
                    this.Effect = null;
                }
            }
            else
            {
                this.Effect = null;
            }
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenAddUserForm();
        }

        private void OpenAddUserForm()
        {
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 10;
            this.Effect = blurEffect;
            AddUserWindow addUserWin = new AddUserWindow(serviceClient);
            addUserWin.Owner = this;
            addUserWin.Closed += AddUserForm_FormClosed;
            addUserWin.ShowDialog();
        }

        private void AddUserForm_FormClosed(object sender, EventArgs e)
        {
            this.Effect = null;
            GetData();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            OpenEditUserForm();
        }

        private void OpenEditUserForm()
        {
            if (dgvUsers.SelectedItems.Count == 1)
            {
                BlurEffect blurEffect = new BlurEffect();
                blurEffect.Radius = 10;
                this.Effect = blurEffect;

                var user = (User)dgvUsers.SelectedItem;
                string selectedUsername = user.Login;

                string origPass = serviceClient.GetPasswordByUsername(selectedUsername);

                EditUserWindow editUserForm = new EditUserWindow(selectedUsername, origPass, serviceClient);
                editUserForm.Owner = this;
                editUserForm.Closed += EditUserForm_FormClosed;
                editUserForm.ShowDialog();

            }
            else if (dgvUsers.SelectedItems.Count > 1)
            {
                CustomMessageBox cmb = new CustomMessageBox("Выберите только ОДНОГО пользователя для редактирования",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
            }
            else
            {
                CustomMessageBox cmb = new CustomMessageBox("Выберите пользователя для редактирования",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
            }
        }

        private void EditUserForm_FormClosed(object sender, EventArgs e)
        {
            this.Effect = null;
            GetData();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedItem != null)
            {
                User user= (User)dgvUsers.SelectedItem;
                int userId = user.Id;

                GenerateReport(userId);
            }
        }

        private void ViewReportForm_Closed(object sender, EventArgs e)
        {
            this.Effect = null;
        }

        private void GenerateReport(int userId)
        {
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 10;
            this.Effect = blurEffect;
            ViewReportWindow viewReportForm = new ViewReportWindow(serviceClient, userId);
            viewReportForm.Owner = this;
            viewReportForm.Closed += ViewReportForm_Closed;
            viewReportForm.ShowDialog();
        }
    }
}
