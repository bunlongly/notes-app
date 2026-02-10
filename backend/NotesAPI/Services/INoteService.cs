using NotesAPI.Models;

namespace NotesAPI.Services;

public interface INoteService
{
    Task<IEnumerable<Note>> GetAllByUserIdAsync(int userId);    Task<PaginatedResult<Note>> GetPaginatedByUserIdAsync(int userId, int page = 1, int pageSize = 10);    Task<Note?> GetByIdAsync(int id, int userId);
    Task<Note> CreateAsync(CreateNoteRequest request, int userId);
    Task<Note> UpdateAsync(int id, UpdateNoteRequest request, int userId);
    Task<bool> DeleteAsync(int id, int userId);
}
