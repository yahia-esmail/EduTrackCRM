using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadActivityAndAssignmentLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeadActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadId = table.Column<int>(type: "int", nullable: false),
                    ActorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviousStage = table.Column<int>(type: "int", nullable: true),
                    NewStage = table.Column<int>(type: "int", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadActivities_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeadAssignmentLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadId = table.Column<int>(type: "int", nullable: false),
                    FromUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ToUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AssignedByUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadAssignmentLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadAssignmentLogs_AspNetUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LeadAssignmentLogs_AspNetUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LeadAssignmentLogs_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeadActivities_LeadId",
                table: "LeadActivities",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadActivities_Timestamp",
                table: "LeadActivities",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_LeadAssignmentLogs_FromUserId",
                table: "LeadAssignmentLogs",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadAssignmentLogs_LeadId",
                table: "LeadAssignmentLogs",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadAssignmentLogs_Timestamp",
                table: "LeadAssignmentLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_LeadAssignmentLogs_ToUserId",
                table: "LeadAssignmentLogs",
                column: "ToUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeadActivities");

            migrationBuilder.DropTable(
                name: "LeadAssignmentLogs");
        }
    }
}
