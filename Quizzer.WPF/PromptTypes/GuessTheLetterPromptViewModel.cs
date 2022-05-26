using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Admin;
using Quizzer.WPF.Models;

namespace Quizzer.WPF.PromptTypes;

public interface IPromptViewModel
{
    public AdministrationViewModel Administration { get; set; }
    public void GetModel();
}

[ObservableObject]
internal partial class GuessTheLetterPromptViewModel : IPromptViewModel
{
    public AdministrationViewModel Administration { get; set; }
    [ObservableProperty]
    public string _showText = "";
    [ObservableProperty]
    public int _width = 150;
    [ICommand]
    public void GetModel()
    {
        var t = new GuessTheLetterPrompt()
        {
            ShowText = _showText,
            Width = _width,
        };
        Administration.Prompts.Add(t);
        ShowText = "";
        Width = 150;
    }
}

internal class TypeTheWordPromptViewModel : IPromptViewModel
{
    public AdministrationViewModel Administration { get; set; }
    public string ShowText { get; set; } = null!;
    public int Width { get; set; } = 150;
    public void GetModel()
    {
        var t = new GuessTheLetterPrompt()
        {
            ShowText = ShowText,
            Width = Width,
        };
        Administration.Prompts.Add(t);
    }
}