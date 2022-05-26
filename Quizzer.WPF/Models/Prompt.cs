#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;

namespace Quizzer.WPF.Models;
public abstract class Prompt
{
    public string ShowText { get; set; }
    public string MutilatedText { get; set; } /*only in `GTLPrompt`? */
    public string? ImageURI { get; set; }
    public string MathStuff { get; set; } /* Only in `MathProblem : Prompt` */
    public string CorrectAnswer { get; set; }
    public string Type { get; set; }
    public int Width { get; set; }
    /// <summary>
    /// For types of problems that have set plausible answers.
    /// </summary>
    public List<Answer> Answers { get; set; }
    public abstract Question GenerateQuestion();
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.