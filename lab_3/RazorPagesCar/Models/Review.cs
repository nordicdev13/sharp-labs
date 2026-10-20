using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesCar.Models;

public class Review
{
    public int Id { get; set; }

    [Required]
    public int CarId { get; set; }

    // Identity користувача (з ApplicationDbContext)
    [Required, StringLength(450)]
    public string UserId { get; set; } = string.Empty;

    // Знімок відображуваного імені (email/username)
    [StringLength(128)]
    public string? ReviewerName { get; set; }

    [Range(1, 5, ErrorMessage = "Оцінка повинна бути від 1 до 5")]
    public int Rating { get; set; }

    [StringLength(2000, ErrorMessage = "Відгук не повинен перевищувати 2000 символів")]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Навігаційна властивість
    public Car? Car { get; set; }
}
