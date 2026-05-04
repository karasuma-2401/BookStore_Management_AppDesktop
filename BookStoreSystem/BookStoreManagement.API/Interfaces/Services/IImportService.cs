namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IImportService
    {
        Task<ImportResponseDto> CreateImport(ImportCreateDto dto, int userId);

        Task<IEnumerable<ImportResponseDto>> GetImports();

        Task<ImportResponseDto?> GetImportById(int id);
    }
}