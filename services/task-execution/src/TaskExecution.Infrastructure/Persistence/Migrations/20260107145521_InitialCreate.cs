using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskExecution.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "execution_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    priority = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    payload = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    progress_percent = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    status_message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    result_location = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    retry_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    correlation_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_execution_records", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "processed_events",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    consumer_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_processed_events", x => new { x.event_id, x.consumer_name });
                });

            migrationBuilder.CreateIndex(
                name: "idx_execution_created_at",
                table: "execution_records",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_execution_status",
                table: "execution_records",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_execution_task_id",
                table: "execution_records",
                column: "task_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "execution_records");

            migrationBuilder.DropTable(
                name: "processed_events");
        }
    }
}
