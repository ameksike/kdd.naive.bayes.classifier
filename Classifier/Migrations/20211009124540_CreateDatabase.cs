using Microsoft.EntityFrameworkCore.Migrations;

namespace Classifier.Migrations
{
    public partial class CreateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TopicDoc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    topic = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    docs = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicDoc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TopicWord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    topic = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    word = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    count = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicWord", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TopicDoc");

            migrationBuilder.DropTable(
                name: "TopicWord");
        }
    }
}
