using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebApp.Migrations
{
    /// <inheritdoc />
    public partial class migrationforForUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ForUserId",
                schema: "dbo",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForUserId",
                schema: "dbo",
                table: "Notification");
        }
    }
}
