using System;

namespace team2backend.Services
{
    public interface IUdemyCourseService
    {
        Response GetResponse(string search, int page);

        Boolean HasResults(string search);
    }
}