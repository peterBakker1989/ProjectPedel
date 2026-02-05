using System.Globalization;
using System.Text;
public static class CsvLogger
{
    private static readonly object _lock = new();
    private static readonly string _filePath = "../../../results/data.csv";

    public static void Log(params string[] values)
    {
        lock (_lock)
        {
            Directory.CreateDirectory("results");

            var line = string.Join(",",
                values.Select(v =>
                    $"\"{v.Replace("\"", "\"\"")}\""));

            File.AppendAllText(_filePath, line + Environment.NewLine, Encoding.UTF8);
        }
    }
}

