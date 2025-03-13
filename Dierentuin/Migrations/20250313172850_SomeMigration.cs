using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dierentuin.Migrations
{
    /// <inheritdoc />
    public partial class SomeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zoos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zoos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Enclosures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Climate = table.Column<int>(type: "int", nullable: false),
                    HabitatType = table.Column<int>(type: "int", nullable: false),
                    SecurityLevel = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<double>(type: "float", nullable: false),
                    ZooId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enclosures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enclosures_Zoos_ZooId",
                        column: x => x.ZooId,
                        principalTable: "Zoos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Animals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Species = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Size = table.Column<int>(type: "int", nullable: false),
                    DietaryClass = table.Column<int>(type: "int", nullable: false),
                    ActivityPattern = table.Column<int>(type: "int", nullable: false),
                    EnclosureId = table.Column<int>(type: "int", nullable: true),
                    SpaceRequirement = table.Column<double>(type: "float", nullable: false),
                    SecurityRequirement = table.Column<int>(type: "int", nullable: false),
                    IsAwake = table.Column<bool>(type: "bit", nullable: false),
                    ZooId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animals_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Animals_Enclosures_EnclosureId",
                        column: x => x.EnclosureId,
                        principalTable: "Enclosures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Animals_Zoos_ZooId",
                        column: x => x.ZooId,
                        principalTable: "Zoos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AnimalAnimal",
                columns: table => new
                {
                    AnimalId = table.Column<int>(type: "int", nullable: false),
                    PreyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalAnimal", x => new { x.AnimalId, x.PreyId });
                    table.ForeignKey(
                        name: "FK_AnimalAnimal_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalAnimal_Animals_PreyId",
                        column: x => x.PreyId,
                        principalTable: "Animals",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Zoogdieren" },
                    { 2, "Vogels" },
                    { 3, "Reptielen" },
                    { 4, "Insecten" },
                    { 5, "Amfibieën" }
                });

            migrationBuilder.InsertData(
                table: "Enclosures",
                columns: new[] { "Id", "Climate", "HabitatType", "Name", "SecurityLevel", "Size", "ZooId" },
                values: new object[,]
                {
                    { 1, 0, 0, "Verblijf Outdoors, Home & Kids", 2, 294.56150201904995, null },
                    { 2, 1, 8, "Verblijf Home", 1, 292.42826669770767, null },
                    { 3, 0, 2, "Verblijf Grocery", 0, 354.56484657522913, null },
                    { 4, 2, 2, "Verblijf Electronics & Automotive", 0, 300.37898042568861, null }
                });

            migrationBuilder.InsertData(
                table: "Zoos",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Bogus Dierentuin" });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "ActivityPattern", "CategoryId", "DietaryClass", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species", "ZooId" },
                values: new object[,]
                {
                    { 1, 1, 2, 3, 4, false, "Quinton", 1, 2, 17.061462915264364, "Sleek Frozen Ball", null },
                    { 2, 1, 2, 4, 2, false, "Luisa", 1, 4, 4.8233524841469144, "Intelligent Metal Tuna", null },
                    { 3, 2, 2, 4, 3, true, "Irwin", 0, 1, 18.323993091912502, "Unbranded Granite Gloves", null },
                    { 4, 0, 5, 2, null, true, "Maximilian", 1, 0, 6.0893575872574406, "Handmade Soft Ball", null },
                    { 5, 1, null, 0, 3, false, "Blake", 2, 4, 45.195269010161297, "Gorgeous Steel Salad", null },
                    { 6, 0, null, 2, 1, false, "Cooper", 1, 2, 24.515541356937074, "Ergonomic Frozen Chicken", null },
                    { 7, 1, 2, 1, 4, true, "Adella", 1, 0, 5.8608185240561932, "Refined Cotton Bike", null },
                    { 8, 0, 5, 0, null, false, "Bertram", 2, 3, 27.825042189699275, "Incredible Concrete Shoes", null },
                    { 9, 1, 2, 2, 1, true, "Rozella", 2, 0, 6.3044910729036019, "Generic Cotton Ball", null },
                    { 10, 2, 3, 1, 1, true, "Robin", 0, 1, 48.921927422109214, "Handcrafted Steel Pants", null },
                    { 11, 0, 4, 1, 4, true, "Abraham", 2, 3, 47.586565004404491, "Practical Granite Shoes", null },
                    { 12, 0, 1, 4, null, true, "Bernie", 2, 0, 49.882688286090435, "Unbranded Wooden Computer", null },
                    { 13, 2, 1, 2, 4, true, "Roslyn", 1, 5, 48.481234028365563, "Handcrafted Wooden Keyboard", null },
                    { 14, 2, 5, 0, 1, true, "Rowena", 0, 5, 45.497496432700451, "Licensed Rubber Sausages", null },
                    { 15, 2, 2, 4, 2, false, "Casper", 0, 2, 39.367417353890112, "Generic Metal Car", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalAnimal_PreyId",
                table: "AnimalAnimal",
                column: "PreyId");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_CategoryId",
                table: "Animals",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_EnclosureId",
                table: "Animals",
                column: "EnclosureId");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_ZooId",
                table: "Animals",
                column: "ZooId");

            migrationBuilder.CreateIndex(
                name: "IX_Enclosures_ZooId",
                table: "Enclosures",
                column: "ZooId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalAnimal");

            migrationBuilder.DropTable(
                name: "Animals");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Enclosures");

            migrationBuilder.DropTable(
                name: "Zoos");
        }
    }
}
