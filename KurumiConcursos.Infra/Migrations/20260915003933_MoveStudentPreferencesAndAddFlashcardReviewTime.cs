using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KurumiConcursos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class MoveStudentPreferencesAndAddFlashcardReviewTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "flashcard_intervals_json",
                schema: "kurumi_concursos",
                table: "StudentProfile",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "radar_preferences_json",
                schema: "kurumi_concursos",
                table: "StudentProfile",
                type: "text",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE kurumi_concursos."StudentProfile" AS student
                SET radar_preferences_json = account."RadarPreferencesJson"
                FROM kurumi_concursos."User" AS account
                WHERE student.user_id = account.id
                  AND account."RadarPreferencesJson" IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "RadarPreferencesJson",
                schema: "kurumi_concursos",
                table: "User");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "next_review_at",
                schema: "kurumi_concursos",
                table: "memory_card",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RadarPreferencesJson",
                schema: "kurumi_concursos",
                table: "User",
                type: "text",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE kurumi_concursos."User" AS account
                SET "RadarPreferencesJson" = student.radar_preferences_json
                FROM kurumi_concursos."StudentProfile" AS student
                WHERE account.id = student.user_id
                  AND student.radar_preferences_json IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "flashcard_intervals_json",
                schema: "kurumi_concursos",
                table: "StudentProfile");

            migrationBuilder.DropColumn(
                name: "radar_preferences_json",
                schema: "kurumi_concursos",
                table: "StudentProfile");

            migrationBuilder.DropColumn(
                name: "next_review_at",
                schema: "kurumi_concursos",
                table: "memory_card");

        }
    }
}
