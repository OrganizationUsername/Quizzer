using System.Collections.Generic;
using System.Threading.Tasks;
using Quizzer.WPF.Models;

namespace Quizzer.WPF.Helpers;

public interface IPersistenceService
{
    (string SaveMessage, string ErrorMessage) SavePromptCollection(PromptCollection pc, string newQuizName);
    /// <summary>
    /// Gets names of all PromptCollections and sets up defaults if there are none.
    /// </summary>
    /// <returns></returns>
    List<string> InitializePersistence();
    (List<Prompt> prompts, string? error) GetCollectionQuestions(string SelectedQuiz);
    (bool success, string QuizNameOrError) SoftDeleteSelectedQuiz(string? SelectedQuiz);
    Task<List<Prompt>> GetPromptsFromNamedCollection(string name, bool deleted);
}