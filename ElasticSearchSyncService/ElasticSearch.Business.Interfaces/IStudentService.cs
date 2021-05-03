using System;
using ElasticSearch.Business.Models;

namespace ElasticSearch.Business.Interfaces
{
    public interface IStudentService
    {
        StudentModel Get(string id);
    }
}
