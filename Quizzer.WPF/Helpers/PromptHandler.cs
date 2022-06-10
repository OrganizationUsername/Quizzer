using System;
using System.Collections.Generic;
using System.Linq;
using Quizzer.WPF.PromptTypes;
using Quizzer.WPF.Screens.Admin;

namespace Quizzer.WPF.Helpers;

public class PromptHandler
{
    public List<IPromptViewModel> Prompts { get; set; } = new();
    public void AddPromptViewModel(IPromptViewModel pvm) => Prompts.Add(pvm);
    public IPromptViewModel GetViewModel(Type type) => Prompts.First(x => x.GetType().Name == type.Name);

    public List<NameAndType> GetQuestionTypes()
    {
        var questionTypes = new List<NameAndType>();
        questionTypes.Add(new() { Name = nameof(GuessTheLetterPrompt), Type = typeof(GuessTheLetterPromptViewModel) });
        questionTypes.Add(new() { Name = nameof(TypeTheWordPrompt), Type = typeof(TypeTheWordPromptViewModel) });

        return questionTypes;
    }
}