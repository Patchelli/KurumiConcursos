using KurumiConcursos.Infra.ORM.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KurumiConcursos.Infra.Migrations;

[DbContext(typeof(ApplicationContext))]
[Migration("20260916120000_AddMaterialStudyLocation")]
public partial class AddMaterialStudyLocation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "study_location", schema: "kurumi_concursos", table: "study_resource",
            type: "character varying(500)", maxLength: 500, nullable: true);
        migrationBuilder.AddColumn<string>(
            name: "study_location", schema: "kurumi_concursos", table: "topic_material",
            type: "character varying(500)", maxLength: 500, nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "study_location", schema: "kurumi_concursos", table: "study_resource");
        migrationBuilder.DropColumn(name: "study_location", schema: "kurumi_concursos", table: "topic_material");
    }
}
