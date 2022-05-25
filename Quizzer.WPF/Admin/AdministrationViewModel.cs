using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Models;
using Quizzer.WPF.Quiz;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace Quizzer.WPF.Admin;

internal class AdministrationViewModel : ObservableObject
{
    public MainViewModel MainViewModel { get; }
    public RelayCommand SubmitAnswerCommand => new(LoadQuiz);
    public RelayCommand SelectFileCommand => new(SelectFile);
    public ObservableCollection<string> Quizzes { get; set; }
    public string? SelectedQuiz { get; set; }
    public AdministrationViewModel(MainViewModel mainViewModel)
    {
        MainViewModel = mainViewModel;
        Quizzes = new() { "Clean", "Nasty" };
    }

    public void SelectFile()
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog()
        {
            DefaultExt = "png",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            Filter = "png file | *.png"
        };
        if (openFileDialog.ShowDialog() != true) { return; }
        var bytes = File.ReadAllBytes(openFileDialog.FileName);
        var file = Convert.ToBase64String(bytes);

        using (var resized = new MemoryStream())
        using (var image = Image.Load(bytes))
        {
            var width = Math.Min(image.Width, 75);
            var height = Math.Min(image.Height, 75);

            image.Mutate(x => x.Resize(new ResizeOptions()
            {
                Mode = ResizeMode.Max,
                Size = new(width, height)
            }));

            image.Save(resized, new PngEncoder());
            var Other = Convert.ToBase64String(resized.ToArray());
        }
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