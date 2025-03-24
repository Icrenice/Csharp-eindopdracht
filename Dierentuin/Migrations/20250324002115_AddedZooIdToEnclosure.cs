using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dierentuin.Migrations
{
    /// <inheritdoc />
    public partial class AddedZooIdToEnclosure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enclosures_Zoos_ZooId",
                table: "Enclosures");

            migrationBuilder.AlterColumn<int>(
                name: "ZooId",
                table: "Enclosures",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ActivityPattern", "CategoryId", "EnclosureId", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 2, 5, 2, "Ottis", 2, 0, 45.193264691454019, "Incredible Soft Mouse" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ActivityPattern", "CategoryId", "DietaryClass", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 2, 1, 0, 4, true, "Stone", 0, 3, 31.594854863782611, "Small Fresh Soap" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ActivityPattern", "CategoryId", "DietaryClass", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "SpaceRequirement", "Species" },
                values: new object[] { 0, 3, 1, 4, false, "Holden", 2, 8.1866486963561336, "Intelligent Frozen Chicken" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "DietaryClass", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 2, 1, 1, false, "Ubaldo", 2, 2, 39.179955008238338, "Generic Granite Hat" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ActivityPattern", "CategoryId", "DietaryClass", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 0, 4, 1, "Beau", 1, 0, 24.463827750056687, "Awesome Wooden Chair" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ActivityPattern", "DietaryClass", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 1, 3, 3, true, "Maida", 0, 4, 32.936638637482673, "Gorgeous Granite Chair" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ActivityPattern", "DietaryClass", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 2, 4, null, false, "Ron", 0, 2, 5.8598056935774903, "Fantastic Metal Keyboard" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ActivityPattern", "CategoryId", "EnclosureId", "Name", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 2, null, 1, "Winifred", 2, 40.037743140472138, "Tasty Concrete Chair" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ActivityPattern", "CategoryId", "DietaryClass", "EnclosureId", "Name", "SecurityRequirement", "SpaceRequirement", "Species" },
                values: new object[] { 0, 3, 4, 3, "Eunice", 1, 42.366784166115913, "Handcrafted Granite Table" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CategoryId", "DietaryClass", "EnclosureId", "Name", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 2, 0, null, "Annamae", 5, 19.726876013732753, "Fantastic Steel Mouse" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CategoryId", "DietaryClass", "EnclosureId", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { null, 4, null, "Caterina", 0, 4, 13.682969977824321, "Small Cotton Chicken" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CategoryId", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "SpaceRequirement", "Species" },
                values: new object[] { 4, 4, false, "Gunner", 0, 40.568483333383099, "Tasty Concrete Chicken" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CategoryId", "EnclosureId", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 2, null, "Tavares", 0, 0, 43.480951248312728, "Intelligent Metal Bike" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DietaryClass", "EnclosureId", "IsAwake", "Name", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 1, null, false, "Mustafa", 2, 48.84502351692494, "Licensed Plastic Ball" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "ActivityPattern", "DietaryClass", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 1, 2, 4, true, "Alexie", 1, 3, 43.612421563965235, "Tasty Rubber Bacon" });

            migrationBuilder.UpdateData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Climate", "HabitatType", "Name", "Size", "ZooId" },
                values: new object[] { 2, 8, "Verblijf Tools, Games & Games", 297.63126551001062, 1 });

            migrationBuilder.UpdateData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "HabitatType", "Name", "SecurityLevel", "Size", "ZooId" },
                values: new object[] { 4, "Verblijf Home, Garden & Games", 2, 422.86658100267965, 1 });

            migrationBuilder.UpdateData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Climate", "HabitatType", "Name", "Size", "ZooId" },
                values: new object[] { 1, 8, "Verblijf Shoes & Industrial", 137.66378554093046, 1 });

            migrationBuilder.UpdateData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "HabitatType", "Name", "Size", "ZooId" },
                values: new object[] { 1, "Verblijf Sports, Garden & Beauty", 117.37705303934428, 1 });

            migrationBuilder.AddForeignKey(
                name: "FK_Enclosures_Zoos_ZooId",
                table: "Enclosures",
                column: "ZooId",
                principalTable: "Zoos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enclosures_Zoos_ZooId",
                table: "Enclosures");

            migrationBuilder.AlterColumn<int>(
                name: "ZooId",
                table: "Enclosures",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ActivityPattern", "CategoryId", "EnclosureId", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 1, 2, 4, "Quinton", 1, 2, 17.061462915264364, "Sleek Frozen Ball" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ActivityPattern", "CategoryId", "DietaryClass", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 1, 2, 4, 2, false, "Luisa", 1, 4, 4.8233524841469144, "Intelligent Metal Tuna" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ActivityPattern", "CategoryId", "DietaryClass", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "SpaceRequirement", "Species" },
                values: new object[] { 2, 2, 4, 3, true, "Irwin", 0, 18.323993091912502, "Unbranded Granite Gloves" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "DietaryClass", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 5, 2, null, true, "Maximilian", 1, 0, 6.0893575872574406, "Handmade Soft Ball" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ActivityPattern", "CategoryId", "DietaryClass", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 1, null, 0, "Blake", 2, 4, 45.195269010161297, "Gorgeous Steel Salad" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ActivityPattern", "DietaryClass", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 0, 2, 1, false, "Cooper", 1, 2, 24.515541356937074, "Ergonomic Frozen Chicken" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ActivityPattern", "DietaryClass", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 1, 1, 4, true, "Adella", 1, 0, 5.8608185240561932, "Refined Cotton Bike" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ActivityPattern", "CategoryId", "EnclosureId", "Name", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 0, 5, null, "Bertram", 3, 27.825042189699275, "Incredible Concrete Shoes" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ActivityPattern", "CategoryId", "DietaryClass", "EnclosureId", "Name", "SecurityRequirement", "SpaceRequirement", "Species" },
                values: new object[] { 1, 2, 2, 1, "Rozella", 2, 6.3044910729036019, "Generic Cotton Ball" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CategoryId", "DietaryClass", "EnclosureId", "Name", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 3, 1, 1, "Robin", 1, 48.921927422109214, "Handcrafted Steel Pants" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CategoryId", "DietaryClass", "EnclosureId", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 4, 1, 4, "Abraham", 2, 3, 47.586565004404491, "Practical Granite Shoes" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CategoryId", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "SpaceRequirement", "Species" },
                values: new object[] { 1, null, true, "Bernie", 2, 49.882688286090435, "Unbranded Wooden Computer" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CategoryId", "EnclosureId", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 1, 4, "Roslyn", 1, 5, 48.481234028365563, "Handcrafted Wooden Keyboard" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DietaryClass", "EnclosureId", "IsAwake", "Name", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 0, 1, true, "Rowena", 5, 45.497496432700451, "Licensed Rubber Sausages" });

            migrationBuilder.UpdateData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "ActivityPattern", "DietaryClass", "EnclosureId", "IsAwake", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species" },
                values: new object[] { 2, 4, 2, false, "Casper", 0, 2, 39.367417353890112, "Generic Metal Car" });

            migrationBuilder.UpdateData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Climate", "HabitatType", "Name", "Size", "ZooId" },
                values: new object[] { 0, 0, "Verblijf Outdoors, Home & Kids", 294.56150201904995, null });

            migrationBuilder.UpdateData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "HabitatType", "Name", "SecurityLevel", "Size", "ZooId" },
                values: new object[] { 8, "Verblijf Home", 1, 292.42826669770767, null });

            migrationBuilder.UpdateData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Climate", "HabitatType", "Name", "Size", "ZooId" },
                values: new object[] { 0, 2, "Verblijf Grocery", 354.56484657522913, null });

            migrationBuilder.UpdateData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "HabitatType", "Name", "Size", "ZooId" },
                values: new object[] { 2, "Verblijf Electronics & Automotive", 300.37898042568861, null });

            migrationBuilder.AddForeignKey(
                name: "FK_Enclosures_Zoos_ZooId",
                table: "Enclosures",
                column: "ZooId",
                principalTable: "Zoos",
                principalColumn: "Id");
        }
    }
}
