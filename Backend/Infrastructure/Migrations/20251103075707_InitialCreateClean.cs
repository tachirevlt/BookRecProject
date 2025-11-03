using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tag_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    year = table.Column<double>(type: "float", nullable: true),
                    books_count = table.Column<int>(type: "int", nullable: true),
                    work_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isbn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    language_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    average_rating = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: true),
                    ratings = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
