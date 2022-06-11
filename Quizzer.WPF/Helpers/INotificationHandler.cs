using System.Windows;

namespace Quizzer.WPF;

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
    public void ShowMessage(string message, string caption) { }
}
