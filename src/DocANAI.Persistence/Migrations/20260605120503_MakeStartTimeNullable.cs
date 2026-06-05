using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocANAI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeStartTimeNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "file_path",
                table: "source_document",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "file_path",
                table: "report",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "file_path",
                table: "question_file",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "question_file_id",
                table: "question",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "task_id",
                table: "question",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_question_task_id",
                table: "question",
                column: "task_id");

            migrationBuilder.AddForeignKey(
                name: "FK_question_processing_task_task_id",
                table: "question",
                column: "task_id",
                principalTable: "processing_task",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_question_processing_task_task_id",
                table: "question");

            migrationBuilder.DropIndex(
                name: "IX_question_task_id",
                table: "question");

            migrationBuilder.DropColumn(
                name: "file_path",
                table: "source_document");

            migrationBuilder.DropColumn(
                name: "file_path",
                table: "report");

            migrationBuilder.DropColumn(
                name: "file_path",
                table: "question_file");

            migrationBuilder.DropColumn(
                name: "task_id",
                table: "question");

            migrationBuilder.AlterColumn<Guid>(
                name: "question_file_id",
                table: "question",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
