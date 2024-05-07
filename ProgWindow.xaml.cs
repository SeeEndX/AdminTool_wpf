using AdminService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
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

                if ((string)tab.Header == "Управление пулами" && tab.Visibility == Visibility.Visible)
                {
                    UpdatePoolsDG();
                }

                if ((string)tab.Header == "Управление сайтами IIS" && tab.Visibility == Visibility.Visible)
                {
                    UpdateSitesDG();
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

        private void UpdateSitesDG()
        {
            List<IISManager.SiteInfo> listOfSites = serviceClient.GetListOfSites();

            dgvSites.ItemsSource = listOfSites;
        }

        private void UpdatePoolsDG()
        {
            List<IISManager.AppPoolInfo> listOfAppPools = serviceClient.GetListOfAppPools();

            dgvPools.ItemsSource = listOfAppPools;
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

        //логика с IIS 
        private void btnAddSite_Click(object sender, EventArgs e)
        {
            ShowAddingIISWebSite();
        }

        private void BlurEffect()
        {
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 10;
            this.Effect = blurEffect;
        }

        private void ShowAddingIISWebSite()
        {
            BlurEffect();

            AddIISSiteWindow addIISWebSite = new AddIISSiteWindow(serviceClient);
            addIISWebSite.Owner = this;
            addIISWebSite.Closed += AddSiteWin_FormClosed;
            addIISWebSite.ShowDialog();
        }

        private void AddSiteWin_FormClosed(object sender, EventArgs e)
        {
            this.Effect = null;
            UpdateSitesDG();
        }

        private void EditSiteWin_FormClosed(object sender, EventArgs e)
        {
            this.Effect = null;
            UpdateSitesDG();
        }

        private void btnEditSite_Click(object sender, EventArgs e)
        {
            if (dgvSites.SelectedItems.Count == 1)
            {
                BlurEffect();
                var site = (IISManager.SiteInfo)dgvSites.SelectedItem;
                EditSiteWindow editSiteWin = new EditSiteWindow(serviceClient, site.Name);
                editSiteWin.Owner = this;
                editSiteWin.Closed += EditSiteWin_FormClosed;
                editSiteWin.ShowDialog();
            }
            else if (dgvSites.SelectedItems.Count > 1)
            {
                CustomMessageBox cmb = new CustomMessageBox("Выберите только ОДИН сайт для редактирования",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
            }
            else
            {
                CustomMessageBox cmb = new CustomMessageBox("Выберите сайт для редактирования",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
            }
        }


        private void btnDeleteSite_Click(object sender, EventArgs e)
        {
            BlurEffect();
            if (dgvSites.SelectedItems.Count == 1)
            {
                foreach (var item in dgvSites.SelectedItems)
                {
                    var site = item as IISManager.SiteInfo;
                    CustomMessageBox cmb = new CustomMessageBox($"Вы уверены, что хотите удалить сайт {site.Name}?",
                CustomMessageBox.MessageBoxButton.OKCancel, CustomMessageBox.MessageBoxType.Warning);
                    var result = cmb.ShowDialog();
                    this.Effect = null;
                    if (result == true)
                    {
                        serviceClient.DeleteWebsite(site.Name);
                        serviceClient.AddReport(currentUser, $"С веб-сервера Microsoft IIS был удален сайт {site.Name}");
                    }
                }
                UpdateSitesDG();

            }
            else if (dgvSites.SelectedItems.Count > 1)
            {
                CustomMessageBox cmb = new CustomMessageBox("Вы уверены, что хотите удалить сайты?",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Warning);
                var result = cmb.ShowDialog();
                this.Effect = null;
                if (result == true)
                {
                    foreach (var item in dgvSites.SelectedItems)
                    {
                        var site = item as IISManager.SiteInfo;

                        serviceClient.DeleteWebsite(site.Name);
                        serviceClient.AddReport(currentUser, $"С веб-сервера Microsoft IIS был удален сайт {site.Name}");
                    }
                    UpdateSitesDG();
                }
            }
            else
            {
                CustomMessageBox cmb = new CustomMessageBox("Выберите сайт для удаления",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Warning);
                cmb.ShowDialog();
                this.Effect = null;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSites.SelectedItems.Count > 0)
                {
                    foreach (var item in dgvSites.SelectedItems)
                    {
                        var site = item as IISManager.SiteInfo;
                        if (site.State == "Stopped")
                        {
                            var res = serviceClient.StartSite(site.Name);
                            if (res != "")
                            {
                                BlurEffect();
                                CustomMessageBox cmb = new CustomMessageBox("На порту этого сайта уже запущен другом сайт!",
                            CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                                cmb.ShowDialog();
                                this.Effect = null;
                            }
                            else serviceClient.AddReport(currentUser, $"На веб-сервере Microsoft IIS был запущен сайт {site.Name}");
                        }
                        else
                        {
                            BlurEffect();
                            CustomMessageBox cmb = new CustomMessageBox("Сайт уже запущен или в процессе запуска",
                        CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Warning);
                            cmb.ShowDialog();
                            this.Effect = null;
                            serviceClient.AddReport(currentUser, $"На веб-сервере Microsoft IIS была осуществленна попытка запустить сайт {site.Name}, который уже запущен");
                        }
                    }
                    UpdateSitesDG();
                }
                else
                {
                    BlurEffect();
                    CustomMessageBox cmb = new CustomMessageBox("Выберите сайт для запуска",
                CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Warning);
                    cmb.ShowDialog();
                    this.Effect = null;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (dgvSites.SelectedItems.Count > 0)
            {
                foreach (var item in dgvSites.SelectedItems)
                {
                    var site = item as IISManager.SiteInfo;
                    if (site.State == "Started")
                    {
                        serviceClient.StopSite(site.Name);
                        serviceClient.AddReport(currentUser, $"На веб-сервере Microsoft IIS был остановлен сайт {site.Name}");
                    }
                    else
                    {
                        BlurEffect();
                        CustomMessageBox cmb = new CustomMessageBox("Сайт уже остановлен или в процессе остановки",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Warning);
                        cmb.ShowDialog();
                        this.Effect = null;
                        serviceClient.AddReport(currentUser, $"На веб-сервере Microsoft IIS была осуществленна попытка остановить сайт {site.Name}, который уже остановлен");
                    }
                }

                UpdateSitesDG();
            }
            else
            {
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Выберите сайт для остановки",
            CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Warning);
                cmb.ShowDialog();
                this.Effect = null;
            }
        }

        private void btnAddPool_Click(object sender, EventArgs e)
        {
            ShowAddingIISPool();
        }

        private void ShowAddingIISPool()
        {
            BlurEffect();

            AddIISPoolWindow addIISWebSite = new AddIISPoolWindow(serviceClient);
            addIISWebSite.Owner = this;
            addIISWebSite.Closed += AddSiteWin_FormClosed;
            addIISWebSite.ShowDialog();
        }

        private void btnEditPool_Click(object sender, EventArgs e)
        {
            ShowEditingIISPool();
        }

        private void ShowEditingIISPool()
        {
            if (dgvPools.SelectedItems.Count == 1)
            {
                BlurEffect();
                var pool = (IISManager.AppPoolInfo)dgvPools.SelectedItem;
                EditIISPoolWindow editPoolWin = new EditIISPoolWindow(serviceClient, pool.Name);
                editPoolWin.Owner = this;
                editPoolWin.Closed += EditPoolWin_FormClosed;
                editPoolWin.ShowDialog();
            }
            else if (dgvSites.SelectedItems.Count > 1)
            {
                CustomMessageBox cmb = new CustomMessageBox("Выберите только ОДИН пул для редактирования",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
            }
            else
            {
                CustomMessageBox cmb = new CustomMessageBox("Выберите пул для редактирования",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
            }
        }

        private void EditPoolWin_FormClosed(object sender, EventArgs e)
        {
            this.Effect = null;
            UpdatePoolsDG();
        }

        private void btnDeletePool_Click(object sender, EventArgs e)
        {
            BlurEffect();
            if (dgvPools.SelectedItems.Count == 1)
            {
                foreach (var item in dgvPools.SelectedItems)
                {
                    var pool = item as IISManager.AppPoolInfo;
                    CustomMessageBox cmb = new CustomMessageBox($"Вы уверены, что хотите удалить пул {pool.Name}?",
                CustomMessageBox.MessageBoxButton.OKCancel, CustomMessageBox.MessageBoxType.Warning);
                    var result = cmb.ShowDialog();
                    this.Effect = null;
                    if (result == true)
                    {
                        serviceClient.DeletePool(pool.Name);
                        serviceClient.AddReport(currentUser, $"С веб-сервера Microsoft IIS был удален пул {pool.Name}");
                    }
                }
                UpdatePoolsDG();

            }
            else if (dgvSites.SelectedItems.Count > 1)
            {
                CustomMessageBox cmb = new CustomMessageBox("Вы уверены, что хотите удалить пулы?",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Warning);
                var result = cmb.ShowDialog();
                this.Effect = null;
                if (result == true)
                {
                    foreach (var item in dgvPools.SelectedItems)
                    {
                        var pool = item as IISManager.AppPoolInfo;

                        serviceClient.DeletePool(pool.Name);
                        serviceClient.AddReport(currentUser, $"С веб-сервера Microsoft IIS был удален пул {pool.Name}");
                    }
                    UpdatePoolsDG();
                }
            }
            else
            {
                CustomMessageBox cmb = new CustomMessageBox("Выберите пул для удаления",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Warning);
                cmb.ShowDialog();
                this.Effect = null;
            }
        }

        private void btnStartPool_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPools.SelectedItems.Count > 0)
                {
                    foreach (var item in dgvPools.SelectedItems)
                    {
                        var pool = item as IISManager.AppPoolInfo;
                        if (pool.State == "Stopped")
                        {
                            serviceClient.StartAppPool(pool.Name);
                            serviceClient.AddReport(currentUser, $"На веб-сервере Microsoft IIS был запущен пул {pool.Name}");
                        }
                        else
                        {
                            BlurEffect();
                            CustomMessageBox cmb = new CustomMessageBox("Пул уже запущен или в процессе запуска",
                        CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Warning);
                            cmb.ShowDialog();
                            this.Effect = null;
                            serviceClient.AddReport(currentUser, $"На веб-сервере Microsoft IIS была осуществленна попытка запустить пул {pool.Name}, который уже запущен");
                        }
                    }
                    UpdatePoolsDG();
                }
                else
                {
                    BlurEffect();
                    CustomMessageBox cmb = new CustomMessageBox("Выберите пул для запуска",
                CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Warning);
                    cmb.ShowDialog();
                    this.Effect = null;
                }
            }
            catch (Exception ex)
            {
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox(ex.ToString(),
            CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Error);
                cmb.ShowDialog();
                this.Effect = null;
            }

        }

        private void btnStopPool_Click(object sender, EventArgs e)
        {
            if (dgvPools.SelectedItems.Count > 0)
            {
                foreach (var item in dgvPools.SelectedItems)
                {
                    var pool = item as IISManager.AppPoolInfo;
                    if (pool.State == "Started")
                    {
                        serviceClient.StopAppPool(pool.Name);
                        serviceClient.AddReport(currentUser, $"На веб-сервере Microsoft IIS был остановлен пул {pool.Name}");
                    }
                    else
                    {
                        BlurEffect();
                        CustomMessageBox cmb = new CustomMessageBox("Пул уже остановлен или в процессе остановки",
                    CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Warning);
                        cmb.ShowDialog();
                        this.Effect = null;
                        serviceClient.AddReport(currentUser, $"На веб-сервере Microsoft IIS была осуществленна попытка остановить сайт {pool.Name}, который уже остановлен");
                    }
                }

                UpdatePoolsDG();
            }
            else
            {
                BlurEffect();
                CustomMessageBox cmb = new CustomMessageBox("Выберите пул для остановки",
            CustomMessageBox.MessageBoxButton.OK, CustomMessageBox.MessageBoxType.Warning);
                cmb.ShowDialog();
                this.Effect = null;
            }
        }
    }
}
