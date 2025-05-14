// Module.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel; // Добавить для INotifyPropertyChanged

namespace CSharpTrainer
{
    public class Module : ObservableObject // Наследуемся от ObservableObject
    {
        public int Id { get; set; } // Убедись, что Id уникален для каждого модуля при создании
        public string Title { get; set; }
        public string Description { get; set; }

        public List<PracticeTask> Tasks { get; set; } = new List<PracticeTask>(); // Инициализируем

        public PracticeTask GetRandomTask()
        {
            if (Tasks == null || Tasks.Count == 0) return null;
            Random rnd = new Random();
            return Tasks[rnd.Next(Tasks.Count)];
        }

        // Это свойство теперь будет автоматически вычисляться и уведомлять об изменениях,
        // если Tasks или IsCompleted у задач меняются.
        // Но для автоматического обновления UI при изменении IsCompleted задачи,
        // нужно, чтобы Module слушал изменения в своих задачах.
        // Пока мы будем обновлять его при выходе из ModulePage.
        // Для более "живого" обновления IsCompleted модуля, потребовалась бы более сложная логика.
        private bool _isCompletedCache; // Кэш для избежания лишних вычислений
        public bool IsCompleted
        {
            get
            {
                // Вычисляем при каждом запросе, если не кэшировано (или кэш невалиден)
                // Для простоты, UI MainWindow будет обновлять это при закрытии ModulePage
                return Tasks != null && Tasks.All(t => t.IsCompleted);
            }
        }

        // Метод для принудительного уведомления об изменении IsCompleted
        public void RaiseIsCompletedChanged()
        {
            OnPropertyChanged(nameof(IsCompleted));
        }


        // Добавим конструктор для удобства присвоения Id
        private static int _nextId = 1;
        public Module()
        {
            Id = _nextId++;
            // Подписываемся на изменение IsCompleted каждой задачи, чтобы обновить IsCompleted модуля
            // Но это нужно делать после того, как Tasks будет заполнен.
            // Это усложнение, пока оставим обновление IsCompleted модуля при выходе из ModulePage.
        }
    }
}