using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebApp.Migrations
{
    /// <inheritdoc />
    public partial class migrationforlivennotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "MessageType",
                schema: "dbo",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                schema: "dbo",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);

         

            migrationBuilder.CreateTable(
                name: "HubConnections",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HubConnections", x => x.Id);
                });

        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "HubConnections",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "MessageType",
                schema: "dbo",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "Username",
                schema: "dbo",
                table: "Notification");

           }
    }
}
