using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RooMee.Data.Migrations
{
    /// <inheritdoc />
    public partial class EpostaBildirim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EpostaBildirim",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true); // var olan kullanıcılarda bildirimler açık başlasın
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EpostaBildirim",
                table: "AspNetUsers");
        }
    }
}
