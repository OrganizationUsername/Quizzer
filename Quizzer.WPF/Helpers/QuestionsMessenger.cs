using System;
using System.Collections.Generic;
using Quizzer.WPF.Models;

namespace Quizzer.WPF.Helpers;

public class QuestionsMessenger
{
    public event Action<List<Question>>? QuestionsLoaded;
    public void LoadQuestions(List<Question> product) => QuestionsLoaded?.Invoke(product); // `?` so if no subscribers, no NRE 
}

public class PromptMessenger
{
    public event Action<Prompt>? PromptLoaded;
    public event Action<Prompt>? PromptPassed;
    public event Action? PromptNulled;
    public void NullPrompt() => PromptNulled?.Invoke();
    public void PassPrompt(Prompt product) => PromptPassed?.Invoke(product);
    public void LoadQuestions(Prompt product) => PromptLoaded?.Invoke(product); // `?` so if no subscribers, no NRE 
}