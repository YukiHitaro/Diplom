using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CSharpTrainer.ProgressManager;

namespace CSharpTrainer
{
    public class UserProgress
    {
        public Dictionary<int, List<TaskProgressData>> ModuleTaskProgress { get; set; } = new();
    }
}
