﻿using System;
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
    private readonly QuestionsMessenger _questionsMessenger;
    public ObservableCollection<string> Quizzes { get; set; }
    public ObservableCollection<NameAndType> QuestionTypes { get; set; }
    private readonly string _directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Quizzer");
    //[ICommand] private void ShowTextBox() => MessageBox.Show("Hello!");
    public bool CanExecuteThing() => !string.IsNullOrWhiteSpace(_newQuizName) && (SelectedQuiz == _newQuizName || !existingPromptsCollectionNamesLower.Contains(_newQuizName.ToLower()));
    [ObservableProperty] private string _newQuizName = "";
    partial void OnNewQuizNameChanged(string value) => GetJsonCommand.NotifyCanExecuteChanged();
    private readonly HashSet<string> existingPromptsCollectionNames = new();
    private readonly HashSet<string> existingPromptsCollectionNamesLower = new();
    [ObservableProperty] private string? _selectedQuiz;
    [ObservableProperty] private IPromptViewModel? _promptViewModel;
    [ObservableProperty] private ObservableCollection<Prompt> _prompts = null!;
    [ObservableProperty] private Prompt? _selectedPrompt;
    [ObservableProperty] private NameAndType _selectedQuestionType = null!;
    [ObservableProperty] private bool _canSelectQuiz = true;

    [ICommand] public void PassNull() => _promptMessenger.NullPrompt();

    public AdministrationViewModel(QuestionsMessenger questionsMessenger, PromptMessenger promptMessenger)
    {
        _promptMessenger = promptMessenger;
        _promptMessenger.PromptLoaded += ReceivePrompt;

        _questionsMessenger = questionsMessenger;
        Prompts = new();

        Quizzes = new();
        QuestionTypes = new();
        QuestionTypes.Add(new() { Name = nameof(GuessTheLetterPrompt), Type = typeof(GuessTheLetterPromptViewModel) });
        QuestionTypes.Add(new() { Name = nameof(TypeTheWordPrompt), Type = typeof(TypeTheWordPromptViewModel) });
        SelectedQuestionType = QuestionTypes.First();
    }

    partial void OnSelectedQuestionTypeChanged(NameAndType value) => PromptViewModel = (IPromptViewModel)App.Current.Services.GetService(_selectedQuestionType.Type);

    partial void OnSelectedPromptChanged(Prompt? value)
    {
        if (value is null) { return; }
        SelectedQuestionType = QuestionTypes.First(x => x.Name == value.Type);
        _promptMessenger.PassPrompt(value);
    }

    public void ReceivePrompt(Prompt p)
    {
        var existingPrompt = Prompts.FirstOrDefault(x => x.PromptId == p.PromptId);
        if (existingPrompt is not null)
        {
            existingPrompt.ShowText = p.ShowText;
            existingPrompt.ImageURI = p.ImageURI;
            existingPrompt.Width = p.Width;
            return;
        }
        Prompts.Add(p);
    }

    partial void OnSelectedQuizChanged(string? value)
    {
        if (value is null) { return; }
        var prompts = JsonSerializer.Deserialize<PromptCollection>(File.ReadAllText(Path.Combine(_directory, $"{value}.prompts")));
        if (prompts is null) { return; }

        Prompts.Clear();
        if (prompts.GuessTheLetterPrompts != null) { foreach (var prompt in prompts.GuessTheLetterPrompts) { Prompts.Add(prompt); } }
        if (prompts.TypeTheWordPrompts != null) { foreach (var prompt in prompts.TypeTheWordPrompts) { Prompts.Add(prompt); } }

        NewQuizName = value;
    }

    [ICommand]
    public void DeleteSelectedQuestion()
    {
        var promptToDelete = Prompts.FirstOrDefault(x => x.PromptId == SelectedPrompt?.PromptId);
        if (promptToDelete is null) { return; }
        Prompts.Remove(promptToDelete);
    }

    [ICommand]
    public void CreateNewPromptCollection()
    {
        _selectedQuiz = null;
        CanSelectQuiz = false;
        Prompts.Clear();
    }

    [ICommand(CanExecute = nameof(CanExecuteThing))]
    public void GetJson()
    {
        if (nameof(_promptMessenger) == "_promptMessenger") _ = 1;
        var result = new PromptCollection()
        {
            GuessTheLetterPrompts = Prompts.Where(x => x.GetType().Name == "GuessTheLetterPrompt").Cast<GuessTheLetterPrompt>().ToList(),
            TypeTheWordPrompts = Prompts.Where(x => x.GetType().Name == "TypeTheWordPrompt").Cast<TypeTheWordPrompt>().ToList(),
        };
        var serialized = JsonSerializer.Serialize(result,
            new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            });
        File.WriteAllText(Path.Combine(_directory, $"{_newQuizName}.prompts"), serialized);

        Debug.WriteLine(serialized);
        Prompts.Clear();
        if (!existingPromptsCollectionNames.Contains(_newQuizName) && !existingPromptsCollectionNamesLower.Contains(_newQuizName))
        {
            existingPromptsCollectionNames.Add(_newQuizName);
            existingPromptsCollectionNamesLower.Add(_newQuizName);
            Quizzes.Add(_newQuizName);
        }
        CanSelectQuiz = true;
    }

    [ICommand]
    public void Loaded()
    {
        if (!Directory.Exists(_directory)) { Directory.CreateDirectory(_directory); }
        CreateDefaultPrompts(_directory);
        if (Quizzes.Count > 0) { return; }

        var files = Directory.GetFiles(_directory);
        GetValidPromptCollections(files);
        var tempList = existingPromptsCollectionNames.ToList();
        Quizzes.Clear();
        foreach (var t in tempList) { Quizzes.Add(t); }
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

            var qs = prompts.OrderBy(_ => Random.Shared.Next()).Select(p => p.GenerateQuestion()).ToList();
            _questionsMessenger.LoadQuestions(qs);
        }
        catch (Exception e) { MessageBox.Show($"There was an issue loading the prompts from ({SelectedQuiz}).{Environment.NewLine}{e}"); }
    }

    private void GetValidPromptCollections(string[] files)
    {
        foreach (var file in files)
        {
            try
            {
                //I shouldn't have to deserialize just to prove it exists.
                var promptPackage = JsonSerializer.Deserialize<PromptCollection>(File.ReadAllText(file));
                //I could iterate through it and make sure all of them have the minimum required fields.
                if (promptPackage is not null &&
                    ((promptPackage.GuessTheLetterPrompts != null && promptPackage.GuessTheLetterPrompts.Any()) ||
                     (promptPackage.TypeTheWordPrompts != null && promptPackage.TypeTheWordPrompts.Any()))
                   )
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    existingPromptsCollectionNames.Add(fileName);
                    existingPromptsCollectionNamesLower.Add(fileName.ToLower());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
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
                    TypeTheWordPrompts = Prompts.Where(x => x.GetType().Name == "TypeTheWordPrompt").Cast<TypeTheWordPrompt>().ToList(),
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
                    TypeTheWordPrompts = Prompts.Where(x => x.GetType().Name == "TypeTheWordPrompt").Cast<TypeTheWordPrompt>().ToList(),
                };
                var serialized = JsonSerializer.Serialize(result, options);
                File.WriteAllText(dirtyPath, serialized);
            }
        }
        catch (Exception ex) { MessageBox.Show($"An error occurred:{Environment.NewLine}{ex}"); }
    }
}