﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Helpers;
using Quizzer.WPF.Models;
using Quizzer.WPF.PromptTypes;

namespace Quizzer.WPF.Screens.Admin;

[ObservableObject]
public partial class TempQuizViewModel
{
    //I need to work on this so I can save it.
    [ObservableProperty] public string _name;
    [ObservableProperty] public ObservableCollection<Prompt> _prompts;

    public void sth() { }
}

[ObservableObject]
public partial class AdministrationViewModel
{
    private readonly PromptMessenger _promptMessenger;
    public RelayCommand SubmitAnswerCommand => new(LoadQuiz);
    public ObservableCollection<string> Quizzes { get; set; }
    public ObservableCollection<NameAndType> QuestionTypes { get; set; }
    public string? SelectedQuiz { get; set; }
    public RelayCommand LoadedCommand => new(Loaded);
    public IRelayCommand GetJsonCommand => new RelayCommand(GetJsonOfQuestions, () => !string.IsNullOrWhiteSpace(_newQuizName));
    private readonly string _directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Quizzer");

    [ObservableProperty] private IPromptViewModel? _promptViewModel;
    public ObservableCollection<Prompt> Prompts { get; set; }
    public QuestionsMessenger QuestionsMessenger { get; set; }
    [AlsoNotifyChangeFor(nameof(GetJsonCommand))]
    [ObservableProperty] private string _newQuizName = "";
    public AdministrationViewModel(QuestionsMessenger questionsMessenger, PromptMessenger promptMessenger)
    {
        _promptMessenger = promptMessenger;
        _promptMessenger.PromptLoaded += ReceivePrompt;
        if (nameof(_promptMessenger) == "_promptMessenger") _ = 1;

        QuestionsMessenger = questionsMessenger;
        Prompts = new();

        Quizzes = new() { "Clean", "Nasty" }; //ToDo: This should be an object of strings and other stuff. Not the way it is now.
        QuestionTypes = new();
        QuestionTypes.Add(new() { Name = nameof(GuessTheLetterPrompt), Type = typeof(GuessTheLetterPromptViewModel) });
        QuestionTypes.Add(new() { Name = nameof(TypeTheWordPrompt), Type = typeof(TypeTheWordPromptViewModel) });
        SelectedQuestionType = QuestionTypes.First();
    }

    private NameAndType _selectedQuestionType = null!;
    public NameAndType SelectedQuestionType
    {
        get => _selectedQuestionType;
        set { _selectedQuestionType = value; PromptViewModel = (IPromptViewModel)App.Current.Services.GetService(_selectedQuestionType.Type); }
    }

    public void ReceivePrompt(Prompt p) => Prompts.Add(p);

    public void GetJsonOfQuestions()
    {
        //If the new quiz name is blank, return
        //If they select a quiz, I should change the questions that are shown.
        var text = System.Text.Json.JsonSerializer.Serialize(Prompts);

        var result = new PromptCollection()
        {
            GuessTheLetterPrompts = Prompts.Where(x => x.GetType().Name == "GuessTheLetterPrompt").Cast<GuessTheLetterPrompt>().ToList(),
            TypeTheWordPrompts = Prompts.Where(x => x.GetType().Name == "TypeTheWordPrompt").Cast<TypeTheWordPrompt>().ToList(),
        };
        JsonSerializerOptions options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var serialized = System.Text.Json.JsonSerializer.Serialize(result, options);

        Console.WriteLine(serialized);

        Debug.WriteLine(text);
        Quizzes.Add(Quizzes.Count.ToString());
    }
    public void Loaded()
    {
        if (!Directory.Exists(_directory)) { Directory.CreateDirectory(_directory); }
        /* Get existing questions there. */
    }

    public void LoadQuiz()
    {
        if (SelectedQuiz is null) return;
        List<Question> qs;
        switch (SelectedQuiz)
        {
            case "Clean":
                qs = new(PromptService.GetCleanPrompts().Select(x => x.GenerateQuestion()));
                break;
            case "Nasty":
                qs = new(PromptService.GetDirtyPrompts().Select(x => x.GenerateQuestion()));
                break;
            default:
                qs = new(Prompts.Select(x => x.GenerateQuestion()));
                break;
        }
        QuestionsMessenger.LoadQuestions(qs);
    }
}

public class PromptCollection
{
    public List<GuessTheLetterPrompt> GuessTheLetterPrompts { get; set; }
    public List<TypeTheWordPrompt> TypeTheWordPrompts { get; set; }
}