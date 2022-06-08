using System;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Helpers;
using Quizzer.WPF.Models;

namespace Quizzer.WPF.PromptTypes;

public interface IPromptViewModel { }

[ObservableObject]
internal partial class GuessTheLetterPromptViewModel : IPromptViewModel
{
    private readonly PromptMessenger _promptMessenger;
    [ObservableProperty] public string _showText = "";
    [ObservableProperty] public int _width = 150;
    [ObservableProperty] public string? _imageUri;
    [ObservableProperty] private string _saveUpdateText = "Save Prompt";
    private const string savePromptString = "Save Prompt";
    private const string updatePromptString = "Update Prompt";
    public readonly string Type = "GuessTheLetterPrompt";
    private Guid _guid;
    private string lastDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public GuessTheLetterPromptViewModel(PromptMessenger promptMessenger)
    {
        _promptMessenger = promptMessenger;
        _promptMessenger.PromptPassed += UpdatePrompt;
        _promptMessenger.PromptNulled += ResetViewModel;
        _guid = Guid.NewGuid();
    }

    private void UpdatePrompt(Prompt p)
    {
        ShowText = p.ShowText;
        Width = p.Width;
        _guid = p.PromptId;
        ImageUri = p.ImageURI;
        SaveUpdateText = updatePromptString;
    }

    public RelayCommand GetModelCommand => new(GetModel);
    public RelayCommand SelectImageCommand => new(SelectImage);
    public void SelectImage()
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog()
        {
            DefaultExt = "png",
            InitialDirectory = lastDirectory,
            Filter = "png file | *.png"
        };
        if (openFileDialog.ShowDialog() != true) { return; }


        lastDirectory = Path.GetFullPath(openFileDialog.FileName);
        var bytes = File.ReadAllBytes(openFileDialog.FileName);
        ImageUri = ImageHelper.ImageToString(bytes, 75, 75);
        Debug.WriteLine(_imageUri);
    }

    public void GetModel()
    {
        if (string.IsNullOrWhiteSpace(ShowText)) return;
        _promptMessenger.LoadQuestions(new GuessTheLetterPrompt() { ShowText = _showText.ToUpperInvariant(), Width = _width, Type = Type, ImageURI = _imageUri, PromptId = _guid, });
        ResetViewModel();
    }

    public void ResetViewModel()
    {
        ShowText = "";
        Width = 150;
        ImageUri = null;
        _guid = Guid.NewGuid();
        SaveUpdateText = savePromptString;
    }
}