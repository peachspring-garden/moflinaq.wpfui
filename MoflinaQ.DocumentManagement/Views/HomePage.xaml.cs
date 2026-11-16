using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MoflinaQ.DocumentManagement.Views
{
    public sealed class WorkItem
    {
        public string Status { get; set; }
        public string Title { get; set; }
        public string DateText { get; set; }
        public Brush ChipBackground { get; set; }
        public Brush ChipForeground { get; set; }
    }

    public sealed class NoticeItem
    {
        public string Title { get; set; }
        public string Meta { get; set; }
        public bool IsUnread { get; set; }
        public Visibility UnreadVisibility => IsUnread ? Visibility.Visible : Visibility.Hidden;
    }

    public partial class HomePage : Page
    {
        private static readonly Brush ReturnedBackground = BrushOf(0xF9, 0xDE, 0xDC);
        private static readonly Brush ReturnedForeground = BrushOf(0x8C, 0x1D, 0x18);
        private static readonly Brush PendingBackground = BrushOf(0xE7, 0xEA, 0xED);
        private static readonly Brush PendingForeground = BrushOf(0x3C, 0x40, 0x43);
        private static readonly Brush ActiveBackground = BrushOf(0xE8, 0xF0, 0xFE);
        private static readonly Brush ActiveForeground = BrushOf(0x17, 0x4E, 0xA6);

        public HomePage()
        {
            InitializeComponent();

            TodayText.Text = DateTime.Today.ToString("yyyy年M月d日（ddd）", new CultureInfo("ja-JP"));

            AlertsList.ItemsSource = new[]
            {
                Returned("「院内通知様式（共通）」差戻し対応", "11月12日"),
                Pending("「購買管理要領」改訂の承認", "11月14日"),
                Pending("「契約事務取扱要領」改訂の承認", "11月19日")
            };

            TasksList.ItemsSource = new[]
            {
                Pending("「購買管理要領」改訂の承認", "11月14日"),
                Active("「文書管理規程」改訂案のレビュー", "11月17日"),
                Returned("「院内通知様式（共通）」差戻し対応", "11月12日"),
                Pending("「契約事務取扱要領」改訂の承認", "11月19日")
            };

            NoticesList.ItemsSource = new[]
            {
                new NoticeItem { Title = "「文書管理規程」改訂案の意見募集を開始しました", Meta = "お知らせ・本日 09:00", IsUnread = true },
                new NoticeItem { Title = "「購買管理要領」改訂の確認依頼が届いています", Meta = "依頼・本日 08:30", IsUnread = true },
                new NoticeItem { Title = "「防災管理規程」の見直し期限が近づいています（11月20日）", Meta = "期限・昨日 17:00", IsUnread = true },
                new NoticeItem { Title = "施設課より定期点検のお知らせ（11月18日実施）", Meta = "お知らせ・昨日 13:00" },
                new NoticeItem { Title = "情報システム課よりメンテナンスのお知らせ（11月16日 22:00〜）", Meta = "お知らせ・11月11日" }
            };
        }

        private static Brush BrushOf(byte r, byte g, byte b)
        {
            var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
            brush.Freeze();
            return brush;
        }

        private static WorkItem Returned(string title, string dateText)
        {
            return new WorkItem
            {
                Status = "差戻し",
                Title = title,
                DateText = dateText,
                ChipBackground = ReturnedBackground,
                ChipForeground = ReturnedForeground
            };
        }

        private static WorkItem Pending(string title, string dateText)
        {
            return new WorkItem
            {
                Status = "承認待ち",
                Title = title,
                DateText = dateText,
                ChipBackground = PendingBackground,
                ChipForeground = PendingForeground
            };
        }

        private static WorkItem Active(string title, string dateText)
        {
            return new WorkItem
            {
                Status = "対応中",
                Title = title,
                DateText = dateText,
                ChipBackground = ActiveBackground,
                ChipForeground = ActiveForeground
            };
        }
    }
}
