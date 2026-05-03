public class ImportResponseDto
{
    public int ImportId { get; set; }
    public DateTime ImportDate { get; set; }
    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;
    public List<ImportDetailResponseDto> Details { get; set; } = new();
}