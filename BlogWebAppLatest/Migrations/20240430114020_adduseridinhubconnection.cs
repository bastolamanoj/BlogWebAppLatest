using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebApp.Migrations
{
    /// <inheritdoc />
    public partial class adduseridinhubconnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "dbo",
                table: "HubConnections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "dbo",
                table: "HubConnections");
        }
    }
}
