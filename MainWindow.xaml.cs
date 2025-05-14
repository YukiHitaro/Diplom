using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Text.Json;
using System.IO;
namespace CSharpTrainer
{
    public partial class MainWindow : Window
    {
        private List<Module> Modules = new List<Module>();
        //private const string ProgressFile = "progress.json";

        public MainWindow()
        {
            InitializeComponent();
            LoadModules();       // Теперь этот метод ТОЛЬКО заполняет коллекцию this.Modules
            LoadProgress();      // Применяем сохраненный прогресс к объектам в this.Modules

            
            UpdateOverallProgress();
            foreach (var module in Modules) // Обновить состояние IsCompleted для всех модулей после загрузки
            {
                module.RaiseIsCompletedChanged();
            }
            ModulePage.ProgressShouldBeSavedAndUpdated += OnProgressShouldBeSavedAndUpdated;
        }

        private void OnProgressShouldBeSavedAndUpdated()
        {
            SaveProgress(); 
            UpdateOverallProgress();
            foreach (var module in Modules)
            {
                module.RaiseIsCompletedChanged();
            }
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
                    TaskDescription = "1. Объявите переменную типа int с именем 'age' и значением 30.",
                    DetailedDescription = "Создайте целочисленную переменную age и инициализируйте её значением 30. Пример: int age = 30;",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program" &&
                                           m.Modifiers.Any(SyntaxKind.StaticKeyword));

                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();

