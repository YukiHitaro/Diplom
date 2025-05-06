using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
namespace CSharpTrainer
{
    public static class ProgressManager
    {
        private const string FilePath = "user_progress.json";

        public static void SaveProgress(List<Module> modules)
        {
            var progress = new UserProgress();

            foreach (var module in modules)
            {
                progress.ModuleTaskCompletion[module.Id] = module.Tasks
                    .Select(t => t.IsCompleted).ToList();
            }

            var json = JsonSerializer.Serialize(progress, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static void LoadProgress(List<Module> modules)
        {
            if (!File.Exists(FilePath)) return;

            var json = File.ReadAllText(FilePath);
            var progress = JsonSerializer.Deserialize<UserProgress>(json);

            if (progress?.ModuleTaskCompletion == null) return;

            foreach (var module in modules)
            {
                if (progress.ModuleTaskCompletion.TryGetValue(module.Id, out var taskCompletionList))
                {
                    for (int i = 0; i < module.Tasks.Count && i < taskCompletionList.Count; i++)
                    {
                        module.Tasks[i].IsCompleted = taskCompletionList[i];
                    }
                }
            }
        }
    }

}
