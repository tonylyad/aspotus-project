using System.ComponentModel.DataAnnotations;

namespace Aspotus.Catalog.Api.Models.Requests;

/// <summary>
/// Запрос на создание нового автомобиля.
/// </summary>
public class CreateCarRequest
{
    /// <summary>
    /// Идентификатор марки автомобиля.
    /// </summary>
    [Required(ErrorMessage = "Марка автомобиля обязательна.")]
    public Guid BrandId { get; set; }

    /// <summary>
    /// Идентификатор модели автомобиля.
    /// </summary>
    [Required(ErrorMessage = "Модель автомобиля обязательна.")]
    public Guid ModelId { get; set; }

    /// <summary>
    /// Идентификатор поколения автомобиля.
    /// </summary>
    [Required(ErrorMessage = "Поколение автомобиля обязательно.")]
    public Guid GenerationId { get; set; }

    /// <summary>
    /// Год выпуска автомобиля.
    /// </summary>
    [Range(1900, 3000, ErrorMessage = "Год выпуска должен быть в диапазоне от 1900 до 3000.")]
    public int Year { get; set; }

    /// <summary>
    /// Тип кузова автомобиля.
    /// </summary>
    [Required(ErrorMessage = "Тип кузова обязателен для заполнения.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Тип кузова должен содержать от 2 до 50 символов.")]
    public string BodyType { get; set; } = null!;

    /// <summary>
    /// Объём двигателя автомобиля.
    /// </summary>
    [Range(0.1, 20.0, ErrorMessage = "Объём двигателя должен быть в диапазоне от 0.1 до 20.0 литров.")]
    public decimal EngineVolume { get; set; }

    /// <summary>
    /// Тип топлива автомобиля.
    /// </summary>
    [Required(ErrorMessage = "Тип топлива обязателен для заполнения.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Тип топлива должен содержать от 2 до 50 символов.")]
    public string FuelType { get; set; } = null!;

    /// <summary>
    /// Тип коробки передач автомобиля.
    /// </summary>
    [Required(ErrorMessage = "Тип коробки передач обязателен для заполнения.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Тип коробки передач должен содержать от 2 до 50 символов.")]
    public string TransmissionType { get; set; } = null!;

    /// <summary>
    /// Тип привода автомобиля.
    /// </summary>
    [Required(ErrorMessage = "Тип привода обязателен для заполнения.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Тип привода должен содержать от 2 до 50 символов.")]
    public string DriveType { get; set; } = null!;
}