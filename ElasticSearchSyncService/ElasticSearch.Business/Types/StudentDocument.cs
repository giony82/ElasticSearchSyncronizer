namespace Common.Elastic.Types
{
    public class StudentDocument:DocumentBase
    {        
        public string Name { get; set; }

        public StudentAddressDocument Address { get; set; }
    }
}
