using Dapper;
using NotesAPI.Data;
using NotesAPI.Models;

namespace NotesAPI.Services;

public class NoteService : INoteService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public NoteService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Note>> GetAllByUserIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Note>(
            "SELECT * FROM Notes WHERE UserId = @UserId ORDER BY UpdatedAt DESC",
            new { UserId = userId });
    }

    public async Task<PaginatedResult<Note>> GetPaginatedByUserIdAsync(int userId, int page = 1, int pageSize = 10)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        // Get total count
        var totalCount = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Notes WHERE UserId = @UserId",
            new { UserId = userId });
        
        // Get paginated data
        var offset = (page - 1) * pageSize;
        var notes = await connection.QueryAsync<Note>(
            "SELECT * FROM Notes WHERE UserId = @UserId ORDER BY UpdatedAt DESC LIMIT @PageSize OFFSET @Offset",
            new { UserId = userId, PageSize = pageSize, Offset = offset });
        
        return new PaginatedResult<Note>
        {
            Items = notes.ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Note?> GetByIdAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Note>(
            "SELECT * FROM Notes WHERE Id = @Id AND UserId = @UserId",
            new { Id = id, UserId = userId });
    }

    public async Task<Note> CreateAsync(CreateNoteRequest request, int userId)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new Exception("Title is required");
        }

        using var connection = _connectionFactory.CreateConnection();
        
        // Check if user already has a note with this title (unique per user)
        var existingNote = await connection.QueryFirstOrDefaultAsync<Note>(
            "SELECT * FROM Notes WHERE UserId = @UserId AND Title = @Title",
            new { UserId = userId, Title = request.Title });
        
        if (existingNote != null)
        {
            throw new Exception($"You already have a note with the title '{request.Title}'. Please use a different title.");
        }
        
        var now = DateTime.UtcNow;
        
        var sql = @"INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt) 
                    VALUES (@UserId, @Title, @Content, @CreatedAt, @UpdatedAt);
                    SELECT last_insert_rowid()";

        var noteId = await connection.ExecuteScalarAsync<int>(sql, new
        {
            UserId = userId,
            Title = request.Title,
            Content = request.Content ?? string.Empty,
            CreatedAt = now,
            UpdatedAt = now
        });

        return new Note
        {
            Id = noteId,
            UserId = userId,
            Title = request.Title,
            Content = request.Content ?? string.Empty,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public async Task<Note> UpdateAsync(int id, UpdateNoteRequest request, int userId)
    {
        var existingNote = await GetByIdAsync(id, userId);
        if (existingNote == null)
        {
            throw new Exception("Note not found");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new Exception("Title is required");
        }

        using var connection = _connectionFactory.CreateConnection();
        
        // Check if another note with this title exists (excluding current note)
        var duplicateNote = await connection.QueryFirstOrDefaultAsync<Note>(
            "SELECT * FROM Notes WHERE UserId = @UserId AND Title = @Title AND Id != @Id",
            new { UserId = userId, Title = request.Title, Id = id });
        
        if (duplicateNote != null)
        {
            throw new Exception($"You already have another note with the title '{request.Title}'. Please use a different title.");
        }
        
        var now = DateTime.UtcNow;
        
        var sql = @"UPDATE Notes 
                    SET Title = @Title, Content = @Content, UpdatedAt = @UpdatedAt 
                    WHERE Id = @Id AND UserId = @UserId";

        await connection.ExecuteAsync(sql, new
        {
            Id = id,
            UserId = userId,
            Title = request.Title,
            Content = request.Content ?? string.Empty,
            UpdatedAt = now
        });

        return new Note
        {
            Id = id,
            UserId = userId,
            Title = request.Title,
            Content = request.Content ?? string.Empty,
            CreatedAt = existingNote.CreatedAt,
            UpdatedAt = now
        };
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.ExecuteAsync(
            "DELETE FROM Notes WHERE Id = @Id AND UserId = @UserId",
            new { Id = id, UserId = userId });
        
        return result > 0;
    }
}
