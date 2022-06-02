using System;
using System.Collections.Generic;
using Quizzer.WPF.Models;

namespace Quizzer.WPF.Helpers;

public class QuestionsMessenger
{
    public event Action<List<Question>>? QuestionsLoaded;
    public void LoadQuestions(List<Question> product) => QuestionsLoaded?.Invoke(product); // `?` so if no subscribers, no NRE 
}