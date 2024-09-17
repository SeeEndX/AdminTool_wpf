using AdminService;
using Microsoft.Web.Administration;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace AdminTool_wpf
{
    /// <summary>
    /// Логика взаимодействия для AddIISPoolWindow.xaml
    /// </summary>
    /// окно добавления пула IIS
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

        //метод для размытия заднего фона
        private void BlurEffect()
        {
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 10;
            this.Effect = blurEffect;
        }

        //метод по нажатию кнопки добавить
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

        //метод по нажатию кнопки закрыть
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(0.1));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(OpacityProperty, anim);
        }

        //метод по переносу окна
        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) addPool.DragMove();
        }

        //метод по нажатию кнопки назад
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //метод, вызываемый при вводе текста в поле MemInput
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

        //метод, вызываемый при вводе текста в поле RestartInput
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

        //метод, вызываемый при вводе текста в поля memInput и RestartInput
        private void memAndRestartInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
    }
}
