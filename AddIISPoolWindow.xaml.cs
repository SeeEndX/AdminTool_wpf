using AdminService;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using Microsoft.Web.Administration;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static AdminService.AdministrativeService;
using System.Xml.Linq;

namespace AdminTool_wpf
{
    /// <summary>
    /// Логика взаимодействия для AddIISPoolWindow.xaml
    /// </summary>
    public partial class AddIISPoolWindow : Window
    {
        AddIISPoolWindow addPool;
        IAdminService serviceClient;
        private ManagedPipelineMode mode;

        public AddIISPoolWindow(IAdminService serviceClient)
        {
            this.serviceClient = serviceClient;
            InitializeComponent();
            addPool = this;
            cbMode.SelectedItem = ManagedPipelineMode.Classic;
        }

        private void BlurEffect()
        {
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 10;
            this.Effect = blurEffect;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tbPoolName.Text == "")
            {
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Введите название пула!",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
            }
            else
            {
                var poolName = tbPoolName.Text;

                var memoryLimit = string.IsNullOrEmpty(tbMemLimit.Text) ? 0 : int.Parse(tbMemLimit.Text);
                var intervalMinutes = (int.Parse(tbRestart.Text) == 0) ? 30 : int.Parse(tbRestart.Text);
                mode = (cbMode.Text == "Классический") ? ManagedPipelineMode.Classic : ManagedPipelineMode.Integrated;

                serviceClient.CreatePool(poolName, mode, memoryLimit, intervalMinutes);
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Пул был создан!",
            CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Success);
                cmb.ShowDialog();
                this.Effect = null;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.1));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(OpacityProperty, anim);
        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) addPool.DragMove();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MemInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            int port;
            if (!int.TryParse(tbMemLimit.Text, out port) || port < 1 || port > 1024)
            {
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Введите ограничение по памяти (МБ) от 1 до 1024!",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
                tbMemLimit.Text = string.Empty;
            }
        }

        private void RestartInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            int port;
            if (!int.TryParse(tbRestart.Text, out port) || port < 1 || port > 1024)
            {
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Введите интервал перезапуска (мин) от 1 до 120!",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
                tbRestart.Text = string.Empty;
            }
        }

        private void memAndRestartInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
    }
}
