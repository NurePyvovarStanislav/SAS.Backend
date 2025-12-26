using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAS.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Users_UserId",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "IX_Fields_UserId",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Fields");

            migrationBuilder.AddColumn<Guid>(
                name: "FieldId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FieldId",
                table: "Users",
                column: "FieldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Fields_FieldId",
                table: "Users",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "FieldId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Fields_FieldId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_FieldId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FieldId",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Fields",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Fields_UserId",
                table: "Fields",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Users_UserId",
                table: "Fields",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
