using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VistawaysTechRecruitPortal.Migrations
{
    /// <inheritdoc />
    public partial class CandidateEducationDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AddStringColumn(migrationBuilder, "UndergraduateUniversity");
            AddStringColumn(migrationBuilder, "UndergraduateLocation");
            AddStringColumn(migrationBuilder, "UndergraduateDegree");
            AddStringColumn(migrationBuilder, "UndergraduateSpecialization");
            AddNullableIntColumn(migrationBuilder, "UndergraduatePassingYear");
            AddNullableDecimalColumn(migrationBuilder, "UndergraduatePercentage");

            AddStringColumn(migrationBuilder, "PostgraduateUniversity");
            AddStringColumn(migrationBuilder, "PostgraduateLocation");
            AddStringColumn(migrationBuilder, "PostgraduateDegree");
            AddStringColumn(migrationBuilder, "PostgraduateSpecialization");
            AddNullableIntColumn(migrationBuilder, "PostgraduatePassingYear");
            AddNullableDecimalColumn(migrationBuilder, "PostgraduatePercentage");

            AddStringColumn(migrationBuilder, "IntermediateCollege");
            AddStringColumn(migrationBuilder, "IntermediateLocation");
            AddStringColumn(migrationBuilder, "IntermediateBoard");
            AddStringColumn(migrationBuilder, "IntermediateHallTicket");
            AddNullableIntColumn(migrationBuilder, "IntermediatePassingYear");
            AddNullableDecimalColumn(migrationBuilder, "IntermediatePercentage");

            AddStringColumn(migrationBuilder, "TenthInstitute");
            AddStringColumn(migrationBuilder, "TenthLocation");
            AddStringColumn(migrationBuilder, "TenthBoard");
            AddStringColumn(migrationBuilder, "TenthHallTicket");
            AddNullableIntColumn(migrationBuilder, "TenthPassingYear");
            AddNullableDecimalColumn(migrationBuilder, "TenthPercentage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            DropColumn(migrationBuilder, "TenthPercentage");
            DropColumn(migrationBuilder, "TenthPassingYear");
            DropColumn(migrationBuilder, "TenthHallTicket");
            DropColumn(migrationBuilder, "TenthBoard");
            DropColumn(migrationBuilder, "TenthLocation");
            DropColumn(migrationBuilder, "TenthInstitute");

            DropColumn(migrationBuilder, "IntermediatePercentage");
            DropColumn(migrationBuilder, "IntermediatePassingYear");
            DropColumn(migrationBuilder, "IntermediateHallTicket");
            DropColumn(migrationBuilder, "IntermediateBoard");
            DropColumn(migrationBuilder, "IntermediateLocation");
            DropColumn(migrationBuilder, "IntermediateCollege");

            DropColumn(migrationBuilder, "PostgraduatePercentage");
            DropColumn(migrationBuilder, "PostgraduatePassingYear");
            DropColumn(migrationBuilder, "PostgraduateSpecialization");
            DropColumn(migrationBuilder, "PostgraduateDegree");
            DropColumn(migrationBuilder, "PostgraduateLocation");
            DropColumn(migrationBuilder, "PostgraduateUniversity");

            DropColumn(migrationBuilder, "UndergraduatePercentage");
            DropColumn(migrationBuilder, "UndergraduatePassingYear");
            DropColumn(migrationBuilder, "UndergraduateSpecialization");
            DropColumn(migrationBuilder, "UndergraduateDegree");
            DropColumn(migrationBuilder, "UndergraduateLocation");
            DropColumn(migrationBuilder, "UndergraduateUniversity");
        }

        private static void AddStringColumn(MigrationBuilder migrationBuilder, string columnName)
        {
            migrationBuilder.Sql($@"
                IF COL_LENGTH('Candidates', '{columnName}') IS NULL
                BEGIN
                    ALTER TABLE [Candidates]
                    ADD [{columnName}] nvarchar(max) NOT NULL
                    CONSTRAINT [DF_Candidates_{columnName}] DEFAULT N''
                END");
        }

        private static void AddNullableIntColumn(MigrationBuilder migrationBuilder, string columnName)
        {
            migrationBuilder.Sql($@"
                IF COL_LENGTH('Candidates', '{columnName}') IS NULL
                BEGIN
                    ALTER TABLE [Candidates]
                    ADD [{columnName}] int NULL
                END");
        }

        private static void AddNullableDecimalColumn(MigrationBuilder migrationBuilder, string columnName)
        {
            migrationBuilder.Sql($@"
                IF COL_LENGTH('Candidates', '{columnName}') IS NULL
                BEGIN
                    ALTER TABLE [Candidates]
                    ADD [{columnName}] decimal(18,2) NULL
                END");
        }

        private static void DropColumn(MigrationBuilder migrationBuilder, string columnName)
        {
            migrationBuilder.Sql($@"
                IF COL_LENGTH('Candidates', '{columnName}') IS NOT NULL
                BEGIN
                    ALTER TABLE [Candidates]
                    DROP COLUMN [{columnName}]
                END");
        }
    }
}
