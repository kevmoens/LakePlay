using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LakePlay.Migrations
{
    public partial class AddOptionFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TriviaQuestionOption_Questions_TriviaQuestionId",
                table: "TriviaQuestionOption");

            migrationBuilder.AddForeignKey(
                name: "FK_TriviaQuestionOption_Questions_TriviaQuestionId",
                table: "TriviaQuestionOption",
                column: "TriviaQuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TriviaQuestionOption_Questions_TriviaQuestionId",
                table: "TriviaQuestionOption");

            migrationBuilder.AddForeignKey(
                name: "FK_TriviaQuestionOption_Questions_TriviaQuestionId",
                table: "TriviaQuestionOption",
                column: "TriviaQuestionId",
                principalTable: "Questions",
                principalColumn: "Id");
        }
    }
}
