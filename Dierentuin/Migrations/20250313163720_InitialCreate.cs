using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dierentuin.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                table: "Animals",
                columns: new[] { "Id", "ActivityPattern", "CategoryId", "DietaryClass", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species", "ZooId" },
                values: new object[,]
                {
                    { 5, 0, null, 1, null, true, "Marianne", 0, 5, 2.0121173016563718, "Licensed Frozen Chips", null },
                    { 9, 0, null, 4, null, true, "Rashad", 2, 2, 18.356800654888801, "Refined Fresh Chair", null },
                    { 12, 0, null, 3, null, true, "Jace", 0, 4, 6.2873170494429145, "Intelligent Rubber Mouse", null }
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
                    { 1, 1, 4, "Verblijf Automotive", 2, 63.919591142295523, null },
                    { 2, 1, 0, "Verblijf Home", 1, 249.46460679983841, null },
                    { 3, 1, 4, "Verblijf Sports", 0, 499.29931112016925, null },
                    { 4, 0, 8, "Verblijf Jewelery & Grocery", 0, 90.954737469493537, null }
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
                    { 1, 0, 4, 1, 4, false, "Ova", 2, 2, 6.6255988682813198, "Small Rubber Towels", null },
                    { 2, 0, null, 4, 2, true, "Franco", 2, 3, 32.915255617942101, "Small Cotton Cheese", null },
                    { 3, 1, 4, 1, 1, false, "Jeromy", 1, 4, 35.968096386475764, "Licensed Soft Car", null },
                    { 4, 0, 5, 1, null, false, "Ole", 0, 4, 45.080924441386031, "Fantastic Frozen Salad", null },
                    { 6, 1, 5, 1, 1, false, "Angelo", 2, 0, 11.036909445021319, "Awesome Wooden Shoes", null },
                    { 7, 0, 5, 1, null, true, "Cecelia", 2, 3, 43.406071383183786, "Small Cotton Computer", null },
                    { 8, 0, null, 0, 2, false, "Rudolph", 1, 0, 47.995743945981722, "Ergonomic Fresh Shirt", null },
                    { 10, 1, 5, 4, 4, true, "Margaret", 1, 3, 24.295974835702737, "Handmade Rubber Table", null },
                    { 11, 1, null, 1, 1, false, "Fredrick", 2, 3, 15.261966761217915, "Generic Wooden Pizza", null },
                    { 13, 2, 1, 2, 3, true, "Queen", 0, 4, 10.665850194973547, "Ergonomic Wooden Computer", null },
                    { 14, 2, 1, 2, 1, false, "Isaac", 0, 4, 37.016702220449844, "Gorgeous Plastic Mouse", null },
                    { 15, 0, 5, 1, 3, true, "Laney", 1, 2, 5.2729475521702005, "Refined Soft Chips", null }
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
