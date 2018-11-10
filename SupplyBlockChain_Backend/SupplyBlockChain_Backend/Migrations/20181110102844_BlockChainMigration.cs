using Microsoft.EntityFrameworkCore.Migrations;

namespace SupplyBlockChain_Backend.Migrations
{
    public partial class BlockChainMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllBlockChains",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Difficulty = table.Column<int>(nullable: false),
                    SupplyBlockChain = table.Column<string>(nullable: true),
                    Verifier_1BlockChain = table.Column<string>(nullable: true),
                    Verifier_2BlockChain = table.Column<string>(nullable: true),
                    Verifier_3BlockChain = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllBlockChains", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllBlockChains");
        }
    }
}
