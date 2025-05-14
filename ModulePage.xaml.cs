// ModulePage.xaml.cs
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace CSharpTrainer
{
    public partial class ModulePage : Window
    {
        private Module _module;
        public bool IsCompleted => _module.IsCompleted;
        private PracticeTask _selectedTask;
        private string _tempFilePath = Path.Combine(Path.GetTempPath(), "UserSubmission.cs");
        private int _attemptCount = 0;

        public static event Action ProgressShouldBeSavedAndUpdated;

        private const string CodeTemplate = @"
using System;
using System.Collections.Generic;
using System.Linq;
// Добавьте using System.Threading.Tasks; если нужны Task, Parallel
// using System.Threading.Tasks;
// Добавьте using System.Net.Http; если нужен HttpClient
// using System.Net.Http;
// Добавьте using System.Data; и специфичный для БД, если ADO.NET
// using System.Data; 
// using Microsoft.Data.Sqlite; // или System.Data.SQLite;

namespace UserSubmission
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*USER_CODE*/
        }
    }
    // Пользователь может определять свои классы вне класса Program, если нужно
    /*ADDITIONAL_CLASSES_AREA*/
}
";
        // Для задач, где нужно создавать классы, можно использовать ADDITIONAL_CLASSES_AREA
        // Или научить пользователя помещать их в Program.Main, что менее реалистично.
        // Для простоты, пока будем предполагать, что большинство кода пишется в Main.

        public ModulePage(Module module)
        {
            InitializeComponent();
            _module = module;
            DataContext = _module;

            TaskList.ItemsSource = _module.Tasks;
            UpdateModuleProgress();

            if (_module.Tasks.Any()) // Выбираем первую задачу по умолчанию
            {
                TaskList.SelectedIndex = 0;
            }
        }

        private void UpdateModuleProgress()
        {
            int completedTasks = _module.Tasks.Count(t => t.IsCompleted);
            double progress = _module.Tasks.Count > 0 ? (double)completedTasks / _module.Tasks.Count * 100 : 0;
            ModuleProgressBar.Value = progress;
        }

        private void TaskList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedTask = TaskList.SelectedItem as PracticeTask;
            if (_selectedTask != null)
            {
                _attemptCount = 0; // Сбрасываем счетчик попыток при смене задачи
                HintText.Text = "Подсказка будет доступна после 3 неудачных попыток"; // Сброс текста подсказки
                HintText.Visibility = _selectedTask.IsCompleted ? Visibility.Collapsed : Visibility.Hidden;
                DetailedDescriptionBorder.Visibility = Visibility.Collapsed;

                TaskBlock.Text = _selectedTask.TaskDescription;
                DetailedDescriptionBlock.Text = _selectedTask.DetailedDescription;
                AnswerBox.Text = _selectedTask.UserCode ?? string.Empty; // Загружаем сохраненный код
                AnswerBox.IsEnabled = !_selectedTask.IsCompleted;
                StartTaskButton.Visibility = _selectedTask.IsCompleted ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                TaskBlock.Text = "Выберите задачу из списка.";
                DetailedDescriptionBlock.Text = string.Empty;
                AnswerBox.Text = string.Empty;
                AnswerBox.IsEnabled = false;
                StartTaskButton.Visibility = Visibility.Collapsed;
                HintText.Visibility = Visibility.Hidden;
                DetailedDescriptionBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void AnswerBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectedTask != null && !_selectedTask.IsCompleted)
            {
                _selectedTask.UserCode = AnswerBox.Text;
            }
        }

        private void ResetModuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_module == null) return;

            var confirmation = MessageBox.Show(
                $"Вы уверены, что хотите сбросить весь прогресс для модуля \"{_module.Title}\"? " +
                "Весь введенный код для задач этого модуля также будет удален.",
                "Подтверждение сброса",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmation == MessageBoxResult.Yes)
            {
                foreach (var task in _module.Tasks)
                {
                    task.IsCompleted = false;
                    task.UserCode = string.Empty;
                }

                TaskList.Items.Refresh();
                UpdateModuleProgress();

                // Обновляем правую панель, если задача была выбрана
                if (TaskList.SelectedItem is PracticeTask currentTask)
                {
                    // Вызываем TaskList_SelectionChanged искусственно для обновления правой панели
                    var selectedIndex = TaskList.SelectedIndex;
                    TaskList.SelectedIndex = -1; // Сбрасываем выбор, чтобы гарантированно сработал SelectionChanged
                    TaskList.SelectedIndex = selectedIndex; // Восстанавливаем выбор
                }
                else // Если ничего не было выбрано, но модуль сброшен
                {
                    TaskBlock.Text = "Выберите задачу из списка.";
                    DetailedDescriptionBlock.Text = string.Empty;
                    AnswerBox.Text = string.Empty;
                    AnswerBox.IsEnabled = false;
                    StartTaskButton.Visibility = Visibility.Collapsed;
                    HintText.Visibility = Visibility.Hidden;
                    DetailedDescriptionBorder.Visibility = Visibility.Collapsed;
                }


                ProgressShouldBeSavedAndUpdated?.Invoke();

                MessageBox.Show($"Прогресс для модуля \"{_module.Title}\" сброшен.", "Прогресс сброшен", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void StartTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTask == null) return;
            if (_selectedTask.IsCompleted)
            {
                MessageBox.Show("Это задание уже выполнено.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // UserCode уже должен быть обновлен в _selectedTask благодаря AnswerBox_TextChanged
            // string userCode = AnswerBox.Text; // Это можно убрать, если используется TextChanged
            // _selectedTask.UserCode = userCode; // И это тоже

            if (string.IsNullOrWhiteSpace(_selectedTask.UserCode))
            {
                MessageBox.Show("Пожалуйста, введите код.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Разделяем код на тот, что внутри Main, и тот, что определяет классы
                string mainUserCode = _selectedTask.UserCode;
                string additionalClassesCode = string.Empty;

                // Простая эвристика для разделения: если есть 'class Program { static void Main', то остальное может быть классами
                // Это очень грубо и может не работать для всех случаев.
                // Более надежно было бы просить пользователя явно разделять или использовать регионы.
                // Для текущей задачи, если пользователь определяет классы, они должны быть вне Program.Main в шаблоне.
                // Пока оставим простой вариант: весь код пользователя идет в /*USER_CODE*/.
                // Если нужно определение классов, пользователь должен их писать так, чтобы они были доступны.

                string fullCode = CodeTemplate.Replace("/*USER_CODE*/", mainUserCode)
                                            .Replace("/*ADDITIONAL_CLASSES_AREA*/", additionalClassesCode);
                File.WriteAllText(_tempFilePath, fullCode);

                var result = CompileAndValidate(_tempFilePath);
                if (result)
                {
                    MessageBox.Show("Верно! Задание выполнено ✅", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    _selectedTask.IsCompleted = true;

                    StartTaskButton.Visibility = Visibility.Collapsed;
                    AnswerBox.IsEnabled = false;
                    HintText.Visibility = Visibility.Collapsed;
                    DetailedDescriptionBorder.Visibility = Visibility.Collapsed;

                    UpdateModuleProgress();
                    ProgressShouldBeSavedAndUpdated?.Invoke();

                    if (_module.IsCompleted)
                    {
                        MessageBox.Show($"Поздравляем! Модуль \"{_module.Title}\" полностью пройден!", "Модуль завершен!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    _attemptCount++;
                    string attemptMessage = $"Неверно! Код скомпилировался, но не прошёл проверку.";
                    if (_attemptCount >= 3)
                    {
                        HintText.Visibility = Visibility.Visible;
                        DetailedDescriptionBorder.Visibility = Visibility.Visible;
                        attemptMessage += $"\nПодсказка доступна. Попытка {_attemptCount}.";
                    }
                    else
                    {
                        attemptMessage += $" Попытка {_attemptCount}/3.";
                    }
                    HintText.Text = $"Попытка {_attemptCount}/3. Подсказка:";


                    MessageBox.Show(attemptMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                _attemptCount++;
                string compileErrorMessage = $"Ошибка компиляции:\n{ex.Message}";
                if (_attemptCount >= 3)
                {
                    HintText.Visibility = Visibility.Visible;
                    DetailedDescriptionBorder.Visibility = Visibility.Visible;
                    compileErrorMessage += $"\nПодсказка доступна. Попытка {_attemptCount}.";
                }
                else
                {
                    compileErrorMessage += $" Попытка {_attemptCount}/3.";
                }
                HintText.Text = $"Попытка {_attemptCount}/3. Подсказка:";

                MessageBox.Show(compileErrorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CompileAndValidate(string filePath)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(filePath));

            var compilation = CSharpCompilation.Create(
                assemblyName: "UserSubmission",
                syntaxTrees: new[] { syntaxTree },
                references: new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(System.ComponentModel.EditorBrowsableAttribute).Assembly.Location), // Для System
                    MetadataReference.CreateFromFile(typeof(System.Collections.Generic.List<>).Assembly.Location), // Для List<T>
                    MetadataReference.CreateFromFile(typeof(System.ComponentModel.INotifyPropertyChanged).Assembly.Location), // Для INotifyPropertyChanged (хотя обычно не нужен в коде пользователя)
                    MetadataReference.CreateFromFile(typeof(System.Threading.Tasks.Task).Assembly.Location), // Для Task, async/await
                    MetadataReference.CreateFromFile(typeof(System.Threading.Thread).Assembly.Location), // Для Thread
                    MetadataReference.CreateFromFile(typeof(System.Net.Http.HttpClient).Assembly.Location), // Для HttpClient
                    MetadataReference.CreateFromFile(typeof(System.Data.Common.DbConnection).Assembly.Location), // Для ADO.NET
                    // Если используется System.Data.SQLite, то его сборку нужно добавить. Обычно это NuGet пакет.
                    // MetadataReference.CreateFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System.Data.SQLite.dll")), // Пример
                    // Если используется Entity Framework Core, то также его сборки.
                    // MetadataReference.CreateFromFile(typeof(Microsoft.EntityFrameworkCore.DbContext).Assembly.Location), // Пример для EF Core
                },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                var errors = string.Join("\n", result.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => d.GetMessage()));
                throw new Exception(errors);
            }

            ms.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(ms.ToArray());

            RoslynContext.Set(syntaxTree, compilation);
            bool validationResult = false;
            try
            {
                validationResult = _selectedTask.ValidationMethod?.Invoke(assembly) ?? false;
            }
            finally
            {
                RoslynContext.Clear();
            }
            return validationResult;
        }
    }
}