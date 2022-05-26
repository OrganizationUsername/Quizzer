﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Models;
using Quizzer.WPF.PromptTypes;
using Quizzer.WPF.Quiz;

namespace Quizzer.WPF.Admin;

public class AdministrationViewModel : ObservableObject
{
    public MainViewModel MainViewModel { get; }
    public RelayCommand SubmitAnswerCommand => new(LoadQuiz);
    public ObservableCollection<string> Quizzes { get; set; }
    public ObservableCollection<NameAndType> QuestionTypes { get; set; }
    public string? SelectedQuiz { get; set; }
    public RelayCommand LoadedCommand => new(Loaded);
    public RelayCommand GetJsonCommand => new(GetJsonOfQuestions);
    private readonly string _directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Quizzer");
    private NameAndType _selectedQuestionType = null!;
    public IPromptViewModel PromptViewModel { get; set; } = null!;
    public ObservableCollection<Prompt> Prompts { get; set; }
    public AdministrationViewModel(MainViewModel mainViewModel)
    {
        MainViewModel = mainViewModel;
        Prompts = new();

        Quizzes = new() { "Clean", "Nasty" };
        QuestionTypes = new();
        //ToDo: Figure out how to do this with just a Type, or maybe a Type and a colloquial name.
        QuestionTypes.Add(new() { Name = nameof(GuessTheLetterPrompt), Type = typeof(GuessTheLetterPromptViewModel) });
        QuestionTypes.Add(new() { Name = nameof(TypeTheWordPrompt), Type = typeof(TypeTheWordPromptViewModel) });
        SelectedQuestionType = QuestionTypes.First();
    }

    public void GetJsonOfQuestions()
    {
        var text = System.Text.Json.JsonSerializer.Serialize(Prompts);
        Debug.WriteLine(text);
        Quizzes.Add(Quizzes.Count.ToString());
    }
    public NameAndType SelectedQuestionType
    {
        get => _selectedQuestionType;
        set
        {
            _selectedQuestionType = value;
            var type = Assembly.GetExecutingAssembly().GetTypes().First(t => t.Name == _selectedQuestionType.Type.Name);
            PromptViewModel = (IPromptViewModel)Activator.CreateInstance(type)!;
            PromptViewModel.Administration = this;
            OnPropertyChanged(nameof(PromptViewModel));
        }
    }

    public void Loaded() { if (!Directory.Exists(_directory)) { Directory.CreateDirectory(_directory); } }

    public void LoadQuiz()
    {
        if (SelectedQuiz is null) return;
        MainViewModel.QuizViewModel.Questions.Clear();
        ObservableCollection<Question> qs;
        switch (SelectedQuiz)
        {
            case "Clean":
                qs = new(PromptService.GetCleanPrompts().Select(x => x.GenerateQuestion()));
                foreach (var question in qs) { MainViewModel.QuizViewModel.Questions.Add(question); }
                MainViewModel.QuizViewModel.CurrentQuestions = MainViewModel.QuizViewModel.Questions.First();
                break;
            case "Nasty":
                qs = new(PromptService.GetDirtyPrompts().Select(x => x.GenerateQuestion()));
                foreach (var question in qs) { MainViewModel.QuizViewModel.Questions.Add(question); }
                MainViewModel.QuizViewModel.CurrentQuestions = MainViewModel.QuizViewModel.Questions.First();
                break;
            default:
                qs = new(Prompts.Select(x => x.GenerateQuestion()));
                foreach (var question in qs) { MainViewModel.QuizViewModel.Questions.Add(question); }
                MainViewModel.QuizViewModel.CurrentQuestions = MainViewModel.QuizViewModel.Questions.First();
                break;
        }
    }
}