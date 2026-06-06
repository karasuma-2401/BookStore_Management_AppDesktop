public class CustomerUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public required string Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}