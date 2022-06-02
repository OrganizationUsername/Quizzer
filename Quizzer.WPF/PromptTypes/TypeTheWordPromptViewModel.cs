using CommunityToolkit.Mvvm.ComponentModel;
using Quizzer.WPF.Helpers;
using Quizzer.WPF.Screens.Admin;

namespace Quizzer.WPF.PromptTypes;

[ObservableObject]
internal partial class TypeTheWordPromptViewModel : IPromptViewModel
{
    private readonly PromptMessenger _promptMessenger;

    public AdministrationViewModel Administration { get; set; } = null!;
    [ObservableProperty] public string _showText = "";
    [ObservableProperty] public int _width = 150;
    [ObservableProperty] public string? _imageUri = null;
    public readonly string Type = "TypeTheWordPrompt";

    public TypeTheWordPromptViewModel(PromptMessenger promptMessenger)
    {
        _promptMessenger = promptMessenger;
    }

    public void GetModel()
    {
        if (_imageUri is null || string.IsNullOrWhiteSpace(ShowText)) { return; }
        var t = new GuessTheLetterPrompt()
        {
            ShowText = _showText.ToUpperInvariant(),
            Width = _width,
            ImageURI = _imageUri,
            Type = Type,
        };
        Administration.Prompts.Add(t);
        ShowText = "";
        Width = 150;
        _imageUri = null;
    }
}