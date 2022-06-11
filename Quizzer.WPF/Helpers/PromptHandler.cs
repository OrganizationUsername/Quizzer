using System;
using System.Collections.Generic;
using System.Linq;
using Quizzer.WPF.Models;
using Quizzer.WPF.PromptTypes;

namespace Quizzer.WPF.Helpers;

public class PromptHandler
{
    public List<IPromptViewModel> Prompts { get; set; } = new();
    public void AddPromptViewModel(IPromptViewModel pvm) => Prompts.Add(pvm);
    public IPromptViewModel GetViewModel(Type type) => Prompts.First(x => x.GetType().Name == type.Name);

    public List<NameAndType> GetQuestionTypes()
    {
        var questionTypes = new List<NameAndType>
        {
            new() { Name = nameof(GuessTheLetterPrompt), Type = typeof(GuessTheLetterPromptViewModel) },
            new() { Name = nameof(TypeTheWordPrompt), Type = typeof(TypeTheWordPromptViewModel) }
        };

        return questionTypes;
    }
}