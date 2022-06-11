using System.Windows;

namespace Quizzer.WPF.Helpers;

public interface INotificationHandler
{
    void ShowMessage(string message, string caption);
}

public class MessageBoxNotificationHandler : INotificationHandler
{
    public void ShowMessage(string message, string caption) => MessageBox.Show(message, caption);
}

public class TestingNotificationHandler : INotificationHandler
{
    public void ShowMessage(string message, string caption)
    {
        _ = @"This doesn't do anything because it's only used in Unit Tests.";
    }
}
