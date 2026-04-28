public class ImportCreateDto
{
    public int UserId { get; set; }
    public List<ImportDetailCreateDto> Details { get; set; } = new();
}