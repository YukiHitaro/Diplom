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
                if (module.Tasks != null) // Добавим проверку на null
                {
                    progress.ModuleTaskProgress[module.Id] = module.Tasks
                        .Select(t => new TaskProgressData { IsCompleted = t.IsCompleted, UserCode = t.UserCode ?? string.Empty }) // Добавим ?? string.Empty на всякий случай
                        .ToList();
                }
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(progress, options);
            File.WriteAllText(FilePath, json);
        }
        public class TaskProgressData
        {
            public bool IsCompleted { get; set; }
            public string UserCode { get; set; } = string.Empty;
        }
        public static void LoadProgress(List<Module> modules)
        {
            if (!File.Exists(FilePath)) return;

            var json = File.ReadAllText(FilePath);
            var progress = JsonSerializer.Deserialize<UserProgress>(json);

            if (progress?.ModuleTaskProgress == null) return;

            foreach (var module in modules)
            {
                if (module.Tasks != null && progress.ModuleTaskProgress.TryGetValue(module.Id, out var taskProgressDataList))
                {
                    for (int i = 0; i < module.Tasks.Count && i < taskProgressDataList.Count; i++)
                    {
                        module.Tasks[i].IsCompleted = taskProgressDataList[i].IsCompleted;
                        module.Tasks[i].UserCode = taskProgressDataList[i].UserCode;
                    }
                }
            }
        }
    }
}
