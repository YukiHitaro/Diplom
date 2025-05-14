using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;

namespace CSharpTrainer
{
    public static class RoslynContext
    {
        [ThreadStatic]
        private static SyntaxTree _currentSyntaxTree;
        [ThreadStatic]
        private static Compilation _currentCompilation;

        public static void Set(SyntaxTree tree, Compilation comp)
        {
            _currentSyntaxTree = tree;
            _currentCompilation = comp;
        }

        public static (SyntaxTree syntaxTree, Compilation compilation) GetCurrent() => (_currentSyntaxTree, _currentCompilation);

        public static void Clear()
        {
            _currentSyntaxTree = null;
            _currentCompilation = null;
        }
    }
}