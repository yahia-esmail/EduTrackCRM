using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddDuplicateLeadLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DuplicateLeadLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    AttemptedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExistingLeadId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuplicateLeadLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DuplicateLeadLogs_Leads_ExistingLeadId",
                        column: x => x.ExistingLeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DuplicateLeadLogs_AttemptedAt",
                table: "DuplicateLeadLogs",
                column: "AttemptedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DuplicateLeadLogs_Email",
                table: "DuplicateLeadLogs",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_DuplicateLeadLogs_ExistingLeadId",
                table: "DuplicateLeadLogs",
                column: "ExistingLeadId");

            migrationBuilder.CreateIndex(
                name: "IX_DuplicateLeadLogs_Phone",
                table: "DuplicateLeadLogs",
                column: "Phone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DuplicateLeadLogs");
        }
    }
}
