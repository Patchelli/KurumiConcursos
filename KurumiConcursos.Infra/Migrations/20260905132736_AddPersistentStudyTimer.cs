using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KurumiConcursos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddPersistentStudyTimer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "study_timer_session",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_id = table.Column<long>(type: "bigint", nullable: false),
                    knowledge_area_id = table.Column<long>(type: "bigint", nullable: false),
                    syllabus_node_id = table.Column<long>(type: "bigint", nullable: true),
                    mode = table.Column<int>(type: "integer", nullable: false),
                    phase = table.Column<int>(type: "integer", nullable: false),
                    is_running = table.Column<bool>(type: "boolean", nullable: false),
                    accumulated_focus_seconds = table.Column<int>(type: "integer", nullable: false),
                    current_phase_seconds = table.Column<int>(type: "integer", nullable: false),
                    running_since = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    focus_minutes = table.Column<int>(type: "integer", nullable: false),
                    short_break_minutes = table.Column<int>(type: "integer", nullable: false),
                    long_break_minutes = table.Column<int>(type: "integer", nullable: false),
                    cycles = table.Column<int>(type: "integer", nullable: false),
                    current_cycle = table.Column<int>(type: "integer", nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study_timer_session", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_study_timer_session_user_id",
                schema: "kurumi_concursos",
                table: "study_timer_session",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "study_timer_session",
                schema: "kurumi_concursos");
        }
    }
}
