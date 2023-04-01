namespace OpenAI.Models;

public class IndexViewModel
{
    public List<string> Messages { get; }

    public string SqlQuery { get; }

    public List<Employee> Employees { get; }

    public IndexViewModel()
    {
        Messages = new();
        SqlQuery = string.Empty;
        Employees = new();
    }

    public IndexViewModel(List<string> messages, string sqlQuery, List<Employee> employees)
    {
        Messages = messages;
        SqlQuery = sqlQuery;
        Employees = employees;
    }
}
