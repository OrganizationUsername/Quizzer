using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Helpers;
using Quizzer.WPF.Models;
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
    private Guid _guid;

    public TypeTheWordPromptViewModel(PromptMessenger promptMessenger)
    {
        _promptMessenger = promptMessenger;
        _promptMessenger.PromptPassed += UpdatePrompt;
        _guid = Guid.NewGuid();
    }

    private void UpdatePrompt(Prompt p)
    {
        ShowText = p.ShowText;
        Width = p.Width;
        _guid = p.PromptId;
    }

    public void GetModel()
    {
        if (string.IsNullOrWhiteSpace(ShowText)) { return; }
        _promptMessenger.LoadQuestions(new TypeTheWordPrompt()
        {
            ShowText = _showText.ToUpperInvariant(),
            Width = _width,
            Type = Type,
            PromptId = _guid,
        });
        ResetViewModel();
    }
    public void ResetViewModel()
    {
        ShowText = "";
        Width = 150;
        _imageUri = null;
        _guid = Guid.NewGuid();
    }
}