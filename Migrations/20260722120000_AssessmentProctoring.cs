using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VistawaysTechRecruitPortal.Migrations
{
    /// <inheritdoc />
    public partial class AssessmentProctoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ConsentGivenOn",
                table: "AssessmentInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedOn",
                table: "AssessmentInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TerminatedForViolation",
                table: "AssessmentInvitations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ViolationReason",
                table: "AssessmentInvitations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WasTerminatedForViolation",
                table: "AssessmentResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TerminationReason",
                table: "AssessmentResults",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsentGivenOn",
                table: "AssessmentInvitations");

            migrationBuilder.DropColumn(
                name: "StartedOn",
                table: "AssessmentInvitations");

            migrationBuilder.DropColumn(
                name: "TerminatedForViolation",
                table: "AssessmentInvitations");

            migrationBuilder.DropColumn(
                name: "ViolationReason",
                table: "AssessmentInvitations");

            migrationBuilder.DropColumn(
                name: "WasTerminatedForViolation",
                table: "AssessmentResults");

            migrationBuilder.DropColumn(
                name: "TerminationReason",
                table: "AssessmentResults");
        }
    }
}
