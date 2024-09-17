using AdminService;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace AdminTool_wpf
{
    /// <summary>
    /// Логика взаимодействия для EditSiteWindow.xaml
    /// </summary>
    public partial class EditSiteWindow : Window
    {
        EditSiteWindow editSite;
        IAdminService serviceClient;
        private string currentName;
        private string newName;

        public EditSiteWindow(IAdminService serviceClient, string currentName)
        {
            this.currentName = currentName;
            this.serviceClient = serviceClient;
            InitializeComponent();
            tbSiteName.Text = currentName;
            editSite = this;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (tbPath.Text == "" || tbSiteName.Text == "" )
            {
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Введите все данные!",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
            }
            else
            {
                if (tbPath.Text == currentName)
                {
                    string path = tbPath.Text;
                    newName =currentName;
                    serviceClient.ModifyWebsite(currentName, newName, path);
                    BlurEffect();
                    CustomMessageBox cmb = new CustomMessageBox("Сайт был изменен",
                CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Success);
                    cmb.ShowDialog();
                    this.Effect = null;
                }
                else
                {
                    string path = tbPath.Text;
                    newName = tbSiteName.Text;
                    serviceClient.ModifyWebsite(currentName, newName, path);
                    BlurEffect();
                    CustomMessageBox cmb = new CustomMessageBox("Сайт был изменен",
                CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Success);
                    cmb.ShowDialog();
                    this.Effect = null;
                }
            }
        }

        private void ChooseDirectory(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                tbPath.Text = dialog.FileName;
            }
        }

        private void tbPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            string path = tbPath.Text;

            if (!Directory.Exists(path))
            {
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Введенного пути не существует!",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
                tbPath.Text = string.Empty;
            }
        }

        private void BlurEffect()
        {
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 10;
            this.Effect = blurEffect;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.1));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(OpacityProperty, anim);
        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) editSite.DragMove();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PortInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
    }
}
