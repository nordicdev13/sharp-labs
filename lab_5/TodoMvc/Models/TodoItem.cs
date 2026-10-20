using System;
using System.ComponentModel.DataAnnotations;

namespace TodoMvc.Models;

public enum Priority
{
    [Display(Name = "Низький")]
    Low = 0,
    [Display(Name = "Середній")]
    Medium = 1,
    [Display(Name = "Високий")]
    High = 2
}

public class TodoItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле Назва є обов'язковим")]
    [StringLength(100, ErrorMessage = "Назва не може бути довшою за 100 символів")]
    [Display(Name = "Назва")]
    public string Title { get; set; } = "";

    [Display(Name = "Опис")]
    public string? Description { get; set; }

    [Display(Name = "Готово?")]
    public bool IsDone { get; set; }

    [Display(Name = "Дедлайн")]
    [DataType(DataType.Date)]
    public DateTime? DueAt { get; set; }

    [Display(Name = "Пріоритет")]
    public Priority Priority { get; set; } = Priority.Medium;

    [Display(Name = "Створено")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string PriorityDisplay => Priority switch
    {
        Priority.Low => "Низький",
        Priority.Medium => "Середній",
        Priority.High => "Високий",
        _ => Priority.ToString()
    };
}
