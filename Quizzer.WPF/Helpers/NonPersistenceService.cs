using System.Collections.Generic;
using System.Threading.Tasks;
using Quizzer.WPF.Models;

namespace Quizzer.WPF.Helpers;

public class NonPersistenceService : IPersistenceService
{
    public (string, string) SavePromptCollection(PromptCollection pc, string newQuizName) => ("Successful save.", string.Empty);
    public HashSet<string> InitializePersistence()
    {
        throw new System.NotImplementedException();
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