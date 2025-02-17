using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace userService.Migrations
{
    /// <inheritdoc />
    public partial class deleteColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelationUsers_Devices_DeviceId",
                table: "RelationUsers");

            migrationBuilder.DropIndex(
                name: "IX_RelationUsers_DeviceId",
                table: "RelationUsers");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "RelationUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeviceId",
                table: "RelationUsers",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_RelationUsers_DeviceId",
                table: "RelationUsers",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_RelationUsers_Devices_DeviceId",
                table: "RelationUsers",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");
        }
    }
}
