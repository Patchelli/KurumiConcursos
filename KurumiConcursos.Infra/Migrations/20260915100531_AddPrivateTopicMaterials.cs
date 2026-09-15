using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KurumiConcursos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddPrivateTopicMaterials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "topic_material",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    syllabus_node_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    nextcloud_path = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    mime_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topic_material", x => x.id);
                    table.ForeignKey(
                        name: "FK_topic_material_syllabus_node_syllabus_node_id",
                        column: x => x.syllabus_node_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "syllabus_node",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_topic_material_syllabus_node_id",
                schema: "kurumi_concursos",
                table: "topic_material",
                column: "syllabus_node_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "topic_material",
                schema: "kurumi_concursos");
        }
    }
}
