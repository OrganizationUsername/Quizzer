using Quizzer.WPF.Helpers;
using Quizzer.WPF.PromptTypes;
using Xunit;
using Quizzer.WPF.Screens.Admin;

namespace Quizzer.Tests;

public class AdminViewModelTests
{
    public AdministrationViewModel GetAdminViewModel()
    {
        var questionsMessenger = new QuestionsMessenger();
        var promptMessenger = new PromptMessenger();
        var promptHandler = new PromptHandler();
        var persistenceHandler = new NonPersistenceService();

        promptHandler.AddPromptViewModel(new GuessTheLetterPromptViewModel(promptMessenger));
        promptHandler.AddPromptViewModel(new TypeTheWordPromptViewModel(promptMessenger));
        var result = new AdministrationViewModel(questionsMessenger, promptMessenger, promptHandler, persistenceHandler);
        result.Loaded();
        return result;
    }

    [Fact]
    public void Initialization_Ok()
    {
        var avm = GetAdminViewModel();

        avm.NewQuizName = "First";
        Assert.True(avm.SelectedQuestionType.Type == typeof(GuessTheLetterPromptViewModel)); //This is terrible.
        Assert.True(avm.Prompts.Count == 0);
        Assert.True(avm.Quizzes.Count == 2);
    }

    [Fact]
    public void AddPrompt_Ok()
    {
        var avm = GetAdminViewModel();

        avm.NewQuizName = "First";
        var x = avm.PromptViewModel!;
        x.ShowText = "A real word.";
        x.GetModel();
        Assert.True(avm.Prompts.Count == 1);
        Assert.True(string.IsNullOrEmpty(x.ShowText));
    }

    [Fact]
    public void AddEmptyShowText_Bad()
    {
        var avm = GetAdminViewModel();
        avm.NewQuizName = "First";
        var x = avm.PromptViewModel!;
        x.ShowText = "    ";
        x.GetModel();
        Assert.True(avm.Prompts.Count == 0);
    }

    [Fact]
    public void AllQuestionTypesAreValid()
    {
        //Incredibly brittle and should fail once I add more complicated PromptViewModels.
        //Having a question that requires multiple question choices should fail.
        var avm = GetAdminViewModel();
        avm.NewQuizName = "First";

        for (var i = 0; i < avm.QuestionTypes.Count; i++)
        {
            avm.SelectedQuestionType = avm.QuestionTypes[i];
            var x = avm.PromptViewModel!;
            Assert.NotNull(x);
            x.ShowText = x.GetType().Name;
            x.GetModel();
            Assert.True(avm.Prompts.Count == i + 1);
        }

        for (var i = 0; i < avm.QuestionTypes.Count; i++)
        {
            Assert.True(avm.Prompts[i].GetType().Name == avm.QuestionTypes[i].Name);
        }
        avm.SavePromptCollection();
        Assert.True(avm.Prompts.Count == 0);
        Assert.True(avm.Quizzes.Count == 3);
    }
}