using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VistawaysTechRecruitPortal.Migrations
{
    /// <inheritdoc />
    public partial class RecruitmentWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('AssessmentInvitations', 'CompletedOn') IS NULL
                BEGIN
                    ALTER TABLE [AssessmentInvitations]
                    ADD [CompletedOn] datetime2 NULL
                END");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Candidates', 'ProfileReviewed') IS NULL
                BEGIN
                    ALTER TABLE [Candidates]
                    ADD [ProfileReviewed] bit NOT NULL
                    CONSTRAINT [DF_Candidates_ProfileReviewed] DEFAULT CAST(0 AS bit)
                END");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Candidates', 'RecruitmentStatus') IS NULL
                BEGIN
                    ALTER TABLE [Candidates]
                    ADD [RecruitmentStatus] nvarchar(max) NOT NULL
                    CONSTRAINT [DF_Candidates_RecruitmentStatus] DEFAULT N'Registered'
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('AssessmentInvitations', 'CompletedOn') IS NOT NULL
                BEGIN
                    ALTER TABLE [AssessmentInvitations]
                    DROP COLUMN [CompletedOn]
                END");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Candidates', 'ProfileReviewed') IS NOT NULL
                BEGIN
                    ALTER TABLE [Candidates]
                    DROP COLUMN [ProfileReviewed]
                END");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Candidates', 'RecruitmentStatus') IS NOT NULL
                BEGIN
                    ALTER TABLE [Candidates]
                    DROP COLUMN [RecruitmentStatus]
                END");
        }
    }
}
