using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KurumiConcursos.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "kurumi_concursos");

            migrationBuilder.CreateTable(
                name: "achievement_milestone",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_id = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    title = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    achieved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    metrics_json = table.Column<string>(type: "jsonb", nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievement_milestone", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "calendar_event",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_date = table.Column<DateOnly>(type: "date", nullable: false),
                    title = table.Column<string>(type: "varchar(255)", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "varchar(1000)", nullable: true),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calendar_event", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "DomainLogger",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    action = table.Column<byte>(type: "smallint", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    action_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_id = table.Column<string>(type: "varchar(80)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainLogger", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "flash_collection",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_id = table.Column<long>(type: "bigint", nullable: false),
                    parent_id = table.Column<long>(type: "bigint", nullable: true),
                    title = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_flash_collection", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "focus_session",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_id = table.Column<long>(type: "bigint", nullable: false),
                    knowledge_area_id = table.Column<long>(type: "bigint", nullable: true),
                    syllabus_node_id = table.Column<long>(type: "bigint", nullable: true),
                    study_date = table.Column<DateOnly>(type: "date", nullable: false),
                    duration_seconds = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_focus_session", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mock_assessment",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    assessment_date = table.Column<DateOnly>(type: "date", nullable: false),
                    total_questions = table.Column<int>(type: "integer", nullable: false),
                    correct_answers = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mock_assessment", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "practice_entry",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_id = table.Column<long>(type: "bigint", nullable: false),
                    knowledge_area_id = table.Column<long>(type: "bigint", nullable: true),
                    syllabus_node_id = table.Column<long>(type: "bigint", nullable: true),
                    practice_date = table.Column<DateOnly>(type: "date", nullable: false),
                    questions_answered = table.Column<int>(type: "integer", nullable: false),
                    correct_answers = table.Column<int>(type: "integer", nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_practice_entry", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "review_appointment",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_review_appointment", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "study_resource",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_id = table.Column<long>(type: "bigint", nullable: false),
                    knowledge_area_id = table.Column<long>(type: "bigint", nullable: true),
                    syllabus_node_id = table.Column<long>(type: "bigint", nullable: true),
                    title = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    url = table.Column<string>(type: "text", nullable: false),
                    kind = table.Column<int>(type: "integer", nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study_resource", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "study_routine",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "varchar(180)", nullable: false),
                    kind = table.Column<int>(type: "integer", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    configuration_json = table.Column<string>(type: "jsonb", nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study_routine", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "study_routine_block",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_id = table.Column<long>(type: "bigint", nullable: false),
                    study_routine_id = table.Column<long>(type: "bigint", nullable: false),
                    syllabus_node_id = table.Column<long>(type: "bigint", nullable: false),
                    scheduled_for = table.Column<DateOnly>(type: "date", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    planned_minutes = table.Column<int>(type: "integer", nullable: false),
                    completed_minutes = table.Column<int>(type: "integer", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study_routine_block", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    normalized_username = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "varchar(50)", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false),
                    identifier = table.Column<string>(type: "varchar(100)", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    preferred_language = table.Column<int>(type: "integer", nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_access_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "memory_card",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    collection_id = table.Column<long>(type: "bigint", nullable: false),
                    front = table.Column<string>(type: "text", nullable: false),
                    back = table.Column<string>(type: "text", nullable: false),
                    next_review_on = table.Column<DateOnly>(type: "date", nullable: true),
                    interval_days = table.Column<int>(type: "integer", nullable: false),
                    ease_factor = table.Column<decimal>(type: "numeric(6,3)", precision: 6, scale: 3, nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memory_card", x => x.id);
                    table.ForeignKey(
                        name: "FK_memory_card_flash_collection_collection_id",
                        column: x => x.collection_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "flash_collection",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mock_assessment_breakdown",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mock_assessment_id = table.Column<long>(type: "bigint", nullable: false),
                    knowledge_area_id = table.Column<long>(type: "bigint", nullable: false),
                    total_questions = table.Column<int>(type: "integer", nullable: false),
                    correct_answers = table.Column<int>(type: "integer", nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mock_assessment_breakdown", x => x.id);
                    table.ForeignKey(
                        name: "FK_mock_assessment_breakdown_mock_assessment_mock_assessment_id",
                        column: x => x.mock_assessment_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "mock_assessment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaim",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaim", x => x.id);
                    table.ForeignKey(
                        name: "FK_RoleClaim_Role_role_id",
                        column: x => x.role_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "Role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminProfile",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminProfile", x => x.id);
                    table.ForeignKey(
                        name: "FK_AdminProfile_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalData",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "varchar(255)", nullable: true),
                    document = table.Column<string>(type: "varchar(50)", nullable: true),
                    phone = table.Column<string>(type: "varchar(50)", nullable: true),
                    age = table.Column<int>(type: "integer", nullable: true),
                    date_of_birth = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalData", x => x.id);
                    table.ForeignKey(
                        name: "FK_PersonalData_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentProfile",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProfile", x => x.id);
                    table.UniqueConstraint("AK_StudentProfile_user_id", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_StudentProfile_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaim",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserClaim_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogin_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_role_id",
                        column: x => x.role_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "Role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserToken",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => new { x.user_id, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserToken_User_user_id",
                        column: x => x.user_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "memory_recall",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    memory_card_id = table.Column<long>(type: "bigint", nullable: false),
                    grade = table.Column<int>(type: "integer", nullable: false),
                    answered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    previous_interval_days = table.Column<int>(type: "integer", nullable: false),
                    new_interval_days = table.Column<int>(type: "integer", nullable: false),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memory_recall", x => x.id);
                    table.ForeignKey(
                        name: "FK_memory_recall_memory_card_memory_card_id",
                        column: x => x.memory_card_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "memory_card",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exam_journey",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    institution = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    exam_board = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    position = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    salary = table.Column<decimal>(type: "numeric(14,2)", precision: 14, scale: 2, nullable: true),
                    openings = table.Column<int>(type: "integer", nullable: true),
                    notice_url = table.Column<string>(type: "text", nullable: true),
                    exam_date = table.Column<DateOnly>(type: "date", nullable: true),
                    stage = table.Column<int>(type: "integer", nullable: false),
                    include_in_statistics = table.Column<bool>(type: "boolean", nullable: false),
                    completed_syllabus_cycles = table.Column<int>(type: "integer", nullable: false),
                    logo_url = table.Column<string>(type: "text", nullable: true),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exam_journey", x => x.id);
                    table.ForeignKey(
                        name: "FK_exam_journey_StudentProfile_user_id",
                        column: x => x.user_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "StudentProfile",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "knowledge_area",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    journey_id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    weight = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    expected_questions = table.Column<int>(type: "integer", nullable: true),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_knowledge_area", x => x.id);
                    table.ForeignKey(
                        name: "FK_knowledge_area_exam_journey_journey_id",
                        column: x => x.journey_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "exam_journey",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "syllabus_node",
                schema: "kurumi_concursos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    knowledge_area_id = table.Column<long>(type: "bigint", nullable: false),
                    parent_id = table.Column<long>(type: "bigint", nullable: true),
                    title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    progress = table.Column<int>(type: "integer", nullable: false),
                    study_started_on = table.Column<DateOnly>(type: "date", nullable: true),
                    studied_on = table.Column<DateOnly>(type: "date", nullable: true),
                    creation_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_update_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_syllabus_node", x => x.id);
                    table.ForeignKey(
                        name: "FK_syllabus_node_knowledge_area_knowledge_area_id",
                        column: x => x.knowledge_area_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "knowledge_area",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_syllabus_node_syllabus_node_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "kurumi_concursos",
                        principalTable: "syllabus_node",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_admin_profile_user_id",
                schema: "kurumi_concursos",
                table: "AdminProfile",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_calendar_event_user_id",
                schema: "kurumi_concursos",
                table: "calendar_event",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_exam_journey_user_id",
                schema: "kurumi_concursos",
                table: "exam_journey",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_knowledge_area_journey_id",
                schema: "kurumi_concursos",
                table: "knowledge_area",
                column: "journey_id");

            migrationBuilder.CreateIndex(
                name: "IX_memory_card_collection_id",
                schema: "kurumi_concursos",
                table: "memory_card",
                column: "collection_id");

            migrationBuilder.CreateIndex(
                name: "IX_memory_recall_memory_card_id",
                schema: "kurumi_concursos",
                table: "memory_recall",
                column: "memory_card_id");

            migrationBuilder.CreateIndex(
                name: "IX_mock_assessment_breakdown_mock_assessment_id",
                schema: "kurumi_concursos",
                table: "mock_assessment_breakdown",
                column: "mock_assessment_id");

            migrationBuilder.CreateIndex(
                name: "ux_personal_data_user_id",
                schema: "kurumi_concursos",
                table: "PersonalData",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "kurumi_concursos",
                table: "Role",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaim_role_id",
                schema: "kurumi_concursos",
                table: "RoleClaim",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_student_profile_user_id",
                schema: "kurumi_concursos",
                table: "StudentProfile",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_study_routine_user_journey",
                schema: "kurumi_concursos",
                table: "study_routine",
                columns: new[] { "user_id", "journey_id" });

            migrationBuilder.CreateIndex(
                name: "ix_study_routine_block_user_date",
                schema: "kurumi_concursos",
                table: "study_routine_block",
                columns: new[] { "user_id", "scheduled_for" });

            migrationBuilder.CreateIndex(
                name: "IX_syllabus_node_knowledge_area_id",
                schema: "kurumi_concursos",
                table: "syllabus_node",
                column: "knowledge_area_id");

            migrationBuilder.CreateIndex(
                name: "IX_syllabus_node_parent_id",
                schema: "kurumi_concursos",
                table: "syllabus_node",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "kurumi_concursos",
                table: "User",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "kurumi_concursos",
                table: "User",
                column: "normalized_username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_user_identifier",
                schema: "kurumi_concursos",
                table: "User",
                column: "identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_user_id",
                schema: "kurumi_concursos",
                table: "UserClaim",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_user_id",
                schema: "kurumi_concursos",
                table: "UserLogin",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_role_id",
                schema: "kurumi_concursos",
                table: "UserRole",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "achievement_milestone",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "AdminProfile",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "calendar_event",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "DomainLogger",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "focus_session",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "memory_recall",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "mock_assessment_breakdown",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "PersonalData",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "practice_entry",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "review_appointment",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "RoleClaim",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "study_resource",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "study_routine",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "study_routine_block",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "syllabus_node",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "UserClaim",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "UserLogin",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "UserToken",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "memory_card",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "mock_assessment",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "knowledge_area",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "flash_collection",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "exam_journey",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "StudentProfile",
                schema: "kurumi_concursos");

            migrationBuilder.DropTable(
                name: "User",
                schema: "kurumi_concursos");
        }
    }
}
