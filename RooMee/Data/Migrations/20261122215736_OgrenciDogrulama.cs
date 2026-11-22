using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RooMee.Data.Migrations
{
    /// <inheritdoc />
    public partial class OgrenciDogrulama : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OgrenciDogrulamaTarihi",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OgrenciDogrulandi",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OgrenciEposta",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OgrenciDogrulamaTarihi",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OgrenciDogrulandi",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OgrenciEposta",
                table: "AspNetUsers");
        }
    }
}
