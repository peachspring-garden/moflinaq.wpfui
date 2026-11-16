using System.Windows;
using System.Windows.Controls;

namespace MoflinaQ.DocumentManagement.Views
{
    public abstract class NavigationPage : Page
    {
        protected NavigationPage(string title)
        {
            Title = title;
            Content = new TextBlock
            {
                Text = title,
                Margin = new Thickness(32, 24, 32, 0),
                FontSize = 28,
                FontWeight = FontWeights.SemiBold
            };
        }
    }

    public sealed class HomePage : NavigationPage
    {
        public HomePage() : base("ホーム")
        {
        }
    }

    public sealed class RepairRequestPage : NavigationPage
    {
        public RepairRequestPage() : base("修理依頼書")
        {
        }
    }

    public sealed class ApprovalRequestPage : NavigationPage
    {
        public ApprovalRequestPage() : base("稟議書")
        {
        }
    }

    public sealed class SettingsPage : NavigationPage
    {
        public SettingsPage() : base("設定")
        {
        }
    }
}
