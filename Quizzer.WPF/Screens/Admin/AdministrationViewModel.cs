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
    private readonly string _explorer = Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe";
    private readonly PromptMessenger _promptMessenger;
    internal readonly PromptHandler _promptHandler;
    private readonly IPersistenceService _persistenceService;
    private readonly QuestionsMessenger _questionsMessenger;
    public ObservableCollection<string> Quizzes { get; set; }
    public ObservableCollection<NameAndType> QuestionTypes { get; set; }
    private readonly string _directory = Path.Combine(Environment.SpecialFolder.CommonDocuments.ToString(), "Quizzer");
    public bool CanExecuteThing() => !string.IsNullOrWhiteSpace(_newQuizName) && (SelectedQuiz == _newQuizName || !existingPromptsCollectionNamesLower.Contains(_newQuizName.ToLower()));
    [ObservableProperty] private string _newQuizName = "";
    partial void OnNewQuizNameChanged(string value) => SavePromptCollectionCommand.NotifyCanExecuteChanged();
    private readonly HashSet<string> existingPromptsCollectionNames = new();
    private readonly HashSet<string> existingPromptsCollectionNamesLower = new();
    [ObservableProperty] private string? _selectedQuiz;
    [ObservableProperty] private IPromptViewModel? _promptViewModel;
    [ObservableProperty] private ObservableCollection<Prompt> _prompts = null!;
    [ObservableProperty] private Prompt? _selectedPrompt;
    [ObservableProperty] private NameAndType _selectedQuestionType = null!;
    [ObservableProperty] private bool _canSelectQuiz = true;

    [ICommand]
    public void PassNull()
    {
        _promptMessenger.NullPrompt();
        SelectedPrompt = null;
    }

    public AdministrationViewModel(QuestionsMessenger questionsMessenger, PromptMessenger promptMessenger, PromptHandler promptHandler, IPersistenceService persistenceService)
    {
        _promptMessenger = promptMessenger;
        _promptMessenger.PromptLoaded += ReceivePrompt;

        _questionsMessenger = questionsMessenger;
        Prompts = new();

        _promptHandler = promptHandler;
        _persistenceService = persistenceService;

        Quizzes = new();
        QuestionTypes = new(promptHandler.GetQuestionTypes());
        SelectedQuestionType = QuestionTypes.First();
    }

    partial void OnSelectedQuestionTypeChanged(NameAndType value) => PromptViewModel = _promptHandler.GetViewModel(_selectedQuestionType.Type);

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

    async partial void OnSelectedQuizChanged(string? value)
    {
        if (value is null) { return; }

        var prompts = await _persistenceService.GetPromptsFromNamedCollection(value, false);
        if (!prompts.Any()) { return; }

        Prompts.Clear();
        foreach (var p in prompts) { Prompts.Add(p); }

        NewQuizName = value;
    }

    [ICommand]
    public void DeleteSelectedQuestion()
    {
        var promptToDelete = Prompts.FirstOrDefault(x => x.PromptId == SelectedPrompt?.PromptId);
        if (promptToDelete is null) { return; }
        Prompts.Remove(promptToDelete);
    }

    [ICommand] public void OpenQuizDirectory() => Process.Start(_explorer, _directory);

    [ICommand]
    public void DeleteSelectedQuiz()
    {
        var (success, quizNameOrError) = _persistenceService.SoftDeleteSelectedQuiz(SelectedQuiz);
        if (success) { Quizzes.Remove(quizNameOrError); }
        else { MessageBox.Show($"There was an error:{Environment.NewLine}{quizNameOrError}"); }
    }

    [ICommand]
    public void CreateNewPromptCollection()
    {
        _selectedQuiz = null;
        CanSelectQuiz = false;
        Prompts.Clear();
    }

    [ICommand(CanExecute = nameof(CanExecuteThing))]
    public void SavePromptCollection()
    {
        if (nameof(_promptMessenger) == "_promptMessenger") _ = 1;
        var result = new PromptCollection()
        {
            GuessTheLetterPrompts = Prompts.Where(x => x.GetType().Name == "GuessTheLetterPrompt").Cast<GuessTheLetterPrompt>().ToList(),
            TypeTheWordPrompts = Prompts.Where(x => x.GetType().Name == "TypeTheWordPrompt").Cast<TypeTheWordPrompt>().ToList(),
        };

        var (saveMessage, errorMessage) = _persistenceService.SavePromptCollection(result, _newQuizName);

        if (string.IsNullOrWhiteSpace(saveMessage))
        {
            MessageBox.Show($"There was an issue saving.{Environment.NewLine}{errorMessage}");
            return;
        }

        Trace.WriteLine(saveMessage);
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
        var existingNames = _persistenceService.InitializePersistence();
        Quizzes.Clear();
        existingPromptsCollectionNames.Clear();
        existingPromptsCollectionNamesLower.Clear();
        foreach (var fileName in existingNames)
        {
            existingPromptsCollectionNames.Add(fileName);
            existingPromptsCollectionNamesLower.Add(fileName.ToLower());
            Quizzes.Add(fileName);
        }
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
}