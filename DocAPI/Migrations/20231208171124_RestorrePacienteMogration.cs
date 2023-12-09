using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAPI.Migrations
{
    public partial class RestorrePacienteMogration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultas_Consultorios_ConsultorioID",
                table: "Consultas");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultas_Pacientes_PacienteID",
                table: "Consultas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Consultas",
                table: "Consultas");

            migrationBuilder.DropIndex(
                name: "IX_Consultas_PacienteID",
                table: "Consultas");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "Consultas");

            migrationBuilder.UpdateData(
                table: "Consultas",
                keyColumn: "PacienteID",
                keyValue: null,
                column: "PacienteID",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "PacienteID",
                table: "Consultas",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Consultas",
                keyColumn: "ConsultorioID",
                keyValue: null,
                column: "ConsultorioID",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ConsultorioID",
                table: "Consultas",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Consultas",
                table: "Consultas",
                columns: new[] { "PacienteID", "ConsultorioID" });

            migrationBuilder.AddForeignKey(
                name: "FK_Consultas_Consultorios_ConsultorioID",
                table: "Consultas",
                column: "ConsultorioID",
                principalTable: "Consultorios",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultas_Pacientes_PacienteID",
                table: "Consultas",
                column: "PacienteID",
                principalTable: "Pacientes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultas_Consultorios_ConsultorioID",
                table: "Consultas");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultas_Pacientes_PacienteID",
                table: "Consultas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Consultas",
                table: "Consultas");

            migrationBuilder.AlterColumn<string>(
                name: "ConsultorioID",
                table: "Consultas",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "PacienteID",
                table: "Consultas",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ID",
                table: "Consultas",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Consultas",
                table: "Consultas",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_PacienteID",
                table: "Consultas",
                column: "PacienteID");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultas_Consultorios_ConsultorioID",
                table: "Consultas",
                column: "ConsultorioID",
                principalTable: "Consultorios",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultas_Pacientes_PacienteID",
                table: "Consultas",
                column: "PacienteID",
                principalTable: "Pacientes",
                principalColumn: "ID");
        }
    }
}
