-- Run this against your VistawaysTechRecruitPortal database (e.g. in SSMS or
-- the SQL Server Object Explorer in Visual Studio) to add the columns the new
-- assessment-proctoring feature needs, without depending on EF Core's
-- migration tooling. Safe to re-run; each block only acts if the column is
-- missing.

IF COL_LENGTH('AssessmentInvitations', 'ConsentGivenOn') IS NULL
    ALTER TABLE AssessmentInvitations ADD ConsentGivenOn datetime2 NULL;

IF COL_LENGTH('AssessmentInvitations', 'StartedOn') IS NULL
    ALTER TABLE AssessmentInvitations ADD StartedOn datetime2 NULL;

IF COL_LENGTH('AssessmentInvitations', 'TerminatedForViolation') IS NULL
    ALTER TABLE AssessmentInvitations ADD TerminatedForViolation bit NOT NULL DEFAULT (0);

IF COL_LENGTH('AssessmentInvitations', 'ViolationReason') IS NULL
    ALTER TABLE AssessmentInvitations ADD ViolationReason nvarchar(max) NULL;

IF COL_LENGTH('AssessmentResults', 'WasTerminatedForViolation') IS NULL
    ALTER TABLE AssessmentResults ADD WasTerminatedForViolation bit NOT NULL DEFAULT (0);

IF COL_LENGTH('AssessmentResults', 'TerminationReason') IS NULL
    ALTER TABLE AssessmentResults ADD TerminationReason nvarchar(max) NULL;

-- Tell EF Core this migration is already applied, so it won't try (and fail)
-- to re-run it the next time the app starts and calls db.Database.Migrate().
IF NOT EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260722120000_AssessmentProctoring'
)
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES ('20260722120000_AssessmentProctoring', '10.0.9');
