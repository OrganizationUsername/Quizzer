using System.Collections.Generic;
using Quizzer.WPF.PromptTypes;

namespace Quizzer.WPF.Models;

public class PromptCollection
{
    public bool Deleted { get; set; } = false;
    public List<GuessTheLetterPrompt>? GuessTheLetterPrompts { get; set; }
    public List<TypeTheWordPrompt>? TypeTheWordPrompts { get; set; }

    public List<Prompt> GetPrompts()
    {
        var result = new List<Prompt>();
        if (GuessTheLetterPrompts is not null) { foreach (var p in GuessTheLetterPrompts) { result.Add(p); } }
        if (TypeTheWordPrompts is not null) { foreach (var p in TypeTheWordPrompts) { result.Add(p); } }

        return result;
    }
}