#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;

namespace Quizzer.WPF.Models;

/*
I'm imagining for a word list, I'd be able to go through and generate several questions.
Words that have images... Can put up a word and show 4 images and make them select the correct one.
I guess for some types of Prompts, I should be able to directly transfer them to other types of questions.
Kind of pad the number of questions by making them in other forms.
 */

/* Prompts still don't have defined answers. */
public abstract class Prompt
{
    public string ShowText { get; set; }
    public string MutilatedText { get; set; } /*only in `GTLPrompt`? */
    public string? ImageURI { get; set; }
    public string MathStuff { get; set; } /* Only in `MathProblem : Prompt` */
    public string CorrectAnswer { get; set; }
    /// <summary>
    /// Some basic option or json. Maybe I should use a List of String
    /// </summary>
    public string Options { get; set; }
    /// <summary>
    /// Probably don't even want to use this. I should probably store inherited types.
    /// </summary>
    public string Type { get; set; }
    public int Width { get; set; }
    /// <summary>
    /// For types of problems that have set plausible answers.
    /// </summary>
    public List<Answer> Answers { get; set; }
    public abstract Question GenerateQuestion();
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.