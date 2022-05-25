using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quizzer.WPF.Models;

namespace Quizzer.WPF.PromptTypes;

public class GuessTheLetterPrompt : Prompt
{
    private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const int AnswerCount = 4;
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
        var letterCopy = Letters.ToCharArray().ToList();
        while (possibleAnswers.Count < AnswerCount && letterCopy.Count > 0)
        {
            index = Random.Shared.Next(0, letterCopy.Count);
            possibleAnswers.Add(letterCopy[index]);
            letterCopy.RemoveAt(index);
        }

        var ans = possibleAnswers.OrderBy(x => x).Select(x => new Answer()
        {
            CorrectAnswer = x.ToString() == CorrectAnswer,
            Text = x.ToString()
        }).ToList();
        return new() { Prompt = this, Answers = new(ans) };
    }
}