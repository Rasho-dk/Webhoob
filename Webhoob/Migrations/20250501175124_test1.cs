using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webhoob.Migrations
{
    /// <inheritdoc />
    public partial class test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Webhooks",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    CallbackUrl = table.Column<string>(type: "TEXT", nullable: false),
                    EventTypesCsv = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Webhooks", x => x.Guid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Webhooks");
        }
    }
}
