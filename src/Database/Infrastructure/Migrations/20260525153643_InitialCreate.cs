using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "infrapilot");

            migrationBuilder.CreateTable(
                name: "workflows",
                schema: "infrapilot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_request = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workflows", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workflow_execution_events",
                schema: "infrapilot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    plan_json = table.Column<string>(type: "jsonb", nullable: true),
                    current_agent = table.Column<string>(type: "text", nullable: true),
                    agent_output = table.Column<string>(type: "jsonb", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workflow_execution_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_workflow_execution_events_workflows_workflow_id",
                        column: x => x.workflow_id,
                        principalSchema: "infrapilot",
                        principalTable: "workflows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_workflow_execution_events_workflow_id",
                schema: "infrapilot",
                table: "workflow_execution_events",
                column: "workflow_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workflow_execution_events",
                schema: "infrapilot");

            migrationBuilder.DropTable(
                name: "workflows",
                schema: "infrapilot");
        }
    }
}
