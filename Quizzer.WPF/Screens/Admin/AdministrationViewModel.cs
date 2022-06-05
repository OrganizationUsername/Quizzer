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
    public bool CanExecuteThing() => !string.IsNullOrWhiteSpace(_newQuizName) && !existingPromptsCollectionNamesLower.Contains(_newQuizName.ToLower()); //For this reason, I should have a HashSet of PromptCollection names.
    [ObservableProperty] private string _newQuizName = "";
    partial void OnNewQuizNameChanged(string value) => GetJsonCommand.NotifyCanExecuteChanged();
    private HashSet<string> existingPromptsCollectionNames = new();
    private HashSet<string> existingPromptsCollectionNamesLower = new();
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
        //Rename it to CreateNewPromptsCollection
        //When they click add a new PromptCollection, that's when I add a new entry on the far left
        //Then they have an empty list of questions. As they add them, the list populates
        //


        //If the new quiz name is blank, return
        //If they select a quiz, I should change the questions that are shown.
        if (nameof(_promptMessenger) == "_promptMessenger") _ = 1;
        var result = new PromptCollection()
        {
            GuessTheLetterPrompts = Prompts.Where(x => x.GetType().Name == "GuessTheLetterPrompt").Cast<GuessTheLetterPrompt>().ToList(),
            TypeTheWordPrompts = Prompts.Where(x => x.GetType().Name == "TypeTheWordPrompt").Cast<TypeTheWordPrompt>().ToList(),
        };
        var serialized = JsonSerializer.Serialize(result, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

        File.WriteAllText(Path.Combine(_directory, $"{_newQuizName}.prompts"), serialized);

        Debug.WriteLine(serialized);
        Quizzes.Add(_newQuizName);
    }

    [ICommand]
    public void Loaded()
    {
        //The stuff in here is quite heavy, and should only be done once, not every time SelectedTabItem changes.

        if (!Directory.Exists(_directory)) { Directory.CreateDirectory(_directory); }
        CreateDefaultPrompts(_directory);

        if (Quizzes.Count > 0) { return; }


        var files = Directory.GetFiles(_directory);
        foreach (var file in files)
        {
            var promptPackage = JsonSerializer.Deserialize<PromptCollection>(File.ReadAllText(file));
            //I could iterate through it and make sure all of them have the minimum required fields.
            if (promptPackage is not null)
            {
                existingPromptsCollectionNames.Add(Path.GetFileNameWithoutExtension(file));
                existingPromptsCollectionNamesLower.Add(Path.GetFileNameWithoutExtension(file).ToLower());
            }
        }
        var tempList = existingPromptsCollectionNames.ToList();
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

        try
        {
            var serialized = File.ReadAllText(Path.Combine(_directory, $"{SelectedQuiz}.prompts"));
            var promptPackage = JsonSerializer.Deserialize<PromptCollection>(serialized);
            var prompts = promptPackage!.GetPrompts();
            if (!prompts.Any()) { MessageBox.Show($"The selected Prompts package ({SelectedQuiz}) contained no valid prompts!"); return; }

            var qs = prompts.OrderBy(x => Random.Shared.Next()).Select(x => x.GenerateQuestion()).ToList();
            QuestionsMessenger.LoadQuestions(qs);
        }
        catch (Exception e) { MessageBox.Show($"There was an issue loading the prompts from ({SelectedQuiz}).{Environment.NewLine}{e}"); }
    }
}