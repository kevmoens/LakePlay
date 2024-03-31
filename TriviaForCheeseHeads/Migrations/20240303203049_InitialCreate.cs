using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriviaForCheeseHeads.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    Used = table.Column<bool>(type: "INTEGER", nullable: false),
                    AskedThisRound = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TriviaQuestionOption",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    QuestionId = table.Column<string>(type: "TEXT", nullable: false),
                    IsAnswer = table.Column<bool>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    TriviaQuestionId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriviaQuestionOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TriviaQuestionOption_Questions_TriviaQuestionId",
                        column: x => x.TriviaQuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TriviaQuestionOption_TriviaQuestionId",
                table: "TriviaQuestionOption",
                column: "TriviaQuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TriviaQuestionOption");

            migrationBuilder.DropTable(
                name: "Questions");
        }
    }
}
