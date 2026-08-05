using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VistawaysTechRecruitPortal.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCandidateRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('Candidates', 'Role') IS NULL
                BEGIN
                    ALTER TABLE [Candidates]
                    ADD [Role] nvarchar(max) NOT NULL
                    CONSTRAINT [DF_Candidates_Role] DEFAULT N'Candidate'
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('Candidates', 'Role') IS NOT NULL
                BEGIN
                    ALTER TABLE [Candidates]
                    DROP COLUMN [Role]
                END");
        }
    }
}
