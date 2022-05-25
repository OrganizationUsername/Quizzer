using System.Collections.Generic;

namespace Quizzer.WPF.Models;
public class Question
{
    public Prompt Prompt { get; set; } = null!;
    public List<Answer> Answers { get; set; } = new();
}