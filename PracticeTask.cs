
using System;
using System.Reflection;

namespace CSharpTrainer
{
    public class PracticeTask : ObservableObject
    {
        public string TaskDescription { get; set; }
        public string DetailedDescription { get; set; }
        public Func<Assembly, bool> ValidationMethod { get; set; }

        private string _userCode = string.Empty;
        public string UserCode
        {
            get => _userCode;
            set => SetProperty(ref _userCode, value);
        }

        private bool _isCompleted = false;
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (SetProperty(ref _isCompleted, value))
                {
                    
                }
            }
        }
    }
}