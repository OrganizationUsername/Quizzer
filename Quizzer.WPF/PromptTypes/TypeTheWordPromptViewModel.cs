using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Helpers;
using Quizzer.WPF.Screens.Admin;

namespace Quizzer.WPF.PromptTypes;

[ObservableObject]
internal partial class TypeTheWordPromptViewModel : IPromptViewModel
{
    private readonly PromptMessenger _promptMessenger;

    [ObservableProperty] public string _showText = "";
    [ObservableProperty] public int _width = 150;
    [ObservableProperty] public string? _imageUri = null;
    public readonly string Type = "TypeTheWordPrompt";
    public RelayCommand GetModelCommand => new(GetModel);
    public TypeTheWordPromptViewModel(PromptMessenger promptMessenger) { _promptMessenger = promptMessenger; }

    public void GetModel()
    {
        if (string.IsNullOrWhiteSpace(ShowText)) { return; }
        _promptMessenger.LoadQuestions(new TypeTheWordPrompt()
        {
            ShowText = _showText.ToUpperInvariant(),
            Width = _width,
            Type = Type,
            PromptId = Guid.NewGuid(),
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