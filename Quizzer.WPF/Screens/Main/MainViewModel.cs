using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Screens.Admin;
using Quizzer.WPF.Screens.Quiz;

namespace Quizzer.WPF.Screens.Main;

[ObservableObject]
public partial class MainViewModel
{
    public AdministrationViewModel AdministrationViewModel { get; set; }
    public QuizViewModel QuizViewModel { get; set; }
    public RelayCommand LoadedCommand => new(Loaded);
    public MainViewModel()
    {
        AdministrationViewModel = new(this);
        QuizViewModel = new();
    }
    public void Loaded() => Debug.WriteLine($"I forget why I wanted this.");
}