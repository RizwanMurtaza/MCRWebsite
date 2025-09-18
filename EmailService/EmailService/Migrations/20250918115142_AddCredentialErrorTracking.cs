using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailService.Migrations
{
    /// <inheritdoc />
    public partial class AddCredentialErrorTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                table: "EmailQueueItems",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsCredentialError",
                table: "EmailQueueItems",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailureReason",
                table: "EmailQueueItems");

            migrationBuilder.DropColumn(
                name: "IsCredentialError",
                table: "EmailQueueItems");
        }
    }
}
