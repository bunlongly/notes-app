using Microsoft.Data.Sqlite;
using System.Data;
using Dapper;

namespace NotesAPI.Data;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
        InitializeDatabase();
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Create Users table
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Email TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL,
                FullName TEXT NOT NULL,
                CreatedAt TEXT NOT NULL
            )");

        // Create Notes table
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Notes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Title TEXT NOT NULL,
                Content TEXT,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL,
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            )");

        // Create RefreshTokens table for secure token management
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS RefreshTokens (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Token TEXT NOT NULL UNIQUE,
                ExpiresAt TEXT NOT NULL,
                CreatedAt TEXT NOT NULL,
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            )");

        // Create indexes for performance optimization
        connection.Execute("CREATE INDEX IF NOT EXISTS IX_Notes_UserId ON Notes(UserId)");
        connection.Execute("CREATE INDEX IF NOT EXISTS IX_Notes_UpdatedAt ON Notes(UpdatedAt DESC)");
        connection.Execute("CREATE INDEX IF NOT EXISTS IX_RefreshTokens_UserId ON RefreshTokens(UserId)");
        connection.Execute("CREATE INDEX IF NOT EXISTS IX_RefreshTokens_Token ON RefreshTokens(Token)");
    }
}
