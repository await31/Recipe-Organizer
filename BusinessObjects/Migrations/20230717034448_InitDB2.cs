using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class InitDB2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nutrition_Ingredients_IngredientId",
                table: "Nutrition");

            migrationBuilder.DropForeignKey(
                name: "FK_Nutrition_Recipes_Id",
                table: "Nutrition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Nutrition",
                table: "Nutrition");

            migrationBuilder.RenameTable(
                name: "Nutrition",
                newName: "Nutritions");

            migrationBuilder.RenameIndex(
                name: "IX_Nutrition_IngredientId",
                table: "Nutritions",
                newName: "IX_Nutritions_IngredientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Nutritions",
                table: "Nutritions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Nutritions_Ingredients_IngredientId",
                table: "Nutritions",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Nutritions_Recipes_Id",
                table: "Nutritions",
                column: "Id",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nutritions_Ingredients_IngredientId",
                table: "Nutritions");

            migrationBuilder.DropForeignKey(
                name: "FK_Nutritions_Recipes_Id",
                table: "Nutritions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Nutritions",
                table: "Nutritions");

            migrationBuilder.RenameTable(
                name: "Nutritions",
                newName: "Nutrition");

            migrationBuilder.RenameIndex(
                name: "IX_Nutritions_IngredientId",
                table: "Nutrition",
                newName: "IX_Nutrition_IngredientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Nutrition",
                table: "Nutrition",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Nutrition_Ingredients_IngredientId",
                table: "Nutrition",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Nutrition_Recipes_Id",
                table: "Nutrition",
                column: "Id",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
