using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KurumiConcursos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddFlashcardCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "model",
                schema: "kurumi_concursos",
                table: "memory_card",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "type",
                schema: "kurumi_concursos",
                table: "memory_card",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "knowledge_area_id",
                schema: "kurumi_concursos",
                table: "flash_collection",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "syllabus_node_id",
                schema: "kurumi_concursos",
                table: "flash_collection",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_flash_collection_user_id_journey_id_knowledge_area_id_sylla~",
                schema: "kurumi_concursos",
                table: "flash_collection",
                columns: new[] { "user_id", "journey_id", "knowledge_area_id", "syllabus_node_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_flash_collection_user_id_journey_id_knowledge_area_id_sylla~",
                schema: "kurumi_concursos",
                table: "flash_collection");

            migrationBuilder.DropColumn(
                name: "model",
                schema: "kurumi_concursos",
                table: "memory_card");

            migrationBuilder.DropColumn(
                name: "type",
                schema: "kurumi_concursos",
                table: "memory_card");

            migrationBuilder.DropColumn(
                name: "knowledge_area_id",
                schema: "kurumi_concursos",
                table: "flash_collection");

            migrationBuilder.DropColumn(
                name: "syllabus_node_id",
                schema: "kurumi_concursos",
                table: "flash_collection");
        }
    }
}
