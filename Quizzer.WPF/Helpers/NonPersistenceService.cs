using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Quizzer.WPF.Models;
using Quizzer.WPF.PromptTypes;

namespace Quizzer.WPF.Helpers;

public class NonPersistenceService : IPersistenceService
{
    private readonly List<(PromptCollection pc, string Name)> _promptCollections = new();

    public (string, string) SavePromptCollection(PromptCollection pc, string newQuizName) => ("Successful save.", string.Empty);
    public HashSet<string> InitializePersistence()
    {
        _promptCollections.Add((new PromptCollection() { GuessTheLetterPrompts = PromptInitializationService.GetDirtyPrompts().Cast<GuessTheLetterPrompt>().ToList(), }, "Dirty"));
        _promptCollections.Add((new PromptCollection() { GuessTheLetterPrompts = PromptInitializationService.GetDirtyPrompts().Cast<GuessTheLetterPrompt>().ToList(), }, "Clear"));
        return _promptCollections.Select(x => x.Name).ToHashSet();
    }

    public (bool success, string QuizNameOrError) SoftDeleteSelectedQuiz(string? SelectedQuiz)
    {
        throw new System.NotImplementedException();
    }

    public (HashSet<string> existingNames, HashSet<string> existingNamesLower, List<string> quizNames) GetPromptCollections(bool includingDeleted)
    {
        throw new System.NotImplementedException();
    }

    public Task<List<Prompt>> GetPromptsFromNamedCollection(string name, bool deleted)
    {
        throw new System.NotImplementedException();
    }
}