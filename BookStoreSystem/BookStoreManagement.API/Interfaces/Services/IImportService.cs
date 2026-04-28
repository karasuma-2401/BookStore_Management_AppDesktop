namespace BookStoreManagement.API.Interfaces.Services
{
    public interface IImportService
    {
        Task<ImportResponseDto> CreateImport(ImportCreateDto dto);

        Task<IEnumerable<ImportResponseDto>> GetImports();

        Task<ImportResponseDto?> GetImportById(int id);
    }
}