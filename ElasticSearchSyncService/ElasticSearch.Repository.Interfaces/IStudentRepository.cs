using System;
using ElasticSearch.Repository.Entities;

namespace ElasticSearch.Repository.Interfaces
{
    public interface IStudentRepository
    {
        StudentEntity Get(string id);
    }        
}
