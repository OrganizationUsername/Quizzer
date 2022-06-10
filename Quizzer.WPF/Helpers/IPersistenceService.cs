using Quizzer.WPF.Models;

namespace Quizzer.WPF.Helpers;

public interface IPersistenceService
{
    string SavePromptCollection(PromptCollection pc, string newQuizName);
}