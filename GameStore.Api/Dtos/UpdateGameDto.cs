using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class UpdateGameDto(
  [StringLength(50)] string Name,
  [StringLength(20)] string Genre,
  [Range(0, 100)] decimal? Price,
  DateOnly? ReleaseDate
);