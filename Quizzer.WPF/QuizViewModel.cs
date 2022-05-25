using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Models;

namespace Quizzer.WPF;

internal class MainViewModel : ObservableObject
{
    public AdministrationViewModel AdministrationViewModel { get; set; }
    public QuizViewModel QuizViewModel { get; set; }
    public RelayCommand LoadedCommand => new(Loaded);
    public MainViewModel()
    {
        AdministrationViewModel = new(this);
        QuizViewModel = new();

    }

    public void Loaded()
    {

    }
}

internal class AdministrationViewModel : ObservableObject
{
    public MainViewModel MainViewModel { get; }
    public RelayCommand SubmitAnswerCommand => new(LoadQuiz);
    public RelayCommand SelectFileCommand => new(SelectFile);
    public ObservableCollection<string> Quizzes { get; set; }
    public string? SelectedQuiz { get; set; }
    public AdministrationViewModel(MainViewModel MainViewModel)
    {
        this.MainViewModel = MainViewModel;
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

internal class QuizViewModel : ObservableObject
{
    private Question _currentQuestions = null!;
    public ObservableCollection<Question> Questions { get; set; } = new();
    public Question CurrentQuestions { get => _currentQuestions; set => SetProperty(ref _currentQuestions, value); }
    public List<string> Results { get; set; } = new();
    public RelayCommand<string> SubmitAnswerCommand => new(SubmitAnswer!);

    public QuizViewModel()
    {
        var qs = new ObservableCollection<Question>(PromptGetter.GetCleanPrompts().Select(x => x.GenerateQuestion()));

        foreach (var question in qs) { Questions.Add(question); }
        CurrentQuestions = Questions.First();
    }

    public void SubmitAnswer(string a)
    {
        Results.Add(a == CurrentQuestions.Prompt.CorrectAnswer ? "Good!" : "Bad");
        Questions.RemoveAt(0);
        if (Questions.Count == 0)
        {
            MessageBox.Show($"Results: {string.Join(Environment.NewLine, Results)}");
            Results.Clear();
            return;
        }
        CurrentQuestions = Questions.First();
        OnPropertyChanged(nameof(CurrentQuestions.Prompt.MutilatedText));
    }
}