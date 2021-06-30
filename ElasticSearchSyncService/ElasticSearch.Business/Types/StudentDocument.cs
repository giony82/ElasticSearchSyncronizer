using Common.Elastic.Types;

namespace ElasticSearch.Business.Types
{
    public class StudentDocument:DocumentBase
    {        
        public string Name { get; set; }

        public StudentAddressDocument Address { get; set; }

        public object DynamicContent { get; set; }
    }
}
