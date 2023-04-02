namespace OpenAI.Models;

public class IndexViewModel
{
    public List<string> Messages { get; }

    public string SqlQuery { get; }

    public List<dynamic> Data { get; }

    public IndexViewModel()
    {
        Messages = new();
        SqlQuery = string.Empty;
        Data = new List<dynamic>();
    }

    public IndexViewModel(List<string> messages, string sqlQuery, List<dynamic> data)
    {
        Messages = messages;
        SqlQuery = sqlQuery;
        Data = data;
    }
}
