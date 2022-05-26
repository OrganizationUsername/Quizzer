using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Models;
using Quizzer.WPF.PromptTypes;
using Quizzer.WPF.Quiz;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Quizzer.WPF.Admin;

public class NameAndType
{
    public string Name { get; set; }
    public Type Type { get; set; }
}
public class AdministrationViewModel : ObservableObject
{
    public MainViewModel MainViewModel { get; }
    public RelayCommand SubmitAnswerCommand => new(LoadQuiz);
    public RelayCommand SelectImageCommand => new(SelectImage);
    public ObservableCollection<string> Quizzes { get; set; }
    public ObservableCollection<NameAndType> QuestionTypes { get; set; }
    public string? SelectedQuiz { get; set; }
    public RelayCommand LoadedCommand => new(Loaded);
    private readonly string _directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Quizzer");
    private NameAndType _selectedQuestionType;
    public IPromptViewModel PromptViewModel { get; set; }
    public ObservableCollection<Prompt> Prompts { get; set; }
    public AdministrationViewModel(MainViewModel mainViewModel)
    {
        MainViewModel = mainViewModel;
        Prompts = new();

        Quizzes = new() { "Clean", "Nasty" };
        QuestionTypes = new();
        QuestionTypes.Add(new() { Name = nameof(GuessTheLetterPrompt), Type = typeof(GuessTheLetterPromptViewModel) });
        QuestionTypes.Add(new() { Name = nameof(TypeTheWordPrompt), Type = typeof(TypeTheWordPromptViewModel) });
        SelectedQuestionType = QuestionTypes.First();
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

    public void SelectImage()
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog()
        {
            DefaultExt = "png",
            InitialDirectory = _directory,
            Filter = "png file | *.png"
        };
        if (openFileDialog.ShowDialog() != true) { return; }
        var bytes = File.ReadAllBytes(openFileDialog.FileName);

        using var resized = new MemoryStream();
        using var image = Image.Load(bytes);

        var width = Math.Min(image.Width, 75);
        var height = Math.Min(image.Height, 75);

        image.Mutate(x => x.Resize(new ResizeOptions() { Mode = ResizeMode.Max, Size = new(width, height) }));
        image.Save(resized, new PngEncoder());

        var Other = Convert.ToBase64String(resized.ToArray());
    }

    public void LoadQuiz()
    {
        if (SelectedQuiz is null) return;
        MainViewModel.QuizViewModel.Questions.Clear();
        ObservableCollection<Question> qs;
        switch (SelectedQuiz)
        {
            case "Clean":
                qs = new(PromptGetter.GetCleanPrompts().Select(x => x.GenerateQuestion()));
                foreach (var question in qs) { MainViewModel.QuizViewModel.Questions.Add(question); }
                MainViewModel.QuizViewModel.CurrentQuestions = MainViewModel.QuizViewModel.Questions.First();
                break;
            case "Nasty":
                qs = new(PromptGetter.GetDirtyPrompts().Select(x => x.GenerateQuestion()));
                foreach (var question in qs) { MainViewModel.QuizViewModel.Questions.Add(question); }
                MainViewModel.QuizViewModel.CurrentQuestions = MainViewModel.QuizViewModel.Questions.First();
                break;
            default:
                break;
        }
    }
}