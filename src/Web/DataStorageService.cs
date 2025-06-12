using System.Dynamic;
using System.Text.Json;

namespace AiuiWeb;

public class DataStorageService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DataStorageService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public List<dynamic>? Get()
    {
        var rowsJson = _httpContextAccessor.HttpContext!.Session.GetString("DataStorage");

        if (rowsJson is null)
        {
            return null;
        }

        var rows = JsonSerializer.Deserialize<List<ExpandoObject>>(rowsJson);

        if (rows is null)
        {
            return null;
        }

        return rows.Select(item => (dynamic)item).ToList();
    }

    public void Set(List<dynamic>? rows)
    {
        if (rows is null)
        {
            return;
        }

        var newRowsJson = JsonSerializer.Serialize(rows);

        _httpContextAccessor.HttpContext!.Session.SetString("DataStorage", newRowsJson);
    }
}
