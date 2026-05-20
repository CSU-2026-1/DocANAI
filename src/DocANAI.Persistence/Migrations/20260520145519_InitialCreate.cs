using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocANAI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "a_i_model",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    version = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_a_i_model", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "format",
                columns: table => new
                {
                    extension = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    max_size = table.Column<int>(type: "integer", nullable: false),
                    mime_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_format", x => x.extension);
                });

            migrationBuilder.CreateTable(
                name: "priority_level",
                columns: table => new
                {
                    level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    weight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_priority_level", x => x.level);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    user_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "priority",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    priority_level = table.Column<string>(type: "character varying(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_priority", x => x.id);
                    table.ForeignKey(
                        name: "FK_priority_priority_level_priority_level",
                        column: x => x.priority_level,
                        principalTable: "priority_level",
                        principalColumn: "level",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "processing_task",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    model_id = table.Column<Guid>(type: "uuid", nullable: false),
                    priority_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_processing_task", x => x.id);
                    table.ForeignKey(
                        name: "fk_tasks_model",
                        column: x => x.model_id,
                        principalTable: "a_i_model",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tasks_priority",
                        column: x => x.priority_id,
                        principalTable: "priority",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tasks_user",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_file",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    extension = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    filename = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_file", x => x.id);
                    table.ForeignKey(
                        name: "FK_question_file_format_extension",
                        column: x => x.extension,
                        principalTable: "format",
                        principalColumn: "extension",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_question_file_processing_task_task_id",
                        column: x => x.task_id,
                        principalTable: "processing_task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_question_file_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "report",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    extension = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    deletion_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_report_format_extension",
                        column: x => x.extension,
                        principalTable: "format",
                        principalColumn: "extension",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_report_processing_task_task_id",
                        column: x => x.task_id,
                        principalTable: "processing_task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "source_document",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    extension = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    filename = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    size = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_source_document", x => x.id);
                    table.ForeignKey(
                        name: "FK_source_document_format_extension",
                        column: x => x.extension,
                        principalTable: "format",
                        principalColumn: "extension",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_source_document_processing_task_task_id",
                        column: x => x.task_id,
                        principalTable: "processing_task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_source_document_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_number = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question", x => x.id);
                    table.ForeignKey(
                        name: "FK_question_question_file_question_file_id",
                        column: x => x.question_file_id,
                        principalTable: "question_file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "answer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    model_accuracy = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answer", x => x.id);
                    table.ForeignKey(
                        name: "FK_answer_processing_task_task_id",
                        column: x => x.task_id,
                        principalTable: "processing_task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_answer_question_question_id",
                        column: x => x.question_id,
                        principalTable: "question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_answer_question_id",
                table: "answer",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_answer_task_id",
                table: "answer",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_priority_priority_level",
                table: "priority",
                column: "priority_level");

            migrationBuilder.CreateIndex(
                name: "IX_processing_task_model_id",
                table: "processing_task",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "IX_processing_task_priority_id",
                table: "processing_task",
                column: "priority_id");

            migrationBuilder.CreateIndex(
                name: "IX_processing_task_user_id",
                table: "processing_task",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_question_file_id",
                table: "question",
                column: "question_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_file_extension",
                table: "question_file",
                column: "extension");

            migrationBuilder.CreateIndex(
                name: "IX_question_file_task_id",
                table: "question_file",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_file_user_id",
                table: "question_file",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_extension",
                table: "report",
                column: "extension");

            migrationBuilder.CreateIndex(
                name: "IX_report_task_id",
                table: "report",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_source_document_extension",
                table: "source_document",
                column: "extension");

            migrationBuilder.CreateIndex(
                name: "IX_source_document_task_id",
                table: "source_document",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_source_document_user_id",
                table: "source_document",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "answer");

            migrationBuilder.DropTable(
                name: "report");

            migrationBuilder.DropTable(
                name: "source_document");

            migrationBuilder.DropTable(
                name: "question");

            migrationBuilder.DropTable(
                name: "question_file");

            migrationBuilder.DropTable(
                name: "format");

            migrationBuilder.DropTable(
                name: "processing_task");

            migrationBuilder.DropTable(
                name: "a_i_model");

            migrationBuilder.DropTable(
                name: "priority");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "priority_level");
        }
    }
}
