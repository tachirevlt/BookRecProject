using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFavoriteBooks_Books_FavoriteBooksbook_id",
                table: "UserFavoriteBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFavoriteBooks_Users_UserEntityUserId",
                table: "UserFavoriteBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPurchasedBooks_Books_PurchasedBooksbook_id",
                table: "UserPurchasedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPurchasedBooks_Users_UserEntity1UserId",
                table: "UserPurchasedBooks");

            migrationBuilder.RenameColumn(
                name: "Sex",
                table: "Users",
                newName: "sex");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "user_name");

            migrationBuilder.RenameColumn(
                name: "HashedPassword",
                table: "Users",
                newName: "hashed_password");

            migrationBuilder.RenameColumn(
                name: "CurrentBalance",
                table: "Users",
                newName: "current_balance");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UserEntity1UserId",
                table: "UserPurchasedBooks",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "PurchasedBooksbook_id",
                table: "UserPurchasedBooks",
                newName: "book_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserPurchasedBooks_UserEntity1UserId",
                table: "UserPurchasedBooks",
                newName: "IX_UserPurchasedBooks_user_id");

            migrationBuilder.RenameColumn(
                name: "UserEntityUserId",
                table: "UserFavoriteBooks",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "FavoriteBooksbook_id",
                table: "UserFavoriteBooks",
                newName: "book_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserFavoriteBooks_UserEntityUserId",
                table: "UserFavoriteBooks",
                newName: "IX_UserFavoriteBooks_user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Reviews",
                newName: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavoriteBooks_Books_book_id",
                table: "UserFavoriteBooks",
                column: "book_id",
                principalTable: "Books",
                principalColumn: "book_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavoriteBooks_Users_user_id",
                table: "UserFavoriteBooks",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPurchasedBooks_Books_book_id",
                table: "UserPurchasedBooks",
                column: "book_id",
                principalTable: "Books",
                principalColumn: "book_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPurchasedBooks_Users_user_id",
                table: "UserPurchasedBooks",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFavoriteBooks_Books_book_id",
                table: "UserFavoriteBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFavoriteBooks_Users_user_id",
                table: "UserFavoriteBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPurchasedBooks_Books_book_id",
                table: "UserPurchasedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPurchasedBooks_Users_user_id",
                table: "UserPurchasedBooks");

            migrationBuilder.RenameColumn(
                name: "sex",
                table: "Users",
                newName: "Sex");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "Users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "hashed_password",
                table: "Users",
                newName: "HashedPassword");

            migrationBuilder.RenameColumn(
                name: "current_balance",
                table: "Users",
                newName: "CurrentBalance");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserPurchasedBooks",
                newName: "UserEntity1UserId");

            migrationBuilder.RenameColumn(
                name: "book_id",
                table: "UserPurchasedBooks",
                newName: "PurchasedBooksbook_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserPurchasedBooks_user_id",
                table: "UserPurchasedBooks",
                newName: "IX_UserPurchasedBooks_UserEntity1UserId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserFavoriteBooks",
                newName: "UserEntityUserId");

            migrationBuilder.RenameColumn(
                name: "book_id",
                table: "UserFavoriteBooks",
                newName: "FavoriteBooksbook_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserFavoriteBooks_user_id",
                table: "UserFavoriteBooks",
                newName: "IX_UserFavoriteBooks_UserEntityUserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Reviews",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavoriteBooks_Books_FavoriteBooksbook_id",
                table: "UserFavoriteBooks",
                column: "FavoriteBooksbook_id",
                principalTable: "Books",
                principalColumn: "book_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavoriteBooks_Users_UserEntityUserId",
                table: "UserFavoriteBooks",
                column: "UserEntityUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPurchasedBooks_Books_PurchasedBooksbook_id",
                table: "UserPurchasedBooks",
                column: "PurchasedBooksbook_id",
                principalTable: "Books",
                principalColumn: "book_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPurchasedBooks_Users_UserEntity1UserId",
                table: "UserPurchasedBooks",
                column: "UserEntity1UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
