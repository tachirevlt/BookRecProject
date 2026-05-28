using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewBookColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cost",
                table: "Books",
                newName: "price");

            migrationBuilder.AddColumn<string>(
                name: "badge",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "chapters",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "longDescription",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "mood",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "pages",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "previewText",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "readTime",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "badge",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "chapters",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "description",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "longDescription",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "mood",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "pages",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "previewText",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "readTime",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "Books",
                newName: "cost");
        }
    }
}
