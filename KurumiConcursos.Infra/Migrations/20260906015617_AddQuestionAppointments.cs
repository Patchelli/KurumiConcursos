using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KurumiConcursos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "question_appointment",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_id = table.Column<long>(type: "bigint", nullable: false),
                    syllabus_node_id = table.Column<long>(type: "bigint", nullable: false),
                    scheduled_for = table.Column<DateOnly>(type: "date", nullable: false),
                    completed = table.Column<bool>(type: "boolean", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    superseded = table.Column<bool>(type: "boolean", nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_appointment", x => x.id);
                    table.ForeignKey(
                        name: "FK_question_appointment_syllabus_node_syllabus_node_id",
                        column: x => x.syllabus_node_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "syllabus_node",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_question_appointment_syllabus_node_id",
                schema: "kurumi_concursos",
                table: "question_appointment",
                column: "syllabus_node_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_appointment_user_id_journey_id_scheduled_for",
                schema: "kurumi_concursos",
                table: "question_appointment",
                columns: new[] { "user_id", "journey_id", "scheduled_for" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "question_appointment",
                schema: "kurumi_concursos");
        }
    }
}
