﻿#pragma checksum "..\..\ModulePage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "661DB026583C65C79DAB4E2FFFA9B82C249A4D5AEC295543C9F4A643AE1E0D68"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace CSharpTrainer {
    
    
    /// <summary>
    /// ModulePage
    /// </summary>
    public partial class ModulePage : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\ModulePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox TaskList;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\ModulePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TitleBlock;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\ModulePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ResetModuleButton;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\ModulePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar ModuleProgressBar;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\ModulePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock DescriptionBlock;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\ModulePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TaskBlock;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\ModulePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock HintText;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\ModulePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border DetailedDescriptionBorder;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\ModulePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock DetailedDescriptionBlock;
        
        #line default
        #line hidden
        
        
        #line 107 "..\..\ModulePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox AnswerBox;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\ModulePage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button StartTaskButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CSharpTrainer;component/modulepage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ModulePage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.TaskList = ((System.Windows.Controls.ListBox)(target));
            
            #line 18 "..\..\ModulePage.xaml"
            this.TaskList.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.TaskList_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.TitleBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.ResetModuleButton = ((System.Windows.Controls.Button)(target));
            
            #line 55 "..\..\ModulePage.xaml"
            this.ResetModuleButton.Click += new System.Windows.RoutedEventHandler(this.ResetModuleButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ModuleProgressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 5:
            this.DescriptionBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.TaskBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.HintText = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.DetailedDescriptionBorder = ((System.Windows.Controls.Border)(target));
            return;
            case 9:
            this.DetailedDescriptionBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.AnswerBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 111 "..\..\ModulePage.xaml"
            this.AnswerBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.AnswerBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 11:
            this.StartTaskButton = ((System.Windows.Controls.Button)(target));
            
            #line 117 "..\..\ModulePage.xaml"
            this.StartTaskButton.Click += new System.Windows.RoutedEventHandler(this.StartTaskButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

