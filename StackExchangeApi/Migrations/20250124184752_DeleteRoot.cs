using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StackExchangeApi.Migrations
{
    /// <inheritdoc />
    public partial class DeleteRoot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Roots_RootId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "Roots");

            migrationBuilder.DropIndex(
                name: "IX_Items_RootId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "RootId",
                table: "Items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RootId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Roots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HasMore = table.Column<bool>(type: "bit", nullable: false),
                    QuotaMax = table.Column<int>(type: "int", nullable: false),
                    QuotaRemaining = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_RootId",
                table: "Items",
                column: "RootId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Roots_RootId",
                table: "Items",
                column: "RootId",
                principalTable: "Roots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
