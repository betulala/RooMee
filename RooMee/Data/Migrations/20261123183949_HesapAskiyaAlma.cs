using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RooMee.Data.Migrations
{
    /// <inheritdoc />
    public partial class HesapAskiyaAlma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AskiyaAlindi",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "AskiyaAlinmaTarihi",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AskiyaAlmaNedeni",
                table: "AspNetUsers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AskiyaAlindi",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AskiyaAlinmaTarihi",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AskiyaAlmaNedeni",
                table: "AspNetUsers");
        }
    }
}
