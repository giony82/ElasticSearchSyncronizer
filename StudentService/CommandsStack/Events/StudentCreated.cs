
using Common.Interfaces;

namespace StudentService.Business.Events
{
    /// <summary>
    /// Event triggered when students have been created.
    /// </summary>
    public class StudentCreated : IEvent
    {
        public string StudentId { get; set; }

        public StudentCreated(string studentId)
        {
            StudentId = studentId;
        }
    }
}
