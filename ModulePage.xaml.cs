using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpTrainer
{
    public partial class ModulePage : Window
    {
        private Module _module;
        public bool IsCompleted => _module.IsCompleted;
        private PracticeTask _selectedTask;

        // Шаблон кода (добавлены using)
        private const string CodeTemplate = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace UserSubmission
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // <UserCode>
        }
    }
}
";

        public ModulePage(Module module)
        {
            InitializeComponent();
            _module = module;

            TitleBlock.Text = module.Title;
            DescriptionBlock.Text = module.Description;
            TaskList.ItemsSource = _module.Tasks;

            // Calculate and display module progress
            UpdateModuleProgress();
        }

        private void UpdateModuleProgress()
        {
            int completedTasks = _module.Tasks.Count(t => t.IsCompleted);
            double progress = (double)completedTasks / _module.Tasks.Count * 100;
            ModuleProgressBar.Value = progress;
        }

        private void TaskList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedTask = TaskList.SelectedItem as PracticeTask;
            if (_selectedTask != null)
            {
                TaskBlock.Text = _selectedTask.TaskDescription;
                StartTaskButton.Visibility = Visibility.Visible;
                AnswerBox.Text = string.Empty;
            }
        }

        private void StartTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string userCode = AnswerBox.Text;

            if (string.IsNullOrWhiteSpace(userCode))
            {
                MessageBox.Show("Пожалуйста, введите код.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var result = CompileAndValidate(userCode);
                if (result)
                {
                    MessageBox.Show("Верно! Задание выполнено ✅", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    _selectedTask.IsCompleted = true;
                    UpdateModuleProgress();

                    // Проверка завершён ли весь модуль
                    if (_module.IsCompleted)
                    {
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Код скомпилировался, но не прошёл проверку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка компиляции или выполнения:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CompileAndValidate(string code)
        {
            string fullCode = CodeTemplate.Replace("// <UserCode>", code);

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(fullCode);

            var compilation = CSharpCompilation.Create(
                assemblyName: "UserSubmission",
                syntaxTrees: new[] { syntaxTree },
                references: new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location)
                },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                var errors = string.Join("\n", result.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => d.ToString()));
                throw new Exception(errors);
            }

            ms.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(ms.ToArray());

            return _selectedTask.ValidationMethod?.Invoke(assembly) ?? false;
        }
    }
}