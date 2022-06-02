using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Screens.Admin;
using Quizzer.WPF.Screens.Quiz;
using Microsoft.Extensions.DependencyInjection;


namespace Quizzer.WPF.Screens.Main;

[ObservableObject]
public partial class MainViewModel
{
    [ObservableProperty] private AdministrationViewModel _administrationViewModel;
    [ObservableProperty] private QuizViewModel _quizViewModel;
    public RelayCommand LoadedCommand => new(Loaded);
    public MainViewModel()
    {
        _administrationViewModel = App.Current.Services.GetService<AdministrationViewModel>()!;
        _quizViewModel = App.Current.Services.GetService<QuizViewModel>()!;
    }
    public void Loaded()
    {
        Debug.WriteLine($"I forget why I wanted this.");
    }
}