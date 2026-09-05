using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KurumiConcursos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddStudySummaryHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "study_summary",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_id = table.Column<long>(type: "bigint", nullable: false),
                    syllabus_node_id = table.Column<long>(type: "bigint", nullable: false),
                    review_appointment_id = table.Column<long>(type: "bigint", nullable: true),
                    is_review = table.Column<bool>(type: "boolean", nullable: false),
                    content = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study_summary", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_study_summary_user_id_syllabus_node_id_creation_date",
                schema: "kurumi_concursos",
                table: "study_summary",
                columns: new[] { "user_id", "syllabus_node_id", "creation_date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "study_summary",
                schema: "kurumi_concursos");
        }
    }
}
