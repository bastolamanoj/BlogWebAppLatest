using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpadatedBlogcategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogBlogCategory",
                schema: "dbo");

            migrationBuilder.AddColumn<Guid>(
                name: "BlogId",
                schema: "dbo",
                table: "BlogCategories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategories_BlogId",
                schema: "dbo",
                table: "BlogCategories",
                column: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogCategories_Blogs_BlogId",
                schema: "dbo",
                table: "BlogCategories",
                column: "BlogId",
                principalSchema: "dbo",
                principalTable: "Blogs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogCategories_Blogs_BlogId",
                schema: "dbo",
                table: "BlogCategories");

            migrationBuilder.DropIndex(
                name: "IX_BlogCategories_BlogId",
                schema: "dbo",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "BlogId",
                schema: "dbo",
                table: "BlogCategories");

            migrationBuilder.CreateTable(
                name: "BlogBlogCategory",
                schema: "dbo",
                columns: table => new
                {
                    BlogCategoriesId = table.Column<int>(type: "int", nullable: false),
                    BlogsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogBlogCategory", x => new { x.BlogCategoriesId, x.BlogsId });
                    table.ForeignKey(
                        name: "FK_BlogBlogCategory_BlogCategories_BlogCategoriesId",
                        column: x => x.BlogCategoriesId,
                        principalSchema: "dbo",
                        principalTable: "BlogCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogBlogCategory_Blogs_BlogsId",
                        column: x => x.BlogsId,
                        principalSchema: "dbo",
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogBlogCategory_BlogsId",
                schema: "dbo",
                table: "BlogBlogCategory",
                column: "BlogsId");
        }
    }
}
