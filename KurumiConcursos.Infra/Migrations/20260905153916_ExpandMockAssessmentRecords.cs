using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KurumiConcursos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class ExpandMockAssessmentRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "error_reasons_json",
                schema: "kurumi_concursos",
                table: "mock_assessment_breakdown",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "notes",
                schema: "kurumi_concursos",
                table: "mock_assessment_breakdown",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "voided_questions",
                schema: "kurumi_concursos",
                table: "mock_assessment_breakdown",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "duration_minutes",
                schema: "kurumi_concursos",
                table: "mock_assessment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "source",
                schema: "kurumi_concursos",
                table: "mock_assessment",
                type: "character varying(180)",
                maxLength: 180,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "error_reasons_json",
                schema: "kurumi_concursos",
                table: "mock_assessment_breakdown");

            migrationBuilder.DropColumn(
                name: "notes",
                schema: "kurumi_concursos",
                table: "mock_assessment_breakdown");

            migrationBuilder.DropColumn(
                name: "voided_questions",
                schema: "kurumi_concursos",
                table: "mock_assessment_breakdown");

            migrationBuilder.DropColumn(
                name: "duration_minutes",
                schema: "kurumi_concursos",
                table: "mock_assessment");

            migrationBuilder.DropColumn(
                name: "source",
                schema: "kurumi_concursos",
                table: "mock_assessment");
        }
    }
}
