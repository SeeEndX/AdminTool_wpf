using AdminService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {
        EditUserWindow editUserWin;
        IAdminService serviceClient;
        public ObservableCollection<FunctionItem> FunctionItems { get; set; }
        private string originalUsername;
        private string originalPass;

        public EditUserWindow(string username, string pass, IAdminService serviceClient)
        {
            originalUsername = username;
            originalPass = pass;

            this.serviceClient = serviceClient;
            editUserWin = this;
            InitializeComponent();
            tbLogin.Text = originalUsername;
            FunctionItems = new ObservableCollection<FunctionItem>();
            
            RefreshFunctionList();
            this.DataContext = this;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.1));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(OpacityProperty, anim);
        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) editUserWin.DragMove();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string newUsername = tbLogin.Text;
            string newPassword = string.IsNullOrEmpty(tbPass.Text) ? originalPass : tbPass.Text;
            string newPasswordConf = string.IsNullOrEmpty(tbPassConf.Text) ? originalPass : tbPassConf.Text;

            this.Effect = new BlurEffect { Radius = 10 };

            if (string.IsNullOrEmpty(newUsername))
            {
                ShowMessageBox("Заполните все поля", CustomMessageBox.MessageBoxType.Warning);
                return;
            }

            if (newPassword != newPasswordConf)
            {
                ShowMessageBox("Пароли не совпадают", CustomMessageBox.MessageBoxType.Error);
                return;
            }

            if (newUsername == originalUsername)
            {
                HandleSameUsername(newUsername, newPassword);
            }
            else
            {
                HandleNewUsername(newUsername, newPassword);
            }
        }

        private void HandleSameUsername(string username, string password)
        {
            if (password == originalPass)
            {
                if (FunctionsChanged())
                {
                    EditUser(username, password);
                    ShowMessageBox("Функции были изменены", CustomMessageBox.MessageBoxType.Success);
                }
                else
                {
                    ShowMessageBox("Данные не были изменены, Вы не вводили новых данных", CustomMessageBox.MessageBoxType.Warning);
                }
            }
            else
            {
                if (EditUser(username, password))
                {
                    ShowMessageBox("Пароль успешно изменен", CustomMessageBox.MessageBoxType.Success);
                }
                else
                {
                    ShowMessageBox("Ошибка при изменении пароля", CustomMessageBox.MessageBoxType.Error);
                }
            }
        }

        private void HandleNewUsername(string username, string password)
        {
            if (serviceClient.IsUserExists(username))
            {
                ShowMessageBox("Пользователь с таким логином уже существует", CustomMessageBox.MessageBoxType.Error);
            }
            else
            {
                if (EditUser(username, password))
                {
                    ShowMessageBox("Имя пользователя успешно изменено", CustomMessageBox.MessageBoxType.Success);
                }
                else
                {
                    ShowMessageBox("Ошибка при изменении имени пользователя", CustomMessageBox.MessageBoxType.Error);
                }
            }
        }

        private bool FunctionsChanged()
        {
            var assignedFunctionNames = serviceClient.GetAssignedFunctionNames(originalUsername);

            foreach (var functionItem in FunctionItems)
            {
                if (functionItem.IsChecked != assignedFunctionNames.Contains(functionItem.Name))
                {
                    return true;
                }
            }

            return false;
        }

        private void ShowMessageBox(string message, CustomMessageBox.MessageBoxType type)
        {
            CustomMessageBox messageBox = new CustomMessageBox(message,
                        CustomMessageBox.MessageBoxButton.OK, type);
            messageBox.ShowDialog();
            this.Effect = null;
        }

        private bool EditUser(string login, string password)
        {
            int rowsAffected = serviceClient.EditUser(originalUsername, login, password);
            AssignFunctionsToUser(login);
            return rowsAffected > 0;
        }

        private void RefreshFunctionList()
        {
            var allFunctionNames = serviceClient.GetAllFunctionNames() ?? new List<string>();
            var assignedFunctionNames = serviceClient.GetAssignedFunctionNames(originalUsername) ?? new List<string>();

            var newFunctionItems = allFunctionNames.Select(functionName => new FunctionItem
            {
                Name = functionName,
                IsChecked = assignedFunctionNames.Contains(functionName)
            });

            FunctionItems = new ObservableCollection<FunctionItem>(newFunctionItems);
        }

        private void AssignFunctionsToUser(string username)
        {
            List<string> selectedFunctionNames = new List<string>();

            foreach (var functionItem in FunctionItems)
            {
                if (functionItem.IsChecked)
                {
                    selectedFunctionNames.Add(functionItem.Name);
                }
            }

            serviceClient.SaveAssignedFunctions(username, selectedFunctionNames);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
