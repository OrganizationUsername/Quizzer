using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quizzer.WPF.Models;
using Quizzer.WPF.PromptTypes;

namespace Quizzer.WPF.Helpers;

public class NonPersistenceService : IPersistenceService
{
    private readonly List<(PromptCollection pc, string Name)> _promptCollections = new();
    public (string, string) SavePromptCollection(PromptCollection pc, string newQuizName) => ("Successful save.", string.Empty);
    public List<string> InitializePersistence()
    {
        _promptCollections.Add((new() { GuessTheLetterPrompts = PromptInitializationService.GetDirtyPrompts().Cast<GuessTheLetterPrompt>().ToList(), }, "Dirty"));
        _promptCollections.Add((new() { GuessTheLetterPrompts = PromptInitializationService.GetCleanPrompts().Cast<GuessTheLetterPrompt>().ToList(), }, "Clean"));
        return _promptCollections.Select(x => x.Name).ToList();
    }
    public (List<Prompt> prompts, string? error) GetCollectionQuestions(string SelectedQuiz)
    {
        (PromptCollection pc, string Name) result = _promptCollections.FirstOrDefault(x => x.Name == SelectedQuiz);
        if (result.Equals(default)) { return (new(), "Couldn't find PromptCollection"); }
        return (result.pc.GetPrompts(), null);
    }

    public (bool success, string QuizNameOrError) SoftDeleteSelectedQuiz(string? SelectedQuiz) => throw new NotImplementedException();
    public async Task<(List<Prompt> prompts, string? error)> GetPromptsFromNamedCollection(string name, bool deleted)
    {
        var result = _promptCollections.FirstOrDefault(x => x.Name == name);
        if (result.Equals(default)) { return await Task.FromResult((new List<Prompt>(), "Couldn't find PromptCollection")); }
        return await Task.FromResult((result.pc.GetPrompts(), null as string));
    }
}