using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Models;

namespace Quizzer.WPF.Admin;

internal class AdministrationViewModel : ObservableObject
{
    public MainViewModel MainViewModel { get; }
    public RelayCommand SubmitAnswerCommand => new(LoadQuiz);
    public RelayCommand SelectFileCommand => new(SelectFile);
    public ObservableCollection<string> Quizzes { get; set; }
    public string? SelectedQuiz { get; set; }
    public AdministrationViewModel(MainViewModel mainViewModel)
    {
        this.MainViewModel = mainViewModel;
        Quizzes = new() { "Clean", "Nasty" };
    }

    public void SelectFile()
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog()
        {
            DefaultExt = "png",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            Filter = "png file | *.png"
        };
        if (openFileDialog.ShowDialog() != true) { return; }
        var bytes = File.ReadAllBytes(openFileDialog.FileName);
        var file = Convert.ToBase64String(bytes);
    }
    public void LoadQuiz()
    {
        if (SelectedQuiz is null) return;
        MainViewModel.QuizViewModel.Questions.Clear();
        ObservableCollection<Question> qs;
        switch (SelectedQuiz)
        {
            case "Clean":
                qs = new(PromptGetter.GetCleanPrompts().Select(x => x.GenerateQuestion()));
                foreach (var question in qs) { MainViewModel.QuizViewModel.Questions.Add(question); }
                MainViewModel.QuizViewModel.CurrentQuestions = MainViewModel.QuizViewModel.Questions.First();
                break;
            case "Nasty":
                qs = new(PromptGetter.GetDirtyPrompts().Select(x => x.GenerateQuestion()));
                foreach (var question in qs) { MainViewModel.QuizViewModel.Questions.Add(question); }
                MainViewModel.QuizViewModel.CurrentQuestions = MainViewModel.QuizViewModel.Questions.First();
                break;
            default:
                break;
        }
    }
}