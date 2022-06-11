﻿using System;
using System.Collections.Generic;
using Quizzer.WPF.Models;
using Quizzer.WPF.PromptTypes;

namespace Quizzer.WPF.Helpers;

public static class PromptInitializationService
{
    //ToDo: These two should both be written as PromptCollections, not a list of prompts.
    public static List<Prompt> GetCleanPrompts() => new()
    {
        new GuessTheLetterPrompt() { ShowText = "DOG", Width = 150, PromptId = Guid.NewGuid(), Type = "GuessTheLetterPrompt" },
        new GuessTheLetterPrompt() { ShowText = "CAT", Width = 150, PromptId = Guid.NewGuid(), Type = "GuessTheLetterPrompt" },
        new GuessTheLetterPrompt() { ShowText = "THIS IS A VERY LONG SENTENCE", Width = 300, PromptId = Guid.NewGuid(), Type = "GuessTheLetterPrompt" },
    };

    public static List<Prompt> GetDirtyPrompts() => new()
    {
        new GuessTheLetterPrompt() { ShowText = "FART", Width = 150, ImageURI = @"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", PromptId = Guid.NewGuid(), Type = "GuessTheLetterPrompt" },
        new GuessTheLetterPrompt() { ShowText = "BUTT", Width = 150, PromptId = Guid.NewGuid(), Type = "GuessTheLetterPrompt" },
        new GuessTheLetterPrompt() { ShowText = "THESE WORDS ARE INAPPROPRIATE", Width = 300, PromptId = Guid.NewGuid(), Type = "GuessTheLetterPrompt" },
    };
}