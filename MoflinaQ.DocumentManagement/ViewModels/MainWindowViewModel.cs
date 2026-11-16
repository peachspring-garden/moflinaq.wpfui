using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoflinaQ.DocumentManagement.Views;
using Wpf.Ui.Controls;

namespace MoflinaQ.DocumentManagement.ViewModels
{
    public enum MenuPermission
    {
        ApprovalRequests, Contracts, Policies, Reservations, Purchasing,
        RepairRequests, Equipment, Maintenance, Contractors, MeterReading,
        HelpDesk, ItAssets, AccountRequests, Incidents, Faq, Administration, Notifications
    }

    public sealed class MainWindowViewModel : ObservableObject
    {
        private int fiscalYear;
        private string currentPageTitle = "ホーム";

        public MainWindowViewModel(string userName, string department, IEnumerable<MenuPermission> permissions)
        {
            UserName = userName;
            Department = department;
            UserInitial = string.IsNullOrWhiteSpace(userName) ? "?" : userName.Trim().Substring(0, 1);
            fiscalYear = DateTime.Today.Month < 4 ? DateTime.Today.Year - 1 : DateTime.Today.Year;

            PreviousYearCommand = new RelayCommand(() => FiscalYear--, () => FiscalYear > 1);
            NextYearCommand = new RelayCommand(() => FiscalYear++, () => FiscalYear < 9999);
            RefreshCommand = new RelayCommand(() => RefreshRequested?.Invoke(this, EventArgs.Empty));

            var allowed = new HashSet<MenuPermission>(permissions);
            MenuItems = BuildMenu(allowed).ToList().AsReadOnly();
        }

        public string UserName { get; }
        public string Department { get; }
        public string UserInitial { get; }
        public IReadOnlyList<NavigationViewItem> MenuItems { get; }
        public IRelayCommand PreviousYearCommand { get; }
        public IRelayCommand NextYearCommand { get; }
        public IRelayCommand RefreshCommand { get; }

        public event EventHandler RefreshRequested;

        public int FiscalYear
        {
            get => fiscalYear;
            set
            {
                if (value < 1 || value > 9999)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "年度は1から9999の範囲で入力してください。");
                }

                if (SetProperty(ref fiscalYear, value))
                {
                    PreviousYearCommand.NotifyCanExecuteChanged();
                    NextYearCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public string CurrentPageTitle
        {
            get => currentPageTitle;
            set => SetProperty(ref currentPageTitle, value);
        }

        private static IEnumerable<NavigationViewItem> BuildMenu(ISet<MenuPermission> allowed)
        {
            yield return CreateItem("ホーム", SymbolRegular.Home24, typeof(HomePage));

            var groups = new[]
            {
                CreateGroup("総務課", SymbolRegular.Folder24,
                    CreateEntry(allowed, MenuPermission.ApprovalRequests, "稟議書", SymbolRegular.Document24, typeof(ApprovalRequestPage)),
                    CreateEntry(allowed, MenuPermission.Contracts, "契約・更新管理", SymbolRegular.Document24),
                    CreateEntry(allowed, MenuPermission.Policies, "院内文書・規程管理", SymbolRegular.Library24),
                    CreateEntry(allowed, MenuPermission.Reservations, "会議室・公用車・備品予約", SymbolRegular.Calendar24),
                    CreateEntry(allowed, MenuPermission.Purchasing, "購買・備品管理", SymbolRegular.ShoppingBag24)),
                CreateGroup("施設課", SymbolRegular.Wrench24,
                    CreateEntry(allowed, MenuPermission.RepairRequests, "修理依頼書", SymbolRegular.Edit24, typeof(RepairRequestPage)),
                    CreateEntry(allowed, MenuPermission.Equipment, "設備台帳", SymbolRegular.TextAlignLeft24),
                    CreateEntry(allowed, MenuPermission.Maintenance, "メンテナンス・定期点検", SymbolRegular.ClipboardTask24),
                    CreateEntry(allowed, MenuPermission.Contractors, "工事・業者管理", SymbolRegular.Person24),
                    CreateEntry(allowed, MenuPermission.MeterReading, "検針・使用料管理", SymbolRegular.Gauge24)),
                CreateGroup("情報システム部門", SymbolRegular.Code24,
                    CreateEntry(allowed, MenuPermission.HelpDesk, "システム連絡票・ヘルプ", SymbolRegular.Chat24),
                    CreateEntry(allowed, MenuPermission.ItAssets, "IT資産・ライセンス管理", SymbolRegular.Desktop24),
                    CreateEntry(allowed, MenuPermission.AccountRequests, "アカウント・権限申請", SymbolRegular.Person24),
                    CreateEntry(allowed, MenuPermission.Incidents, "障害・変更管理", SymbolRegular.Warning24),
                    CreateEntry(allowed, MenuPermission.Faq, "FAQ・運用手順書", SymbolRegular.Question24),
                    CreateGroup("システム管理", SymbolRegular.Settings24,
                        CreateEntry(allowed, MenuPermission.Administration, "管理者メニュー", SymbolRegular.Person24))),
                CreateGroup("共通", SymbolRegular.List24,
                    CreateEntry(allowed, MenuPermission.Notifications, "通知", SymbolRegular.Alert24))
            };

            foreach (var group in groups.Where(group => group != null))
            {
                yield return group;
            }
        }

        private static NavigationViewItem CreateEntry(ISet<MenuPermission> allowed, MenuPermission permission,
            string title, SymbolRegular icon, Type pageType = null)
        {
            return allowed.Contains(permission) ? CreateItem(title, icon, pageType) : null;
        }

        private static NavigationViewItem CreateItem(string title, SymbolRegular icon, Type pageType)
        {
            return new NavigationViewItem
            {
                Content = title,
                Icon = new SymbolIcon { Symbol = icon },
                TargetPageType = pageType,
                IsEnabled = pageType != null
            };
        }

        private static NavigationViewItem CreateGroup(string title, SymbolRegular icon, params NavigationViewItem[] children)
        {
            var items = children.Where(child => child != null).ToList();
            if (items.Count == 0)
            {
                return null;
            }

            var group = new NavigationViewItem
            {
                Content = title,
                Icon = new SymbolIcon { Symbol = icon },
                IsExpanded = true
            };
            foreach (var item in items)
            {
                group.MenuItems.Add(item);
            }
            return group;
        }
    }
}
