using System;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quizzer.WPF.Admin;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Quizzer.WPF.PromptTypes;

public interface IPromptViewModel
{
    public AdministrationViewModel Administration { get; set; }
}

[ObservableObject]
internal partial class GuessTheLetterPromptViewModel : IPromptViewModel
{
    public AdministrationViewModel Administration { get; set; } = null!;
    [ObservableProperty] public string _showText = "";
    [ObservableProperty] public int _width = 150;
    [ObservableProperty] public string? _imageUri = null;
    public readonly string Type = "GuessTheLetterPrompt";

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

        using var resized = new MemoryStream();
        using var image = Image.Load(bytes);

        var width = Math.Min(image.Width, 75);
        var height = Math.Min(image.Height, 75);

        image.Mutate(x => x.Resize(new ResizeOptions() { Mode = ResizeMode.Max, Size = new(width, height) }));
        image.Save(resized, new PngEncoder());

        _imageUri = Convert.ToBase64String(resized.ToArray());
        Debug.WriteLine(_imageUri);
    }
    public void GetModel()
    {
        if (string.IsNullOrWhiteSpace(ShowText)) return;
        var t = new GuessTheLetterPrompt()
        {
            ShowText = _showText,
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