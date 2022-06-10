namespace Quizzer.WPF.PromptTypes;

public interface IPromptViewModel
{
    public string ShowText { get; set; }
    public string ImageUri { get; set; }
    void GetModel();
}