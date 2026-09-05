using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KurumiConcursos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class ExpandPracticeEntryDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "error_reasons_json",
                schema: "kurumi_concursos",
                table: "practice_entry",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "notes",
                schema: "kurumi_concursos",
                table: "practice_entry",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "voided_questions",
                schema: "kurumi_concursos",
                table: "practice_entry",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "error_reasons_json",
                schema: "kurumi_concursos",
                table: "practice_entry");

            migrationBuilder.DropColumn(
                name: "notes",
                schema: "kurumi_concursos",
                table: "practice_entry");

            migrationBuilder.DropColumn(
                name: "voided_questions",
                schema: "kurumi_concursos",
                table: "practice_entry");
        }
    }
}
