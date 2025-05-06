using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTrainer
{
    public class Module
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public List<PracticeTask> Tasks { get; set; }

        public PracticeTask GetRandomTask()
        {
            if (Tasks == null || Tasks.Count == 0) return null;
            Random rnd = new Random();
            return Tasks[rnd.Next(Tasks.Count)];
        }

        public bool IsCompleted => Tasks != null && Tasks.All(t => t.IsCompleted);
    }


}
