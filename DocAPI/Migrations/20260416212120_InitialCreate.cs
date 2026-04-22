using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tabela = table.Column<string>(type: "varchar(100)", nullable: false),
                    RegistroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Operacao = table.Column<string>(type: "varchar(30)", nullable: false),
                    ValorAnterior = table.Column<string>(type: "varchar(max)", nullable: true),
                    ValorNovo = table.Column<string>(type: "varchar(max)", nullable: true),
                    Usuario = table.Column<string>(type: "varchar(100)", nullable: false),
                    DataOperacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistDefinition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistDefinition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CID",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "varchar(10)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(300)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CID", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "ConciliacaoFinanceira",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Periodo = table.Column<DateTime>(type: "date", nullable: false),
                    JsonOriginal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConciliacaoFinanceira", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DemonstrativoFinanceiro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Periodo = table.Column<DateTime>(type: "date", nullable: false),
                    JsonPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemonstrativoFinanceiro", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Paciente",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Nascimento = table.Column<DateTime>(type: "date", nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    RG = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Plano = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Carteira = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Endereco_Logradouro = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Endereco_Numero = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Endereco_Bairro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Endereco_Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Endereco_UF = table.Column<string>(type: "nchar(2)", fixedLength: true, maxLength: 2, nullable: true),
                    Endereco_CEP = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Deletado = table.Column<bool>(type: "bit", nullable: false),
                    DeletadoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paciente", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItemDefinition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChecklistDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Obrigatorio = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistItemDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItemDefinition_ChecklistDefinition_ChecklistDefinitionId",
                        column: x => x.ChecklistDefinitionId,
                        principalTable: "ChecklistDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuiaFinanceira",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DemonstrativoFinanceiroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoGuia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TipoGuiaDescricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuiaFinanceira", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuiaFinanceira_DemonstrativoFinanceiro_DemonstrativoFinanceiroId",
                        column: x => x.DemonstrativoFinanceiroId,
                        principalTable: "DemonstrativoFinanceiro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Atendimento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EtapaAtual = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MensagemParaMedico = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Deletado = table.Column<bool>(type: "bit", nullable: false),
                    DeletadoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atendimento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Atendimento_Paciente_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Paciente",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemFinanceiro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GuiaFinanceiraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Lote = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Guia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Protocolo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NomeUsuario = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TipoUsuario = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Info = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Data = table.Column<DateTime>(type: "date", nullable: false),
                    ServicoCodigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ServicoDescricao = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Quantidade = table.Column<int>(type: "int", nullable: true),
                    ValorTabela = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    GrauParticipacao = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PercentualVia = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    HonorarioFator = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    ValorPago = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemFinanceiro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemFinanceiro_GuiaFinanceira_GuiaFinanceiraId",
                        column: x => x.GuiaFinanceiraId,
                        principalTable: "GuiaFinanceira",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AtendimentoPendencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtendimentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Resolvido = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtendimentoPendencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AtendimentoPendencias_Atendimento_AtendimentoId",
                        column: x => x.AtendimentoId,
                        principalTable: "Atendimento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistExecution",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChecklistDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtendimentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistExecution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistExecution_Atendimento_AtendimentoId",
                        column: x => x.AtendimentoId,
                        principalTable: "Atendimento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistExecution_ChecklistDefinition_ChecklistDefinitionId",
                        column: x => x.ChecklistDefinitionId,
                        principalTable: "ChecklistDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClinicalEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtendimentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoEvento = table.Column<string>(type: "varchar(30)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DataEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicalEvent_Atendimento_AtendimentoId",
                        column: x => x.AtendimentoId,
                        principalTable: "Atendimento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prontuario",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PacienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtendimentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Versao = table.Column<int>(type: "int", nullable: false),
                    ProntuarioAnteriorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Deletado = table.Column<bool>(type: "bit", nullable: false),
                    DeletadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataConsulta = table.Column<DateTime>(type: "date", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    InformacoesExtras = table.Column<string>(type: "varchar(max)", nullable: true),
                    DescricaoBasica_NomePaciente = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DescricaoBasica_Cpf = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    DescricaoBasica_Idade = table.Column<int>(type: "int", nullable: false),
                    DescricaoBasica_Profissao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DescricaoBasica_Religiao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DescricaoBasica_QD = table.Column<string>(type: "varchar(max)", nullable: false),
                    DescricaoBasica_AtividadeFisica = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Ago_Menarca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ago_DUM = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ago_Paridade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ago_DesejoGestacao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ago_VacinaHPV = table.Column<int>(type: "int", nullable: false),
                    Ago_Cco = table.Column<string>(type: "varchar(max)", nullable: false),
                    Ago_MAC_TRH = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ago_Intercorrencias = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Ago_Amamentacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ago_VidaSexual = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ago_Relacionamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ago_Parceiros = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ago_Coitarca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ago_IST = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Antecedentes_Comorbidades = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Antecedentes_Medicacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Antecedentes_Neoplasias = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Antecedentes_Cirurgias = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Antecedentes_Alergias = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Antecedentes_Vicios = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Antecedentes_HabitoIntestinal = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Antecedentes_Vacinas = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AntecedentesFamiliares_Neoplasias = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AntecedentesFamiliares_Comorbidades = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PosOp_PeriodoSeguimento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PosOp_Conclusao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PosOp_ExameMacro = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prontuario", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Prontuario_Atendimento_AtendimentoId",
                        column: x => x.AtendimentoId,
                        principalTable: "Atendimento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prontuario_Paciente_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Paciente",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prontuario_Prontuario_ProntuarioAnteriorId",
                        column: x => x.ProntuarioAnteriorId,
                        principalTable: "Prontuario",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemConciliacao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConciliacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemFinanceiroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtendimentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProcedimentoInternacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ValorEsperado = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ValorPago = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Diferenca = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemConciliacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemConciliacao_ConciliacaoFinanceira_ConciliacaoId",
                        column: x => x.ConciliacaoId,
                        principalTable: "ConciliacaoFinanceira",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemConciliacao_ItemFinanceiro_ItemFinanceiroId",
                        column: x => x.ItemFinanceiroId,
                        principalTable: "ItemFinanceiro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItemExecution",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChecklistExecutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChecklistItemDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Concluido = table.Column<bool>(type: "bit", nullable: false),
                    ConcluidoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConcluidoPor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistItemExecution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItemExecution_ChecklistExecution_ChecklistExecutionId",
                        column: x => x.ChecklistExecutionId,
                        principalTable: "ChecklistExecution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistItemExecution_ChecklistItemDefinition_ChecklistItemDefinitionId",
                        column: x => x.ChecklistItemDefinitionId,
                        principalTable: "ChecklistItemDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Exame",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProntuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "varchar(20)", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", nullable: false),
                    Status = table.Column<string>(type: "varchar(40)", nullable: true),
                    DataSolicitacao = table.Column<DateTime>(type: "date", nullable: true),
                    DataResultado = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exame", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Exame_Prontuario_ProntuarioId",
                        column: x => x.ProntuarioId,
                        principalTable: "Prontuario",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Internacao",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProntuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<DateTime>(type: "date", nullable: false),
                    IndicacaoClinica = table.Column<string>(type: "varchar(max)", nullable: false),
                    Observacao = table.Column<string>(type: "varchar(max)", nullable: false),
                    CIDCodigo = table.Column<string>(type: "varchar(10)", nullable: false),
                    TempoDoenca = table.Column<string>(type: "varchar(40)", nullable: false),
                    Diarias = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "varchar(20)", nullable: false),
                    Regime = table.Column<string>(type: "varchar(20)", nullable: false),
                    Carater = table.Column<string>(type: "varchar(20)", nullable: false),
                    UsaOPME = table.Column<bool>(type: "bit", nullable: false),
                    Local = table.Column<string>(type: "varchar(40)", nullable: false),
                    Guia = table.Column<string>(type: "varchar(40)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Internacao", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Internacao_CID_CIDCodigo",
                        column: x => x.CIDCodigo,
                        principalTable: "CID",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Internacao_Prontuario_ProntuarioId",
                        column: x => x.ProntuarioId,
                        principalTable: "Prontuario",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProntuarioAcaoCD",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProntuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProntuarioAcaoCD", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProntuarioAcaoCD_Prontuario_ProntuarioId",
                        column: x => x.ProntuarioId,
                        principalTable: "Prontuario",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agendamento",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InternacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtendimentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "varchar(20)", nullable: false),
                    Aviso = table.Column<string>(type: "varchar(20)", nullable: false),
                    Data = table.Column<DateTime>(type: "date", nullable: false),
                    Horario = table.Column<TimeSpan>(type: "time", nullable: false),
                    Local = table.Column<string>(type: "varchar(100)", nullable: false),
                    Sala = table.Column<string>(type: "varchar(10)", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", nullable: false),
                    InstrucaoStatus = table.Column<string>(type: "varchar(30)", nullable: false),
                    AtestadoStatus = table.Column<string>(type: "varchar(30)", nullable: false),
                    DataConsulta = table.Column<DateTime>(type: "date", nullable: false),
                    SenhaAgendamento_Codigo = table.Column<string>(type: "varchar(20)", nullable: true),
                    SenhaAgendamento_DataPedido = table.Column<DateTime>(type: "date", nullable: true),
                    SenhaAgendamento_DataLiberacao = table.Column<DateTime>(type: "date", nullable: true),
                    SenhaAgendamento_Validade = table.Column<DateTime>(type: "date", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deletado = table.Column<bool>(type: "bit", nullable: false),
                    DeletadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PacienteID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamento", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Agendamento_Atendimento_AtendimentoId",
                        column: x => x.AtendimentoId,
                        principalTable: "Atendimento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendamento_Internacao_InternacaoId",
                        column: x => x.InternacaoId,
                        principalTable: "Internacao",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendamento_Paciente_PacienteID",
                        column: x => x.PacienteID,
                        principalTable: "Paciente",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ProcedimentoInternacao",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InternacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodigoProcedimento = table.Column<string>(type: "varchar(20)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedimentoInternacao", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProcedimentoInternacao_Internacao_InternacaoId",
                        column: x => x.InternacaoId,
                        principalTable: "Internacao",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamento_AtendimentoId",
                table: "Agendamento",
                column: "AtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamento_Data_Horario",
                table: "Agendamento",
                columns: new[] { "Data", "Horario" });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamento_InternacaoId",
                table: "Agendamento",
                column: "InternacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamento_PacienteID",
                table: "Agendamento",
                column: "PacienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamento_Status",
                table: "Agendamento",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Atendimento_EtapaAtual",
                table: "Atendimento",
                column: "EtapaAtual");

            migrationBuilder.CreateIndex(
                name: "IX_Atendimento_PacienteId",
                table: "Atendimento",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_AtendimentoPendencias_AtendimentoId",
                table: "AtendimentoPendencias",
                column: "AtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_DataOperacao",
                table: "AuditLog",
                column: "DataOperacao");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_Tabela_RegistroId",
                table: "AuditLog",
                columns: new[] { "Tabela", "RegistroId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistExecution_AtendimentoId_ChecklistDefinitionId",
                table: "ChecklistExecution",
                columns: new[] { "AtendimentoId", "ChecklistDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistExecution_ChecklistDefinitionId",
                table: "ChecklistExecution",
                column: "ChecklistDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItemDefinition_ChecklistDefinitionId",
                table: "ChecklistItemDefinition",
                column: "ChecklistDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItemDefinition_ChecklistDefinitionId_Ordem",
                table: "ChecklistItemDefinition",
                columns: new[] { "ChecklistDefinitionId", "Ordem" });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItemExecution_ChecklistExecutionId",
                table: "ChecklistItemExecution",
                column: "ChecklistExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItemExecution_ChecklistExecutionId_ChecklistItemDefinitionId",
                table: "ChecklistItemExecution",
                columns: new[] { "ChecklistExecutionId", "ChecklistItemDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItemExecution_ChecklistItemDefinitionId",
                table: "ChecklistItemExecution",
                column: "ChecklistItemDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalEvent_AtendimentoId",
                table: "ClinicalEvent",
                column: "AtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalEvent_DataEvento",
                table: "ClinicalEvent",
                column: "DataEvento");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalEvent_TipoEvento",
                table: "ClinicalEvent",
                column: "TipoEvento");

            migrationBuilder.CreateIndex(
                name: "IX_ConciliacaoFinanceira_CriadoEm",
                table: "ConciliacaoFinanceira",
                column: "CriadoEm");

            migrationBuilder.CreateIndex(
                name: "IX_ConciliacaoFinanceira_Periodo",
                table: "ConciliacaoFinanceira",
                column: "Periodo");

            migrationBuilder.CreateIndex(
                name: "IX_Exame_DataSolicitacao",
                table: "Exame",
                column: "DataSolicitacao");

            migrationBuilder.CreateIndex(
                name: "IX_Exame_ProntuarioId",
                table: "Exame",
                column: "ProntuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Exame_Status",
                table: "Exame",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_GuiaFinanceira_DemonstrativoFinanceiroId",
                table: "GuiaFinanceira",
                column: "DemonstrativoFinanceiroId");

            migrationBuilder.CreateIndex(
                name: "IX_GuiaFinanceira_TipoGuia",
                table: "GuiaFinanceira",
                column: "TipoGuia");

            migrationBuilder.CreateIndex(
                name: "IX_Internacao_CIDCodigo",
                table: "Internacao",
                column: "CIDCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Internacao_Data",
                table: "Internacao",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_Internacao_ProntuarioId",
                table: "Internacao",
                column: "ProntuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemConciliacao_AtendimentoId",
                table: "ItemConciliacao",
                column: "AtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemConciliacao_AtendimentoId_Status",
                table: "ItemConciliacao",
                columns: new[] { "AtendimentoId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemConciliacao_ConciliacaoId",
                table: "ItemConciliacao",
                column: "ConciliacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemConciliacao_ConciliacaoId_Status",
                table: "ItemConciliacao",
                columns: new[] { "ConciliacaoId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemConciliacao_ItemFinanceiroId",
                table: "ItemConciliacao",
                column: "ItemFinanceiroId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemConciliacao_ItemFinanceiroId_ConciliacaoId",
                table: "ItemConciliacao",
                columns: new[] { "ItemFinanceiroId", "ConciliacaoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemConciliacao_ItemFinanceiroId_Status",
                table: "ItemConciliacao",
                columns: new[] { "ItemFinanceiroId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemConciliacao_ProcedimentoInternacaoId",
                table: "ItemConciliacao",
                column: "ProcedimentoInternacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemConciliacao_ProcedimentoInternacaoId_Status",
                table: "ItemConciliacao",
                columns: new[] { "ProcedimentoInternacaoId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemConciliacao_Status",
                table: "ItemConciliacao",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ItemConciliacao_Status_Diferenca",
                table: "ItemConciliacao",
                columns: new[] { "Status", "Diferenca" },
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_ItemFinanceiro_Data",
                table: "ItemFinanceiro",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFinanceiro_Guia",
                table: "ItemFinanceiro",
                column: "Guia");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFinanceiro_Guia_ServicoCodigo",
                table: "ItemFinanceiro",
                columns: new[] { "Guia", "ServicoCodigo" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemFinanceiro_GuiaFinanceiraId",
                table: "ItemFinanceiro",
                column: "GuiaFinanceiraId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFinanceiro_Lote",
                table: "ItemFinanceiro",
                column: "Lote");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFinanceiro_Protocolo",
                table: "ItemFinanceiro",
                column: "Protocolo");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFinanceiro_ServicoCodigo",
                table: "ItemFinanceiro",
                column: "ServicoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFinanceiro_ServicoCodigo_Data",
                table: "ItemFinanceiro",
                columns: new[] { "ServicoCodigo", "Data" });

            migrationBuilder.CreateIndex(
                name: "IX_Paciente_CPF",
                table: "Paciente",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Paciente_Email",
                table: "Paciente",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedimentoInternacao_InternacaoId",
                table: "ProcedimentoInternacao",
                column: "InternacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Prontuario_AtendimentoId",
                table: "Prontuario",
                column: "AtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Prontuario_PacienteId",
                table: "Prontuario",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Prontuario_PacienteId_Versao",
                table: "Prontuario",
                columns: new[] { "PacienteId", "Versao" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prontuario_ProntuarioAnteriorId",
                table: "Prontuario",
                column: "ProntuarioAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProntuarioAcaoCD_ProntuarioId",
                table: "ProntuarioAcaoCD",
                column: "ProntuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agendamento");

            migrationBuilder.DropTable(
                name: "AtendimentoPendencias");

            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "ChecklistItemExecution");

            migrationBuilder.DropTable(
                name: "ClinicalEvent");

            migrationBuilder.DropTable(
                name: "Exame");

            migrationBuilder.DropTable(
                name: "ItemConciliacao");

            migrationBuilder.DropTable(
                name: "ProcedimentoInternacao");

            migrationBuilder.DropTable(
                name: "ProntuarioAcaoCD");

            migrationBuilder.DropTable(
                name: "ChecklistExecution");

            migrationBuilder.DropTable(
                name: "ChecklistItemDefinition");

            migrationBuilder.DropTable(
                name: "ConciliacaoFinanceira");

            migrationBuilder.DropTable(
                name: "ItemFinanceiro");

            migrationBuilder.DropTable(
                name: "Internacao");

            migrationBuilder.DropTable(
                name: "ChecklistDefinition");

            migrationBuilder.DropTable(
                name: "GuiaFinanceira");

            migrationBuilder.DropTable(
                name: "CID");

            migrationBuilder.DropTable(
                name: "Prontuario");

            migrationBuilder.DropTable(
                name: "DemonstrativoFinanceiro");

            migrationBuilder.DropTable(
                name: "Atendimento");

            migrationBuilder.DropTable(
                name: "Paciente");
        }
    }
}
