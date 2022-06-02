using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Models;
using Quizzer.WPF.PromptTypes;
using Quizzer.WPF.Screens.Main;

namespace Quizzer.WPF.Screens.Admin;

[ObservableObject]
public partial class TempQuizViewModel
{
    //I need to work on this so I can save it.
    [ObservableProperty] public string _name;
    [ObservableProperty] public ObservableCollection<Prompt> _prompts;

    public void sth() { }
}

public class QuestionsMessenger
{
    public event Action<List<Question>> QuestionsLoaded;
    public void LoadQuestions(List<Question> product) => QuestionsLoaded?.Invoke(product); // `?` so if no subscribers, no NRE 
}

[ObservableObject]
public partial class AdministrationViewModel
{
    //public MainViewModel MainViewModel { get; }
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
    public QuestionsMessenger QuestionsMessenger { get; set; }
    public AdministrationViewModel(QuestionsMessenger questionsMessenger)
    {
        QuestionsMessenger = questionsMessenger;
        //MainViewModel = mainViewModel;
        Prompts = new();

        Quizzes = new() { "Clean", "Nasty" }; //ToDo: This should be an object of strings and other stuff. Not the way it is now.
        QuestionTypes = new();
        //ToDo: Figure out how to do this with just a Type, or maybe a Type and a colloquial name.
        QuestionTypes.Add(new() { Name = nameof(GuessTheLetterPrompt), Type = typeof(GuessTheLetterPromptViewModel) });
        QuestionTypes.Add(new() { Name = nameof(TypeTheWordPrompt), Type = typeof(TypeTheWordPromptViewModel) });
        SelectedQuestionType = QuestionTypes.First();
    }

    public void GetJsonOfQuestions()
    {
        //If the new quiz name is blank, return
        //If they select a quiz, I should change the questions that are shown.
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
            var type = Assembly.GetExecutingAssembly().GetTypes().First(t => t.Name == _selectedQuestionType.Type!.Name);
            PromptViewModel = (IPromptViewModel)Activator.CreateInstance(type)!;
            PromptViewModel.Administration = this;
            OnPropertyChanged(nameof(PromptViewModel));
        }
    }

    public void Loaded() => Task.Run(() => { if (!Directory.Exists(_directory)) { Directory.CreateDirectory(_directory); } });

    public void LoadQuiz()
    {
        if (SelectedQuiz is null) return;
        //MainViewModel.QuizViewModel.Questions.Clear();
        List<Question> qs;
        switch (SelectedQuiz)
        {
            case "Clean":
                qs = new(PromptService.GetCleanPrompts().Select(x => x.GenerateQuestion()));
                QuestionsMessenger.LoadQuestions(qs);
                //foreach (var question in qs) { MainViewModel.QuizViewModel.Questions.Add(question); }
                //ToDo: Use simple messaging to stop this really coupled stuff.
                //MainViewModel.QuizViewModel.CurrentQuestions = MainViewModel.QuizViewModel.Questions.First();
                break;
            case "Nasty":
                qs = new(PromptService.GetDirtyPrompts().Select(x => x.GenerateQuestion()));
                QuestionsMessenger.LoadQuestions(qs);
                //foreach (var question in qs) { MainViewModel.QuizViewModel.Questions.Add(question); }
                //MainViewModel.QuizViewModel.CurrentQuestions = MainViewModel.QuizViewModel.Questions.First();
                break;
            default:
                qs = new(Prompts.Select(x => x.GenerateQuestion()));
                QuestionsMessenger.LoadQuestions(qs);
                //foreach (var question in qs) { MainViewModel.QuizViewModel.Questions.Add(question); }
                //MainViewModel.QuizViewModel.CurrentQuestions = MainViewModel.QuizViewModel.Questions.First();
                break;
        }
    }
}