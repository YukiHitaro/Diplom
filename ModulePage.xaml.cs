using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.CodeAnalysis.CSharp.Syntax; // Добавляем using для Roslyn

namespace CSharpTrainer
{
    public partial class ModulePage : Window
    {
        private readonly Module _module;
        private readonly PracticeTask _currentTask;
        public bool IsCompleted { get; private set; }

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

        public ModulePage(Module module, PracticeTask task)
        {
            InitializeComponent();
            _module = module;
            _currentTask = task;

            TitleBlock.Text = module.Title;
            DescriptionBlock.Text = module.Description;
            TaskBlock.Text = _currentTask.TaskDescription;
        }

        private void CheckAnswer_Click(object sender, RoutedEventArgs e)
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
                    MessageBox.Show("Верно! Модуль пройден ✅", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    IsCompleted = true;
                    this.Close();
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

            return _currentTask.ValidationMethod?.Invoke(assembly) ?? false;
        }
    }
}