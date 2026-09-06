using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KurumiConcursos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceAdaptation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "adaptation_trigger",
                schema: "kurumi_concursos",
                table: "review_appointment",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "adaptation_trigger",
                schema: "kurumi_concursos",
                table: "question_appointment",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "adaptation_trigger",
                schema: "kurumi_concursos",
                table: "review_appointment");

            migrationBuilder.DropColumn(
                name: "adaptation_trigger",
                schema: "kurumi_concursos",
                table: "question_appointment");
        }
    }
}
