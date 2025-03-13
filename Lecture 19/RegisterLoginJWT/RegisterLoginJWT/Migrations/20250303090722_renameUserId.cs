using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegisterLoginJWT.Migrations
{
    /// <inheritdoc />
    public partial class renameUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Users",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "ID");
        }
    }
}
