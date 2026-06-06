using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocANAI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataAndIncreaseMimeTypeLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "processing_task",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "mime_type",
                table: "format",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
            
            migrationBuilder.Sql(@"
                INSERT INTO a_i_model (id, name, version, is_available)
                VALUES (gen_random_uuid(), 'llama3', 'latest', true)
                ON CONFLICT DO NOTHING;

                INSERT INTO priority_level (level, weight) 
                VALUES ('normal', 1) 
                ON CONFLICT DO NOTHING;

                INSERT INTO priority (id, priority_level)
                VALUES ('11111111-1111-1111-1111-111111111111', 'normal')
                ON CONFLICT DO NOTHING;

                INSERT INTO format (extension, max_size, mime_type) 
                VALUES
                ('.pdf', 104857600, 'application/pdf'),
                ('.docx', 104857600, 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'),
                ('.xlsx', 104857600, 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')
                ON CONFLICT DO NOTHING;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "start_time",
                table: "processing_task",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "mime_type",
                table: "format",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
