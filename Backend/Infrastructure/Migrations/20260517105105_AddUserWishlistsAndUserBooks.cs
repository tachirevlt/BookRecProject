using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserWishlistsAndUserBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavoriteBooks");

            migrationBuilder.DropTable(
                name: "UserPurchasedBooks");

            migrationBuilder.CreateTable(
                name: "UserBooks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    book_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    purchase_price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    purchase_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    current_chapter = table.Column<int>(type: "int", nullable: false),
                    last_read_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBooks", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserBooks_Books_book_id",
                        column: x => x.book_id,
                        principalTable: "Books",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBooks_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    book_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    added_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    price_at_addition = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    collection_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.id);
                    table.ForeignKey(
                        name: "FK_Wishlists_Books_book_id",
                        column: x => x.book_id,
                        principalTable: "Books",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wishlists_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBooks_book_id",
                table: "UserBooks",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserBooks_user_id",
                table: "UserBooks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_book_id",
                table: "Wishlists",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_user_id",
                table: "Wishlists",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBooks");

            migrationBuilder.DropTable(
                name: "Wishlists");

            migrationBuilder.CreateTable(
                name: "UserFavoriteBooks",
                columns: table => new
                {
                    book_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteBooks", x => new { x.book_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_UserFavoriteBooks_Books_book_id",
                        column: x => x.book_id,
                        principalTable: "Books",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteBooks_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPurchasedBooks",
                columns: table => new
                {
                    book_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPurchasedBooks", x => new { x.book_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_UserPurchasedBooks_Books_book_id",
                        column: x => x.book_id,
                        principalTable: "Books",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPurchasedBooks_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteBooks_user_id",
                table: "UserFavoriteBooks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserPurchasedBooks_user_id",
                table: "UserPurchasedBooks",
                column: "user_id");
        }
    }
}
