using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VistawaysTechRecruitPortal.Migrations
{
    /// <inheritdoc />
    public partial class SyncCandidateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QualificationType",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QualificationType",
                table: "Candidates");
        }
    }
}
