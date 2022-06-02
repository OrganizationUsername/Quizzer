using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Quizzer.WPF.Helpers;
using Quizzer.WPF.PromptTypes;
using Quizzer.WPF.Screens.Admin;
using Quizzer.WPF.Screens.Main;
using Quizzer.WPF.Screens.Quiz;

namespace Quizzer.WPF
{
    public partial class App : Application
    {
        public ServiceProvider Services { get; set; }
        public new static App Current => (App)Application.Current;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();
            var x = new MainWindow();/*{ DataContext = Services.GetService<MainViewModel>() };*/ /*if this were VM-first, I wouldn't have this issue. */
            x.Show();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<QuestionsMessenger>();
            services.AddSingleton<PromptMessenger>();
            services.AddSingleton<AdministrationViewModel>();
            services.AddSingleton<QuizViewModel>();
            services.AddSingleton<GuessTheLetterPromptViewModel>();
            services.AddSingleton<MainViewModel>();
        }
    }
}
