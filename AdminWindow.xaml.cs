using AdminService;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            Closed += AdminWindow_Closed;
            GetData();
        }

        public void DataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var row = ItemsControl.ContainerFromElement((DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;
            if (row != null && !row.IsEditing)
            {
                // Если строка уже выделена, снимаем выделение
                if (row.IsSelected)
                {
                    row.IsSelected = false;
                }
                else
                {
                    // Выделяем строку
                    row.IsSelected = true;
                }
            }
        }

        private void AdminWindow_Closed(object sender, EventArgs e)
        {
            if (Tag is Window authWindow)
            {
                authWindow.Show();
            }
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

        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) adminWindow.DragMove();
        }

        private bool isMaximized = false;
        private void Maximize(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (isMaximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 800;
                    this.Height = 450;

                    isMaximized = false;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                    isMaximized = true;
                }
            }
        }

        private void GetData()
        {
            try
            {
                dgvUsers.ItemsSource = serviceClient.GetUserData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(OpacityProperty, anim);
        }

        /*private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenAddUserForm();
        }

        private void OpenAddUserForm()
        {
            AddUserForm addUserForm = new AddUserForm(serviceClient);
            addUserForm.Tag = this;
            addUserForm.FormClosed += AddUserForm_FormClosed;
            addUserForm.Show(this);

            this.Enabled = false;
        }

        private void AddUserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Enabled = true;
            GetData();
        }*/

        /*private void btnEdit_Click(object sender, EventArgs e)
        {
            OpenEditUserForm();
        }*/

        /*private void OpenEditUserForm()
        {
            if (dgvUsers.SelectedCells.Count > 0)
            {
                int rowIndex = dgvUsers.SelectedCells[0].RowIndex;
                string selectedUsername = dgvUsers.Rows[rowIndex].Cells["Пользователь"].Value.ToString();

                EditUserForm editUserForm = new EditUserForm(selectedUsername, serviceClient);
                editUserForm.Tag = this;
                editUserForm.FormClosed += EditUserForm_FormClosed;
                editUserForm.Show(this);
                this.Enabled = false;
            }
            else
            {
                MessageBox.Show("Выберите пользователя для редактирования.");
            }
        }*/

        /*private void EditUserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Enabled = true;
            GetData();
        }*/

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
