using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SYT.BnBCheckIn.Migrations
{
    /// <inheritdoc />
    public partial class AddedUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Unit = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Pico = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RFID = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Building = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StartTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CheckInRef = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usages", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usages");
        }
    }
}
