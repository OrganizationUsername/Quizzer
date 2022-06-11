using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using Quizzer.WPF.Models;
using Quizzer.WPF.PromptTypes;

namespace Quizzer.WPF.Helpers;

public class JsonPersistenceService : IPersistenceService
{
    private readonly string _directory = Path.Combine(Environment.SpecialFolder.CommonDocuments.ToString(), "Quizzer");
    public (string SaveMessage, string ErrorMessage) SavePromptCollection(PromptCollection pc, string newQuizName)
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
            return (serialized, string.Empty);
        }
        catch (Exception ex)
        {
            return (string.Empty, ex.ToString());
        }
    }

    public (bool success, string QuizNameOrError) SoftDeleteSelectedQuiz(string? SelectedQuiz)
    {
        try
        {
            if (SelectedQuiz is null) { return (false, "Quiz not found"); }

            //Change selected
            var serializedThing = File.ReadAllText(Path.Combine(_directory, $"{SelectedQuiz}.prompts"));
            var promptPackage = JsonSerializer.Deserialize<PromptCollection>(serializedThing);
            if (promptPackage is null) { return (false, "Quiz not found"); }
            promptPackage.Deleted = true;

            var serialized = JsonSerializer.Serialize(promptPackage,
                new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                });
            File.WriteAllText(Path.Combine(_directory, $"{SelectedQuiz}.prompts"), serialized);
            return (true, SelectedQuiz);
        }
        catch (Exception e) { return (false, e.ToString()); }
    }

    public HashSet<string> InitializePersistence()
    {
        return GetPromptCollections(true);
    }

    public HashSet<string> GetPromptCollections(bool includingDeleted)
    {
        if (!Directory.Exists(_directory)) { Directory.CreateDirectory(_directory); }
        CreateDefaultPrompts(_directory);

        var filePaths = Directory.GetFiles(_directory);
        var (existingNames, existingNamesLower) = GetValidPromptCollections(filePaths);

        return existingNames;
    }

    private (HashSet<string> existingNames, HashSet<string> existingNamesLower) GetValidPromptCollections(string[] filePaths)
    {
        var existingPromptsCollectionNames = new HashSet<string>();
        var existingPromptsCollectionNamesLower = new HashSet<string>();
        foreach (var file in filePaths)
        {
            try
            {
                //I shouldn't have to deserialize just to prove it exists. But I do have to deserialize to figure out if it's been soft-deleted.
                var promptPackage = JsonSerializer.Deserialize<PromptCollection>(File.ReadAllText(file));
                //I could iterate through it and make sure all of them have the minimum required fields.

                if (promptPackage is not null && /*I should include it even if it's deleted, so we don't end up overwriting something that exists.*/
                    ((promptPackage.GuessTheLetterPrompts != null && promptPackage.GuessTheLetterPrompts.Any()) || /*has either or*/
                     (promptPackage.TypeTheWordPrompts != null && promptPackage.TypeTheWordPrompts.Any()))
                   )
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    existingPromptsCollectionNames.Add(fileName);
                    existingPromptsCollectionNamesLower.Add(fileName.ToLower());
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                //I should return a Task that includes a place for List<string> errors
            }
        }
        //ToDo: When I'm returning them, I should somehow indicate if they're deleted or not.
        //I should focus less on the deleted stuff.
        return (existingPromptsCollectionNames, existingPromptsCollectionNamesLower);
    }

    private void CreateDefaultPrompts(string directory)
    {
        var serializerOptions = new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = true };
        try
        {
            var cleanPath = Path.Combine(directory, "Clean.prompts");
            if (Directory.Exists(directory) && !File.Exists(cleanPath))
            {
                var result = new PromptCollection()
                {
                    GuessTheLetterPrompts = PromptInitializationService.GetCleanPrompts().Cast<GuessTheLetterPrompt>().ToList(),
                };
                var serialized = JsonSerializer.Serialize(result, serializerOptions);
                File.WriteAllText(cleanPath, serialized);
            }

            var dirtyPath = Path.Combine(directory, "Dirty.prompts");
            if (Directory.Exists(directory) && !File.Exists(dirtyPath))
            {
                var result = new PromptCollection()
                {
                    GuessTheLetterPrompts = PromptInitializationService.GetDirtyPrompts().Cast<GuessTheLetterPrompt>().ToList(),
                };
                var serialized = JsonSerializer.Serialize(result, serializerOptions);
                File.WriteAllText(dirtyPath, serialized);
            }
        }
        catch (Exception ex) { MessageBox.Show($"An error occurred:{Environment.NewLine}{ex}"); }
    }

    public async Task<List<Prompt>> GetPromptsFromNamedCollection(string name, bool deleted)
    {
        try
        {
            await using (var payLoad = File.OpenRead(Path.Combine(_directory, $"{name}.prompts{(deleted ? "x" : "")}")))
            {
                var ps = await JsonSerializer.DeserializeAsync<PromptCollection>(payLoad);
                if (ps is null) { return await Task.FromResult(new List<Prompt>()); }

                var prompts = new List<Prompt>();
                if (ps.GuessTheLetterPrompts is not null) { foreach (var prompt in ps.GuessTheLetterPrompts) { prompts.Add(prompt); } }
                if (ps.TypeTheWordPrompts is not null) { foreach (var prompt in ps.TypeTheWordPrompts) { prompts.Add(prompt); } }

                return prompts;
            }
        }
        catch (Exception e) { return await Task.FromResult(new List<Prompt>()); }
    }
}