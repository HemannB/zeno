CREATE TABLE IF NOT EXISTS Projects (
    Id         INTEGER PRIMARY KEY AUTOINCREMENT,
    Name       TEXT    NOT NULL,
    Color      TEXT    NOT NULL DEFAULT '#6366F1',
    IsArchived INTEGER NOT NULL DEFAULT 0,
    CreatedAt  TEXT    NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS Tasks (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Title       TEXT    NOT NULL,
    Notes       TEXT,
    IsCompleted INTEGER NOT NULL DEFAULT 0,
    IsFavorite  INTEGER NOT NULL DEFAULT 0,
    Priority    INTEGER NOT NULL DEFAULT 1,
    Recurrence  INTEGER NOT NULL DEFAULT 0,
    ProjectId   INTEGER REFERENCES Projects(Id) ON DELETE SET NULL,
    DueDate     TEXT,
    DueTime     TEXT,
    CompletedAt TEXT,
    CreatedAt   TEXT    NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS PomodoroSessions (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    TaskId       INTEGER REFERENCES Tasks(Id) ON DELETE SET NULL,
    StartedAt    TEXT    NOT NULL DEFAULT (datetime('now')),
    CompletedAt  TEXT,
    WasCompleted INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS WaterLog (
    Id      INTEGER PRIMARY KEY AUTOINCREMENT,
    Glasses INTEGER NOT NULL DEFAULT 0,
    Goal    INTEGER NOT NULL DEFAULT 8,
    Date    TEXT    NOT NULL UNIQUE DEFAULT (date('now'))
);
