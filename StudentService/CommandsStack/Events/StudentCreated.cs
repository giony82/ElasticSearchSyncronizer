
using Common.Interfaces;

namespace Common.Events
{
    public class StudentCreated : IEvent
    {
        public string StudentId { get; set; }

        public StudentCreated(string studentId)
        {
            StudentId = studentId;
        }
    }
}
