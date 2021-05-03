
using Common.Interfaces;

namespace Common.Events
{
    public class StudentUpdated : IEvent
    {
        public string StudentId { get; set; }

        public StudentUpdated(string studentId)
        {
            StudentId = studentId;
        }
    }
}
