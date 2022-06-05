using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Helpers;
using Quizzer.WPF.Models;
using Quizzer.WPF.PromptTypes;

namespace Quizzer.WPF.Screens.Admin;

[ObservableObject]
public partial class AdministrationViewModel
{
    private readonly PromptMessenger _promptMessenger;
    public ObservableCollection<string> Quizzes { get; set; }
    public ObservableCollection<NameAndType> QuestionTypes { get; set; }
    [ObservableProperty] private string? _selectedQuiz;
    private readonly string _directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Quizzer");
    [ICommand] private void ShowTextBox() => MessageBox.Show("Hello!");
    public bool CanExecuteThing() => !string.IsNullOrWhiteSpace(_newQuizName);
    [ObservableProperty] private string _newQuizName = "";
    partial void OnNewQuizNameChanged(string value) => GetJsonCommand.NotifyCanExecuteChanged();

    [ObservableProperty] private IPromptViewModel? _promptViewModel;
    [ObservableProperty] private ObservableCollection<Prompt> _prompts = null!;
    public QuestionsMessenger QuestionsMessenger { get; set; }

    public AdministrationViewModel(QuestionsMessenger questionsMessenger, PromptMessenger promptMessenger)
    {
        _promptMessenger = promptMessenger;
        _promptMessenger.PromptLoaded += ReceivePrompt;

        QuestionsMessenger = questionsMessenger;
        Prompts = new();

        Quizzes = new();
        QuestionTypes = new();
        QuestionTypes.Add(new() { Name = nameof(GuessTheLetterPrompt), Type = typeof(GuessTheLetterPromptViewModel) });
        QuestionTypes.Add(new() { Name = nameof(TypeTheWordPrompt), Type = typeof(TypeTheWordPromptViewModel) });
        SelectedQuestionType = QuestionTypes.First();
    }

    [ObservableProperty] private NameAndType _selectedQuestionType = null!;
    partial void OnSelectedQuestionTypeChanged(NameAndType value) => PromptViewModel = (IPromptViewModel)App.Current.Services.GetService(_selectedQuestionType.Type);

    public void ReceivePrompt(Prompt p) => Prompts.Add(p);

    [ICommand(CanExecute = nameof(CanExecuteThing))]
    public void GetJson()
    {
        //If the new quiz name is blank, return
        //If they select a quiz, I should change the questions that are shown.
        if (nameof(_promptMessenger) == "_promptMessenger") _ = 1;
        var result = new PromptCollection()
        {
            GuessTheLetterPrompts = Prompts.Where(x => x.GetType().Name == "GuessTheLetterPrompt").Cast<GuessTheLetterPrompt>().ToList(),
            TypeTheWordPrompts = Prompts.Where(x => x.GetType().Name == "TypeTheWordPrompt").Cast<TypeTheWordPrompt>().ToList(),
        };
        JsonSerializerOptions options = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        var serialized = JsonSerializer.Serialize(result, options);

        Debug.WriteLine(serialized);
        Quizzes.Add(Quizzes.Count.ToString());
    }

    [ICommand]
    public void Loaded()
    {
        //The stuff in here is quite heavy, and should only be done once, not every time SelectedTabItem changes.

        if (!Directory.Exists(_directory)) { Directory.CreateDirectory(_directory); }
        CreateDefaultPrompts(_directory);

        var fileHashSet = new HashSet<string>();

        var files = Directory.GetFiles(_directory);
        foreach (var file in files)
        {
            var promptPackage = JsonSerializer.Deserialize<PromptCollection>(File.ReadAllText(file));
            //I could iterate through it and make sure all of them have the minimum required fields.
            if (promptPackage is not null) { fileHashSet.Add(Path.GetFileNameWithoutExtension(file)); }
        }
        var tempList = fileHashSet.ToList();
        Quizzes.Clear();
        foreach (var t in tempList) { Quizzes.Add(t); }
    }

    private void CreateDefaultPrompts(string directory)
    {
        JsonSerializerOptions options = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = true };
        try
        {
            var cleanPath = Path.Combine(directory, "Clean.prompts");
            if (Directory.Exists(directory) && !File.Exists(cleanPath))
            {
                var result = new PromptCollection()
                {
                    GuessTheLetterPrompts = PromptService.GetCleanPrompts().Cast<GuessTheLetterPrompt>().ToList(),
                    TypeTheWordPrompts = Prompts.Where(x => x.GetType().Name == "TypeTheWordPrompt")
                        .Cast<TypeTheWordPrompt>().ToList(),
                };
                var serialized = JsonSerializer.Serialize(result, options);
                File.WriteAllText(cleanPath, serialized);
            }

            var dirtyPath = Path.Combine(directory, "Dirty.prompts");
            if (Directory.Exists(directory) && !File.Exists(dirtyPath))
            {
                var result = new PromptCollection()
                {
                    GuessTheLetterPrompts = PromptService.GetDirtyPrompts().Cast<GuessTheLetterPrompt>().ToList(),
                    TypeTheWordPrompts = Prompts.Where(x => x.GetType().Name == "TypeTheWordPrompt")
                        .Cast<TypeTheWordPrompt>().ToList(),
                };
                var serialized = JsonSerializer.Serialize(result, options);
                File.WriteAllText(dirtyPath, serialized);
            }
        }
        catch (Exception ex) { MessageBox.Show($"An error occurred:{Environment.NewLine}{ex}"); }
    }

    [ICommand]
    public void SetQuiz()
    {
        if (SelectedQuiz is null) { return; }
        List<Question> qs;
        var serialized = File.ReadAllText(Path.Combine(_directory, $"{SelectedQuiz}.prompts"));
        var promptPackage = JsonSerializer.Deserialize<PromptCollection>(serialized);
        var prompts = new List<Prompt>();
        foreach (var p in promptPackage.GuessTheLetterPrompts) { prompts.Add(p); }
        foreach (var p in promptPackage.TypeTheWordPrompts) { prompts.Add(p); }
        qs = prompts.OrderBy(x => Random.Shared.Next()).Select(x => x.GenerateQuestion()).ToList();
        QuestionsMessenger.LoadQuestions(qs);
    }
}

public class PromptCollection
{
    public List<GuessTheLetterPrompt> GuessTheLetterPrompts { get; set; }
    public List<TypeTheWordPrompt> TypeTheWordPrompts { get; set; }
}