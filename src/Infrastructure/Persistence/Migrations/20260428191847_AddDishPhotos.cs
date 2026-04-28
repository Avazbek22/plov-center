using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlovCenter.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDishPhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "dishes");

            migrationBuilder.CreateTable(
                name: "dish_photos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DishId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelativePath = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dish_photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dish_photos_dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dish_photos_DishId_SortOrder",
                table: "dish_photos",
                columns: new[] { "DishId", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dish_photos");

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "dishes",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);
        }
    }
}
