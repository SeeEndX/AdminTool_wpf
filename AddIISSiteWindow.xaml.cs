using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Shapes;
using System.Xml.Linq;
using AdminService;
using Microsoft.WindowsAPICodePack.Dialogs;
using static AdminService.AdministrativeService;


namespace AdminTool_wpf
{
    /// <summary>
    /// Логика взаимодействия для AddIISSiteWindow.xaml
    /// </summary>
    /// класс окна добавления сайта IIS
    public partial class AddIISSiteWindow : Window
    {
        AddIISSiteWindow addSite;
        IAdminService serviceClient;

        public AddIISSiteWindow(IAdminService serviceClient)
        {
            this.serviceClient = serviceClient;
            InitializeComponent();
            addSite = this;
        }

        //метод по нажатию выбрать папку
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

        //метод, вызываемый при изменении пути к папке
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

        //метод по размытию заднего фона
        private void BlurEffect()
        {
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 10;
            this.Effect = blurEffect;
        }

        //метод по нажатию кнопки добавить
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tbSiteName.Text == "" || tbPath.Text == "" || tbPort.Text == "")
            {
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Введите все данные!",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
            }
            else
            {
                string path = tbPath.Text;
                string name = tbSiteName.Text;
                int port;
                bool result = int.TryParse(tbPort.Text, out port);
                serviceClient.CreateWebsite(name, path, port);
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Сайт был создан!",
            CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Success);
                cmb.ShowDialog();
                this.Effect = null;
            }
        }

        //метод по нажатию кнопки закрыть
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.1));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(OpacityProperty, anim);
        }

        //метод по перемещению окна
        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) addSite.DragMove();
        }

        //метод по нажатию кнопки назад
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //метод, вызываемый при изменении текста в поле ввода порта
        private void PortInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            int port;
            if (!int.TryParse(tbPort.Text, out port) || port < 0 || port > 65535)
            {
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Введите порт в пределах от 0 до 65535!", 
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
                tbPort.Text = string.Empty;
            }
        }

        //метод, вызываемый до изменения текста в поле ввода порта
        private void PortInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
    }
}
