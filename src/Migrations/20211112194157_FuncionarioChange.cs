using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaCadastro.Migrations
{
    public partial class FuncionarioChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Senha",
                table: "Funcionarios",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Senha",
                table: "Funcionarios");
        }
    }
}
