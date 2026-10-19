using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Students_Info_Api.Migrations
{
    /// <inheritdoc />
    public partial class tokengenerationdm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "PasswordResetToken",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "PasswordResetToken");
        }
    }
}
