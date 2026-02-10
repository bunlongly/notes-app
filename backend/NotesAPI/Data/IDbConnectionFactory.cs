using System.Data;

namespace NotesAPI.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
