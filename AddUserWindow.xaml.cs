using AdminService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace AdminTool_wpf
{
    /// <summary>
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        AddUserWindow addUserWin;
        IAdminService serviceClient;

        public ObservableCollection<FunctionItem> FunctionItems { get; set; }

        public AddUserWindow(IAdminService serviceClient)
        {
            this.serviceClient = serviceClient;
            addUserWin = this;
            InitializeComponent();
            FunctionItems = new ObservableCollection<FunctionItem>();
            this.DataContext = this;
            RefreshFunctionList();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.1));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(OpacityProperty, anim);
        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) addUserWin.DragMove();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string newUsername = tbLogin.Text;
            string newPassword = tbPass.Text;
            string newPasswordConf = tbPassConf.Text;

            this.Effect = new BlurEffect { Radius = 10 };
            if (newUsername != "" && newPassword != "" && newPasswordConf != "")
            {
                if (!serviceClient.IsUserExists(newUsername) && newPassword != newPasswordConf)
                {
                    ShowMessageBox("Пароли не совпадают", CustomMessageBox.MessageBoxType.Error);
                    return;
                }
                else if (!serviceClient.IsUserExists(newUsername) && AddUser(newUsername, newPassword))
                {
                    ShowMessageBox("Пользователь успешно добавлен", CustomMessageBox.MessageBoxType.Success);
                    return;
                }
                else
                {
                    ShowMessageBox("Пользователь с таким логином уже существует", CustomMessageBox.MessageBoxType.Error);
                    return;
                }
            }
            else
            {
                ShowMessageBox("Заполните все поля", CustomMessageBox.MessageBoxType.Error);
            }
        }

        private void ShowMessageBox(string message, CustomMessageBox.MessageBoxType type)
        {
            CustomMessageBox messageBox = new CustomMessageBox(message,
                        CustomMessageBox.MessageBoxButton.OK, type);
            messageBox.ShowDialog();
            this.Effect = null;
        }

        private bool AddUser(string login, string password)
        {
            int rowsAffected = serviceClient.AddUser(login, password);
            AssignFunctionsToUser(login);
            return rowsAffected > 0;
        }

        private void RefreshFunctionList()
        {
            List<string> allFunctionNames = serviceClient.GetAllFunctionNames();

            if (FunctionItems.Count > 0) FunctionItems.Clear();
            
            if (allFunctionNames != null)
            {
                foreach (var functionName in allFunctionNames)
                {
                    FunctionItems.Add(new FunctionItem
                    {
                        Name = functionName,
                        IsChecked = false
                    });
                }
            }
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
    public class FunctionItem
    {
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }

}
