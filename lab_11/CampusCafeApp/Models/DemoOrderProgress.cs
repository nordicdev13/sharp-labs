namespace CampusCafe.Models;

public static class DemoOrderProgress
{
    public static string GetStatus(DateTime createdUtc, DateTime nowUtc)
    {
        var age = nowUtc - createdUtc;
        return age.TotalSeconds switch
        {
            < 15 => "Прийнято",
            < 30 => "Готується",
            _ => "Готове до отримання"
        };
    }
    public static bool IsComplete(DateTime createdUtc, DateTime nowUtc)
        => (nowUtc - createdUtc).TotalSeconds >= 30;
}
