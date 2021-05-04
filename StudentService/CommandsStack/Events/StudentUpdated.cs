
using Common.Interfaces;

namespace StudentService.Business.Events
{
    /// <summary>
    /// Event triggered when students have been created.
    /// </summary>
    public class StudentUpdated : IEvent
    {
        public string StudentId { get; set; }

        public StudentUpdated(string studentId)
        {
            StudentId = studentId;
        }
    }
}
