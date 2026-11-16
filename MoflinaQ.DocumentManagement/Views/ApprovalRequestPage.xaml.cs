using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MoflinaQ.DocumentManagement.Views
{
    public enum ApprovalStatus
    {
        Pending,
        Returned,
        Approved
    }

    public enum StepState
    {
        Approved,
        Current,
        Returned,
        Waiting
    }

    public sealed class ApprovalStep
    {
        public string Role { get; set; }
        public string Name { get; set; }
        public StepState State { get; set; }
        public string DecidedOn { get; set; } = "―";
        public Visibility ArrowVisibility { get; set; } = Visibility.Visible;

        public string StateText
        {
            get
            {
                switch (State)
                {
                    case StepState.Approved: return "承認済";
                    case StepState.Current: return "承認待ち";
                    case StepState.Returned: return "差戻し";
                    default: return "未到達";
                }
            }
        }

        public Brush ChipBackground
        {
            get
            {
                switch (State)
                {
                    case StepState.Approved: return ApprovalBrushes.ApprovedBackground;
                    case StepState.Current: return ApprovalBrushes.CurrentBackground;
                    case StepState.Returned: return ApprovalBrushes.ReturnedBackground;
                    default: return ApprovalBrushes.WaitingBackground;
                }
            }
        }

        public Brush ChipBorder
        {
            get
            {
                switch (State)
                {
                    case StepState.Approved: return ApprovalBrushes.ApprovedBackground;
                    case StepState.Current: return ApprovalBrushes.CurrentBorder;
                    case StepState.Returned: return ApprovalBrushes.ReturnedBorder;
                    default: return ApprovalBrushes.WaitingBorder;
                }
            }
        }

        public Brush ChipForeground
        {
            get
            {
                switch (State)
                {
                    case StepState.Approved: return Brushes.White;
                    case StepState.Current: return ApprovalBrushes.CurrentForeground;
                    case StepState.Returned: return ApprovalBrushes.ReturnedForeground;
                    default: return ApprovalBrushes.WaitingForeground;
                }
            }
        }
    }

    public sealed class ApprovalDocument
    {
        public string Number { get; set; }
        public string DraftDate { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Drafter { get; set; }
        public long Amount { get; set; }
        public string Content { get; set; }
        public ApprovalStatus Status { get; set; }
        public List<ApprovalStep> Steps { get; set; } = new List<ApprovalStep>();
        public List<string> Attachments { get; set; } = new List<string>();
        public List<string> History { get; set; } = new List<string>();

        public string DeptDrafter => Department + " / " + Drafter;
        public string AmountText => "¥" + Amount.ToString("N0");
        public int AttachmentCount => Attachments.Count;

        public string StatusText
        {
            get
            {
                switch (Status)
                {
                    case ApprovalStatus.Returned: return "差戻し";
                    case ApprovalStatus.Approved: return "決裁済";
                    default: return "承認待ち";
                }
            }
        }

        public Brush StatusBackground
        {
            get
            {
                switch (Status)
                {
                    case ApprovalStatus.Returned: return ApprovalBrushes.ReturnedBackground;
                    case ApprovalStatus.Approved: return ApprovalBrushes.DecidedBackground;
                    default: return ApprovalBrushes.CurrentBackground;
                }
            }
        }

        public Brush StatusForeground
        {
            get
            {
                switch (Status)
                {
                    case ApprovalStatus.Returned: return ApprovalBrushes.ReturnedForeground;
                    case ApprovalStatus.Approved: return ApprovalBrushes.DecidedForeground;
                    default: return ApprovalBrushes.CurrentForeground;
                }
            }
        }

        public ApprovalStep CurrentStep => Steps.FirstOrDefault(step => step.State == StepState.Current);
    }

    internal static class ApprovalBrushes
    {
        public static readonly Brush ApprovedBackground = Freeze(0x2E, 0x7D, 0x4F);
        public static readonly Brush CurrentBackground = Freeze(0xFD, 0xF0, 0xE0);
        public static readonly Brush CurrentBorder = Freeze(0xDE, 0x9A, 0x2E);
        public static readonly Brush CurrentForeground = Freeze(0x8A, 0x5A, 0x00);
        public static readonly Brush ReturnedBackground = Freeze(0xF9, 0xDE, 0xDC);
        public static readonly Brush ReturnedBorder = Freeze(0xC4, 0x2B, 0x1C);
        public static readonly Brush ReturnedForeground = Freeze(0x8C, 0x1D, 0x18);
        public static readonly Brush WaitingBackground = Freeze(0xED, 0xEE, 0xF0);
        public static readonly Brush WaitingBorder = Freeze(0xC9, 0xCC, 0xD1);
        public static readonly Brush WaitingForeground = Freeze(0x6B, 0x70, 0x75);
        public static readonly Brush DecidedBackground = Freeze(0xDF, 0xF2, 0xE3);
        public static readonly Brush DecidedForeground = Freeze(0x1E, 0x6B, 0x34);

        private static Brush Freeze(byte r, byte g, byte b)
        {
            var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
            brush.Freeze();
            return brush;
        }
    }

    internal static class ApprovalStore
    {
        // 承認経路は固定：所属長 → 企画部長 → 部長 → 看護部長 → 事務長 → 本部長 → 院長 → 会長
        public static readonly string[] Roles =
        {
            "所属長", "企画部長", "部長", "看護部長", "事務長", "本部長", "院長", "会長"
        };

        private static readonly string[] ApproverNames =
        {
            "佐藤 健一", "中村 直樹", "山本 和夫", "小林 恵子", "伊藤 誠", "渡辺 修", "木村 正", "森 健二"
        };

        public static List<ApprovalDocument> Documents { get; } = CreateSamples();

        public static List<ApprovalStep> CreateSteps(int approvedCount, ApprovalStatus status, params string[] decidedDates)
        {
            var steps = new List<ApprovalStep>();
            for (var i = 0; i < Roles.Length; i++)
            {
                var step = new ApprovalStep
                {
                    Role = Roles[i],
                    Name = ApproverNames[i],
                    ArrowVisibility = i == Roles.Length - 1 ? Visibility.Collapsed : Visibility.Visible
                };
                if (i < approvedCount)
                {
                    step.State = StepState.Approved;
                    step.DecidedOn = i < decidedDates.Length ? decidedDates[i] : "―";
                }
                else if (i == approvedCount && status == ApprovalStatus.Pending)
                {
                    step.State = StepState.Current;
                }
                else if (i == approvedCount && status == ApprovalStatus.Returned)
                {
                    step.State = StepState.Returned;
                }
                else
                {
                    step.State = StepState.Waiting;
                }
                steps.Add(step);
            }
            return steps;
        }

        private static List<ApprovalDocument> CreateSamples()
        {
            return new List<ApprovalDocument>
            {
                new ApprovalDocument
                {
                    Number = "RNG-2026-104",
                    DraftDate = "2026-11-12",
                    Title = "看護部業務用PHS端末（45台）一斉更新および内線交換機設定委託",
                    Department = "総務課",
                    Drafter = "高橋 誠",
                    Amount = 3480000,
                    Content = "看護部で使用するPHS端末45台が保守期限を迎えるため、一斉更新と内線交換機の設定委託を起案します。",
                    Status = ApprovalStatus.Pending,
                    Steps = CreateSteps(3, ApprovalStatus.Pending, "11月12日", "11月13日", "11月14日"),
                    Attachments = new List<string> { "見積書（PHS端末）.pdf", "内線交換機設定仕様書.pdf" },
                    History = new List<string>
                    {
                        "2026-11-14 山本 和夫（部長）承認",
                        "2026-11-13 中村 直樹（企画部長）承認",
                        "2026-11-12 佐藤 健一（所属長）承認",
                        "2026-11-12 高橋 誠 起案"
                    }
                },
                new ApprovalDocument
                {
                    Number = "RNG-2026-103",
                    DraftDate = "2026-11-10",
                    Title = "中央監視室 空調自動制御サーバOS延長保守契約の件",
                    Department = "施設課",
                    Drafter = "田村 浩二",
                    Amount = 960000,
                    Content = "中央監視室の空調自動制御サーバOSについて、延長保守契約を締結する件です。",
                    Status = ApprovalStatus.Pending,
                    Steps = CreateSteps(0, ApprovalStatus.Pending),
                    Attachments = new List<string> { "保守契約書（案）.pdf" },
                    History = new List<string> { "2026-11-10 田村 浩二 起案" }
                },
                new ApprovalDocument
                {
                    Number = "RNG-2026-102",
                    DraftDate = "2026-11-06",
                    Title = "院内無線LAN（Wi-Fi 6E）アクセスポイント増設工事（4階・5階病棟）",
                    Department = "医療情報システム課",
                    Drafter = "西野 勝俊",
                    Amount = 2150000,
                    Content = "4階・5階病棟の無線LANカバレッジ改善のため、Wi-Fi 6Eアクセスポイントの増設工事を起案します。",
                    Status = ApprovalStatus.Returned,
                    Steps = CreateSteps(2, ApprovalStatus.Returned, "11月6日", "11月7日"),
                    Attachments = new List<string> { "工事見積書.pdf", "アクセスポイント配置図.pdf" },
                    History = new List<string>
                    {
                        "2026-11-08 山本 和夫（部長）差戻し：見積単価の内訳を追記してください",
                        "2026-11-07 中村 直樹（企画部長）承認",
                        "2026-11-06 佐藤 健一（所属長）承認",
                        "2026-11-06 西野 勝俊 起案"
                    }
                },
                new ApprovalDocument
                {
                    Number = "RNG-2026-101",
                    DraftDate = "2026-11-01",
                    Title = "年末調整・マイナンバー収集クラウドサービス年間ライセンス更新",
                    Department = "総務課",
                    Drafter = "鈴木 美咲",
                    Amount = 540000,
                    Content = "年末調整・マイナンバー収集クラウドサービスの年間ライセンス更新の件です。",
                    Status = ApprovalStatus.Approved,
                    Steps = CreateSteps(8, ApprovalStatus.Approved,
                        "11月1日", "11月2日", "11月3日", "11月4日", "11月5日", "11月6日", "11月7日", "11月8日"),
                    Attachments = new List<string> { "ライセンス更新見積書.pdf" },
                    History = new List<string>
                    {
                        "2026-11-08 森 健二（会長）承認",
                        "2026-11-07 木村 正（院長）承認",
                        "2026-11-06 渡辺 修（本部長）承認",
                        "2026-11-05 伊藤 誠（事務長）承認",
                        "2026-11-04 小林 恵子（看護部長）承認",
                        "2026-11-03 山本 和夫（部長）承認",
                        "2026-11-02 中村 直樹（企画部長）承認",
                        "2026-11-01 佐藤 健一（所属長）承認",
                        "2026-11-01 鈴木 美咲 起案"
                    }
                }
            };
        }
    }

    public partial class ApprovalRequestPage : Page
    {
        private string currentFilter = "All";
        private ApprovalDocument currentDocument;
        private bool isNewDraft;
        private bool suppressSelection;

        public ApprovalRequestPage()
        {
            InitializeComponent();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            IEnumerable<ApprovalDocument> docs = ApprovalStore.Documents;
            switch (currentFilter)
            {
                case "Pending":
                    docs = docs.Where(doc => doc.Status == ApprovalStatus.Pending);
                    break;
                case "Returned":
                    docs = docs.Where(doc => doc.Status == ApprovalStatus.Returned);
                    break;
                case "Approved":
                    docs = docs.Where(doc => doc.Status == ApprovalStatus.Approved);
                    break;
            }
            RowsList.ItemsSource = docs.ToList();
            UpdateFilterButtons();
        }

        private void UpdateFilterButtons()
        {
            foreach (var button in new[] { FilterAll, FilterPending, FilterReturned, FilterApproved })
            {
                var selected = (string)button.Tag == currentFilter;
                button.Background = selected
                    ? (Brush)FindResource("TextFillColorPrimaryBrush")
                    : Brushes.Transparent;
                button.Foreground = selected
                    ? Brushes.White
                    : (Brush)FindResource("TextFillColorPrimaryBrush");
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            currentFilter = (string)((Button)sender).Tag;
            ApplyFilter();
        }

        private void RowsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (suppressSelection)
            {
                return;
            }
            if (RowsList.SelectedItem is ApprovalDocument doc)
            {
                OpenDetail(doc, false);
                suppressSelection = true;
                RowsList.SelectedItem = null;
                suppressSelection = false;
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            OpenDetail(null, true);
        }

        private void OpenDetail(ApprovalDocument doc, bool newDraft)
        {
            currentDocument = doc;
            isNewDraft = newDraft;
            ListRoot.Visibility = Visibility.Collapsed;
            DetailRoot.Visibility = Visibility.Visible;

            DetailViewPanel.Visibility = newDraft ? Visibility.Collapsed : Visibility.Visible;
            DetailEditPanel.Visibility = newDraft ? Visibility.Visible : Visibility.Collapsed;
            EditOpsPanel.Visibility = newDraft ? Visibility.Visible : Visibility.Collapsed;
            OpsPanel.Visibility = Visibility.Collapsed;
            EditValidation.Visibility = Visibility.Collapsed;

            if (newDraft)
            {
                DetailNumber.Text = "新規稟議";
                DetailStatusChip.Visibility = Visibility.Collapsed;
                EditTitle.Text = string.Empty;
                EditDepartment.Text = string.Empty;
                EditDrafter.Text = string.Empty;
                EditAmount.Text = string.Empty;
                EditContent.Text = string.Empty;
                return;
            }

            DetailStatusChip.Visibility = Visibility.Visible;
            DetailNumber.Text = doc.Number;
            DetailStatusText.Text = doc.StatusText;
            DetailStatusChip.Background = doc.StatusBackground;
            DetailStatusText.Foreground = doc.StatusForeground;
            DetailTitle.Text = doc.Title;
            DetailDraftDate.Text = doc.DraftDate;
            DetailDeptDrafter.Text = doc.DeptDrafter;
            DetailAmount.Text = doc.AmountText;
            DetailContent.Text = doc.Content;
            DetailStepsList.ItemsSource = doc.Steps;
            DetailAttachmentsList.ItemsSource = doc.Attachments;
            DetailNoAttachments.Visibility = doc.Attachments.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            DetailHistoryList.ItemsSource = doc.History;
            OpsPanel.Visibility = doc.Status == ApprovalStatus.Pending ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            DetailRoot.Visibility = Visibility.Collapsed;
            ListRoot.Visibility = Visibility.Visible;
            currentDocument = null;
            ApplyFilter();
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            var doc = currentDocument;
            var step = doc?.CurrentStep;
            if (doc == null || step == null)
            {
                return;
            }

            step.State = StepState.Approved;
            step.DecidedOn = DateTime.Today.ToString("M月d日");
            doc.History.Insert(0, DateTime.Today.ToString("yyyy-MM-dd") + " " + step.Name + "（" + step.Role + "）承認");

            var index = doc.Steps.IndexOf(step);
            if (index == doc.Steps.Count - 1)
            {
                doc.Status = ApprovalStatus.Approved;
            }
            else
            {
                doc.Steps[index + 1].State = StepState.Current;
            }
            OpenDetail(doc, false);
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            var doc = currentDocument;
            var step = doc?.CurrentStep;
            if (doc == null || step == null)
            {
                return;
            }

            step.State = StepState.Returned;
            doc.Status = ApprovalStatus.Returned;
            doc.History.Insert(0, DateTime.Today.ToString("yyyy-MM-dd") + " " + step.Name + "（" + step.Role + "）差戻し");
            OpenDetail(doc, false);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var title = EditTitle.Text.Trim();
            long amount;
            if (title.Length == 0 || !long.TryParse(EditAmount.Text.Trim(), out amount) || amount < 0)
            {
                EditValidation.Text = "件名を入力し、申請金額は0以上の数値で入力してください。";
                EditValidation.Visibility = Visibility.Visible;
                return;
            }

            var nextNumber = ApprovalStore.Documents
                .Select(d => int.Parse(d.Number.Substring(d.Number.Length - 3)))
                .DefaultIfEmpty(100)
                .Max() + 1;
            var doc = new ApprovalDocument
            {
                Number = "RNG-2026-" + nextNumber,
                DraftDate = DateTime.Today.ToString("yyyy-MM-dd"),
                Title = title,
                Department = EditDepartment.Text.Trim(),
                Drafter = EditDrafter.Text.Trim(),
                Amount = amount,
                Content = EditContent.Text.Trim(),
                Status = ApprovalStatus.Pending,
                Steps = ApprovalStore.CreateSteps(0, ApprovalStatus.Pending),
                Attachments = new List<string>(),
                History = new List<string>
                {
                    DateTime.Today.ToString("yyyy-MM-dd") + " " + EditDrafter.Text.Trim() + " 起案"
                }
            };
            ApprovalStore.Documents.Insert(0, doc);
            BackButton_Click(sender, e);
        }
    }
}
