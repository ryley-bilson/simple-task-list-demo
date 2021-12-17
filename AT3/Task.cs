using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace AT3
{
    public enum TaskPriority
    {
        Major,
        High,
        Moderate,
        Low,
        Minor
    }
    [ContentProperty("ID")]
    [Serializable()]
    public class Task
    {
        private int id;
        private DateTime dueDateTime;
        private String title;
        private String details;
        private TaskPriority priority;
        public Task(int _id, DateTime _dueDateTime, String _title,
                    String _details, TaskPriority _priority)
        {
            id = _id;
            dueDateTime = _dueDateTime;
            title = _title;
            details = _details;
            priority = _priority;
        }
        public int ID
        {
            get => id;
            set => id = value;
        }
        public DateTime DueDateTime
        {
            get => dueDateTime;
            set => dueDateTime = value;
        }
        public String DueDateTimeString()
        {
            return DueDateTime.ToString("dd/MM/yyyy HH:mm");
        }
        public string Title
        {
            get => title; 
            set => title = value;
        }
        public string Details
        {
            get => details;
            set => details = value;
        }
        public TaskPriority Priority
        {
            get => priority;
            set => priority = value;
        }
    }
}
