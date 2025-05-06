using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax; 
using Microsoft.CodeAnalysis;
using System.Text.Json;
using System.IO;
namespace CSharpTrainer
{
    public partial class MainWindow : Window
    {
        private List<Module> Modules = new List<Module>();
        private const string ProgressFile = "progress.json";

        public MainWindow()
        {
            InitializeComponent();
            LoadModules();
            LoadProgress();
            UpdateProgress();
        }

        private void LoadModules()
        {
            Modules = new List<Module>
            {
                new Module
                {
                    Title = "Переменные и типы данных",
                    Description = "Изучите базовые типы данных в C#: int, string, bool и т.д.",
                    Tasks = new List<PracticeTask>
                    {
                        new PracticeTask
                        {
                            TaskDescription = "Объявите переменную int с именем 'age' и значением 30.",
                            ValidationMethod = (assembly) =>
                            {
                                try
                                {
                                    var type = assembly.GetType("UserSubmission.Program");
                                    if (type == null) return false;

                                    var method = type.GetMethod("Main");
                                    if (method == null) return false;


                                    return true;

                                }
                                catch
                                {
                                    return false;
                                }
                            }
                        },
                        new PracticeTask
                        {
                            TaskDescription = "Создайте строковую переменную 'name' и присвойте ей значение 'Аня'.",
                            ValidationMethod = (assembly) => {
                                return true;
                            }
                        },
                        new PracticeTask
                        {
                            TaskDescription = "Объявите логическую переменную 'isActive' и установите её в true.",
                            ValidationMethod = (assembly) => {
                                return true;
                            }
                        }
                    }
                },
                new Module
                {
                    Title = "Условия и циклы",
                    Description = "Операторы if, switch и циклы for, while.",
                    Tasks = new List<PracticeTask>
                    {
                        new PracticeTask
                        {
                            TaskDescription = "Напишите цикл for, который выводит числа от 1 до 10.",
                            ValidationMethod = (assembly) =>
                            {
                                return true;
                            }
                        },
                        new PracticeTask
                        {
                            TaskDescription = "Напишите условие if, проверяющее, что число больше 100.",
                            ValidationMethod = (assembly) => {
                                return true;
                            }
                        },
                        new PracticeTask
                        {
                            TaskDescription = "Реализуйте switch-case для дней недели.",
                            ValidationMethod = (assembly) => {
                                return true;
                            }
                        }
                    }
                },
                new Module
                {
                    Title = "Массивы и списки",
                    Description = "Работа с массивами и списками в C#.",
                    Tasks = new List<PracticeTask>
                    {
                        new PracticeTask { TaskDescription = "Создайте массив из 5 чисел и выведите его элементы." },
                        new PracticeTask { TaskDescription = "Создайте список строк с названиями трёх городов." },
                        new PracticeTask { TaskDescription = "Добавьте элемент в список чисел и удалите его." }
                    }
                },
                new Module
                {
                    Title = "Классы и объекты",
                    Description = "Основы объектно-ориентированного программирования в C#.",
                    Tasks = new List<PracticeTask>
                    {
                        new PracticeTask { TaskDescription = "Создайте класс 'Car' с полем 'Model'." },
                        new PracticeTask { TaskDescription = "Создайте объект класса 'Person' и выведите его имя." },
                        new PracticeTask { TaskDescription = "Добавьте в класс метод, возвращающий строку." }
                    }
                },
                new Module
                {
                    Title = "Обработка исключений",
                    Description = "Как обрабатывать ошибки и исключения в C#.",
                    Tasks = new List<PracticeTask>
                    {
                        new PracticeTask { TaskDescription = "Используйте try-catch для обработки деления на 0." },
                        new PracticeTask { TaskDescription = "Обработайте исключение при чтении файла." },
                        new PracticeTask { TaskDescription = "Создайте собственное исключение и вызовите его." }
                    }
                }
            };

            foreach (var module in Modules)
                ModulesList.Items.Add(module);
        }

        private void OpenModule_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Grid grid = (Grid)VisualTreeHelper.GetParent(button);

            if (grid != null)
            {
                Module module = (Module)grid.DataContext;

                var incompleteTasks = module.Tasks.Where(t => !t.IsCompleted).ToList();
                if (!incompleteTasks.Any())
                {
                    MessageBox.Show("Все задачи в этом модуле уже выполнены.");
                    return;
                }

                var random = new Random();
                var task = incompleteTasks[random.Next(incompleteTasks.Count)];

                var moduleWindow = new ModulePage(module, task);
                moduleWindow.ShowDialog();

                if (moduleWindow.IsCompleted)
                {
                    task.IsCompleted = true;
                    SaveProgress();
                    UpdateProgress();
                }
            }
            else
            {
                MessageBox.Show("Не удалось найти модуль.");
            }
        }

        private void UpdateProgress()
        {
            int completed = Modules.Count(m => m.IsCompleted);
            double progress = (double)completed / Modules.Count * 100;

            DoubleAnimation animation = new DoubleAnimation
            {
                To = progress,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            MainProgressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
        }

        private void SaveProgress()
        {
            try
            {
                var progressData = Modules.Select(m => new
                {
                    m.Title,
                    CompletedTasks = m.Tasks.Select(t => t.IsCompleted).ToList()
                }).ToList();

                var json = JsonSerializer.Serialize(progressData);
                File.WriteAllText(ProgressFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении прогресса: " + ex.Message);
            }
        }

        private void LoadProgress()
        {
            try
            {
                if (!File.Exists(ProgressFile)) return;

                var json = File.ReadAllText(ProgressFile);
                var progressData = JsonSerializer.Deserialize<List<ModuleProgress>>(json);

                foreach (var moduleProgress in progressData)
                {
                    var module = Modules.FirstOrDefault(m => m.Title == moduleProgress.Title);
                    if (module == null) continue;

                    for (int i = 0; i < module.Tasks.Count && i < moduleProgress.CompletedTasks.Count; i++)
                    {
                        module.Tasks[i].IsCompleted = moduleProgress.CompletedTasks[i];
                    }
                 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке прогресса: " + ex.Message);
            }
        }

        private class ModuleProgress
        {
            public string Title { get; set; }
            public List<bool> CompletedTasks { get; set; }
        }
    }
}