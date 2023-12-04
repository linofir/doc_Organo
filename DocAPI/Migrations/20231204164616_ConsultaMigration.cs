using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAPI.Migrations
{
    public partial class ConsultaMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultorios_Consultas_ConsultaID",
                table: "Consultorios");

            migrationBuilder.DropIndex(
                name: "IX_Consultorios_ConsultaID",
                table: "Consultorios");

            migrationBuilder.DropColumn(
                name: "ConsultaID",
                table: "Consultorios");

            migrationBuilder.AddColumn<string>(
                name: "ConsultorioID",
                table: "Consultas",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_ConsultorioID",
                table: "Consultas",
                column: "ConsultorioID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultas_Consultorios_ConsultorioID",
                table: "Consultas",
                column: "ConsultorioID",
                principalTable: "Consultorios",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultas_Consultorios_ConsultorioID",
                table: "Consultas");

            migrationBuilder.DropIndex(
                name: "IX_Consultas_ConsultorioID",
                table: "Consultas");

            migrationBuilder.DropColumn(
                name: "ConsultorioID",
                table: "Consultas");

            migrationBuilder.AddColumn<string>(
                name: "ConsultaID",
                table: "Consultorios",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Consultorios_ConsultaID",
                table: "Consultorios",
                column: "ConsultaID");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultorios_Consultas_ConsultaID",
                table: "Consultorios",
                column: "ConsultaID",
                principalTable: "Consultas",
                principalColumn: "ID");
        }
    }
}
