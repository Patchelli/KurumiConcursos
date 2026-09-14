using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KurumiConcursos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddCognitivePairing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cognitive_pairing",
                schema: "kurumi_concursos",
                table: "syllabus_node",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cognitive_pairing",
                schema: "kurumi_concursos",
                table: "syllabus_node");
        }
    }
}
