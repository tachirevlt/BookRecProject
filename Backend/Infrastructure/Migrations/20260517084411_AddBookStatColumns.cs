using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookStatColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "average_rating",
                table: "Books",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "favorite_30d",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "favorite_7d",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "purchases_30d",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "purchases_7d",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_ratings",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "views_30d",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "views_7d",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "average_rating",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "favorite_30d",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "favorite_7d",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "purchases_30d",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "purchases_7d",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "total_ratings",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "views_30d",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "views_7d",
                table: "Books");
        }
    }
}
