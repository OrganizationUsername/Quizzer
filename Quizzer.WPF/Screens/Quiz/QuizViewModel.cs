using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Synthesis;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Helpers;
using Quizzer.WPF.Models;


namespace Quizzer.WPF.Screens.Quiz;

[ObservableObject]
public partial class QuizViewModel
{
    [AlsoNotifyCanExecuteFor(nameof(SpeakCommand))][ObservableProperty] private Question? _currentQuestion;
    public ObservableCollection<Question> Questions { get; set; } = new();
    private List<string> Results { get; } = new();
    public RelayCommand<string> SubmitAnswerCommand => new(SubmitAnswer!);
    public RelayCommand SpeakCommand => new(Speak);
    private readonly SpeechSynthesizer speechSynthesizer = new();
    private readonly QuestionsMessenger _questionsMessenger;

    public QuizViewModel(QuestionsMessenger questionsMessenger)
    {
        _questionsMessenger = questionsMessenger;
        _questionsMessenger.QuestionsLoaded += ReceiveQuestions;
    }

    private void ReceiveQuestions(List<Question> products)
    {
        Questions.Clear();
        foreach (var p in products) { Questions.Add(p); }

        CurrentQuestion = Questions.FirstOrDefault();
    }

    public void Speak()
    {
        if (CurrentQuestion is null) { return; }
        speechSynthesizer.Speak(CurrentQuestion.Prompt.ShowText);
        //It can lock up here forever if you keep hitting it after everything's finished?
    }

    public void SubmitAnswer(string a)
    {
        Results.Add(a == CurrentQuestion?.Prompt.CorrectAnswer ? "Good!" : "Bad");
        if (Questions.Count == 0) { return; }
        Questions.RemoveAt(0);
        if (Questions.Count == 0)
        {
            MessageBox.Show($"Results: {string.Join(Environment.NewLine, Results)}");
            Results.Clear();
            return;
        }
        CurrentQuestion = Questions.FirstOrDefault();
    }
}