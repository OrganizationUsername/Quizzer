namespace Quizzer.WPF.Models;
public abstract class Prompt
{
    public string ShowText { get; set; }
    public string MutilatedText { get; set; } /*only in `GTLPrompt` */
    public string ImageURI { get; set; }
    public string MathStuff { get; set; } /* Only in `MathProblem : Prompt` */
    public string CorrectAnswer { get; set; }
    public string Type { get; set; }
    public int Width { get; set; }
    public abstract Question GenerateQuestion();
}