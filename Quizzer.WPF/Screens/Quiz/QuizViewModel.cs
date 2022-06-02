using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Synthesis;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Models;
using Quizzer.WPF.Screens.Admin;


namespace Quizzer.WPF.Screens.Quiz;

[ObservableObject]
public partial class QuizViewModel
{
    private Question _currentQuestions = null!;
    public ObservableCollection<Question> Questions { get; set; } = new();
    public Question CurrentQuestions { get => _currentQuestions; set => SetProperty(ref _currentQuestions, value); }
    public List<string> Results { get; set; } = new();
    public RelayCommand<string> SubmitAnswerCommand => new(SubmitAnswer!);
    public RelayCommand SpeakCommand => new(Speak);
    private readonly SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
    private readonly QuestionsMessenger _questionsMessenger;

    public QuizViewModel(QuestionsMessenger questionsMessenger)
    {
        _questionsMessenger = questionsMessenger;
        _questionsMessenger.QuestionsLoaded += ReceiveQuestions;
    }
    private void ReceiveQuestions(List<Question> products)
    {
        foreach (var p in products) { Questions.Add(p); }
        CurrentQuestions = Questions.First();
    }

    public void Speak() => speechSynthesizer.Speak(CurrentQuestions.Prompt.ShowText);

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