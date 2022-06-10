using Quizzer.WPF.Models;

namespace Quizzer.WPF.Helpers;

public class NonPersistenceService : IPersistenceService
{
    public string SavePromptCollection(PromptCollection pc, string newQuizName) => "Successful save.";
}