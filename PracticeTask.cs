using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTrainer
{
    public class PracticeTask
    {
        public string TaskDescription { get; set; }
        public Func<Assembly, bool> ValidationMethod { get; set; }

        public bool IsCompleted { get; set; } = false; 
    }


}
