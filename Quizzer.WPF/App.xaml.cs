using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Quizzer.WPF.Helpers;
using Quizzer.WPF.PromptTypes;
using Quizzer.WPF.Screens.Admin;
using Quizzer.WPF.Screens.Main;
using Quizzer.WPF.Screens.Quiz;

namespace Quizzer.WPF;

public partial class App : Application
{
    public ServiceProvider Services { get; set; } = null!;

    public App()
    {
        Setup();
        var x = new MainWindow() { DataContext = Services.GetService<MainViewModel>() }; /*if this were VM-first, I wouldn't have this issue. */
        x.Show();
    }

    private void Setup()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();
        var ph = Services.GetService<PromptHandler>()!;
        ph.AddPromptViewModel(Services.GetService<TypeTheWordPromptViewModel>()!);
        ph.AddPromptViewModel(Services.GetService<GuessTheLetterPromptViewModel>()!);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IPersistenceService, JsonPersistenceService>();
        services.AddSingleton<INotificationHandler, MessageBoxNotificationHandler>();
        services.AddSingleton<QuestionsMessenger>();
        services.AddSingleton<PromptMessenger>();
        services.AddSingleton<AdministrationViewModel>();
        services.AddSingleton<QuizViewModel>();
        services.AddSingleton<TypeTheWordPromptViewModel>();
        services.AddSingleton<GuessTheLetterPromptViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<PromptHandler>();
    }
}