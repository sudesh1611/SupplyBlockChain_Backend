using Microsoft.EntityFrameworkCore.Migrations;

namespace SupplyBlockChain_Backend.Migrations.ProductInfoDb
{
    public partial class SecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductsInfos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductName = table.Column<string>(nullable: true),
                    ProductType = table.Column<string>(nullable: true),
                    ProductID = table.Column<string>(nullable: true),
                    ProductCreator = table.Column<int>(nullable: false),
                    CreationDate = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsInfos", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductsInfos");
        }
    }
}
