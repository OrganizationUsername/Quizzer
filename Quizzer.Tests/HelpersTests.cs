using System.Linq;
using Quizzer.WPF.Helpers;
using Xunit;

namespace Quizzer.Tests;

public class HelpersTests
{
    [Fact]
    public void CheckGetCleanPrompts_Ok()
    {
        var ps = PromptInitializationService.GetCleanPrompts();
        Assert.Equal(3, ps.Count);
        Assert.Equal(0, ps.Count(x => x.ImageURI is not null));
        foreach (var prompt in ps)
        {
            var q = prompt.GenerateQuestion();
            Assert.Equal(4, q.Answers.Count);
            Assert.Contains(q.Answers, x => x.Text == q.Prompt.CorrectAnswer);
        }
    }
    [Fact]
    public void CheckGetDirtyPrompts_Ok()
    {
        var ps = PromptInitializationService.GetDirtyPrompts();
        Assert.Equal(3, ps.Count);
        Assert.Equal(1, ps.Count(x => x.ImageURI is not null));
        foreach (var prompt in ps)
        {
            var q = prompt.GenerateQuestion();
            Assert.Equal(4, q.Answers.Count);
            Assert.Contains(q.Answers, x => x.Text == q.Prompt.CorrectAnswer);
        }
    }
}