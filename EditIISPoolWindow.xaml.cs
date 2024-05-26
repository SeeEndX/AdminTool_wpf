using AdminService;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для EditIISPoolWindow.xaml
    /// </summary>
    public partial class EditIISPoolWindow : Window
    {
        EditIISPoolWindow editPool;
        IAdminService serviceClient;
        private ManagedPipelineMode mode;
        private string currentPoolName;


        public EditIISPoolWindow(IAdminService serviceClient, string currentName)
        {
            this.serviceClient = serviceClient;
            InitializeComponent();
            editPool = this;
            currentPoolName = currentName;
            tbPoolName.Text = currentPoolName;
            cbMode.SelectedItem = ManagedPipelineMode.Classic;
        }

        private void BlurEffect()
        {
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 10;
            this.Effect = blurEffect;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbPoolName.Text))
            {
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Введите название пула!",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
            }
            else if (string.IsNullOrEmpty(tbMemLimit.Text) || string.IsNullOrEmpty(tbRestart.Text))
            {
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Заполните все поля!",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
            }
            else
            {
                var memoryLimit = int.Parse(tbMemLimit.Text);
                var intervalMinutes = int.Parse(tbRestart.Text) == 0 ? 30 : int.Parse(tbRestart.Text);
                var mode = cbMode.Text == "Классический" ? ManagedPipelineMode.Classic : ManagedPipelineMode.Integrated;

                if (tbPoolName.Text == currentPoolName)
                {
                    serviceClient.ModifyPool(currentPoolName, currentPoolName, mode, memoryLimit, intervalMinutes);
                }
                else
                {
                    var poolName = tbPoolName.Text;
                    serviceClient.ModifyPool(currentPoolName, poolName, mode, memoryLimit, intervalMinutes);
                }

                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Пул был изменен!",
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
            if (Mouse.LeftButton == MouseButtonState.Pressed) editPool.DragMove();
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
                tbMemLimit.TextChanged -= MemInput_TextChanged;
                tbMemLimit.Text = string.Empty;
                tbMemLimit.TextChanged += MemInput_TextChanged;
            }
        }

        private void RestartInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            int port;
            if (!int.TryParse(tbRestart.Text, out port) || port < 1 || port > 120)
            {
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Введите интервал перезапуска (мин) от 1 до 120!",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
                tbRestart.TextChanged -= RestartInput_TextChanged;
                tbRestart.Text = string.Empty;
                tbRestart.TextChanged += RestartInput_TextChanged;
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
