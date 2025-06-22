using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TriviaForCheeseHeads.Migrations
{
    public partial class OptionFKchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TriviaQuestionOption");

            migrationBuilder.CreateTable(
                name: "TriviaQuestionOptions",
                columns: table => new
                {
                    QuestionId = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    IsAnswer = table.Column<bool>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriviaQuestionOptions", x => new { x.Id, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_TriviaQuestionOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TriviaQuestionOptions_QuestionId",
                table: "TriviaQuestionOptions",
                column: "QuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TriviaQuestionOptions");

            migrationBuilder.CreateTable(
                name: "TriviaQuestionOption",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    IsAnswer = table.Column<bool>(type: "INTEGER", nullable: false),
                    QuestionId = table.Column<string>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    TriviaQuestionId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriviaQuestionOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TriviaQuestionOption_Questions_TriviaQuestionId",
                        column: x => x.TriviaQuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TriviaQuestionOption_TriviaQuestionId",
                table: "TriviaQuestionOption",
                column: "TriviaQuestionId");
        }
    }
}
