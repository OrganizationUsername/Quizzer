using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Quizzer.WPF.Models;

namespace Quizzer.WPF.Helpers;

public class JsonPersistenceService : IPersistenceService
{
    private readonly string _directory = Path.Combine(Environment.SpecialFolder.CommonDocuments.ToString(), "Quizzer");
    public string SavePromptCollection(PromptCollection pc, string newQuizName)
    {
        try
        {
            var serialized = JsonSerializer.Serialize(pc,
                new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                });
            File.WriteAllText(Path.Combine(_directory, $"{newQuizName}.prompts"), serialized);
            return serialized;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public void SoftDeletePromptCollection()
    {
        //Maybe for soft-deleting, change the extension to `.promptsX`
    }

    public void GetPromptCollections()
    {

    }
}