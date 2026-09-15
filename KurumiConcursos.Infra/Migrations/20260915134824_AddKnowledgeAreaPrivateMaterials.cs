using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KurumiConcursos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddKnowledgeAreaPrivateMaterials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "syllabus_node_id",
                schema: "kurumi_concursos",
                table: "topic_material",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "knowledge_area_id",
                schema: "kurumi_concursos",
                table: "topic_material",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_topic_material_knowledge_area_id",
                schema: "kurumi_concursos",
                table: "topic_material",
                column: "knowledge_area_id");

            migrationBuilder.AddForeignKey(
                name: "FK_topic_material_knowledge_area_knowledge_area_id",
                schema: "kurumi_concursos",
                table: "topic_material",
                column: "knowledge_area_id",
                principalSchema: "kurumi_concursos",
                principalTable: "knowledge_area",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_topic_material_knowledge_area_knowledge_area_id",
                schema: "kurumi_concursos",
                table: "topic_material");

            migrationBuilder.DropIndex(
                name: "IX_topic_material_knowledge_area_id",
                schema: "kurumi_concursos",
                table: "topic_material");

            migrationBuilder.DropColumn(
                name: "knowledge_area_id",
                schema: "kurumi_concursos",
                table: "topic_material");

            migrationBuilder.AlterColumn<long>(
                name: "syllabus_node_id",
                schema: "kurumi_concursos",
                table: "topic_material",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
