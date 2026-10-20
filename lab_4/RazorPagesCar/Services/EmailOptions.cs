namespace RazorPagesCar.Services;

public class EmailOptions
{
    public string Host { get; set; } = "smtp.example.com";
    public int Port { get; set; } = 587;
    public string User { get; set; } = "";
    public string Password { get; set; } = "";
    public string From { get; set; } = "support@example.com";
    public string FromName { get; set; } = "CarCatalog Support";
    public bool UseSsl { get; set; } = true;
}
