using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Screens.Admin;
using Quizzer.WPF.Screens.Quiz;

namespace Quizzer.WPF.Screens.Main;

[ObservableObject]
public partial class MainViewModel
{
    [ObservableProperty] private AdministrationViewModel _administrationViewModel;
    [ObservableProperty] private QuizViewModel _quizViewModel;
    public RelayCommand LoadedCommand => new(Loaded);
    public MainViewModel(AdministrationViewModel administrationViewModel, QuizViewModel quizViewModel)
    {
        _administrationViewModel = administrationViewModel;
        _quizViewModel = quizViewModel;
    }
    public void Loaded() => Debug.WriteLine("I forget why I wanted this.");
}