                        foreach (var declarationStatement in localDeclarations)
                        {
                            var typeSymbolInfo = semanticModel.GetTypeInfo(declarationStatement.Declaration.Type);
                            if (typeSymbolInfo.Type == null || typeSymbolInfo.Type.SpecialType != SpecialType.System_Int32) continue;

                            foreach (var variable in declarationStatement.Declaration.Variables)
                            {
                                if (variable.Identifier.Text == "age")
                                {
                                    if (variable.Initializer != null && variable.Initializer.Value != null)
                                    {
                                        var constantValue = semanticModel.GetConstantValue(variable.Initializer.Value);
                                        if (constantValue.HasValue && constantValue.Value is int intValue && intValue == 30)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "2. Создайте строковую переменную 'name' со значением 'Аня'.",
                    DetailedDescription = "Объявите переменную типа string с именем name и присвойте ей значение \"Аня\". Пример: string name = \"Аня\";",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program" &&
                                           m.Modifiers.Any(SyntaxKind.StaticKeyword));

                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();

                        foreach (var declarationStatement in localDeclarations)
                        {
                            var typeSymbolInfo = semanticModel.GetTypeInfo(declarationStatement.Declaration.Type);
                            if (typeSymbolInfo.Type == null || typeSymbolInfo.Type.SpecialType != SpecialType.System_String) continue;

                            foreach (var variable in declarationStatement.Declaration.Variables)
                            {
                                if (variable.Identifier.Text == "name")
                                {
                                    if (variable.Initializer != null && variable.Initializer.Value is LiteralExpressionSyntax literal)
                                    {
                                        if (literal.IsKind(SyntaxKind.StringLiteralExpression) && literal.Token.ValueText == "Аня")
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "3. Объявите логическую переменную 'isActive' со значением true.",
                    DetailedDescription = "Создайте переменную типа bool с именем isActive и установите её в true. Пример: bool isActive = true;",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program" &&
                                           m.Modifiers.Any(SyntaxKind.StaticKeyword));
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var declarationStatement in localDeclarations)
                        {
                            var typeSymbolInfo = semanticModel.GetTypeInfo(declarationStatement.Declaration.Type);
                            if (typeSymbolInfo.Type == null || typeSymbolInfo.Type.SpecialType != SpecialType.System_Boolean) continue;

                            foreach (var variable in declarationStatement.Declaration.Variables)
                            {
                                if (variable.Identifier.Text == "isActive")
                                {
                                    if (variable.Initializer != null && variable.Initializer.Value != null)
                                    {
                                        var constantValue = semanticModel.GetConstantValue(variable.Initializer.Value);
                                        if (constantValue.HasValue && constantValue.Value is bool boolValue && boolValue == true)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "4. Создайте переменную типа double с именем 'price' и значением 19.99.",
                    DetailedDescription = "Объявите переменную типа double с именем price и значением 19.99. Пример: double price = 19.99;",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program" &&
                                           m.Modifiers.Any(SyntaxKind.StaticKeyword));
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var declarationStatement in localDeclarations)
                        {
                            var typeSymbolInfo = semanticModel.GetTypeInfo(declarationStatement.Declaration.Type);
                            if (typeSymbolInfo.Type == null || typeSymbolInfo.Type.SpecialType != SpecialType.System_Double) continue;

                            foreach (var variable in declarationStatement.Declaration.Variables)
                            {
                                if (variable.Identifier.Text == "price")
                                {
                                    if (variable.Initializer != null && variable.Initializer.Value != null)
                                    {
                                        var constantValue = semanticModel.GetConstantValue(variable.Initializer.Value);
                                        // Сравнение double требует осторожности из-за точности
                                        if (constantValue.HasValue && constantValue.Value is double doubleValue && Math.Abs(doubleValue - 19.99) < 0.0001)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "5. Объявите константу типа string с именем 'GREETING' и значением 'Hello'.",
                    DetailedDescription = "Создайте константу типа string с именем GREETING и значением \"Hello\". Пример: const string GREETING = \"Hello\";",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        // --- 1. Проверка локальной константы в Main ---
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c1 && c1.Identifier.Text == "Program" &&
                                           m.Modifiers.Any(SyntaxKind.StaticKeyword));

                        if (mainMethodNode != null && mainMethodNode.Body != null)
                        {
                            var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                            foreach (var declarationStatement in localDeclarations)
                            {
                                // Проверяем, что это константа (LocalDeclarationStatementSyntax)
                                if (!declarationStatement.Modifiers.Any(SyntaxKind.ConstKeyword)) continue;

                                var typeSymbolInfo = semanticModel.GetTypeInfo(declarationStatement.Declaration.Type);
                                if (typeSymbolInfo.Type == null || typeSymbolInfo.Type.SpecialType != SpecialType.System_String) continue;

                                foreach (var variable in declarationStatement.Declaration.Variables)
                                {
                                    if (variable.Identifier.Text == "GREETING")
                                    {
                                        if (variable.Initializer != null && variable.Initializer.Value is LiteralExpressionSyntax literal)
                                        {
                                            if (literal.IsKind(SyntaxKind.StringLiteralExpression) && literal.Token.ValueText == "Hello")
                                            {
                                                return true; // Найдена локальная константа
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // --- 2. Проверка константы уровня класса Program (FieldDeclarationSyntax) ---
                        var programClassNode = root.DescendantNodes()
                            .OfType<ClassDeclarationSyntax>()
                            .FirstOrDefault(c => c.Identifier.Text == "Program");

                        if (programClassNode != null)
                        {
                            var fieldDeclarations = programClassNode.DescendantNodes().OfType<FieldDeclarationSyntax>();
                            foreach (var fieldDeclaration in fieldDeclarations)
                            {
                                // Проверяем, что это константа (FieldDeclarationSyntax)
                                if (!fieldDeclaration.Modifiers.Any(SyntaxKind.ConstKeyword)) continue;

                                var fieldTypeSymbolInfo = semanticModel.GetTypeInfo(fieldDeclaration.Declaration.Type);
                                if (fieldTypeSymbolInfo.Type == null || fieldTypeSymbolInfo.Type.SpecialType != SpecialType.System_String) continue;

                                foreach (var variable in fieldDeclaration.Declaration.Variables)
                                {
                                    if (variable.Identifier.Text == "GREETING")
                                    {
                                        if (variable.Initializer != null && variable.Initializer.Value is LiteralExpressionSyntax literal)
                                        {
                                            if (literal.IsKind(SyntaxKind.StringLiteralExpression) && literal.Token.ValueText == "Hello")
                                            {
                                                return true; // Найдена константа уровня класса
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        return false; // Константа не найдена ни локально, ни на уровне класса
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "6. Создайте переменную типа char с именем 'grade' и значением 'A'.",
                    DetailedDescription = "Объявите переменную типа char с именем grade и значением 'A'. Пример: char grade = 'A';",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program" &&
                                           m.Modifiers.Any(SyntaxKind.StaticKeyword));
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var declarationStatement in localDeclarations)
                        {
                            var typeSymbolInfo = semanticModel.GetTypeInfo(declarationStatement.Declaration.Type);
                            if (typeSymbolInfo.Type == null || typeSymbolInfo.Type.SpecialType != SpecialType.System_Char) continue;

                            foreach (var variable in declarationStatement.Declaration.Variables)
                            {
                                if (variable.Identifier.Text == "grade")
                                {
                                    if (variable.Initializer != null && variable.Initializer.Value is LiteralExpressionSyntax literal)
                                    {
                                        if (literal.IsKind(SyntaxKind.CharacterLiteralExpression) && literal.Token.ValueText == "A")
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "7. Объявите переменную типа decimal с именем 'total' и значением 100.5m.",
                    DetailedDescription = "Создайте переменную типа decimal с именем total и значением 100.5m. Пример: decimal total = 100.5m;",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program" &&
                                           m.Modifiers.Any(SyntaxKind.StaticKeyword));
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var declarationStatement in localDeclarations)
                        {
                            var typeSymbolInfo = semanticModel.GetTypeInfo(declarationStatement.Declaration.Type);
                            if (typeSymbolInfo.Type == null || typeSymbolInfo.Type.SpecialType != SpecialType.System_Decimal) continue;

                            foreach (var variable in declarationStatement.Declaration.Variables)
                            {
                                if (variable.Identifier.Text == "total")
                                {
                                    if (variable.Initializer != null && variable.Initializer.Value is LiteralExpressionSyntax literal)
                                    {
                                        // Проверяем, что литерал числовой и имеет суффикс 'm' или 'M'
                                        if (literal.IsKind(SyntaxKind.NumericLiteralExpression) &&
                                            (literal.Token.Text.EndsWith("m") || literal.Token.Text.EndsWith("M")))
                                        {
                                            var constantValue = semanticModel.GetConstantValue(variable.Initializer.Value);
                                            if (constantValue.HasValue && constantValue.Value is decimal decimalValue && decimalValue == 100.5m)
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "8. Создайте переменную типа DateTime с именем 'birthDate' и установите её в текущую дату.",
                    DetailedDescription = "Объявите переменную типа DateTime с именем birthDate и присвойте ей значение DateTime.Now. Пример: DateTime birthDate = DateTime.Now;",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program" &&
                                           m.Modifiers.Any(SyntaxKind.StaticKeyword));
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var declarationStatement in localDeclarations)
                        {
                            var declaredTypeSymbol = semanticModel.GetTypeInfo(declarationStatement.Declaration.Type).Type;
                            if (declaredTypeSymbol == null || declaredTypeSymbol.ToDisplayString() != "System.DateTime") continue;

                            foreach (var variable in declarationStatement.Declaration.Variables)
                            {
                                if (variable.Identifier.Text == "birthDate")
                                {
                                    if (variable.Initializer != null && variable.Initializer.Value is MemberAccessExpressionSyntax memberAccess)
                                    {
                                        // Проверяем, что это DateTime.Now
                                        // memberAccess.Expression должен быть "DateTime"
                                        // memberAccess.Name должен быть "Now"
                                        var symbolInfo = semanticModel.GetSymbolInfo(memberAccess);
                                        if (symbolInfo.Symbol is IPropertySymbol propertySymbol)
                                        {
                                            if (propertySymbol.ContainingType.ToDisplayString() == "System.DateTime" && propertySymbol.Name == "Now")
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "9. Объявите переменную типа object с именем 'obj' и присвойте ей значение null.",
                    DetailedDescription = "Создайте переменную типа object с именем obj и установите её в null. Пример: object obj = null;",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program" &&
                                           m.Modifiers.Any(SyntaxKind.StaticKeyword));
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var declarationStatement in localDeclarations)
                        {
                            var typeSymbolInfo = semanticModel.GetTypeInfo(declarationStatement.Declaration.Type);
                            if (typeSymbolInfo.Type == null || typeSymbolInfo.Type.SpecialType != SpecialType.System_Object) continue;

                            foreach (var variable in declarationStatement.Declaration.Variables)
                            {
                                if (variable.Identifier.Text == "obj")
                                {
                                    if (variable.Initializer != null && variable.Initializer.Value.IsKind(SyntaxKind.NullLiteralExpression))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                                new PracticeTask
                {
                    TaskDescription = "10. Создайте переменную типа int? (nullable int) с именем 'nullableValue' без инициализации.",
                    DetailedDescription = "Объявите nullable переменную типа int с именем nullableValue без присваивания значения. Пример: int? nullableValue;",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program" &&
                                           m.Modifiers.Any(SyntaxKind.StaticKeyword));
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var declarationStatement in localDeclarations)
                        {
                            bool isCorrectNullableIntType = false;
                            var typeSyntaxNode = declarationStatement.Declaration.Type;

                            // Способ 1: Проверка синтаксиса (например, "int?")
                            if (typeSyntaxNode is NullableTypeSyntax nullableSyntax)
                            {
                                if (nullableSyntax.ElementType is PredefinedTypeSyntax predefinedSyntax &&
                                    predefinedSyntax.Keyword.IsKind(SyntaxKind.IntKeyword))
                                {
                                    isCorrectNullableIntType = true;
                                }
                            }
                            // Способ 2: Проверка через семантическую модель (например, тип System.Nullable<System.Int32>)
                            else
                            {
                                ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(typeSyntaxNode).Type;
                                // Проверяем, что typeSymbol не null и является INamedTypeSymbol
                                if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
                                {
                                    // Проверяем, что это Nullable<T> (структура System.Nullable`1)
                                    // и что его аргумент типа - это int (System.Int32)
                                    if (namedTypeSymbol.IsValueType && // Nullable<T> - это структура
                                        namedTypeSymbol.OriginalDefinition.Equals(compilation.GetTypeByMetadataName("System.Nullable`1"), SymbolEqualityComparer.Default) &&
                                        namedTypeSymbol.TypeArguments.Length == 1)
                                    {
                                        ITypeSymbol typeArgument = namedTypeSymbol.TypeArguments.FirstOrDefault();
                                        if (typeArgument != null && typeArgument.SpecialType == SpecialType.System_Int32)
                                        {
                                            isCorrectNullableIntType = true;
                                        }
                                    }
                                }
                            }

                            if (isCorrectNullableIntType)
                            {
                                foreach (var variable in declarationStatement.Declaration.Variables)
                                {
                                    if (variable.Identifier.Text == "nullableValue")
                                    {
                                        // Проверяем, что нет инициализатора
                                        if (variable.Initializer == null)
                                        {
                                            return true; // Найдено: int? nullableValue; (или System.Nullable<int> nullableValue;)
                                        }
                                    }
                                }
                            }
                        }
                        return false; // Если ничего не найдено или условия не выполнены
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
                    TaskDescription = "1. Напишите цикл for, который выводит числа от 1 до 10.",
                    DetailedDescription = "Создайте цикл for, который выводит числа от 1 до 10 в консоль. Пример:\nfor(int i = 1; i <= 10; i++)\n{\n    Console.WriteLine(i);\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var forStatements = mainMethodNode.Body.DescendantNodes().OfType<ForStatementSyntax>();
                        foreach (var forStatement in forStatements)
                        {
                            // Проверяем инициализатор (int i = 1)
                            bool initializerCorrect = false;
                            if (forStatement.Declaration != null && forStatement.Declaration.Variables.Count == 1)
                            {
                                var variable = forStatement.Declaration.Variables.First();
                                if (variable.Identifier.Text == "i" &&
                                    variable.Initializer != null &&
                                    variable.Initializer.Value is LiteralExpressionSyntax initLiteral &&
                                    initLiteral.Token.Value is int initVal && initVal == 1)
                                {
                                    initializerCorrect = true;
                                }
                            }

                            // Проверяем условие (i <= 10)
                            bool conditionCorrect = false;
                            if (forStatement.Condition is BinaryExpressionSyntax conditionExpr &&
                                conditionExpr.IsKind(SyntaxKind.LessThanOrEqualExpression))
                            {
                                if (conditionExpr.Left is IdentifierNameSyntax idName && idName.Identifier.Text == "i" &&
                                    conditionExpr.Right is LiteralExpressionSyntax condLiteral &&
                                    condLiteral.Token.Value is int condVal && condVal == 10)
                                {
                                    conditionCorrect = true;
                                }
                            }
                            
                            // Проверяем инкрементор (i++)
                            bool incrementorCorrect = false;
                            if (forStatement.Incrementors.Count == 1 &&
                                forStatement.Incrementors.First() is PostfixUnaryExpressionSyntax incrementorExpr &&
                                incrementorExpr.IsKind(SyntaxKind.PostIncrementExpression) &&
                                incrementorExpr.Operand is IdentifierNameSyntax incId && incId.Identifier.Text == "i")
                            {
                                incrementorCorrect = true;
                            }

                            // Проверяем тело цикла (Console.WriteLine(i))
                            bool bodyCorrect = false;
                            if (forStatement.Statement is BlockSyntax block)
                            {
                                var consoleWrite = block.DescendantNodes()
                                    .OfType<InvocationExpressionSyntax>()
                                    .FirstOrDefault(inv => inv.Expression.ToString() == "Console.WriteLine" &&
                                                           inv.ArgumentList.Arguments.Count == 1 &&
                                                           inv.ArgumentList.Arguments.First().Expression.ToString() == "i");
                                if (consoleWrite != null)
                                {
                                    bodyCorrect = true;
                                }
                            }


                            if (initializerCorrect && conditionCorrect && incrementorCorrect && bodyCorrect)
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "2. Напишите условие if, проверяющее, что число больше 100.",
                    DetailedDescription = "Создайте условие if, которое проверяет, что переменная number больше 100. Пример:\nint number = 120;\nif(number > 100)\n{\n    Console.WriteLine(\"Число больше 100\");\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // Ищем объявление 'int number'
                        string variableName = "number"; // Имя переменной может быть другим, но условие должно быть на ней
                        bool numberDeclared = mainMethodNode.Body.DescendantNodes()
                            .OfType<LocalDeclarationStatementSyntax>()
                            .Any(lds => lds.Declaration.Variables.Any(v => v.Identifier.Text == variableName &&
                                     semanticModel.GetTypeInfo(lds.Declaration.Type).Type?.SpecialType == SpecialType.System_Int32));
                        
                        // Если 'number' не объявлен как int, можно искать любое объявление int и проверять if на нем
                        // Для упрощения, будем искать if, который оперирует переменной с именем "number" или похожим

                        var ifStatements = mainMethodNode.Body.DescendantNodes().OfType<IfStatementSyntax>();
                        foreach (var ifStatement in ifStatements)
                        {
                            if (ifStatement.Condition is BinaryExpressionSyntax conditionExpr &&
                                conditionExpr.IsKind(SyntaxKind.GreaterThanExpression))
                            {
                                // Проверяем, что левая часть - это идентификатор (наша переменная)
                                // и правая - число 100
                                IdentifierNameSyntax idName = null;
                                LiteralExpressionSyntax literal = null;

                                if (conditionExpr.Left is IdentifierNameSyntax lId) idName = lId;
                                else if (conditionExpr.Right is IdentifierNameSyntax rId) idName = rId; // на случай 100 < number

                                if (conditionExpr.Right is LiteralExpressionSyntax rLit) literal = rLit;
                                else if (conditionExpr.Left is LiteralExpressionSyntax lLit) literal = lLit; // на случай 100 < number

                                if (idName != null && literal != null && literal.Token.Value is int val && val == 100)
                                {
                                    // Дополнительно можно проверить, что переменная "number" из условия - это int
                                    var symbolInfo = semanticModel.GetSymbolInfo(idName);
                                    if (symbolInfo.Symbol is ILocalSymbol localSymbol && localSymbol.Type.SpecialType == SpecialType.System_Int32)
                                    {
                                         return true;
                                    }
                                    // Если это поле класса
                                    else if (symbolInfo.Symbol is IFieldSymbol fieldSymbol && fieldSymbol.Type.SpecialType == SpecialType.System_Int32)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "3. Реализуйте switch-case для дней недели (1-7).",
                    DetailedDescription = "Создайте switch-case конструкцию, которая по номеру дня недели (1-7) выводит его название. Пример:\nint day = 3;\nswitch(day)\n{\n    case 1: Console.WriteLine(\"Понедельник\"); break;\n    // и т.д.\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var switchStatements = mainMethodNode.Body.DescendantNodes().OfType<SwitchStatementSyntax>();
                        foreach (var switchStatement in switchStatements)
                        {
                            // Проверяем, что switch работает с переменной (Expression не null)
                            if (switchStatement.Expression == null) continue;

                            // Ищем как минимум несколько case (например, 3 для базовой проверки)
                            // и хотя бы один default или 7 case-ов.
                            int caseCount = switchStatement.Sections.Count(s => s.Labels.Any(l => l.IsKind(SyntaxKind.CaseSwitchLabel)));
                            bool hasDefault = switchStatement.Sections.Any(s => s.Labels.Any(l => l.IsKind(SyntaxKind.DefaultSwitchLabel)));

                            if (caseCount >= 3) // хотя бы 3 дня недели
                            {
                                // Более строгая проверка: ищем case 1, 2, 3 ... 7
                                var specificCasesFound = new HashSet<int>();
                                foreach(var section in switchStatement.Sections)
                                {
                                    foreach(var label in section.Labels.OfType<CaseSwitchLabelSyntax>())
                                    {
                                        if(label.Value is LiteralExpressionSyntax lit && lit.Token.Value is int dayNum)
                                        {
                                            if(dayNum >=1 && dayNum <=7) specificCasesFound.Add(dayNum);
                                        }
                                    }
                                }
                                if (specificCasesFound.Count >= 5) // Если хотя бы 5 из 7 дней покрыты, считаем ок
                                {
                                    // Проверяем наличие Console.WriteLine в хотя бы одном case
                                    bool hasConsoleWrite = switchStatement.Sections
                                        .Any(s => s.Statements.OfType<ExpressionStatementSyntax>()
                                        .Any(es => es.Expression is InvocationExpressionSyntax inv && inv.Expression.ToString().Contains("Console.WriteLine")));

                                    if(hasConsoleWrite) return true;
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "4. Напишите цикл while, который удваивает число, пока оно меньше 1000.",
                    DetailedDescription = "Создайте цикл while, который начинается с числа 1 и удваивает его на каждой итерации, пока значение не станет больше или равно 1000. Пример:\nint num = 1;\nwhile(num < 1000)\n{\n    num *= 2;\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var whileStatements = mainMethodNode.Body.DescendantNodes().OfType<WhileStatementSyntax>();
                        foreach (var whileStatement in whileStatements)
                        {
                            // Проверяем условие: <переменная> < 1000
                            string controlVarName = null;
                            if (whileStatement.Condition is BinaryExpressionSyntax conditionExpr &&
                                conditionExpr.IsKind(SyntaxKind.LessThanExpression))
                            {
                                if (conditionExpr.Left is IdentifierNameSyntax idName &&
                                    conditionExpr.Right is LiteralExpressionSyntax lit &&
                                    lit.Token.Value is int val && val == 1000)
                                {
                                    controlVarName = idName.Identifier.Text;
                                }
                            }
                            if (controlVarName == null) continue;

                            // Проверяем, что переменная controlVarName объявлена как int и инициализирована (например, 1)
                            // Это сложнее, т.к. объявление может быть до цикла.
                            // Пока упростим: ищем внутри цикла num *= 2 или num = num * 2

                            bool multiplicationCorrect = false;
                            if (whileStatement.Statement is BlockSyntax block)
                            {
                                foreach(var stmt in block.Statements)
                                {
                                    if (stmt is ExpressionStatementSyntax exprStmt)
                                    {
                                        // num *= 2
                                        if (exprStmt.Expression is AssignmentExpressionSyntax assignExpr &&
                                            assignExpr.IsKind(SyntaxKind.MultiplyAssignmentExpression) &&
                                            assignExpr.Left is IdentifierNameSyntax leftId && leftId.Identifier.Text == controlVarName &&
                                            assignExpr.Right is LiteralExpressionSyntax rightLit &&
                                            rightLit.Token.Value is int multVal && multVal == 2)
                                        {
                                            multiplicationCorrect = true;
                                            break;
                                        }
                                        // num = num * 2
                                        if (exprStmt.Expression is AssignmentExpressionSyntax assignExprSimple &&
                                            assignExprSimple.IsKind(SyntaxKind.SimpleAssignmentExpression) &&
                                            assignExprSimple.Left is IdentifierNameSyntax leftIdSimple && leftIdSimple.Identifier.Text == controlVarName &&
                                            assignExprSimple.Right is BinaryExpressionSyntax binExpr && binExpr.IsKind(SyntaxKind.MultiplyExpression) &&
                                            binExpr.Left is IdentifierNameSyntax innerLeftId && innerLeftId.Identifier.Text == controlVarName &&
                                            binExpr.Right is LiteralExpressionSyntax innerRightLit &&
                                            innerRightLit.Token.Value is int innerMultVal && innerMultVal == 2)
                                        {
                                             multiplicationCorrect = true;
                                             break;
                                        }
                                    }
                                }
                            }

                            if (multiplicationCorrect) return true;
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "5. Используйте тернарный оператор для проверки возраста.",
                    DetailedDescription = "Создайте переменную age и с помощью тернарного оператора определите, совершеннолетний ли человек (age >= 18). Пример:\nint age = 20;\nstring status = age >= 18 ? \"Совершеннолетний\" : \"Несовершеннолетний\";",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var conditionalExpressions = mainMethodNode.Body.DescendantNodes().OfType<ConditionalExpressionSyntax>();
                        foreach (var condExpr in conditionalExpressions)
                        {
                            // Условие: <переменная> >= 18
                            if (condExpr.Condition is BinaryExpressionSyntax condition &&
                                condition.IsKind(SyntaxKind.GreaterThanOrEqualExpression))
                            {
                                IdentifierNameSyntax ageVarSyntax = null;
                                LiteralExpressionSyntax literal18 = null;

                                if (condition.Left is IdentifierNameSyntax idLeft) ageVarSyntax = idLeft;
                                else if (condition.Right is IdentifierNameSyntax idRight) ageVarSyntax = idRight; // на случай 18 <= age

                                if (condition.Right is LiteralExpressionSyntax litRight) literal18 = litRight;
                                else if (condition.Left is LiteralExpressionSyntax litLeft) literal18 = litLeft; // на случай 18 <= age
                                
                                if (ageVarSyntax != null && literal18 != null &&
                                    literal18.Token.Value is int val && val == 18)
                                {
                                     // Проверяем, что переменная 'ageVarSyntax' - это int
                                    var ageSymbol = semanticModel.GetSymbolInfo(ageVarSyntax).Symbol;
                                    bool ageIsInt = false;
                                    if (ageSymbol is ILocalSymbol localAge && localAge.Type.SpecialType == SpecialType.System_Int32) ageIsInt = true;
                                    if (ageSymbol is IFieldSymbol fieldAge && fieldAge.Type.SpecialType == SpecialType.System_Int32) ageIsInt = true;
                                    if (ageSymbol is IParameterSymbol paramAge && paramAge.Type.SpecialType == SpecialType.System_Int32) ageIsInt = true;


                                    // Проверяем, что ветки WhenTrue и WhenFalse - это строки
                                    bool truePartIsString = semanticModel.GetTypeInfo(condExpr.WhenTrue).Type?.SpecialType == SpecialType.System_String;
                                    bool falsePartIsString = semanticModel.GetTypeInfo(condExpr.WhenFalse).Type?.SpecialType == SpecialType.System_String;

                                    if (ageIsInt && truePartIsString && falsePartIsString)
                                    {
                                        // Можно проверить, что тернарный оператор присваивается строковой переменной
                                        var parentAssignment = condExpr.Parent.Parent; // ConditionalExpression -> EqualsValueClause -> VariableDeclarator
                                        if (parentAssignment is VariableDeclaratorSyntax vd &&
                                            vd.Parent is VariableDeclarationSyntax vds)
                                        {
                                            var assignedVarType = semanticModel.GetTypeInfo(vds.Type).Type;
                                            if (assignedVarType?.SpecialType == SpecialType.System_String)
                                            {
                                                return true;
                                            }
                                        }
                                        // Или если тернарный оператор используется в другом контексте, где ожидается строка (например, Console.WriteLine)
                                        else if (condExpr.Parent is ArgumentSyntax)
                                        {
                                            return true; // Достаточно гибко
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "6. Напишите цикл do-while для ввода числа от пользователя.",
                    DetailedDescription = "Создайте цикл do-while, который запрашивает у пользователя число, пока не будет введено положительное число. Пример:\nint number;\ndo\n{\n    Console.Write(\"Введите положительное число: \");\n    number = int.Parse(Console.ReadLine());\n} while(number <= 0);",
                    ValidationMethod = (assembly) => // Валидация ввода от пользователя сложна без запуска. Проверяем структуру.
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var doStatements = mainMethodNode.Body.DescendantNodes().OfType<DoStatementSyntax>();
                        foreach (var doStatement in doStatements)
                        {
                            // Проверяем условие: <переменная> <= 0
                            string controlVarName = null;
                            if (doStatement.Condition is BinaryExpressionSyntax conditionExpr &&
                                conditionExpr.IsKind(SyntaxKind.LessThanOrEqualExpression))
                            {
                                if (conditionExpr.Left is IdentifierNameSyntax idName &&
                                    conditionExpr.Right is LiteralExpressionSyntax lit &&
                                    lit.Token.Value is int val && val == 0)
                                {
                                    controlVarName = idName.Identifier.Text;
                                }
                            }
                            if (controlVarName == null) continue;

                            // Проверяем тело: Console.Write, Console.ReadLine, int.Parse, присвоение controlVarName
                            bool consoleWriteFound = false;
                            bool consoleReadAndParseFound = false;

                            if (doStatement.Statement is BlockSyntax block)
                            {
                                foreach(var stmt in block.Statements)
                                {
                                    if (stmt is ExpressionStatementSyntax exprStmt)
                                    {
                                        if (exprStmt.Expression is InvocationExpressionSyntax invExpr)
                                        {
                                            if (invExpr.Expression.ToString().Contains("Console.Write")) consoleWriteFound = true;
                                            
                                            // int.Parse(Console.ReadLine())
                                            if (invExpr.Expression.ToString() == "int.Parse" &&
                                                invExpr.ArgumentList.Arguments.Count == 1 &&
                                                invExpr.ArgumentList.Arguments.First().Expression is InvocationExpressionSyntax readLineInv &&
                                                readLineInv.Expression.ToString() == "Console.ReadLine")
                                            {
                                                // Проверяем, что результат присваивается controlVarName
                                                var parentAssignment = invExpr.Parent;
                                                if (parentAssignment is EqualsValueClauseSyntax eqVal &&
                                                    eqVal.Parent is VariableDeclaratorSyntax varDec &&
                                                    varDec.Identifier.Text == controlVarName)
                                                {
                                                    consoleReadAndParseFound = true;
                                                }
                                                else if (parentAssignment is AssignmentExpressionSyntax assign &&
                                                         assign.Left is IdentifierNameSyntax assignId &&
                                                         assignId.Identifier.Text == controlVarName)
                                                {
                                                    consoleReadAndParseFound = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (consoleWriteFound && consoleReadAndParseFound) return true;
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "7. Используйте if-else if-else для определения времени суток.",
                    DetailedDescription = "Создайте условие, которое по текущему часу (DateTime.Now.Hour) определяет время суток (утро, день, вечер, ночь). Пример:\nint hour = DateTime.Now.Hour;\nif(hour >= 6 && hour < 12)\n{\n    Console.WriteLine(\"Утро\");\n}\nelse if(hour >= 12 && hour < 18)\n{\n    // и т.д.",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // Ищем объявление int hour = DateTime.Now.Hour;
                        bool hourVarInitializedCorrectly = false;
                        string hourVarName = null;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach(var ld in localDeclarations)
                        {
                            if (ld.Declaration.Type is PredefinedTypeSyntax pts && pts.Keyword.IsKind(SyntaxKind.IntKeyword) &&
                                ld.Declaration.Variables.Count == 1)
                            {
                                var variable = ld.Declaration.Variables.First();
                                if (variable.Initializer != null &&
                                    variable.Initializer.Value is MemberAccessExpressionSyntax maes)
                                {
                                    // DateTime.Now.Hour
                                    if (maes.Name.Identifier.Text == "Hour" &&
                                        maes.Expression is MemberAccessExpressionSyntax nowMaes &&
                                        nowMaes.Name.Identifier.Text == "Now" &&
                                        nowMaes.Expression is IdentifierNameSyntax dtId && dtId.Identifier.Text == "DateTime")
                                        {
                                            hourVarInitializedCorrectly = true;
                                            hourVarName = variable.Identifier.Text;
                                            break;
                                        }
                                }
                            }
                        }
                        if (!hourVarInitializedCorrectly || hourVarName == null) return false;

                        var ifStatements = mainMethodNode.Body.DescendantNodes().OfType<IfStatementSyntax>();
                        foreach (var ifStatement in ifStatements)
                        {
                            int elseIfCount = 0;
                            var currentIf = ifStatement;
                            bool usesHourVar = false;

                            // Проверяем первое условие
                            if (currentIf.Condition.ToString().Contains(hourVarName)) usesHourVar = true;

                            while(currentIf.Else != null && currentIf.Else.Statement is IfStatementSyntax elseIf)
                            {
                                elseIfCount++;
                                if (elseIf.Condition.ToString().Contains(hourVarName)) usesHourVar = true;
                                currentIf = elseIf;
                            }
                            // Проверяем последний else (если есть)
                            bool hasFinalElse = currentIf.Else != null;

                            // Ожидаем if, как минимум один else if, и опционально else (или больше else if)
                            // Например, Утро (if), День (else if), Вечер (else if), Ночь (else) -> 2 else if + final else
                            if (usesHourVar && elseIfCount >= 1 && (elseIfCount >=2 || hasFinalElse))
                            {
                                // Проверяем, что в телах есть Console.WriteLine
                                bool consoleWritesPresent = true; // Assume true, prove false
                                if (!(ifStatement.Statement.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().Any(inv => inv.Expression.ToString().Contains("Console.WriteLine"))))
                                    consoleWritesPresent = false;

                                currentIf = ifStatement; // Reset
                                while(currentIf.Else != null && currentIf.Else.Statement is IfStatementSyntax elseIfNext)
                                {
                                    if (!(elseIfNext.Statement.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().Any(inv => inv.Expression.ToString().Contains("Console.WriteLine"))))
                                        consoleWritesPresent = false;
                                    currentIf = elseIfNext;
                                }
                                if (currentIf.Else != null && currentIf.Else.Statement != null)
                                {
                                     if (!(currentIf.Else.Statement.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().Any(inv => inv.Expression.ToString().Contains("Console.WriteLine"))))
                                        consoleWritesPresent = false;
                                }


                                if (consoleWritesPresent) return true;
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "8. Напишите цикл for с вложенным if для поиска простых чисел.",
                    DetailedDescription = "Напишите цикл for от 2 до 100 с вложенным условием if для проверки, является ли число простым. Пример:\nfor(int i = 2; i <= 100; i++)\n{\n    bool isPrime = true;\n    for(int j = 2; j < i; j++)\n    {\n        if(i % j == 0)\n        {\n            isPrime = false;\n            break;\n        }\n    }\n    if(isPrime) Console.WriteLine(i);\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var outerForStatements = mainMethodNode.Body.DescendantNodes().OfType<ForStatementSyntax>();
                        foreach (var outerFor in outerForStatements)
                        {
                            // Внешний цикл (i от 2 до 100)
                            bool outerLoopCorrect = outerFor.Declaration != null &&
                                                   outerFor.Declaration.Variables.Any(v => v.Identifier.Text == "i" && v.Initializer.Value.ToString() == "2") &&
                                                   outerFor.Condition.ToString().Contains("i <= 100") &&
                                                   outerFor.Incrementors.Any(inc => inc.ToString() == "i++");
                            if (!outerLoopCorrect) continue;

                            // Внутри внешнего цикла: объявление bool isPrime = true;
                            bool isPrimeDeclared = outerFor.Statement.DescendantNodes()
                                .OfType<LocalDeclarationStatementSyntax>()
                                .Any(lds => lds.Declaration.Variables.Any(v => v.Identifier.Text == "isPrime" &&
                                                                          v.Initializer.Value.IsKind(SyntaxKind.TrueLiteralExpression)));
                            if (!isPrimeDeclared) continue;

                            // Вложенный цикл for (j от 2 до i-1)
                            var innerFor = outerFor.Statement.DescendantNodes().OfType<ForStatementSyntax>().FirstOrDefault();
                            if (innerFor == null) continue;

                            bool innerLoopCorrect = innerFor.Declaration != null &&
                                                   innerFor.Declaration.Variables.Any(v => v.Identifier.Text == "j" && v.Initializer.Value.ToString() == "2") &&
                                                   innerFor.Condition.ToString().Replace(" ", "").Contains("j<i") && // Убираем пробелы для гибкости
                                                   innerFor.Incrementors.Any(inc => inc.ToString() == "j++");
                            if (!innerLoopCorrect) continue;

                            // Внутри вложенного цикла: if (i % j == 0) { isPrime = false; break; }
                            var innerIf = innerFor.Statement.DescendantNodes().OfType<IfStatementSyntax>().FirstOrDefault();
                            if (innerIf == null) continue;

                            bool innerIfCorrect = innerIf.Condition.ToString().Replace(" ", "") == "i%j==0";
                            bool setsIsPrimeFalse = innerIf.Statement.DescendantNodes()
                                .OfType<AssignmentExpressionSyntax>()
                                .Any(a => a.Left.ToString() == "isPrime" && a.Right.IsKind(SyntaxKind.FalseLiteralExpression));
                            bool hasBreak = innerIf.Statement.DescendantNodes().OfType<BreakStatementSyntax>().Any();

                            if (!(innerIfCorrect && setsIsPrimeFalse && hasBreak)) continue;

                            // После вложенного цикла: if(isPrime) Console.WriteLine(i);
                            var outerIf = outerFor.Statement.DescendantNodes()
                                .OfType<IfStatementSyntax>()
                                .FirstOrDefault(ifs => ifs.Condition.ToString() == "isPrime" &&
                                                 !innerFor.Contains(ifs)); // Убедимся, что это if после внутреннего цикла

                            if (outerIf != null)
                            {
                                bool consoleWriteCorrect = outerIf.Statement.DescendantNodesAndSelf()
                                    .OfType<InvocationExpressionSyntax>()
                                    .Any(inv => inv.Expression.ToString() == "Console.WriteLine" &&
                                                inv.ArgumentList.Arguments.ToString() == "i");
                                if (consoleWriteCorrect) return true;
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "9. Используйте switch с pattern matching для проверки типа переменной.",
                    DetailedDescription = "Создайте switch с pattern matching, который проверяет тип переменной object и выводит соответствующее сообщение. Пример:\nobject obj = \"текст\";\nswitch(obj)\n{\n    case int i: Console.WriteLine(\"Это целое число\"); break;\n    case string s: Console.WriteLine(\"Это строка\"); break;\n    // и т.д.",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // Ищем объявление object obj;
                        bool objDeclared = mainMethodNode.Body.DescendantNodes()
                            .OfType<LocalDeclarationStatementSyntax>()
                            .Any(lds => lds.Declaration.Variables.Any(v => v.Identifier.Text == "obj") &&
                                        semanticModel.GetTypeInfo(lds.Declaration.Type).Type?.SpecialType == SpecialType.System_Object);
                        if (!objDeclared) return false;


                        var switchStatements = mainMethodNode.Body.DescendantNodes().OfType<SwitchStatementSyntax>();
                        foreach (var switchStatement in switchStatements)
                        {
                            // Проверяем, что switch работает с переменной "obj"
                            if (switchStatement.Expression.ToString() != "obj") continue;

                            int patternCaseCount = 0;
                            bool hasIntPattern = false;
                            bool hasStringPattern = false;

                            foreach(var section in switchStatement.Sections)
                            {
                                foreach(var label in section.Labels.OfType<CasePatternSwitchLabelSyntax>())
                                {
                                    patternCaseCount++;
                                    if (label.Pattern is DeclarationPatternSyntax dp)
                                    {
                                        var typeName = dp.Type.ToString();
                                        if (typeName == "int") hasIntPattern = true;
                                        if (typeName == "string") hasStringPattern = true;
                                    }
                                    // Можно добавить другие типы паттернов, если нужно
                                }
                            }
                            
                            // Ожидаем как минимум два case с паттернами, включая int и string
                            if (patternCaseCount >= 2 && hasIntPattern && hasStringPattern)
                            {
                                // Проверяем наличие Console.WriteLine в телах case
                                bool consoleWritesPresent = switchStatement.Sections
                                    .Where(s => s.Labels.Any(l => l is CasePatternSwitchLabelSyntax))
                                    .All(s => s.Statements.OfType<ExpressionStatementSyntax>()
                                        .Any(es => es.Expression is InvocationExpressionSyntax inv &&
                                                   inv.Expression.ToString().Contains("Console.WriteLine")));
                                if (consoleWritesPresent) return true;
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "10. Напишите бесконечный цикл for с условием выхода.",
                    DetailedDescription = "Создайте бесконечный цикл for с условием выхода при достижении определенного значения. Пример:\nint counter = 0;\nfor(;;)\n{\n    counter++;\n    if(counter >= 10) break;\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var forStatements = mainMethodNode.Body.DescendantNodes().OfType<ForStatementSyntax>();
                        foreach (var forStatement in forStatements)
                        {
                            // Проверка на бесконечный цикл: нет Declaration, Condition, Incrementors
                            bool isInfiniteLoop = forStatement.Declaration == null &&
                                                  forStatement.Condition == null &&
                                                  forStatement.Incrementors.Count == 0;

                            if (!isInfiniteLoop)
                            {
                                // Альтернативный вариант "бесконечного" цикла: for(;true;)
                                isInfiniteLoop = forStatement.Declaration == null &&
                                                 forStatement.Condition is LiteralExpressionSyntax litCond &&
                                                 litCond.IsKind(SyntaxKind.TrueLiteralExpression) &&
                                                 forStatement.Incrementors.Count == 0;
                            }

                            if (!isInfiniteLoop) continue;

                            // Внутри цикла: есть if с break
                            if (forStatement.Statement is BlockSyntax block)
                            {
                                var ifStatementsInLoop = block.DescendantNodes().OfType<IfStatementSyntax>();
                                foreach (var ifInLoop in ifStatementsInLoop)
                                {
                                    // Проверяем, что if содержит break
                                    bool hasBreak = ifInLoop.Statement.DescendantNodesAndSelf().OfType<BreakStatementSyntax>().Any();
                                    if (hasBreak)
                                    {
                                        // Проверяем, что условие if не является просто "true" или "false" (т.е. есть реальное условие)
                                        if (!(ifInLoop.Condition is LiteralExpressionSyntax lit && (lit.IsKind(SyntaxKind.TrueLiteralExpression) || lit.IsKind(SyntaxKind.FalseLiteralExpression))))
                                        {
                                             // Дополнительно: ищем инкремент какой-либо переменной перед if
                                             bool counterIncremented = false;
                                             var statementsBeforeIf = block.Statements.TakeWhile(s => s != ifInLoop);
                                             foreach(var stmtBefore in statementsBeforeIf)
                                             {
                                                 if (stmtBefore.DescendantNodesAndSelf().OfType<PostfixUnaryExpressionSyntax>().Any(pue => pue.IsKind(SyntaxKind.PostIncrementExpression) || pue.IsKind(SyntaxKind.PreIncrementExpression)) ||
                                                     stmtBefore.DescendantNodesAndSelf().OfType<AssignmentExpressionSyntax>().Any(ae => ae.IsKind(SyntaxKind.AddAssignmentExpression))) // counter++ или counter += 1
                                                     {
                                                         counterIncremented = true;
                                                         break;
                                                     }
                                             }
                                             // Если есть инкремент (не обязательно), и есть if c break и нетривиальным условием - засчитываем
                                             // Для более строгой проверки можно искать переменную "counter" из примера.
                                             return true;
                                        }
                                    }
                                }
                            }
                        }
                        return false;
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
                new PracticeTask
                {
                    TaskDescription = "1. Создайте массив из 5 целых чисел и выведите его элементы.",
                    DetailedDescription = "Объявите массив из 5 целых чисел, инициализируйте его и выведите все элементы в консоль. Пример:\nint[] numbers = {1, 2, 3, 4, 5};\nforeach(int num in numbers)\n{\n    Console.WriteLine(num);\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // Ищем объявление массива int[]
                        VariableDeclaratorSyntax arrayVariable = null;
                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var ld in localDeclarations)
                        {
                            if (ld.Declaration.Type is ArrayTypeSyntax ats &&
                                ats.ElementType is PredefinedTypeSyntax pts && pts.Keyword.IsKind(SyntaxKind.IntKeyword))
                            {
                                if (ld.Declaration.Variables.Count == 1)
                                {
                                    arrayVariable = ld.Declaration.Variables.First();
                                    // Проверяем инициализацию с 5 элементами
                                    if (arrayVariable.Initializer?.Value is InitializerExpressionSyntax ies &&
                                        ies.IsKind(SyntaxKind.ArrayInitializerExpression) &&
                                        ies.Expressions.Count >= 3) // хотя бы 3, для гибкости, но по заданию 5
                                    {
                                         // Можно еще проверить, что все элементы - LiteralExpressionSyntax (числа)
                                    }
                                    else if (arrayVariable.Initializer?.Value is ArrayCreationExpressionSyntax aces &&
                                             aces.Type.ElementType is PredefinedTypeSyntax pts2 && pts2.Keyword.IsKind(SyntaxKind.IntKeyword) &&
                                             aces.Type.RankSpecifiers.Any(rs => rs.Sizes.Count == 1 && rs.Sizes.First().ToString().Contains("5")))
                                    {
                                        // new int[5]; - менее вероятно по примеру, но возможно
                                    }
                                    else
                                    {
                                        arrayVariable = null; // Не подходит
                                        continue;
                                    }
                                    break;
                                }
                            }
                        }
                        if (arrayVariable == null) return false;
                        string arrayName = arrayVariable.Identifier.Text;

                        // Ищем foreach или for для вывода элементов
                        bool outputLoopFound = false;
                        var foreachStatements = mainMethodNode.Body.DescendantNodes().OfType<ForEachStatementSyntax>();
                        foreach (var feStmt in foreachStatements)
                        {
                            if (feStmt.Expression.ToString() == arrayName)
                            {
                                if (feStmt.Statement.DescendantNodesAndSelf()
                                    .OfType<InvocationExpressionSyntax>()
                                    .Any(inv => inv.Expression.ToString() == "Console.WriteLine" &&
                                                inv.ArgumentList.Arguments.ToString() == feStmt.Identifier.Text))
                                {
                                    outputLoopFound = true;
                                    break;
                                }
                            }
                        }

                        if (!outputLoopFound) // Если foreach не найден, ищем for
                        {
                            var forStatements = mainMethodNode.Body.DescendantNodes().OfType<ForStatementSyntax>();
                            foreach(var forStmt in forStatements)
                            {
                                // Простой for, проверяющий Console.WriteLine(arrayName[...])
                                if (forStmt.Statement.DescendantNodesAndSelf()
                                    .OfType<InvocationExpressionSyntax>()
                                    .Any(inv => inv.Expression.ToString() == "Console.WriteLine" &&
                                                inv.ArgumentList.Arguments.ToString().StartsWith(arrayName + "[")))
                                {
                                    outputLoopFound = true;
                                    break;
                                }
                            }
                        }

                        return outputLoopFound;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "2. Создайте список строк с названиями трёх городов.",
                    DetailedDescription = "Создайте List<string> с названиями трёх городов. Пример:\nList<string> cities = new List<string> {\"Москва\", \"Санкт-Петербург\", \"Новосибирск\"};",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var ld in localDeclarations)
                        {
                            // Проверяем тип: List<string>
                            if (ld.Declaration.Type is GenericNameSyntax gns && gns.Identifier.Text == "List")
                            {
                                if (gns.TypeArgumentList.Arguments.Count == 1 &&
                                    gns.TypeArgumentList.Arguments.First() is PredefinedTypeSyntax pts &&
                                    pts.Keyword.IsKind(SyntaxKind.StringKeyword))
                                {
                                    // Проверяем инициализатор (коллекция или new List<string>())
                                    var variable = ld.Declaration.Variables.FirstOrDefault();
                                    if (variable?.Initializer?.Value is ObjectCreationExpressionSyntax oces) // new List<string>()
                                    {
                                        // Если инициализатор содержит InitializerExpressionSyntax (инициализатор коллекции)
                                        if (oces.Initializer != null && oces.Initializer.Expressions.Count >= 2) // хотя бы 2 города
                                        {
                                            bool allStrings = oces.Initializer.Expressions.All(expr =>
                                                expr is LiteralExpressionSyntax lit && lit.IsKind(SyntaxKind.StringLiteralExpression));
                                            if (allStrings) return true;
                                        }
                                        // Если просто new List<string>() и затем Add
                                        else if (oces.ArgumentList == null || oces.ArgumentList.Arguments.Count == 0)
                                        {
                                            // Нужно будет искать последующие вызовы .Add()
                                            // Для упрощения пока хватит инициализатора коллекции.
                                            // Можно расширить, если этот кейс не проходит.
                                        }
                                    }
                                    // Может быть var cities = new List<string> { ... }
                                    // semanticModel поможет определить тип 'var'
                                    else if (variable?.Initializer?.Value is InitializerExpressionSyntax ies &&
                                             semanticModel.GetTypeInfo(ld.Declaration.Type).ConvertedType?.ToDisplayString() == "System.Collections.Generic.List<string>")
                                    {
                                        if(ies.Expressions.Count >= 2 && ies.Expressions.All(expr => expr is LiteralExpressionSyntax lit && lit.IsKind(SyntaxKind.StringLiteralExpression)))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            // Альтернативная проверка, если тип написан полностью System.Collections.Generic.List<string>
                            else if (ld.Declaration.Type.ToString().Contains("List<string>") || ld.Declaration.Type.ToString().Contains("List<System.String>"))
                            {
                                 var typeSymbol = semanticModel.GetTypeInfo(ld.Declaration.Type).Type;
                                 if (typeSymbol?.ToDisplayString() == "System.Collections.Generic.List<string>")
                                 {
                                     var variable = ld.Declaration.Variables.FirstOrDefault();
                                     if (variable?.Initializer?.Value is ObjectCreationExpressionSyntax oces)
                                     {
                                         if (oces.Initializer != null && oces.Initializer.Expressions.Count >= 2 &&
                                             oces.Initializer.Expressions.All(expr => expr is LiteralExpressionSyntax lit && lit.IsKind(SyntaxKind.StringLiteralExpression)))
                                         {
                                             return true;
                                         }
                                     }
                                 }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "3. Добавьте элемент в список чисел и удалите первый элемент.",
                    DetailedDescription = "Создайте список чисел, добавьте новый элемент и удалите первый элемент. Пример:\nList<int> numbers = new List<int> {1, 2, 3};\nnumbers.Add(4);\nnumbers.RemoveAt(0);",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        string listName = null;
                        // Ищем объявление List<int>
                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var ld in localDeclarations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(ld.Declaration.Type).Type;
                            if (typeSymbol?.ToDisplayString() == "System.Collections.Generic.List<int>")
                            {
                                listName = ld.Declaration.Variables.FirstOrDefault()?.Identifier.Text;
                                break;
                            }
                        }
                        if (listName == null) return false;

                        bool addCalled = false;
                        bool removeAtZeroCalled = false;

                        var invocations = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>();
                        foreach (var inv in invocations)
                        {
                            if (inv.Expression is MemberAccessExpressionSyntax maes)
                            {
                                // numbers.Add(...)
                                if (maes.Expression.ToString() == listName && maes.Name.Identifier.Text == "Add")
                                {
                                    if (inv.ArgumentList.Arguments.Count == 1 &&
                                        semanticModel.GetTypeInfo(inv.ArgumentList.Arguments.First().Expression).ConvertedType?.SpecialType == SpecialType.System_Int32)
                                    {
                                        addCalled = true;
                                    }
                                }
                                // numbers.RemoveAt(0)
                                else if (maes.Expression.ToString() == listName && maes.Name.Identifier.Text == "RemoveAt")
                                {
                                    if (inv.ArgumentList.Arguments.Count == 1 &&
                                        inv.ArgumentList.Arguments.First().Expression is LiteralExpressionSyntax lit &&
                                        lit.Token.Value is int val && val == 0)
                                    {
                                        removeAtZeroCalled = true;
                                    }
                                }
                            }
                        }
                        return addCalled && removeAtZeroCalled;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "4. Отсортируйте массив чисел по возрастанию.",
                    DetailedDescription = "Создайте массив чисел и отсортируйте его по возрастанию. Пример:\nint[] numbers = {5, 3, 8, 1, 2};\nArray.Sort(numbers);",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // Ищем объявление массива int[]
                        string arrayName = null;
                         var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var ld in localDeclarations)
                        {
                            if (ld.Declaration.Type is ArrayTypeSyntax ats &&
                                ats.ElementType is PredefinedTypeSyntax pts && pts.Keyword.IsKind(SyntaxKind.IntKeyword))
                            {
                                arrayName = ld.Declaration.Variables.FirstOrDefault()?.Identifier.Text;
                                break;
                            }
                        }
                        if (arrayName == null) return false;

                        // Ищем Array.Sort(numbers)
                        var invocations = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>();
                        foreach (var inv in invocations)
                        {
                            if (inv.Expression is MemberAccessExpressionSyntax maes &&
                                maes.Expression is IdentifierNameSyntax idName && idName.Identifier.Text == "Array" &&
                                maes.Name.Identifier.Text == "Sort")
                            {
                                if (inv.ArgumentList.Arguments.Count == 1 &&
                                    inv.ArgumentList.Arguments.First().Expression.ToString() == arrayName)
                                {
                                    return true;
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "5. Найдите максимальный элемент в массиве.",
                    DetailedDescription = "Создайте массив чисел и найдите максимальный элемент. Пример:\nint[] numbers = {3, 7, 2, 9, 1};\nint max = numbers.Max();",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        string arrayName = null;
                         var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var ld in localDeclarations)
                        {
                            if (ld.Declaration.Type is ArrayTypeSyntax ats &&
                                ats.ElementType is PredefinedTypeSyntax pts && pts.Keyword.IsKind(SyntaxKind.IntKeyword))
                            {
                                arrayName = ld.Declaration.Variables.FirstOrDefault()?.Identifier.Text;
                                break;
                            }
                        }
                        if (arrayName == null) return false;

                        // Ищем numbers.Max()
                        var invocations = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>();
                        foreach (var inv in invocations)
                        {
                            if (inv.Expression is MemberAccessExpressionSyntax maes &&
                                maes.Expression.ToString() == arrayName &&
                                maes.Name.Identifier.Text == "Max")
                            {
                                if (inv.ArgumentList.Arguments.Count == 0) // Max() - метод расширения LINQ
                                {
                                    // Проверяем, что результат присваивается переменной типа int
                                    var parent = inv.Parent;
                                    if (parent is EqualsValueClauseSyntax eq &&
                                        eq.Parent is VariableDeclaratorSyntax vd &&
                                        vd.Parent is VariableDeclarationSyntax vds)
                                        {
                                            var assignedVarType = semanticModel.GetTypeInfo(vds.Type).Type;
                                            if (assignedVarType?.SpecialType == SpecialType.System_Int32) return true;
                                        }
                                    else if (parent is AssignmentExpressionSyntax aes) // max = numbers.Max();
                                    {
                                        var assignedVarSymbol = semanticModel.GetSymbolInfo(aes.Left).Symbol;
                                        if (assignedVarSymbol is ILocalSymbol local && local.Type.SpecialType == SpecialType.System_Int32) return true;
                                        if (assignedVarSymbol is IFieldSymbol field && field.Type.SpecialType == SpecialType.System_Int32) return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "6. Создайте двумерный массив 3x3 и заполните его.",
                    DetailedDescription = "Создайте двумерный массив 3x3 и заполните его значениями. Пример:\nint[,] matrix = new int[3,3];\nfor(int i = 0; i < 3; i++)\n{\n    for(int j = 0; j < 3; j++)\n    {\n        matrix[i,j] = i + j;\n    }\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // Ищем объявление двумерного массива int[,] matrix = new int[3,3];
                        string matrixName = null;
                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var ld in localDeclarations)
                        {
                            if (ld.Declaration.Type is ArrayTypeSyntax ats &&
                                ats.ElementType is PredefinedTypeSyntax pts && pts.Keyword.IsKind(SyntaxKind.IntKeyword) &&
                                ats.RankSpecifiers.Count == 1 && ats.RankSpecifiers.First().Rank == 2) // Проверка на [,]
                            {
                                var variable = ld.Declaration.Variables.FirstOrDefault();
                                if (variable?.Initializer?.Value is ArrayCreationExpressionSyntax aces)
                                {
                                    if (aces.Type.RankSpecifiers.Count == 1 &&
                                        aces.Type.RankSpecifiers.First().Sizes.Count == 2 &&
                                        aces.Type.RankSpecifiers.First().Sizes[0].ToString() == "3" &&
                                        aces.Type.RankSpecifiers.First().Sizes[1].ToString() == "3")
                                    {
                                        matrixName = variable.Identifier.Text;
                                        break;
                                    }
                                }
                            }
                        }
                        if (matrixName == null) return false;

                        // Ищем вложенные циклы for для заполнения
                        var outerForLoops = mainMethodNode.Body.DescendantNodes().OfType<ForStatementSyntax>();
                        foreach (var outerFor in outerForLoops)
                        {
                            // Упрощенная проверка: внешний цикл до 3
                            if (!(outerFor.Condition.ToString().Contains("< 3") || outerFor.Condition.ToString().Contains("<= 2"))) continue;

                            var innerForLoops = outerFor.Statement.DescendantNodes().OfType<ForStatementSyntax>();
                            foreach (var innerFor in innerForLoops)
                            {
                                // Упрощенная проверка: внутренний цикл до 3
                                if (!(innerFor.Condition.ToString().Contains("< 3") || innerFor.Condition.ToString().Contains("<= 2"))) continue;

                                // Ищем присвоение элементу matrix[i,j]
                                var assignments = innerFor.Statement.DescendantNodes().OfType<AssignmentExpressionSyntax>();
                                foreach (var assign in assignments)
                                {
                                    if (assign.Left is ElementAccessExpressionSyntax eaes &&
                                        eaes.Expression.ToString() == matrixName &&
                                        eaes.ArgumentList.Arguments.Count == 2)
                                    {
                                        // Убедимся, что в правой части есть какое-то выражение (не просто null)
                                        if (assign.Right != null && !assign.Right.IsKind(SyntaxKind.NullLiteralExpression))
                                        {
                                            return true; // Нашли инициализацию и заполнение
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "7. Используйте LINQ для фильтрации списка чисел.",
                    DetailedDescription = "Создайте список чисел и используйте LINQ для выборки чисел больше 5. Пример:\nList<int> numbers = new List<int> {1, 6, 3, 8, 2};\nvar filtered = numbers.Where(n => n > 5).ToList();",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;
                        
                        // Ищем объявление List<int>
                        string listName = null;
                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var ld in localDeclarations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(ld.Declaration.Type).Type;
                            if (typeSymbol?.ToDisplayString() == "System.Collections.Generic.List<int>")
                            {
                                listName = ld.Declaration.Variables.FirstOrDefault()?.Identifier.Text;
                                break;
                            }
                        }
                        if (listName == null) return false; // Список не объявлен

                        // Ищем вызов .Where(...)
                        var invocations = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>();
                        foreach (var invWhere in invocations)
                        {
                            if (invWhere.Expression is MemberAccessExpressionSyntax maesWhere &&
                                maesWhere.Expression.ToString() == listName && // или другая переменная, если .Where().ToList()
                                maesWhere.Name.Identifier.Text == "Where")
                            {
                                if (invWhere.ArgumentList.Arguments.Count == 1 &&
                                    invWhere.ArgumentList.Arguments.First().Expression is LambdaExpressionSyntax lambda)
                                {
                                    // Проверяем тело лямбды: n > 5 или что-то похожее
                                    if (lambda.Body is BinaryExpressionSyntax be &&
                                        (be.IsKind(SyntaxKind.GreaterThanExpression)))
                                    {
                                        // Проверяем, что правая часть - 5
                                        if ((be.Right is LiteralExpressionSyntax lit && lit.Token.Value is int val && val == 5) ||
                                            (be.Left is LiteralExpressionSyntax litL && litL.Token.Value is int valL && valL == 5)) // на случай 5 < n
                                        {
                                            // Опционально: проверить, что результат Where присваивается или используется в ToList()
                                            var parentInvocation = invWhere.Parent.Parent; // Where -> MemberAccess (ToList) -> Invocation (ToList())
                                            if (parentInvocation is InvocationExpressionSyntax invToList &&
                                                invToList.Expression is MemberAccessExpressionSyntax maesToList &&
                                                maesToList.Name.Identifier.Text == "ToList")
                                            {
                                                return true;
                                            }
                                            // Если просто var filtered = numbers.Where(...); тоже засчитаем
                                            if (invWhere.Parent is EqualsValueClauseSyntax) return true;
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "8. Конвертируйте массив в список и обратно.",
                    DetailedDescription = "Создайте массив, конвертируйте его в список, затем обратно в массив. Пример:\nint[] array = {1, 2, 3};\nList<int> list = array.ToList();\nint[] newArray = list.ToArray();",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        string initialArrayName = null;
                        string listName = null;
                        string finalArrayName = null;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        var assignments = mainMethodNode.Body.DescendantNodes().OfType<AssignmentExpressionSyntax>();

                        // 1. Найти объявление int[] array = ...;
                        foreach (var ld in localDeclarations)
                        {
                             if (ld.Declaration.Type is ArrayTypeSyntax ats &&
                                ats.ElementType is PredefinedTypeSyntax pts && pts.Keyword.IsKind(SyntaxKind.IntKeyword))
                            {
                                initialArrayName = ld.Declaration.Variables.FirstOrDefault()?.Identifier.Text;
                                break;
                            }
                        }
                        if (initialArrayName == null) return false;

                        // 2. Найти List<int> list = array.ToList();
                        bool toListFound = false;
                        foreach (var ld in localDeclarations) // ищем объявление list
                        {
                            var variable = ld.Declaration.Variables.FirstOrDefault();
                            if (variable?.Initializer?.Value is InvocationExpressionSyntax inv &&
                                inv.Expression is MemberAccessExpressionSyntax maes &&
                                maes.Expression.ToString() == initialArrayName &&
                                maes.Name.Identifier.Text == "ToList")
                            {
                                var listVarType = semanticModel.GetTypeInfo(ld.Declaration.Type).ConvertedType;
                                if (listVarType?.ToDisplayString() == "System.Collections.Generic.List<int>")
                                {
                                    listName = variable.Identifier.Text;
                                    toListFound = true;
                                    break;
                                }
                            }
                        }
                         if (!toListFound) // ищем присвоение list = array.ToList()
                        {
                            foreach (var assign in assignments)
                            {
                                if (assign.Right is InvocationExpressionSyntax inv &&
                                    inv.Expression is MemberAccessExpressionSyntax maes &&
                                    maes.Expression.ToString() == initialArrayName &&
                                    maes.Name.Identifier.Text == "ToList")
                                {
                                    var listVarSymbol = semanticModel.GetSymbolInfo(assign.Left).Symbol as ILocalSymbol;
                                    if (listVarSymbol?.Type.ToDisplayString() == "System.Collections.Generic.List<int>")
                                    {
                                        listName = listVarSymbol.Name;
                                        toListFound = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!toListFound || listName == null) return false;
                        
                        // 3. Найти int[] newArray = list.ToArray();
                        bool toArrayFound = false;
                        foreach (var ld in localDeclarations) // ищем объявление newArray
                        {
                            var variable = ld.Declaration.Variables.FirstOrDefault();
                            if (variable?.Initializer?.Value is InvocationExpressionSyntax inv &&
                                inv.Expression is MemberAccessExpressionSyntax maes &&
                                maes.Expression.ToString() == listName &&
                                maes.Name.Identifier.Text == "ToArray")
                            {
                                 if (ld.Declaration.Type is ArrayTypeSyntax ats &&
                                    ats.ElementType is PredefinedTypeSyntax pts && pts.Keyword.IsKind(SyntaxKind.IntKeyword))
                                {
                                    finalArrayName = variable.Identifier.Text;
                                    toArrayFound = true;
                                    break;
                                }
                            }
                        }
                        if (!toArrayFound) // ищем присвоение newArray = list.ToArray()
                        {
                             foreach (var assign in assignments)
                            {
                                if (assign.Right is InvocationExpressionSyntax inv &&
                                    inv.Expression is MemberAccessExpressionSyntax maes &&
                                    maes.Expression.ToString() == listName &&
                                    maes.Name.Identifier.Text == "ToArray")
                                {
                                    var arrayVarSymbol = semanticModel.GetSymbolInfo(assign.Left).Symbol as ILocalSymbol;
                                    if (arrayVarSymbol?.Type is IArrayTypeSymbol arrayType && arrayType.ElementType.SpecialType == SpecialType.System_Int32)
                                    {
                                        finalArrayName = arrayVarSymbol.Name;
                                        toArrayFound = true;
                                        break;
                                    }
                                }
                            }
                        }

                        return initialArrayName != null && listName != null && finalArrayName != null && toListFound && toArrayFound;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "9. Создайте словарь (Dictionary) с парами ключ-значение.",
                    DetailedDescription = "Создайте Dictionary<string, int> с несколькими парами ключ-значение. Пример:\nDictionary<string, int> ages = new Dictionary<string, int>\n{\n    [\"Анна\"] = 25,\n    [\"Иван\"] = 30\n};",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var ld in localDeclarations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(ld.Declaration.Type).Type;
                            if (typeSymbol?.ToDisplayString() == "System.Collections.Generic.Dictionary<string, int>")
                            {
                                var variable = ld.Declaration.Variables.FirstOrDefault();
                                // Проверяем инициализатор коллекции
                                if (variable?.Initializer?.Value is ObjectCreationExpressionSyntax oces &&
                                    oces.Initializer != null &&
                                    oces.Initializer.Expressions.Count >= 1) // Хотя бы одна пара
                                {
                                    bool allCorrectPairs = true;
                                    foreach(var expr in oces.Initializer.Expressions)
                                    {
                                        // { "key", value } or { ["key"] = value }
                                        if (expr is InitializerExpressionSyntax pairInit && pairInit.Expressions.Count == 2) // {"key", value}
                                        {
                                            if (!(pairInit.Expressions[0] is LiteralExpressionSyntax keyLit && keyLit.IsKind(SyntaxKind.StringLiteralExpression) &&
                                                  pairInit.Expressions[1] is LiteralExpressionSyntax valLit && valLit.IsKind(SyntaxKind.NumericLiteralExpression)))
                                            {
                                                allCorrectPairs = false; break;
                                            }
                                        }
                                        else if (expr is AssignmentExpressionSyntax assignPair && assignPair.IsKind(SyntaxKind.SimpleAssignmentExpression)) // {["key"] = value}
                                        {
                                            if (!(assignPair.Left is ImplicitElementAccessSyntax implAccess &&
                                                  implAccess.ArgumentList.Arguments.Count == 1 &&
                                                  implAccess.ArgumentList.Arguments.First().Expression is LiteralExpressionSyntax keyLit && keyLit.IsKind(SyntaxKind.StringLiteralExpression) &&
                                                  assignPair.Right is LiteralExpressionSyntax valLit && valLit.IsKind(SyntaxKind.NumericLiteralExpression)))
                                            {
                                                allCorrectPairs = false; break;
                                            }
                                        }
                                        else
                                        {
                                            allCorrectPairs = false; break;
                                        }
                                    }
                                    if (allCorrectPairs) return true;
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "10. Реализуйте стек (Stack) с операциями Push и Pop.",
                    DetailedDescription = "Создайте стек, добавьте элементы с помощью Push и извлеките с помощью Pop. Пример:\nStack<int> stack = new Stack<int>();\nstack.Push(1);\nstack.Push(2);\nint item = stack.Pop();",
                   ValidationMethod = (assembly) =>
                   {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        string stackName = null;
                        // Ищем объявление Stack<T> (будем гибкими с T, но пример int)
                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var ld in localDeclarations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(ld.Declaration.Type).Type;
                            if (typeSymbol is INamedTypeSymbol nts &&
                                nts.Name == "Stack" && nts.ContainingNamespace.ToDisplayString().StartsWith("System.Collections.Generic"))
                            {
                                stackName = ld.Declaration.Variables.FirstOrDefault()?.Identifier.Text;
                                break;
                            }
                        }
                        if (stackName == null) return false;

                        bool pushCalled = false;
                        bool popCalledAndAssigned = false;

                        var invocations = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>();
                        foreach (var inv in invocations)
                        {
                            if (inv.Expression is MemberAccessExpressionSyntax maes)
                            {
                                if (maes.Expression.ToString() == stackName && maes.Name.Identifier.Text == "Push")
                                {
                                    if (inv.ArgumentList.Arguments.Count == 1) // Push(item)
                                    {
                                        pushCalled = true;
                                    }
                                }
                                else if (maes.Expression.ToString() == stackName && maes.Name.Identifier.Text == "Pop")
                                {
                                    if (inv.ArgumentList.Arguments.Count == 0) // Pop()
                                    {
                                        // Проверяем, что результат Pop присваивается
                                        var parent = inv.Parent;
                                        if (parent is EqualsValueClauseSyntax || parent is AssignmentExpressionSyntax)
                                        {
                                            popCalledAndAssigned = true;
                                        }
                                    }
                                }
                            }
                        }
                        return pushCalled && popCalledAndAssigned;
                   }
                }
            }
        },
        new Module
        {
            Title = "Классы и объекты",
            Description = "Основы объектно-ориентированного программирования в C#.",
            Tasks = new List<PracticeTask>
            {
                new PracticeTask
                {
                    TaskDescription = "1. Создайте класс 'Car' с полем 'Model'.",
                    DetailedDescription = "Создайте класс Car с публичным полем Model типа string. Пример:\npublic class Car\n{\n    public string Model;\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        
                        // Ищем класс 'Car'
                        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                        foreach (var classDecl in classDeclarations)
                        {
                            if (classDecl.Identifier.Text == "Car")
                            {
                                // Ищем публичное поле 'Model' типа string
                                var fieldDeclarations = classDecl.Members.OfType<FieldDeclarationSyntax>();
                                foreach (var fieldDecl in fieldDeclarations)
                                {
                                    if (fieldDecl.Modifiers.Any(SyntaxKind.PublicKeyword) &&
                                        fieldDecl.Declaration.Type.ToString() == "string") // или PredefinedTypeSyntax(SyntaxKind.StringKeyword)
                                    {
                                        if (fieldDecl.Declaration.Variables.Any(v => v.Identifier.Text == "Model"))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "2. Создайте объект класса 'Person' и выведите его имя.",
                    DetailedDescription = "Создайте класс Person с полем Name, создайте объект этого класса и выведите имя. Пример:\n// Предполагается, что класс Person уже создан:\n// public class Person { public string Name; }\nPerson person = new Person();\nperson.Name = \"Иван\";\nConsole.WriteLine(person.Name);",
                   ValidationMethod = (assembly) =>
                   {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        // 1. Проверяем наличие класса Person с публичным полем Name типа string
                        bool personClassExists = false;
                        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                        foreach (var classDecl in classDeclarations)
                        {
                            if (classDecl.Identifier.Text == "Person")
                            {
                                var field = classDecl.Members.OfType<FieldDeclarationSyntax>()
                                    .FirstOrDefault(f => f.Modifiers.Any(SyntaxKind.PublicKeyword) &&
                                                         f.Declaration.Type.ToString() == "string" && // или PredefinedTypeSyntax
                                                         f.Declaration.Variables.Any(v => v.Identifier.Text == "Name"));
                                var property = classDecl.Members.OfType<PropertyDeclarationSyntax>()
                                    .FirstOrDefault(p => p.Modifiers.Any(SyntaxKind.PublicKeyword) &&
                                                         p.Type.ToString() == "string" && // или PredefinedTypeSyntax
                                                         p.Identifier.Text == "Name" &&
                                                         p.AccessorList != null && // Должен быть хотя бы get
                                                         p.AccessorList.Accessors.Any(acc => acc.IsKind(SyntaxKind.GetAccessorDeclaration))); 
                                                         // Для этого задания поле или свойство с public get подойдёт.
                                
                                if (field != null || property != null)
                                {
                                    personClassExists = true;
                                    break;
                                }
                            }
                        }
                        if (!personClassExists) return false;

                        // 2. Ищем в Main: Person person = new Person();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        string personVarName = null;
                        var objectCreations = mainMethodNode.Body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                        foreach (var oc in objectCreations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(oc).Type;
                            if (typeSymbol?.Name == "Person")
                            {
                                // Ищем присвоение этой переменной
                                if (oc.Parent is EqualsValueClauseSyntax evc && evc.Parent is VariableDeclaratorSyntax vd)
                                {
                                    personVarName = vd.Identifier.Text;
                                    break;
                                }
                            }
                        }
                        if (personVarName == null) return false;
                        
                        // 3. Ищем person.Name = "..."; (Присвоение)
                        bool nameAssigned = false;
                        var assignments = mainMethodNode.Body.DescendantNodes().OfType<AssignmentExpressionSyntax>();
                        foreach(var assign in assignments)
                        {
                            if (assign.Left is MemberAccessExpressionSyntax maes &&
                                maes.Expression.ToString() == personVarName &&
                                maes.Name.Identifier.Text == "Name" &&
                                assign.Right is LiteralExpressionSyntax lit && lit.IsKind(SyntaxKind.StringLiteralExpression))
                            {
                                nameAssigned = true;
                                break;
                            }
                        }
                        if (!nameAssigned) return false; // В примере есть присвоение

                        // 4. Ищем Console.WriteLine(person.Name);
                        bool namePrinted = mainMethodNode.Body.DescendantNodes()
                            .OfType<InvocationExpressionSyntax>()
                            .Any(inv => inv.Expression.ToString() == "Console.WriteLine" &&
                                        inv.ArgumentList.Arguments.Count == 1 &&
                                        inv.ArgumentList.Arguments.First().Expression.ToString() == personVarName + ".Name");

                        return namePrinted;
                   }
                },
                new PracticeTask
                {
                    TaskDescription = "3. Добавьте в класс метод, возвращающий строку.",
                    DetailedDescription = "Создайте класс с методом, который возвращает строку. Пример:\npublic class Greeter\n{\n    public string SayHello()\n    {\n        return \"Привет\";\n    }\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        var root = syntaxTree.GetRoot();
                        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                        foreach (var classDecl in classDeclarations)
                        {
                            // Ищем публичный метод, возвращающий string
                            var methodDeclarations = classDecl.Members.OfType<MethodDeclarationSyntax>();
                            foreach (var methodDecl in methodDeclarations)
                            {
                                // methodDecl.ReturnType.ToString() == "string" или PredefinedTypeSyntax(SyntaxKind.StringKeyword)
                                // methodDecl.ParameterList.Parameters.Count == 0 (если метод без параметров, как в примере)
                                // methodDecl.Body должен содержать ReturnStatementSyntax с LiteralExpressionSyntax (строка)
                                if (methodDecl.Modifiers.Any(SyntaxKind.PublicKeyword) && // Не обязательно public по заданию, но хорошо бы
                                    methodDecl.ReturnType.ToString() == "string") // Или PredefinedTypeSyntax
                                {
                                    // Проверяем, что метод действительно возвращает строку
                                    var returnStatements = methodDecl.Body?.DescendantNodes().OfType<ReturnStatementSyntax>();
                                    if (returnStatements != null && returnStatements.Any(rs => rs.Expression is LiteralExpressionSyntax lit && lit.IsKind(SyntaxKind.StringLiteralExpression) || rs.Expression !=null )) // Хотя бы один return string
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "4. Создайте конструктор класса с параметрами.",
                    DetailedDescription = "Создайте класс с конструктором, принимающим параметры. Пример:\npublic class Book\n{\n    public string Title;\n    public Book(string title)\n    {\n        Title = title;\n    }\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        var root = syntaxTree.GetRoot();
                        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                        foreach (var classDecl in classDeclarations)
                        {
                            var constructorDeclarations = classDecl.Members.OfType<ConstructorDeclarationSyntax>();
                            foreach (var ctorDecl in constructorDeclarations)
                            {
                                // Конструктор должен принимать хотя бы один параметр
                                if (ctorDecl.ParameterList.Parameters.Count > 0)
                                {
                                    // Дополнительно: проверить, что параметр используется для инициализации поля/свойства
                                    // Например, если параметр 'title' и есть поле/свойство 'Title'
                                    // и в теле конструктора есть this.Title = title; или Title = title;
                                    bool parameterUsedToAssignField = false;
                                    var firstParamName = ctorDecl.ParameterList.Parameters.First().Identifier.Text;

                                    var assignmentsInCtor = ctorDecl.Body?.DescendantNodes().OfType<AssignmentExpressionSyntax>();
                                    if (assignmentsInCtor != null)
                                    {
                                        foreach(var assign in assignmentsInCtor)
                                        {
                                            if (assign.Right.ToString() == firstParamName) // title присваивается
                                            {
                                                // Проверяем, что левая часть - это поле/свойство того же класса
                                                if (assign.Left is IdentifierNameSyntax idLeft && classDecl.Members.Any(m => (m is FieldDeclarationSyntax fd && fd.Declaration.Variables.Any(v => v.Identifier.Text == idLeft.Identifier.Text)) || (m is PropertyDeclarationSyntax pd && pd.Identifier.Text == idLeft.Identifier.Text) ))
                                                {
                                                    parameterUsedToAssignField = true;
                                                    break;
                                                }
                                                if (assign.Left is MemberAccessExpressionSyntax maesLeft && maesLeft.Expression.IsKind(SyntaxKind.ThisExpression))
                                                {
                                                    parameterUsedToAssignField = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (parameterUsedToAssignField) return true;
                                }
                            }
                        }
                        return false;
                    }
                },
                                new PracticeTask
                {
                    TaskDescription = "5. Реализуйте свойство (property) с get и set.",
                    DetailedDescription = "Создайте класс со свойством, имеющим get и set аксессоры. Пример:\npublic class Person\n{\n    private string name;\n    public string Name\n    {\n        get { return name; }\n        set { name = value; }\n    }\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree); // Добавил для семантики, если понадобится
                        
                        var root = syntaxTree.GetRoot();
                        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                        foreach (var classDecl in classDeclarations)
                        {
                            var propertyDeclarations = classDecl.Members.OfType<PropertyDeclarationSyntax>();
                            foreach (var propDecl in propertyDeclarations)
                            {
                                bool hasGet = false;
                                bool hasSet = false;

                                if (propDecl.AccessorList != null)
                                {
                                    AccessorDeclarationSyntax getterSyntax = null;
                                    AccessorDeclarationSyntax setterSyntax = null;

                                    foreach (var accessor in propDecl.AccessorList.Accessors)
                                    {
                                        if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                                        {
                                            hasGet = true;
                                            getterSyntax = accessor;
                                        }
                                        if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
                                        {
                                            hasSet = true;
                                            setterSyntax = accessor;
                                        }
                                    }

                                    if (hasGet && hasSet)
                                    {
                                        // Проверяем, это авто-свойство или свойство с телом
                                        bool isAutoProperty = getterSyntax?.Body == null && getterSyntax?.ExpressionBody == null &&
                                                              setterSyntax?.Body == null && setterSyntax?.ExpressionBody == null;

                                        if (isAutoProperty)
                                        {
                                            return true; // Авто-свойство { get; set; } найдено
                                        }
                                        else // Свойство с телом
                                        {
                                            string backingFieldName = null;
                                            // Ищем return <backingField>; в get
                                            if (getterSyntax?.Body?.DescendantNodes().OfType<ReturnStatementSyntax>().FirstOrDefault()?.Expression is IdentifierNameSyntax idNameGet)
                                            {
                                                backingFieldName = idNameGet.Identifier.Text;
                                            }
                                            else if (getterSyntax?.ExpressionBody?.Expression is IdentifierNameSyntax idNameGetExpr) // get => backingField;
                                            {
                                                backingFieldName = idNameGetExpr.Identifier.Text;
                                            }


                                            bool backingFieldSetInSetter = false;
                                            // Ищем <backingField> = value; в set
                                            var assignmentInSet = setterSyntax?.Body?.DescendantNodes().OfType<AssignmentExpressionSyntax>().FirstOrDefault();
                                            if (assignmentInSet != null)
                                            {
                                                if (assignmentInSet.Left is IdentifierNameSyntax idNameSet &&
                                                    idNameSet.Identifier.Text == backingFieldName &&
                                                    assignmentInSet.Right.ToString() == "value")
                                                {
                                                    backingFieldSetInSetter = true;
                                                }
                                            }
                                            else if (setterSyntax?.ExpressionBody?.Expression is AssignmentExpressionSyntax assignExprBody) // set => backingField = value;
                                            {
                                                 if (assignExprBody.Left is IdentifierNameSyntax idNameSet &&
                                                    idNameSet.Identifier.Text == backingFieldName &&
                                                    assignExprBody.Right.ToString() == "value")
                                                {
                                                    backingFieldSetInSetter = true;
                                                }
                                            }
                                        
                                            // Проверяем, что backingFieldName - это приватное поле класса
                                            if (backingFieldName != null && backingFieldSetInSetter)
                                            {
                                                var field = classDecl.Members.OfType<FieldDeclarationSyntax>()
                                                    .FirstOrDefault(f => (f.Modifiers.Any(SyntaxKind.PrivateKeyword) || f.Modifiers.Count == 0 /*по умолчанию private в классе*/) &&
                                                                         f.Declaration.Variables.Any(v => v.Identifier.Text == backingFieldName));
                                                if (field != null) return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "6. Создайте статический метод и поле.",
                    DetailedDescription = "Создайте класс со статическим методом и статическим полем. Пример:\npublic class Calculator\n{\n    public static int LastResult;\n    public static int Add(int a, int b)\n    {\n        LastResult = a + b;\n        return LastResult;\n    }\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        var root = syntaxTree.GetRoot();
                        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                        foreach (var classDecl in classDeclarations)
                        {
                            bool staticFieldFound = false;
                            bool staticMethodFound = false;

                            // Ищем статическое поле
                            var fieldDeclarations = classDecl.Members.OfType<FieldDeclarationSyntax>();
                            foreach (var fieldDecl in fieldDeclarations)
                            {
                                if (fieldDecl.Modifiers.Any(SyntaxKind.StaticKeyword))
                                {
                                    staticFieldFound = true;
                                    break;
                                }
                            }

                            // Ищем статический метод
                            var methodDeclarations = classDecl.Members.OfType<MethodDeclarationSyntax>();
                            foreach (var methodDecl in methodDeclarations)
                            {
                                if (methodDecl.Modifiers.Any(SyntaxKind.StaticKeyword))
                                {
                                    staticMethodFound = true;
                                    break;
                                }
                            }

                            if (staticFieldFound && staticMethodFound)
                            {
                                // Опционально: проверить, что метод использует поле, как в примере
                                return true;
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "7. Реализуйте наследование классов.",
                    DetailedDescription = "Создайте базовый класс и производный класс. Пример:\npublic class Animal\n{\n    public void Eat() { Console.WriteLine(\"Eating\"); }\n}\npublic class Dog : Animal\n{\n    public void Bark() { Console.WriteLine(\"Barking\"); }\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

                        if (classDeclarations.Count < 2) return false; // Нужно хотя бы два класса

                        foreach (var derivedClassDecl in classDeclarations)
                        {
                            if (derivedClassDecl.BaseList != null && derivedClassDecl.BaseList.Types.Count > 0)
                            {
                                var baseTypeNameSyntax = derivedClassDecl.BaseList.Types.First().Type;
                                var baseTypeSymbolInfo = semanticModel.GetSymbolInfo(baseTypeNameSyntax);

                                if (baseTypeSymbolInfo.Symbol is ITypeSymbol baseTypeSymbol)
                                {
                                    // Ищем объявление этого базового класса в коде пользователя
                                    var baseClassDeclaration = classDeclarations.FirstOrDefault(c => c.Identifier.Text == baseTypeSymbol.Name && c != derivedClassDecl);
                                    if (baseClassDeclaration != null)
                                    {
                                        // Убедимся, что производный класс не является базовым для базового (нет циклов)
                                        if (baseClassDeclaration.BaseList == null || !baseClassDeclaration.BaseList.Types.Any(t => t.Type.ToString() == derivedClassDecl.Identifier.Text))
                                        {
                                            // Дополнительно: проверить наличие методов, как в примере
                                            bool baseMethodExists = baseClassDeclaration.Members.OfType<MethodDeclarationSyntax>().Any(); // (e.g. Eat)
                                            bool derivedMethodExists = derivedClassDecl.Members.OfType<MethodDeclarationSyntax>().Any(); // (e.g. Bark)

                                            if(baseMethodExists && derivedMethodExists) return true;
                                            // Если просто есть наследование - тоже можно засчитать
                                            // return true; 
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "8. Переопределите метод ToString().",
                    DetailedDescription = "Создайте класс и переопределите метод ToString(). Пример:\npublic class Product\n{\n    public string Name;\n    public override string ToString()\n    {\n        return $\"Product: {Name}\";\n    }\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        var root = syntaxTree.GetRoot();
                        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                        foreach (var classDecl in classDeclarations)
                        {
                            var methodDeclarations = classDecl.Members.OfType<MethodDeclarationSyntax>();
                            foreach (var methodDecl in methodDeclarations)
                            {
                                if (methodDecl.Identifier.Text == "ToString" &&
                                    methodDecl.Modifiers.Any(SyntaxKind.OverrideKeyword) &&
                                    methodDecl.ReturnType.ToString() == "string" && // или PredefinedTypeSyntax
                                    methodDecl.ParameterList.Parameters.Count == 0)
                                {
                                    // Проверяем, что метод что-то возвращает (не пустой и не просто throw)
                                    if (methodDecl.Body != null && methodDecl.Body.Statements.OfType<ReturnStatementSyntax>().Any())
                                    {
                                        return true;
                                    }
                                    else if (methodDecl.ExpressionBody != null) // C# 6 expression-bodied member
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "9. Создайте абстрактный класс и производный класс.",
                    DetailedDescription = "Создайте абстрактный класс с абстрактным методом и реализуйте его в производном классе. Пример:\npublic abstract class Shape\n{\n    public abstract double Area();\n}\npublic class Circle : Shape\n{\n    public double Radius;\n    public override double Area()\n    {\n        return Math.PI * Radius * Radius;\n    }\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

                        ClassDeclarationSyntax abstractClass = null;
                        MethodDeclarationSyntax abstractMethod = null;

                        // Ищем абстрактный класс с абстрактным методом
                        foreach (var classDecl in classDeclarations)
                        {
                            if (classDecl.Modifiers.Any(SyntaxKind.AbstractKeyword))
                            {
                                foreach (var member in classDecl.Members.OfType<MethodDeclarationSyntax>())
                                {
                                    if (member.Modifiers.Any(SyntaxKind.AbstractKeyword))
                                    {
                                        abstractClass = classDecl;
                                        abstractMethod = member;
                                        break;
                                    }
                                }
                            }
                            if (abstractClass != null) break;
                        }

                        if (abstractClass == null || abstractMethod == null) return false;

                        // Ищем производный класс, который реализует этот абстрактный метод
                        foreach (var derivedClassDecl in classDeclarations)
                        {
                            if (derivedClassDecl == abstractClass) continue; // Не тот же класс

                            if (derivedClassDecl.BaseList != null &&
                                derivedClassDecl.BaseList.Types.Any(bt => bt.Type.ToString() == abstractClass.Identifier.Text))
                            {
                                // Класс наследуется от нашего абстрактного класса
                                // Ищем переопределенный метод
                                foreach (var methodImpl in derivedClassDecl.Members.OfType<MethodDeclarationSyntax>())
                                {
                                    if (methodImpl.Identifier.Text == abstractMethod.Identifier.Text &&
                                        methodImpl.Modifiers.Any(SyntaxKind.OverrideKeyword) &&
                                        methodImpl.ParameterList.Parameters.Count == abstractMethod.ParameterList.Parameters.Count) // и совпадение сигнатур по типам
                                    {
                                        // Проверяем, что метод имеет тело (не абстрактный сам)
                                        if (methodImpl.Body != null || methodImpl.ExpressionBody != null)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "10. Реализуйте интерфейс в классе.",
                    DetailedDescription = "Создайте интерфейс и класс, который его реализует. Пример:\npublic interface ILogger\n{\n    void Log(string message);\n}\npublic class ConsoleLogger : ILogger\n{\n    public void Log(string message)\n    {\n        Console.WriteLine(message);\n    }\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();

                        InterfaceDeclarationSyntax interfaceDeclaration = null;
                        MethodDeclarationSyntax interfaceMethod = null; // Или PropertyDeclarationSyntax, etc.

                        // 1. Ищем объявление интерфейса с хотя бы одним методом
                        foreach (var ifaceDecl in root.DescendantNodes().OfType<InterfaceDeclarationSyntax>())
                        {
                            interfaceMethod = ifaceDecl.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault();
                            if (interfaceMethod != null) // Упрощенно: первый попавшийся метод интерфейса
                            {
                                interfaceDeclaration = ifaceDecl;
                                break;
                            }
                            // Можно добавить поиск свойств и т.д.
                        }
                        if (interfaceDeclaration == null || interfaceMethod == null) return false;

                        // 2. Ищем класс, который реализует этот интерфейс и этот метод
                        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                        foreach (var classDecl in classDeclarations)
                        {
                            if (classDecl.BaseList != null &&
                                classDecl.BaseList.Types.Any(bt => bt.Type.ToString() == interfaceDeclaration.Identifier.Text))
                            {
                                // Класс заявляет, что реализует интерфейс
                                // Ищем реализацию метода интерфейса
                                foreach (var classMethod in classDecl.Members.OfType<MethodDeclarationSyntax>())
                                {
                                    // Проверяем имя и сигнатуру (упрощенно)
                                    if (classMethod.Identifier.Text == interfaceMethod.Identifier.Text &&
                                        classMethod.ParameterList.Parameters.Count == interfaceMethod.ParameterList.Parameters.Count)
                                    {
                                        // Проверяем совпадение типов параметров (сложнее, требует семантики)
                                        // Для явной реализации имя может быть Interface.Method
                                        ISymbol methodSymbol = semanticModel.GetDeclaredSymbol(classMethod);
                                        if (methodSymbol is IMethodSymbol classMethodSymbol)
                                        {
                                            foreach (var implementedInterfaceMethodSymbol in classMethodSymbol.ExplicitInterfaceImplementations)
                                            {
                                                if (implementedInterfaceMethodSymbol.Name == interfaceMethod.Identifier.Text &&
                                                    implementedInterfaceMethodSymbol.ContainingType.Name == interfaceDeclaration.Identifier.Text)
                                                {
                                                    return true; // Явная реализация
                                                }
                                            }
                                            // Для неявной реализации - проверяем, что метод public и сигнатура совпадает
                                            // и компилятор считает его реализацией
                                            if (classMethod.Modifiers.Any(SyntaxKind.PublicKeyword) && !classMethodSymbol.IsAbstract) // Неявная реализация должна быть public
                                            {
                                                 // Более надежная проверка через семантическую модель:
                                                 // Получаем символ типа класса
                                                 INamedTypeSymbol classTypeSymbol = semanticModel.GetDeclaredSymbol(classDecl);
                                                 if (classTypeSymbol != null)
                                                 {
                                                     ISymbol interfaceMethodSymbolFromInterface = semanticModel.GetDeclaredSymbol(interfaceMethod);
                                                     if (interfaceMethodSymbolFromInterface != null)
                                                     {
                                                         IMethodSymbol implementation = classTypeSymbol.FindImplementationForInterfaceMember(interfaceMethodSymbolFromInterface) as IMethodSymbol;
                                                         if (implementation != null && implementation.Equals(classMethodSymbol, SymbolEqualityComparer.Default))
                                                         {
                                                             return true; // Неявная реализация
                                                         }
                                                     }
                                                 }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                }
            }
        },

                new Module
        {
            Title = "Обработка исключений",
            Description = "Как обрабатывать ошибки и исключения в C#.",
            Tasks = new List<PracticeTask>
            {
                new PracticeTask
                {
                    TaskDescription = "1. Используйте try-catch для обработки деления на 0.",
                    DetailedDescription = "Напишите код, который пытается выполнить деление на 0 и обрабатывает исключение. Пример:\ntry\n{\n    int result = 10 / 0;\n}\ncatch(DivideByZeroException ex)\n{\n    Console.WriteLine(ex.Message);\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var tryStatements = mainMethodNode.Body.DescendantNodes().OfType<TryStatementSyntax>();
                        foreach (var tryStmt in tryStatements)
                        {
                            // 1. Проверяем, что в try блоке есть деление на ноль
                            bool divisionByZeroInTry = false;
                            var binaryExpressionsInTry = tryStmt.Block.DescendantNodes().OfType<BinaryExpressionSyntax>();
                            foreach (var be in binaryExpressionsInTry)
                            {
                                if (be.IsKind(SyntaxKind.DivideExpression) &&
                                    be.Right is LiteralExpressionSyntax lit && lit.Token.ValueText == "0")
                                {
                                    divisionByZeroInTry = true;
                                    break;
                                }
                            }
                            if (!divisionByZeroInTry) continue;

                            // 2. Проверяем, что есть catch (DivideByZeroException)
                            foreach (var catchClause in tryStmt.Catches)
                            {
                                if (catchClause.Declaration != null)
                                {
                                    var caughtExceptionType = semanticModel.GetTypeInfo(catchClause.Declaration.Type).Type;
                                    if (caughtExceptionType?.ToDisplayString() == "System.DivideByZeroException")
                                    {
                                        // Опционально: проверить, что в catch блоке есть Console.WriteLine(ex.Message)
                                        if (catchClause.Block.DescendantNodes().OfType<InvocationExpressionSyntax>()
                                            .Any(inv => inv.Expression.ToString() == "Console.WriteLine" &&
                                                        inv.ArgumentList.Arguments.Count > 0 &&
                                                        inv.ArgumentList.Arguments.First().Expression.ToString().EndsWith(".Message"))) // ex.Message или e.Message etc
                                        {
                                            return true;
                                        }
                                        // Если просто ловит исключение - тоже засчитаем
                                        // return true; 
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "2. Обработайте исключение при чтении файла.",
                    DetailedDescription = "Напишите код, который пытается прочитать несуществующий файл и обрабатывает исключение. Пример:\ntry\n{\n    string content = File.ReadAllText(\"nonexistent.txt\");\n}\ncatch(FileNotFoundException ex)\n{\n    Console.WriteLine(\"Файл не найден: \" + ex.Message);\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var tryStatements = mainMethodNode.Body.DescendantNodes().OfType<TryStatementSyntax>();
                        foreach (var tryStmt in tryStatements)
                        {
                            // 1. Проверяем, что в try блоке есть File.ReadAllText или похожий файловый IO
                            bool fileOperationInTry = tryStmt.Block.DescendantNodes()
                                .OfType<InvocationExpressionSyntax>()
                                .Any(inv => inv.Expression.ToString().StartsWith("File.Read") || inv.Expression.ToString().StartsWith("System.IO.File.Read"));

                            if (!fileOperationInTry) continue;

                            // 2. Проверяем, что есть catch (FileNotFoundException) или IOException
                            foreach (var catchClause in tryStmt.Catches)
                            {
                                if (catchClause.Declaration != null)
                                {
                                    var caughtExceptionType = semanticModel.GetTypeInfo(catchClause.Declaration.Type).Type;
                                    if (caughtExceptionType?.ToDisplayString() == "System.IO.FileNotFoundException" ||
                                        caughtExceptionType?.ToDisplayString() == "System.IO.IOException") // IOException как более общий
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "3. Создайте собственное исключение и вызовите его.",
                    DetailedDescription = "Создайте собственный класс исключения и вызовите его с помощью throw. Пример:\npublic class MyException : Exception\n{\n    public MyException(string message) : base(message) { }\n}\nthrow new MyException(\"Произошла ошибка\");",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        // 1. Ищем объявление кастомного класса исключения, наследующегося от Exception
                        ClassDeclarationSyntax customExceptionClass = null;
                        foreach (var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                        {
                            if (classDecl.BaseList != null)
                            {
                                foreach (var baseTypeSyntax in classDecl.BaseList.Types)
                                {
                                    var baseTypeSymbol = semanticModel.GetTypeInfo(baseTypeSyntax.Type).Type;
                                    // Проверяем всю цепочку наследования до System.Exception
                                    ITypeSymbol currentBase = baseTypeSymbol;
                                    bool inheritsFromSystemException = false;
                                    while(currentBase != null)
                                    {
                                        if (currentBase.ToDisplayString() == "System.Exception")
                                        {
                                            inheritsFromSystemException = true;
                                            break;
                                        }
                                        currentBase = currentBase.BaseType;
                                    }

                                    if (inheritsFromSystemException)
                                    {
                                        customExceptionClass = classDecl;
                                        // Проверяем наличие конструктора с message (опционально, но хорошо бы)
                                        var ctorWithMessage = classDecl.Members.OfType<ConstructorDeclarationSyntax>()
                                            .Any(ctor => ctor.ParameterList.Parameters.Count == 1 &&
                                                         ctor.ParameterList.Parameters.First().Type.ToString() == "string" &&
                                                         ctor.Initializer != null && ctor.Initializer.IsKind(SyntaxKind.BaseConstructorInitializer) &&
                                                         ctor.Initializer.ArgumentList.Arguments.Count == 1);
                                        // if(ctorWithMessage) { /* еще лучше */ }
                                        break;
                                    }
                                }
                            }
                            if (customExceptionClass != null) break;
                        }
                        if (customExceptionClass == null) return false;

                        // 2. Ищем throw new CustomException(...) в Main или другом методе
                        var mainMethodNode = root.DescendantNodes() // Ищем throw в любом месте кода пользователя (не только Main)
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        
                        // Ищем throw в любом методе (включая Main, если он есть)
                        var allMethods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
                        if (mainMethodNode != null && !allMethods.Contains(mainMethodNode))
                        {
                            allMethods = allMethods.Append(mainMethodNode);
                        }


                        foreach(var methodNode in allMethods)
                        {
                            if (methodNode?.Body == null && methodNode?.ExpressionBody == null) continue;

                            var throwStatements = (methodNode.Body ?? (SyntaxNode)methodNode.ExpressionBody.Expression.Parent) // немного грубо для expression body
                                .DescendantNodes().OfType<ThrowStatementSyntax>();

                            foreach (var throwStmt in throwStatements)
                            {
                                if (throwStmt.Expression is ObjectCreationExpressionSyntax oces)
                                {
                                    var thrownExceptionType = semanticModel.GetTypeInfo(oces).Type;
                                    if (thrownExceptionType?.Name == customExceptionClass.Identifier.Text)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "4. Используйте блок finally для освобождения ресурсов.",
                    DetailedDescription = "Напишите код с try-catch-finally, где в finally освобождаются ресурсы. Пример:\nFileStream file = null;\ntry\n{\n    file = File.Open(\"test.txt\", FileMode.Open);\n    // работа с файлом\n}\ncatch\n{\n    Console.WriteLine(\"Ошибка\");\n}\nfinally\n{\n    file?.Close();\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        // SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree); // Может понадобиться для типов

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var tryStatements = mainMethodNode.Body.DescendantNodes().OfType<TryStatementSyntax>();
                        foreach (var tryStmt in tryStatements)
                        {
                            if (tryStmt.Finally != null && tryStmt.Finally.Block.Statements.Count > 0)
                            {
                                // Проверяем, что в finally есть вызов .Close() или .Dispose()
                                // на переменной, которая может быть IDisposable (например, FileStream)
                                bool resourceReleased = false;
                                var invocationsInFinally = tryStmt.Finally.Block.DescendantNodes().OfType<InvocationExpressionSyntax>();
                                foreach(var inv in invocationsInFinally)
                                {
                                    if (inv.Expression is MemberAccessExpressionSyntax maes)
                                    {
                                        if (maes.Name.Identifier.Text == "Close" || maes.Name.Identifier.Text == "Dispose")
                                        {
                                            // Проверить бы еще, что maes.Expression (объект, на котором вызван метод) - это ресурс
                                            // Например, переменная, объявленная до try или в try.
                                            // Для упрощения, просто наличие Close/Dispose в finally уже хорошо.
                                            resourceReleased = true;
                                            break;
                                        }
                                    }
                                }
                                if (resourceReleased) return true;
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "5. Обработайте несколько типов исключений в одном try-catch.",
                    DetailedDescription = "Напишите код, который обрабатывает разные типы исключений. Пример:\ntry\n{\n    // код, который может вызвать разные исключения\n}\ncatch(ArgumentNullException ex)\n{\n    Console.WriteLine(\"Null argument: \" + ex.Message);\n}\ncatch(ArgumentException ex)\n{\n    Console.WriteLine(\"Invalid argument: \" + ex.Message);\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        // SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var tryStatements = mainMethodNode.Body.DescendantNodes().OfType<TryStatementSyntax>();
                        foreach (var tryStmt in tryStatements)
                        {
                            // Проверяем, что есть как минимум два разных catch блока с декларациями
                            if (tryStmt.Catches.Count(c => c.Declaration != null) >= 2)
                            {
                                // Можно дополнительно проверить, что типы исключений в catch разные
                                var caughtTypes = new HashSet<string>();
                                foreach (var catchClause in tryStmt.Catches)
                                {
                                    if (catchClause.Declaration != null)
                                    {
                                        caughtTypes.Add(catchClause.Declaration.Type.ToString());
                                    }
                                }
                                if (caughtTypes.Count >= 2) return true;
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "6. Используйте when для условной обработки исключений.",
                    DetailedDescription = "Напишите обработчик исключений с условием when. Пример:\ntry\n{\n    // код\n}\ncatch(Exception ex) when (ex.Message.Contains(\"специфическая ошибка\"))\n{\n    Console.WriteLine(\"Обработана специфическая ошибка\");\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var tryStatements = mainMethodNode.Body.DescendantNodes().OfType<TryStatementSyntax>();
                        foreach (var tryStmt in tryStatements)
                        {
                            foreach (var catchClause in tryStmt.Catches)
                            {
                                if (catchClause.Filter != null && catchClause.Filter.WhenKeyword.IsKind(SyntaxKind.WhenKeyword))
                                {
                                    // Проверяем, что условие в when не просто true/false
                                    if (!(catchClause.Filter.FilterExpression is LiteralExpressionSyntax lit &&
                                          (lit.IsKind(SyntaxKind.TrueLiteralExpression) || lit.IsKind(SyntaxKind.FalseLiteralExpression))))
                                    {
                                         return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "7. Создайте метод, который бросает исключение.",
                    DetailedDescription = "Создайте метод, который проверяет входные параметры и бросает исключение, если они неверны. Пример:\npublic void Process(int value)\n{\n    if(value < 0)\n        throw new ArgumentException(\"Value cannot be negative\");\n    // остальной код\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        // Ищем любой метод (не только в Program.Main, т.к. это может быть метод в другом классе)
                        var allMethods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
                        foreach (var methodDecl in allMethods)
                        {
                            if (methodDecl.Body == null && methodDecl.ExpressionBody == null) continue;

                            // Ищем if-statement
                            var ifStatements = (methodDecl.Body ?? (SyntaxNode)methodDecl.ExpressionBody.Expression.Parent)
                                .DescendantNodes().OfType<IfStatementSyntax>();
                            foreach (var ifStmt in ifStatements)
                            {
                                // Ищем throw new Exception() внутри if
                                var throwStatements = ifStmt.Statement.DescendantNodesAndSelf().OfType<ThrowStatementSyntax>();
                                foreach (var throwStmt in throwStatements)
                                {
                                    if (throwStmt.Expression is ObjectCreationExpressionSyntax oces)
                                    {
                                        var thrownExceptionType = semanticModel.GetTypeInfo(oces.Type).Type;
                                        // Проверяем, что это какое-то стандартное исключение или наше кастомное
                                        if (thrownExceptionType != null && (thrownExceptionType.ToDisplayString().Contains("Exception") || thrownExceptionType.BaseType?.ToDisplayString().Contains("Exception") == true))
                                        {
                                            // Убедимся, что условие if не тривиально (не просто true/false)
                                            if (!(ifStmt.Condition is LiteralExpressionSyntax lit &&
                                                (lit.IsKind(SyntaxKind.TrueLiteralExpression) || lit.IsKind(SyntaxKind.FalseLiteralExpression))))
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "8. Обработайте AggregateException.",
                    DetailedDescription = "Напишите код, который обрабатывает AggregateException. Пример:\ntry\n{\n    Parallel.Invoke(\n        () => { throw new Exception(\"Ошибка 1\"); },\n        () => { throw new Exception(\"Ошибка 2\"); });\n}\ncatch(AggregateException ex)\n{\n    foreach(var e in ex.InnerExceptions)\n    {\n        Console.WriteLine(e.Message);\n    }\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var tryStatements = mainMethodNode.Body.DescendantNodes().OfType<TryStatementSyntax>();
                        foreach (var tryStmt in tryStatements)
                        {
                            // 1. Проверяем, что в try блоке есть что-то, что может бросить AggregateException
                            // (например, Parallel.Invoke, Task.WaitAll)
                            bool parallelOpInTry = tryStmt.Block.DescendantNodes()
                                .OfType<InvocationExpressionSyntax>()
                                .Any(inv => inv.Expression.ToString().Contains("Parallel.") || inv.Expression.ToString().Contains("Task.Wait"));

                            if (!parallelOpInTry && !tryStmt.Block.ToString().Contains("Task")) // грубая проверка на Task.Run(()=>throw).Wait();
                            {
                                // Если пользователь не использует Parallel.Invoke, а просто создает Task и ждет его,
                                // и этот Task бросает исключение, то тоже может быть AggregateException.
                                // Эта проверка очень упрощенная.
                            }


                            // 2. Проверяем, что есть catch (AggregateException ex)
                            foreach (var catchClause in tryStmt.Catches)
                            {
                                if (catchClause.Declaration != null)
                                {
                                    var caughtExceptionType = semanticModel.GetTypeInfo(catchClause.Declaration.Type).Type;
                                    if (caughtExceptionType?.ToDisplayString() == "System.AggregateException")
                                    {
                                        // Опционально: проверить foreach по InnerExceptions
                                        if (catchClause.Block.DescendantNodes().OfType<ForEachStatementSyntax>()
                                            .Any(fe => fe.Expression.ToString().EndsWith(".InnerExceptions"))) // ex.InnerExceptions
                                        {
                                            return true;
                                        }
                                        // Если просто ловит AggregateException - тоже засчитаем
                                        // return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "9. Используйте throw без параметров в catch.",
                    DetailedDescription = "Напишите обработчик исключений, который перебрасывает исходное исключение. Пример:\ntry\n{\n    // код\n}\ncatch(Exception)\n{\n    // логирование\n    throw; // переброс исходного исключения\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var tryStatements = mainMethodNode.Body.DescendantNodes().OfType<TryStatementSyntax>();
                        foreach (var tryStmt in tryStatements)
                        {
                            foreach (var catchClause in tryStmt.Catches)
                            {
                                // Ищем throw; (без выражения)
                                if (catchClause.Block.DescendantNodes().OfType<ThrowStatementSyntax>()
                                    .Any(ts => ts.Expression == null))
                                {
                                    return true;
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "10. Создайте фильтр исключений с when для логирования.",
                    DetailedDescription = "Реализуйте обработку исключений с фильтрацией по условию и логированием. Пример:\ntry\n{\n    // код\n}\ncatch(Exception ex) when (LogException(ex))\n{\n    Console.WriteLine(\"Обработано исключение\");\n}\nprivate bool LogException(Exception ex)\n{\n    Console.WriteLine($\"Зарегистрировано исключение: {ex.Message}\");\n    return true;\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        // 1. Ищем метод типа `bool LogException(Exception ex)`
                        MethodDeclarationSyntax logMethod = null;
                        string logMethodName = null;

                        var allMethods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
                        foreach(var methodDecl in allMethods)
                        {
                            if (methodDecl.ReturnType.ToString() == "bool" && // или PredefinedTypeSyntax
                                methodDecl.ParameterList.Parameters.Count == 1)
                            {
                                var param = methodDecl.ParameterList.Parameters.First();
                                var paramTypeSymbol = semanticModel.GetTypeInfo(param.Type).Type;
                                if (paramTypeSymbol?.ToDisplayString() == "System.Exception" || paramTypeSymbol?.BaseType?.ToDisplayString() == "System.Exception")
                                {
                                    // Проверяем, что метод содержит Console.WriteLine и возвращает true
                                    bool hasConsoleWrite = methodDecl.Body?.DescendantNodes().OfType<InvocationExpressionSyntax>()
                                                            .Any(inv => inv.Expression.ToString().Contains("Console.WriteLine")) ?? false;
                                    bool returnsTrue = methodDecl.Body?.DescendantNodes().OfType<ReturnStatementSyntax>()
                                                            .Any(rs => rs.Expression is LiteralExpressionSyntax lit && lit.IsKind(SyntaxKind.TrueLiteralExpression)) ?? false;

                                    if(hasConsoleWrite && returnsTrue)
                                    {
                                        logMethod = methodDecl;
                                        logMethodName = methodDecl.Identifier.Text;
                                        break;
                                    }
                                }
                            }
                        }
                        if (logMethod == null) return false;

                        // 2. Ищем try-catch с when, вызывающим этот метод
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false; // Проверяем в Main

                        var tryStatements = mainMethodNode.Body.DescendantNodes().OfType<TryStatementSyntax>();
                        foreach (var tryStmt in tryStatements)
                        {
                            foreach (var catchClause in tryStmt.Catches)
                            {
                                if (catchClause.Filter != null &&
                                    catchClause.Filter.WhenKeyword.IsKind(SyntaxKind.WhenKeyword) &&
                                    catchClause.Filter.FilterExpression is InvocationExpressionSyntax invFilter)
                                {
                                    if (invFilter.Expression.ToString() == logMethodName &&
                                        invFilter.ArgumentList.Arguments.Count == 1 &&
                                        catchClause.Declaration != null && // Убедимся, что ex передается
                                        invFilter.ArgumentList.Arguments.First().Expression.ToString() == catchClause.Declaration.Identifier.Text)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                }
            }
        },
                new Module
        {
            Title = "Работа с базами данных",
            Description = "Изучите основы подключения к базам данных с помощью ADO.NET.",
            Tasks = new List<PracticeTask>
            {
                new PracticeTask
                {
                    TaskDescription = "1. Создайте подключение к базе данных SQLite.",
                    DetailedDescription = "Напишите код для создания подключения к SQLite базе данных. Пример:\n// using System.Data.SQLite; // Может понадобиться, если используется System.Data.SQLite\n// или using Microsoft.Data.Sqlite;\nusing var connection = new SQLiteConnection(\"Data Source=mydatabase.db\");\nconnection.Open();",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // Ищем new SQLiteConnection("...")
                        var objectCreations = mainMethodNode.Body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                        string connectionVarName = null;
                        foreach (var oc in objectCreations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(oc).Type;
                            // Проверяем имя класса, т.к. неймспейс может быть разным (System.Data.SQLite vs Microsoft.Data.Sqlite)
                            if (typeSymbol?.Name == "SQLiteConnection")
                            {
                                if (oc.ArgumentList != null && oc.ArgumentList.Arguments.Count == 1 &&
                                    oc.ArgumentList.Arguments.First().Expression is LiteralExpressionSyntax lit &&
                                    lit.IsKind(SyntaxKind.StringLiteralExpression) && lit.Token.ValueText.Contains("Data Source="))
                                {
                                    // Найти имя переменной
                                    if (oc.Parent is EqualsValueClauseSyntax evc && evc.Parent is VariableDeclaratorSyntax vd)
                                    {
                                        connectionVarName = vd.Identifier.Text;
                                    }
                                    else if (oc.Parent is AssignmentExpressionSyntax aes) // Если присваивается уже объявленной
                                    {
                                        connectionVarName = aes.Left.ToString();
                                    }
                                    break;
                                }
                            }
                        }
                        if (connectionVarName == null) return false;

                        // Ищем connection.Open();
                        var invocations = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>();
                        bool openCalled = invocations.Any(inv =>
                            inv.Expression is MemberAccessExpressionSyntax maes &&
                            maes.Expression.ToString() == connectionVarName &&
                            maes.Name.Identifier.Text == "Open" &&
                            inv.ArgumentList.Arguments.Count == 0);

                        return openCalled;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "2. Создайте SQL-запрос SELECT для выборки всех записей из таблицы 'Users'.",
                    DetailedDescription = "Напишите SQL-запрос для выборки всех записей из таблицы Users. Пример:\nstring sql = \"SELECT * FROM Users\";",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // Ищем объявление строки, содержащей "SELECT * FROM Users"
                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach(var ld in localDeclarations)
                        {
                            if (ld.Declaration.Type.ToString() == "string") // или PredefinedTypeSyntax
                            {
                                foreach(var variable in ld.Declaration.Variables)
                                {
                                    if (variable.Initializer?.Value is LiteralExpressionSyntax lit &&
                                        lit.IsKind(SyntaxKind.StringLiteralExpression) &&
                                        lit.Token.ValueText.Trim().ToUpperInvariant() == "SELECT * FROM USERS")
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "3. Используйте SqlCommand для выполнения SQL-запроса.",
                    DetailedDescription = "Создайте и выполните SqlCommand для SQL-запроса. Пример:\n// Предполагается, что 'connection' уже есть и открыто, а 'sql' содержит запрос.\nusing var command = new SqlCommand(sql, connection);\nusing var reader = command.ExecuteReader();",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        string commandVarName = null;
                        // Ищем new SqlCommand(sql, connection)
                        var objectCreations = mainMethodNode.Body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                        foreach (var oc in objectCreations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(oc).Type;
                            if (typeSymbol?.Name == "SqlCommand") // Проверяем имя класса
                            {
                                if (oc.ArgumentList != null && oc.ArgumentList.Arguments.Count == 2)
                                {
                                    // Аргументы должны быть строкой (sql) и объектом (connection)
                                    var arg1Type = semanticModel.GetTypeInfo(oc.ArgumentList.Arguments[0].Expression).ConvertedType;
                                    var arg2Type = semanticModel.GetTypeInfo(oc.ArgumentList.Arguments[1].Expression).ConvertedType;

                                    if (arg1Type?.SpecialType == SpecialType.System_String &&
                                        (arg2Type?.Name.Contains("Connection") ?? false) ) // SQLiteConnection, SqlConnection, etc.
                                    {
                                        if (oc.Parent is EqualsValueClauseSyntax evc && evc.Parent is VariableDeclaratorSyntax vd)
                                        {
                                            commandVarName = vd.Identifier.Text;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (commandVarName == null) return false;

                        // Ищем command.ExecuteReader() или ExecuteNonQuery() или ExecuteScalar()
                        var invocations = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>();
                        bool executeCalled = invocations.Any(inv =>
                            inv.Expression is MemberAccessExpressionSyntax maes &&
                            maes.Expression.ToString() == commandVarName &&
                            (maes.Name.Identifier.Text == "ExecuteReader" ||
                             maes.Name.Identifier.Text == "ExecuteNonQuery" ||
                             maes.Name.Identifier.Text == "ExecuteScalar"));

                        return executeCalled;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "4. Добавьте параметры в SqlCommand для безопасного выполнения.",
                    DetailedDescription = "Используйте параметризованные запросы для безопасного выполнения SQL. Пример:\n// var command = new SqlCommand(\"INSERT INTO Users (Name) VALUES (@Name)\", connection);\ncommand.Parameters.AddWithValue(\"@Name\", \"Иван\");",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        string commandVarName = null;
                        // Сначала найдем SqlCommand переменную
                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach(var ld in localDeclarations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(ld.Declaration.Type).ConvertedType;
                            if(typeSymbol?.Name == "SqlCommand")
                            {
                                commandVarName = ld.Declaration.Variables.FirstOrDefault()?.Identifier.Text;
                                break;
                            }
                        }
                        if (commandVarName == null) return false;

                        // Ищем command.Parameters.AddWithValue или .Add
                        var invocations = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>();
                        foreach (var inv in invocations)
                        {
                            if (inv.Expression is MemberAccessExpressionSyntax maes &&
                                (maes.Name.Identifier.Text == "AddWithValue" || maes.Name.Identifier.Text == "Add"))
                            {
                                // Проверяем, что это вызвано на command.Parameters
                                if (maes.Expression is MemberAccessExpressionSyntax paramsAccess &&
                                    paramsAccess.Expression.ToString() == commandVarName &&
                                    paramsAccess.Name.Identifier.Text == "Parameters")
                                {
                                    // AddWithValue ожидает 2 аргумента: имя параметра (строка) и значение
                                    if (maes.Name.Identifier.Text == "AddWithValue" && inv.ArgumentList.Arguments.Count == 2 &&
                                        inv.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax paramNameLit &&
                                        paramNameLit.IsKind(SyntaxKind.StringLiteralExpression) && paramNameLit.Token.ValueText.StartsWith("@"))
                                    {
                                        return true;
                                    }
                                    // Add может иметь разные перегрузки, например, Add("@Name", SqlDbType.VarChar)
                                    if (maes.Name.Identifier.Text == "Add" && inv.ArgumentList.Arguments.Count >= 1 &&
                                        inv.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax paramNameLitAdd &&
                                        paramNameLitAdd.IsKind(SyntaxKind.StringLiteralExpression) && paramNameLitAdd.Token.ValueText.StartsWith("@"))
                                    {
                                        return true; // Упрощенная проверка для Add
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "5. Реализуйте CRUD операции для таблицы Products.",
                    DetailedDescription = "Создайте методы для Create, Read, Update и Delete операций с таблицей Products. Пример:\n// Create\ncommand.CommandText = \"INSERT INTO Products (Name, Price) VALUES (@Name, @Price)\";\n// Read\ncommand.CommandText = \"SELECT * FROM Products WHERE Id = @Id\";\n// Update\ncommand.CommandText = \"UPDATE Products SET Price = @Price WHERE Id = @Id\";\n// Delete\ncommand.CommandText = \"DELETE FROM Products WHERE Id = @Id\";",
                    ValidationMethod = (assembly) => // Проверяем наличие SQL строк, характерных для CRUD
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        bool createFound = false;
                        bool readFound = false;
                        bool updateFound = false;
                        bool deleteFound = false;

                        var assignments = mainMethodNode.Body.DescendantNodes().OfType<AssignmentExpressionSyntax>();
                        foreach(var assign in assignments)
                        {
                            // Ищем присвоение CommandText: command.CommandText = "SQL..."
                            if (assign.Left is MemberAccessExpressionSyntax maes &&
                                maes.Name.Identifier.Text == "CommandText" && /*maes.Expression.ToString() - это имя переменной команды*/
                                assign.Right is LiteralExpressionSyntax lit && lit.IsKind(SyntaxKind.StringLiteralExpression))
                            {
                                string sql = lit.Token.ValueText.ToUpperInvariant();
                                if (sql.Contains("INSERT INTO PRODUCTS")) createFound = true;
                                if (sql.Contains("SELECT") && sql.Contains("FROM PRODUCTS")) readFound = true; // Может быть SELECT * или SELECT Id, Name
                                if (sql.Contains("UPDATE PRODUCTS SET")) updateFound = true;
                                if (sql.Contains("DELETE FROM PRODUCTS")) deleteFound = true;
                            }
                        }
                        return createFound && readFound && updateFound && deleteFound;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "6. Используйте транзакцию для групповых операций.",
                    DetailedDescription = "Выполните несколько SQL-команд в одной транзакции. Пример:\n// using var connection = ...; connection.Open();\nusing var transaction = connection.BeginTransaction();\ntry\n{\n    // несколько команд\n    transaction.Commit();\n}\ncatch\n{\n    transaction.Rollback();\n    throw;\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        string connectionVarName = null; // Найти переменную connection
                        string transactionVarName = null;

                        // Ищем connection.BeginTransaction()
                        var invocations = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>();
                        foreach (var inv in invocations)
                        {
                            if (inv.Expression is MemberAccessExpressionSyntax maes &&
                                maes.Name.Identifier.Text == "BeginTransaction")
                            {
                                var targetType = semanticModel.GetTypeInfo(maes.Expression).Type;
                                if (targetType?.Name.Contains("Connection") ?? false) // Проверяем, что вызывается на Connection объекте
                                {
                                    connectionVarName = maes.Expression.ToString();
                                    // Ищем присвоение результата переменной
                                    if (inv.Parent is EqualsValueClauseSyntax evc && evc.Parent is VariableDeclaratorSyntax vd)
                                    {
                                        transactionVarName = vd.Identifier.Text;
                                        break;
                                    }
                                }
                            }
                        }
                        if (transactionVarName == null) return false;

                        // Ищем try-catch-finally или try-catch с transaction.Commit() и transaction.Rollback()
                        var tryStatements = mainMethodNode.Body.DescendantNodes().OfType<TryStatementSyntax>();
                        foreach (var tryStmt in tryStatements)
                        {
                            bool commitInTry = tryStmt.Block.DescendantNodes()
                                .OfType<InvocationExpressionSyntax>()
                                .Any(inv => inv.Expression.ToString() == transactionVarName + ".Commit");

                            bool rollbackInCatch = false;
                            foreach (var catchClause in tryStmt.Catches)
                            {
                                if (catchClause.Block.DescendantNodes()
                                    .OfType<InvocationExpressionSyntax>()
                                    .Any(inv => inv.Expression.ToString() == transactionVarName + ".Rollback"))
                                {
                                    rollbackInCatch = true;
                                    break;
                                }
                            }
                            if (commitInTry && rollbackInCatch) return true;
                        }
                        return false;
                    }
                },
                new PracticeTask // Эта задача очень сложна для точной статической проверки без реальной БД. Упрощаем.
                {
                    TaskDescription = "7. Создайте хранимую процедуру и вызовите её.",
                    DetailedDescription = "Напишите код для создания и вызова хранимой процедуры. Пример:\n// Создание\ncommand.CommandText = \"CREATE PROCEDURE GetUsers AS SELECT * FROM Users\";\ncommand.ExecuteNonQuery();\n// Вызов\ncommand.CommandText = \"GetUsers\";\ncommand.CommandType = CommandType.StoredProcedure;",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        // SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                        var root = syntaxTree.GetRoot();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        bool createProcFound = false;
                        bool callProcFound = false;
                        string commandVarName = null; // Предполагаем, что одна и та же переменная команды используется

                        var assignments = mainMethodNode.Body.DescendantNodes().OfType<AssignmentExpressionSyntax>();
                        foreach(var assign in assignments)
                        {
                            if (assign.Left is MemberAccessExpressionSyntax maesLeft && maesLeft.Name.Identifier.Text == "CommandText")
                            {
                                commandVarName = maesLeft.Expression.ToString(); // Запоминаем имя переменной команды
                                if (assign.Right is LiteralExpressionSyntax lit && lit.IsKind(SyntaxKind.StringLiteralExpression))
                                {
                                    string sql = lit.Token.ValueText.ToUpperInvariant();
                                    if (sql.Contains("CREATE PROCEDURE")) createProcFound = true;
                                    // Для вызова, CommandText будет именем процедуры
                                    else if (!sql.Contains(" ") && sql.Length > 0) // Просто имя, без SQL ключевых слов
                                    {
                                        // Проверяем, что после этого CommandType устанавливается в StoredProcedure
                                        var nextStatements = mainMethodNode.Body.Statements.SkipWhile(s => !s.Contains(assign)).Skip(1);
                                        foreach(var nextStmt in nextStatements)
                                        {
                                            if (nextStmt is ExpressionStatementSyntax ess && ess.Expression is AssignmentExpressionSyntax assignType)
                                            {
                                                if (assignType.Left.ToString() == commandVarName + ".CommandType" &&
                                                    assignType.Right.ToString().Contains("CommandType.StoredProcedure"))
                                                {
                                                    callProcFound = true;
                                                    break;
                                                }
                                            }
                                            if (callProcFound) break;
                                            // Если между присвоением CommandText и CommandType много кода, эта проверка может не сработать.
                                        }
                                    }
                                }
                            }
                        }
                        return createProcFound && callProcFound;
                    }
                },
                                new PracticeTask // EF Core - сложнее, т.к. много магии. Проверяем основные паттерны.
                {
                    TaskDescription = "8. Используйте Entity Framework Core для выборки данных.",
                    DetailedDescription = "Напишите код для выборки данных с помощью EF Core. Пример:\n// public class AppDbContext : DbContext { /* ... */ public DbSet<User> Users { get; set; } }\n// public class User { public int Id { get; set; } public string Name { get; set; } }\nusing var context = new AppDbContext();\nvar users = context.Users.ToList();",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        // 1. Ищем класс, наследующийся от DbContext
                        ClassDeclarationSyntax dbContextClass = null;
                        string dbContextClassName = null;
                        foreach (var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                        {
                            if (classDecl.BaseList != null &&
                                classDecl.BaseList.Types.Any(bt => bt.Type.ToString() == "DbContext" || semanticModel.GetTypeInfo(bt.Type).Type?.BaseType?.Name == "DbContext"))
                            {
                                dbContextClass = classDecl;
                                dbContextClassName = classDecl.Identifier.Text;
                                break;
                            }
                        }
                        if (dbContextClass == null) return false;

                        // 2. В этом DbContext ищем DbSet<T> (например, DbSet<User>)
                        bool dbSetFound = false;
                        string dbSetName = null; // Имя свойства DbSet, например Users
                        foreach(var member in dbContextClass.Members.OfType<PropertyDeclarationSyntax>())
                        {
                            if (member.Type is GenericNameSyntax gns && gns.Identifier.Text == "DbSet")
                            {
                                dbSetFound = true;
                                dbSetName = member.Identifier.Text;
                                break;
                            }
                        }
                        if (!dbSetFound || dbSetName == null) return false;

                        // 3. В Main: using var context = new AppDbContext();
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        string contextVarName = null;
                        var objectCreationsInMain = mainMethodNode.Body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                        foreach (var oc in objectCreationsInMain)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(oc).Type;
                            if (typeSymbol?.Name == dbContextClassName)
                            {
                                if (oc.Parent is EqualsValueClauseSyntax evc && evc.Parent is VariableDeclaratorSyntax vd)
                                {
                                    contextVarName = vd.Identifier.Text;
                                    break;
                                }
                            }
                        }
                        if (contextVarName == null) return false;

                        // 4. Ищем context.Users.ToList() или context.Users (любое LINQ выражение)
                        var memberAccessesInMain = mainMethodNode.Body.DescendantNodes().OfType<MemberAccessExpressionSyntax>();
                        foreach(var maesRootDbSetAccess in memberAccessesInMain) // maesRootDbSetAccess это context.Users
                        {
                            if (maesRootDbSetAccess.Expression.ToString() == contextVarName && maesRootDbSetAccess.Name.Identifier.Text == dbSetName)
                            {
                                // Проверяем, используется ли этот DbSet как начало цепочки LINQ или присваивается
                                var parentNode = maesRootDbSetAccess.Parent;

                                // Случай: var users = context.Users.ToList();
                                if (parentNode is InvocationExpressionSyntax invToList)
                                {
                                    if (invToList.Expression is MemberAccessExpressionSyntax maesToList && maesToList.Name.Identifier.Text == "ToList" && maesToList.Expression == maesRootDbSetAccess)
                                    {
                                        return true;
                                    }
                                }
                                // Случай: var query = context.Users.Where(...); или var users = context.Users;
                                else if (parentNode is EqualsValueClauseSyntax) // присвоение
                                {
                                    return true;
                                }
                                // Случай: context.Users.Where(...).ToList();
                                // Здесь maesRootDbSetAccess это context.Users, а parentNode будет MemberAccessExpressionSyntax для .Where
                                else if (parentNode is MemberAccessExpressionSyntax nextInChain)
                                {
                                    // Если есть следующий вызов в цепочке (Where, Select, etc.), то это использование
                                    // Можно более детально проверять всю цепочку, но для этой задачи само использование уже хорошо
                                    return true;
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask // Очень сложная для статической проверки без инструментов миграции
                {
                    TaskDescription = "9. Реализуйте миграции базы данных в EF Core.",
                    DetailedDescription = "Создайте класс контекста и примените миграции. Пример:\npublic class AppDbContext : DbContext\n{\n    public DbSet<User> Users { get; set; }\n    // protected override void OnConfiguring... (опционально для этой задачи)\n}\n// В Package Manager Console:\n// Add-Migration InitialCreate\n// Update-Database",
                    ValidationMethod = (assembly) => // Проверяем наличие DbContext и DbSet. Сами миграции проверить не можем.
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        // Ищем класс, наследующийся от DbContext
                        ClassDeclarationSyntax dbContextClass = null;
                        foreach (var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                        {
                            if (classDecl.BaseList != null &&
                                classDecl.BaseList.Types.Any(bt => bt.Type.ToString() == "DbContext" || semanticModel.GetTypeInfo(bt.Type).Type?.BaseType?.Name == "DbContext"))
                            {
                                dbContextClass = classDecl;
                                break;
                            }
                        }
                        if (dbContextClass == null) return false;

                        // В этом DbContext ищем хотя бы один DbSet<T>
                        bool dbSetFound = dbContextClass.Members.OfType<PropertyDeclarationSyntax>()
                            .Any(member => member.Type is GenericNameSyntax gns && gns.Identifier.Text == "DbSet");

                        return dbSetFound; // Наличие DbContext с DbSet - это максимум, что мы можем проверить статически для "реализации миграций"
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "10. Оптимизируйте запросы с помощью LINQ to Entities.",
                    DetailedDescription = "Напишите оптимизированные запросы с помощью LINQ to Entities. Пример:\nvar users = context.Users\n    .Where(u => u.Age > 18)\n    .OrderBy(u => u.Name)\n    .Select(u => new { u.Name, u.Email })\n    .ToList();",
                    ValidationMethod = (assembly) => // Проверяем цепочку LINQ вызовов: Where, OrderBy, Select, ToList
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // Ищем переменную DbContext (например, context.Users)
                        string dbSetAccessString = null; // Например, "context.Users"
                         var objectCreations = mainMethodNode.Body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                         foreach(var oc in objectCreations) // Находим context
                         {
                             var typeSymbol = semanticModel.GetTypeInfo(oc.Type).Type;
                             if (typeSymbol?.BaseType?.Name == "DbContext")
                             {
                                 string contextVarName = (oc.Parent as EqualsValueClauseSyntax)?.Parent.ToString().Split('=').First().Trim().Split(' ').Last();
                                 if (contextVarName != null)
                                 {
                                     // Ищем свойство DbSet в этом контексте (например, Users)
                                     var contextClassDecl = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(cd => cd.Identifier.Text == typeSymbol.Name);
                                     var dbSetProp = contextClassDecl?.Members.OfType<PropertyDeclarationSyntax>().FirstOrDefault(p => p.Type is GenericNameSyntax gns && gns.Identifier.Text == "DbSet");
                                     if (dbSetProp != null)
                                     {
                                         dbSetAccessString = $"{contextVarName}.{dbSetProp.Identifier.Text}";
                                         break;
                                     }
                                 }
                             }
                             if (dbSetAccessString != null) break;
                         }
                        if (dbSetAccessString == null) return false;

                        // Ищем цепочку вызовов
                        var invocations = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>();
                        foreach(var invToList in invocations)
                        {
                            if (!(invToList.Expression is MemberAccessExpressionSyntax maesToList && maesToList.Name.Identifier.Text == "ToList")) continue;

                            // maesToList.Expression должен быть .Select(...)
                            if (!(maesToList.Expression is InvocationExpressionSyntax invSelect &&
                                  invSelect.Expression is MemberAccessExpressionSyntax maesSelect && maesSelect.Name.Identifier.Text == "Select")) continue;
                            
                            // maesSelect.Expression должен быть .OrderBy(...) или .OrderByDescending(...)
                             if (!(invSelect.Expression is InvocationExpressionSyntax invOrderBy &&
                                  invOrderBy.Expression is MemberAccessExpressionSyntax maesOrderBy &&
                                  (maesOrderBy.Name.Identifier.Text == "OrderBy" || maesOrderBy.Name.Identifier.Text == "OrderByDescending"))) continue;

                            // maesOrderBy.Expression должен быть .Where(...)
                            if (!(invOrderBy.Expression is InvocationExpressionSyntax invWhere &&
                                  invWhere.Expression is MemberAccessExpressionSyntax maesWhere && maesWhere.Name.Identifier.Text == "Where")) continue;

                            // maesWhere.Expression должен быть dbSetAccessString (context.Users)
                            if (maesWhere.Expression.ToString() == dbSetAccessString)
                            {
                                // Проверяем, что в Select создается анонимный тип или DTO
                                if (invSelect.ArgumentList.Arguments.Count == 1 &&
                                    invSelect.ArgumentList.Arguments.First().Expression is LambdaExpressionSyntax lambdaSelect &&
                                    lambdaSelect.Body is AnonymousObjectCreationExpressionSyntax) // new { u.Name, u.Email }
                                {
                                    return true;
                                }
                            }
                        }
                        return false;
                    }
                }
            }
        },
                new Module
        {
            Title = "Делегаты и события",
            Description = "Научитесь использовать делегаты, анонимные методы и события в C#.",
            Tasks = new List<PracticeTask>
            {
                new PracticeTask
                {
                    TaskDescription = "1. Создайте делегат, который принимает два числа и возвращает их сумму.",
                    DetailedDescription = "Объявите делегат и создайте метод, соответствующий этому делегату. Пример:\ndelegate int MathOperation(int a, int b);\nstatic int Add(int a, int b) => a + b;\nMathOperation operation = Add;",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        // 1. Ищем объявление делегата: delegate int Name(int, int);
                        DelegateDeclarationSyntax delegateDecl = null;
                        string delegateName = null;
                        foreach(var dd in root.DescendantNodes().OfType<DelegateDeclarationSyntax>())
                        {
                            if (dd.ReturnType.ToString() == "int" && // или PredefinedTypeSyntax
                                dd.ParameterList.Parameters.Count == 2 &&
                                dd.ParameterList.Parameters.All(p => p.Type.ToString() == "int")) // или PredefinedTypeSyntax
                            {
                                delegateDecl = dd;
                                delegateName = dd.Identifier.Text;
                                break;
                            }
                        }
                        if (delegateDecl == null) return false;

                        // 2. Ищем метод, соответствующий сигнатуре делегата
                        MethodDeclarationSyntax matchingMethod = null;
                        string matchingMethodName = null;
                        // Ищем в классе Program или в любом другом классе, если пользователь создал свой
                        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                        foreach(var cd in classDeclarations)
                        {
                            foreach(var md in cd.Members.OfType<MethodDeclarationSyntax>())
                            {
                                if (md.ReturnType.ToString() == "int" &&
                                    md.ParameterList.Parameters.Count == 2 &&
                                    md.ParameterList.Parameters.All(p => p.Type.ToString() == "int") &&
                                    (md.Body != null || md.ExpressionBody != null)) // Должен иметь тело
                                {
                                    matchingMethod = md;
                                    matchingMethodName = md.Identifier.Text;
                                    break;
                                }
                            }
                            if (matchingMethod != null) break;
                        }
                        if (matchingMethod == null) return false;
                        
                        // 3. Ищем присвоение переменной типа делегата этому методу в Main
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach(var ld in localDeclarations)
                        {
                            if (ld.Declaration.Type.ToString() == delegateName) // Переменная типа нашего делегата
                            {
                                foreach(var variable in ld.Declaration.Variables)
                                {
                                    if (variable.Initializer?.Value.ToString() == matchingMethodName)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        var assignments = mainMethodNode.Body.DescendantNodes().OfType<AssignmentExpressionSyntax>();
                        foreach(var assign in assignments)
                        {
                            var leftSymbol = semanticModel.GetSymbolInfo(assign.Left).Symbol as ILocalSymbol;
                            if(leftSymbol?.Type?.Name == delegateName && assign.Right.ToString() == matchingMethodName)
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                },
                                new PracticeTask
                {
                    TaskDescription = "2. Создайте событие и подпишитесь на него.",
                    DetailedDescription = "Создайте класс с событием и подпишитесь на это событие. Пример:\npublic class Publisher\n{\n    public event Action<string> MessageSent;\n    public void SendMessage(string message)\n    {\n        MessageSent?.Invoke(message);\n    }\n}\nvar publisher = new Publisher();\npublisher.MessageSent += (msg) => Console.WriteLine(msg);",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        // 1. Ищем класс с событием (event)
                        ClassDeclarationSyntax publisherClass = null;
                        EventFieldDeclarationSyntax eventField = null;
                        string eventName = null;

                        foreach (var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                        {
                            eventField = classDecl.Members.OfType<EventFieldDeclarationSyntax>().FirstOrDefault();
                            if (eventField != null)
                            {
                                publisherClass = classDecl;
                                eventName = eventField.Declaration.Variables.First().Identifier.Text;
                                break;
                            }
                        }
                        if (publisherClass == null || eventField == null || eventName == null) return false;

                        // 2. Ищем в Main создание объекта этого класса
                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        string publisherVarName = null;
                        var objectCreations = mainMethodNode.Body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                        foreach (var oc in objectCreations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(oc).Type;
                            if (typeSymbol?.Name == publisherClass.Identifier.Text)
                            {
                                if (oc.Parent is EqualsValueClauseSyntax evc && evc.Parent is VariableDeclaratorSyntax vd)
                                {
                                    publisherVarName = vd.Identifier.Text;
                                    break;
                                }
                            }
                        }
                        if (publisherVarName == null) return false;

                        // 3. Ищем подписку на событие: publisherVarName.EventName += ...;
                        bool subscriptionFound = false;
                        var assignments = mainMethodNode.Body.DescendantNodes().OfType<AssignmentExpressionSyntax>();
                        foreach (var assign in assignments)
                        {
                            if (assign.IsKind(SyntaxKind.AddAssignmentExpression) && // +=
                                assign.Left is MemberAccessExpressionSyntax maes &&
                                maes.Expression.ToString() == publisherVarName &&
                                maes.Name.Identifier.Text == eventName)
                            {
                                if (assign.Right is ParenthesizedLambdaExpressionSyntax ||
                                    assign.Right is SimpleLambdaExpressionSyntax ||
                                    assign.Right is IdentifierNameSyntax ||
                                    assign.Right is AnonymousMethodExpressionSyntax)
                                {
                                    subscriptionFound = true;
                                    break;
                                }
                            }
                        }

                        // 4. Опционально: Проверяем вызов события (Invoke) в классе Publisher
                        bool invokeCalled = false;
                        var methodsInPublisher = publisherClass.Members.OfType<MethodDeclarationSyntax>();
                        foreach (var methodDecl in methodsInPublisher)
                        {
                            if (methodDecl.Body == null && methodDecl.ExpressionBody == null) continue;

                            var invocationsInMethod = (methodDecl.Body ?? (SyntaxNode)methodDecl.ExpressionBody.Expression) // For expression body, take the expression itself
                                .DescendantNodesAndSelf() // Include self for expression body
                                .OfType<InvocationExpressionSyntax>();

                            foreach (var invNode in invocationsInMethod)
                            {
                                // Проверяем invNode.Expression
                                // Это может быть MemberAccessExpressionSyntax (eventName.Invoke)
                                // или ConditionalAccessExpressionSyntax (eventName?.Invoke)
                                ExpressionSyntax invokedExpression = null;

                                if (invNode.Expression is MemberAccessExpressionSyntax maesInvoke && maesInvoke.Name.Identifier.Text == "Invoke")
                                {
                                    invokedExpression = maesInvoke.Expression; // Это должно быть `eventName`
                                }
                                else if (invNode.Expression is MemberBindingExpressionSyntax mbesInvoke) // Для eventName?.Invoke
                                {
                                     // mbesInvoke.Name.Identifier.Text == "Invoke"
                                     // (mbesInvoke.Parent as ConditionalAccessExpressionSyntax).Expression - это eventName
                                     if (mbesInvoke.Name.Identifier.Text == "Invoke" &&
                                         mbesInvoke.Parent is ConditionalAccessExpressionSyntax condAccess)
                                     {
                                         invokedExpression = condAccess.Expression;
                                     }
                                }

                                if (invokedExpression != null && invokedExpression.ToString() == eventName)
                                {
                                    invokeCalled = true;
                                    break;
                                }
                            }
                            if (invokeCalled) break;
                        }
                        
                        // Задание говорит "Создайте событие и подпишитесь на него."
                        // Вызов события (Invoke) - это хорошая практика, но не строгое требование для этой задачи.
                        // Поэтому, если подписка найдена, засчитываем. Если нужен и Invoke, то && invokeCalled
                        return subscriptionFound; // return subscriptionFound && invokeCalled; (если invoke обязателен)
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "3. Используйте анонимный метод с делегатом.",
                    DetailedDescription = "Создайте делегат с анонимным методом. Пример:\nAction<string> print = delegate(string msg)\n{\n    Console.WriteLine(msg);\n};\nprint(\"Hello\");",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        // SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;
                        
                        // Ищем присвоение переменной анонимного метода: var delegateVar = delegate(...) { ... };
                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach(var ld in localDeclarations)
                        {
                            foreach(var variable in ld.Declaration.Variables)
                            {
                                if (variable.Initializer?.Value is AnonymousMethodExpressionSyntax am)
                                {
                                    // Проверяем, что тип переменной - какой-то делегат (например, Action<string>)
                                    // или var, тогда тип выведется
                                    // Проверяем, что анонимный метод имеет параметры и тело
                                    if (am.ParameterList != null && am.Block.Statements.Count > 0)
                                    {
                                        // Опционально: проверить вызов этой переменной-делегата
                                        var delegateVarName = variable.Identifier.Text;
                                        bool invoked = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>()
                                            .Any(inv => inv.Expression.ToString() == delegateVarName);
                                        if(invoked) return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "4. Примените лямбда-выражение вместо делегата.",
                    DetailedDescription = "Используйте лямбда-выражение для создания делегата. Пример:\nFunc<int, int, int> multiply = (x, y) => x * y;",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach(var ld in localDeclarations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(ld.Declaration.Type).ConvertedType;
                            // Проверяем, что тип переменной - это Func или Action или кастомный делегат
                            if (typeSymbol is INamedTypeSymbol nts && (nts.Name.StartsWith("Func") || nts.Name.StartsWith("Action") || nts.TypeKind == TypeKind.Delegate))
                            {
                                foreach(var variable in ld.Declaration.Variables)
                                {
                                    if (variable.Initializer?.Value is LambdaExpressionSyntax lambda) // ParenthesizedLambdaExpressionSyntax или SimpleLambdaExpressionSyntax
                                    {
                                        // Проверяем, что лямбда имеет параметры и тело/выражение
                                        if ((lambda as ParenthesizedLambdaExpressionSyntax)?.ParameterList.Parameters.Count > 0 ||
                                            (lambda as SimpleLambdaExpressionSyntax)?.Parameter != null) // Для лямбды с одним параметром без скобок
                                        {
                                             if (lambda.Body != null) // Тело блока или выражение
                                             {
                                                 return true;
                                             }
                                        }
                                        // Лямбда без параметров тоже возможна (например, Action act = () => Console.WriteLine(); )
                                        else if ((lambda as ParenthesizedLambdaExpressionSyntax)?.ParameterList.Parameters.Count == 0 && lambda.Body != null)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "5. Создайте и вызовите generic делегат.",
                    DetailedDescription = "Создайте и используйте обобщенный делегат. Пример:\nFunc<string, int, string> format = (s, i) => $\"{s}: {i}\";\nstring result = format(\"Count\", 5);",
                    ValidationMethod = (assembly) => // Похоже на задачу 4, но акцент на generic (Func/Action)
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        string delegateVarName = null;
                        INamedTypeSymbol delegateType = null;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach(var ld in localDeclarations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(ld.Declaration.Type).ConvertedType;
                            if (typeSymbol is INamedTypeSymbol nts && nts.IsGenericType &&
                                (nts.Name.StartsWith("Func") || nts.Name.StartsWith("Action"))) // Это generic Func или Action
                            {
                                foreach(var variable in ld.Declaration.Variables)
                                {
                                    if (variable.Initializer?.Value is LambdaExpressionSyntax)
                                    {
                                        delegateVarName = variable.Identifier.Text;
                                        delegateType = nts;
                                        break;
                                    }
                                }
                            }
                            if (delegateVarName != null) break;
                        }
                        if (delegateVarName == null || delegateType == null) return false;

                        // Ищем вызов этого делегата
                        bool invoked = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>()
                            .Any(inv => inv.Expression.ToString() == delegateVarName &&
                                        inv.ArgumentList.Arguments.Count == delegateType.TypeArguments.Length - (delegateType.Name.StartsWith("Func") ? 1:0) ); // Func имеет на 1 type arg больше (возвращаемый тип)
                        
                        return invoked;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "6. Реализуйте multicast делегат.",
                    DetailedDescription = "Создайте делегат, который вызывает несколько методов. Пример:\nAction<string> log = Console.WriteLine;\nlog += (msg) => File.WriteAllText(\"log.txt\", msg);\nlog(\"Test message\");",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        string delegateVarName = null;
                        // Ищем первое присвоение делегату
                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach (var ld in localDeclarations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(ld.Declaration.Type).ConvertedType;
                            if (typeSymbol?.TypeKind == TypeKind.Delegate)
                            {
                                delegateVarName = ld.Declaration.Variables.FirstOrDefault()?.Identifier.Text;
                                break;
                            }
                        }
                        if (delegateVarName == null) return false;

                        // Ищем += для этого делегата
                        bool addAssignmentFound = false;
                        var assignments = mainMethodNode.Body.DescendantNodes().OfType<AssignmentExpressionSyntax>();
                        foreach(var assign in assignments)
                        {
                            if (assign.IsKind(SyntaxKind.AddAssignmentExpression) && // +=
                                assign.Left.ToString() == delegateVarName)
                            {
                                // Правая часть должна быть методом или лямбдой/анонимным методом
                                if (assign.Right is LambdaExpressionSyntax ||
                                    assign.Right is AnonymousMethodExpressionSyntax ||
                                    assign.Right is IdentifierNameSyntax || // Имя метода
                                    (assign.Right is MemberAccessExpressionSyntax maes && semanticModel.GetSymbolInfo(maes).Symbol is IMethodSymbol)) // Console.WriteLine
                                {
                                    addAssignmentFound = true;
                                    break;
                                }
                            }
                        }
                        return addAssignmentFound;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "7. Используйте встроенные делегаты (Action, Func, Predicate).",
                    DetailedDescription = "Примените встроенные делегаты .NET. Пример:\nAction<string> print = Console.WriteLine;\nFunc<int, int, int> add = (a, b) => a + b;\nPredicate<string> isLong = s => s.Length > 10;",
                    ValidationMethod = (assembly) => // Проверяем использование хотя бы одного из них
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach(var ld in localDeclarations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(ld.Declaration.Type).ConvertedType;
                            if (typeSymbol is INamedTypeSymbol nts)
                            {
                                if (nts.Name.StartsWith("Action") || nts.Name.StartsWith("Func") || nts.Name.StartsWith("Predicate"))
                                {
                                    // Проверяем, что он инициализирован (лямбдой, методом)
                                    if (ld.Declaration.Variables.Any(v => v.Initializer != null))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "8. Создайте event с кастомными аргументами.",
                    DetailedDescription = "Создайте событие с пользовательскими аргументами. Пример:\npublic class PriceChangedEventArgs : EventArgs\n{\n    public decimal OldPrice { get; }\n    public decimal NewPrice { get; }\n}\npublic class Stock\n{\n    public event EventHandler<PriceChangedEventArgs> PriceChanged;\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        // 1. Ищем класс, наследующийся от EventArgs (например, PriceChangedEventArgs)
                        ClassDeclarationSyntax eventArgsClass = null;
                        string eventArgsClassName = null;
                        foreach(var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                        {
                            if (classDecl.BaseList != null &&
                                classDecl.BaseList.Types.Any(bt => bt.Type.ToString() == "EventArgs" || semanticModel.GetTypeInfo(bt.Type).Type?.ToDisplayString() == "System.EventArgs"))
                            {
                                // Должен содержать хотя бы одно публичное свойство (get-only из примера)
                                if (classDecl.Members.OfType<PropertyDeclarationSyntax>().Any(p => p.Modifiers.Any(SyntaxKind.PublicKeyword) && p.AccessorList.Accessors.Any(a=>a.IsKind(SyntaxKind.GetAccessorDeclaration))))
                                {
                                    eventArgsClass = classDecl;
                                    eventArgsClassName = classDecl.Identifier.Text;
                                    break;
                                }
                            }
                        }
                        if (eventArgsClass == null) return false;

                        // 2. Ищем класс с событием типа EventHandler<CustomEventArgs>
                        foreach(var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                        {
                            if (classDecl == eventArgsClass) continue; // Не сам класс EventArgs

                            var eventField = classDecl.Members.OfType<EventFieldDeclarationSyntax>().FirstOrDefault();
                            if (eventField?.Declaration.Type is GenericNameSyntax gns)
                            {
                                // EventHandler<PriceChangedEventArgs>
                                if ((gns.Identifier.Text == "EventHandler" || gns.Identifier.Text == "System.EventHandler") &&
                                    gns.TypeArgumentList.Arguments.Count == 1 &&
                                    gns.TypeArgumentList.Arguments.First().ToString() == eventArgsClassName)
                                {
                                    return true;
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask // Эта задача сложная для полной статической проверки. Упрощаем.
                {
                    TaskDescription = "9. Реализуйте интерфейс IObservable для создания наблюдаемого объекта.",
                    DetailedDescription = "Создайте класс, реализующий IObservable<T>. Пример:\npublic class ObservableNumber : IObservable<int>\n{\n    private List<IObserver<int>> observers = new List<IObserver<int>>();\n    public IDisposable Subscribe(IObserver<int> observer)\n    {\n        observers.Add(observer);\n        return new Unsubscriber(observers, observer); // Unsubscriber - это отдельный класс\n    }\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        // Ищем класс, реализующий IObservable<T>
                        foreach(var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                        {
                            if (classDecl.BaseList != null)
                            {
                                foreach(var baseTypeSyntax in classDecl.BaseList.Types)
                                {
                                    if (baseTypeSyntax.Type is GenericNameSyntax gns &&
                                        (gns.Identifier.Text == "IObservable" || gns.Identifier.Text == "System.IObservable") &&
                                        gns.TypeArgumentList.Arguments.Count == 1)
                                    {
                                        // Найден класс, реализующий IObservable<T>
                                        // Проверяем наличие метода Subscribe(IObserver<T>)
                                        var typeArgString = gns.TypeArgumentList.Arguments.First().ToString();
                                        var subscribeMethod = classDecl.Members.OfType<MethodDeclarationSyntax>()
                                            .FirstOrDefault(md => md.Identifier.Text == "Subscribe" &&
                                                                 md.ParameterList.Parameters.Count == 1 &&
                                                                 md.ParameterList.Parameters.First().Type.ToString().Contains($"IObserver<{typeArgString}>") &&
                                                                 md.ReturnType.ToString().Contains("IDisposable"));
                                        if (subscribeMethod != null)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "10. Используйте event для реализации шаблона Publisher-Subscriber.",
                    DetailedDescription = "Реализуйте шаблон Publisher-Subscriber с использованием событий. Пример:\npublic class NewsPublisher\n{\n    public event Action<string> NewsPublished;\n    public void Publish(string news)\n    {\n        NewsPublished?.Invoke(news);\n    }\n}\npublic class NewsSubscriber\n{\n    public void Subscribe(NewsPublisher publisher)\n    {\n        publisher.NewsPublished += OnNewsPublished;\n    }\n    private void OnNewsPublished(string news)\n    {\n        Console.WriteLine(news);\n    }\n}",
                    ValidationMethod = (assembly) => // Похоже на задачу 2 + явный метод подписки у подписчика
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        // 1. Ищем класс Publisher с событием (например, NewsPublished)
                        ClassDeclarationSyntax publisherClass = null;
                        EventFieldDeclarationSyntax publisherEvent = null;
                        string publisherEventName = null;
                        foreach (var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                        {
                            // Ищем класс с событием и методом для вызова этого события (Publish)
                            var ev = classDecl.Members.OfType<EventFieldDeclarationSyntax>().FirstOrDefault();
                            var publishMethod = classDecl.Members.OfType<MethodDeclarationSyntax>()
                                .FirstOrDefault(md => md.Body?.DescendantNodes().OfType<InvocationExpressionSyntax>() // Ищет вызов события
                                    .Any(inv => inv.Expression.ToString().Contains(ev?.Declaration.Variables.FirstOrDefault()?.Identifier.Text ?? "___EVENTNOTFOUND___")) == true);

                            if (ev != null && publishMethod != null)
                            {
                                publisherClass = classDecl;
                                publisherEvent = ev;
                                publisherEventName = ev.Declaration.Variables.First().Identifier.Text;
                                break;
                            }
                        }
                        if (publisherClass == null || publisherEventName == null) return false;

                        // 2. Ищем класс Subscriber с методом Subscribe(PublisherType publisher)
                        ClassDeclarationSyntax subscriberClass = null;
                        MethodDeclarationSyntax subscribeMethodNode = null;
                        foreach(var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                        {
                            if (classDecl == publisherClass) continue;

                            foreach(var methodDecl in classDecl.Members.OfType<MethodDeclarationSyntax>())
                            {
                                // Метод Subscribe, принимающий объект типа Publisher
                                if (methodDecl.Identifier.Text.ToLowerInvariant().Contains("subscribe") && // Subscribe, AddSubscription, etc.
                                    methodDecl.ParameterList.Parameters.Count == 1 &&
                                    methodDecl.ParameterList.Parameters.First().Type.ToString() == publisherClass.Identifier.Text)
                                {
                                    // Внутри этого метода должна быть подписка на событие
                                    if (methodDecl.Body?.DescendantNodes().OfType<AssignmentExpressionSyntax>()
                                        .Any(assign => assign.IsKind(SyntaxKind.AddAssignmentExpression) && // +=
                                                       assign.Left is MemberAccessExpressionSyntax maes &&
                                                       maes.Expression.ToString() == methodDecl.ParameterList.Parameters.First().Identifier.Text && // publisher.
                                                       maes.Name.Identifier.Text == publisherEventName) == true) // .NewsPublished
                                    {
                                        subscriberClass = classDecl;
                                        subscribeMethodNode = methodDecl;
                                        break;
                                    }
                                }
                            }
                            if (subscriberClass != null) break;
                        }

                        return subscriberClass != null && subscribeMethodNode != null;
                    }
                }
            }
        },
                new Module
        {
            Title = "Асинхронность и многопоточность",
            Description = "Изучите ключевые конструкции асинхронного программирования в C#: async, await, Task и Thread.",
            Tasks = new List<PracticeTask>
            {
                new PracticeTask
                {
                    TaskDescription = "1. Создайте асинхронный метод, который возвращает Task.",
                    DetailedDescription = "Напишите асинхронный метод, который выполняет задержку. Пример:\npublic async Task DoWorkAsync()\n{\n    await Task.Delay(1000);\n    Console.WriteLine(\"Work done\");\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        // Ищем метод с модификатором async и возвращаемым типом Task
                        var asyncMethods = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                            .Where(md => md.Modifiers.Any(SyntaxKind.AsyncKeyword));

                        foreach (var method in asyncMethods)
                        {
                            var returnTypeSymbol = semanticModel.GetTypeInfo(method.ReturnType).Type;
                            if (returnTypeSymbol?.ToDisplayString() == "System.Threading.Tasks.Task") // Task, не Task<T>
                            {
                                // Проверяем наличие await Task.Delay(...) внутри
                                bool awaitDelayFound = method.Body?.DescendantNodes().OfType<AwaitExpressionSyntax>()
                                    .Any(aw => aw.Expression is InvocationExpressionSyntax inv &&
                                               inv.Expression.ToString() == "Task.Delay" && // или System.Threading.Tasks.Task.Delay
                                               inv.ArgumentList.Arguments.Count == 1 &&
                                               inv.ArgumentList.Arguments.First().Expression is LiteralExpressionSyntax) ?? false; // Задержка с литералом
                                
                                if (awaitDelayFound) return true;
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "2. Запустите задачу в новом потоке с использованием Thread.",
                    DetailedDescription = "Создайте и запустите новый поток. Пример:\n// using System.Threading;\nvar thread = new Thread(() =>\n{\n    Console.WriteLine(\"Running in separate thread\");\n});\nthread.Start();",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        string threadVarName = null;
                        // Ищем new Thread(...);
                        var objectCreations = mainMethodNode.Body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                        foreach (var oc in objectCreations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(oc).Type;
                            if (typeSymbol?.ToDisplayString() == "System.Threading.Thread")
                            {
                                // Аргумент конструктора должен быть ThreadStart или ParameterizedThreadStart (лямбда или метод)
                                if (oc.ArgumentList != null && oc.ArgumentList.Arguments.Count == 1 &&
                                    (oc.ArgumentList.Arguments.First().Expression is LambdaExpressionSyntax ||
                                     oc.ArgumentList.Arguments.First().Expression is IdentifierNameSyntax /*имя метода*/))
                                {
                                    if (oc.Parent is EqualsValueClauseSyntax evc && evc.Parent is VariableDeclaratorSyntax vd)
                                    {
                                        threadVarName = vd.Identifier.Text;
                                        break;
                                    }
                                }
                            }
                        }
                        if (threadVarName == null) return false;

                        // Ищем thread.Start();
                        bool startCalled = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>()
                            .Any(inv => inv.Expression is MemberAccessExpressionSyntax maes &&
                                        maes.Expression.ToString() == threadVarName &&
                                        maes.Name.Identifier.Text == "Start" &&
                                        inv.ArgumentList.Arguments.Count == 0);

                        return startCalled;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "3. Создайте метод, который использует async и await.",
                    DetailedDescription = "Напишите асинхронный метод с await. Пример:\n// using System.Net.Http;\npublic async Task<string> DownloadContentAsync(string url)\n{\n    using var client = new HttpClient();\n    return await client.GetStringAsync(url);\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        // Ищем метод с модификатором async
                        var asyncMethods = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                            .Where(md => md.Modifiers.Any(SyntaxKind.AsyncKeyword));

                        foreach (var method in asyncMethods)
                        {
                            // Проверяем, что в теле метода есть хотя бы один await
                            if (method.Body?.DescendantNodes().OfType<AwaitExpressionSyntax>().Any() == true ||
                                method.ExpressionBody?.Expression is AwaitExpressionSyntax) // для async Task<T> Method() => await ...;
                            {
                                // Проверяем, что возвращаемый тип Task или Task<T>
                                var returnTypeSymbol = semanticModel.GetTypeInfo(method.ReturnType).Type;
                                if (returnTypeSymbol?.ToDisplayString() == "System.Threading.Tasks.Task" ||
                                    (returnTypeSymbol is INamedTypeSymbol nts && nts.IsGenericType && nts.ConstructedFrom.ToDisplayString() == "System.Threading.Tasks.Task<TResult>"))
                                {
                                    // Дополнительно, как в примере: await client.GetStringAsync(url)
                                    bool specificAwaitFound = method.Body?.DescendantNodes().OfType<AwaitExpressionSyntax>()
                                        .Any(aw => aw.Expression is InvocationExpressionSyntax inv &&
                                                   inv.Expression is MemberAccessExpressionSyntax maes &&
                                                   (maes.Name.Identifier.Text.EndsWith("Async") || maes.Name.Identifier.Text == "ConfigureAwait") // GetStringAsync, ReadAsync, etc.
                                                   // semanticModel.GetSymbolInfo(maes.Expression).Symbol?.ContainingType?.Name == "HttpClient" // более точная проверка
                                                   ) ?? false;

                                    if (specificAwaitFound || method.ExpressionBody != null) // ExpressionBody уже проверен на Await
                                    {
                                        return true;
                                    } else if (method.Body?.DescendantNodes().OfType<AwaitExpressionSyntax>().Any() == true) {
                                        // Если просто есть await и async Task/Task<T> - засчитаем
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "4. Используйте Task.Run для выполнения CPU-bound операции.",
                    DetailedDescription = "Примените Task.Run для выполнения ресурсоемкой операции. Пример:\nawait Task.Run(() =>\n{\n    // CPU-intensive work\n    for(int i = 0; i < 1000000; i++);\n});",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        // SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // Ищем await Task.Run(...);
                        var awaitExpressions = mainMethodNode.Body.DescendantNodes().OfType<AwaitExpressionSyntax>();
                        foreach (var awaitExpr in awaitExpressions)
                        {
                            if (awaitExpr.Expression is InvocationExpressionSyntax inv &&
                                inv.Expression.ToString() == "Task.Run" && // Или System.Threading.Tasks.Task.Run
                                inv.ArgumentList.Arguments.Count == 1 &&
                                inv.ArgumentList.Arguments.First().Expression is LambdaExpressionSyntax lambda)
                            {
                                // Проверяем, что в лямбде есть какая-то работа (например, цикл for)
                                if (lambda.Block?.DescendantNodes().OfType<ForStatementSyntax>().Any() == true ||
                                    (lambda.Body is ForStatementSyntax)) // for в expression-bodied lambda (маловероятно, но возможно)
                                {
                                    return true;
                                }
                                // Если просто Task.Run с лямбдой - тоже хорошо
                                // return true; 
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "5. Реализуйте параллельное выполнение задач с Task.WhenAll.",
                    DetailedDescription = "Используйте Task.WhenAll для параллельного выполнения. Пример:\nvar task1 = Task.Delay(1000);\nvar task2 = Task.Delay(2000);\nawait Task.WhenAll(task1, task2);",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        // SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // 1. Ищем объявление хотя бы двух Task-ов
                        int taskCount = 0;
                        List<string> taskVarNames = new List<string>();
                        var localDeclarations = mainMethodNode.Body.DescendantNodes().OfType<LocalDeclarationStatementSyntax>();
                        foreach(var ld in localDeclarations)
                        {
                            var typeName = ld.Declaration.Type.ToString();
                            if (typeName == "Task" || typeName.StartsWith("Task<") || typeName.Contains("System.Threading.Tasks.Task"))
                            {
                                taskCount++;
                                taskVarNames.AddRange(ld.Declaration.Variables.Select(v => v.Identifier.Text));
                            }
                        }
                        if (taskCount < 1 && taskVarNames.Count < 1) // Достаточно одной задачи для Task.WhenAll (хотя в примере 2)
                        {
                             // Если задачи не объявлены как переменные, а передаются напрямую в WhenAll: await Task.WhenAll(Task.Run(...), Task.Run(...))
                             // это сложнее валидировать без семантики для Task.Run.
                             // Пока ограничимся поиском переменных Task.
                        }


                        // 2. Ищем await Task.WhenAll(task1, task2, ...);
                        var awaitExpressions = mainMethodNode.Body.DescendantNodes().OfType<AwaitExpressionSyntax>();
                        foreach (var awaitExpr in awaitExpressions)
                        {
                            if (awaitExpr.Expression is InvocationExpressionSyntax inv &&
                                inv.Expression.ToString() == "Task.WhenAll" && // Или System.Threading.Tasks.Task.WhenAll
                                inv.ArgumentList.Arguments.Count >= 1) // Хотя бы одна задача
                            {
                                // Проверяем, что аргументы - это наши Task переменные или Task.Run/Task.Delay
                                bool allArgsAreTasks = true;
                                foreach(var arg in inv.ArgumentList.Arguments)
                                {
                                    if (taskVarNames.Contains(arg.Expression.ToString())) continue; // Переменная Task
                                    if (arg.Expression is InvocationExpressionSyntax argInv &&
                                        (argInv.Expression.ToString() == "Task.Run" || argInv.Expression.ToString() == "Task.Delay")) continue; // Task.Run или Task.Delay

                                    allArgsAreTasks = false; break;
                                }
                                if(allArgsAreTasks) return true;
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "6. Обработайте исключения в асинхронном коде.",
                    DetailedDescription = "Напишите try-catch для асинхронного кода. Пример:\ntry\n{\n    await SomeAsyncMethod();\n}\ncatch(Exception ex)\n{\n    Console.WriteLine(ex.Message);\n}",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        // SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Modifiers.Any(SyntaxKind.AsyncKeyword) || // Ищем в async Main
                                           (m.Identifier.Text == "Main" && m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program")); // Или в обычном Main
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        var tryStatements = mainMethodNode.Body.DescendantNodes().OfType<TryStatementSyntax>();
                        foreach (var tryStmt in tryStatements)
                        {
                            // 1. Проверяем, что в try блоке есть await
                            bool awaitInTry = tryStmt.Block.DescendantNodes().OfType<AwaitExpressionSyntax>().Any();
                            if (!awaitInTry) continue;

                            // 2. Проверяем, что есть хотя бы один catch блок
                            if (tryStmt.Catches.Count > 0)
                            {
                                // Можно проверить, что catch ловит Exception или специфичное исключение
                                return true;
                            }
                        }
                        return false;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "7. Используйте CancellationToken для отмены задачи.",
                    DetailedDescription = "Примените CancellationToken для отмены асинхронной операции. Пример:\nvar cts = new CancellationTokenSource();\nvar task = Task.Run(() =>\n{\n    while(!cts.Token.IsCancellationRequested)\n    {\n        // работа\n    }\n}, cts.Token);\ncts.Cancel();",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // 1. Ищем var cts = new CancellationTokenSource();
                        string ctsVarName = null;
                        var objectCreations = mainMethodNode.Body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                        foreach(var oc in objectCreations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(oc).Type;
                            if (typeSymbol?.ToDisplayString() == "System.Threading.CancellationTokenSource")
                            {
                                if (oc.Parent is EqualsValueClauseSyntax evc && evc.Parent is VariableDeclaratorSyntax vd)
                                {
                                    ctsVarName = vd.Identifier.Text;
                                    break;
                                }
                            }
                        }
                        if (ctsVarName == null) return false;

                        // 2. Ищем Task.Run(..., cts.Token)
                        bool taskRunWithToken = false;
                        var taskRunInvocations = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>()
                            .Where(inv => inv.Expression.ToString() == "Task.Run");
                        foreach(var taskRunInv in taskRunInvocations)
                        {
                            if (taskRunInv.ArgumentList.Arguments.Count >= 2) // lambda + token
                            {
                                var secondArg = taskRunInv.ArgumentList.Arguments[1].Expression;
                                if (secondArg is MemberAccessExpressionSyntax maesToken &&
                                    maesToken.Expression.ToString() == ctsVarName &&
                                    maesToken.Name.Identifier.Text == "Token")
                                {
                                    // Внутри лямбды Task.Run ищем while(!cts.Token.IsCancellationRequested)
                                    if (taskRunInv.ArgumentList.Arguments[0].Expression is LambdaExpressionSyntax lambda)
                                    {
                                        var whileLoops = (lambda.Body as BlockSyntax ?? SyntaxFactory.Block(lambda.Body as StatementSyntax)) // Обернуть если не блок
                                            .DescendantNodes().OfType<WhileStatementSyntax>();
                                        foreach(var whileLoop in whileLoops)
                                        {
                                            if (whileLoop.Condition is PrefixUnaryExpressionSyntax pues &&
                                                pues.IsKind(SyntaxKind.LogicalNotExpression) &&
                                                pues.Operand is MemberAccessExpressionSyntax maesCancelReq &&
                                                maesCancelReq.Name.Identifier.Text == "IsCancellationRequested" &&
                                                maesCancelReq.Expression is MemberAccessExpressionSyntax maesTokenInner &&
                                                maesTokenInner.Expression.ToString() == ctsVarName && // или ссылка на токен, переданный в лямбду
                                                maesTokenInner.Name.Identifier.Text == "Token")
                                            {
                                                taskRunWithToken = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (taskRunWithToken) break;
                        }
                        if (!taskRunWithToken) return false;

                        // 3. Ищем cts.Cancel();
                        bool cancelCalled = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>()
                            .Any(inv => inv.Expression is MemberAccessExpressionSyntax maes &&
                                        maes.Expression.ToString() == ctsVarName &&
                                        maes.Name.Identifier.Text == "Cancel");

                        return cancelCalled;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "8. Создайте и используйте TaskCompletionSource.",
                    DetailedDescription = "Примените TaskCompletionSource для ручного управления Task. Пример:\nvar tcs = new TaskCompletionSource<int>();\ntcs.SetResult(42);\nint result = await tcs.Task;",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // 1. Ищем var tcs = new TaskCompletionSource<T>();
                        string tcsVarName = null;
                        var objectCreations = mainMethodNode.Body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                        foreach(var oc in objectCreations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(oc).Type;
                            if (typeSymbol is INamedTypeSymbol nts && nts.Name == "TaskCompletionSource" && nts.IsGenericType)
                            {
                                if (oc.Parent is EqualsValueClauseSyntax evc && evc.Parent is VariableDeclaratorSyntax vd)
                                {
                                    tcsVarName = vd.Identifier.Text;
                                    break;
                                }
                            }
                        }
                        if (tcsVarName == null) return false;

                        // 2. Ищем tcs.SetResult(...) или SetException(...) или SetCanceled(...)
                        bool setResultCalled = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>()
                            .Any(inv => inv.Expression is MemberAccessExpressionSyntax maes &&
                                        maes.Expression.ToString() == tcsVarName &&
                                        (maes.Name.Identifier.Text == "SetResult" ||
                                         maes.Name.Identifier.Text == "TrySetResult" || // Также засчитаем TrySet...
                                         maes.Name.Identifier.Text == "SetException" ||
                                         maes.Name.Identifier.Text == "TrySetException" ||
                                         maes.Name.Identifier.Text == "SetCanceled" ||
                                         maes.Name.Identifier.Text == "TrySetCanceled"));
                        if (!setResultCalled) return false;

                        // 3. Ищем await tcs.Task;
                        bool awaitTcsTask = mainMethodNode.Body.DescendantNodes().OfType<AwaitExpressionSyntax>()
                            .Any(aw => aw.Expression is MemberAccessExpressionSyntax maes &&
                                       maes.Expression.ToString() == tcsVarName &&
                                       maes.Name.Identifier.Text == "Task");

                        return awaitTcsTask;
                    }
                },
                new PracticeTask // Сложно валидировать корректность работы, только структуру
                {
                    TaskDescription = "9. Реализуйте Producer-Consumer pattern с BlockingCollection.",
                    DetailedDescription = "Создайте производителя и потребителя с BlockingCollection. Пример:\nvar queue = new BlockingCollection<int>();\n// Producer\nTask.Run(() =>\n{\n    for(int i = 0; i < 10; i++)\n    {\n        queue.Add(i);\n    }\n    queue.CompleteAdding();\n});\n// Consumer\nTask.Run(() =>\n{\n    foreach(var item in queue.GetConsumingEnumerable())\n    {\n        Console.WriteLine(item);\n    }\n});",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // 1. Ищем var queue = new BlockingCollection<T>();
                        string queueVarName = null;
                        var objectCreations = mainMethodNode.Body.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
                        foreach(var oc in objectCreations)
                        {
                            var typeSymbol = semanticModel.GetTypeInfo(oc).Type;
                            if (typeSymbol is INamedTypeSymbol nts && nts.Name == "BlockingCollection" && nts.IsGenericType)
                            {
                                if (oc.Parent is EqualsValueClauseSyntax evc && evc.Parent is VariableDeclaratorSyntax vd)
                                {
                                    queueVarName = vd.Identifier.Text;
                                    break;
                                }
                            }
                        }
                        if (queueVarName == null) return false;

                        bool producerFound = false;
                        bool consumerFound = false;

                        // 2. Ищем Task.Run для Producer и Consumer
                        var taskRunInvocations = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>()
                            .Where(inv => inv.Expression.ToString() == "Task.Run" &&
                                          inv.ArgumentList.Arguments.Count == 1 &&
                                          inv.ArgumentList.Arguments.First().Expression is LambdaExpressionSyntax);

                        foreach(var taskRunInv in taskRunInvocations)
                        {
                            var lambda = taskRunInv.ArgumentList.Arguments.First().Expression as LambdaExpressionSyntax;
                            var lambdaBody = lambda.Block ?? SyntaxFactory.Block(lambda.Body as StatementSyntax);

                            // Producer: queue.Add() и queue.CompleteAdding()
                            bool addCalled = lambdaBody.DescendantNodes().OfType<InvocationExpressionSyntax>()
                                .Any(inv => inv.Expression.ToString() == queueVarName + ".Add");
                            bool completeAddingCalled = lambdaBody.DescendantNodes().OfType<InvocationExpressionSyntax>()
                                .Any(inv => inv.Expression.ToString() == queueVarName + ".CompleteAdding");
                            if (addCalled && completeAddingCalled) producerFound = true;

                            // Consumer: foreach(var item in queue.GetConsumingEnumerable())
                            bool consumingForeach = lambdaBody.DescendantNodes().OfType<ForEachStatementSyntax>()
                                .Any(fe => fe.Expression is InvocationExpressionSyntax invFe &&
                                           invFe.Expression.ToString() == queueVarName + ".GetConsumingEnumerable");
                            if (consumingForeach) consumerFound = true;
                        }

                        return producerFound && consumerFound;
                    }
                },
                new PracticeTask
                {
                    TaskDescription = "10. Используйте Parallel.For для параллельной обработки.",
                    DetailedDescription = "Примените Parallel.For для параллельного выполнения цикла. Пример:\nParallel.For(0, 10, i =>\n{\n    Console.WriteLine(i);\n});",
                    ValidationMethod = (assembly) =>
                    {
                        var (syntaxTree, compilation) = RoslynContext.GetCurrent();
                        if (syntaxTree == null || compilation == null) return false;
                        // SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
                        var root = syntaxTree.GetRoot();

                        var mainMethodNode = root.DescendantNodes()
                            .OfType<MethodDeclarationSyntax>()
                            .FirstOrDefault(m => m.Identifier.Text == "Main" &&
                                           m.Parent is ClassDeclarationSyntax c && c.Identifier.Text == "Program");
                        if (mainMethodNode == null || mainMethodNode.Body == null) return false;

                        // Ищем Parallel.For(...)
                        var invocations = mainMethodNode.Body.DescendantNodes().OfType<InvocationExpressionSyntax>();
                        foreach(var inv in invocations)
                        {
                            if (inv.Expression.ToString() == "Parallel.For" || inv.Expression.ToString() == "System.Threading.Tasks.Parallel.For")
                            {
                                // Ожидаем 3 аргумента: fromInclusive, toExclusive, bodyAction
                                if (inv.ArgumentList.Arguments.Count == 3 &&
                                    inv.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax && // from
                                    inv.ArgumentList.Arguments[1].Expression is LiteralExpressionSyntax && // to
                                    inv.ArgumentList.Arguments[2].Expression is LambdaExpressionSyntax)    // body
                                {
                                    return true;
                                }
                            }
                        }
                        return false;
                    }
                }
            }
     }
    }; 

            foreach (var module in Modules)
        ModulesList.Items.Add(module);
}

        private void OpenModule_Click(object sender, RoutedEventArgs e)
        {
            // DataContext кнопки уже будет Module благодаря DataTemplate
            if (sender is Button button && button.DataContext is Module module)
            {
                var moduleWindow = new ModulePage(module);
                moduleWindow.Owner = this;
                moduleWindow.ShowDialog();

                // После закрытия ModulePage, сохраняем прогресс и обновляем UI
                // Это событие также обрабатывается через OnProgressShouldBeSavedAndUpdated,
                // но вызов здесь гарантирует сохранение, если окно было просто закрыто
                // без фактического изменения прогресса через "Проверить".
                SaveProgress();
                UpdateOverallProgress();
                foreach (var m in Modules)
                {
                    m.RaiseIsCompletedChanged();
                }
            }
            else
            {
                // ... (старая логика для подстраховки, если есть) ...
                if (sender is Button oldButton)
                {
                    Grid grid = VisualTreeHelper.GetParent(oldButton) as Grid;
                    if (grid?.DataContext is Module moduleFromGrid)
                    {
                        var moduleWindow = new ModulePage(moduleFromGrid);
                        moduleWindow.Owner = this;
                        moduleWindow.ShowDialog();
                        SaveProgress();
                        UpdateOverallProgress();
                        foreach (var m in Modules)
                        {
                            m.RaiseIsCompletedChanged();
                        }
                        return;
                    }
                }
                MessageBox.Show("Не удалось найти модуль.");
            }
        }


        private void UpdateOverallProgress()
        {
            if (Modules == null || !Modules.Any())
            {
                MainProgressBar.Value = 0;
                return;
            }

            int completedModules = Modules.Count(m => m.IsCompleted);
            double progress = Modules.Count > 0 ? (double)completedModules / Modules.Count * 100 : 0;

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
                ProgressManager.SaveProgress(Modules); // Используем ProgressManager
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
                ProgressManager.LoadProgress(Modules); // Используем ProgressManager
                // Обновление UI после загрузки прогресса будет выполнено в конструкторе MainWindow
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке прогресса: " + ex.Message);
            }
        }

    }
}