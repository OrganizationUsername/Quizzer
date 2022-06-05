using System;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Helpers;

namespace Quizzer.WPF.PromptTypes;

public interface IPromptViewModel { }

[ObservableObject]
internal partial class GuessTheLetterPromptViewModel : IPromptViewModel
{
    private readonly PromptMessenger _promptMessenger;

    [ObservableProperty] public string _showText = "";
    [ObservableProperty] public int _width = 150;
    [ObservableProperty] public string? _imageUri;
    public readonly string Type = "GuessTheLetterPrompt";

    public GuessTheLetterPromptViewModel(PromptMessenger promptMessenger) => _promptMessenger = promptMessenger;

    public RelayCommand GetModelCommand => new(GetModel);
    public RelayCommand SelectImageCommand => new(SelectImage);
    public void SelectImage()
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog()
        {
            DefaultExt = "png",
            InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
            Filter = "png file | *.png"
        };
        if (openFileDialog.ShowDialog() != true) { return; }
        var bytes = File.ReadAllBytes(openFileDialog.FileName);
        _imageUri = ImageHelper.ImageToString(bytes);
        Debug.WriteLine(_imageUri);
    }
    public void GetModel()
    {
        if (string.IsNullOrWhiteSpace(ShowText)) return;
        _promptMessenger.LoadQuestions(new GuessTheLetterPrompt()
        {
            ShowText = _showText.ToUpperInvariant(),
            Width = _width,
            Type = Type,
            ImageURI = _imageUri
        });
        ResetViewModel();
    }

    public void ResetViewModel()
    {
        ShowText = "";
        Width = 150;
        _imageUri = null;
    }
}