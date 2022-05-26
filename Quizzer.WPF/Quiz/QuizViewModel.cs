using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Admin;
using Quizzer.WPF.Models;

namespace Quizzer.WPF.Quiz;

public class MainViewModel : ObservableObject
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

public class QuizViewModel : ObservableObject
{
    private Question _currentQuestions = null!;
    public ObservableCollection<Question> Questions { get; set; } = new();
    public Question CurrentQuestions { get => _currentQuestions; set => SetProperty(ref _currentQuestions, value); }
    public List<string> Results { get; set; } = new();
    public RelayCommand<string> SubmitAnswerCommand => new(SubmitAnswer!);

    public QuizViewModel()
    {
        //var qs = new ObservableCollection<Question>(PromptService.GetCleanPrompts().Select(x => x.GenerateQuestion()));
        //foreach (var question in qs) { QuestionTypes.Add(question); }
        //CurrentQuestions = QuestionTypes.First();
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