using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quizzer.WPF.Models;

namespace Quizzer.WPF.PromptTypes;

public class GuessTheLetterPrompt : Prompt
{
    private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public override Question GenerateQuestion()
    {
        int index;
        do
        {
            index = Random.Shared.Next(0, ShowText.Length);
            CorrectAnswer = ShowText[index].ToString();
        } while (string.IsNullOrWhiteSpace(CorrectAnswer) || !char.IsLetter(ShowText[index]));
        var s = new StringBuilder(ShowText) { [index] = '_' };
        MutilatedText = s.ToString();
        var possibleAnswers = new HashSet<char> { ShowText[index] };
        while (possibleAnswers.Count < 4)
        {
            index = Random.Shared.Next(0, Letters.Length);
            possibleAnswers.Add(Letters[index]);
        }

        var ans = possibleAnswers.OrderBy(x => x).Select(x => new Answer()
        {
            CorrectAnswer = x.ToString() == CorrectAnswer,
            Text = x.ToString()
        }).ToList();
        return new() { Prompt = this, Answers = new(ans) };
    }
}