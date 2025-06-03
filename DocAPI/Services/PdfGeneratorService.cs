using QuestPDF.Fluent; // Certifique-se de ter QuestPDF instalado via NuGet
using QuestPDF.Helpers;
using QuestPDF.Previewer; // Opcional para desenvolvimento
using System.IO;
using DocAPI.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace DocAPI.Services;
public class PdfGeneratorService 
{
    // Não precisa injetar repositórios aqui. Ele apenas constrói o PDF.
    public Stream GeneratePatientReportPdf(Paciente paciente, List<Prontuario> prontuarios)
    {
        Console.WriteLine($"Gerando relatorio para:{paciente.ID}  ");
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10)); // Ajuste de tamanho de fonte padrão para mais espaço

                page.Header()
                    .PaddingBottom(10)
                    .Column(column =>
                    {
                        column.Item().Text("Relatório Médico da Paciente")
                            .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                page.Content()
                    .PaddingVertical(8)
                    .Column(column =>
                    {
                        column.Spacing(16); // Espaçamento entre as seções principais

                        // --- SEÇÃO 1: DADOS DO PACIENTE ---
                        column.Item().Text("Dados da Paciente").SemiBold().FontSize(14).Underline();
                        column.Item().PaddingLeft(8).Border(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Column(patientInfo =>
                        {
                            patientInfo.Spacing(4);
                            patientInfo.Item().Text($"Nome: {paciente.Nome}");
                            patientInfo.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                            patientInfo.Item().Text($"CPF: {paciente.CPF}");
                            patientInfo.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                            patientInfo.Item().Text($"Nascimento: {paciente.Nascimento:dd/MM/yyyy} (Idade: {CalculateAge(paciente.Nascimento)})");
                            patientInfo.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                            patientInfo.Item().Text($"RG: {paciente.RG}");
                            patientInfo.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                            patientInfo.Item().Text($"Email: {paciente.Email}");
                            patientInfo.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                            patientInfo.Item().Text($"Telefone: {paciente.Telefone}");
                            patientInfo.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                            patientInfo.Item().Text($"Plano: {paciente.Plano} (Carteira: {paciente.Carteira})");
                            patientInfo.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                            if (paciente.Endereco != null)
                            {
                                patientInfo.Item().Text($"Endereço: {paciente.Endereco.Logradouro}, {paciente.Endereco.Numero} - {paciente.Endereco.Bairro}, {paciente.Endereco.Cidade} - {paciente.Endereco.UF}, CEP: {paciente.Endereco.CEP} ");
                                //patientInfo.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten3);
                                // patientInfo.Item().PaddingLeft(10).Text($"{paciente.Endereco.Logradouro}, {paciente.Endereco.Numero}");
                                // patientInfo.Item().PaddingLeft(10).Text($"{paciente.Endereco.Bairro}, {paciente.Endereco.Cidade} - {paciente.Endereco.UF}, CEP: {paciente.Endereco.CEP}");
                            }
                        });

                        //column.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3); // Separador

                        // --- SEÇÃO 2: DADOS DOS PRONTUÁRIOS ---
                        column.Item().Text("Prontuários").SemiBold().FontSize(14).Underline();

                        if (prontuarios != null && prontuarios.Any())
                        {
                            // Ordena os prontuários por data mais recente
                            foreach (var prontuario in prontuarios.OrderByDescending(p => p.DataRequisicao))
                            {
                                column.Item().PaddingLeft(8).Border(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Column(prontuarioDetails =>
                                {
                                    prontuarioDetails.Spacing(8);

                                    prontuarioDetails.Item().Text($"Prontuário ID: {prontuario.ID} | Data da Requisição: {prontuario.DataRequisicao:dd/MM/yyyy}");
                                    prontuarioDetails.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten4); // Separador dentro do prontuário

                                    // 2.1 Descrição Básica
                                    prontuarioDetails.Item().Text("Descrição Básica").SemiBold().FontSize(12);
                                    prontuarioDetails.Item().PaddingLeft(10).Column(descBasica =>
                                    {
                                        descBasica.Spacing(4);
                                        //descBasica.Item().Text($"Nome Paciente Prontuário: {prontuario.DescricaoBasica.NomePaciente}");
                                        //descBasica.Item().Text($"CPF: {prontuario.DescricaoBasica.Cpf}");
                                        descBasica.Item().Text($"Idade: {prontuario.DescricaoBasica!.Idade}");
                                        descBasica.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        descBasica.Item().Text($"Profissão: {prontuario.DescricaoBasica.Profissao}");
                                        descBasica.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        descBasica.Item().Text($"Religião: {prontuario.DescricaoBasica.Religiao}");
                                        descBasica.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        descBasica.Item().Text($"Queixa Principal (QD): {prontuario.DescricaoBasica.QD}");
                                        descBasica.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        descBasica.Item().Text($"Atividade Física: {prontuario.DescricaoBasica.AtividadeFisica}");
                                    });

                                    // 2.2 AGO (Antecedentes Ginecológicos e Obstétricos)
                                    prontuarioDetails.Item().Text("AGO").SemiBold().FontSize(12);
                                    prontuarioDetails.Item().PaddingLeft(8).Column(ago =>
                                    {
                                        ago.Spacing(3);
                                        ago.Item().Text($"Menarca: {prontuario.AGO!.Menarca}");
                                        ago.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ago.Item().Text($"DUM: {prontuario.AGO.DUM}");
                                        ago.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ago.Item().Text($"Paridade: {prontuario.AGO.Paridade}");
                                        ago.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ago.Item().Text($"Desejo Gestação: {prontuario.AGO.DesejoGestacao}");
                                        ago.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ago.Item().Text($"Intercorrências: {prontuario.AGO.Intercorrencias}");
                                        ago.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ago.Item().Text($"Amamentação: {prontuario.AGO.Amamentacao}");
                                        ago.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ago.Item().Text($"Vida Sexual: {prontuario.AGO.VidaSexual}");
                                        ago.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ago.Item().Text($"Relacionamento: {prontuario.AGO.Relacionamento}");
                                        ago.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ago.Item().Text($"Parceiros: {prontuario.AGO.Parceiros}");
                                        ago.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ago.Item().Text($"Coitarca: {prontuario.AGO.Coitarca}");
                                        ago.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ago.Item().Text($"IST: {prontuario.AGO.IST}");
                                        // Para exibir o DisplayName do enum
                                        ago.Item().Text($"Vacina HPV: {GetEnumDisplayName(prontuario.AGO.VacinaHPV)}");
                                        ago.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ago.Item().Text($"CCO: {prontuario.AGO.CCO}");
                                        ago.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ago.Item().Text($"MAC/TRH: {prontuario.AGO.MAC_TRH}");
                                        ago.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                    });

                                    // 2.3 Antecedentes 
                                    prontuarioDetails.Item().Text("Antecedentes").SemiBold().FontSize(12);
                                    prontuarioDetails.Item().PaddingLeft(8).Column(ant =>
                                    {
                                        ant.Spacing(3);
                                        ant.Item().Text($"Comorbidades: {prontuario.Antecedentes!.Comorbidades}");
                                        ant.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ant.Item().Text($"Medicação: {prontuario.Antecedentes.Medicacao}");
                                        ant.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ant.Item().Text($"Neoplasias: {prontuario.Antecedentes.Neoplasias}");
                                        ant.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ant.Item().Text($"Cirurgias: {prontuario.Antecedentes.Cirurgias}");
                                        ant.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ant.Item().Text($"Alergias: {prontuario.Antecedentes.Alergias}");
                                        ant.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ant.Item().Text($"Vícios: {prontuario.Antecedentes.Vicios}");
                                        ant.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ant.Item().Text($"Hábito Intestinal: {prontuario.Antecedentes.HabitoIntestinal}");
                                        ant.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        ant.Item().Text($"Vacinas: {prontuario.Antecedentes.Vacinas}");
                                        ant.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                    });

                                    // 2.4 Antecedentes Familiares
                                    prontuarioDetails.Item().Text("2.4 Antecedentes Familiares").SemiBold().FontSize(12);
                                    prontuarioDetails.Item().PaddingLeft(8).Column(antFam =>
                                    {
                                        antFam.Spacing(3);
                                        antFam.Item().Text($"Neoplasias: {prontuario.AntecedentesFamiliares!.Neoplasias}");
                                        antFam.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                        antFam.Item().Text($"Comorbidades: {prontuario.AntecedentesFamiliares.Comorbidades}");
                                        antFam.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                    });

                                    // 2.5 Documentos (CD)
                                    prontuarioDetails.Item().Text("CD").SemiBold().FontSize(12);
                                    if (prontuario.CD != null && prontuario.CD.Any())
                                    {
                                        prontuarioDetails.Item().PaddingLeft(8).Column(cd =>
                                        {
                                            foreach (var acao in prontuario.CD)
                                            {
                                                cd.Item().Text($"- {GetEnumDisplayName(acao)}");
                                                cd.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                            }
                                        });
                                    }
                                    else
                                    {
                                        prontuarioDetails.Item().PaddingLeft(8).Text("Nenhum documento registrado.");
                                    }

                                    // 2.6 Informações Extras
                                    prontuarioDetails.Item().Text("Informações Extras").SemiBold().FontSize(12);
                                    prontuarioDetails.Item().PaddingLeft(8).Text($"{prontuario.InformacoesExtras}");
                                    prontuarioDetails.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                    

                                    // 2.7 Exames
                                    prontuarioDetails.Item().Text("Exames").SemiBold().FontSize(12);
                                    if (prontuario.Exames != null && prontuario.Exames.Any())
                                    {
                                        prontuarioDetails.Item().PaddingLeft(10).Column(exames =>
                                        {
                                            if (prontuario.Exames != null)
                                            {
                                            foreach (var exame in prontuario.Exames)
                                            {
                                                exames.Item().Text($"- {exame.Nome} (Código: {exame.Codigo})");
                                                exames.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                            }
                                            }
                                            else
                                            {
                                                exames.Item().Text("Nenhuma solicitação de internação registrada.");
                                            }
                                        });
                                    }
                                    else
                                    {
                                        prontuarioDetails.Item().PaddingLeft(8).Text("Nenhum exame registrado.");
                                    }

                                    // 2.8 Solicitação de Internação
                                    prontuarioDetails.Item().Text("Solicitação de Internação").SemiBold().FontSize(12);
                                    prontuarioDetails.Item().PaddingLeft(8).Column(solInternacao =>
                                    {
                                        if (prontuario.SolicitacaoInternacao != null)
                                        {
                                            solInternacao.Spacing(3);

                                            solInternacao.Item().Text($"Indicação Clínica: {prontuario.SolicitacaoInternacao.IndicacaoClinica}");
                                            solInternacao.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                            solInternacao.Item().Text($"Observação: {prontuario.SolicitacaoInternacao.Observacao}");
                                             solInternacao.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                            solInternacao.Item().Text($"CID: {prontuario.SolicitacaoInternacao.CID}");
                                            solInternacao.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                            solInternacao.Item().Text($"Tempo de Doença: {prontuario.SolicitacaoInternacao.TempoDoenca}");
                                            solInternacao.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                            solInternacao.Item().Text($"Diárias Solicitadas: {prontuario.SolicitacaoInternacao.Diarias}");
                                            solInternacao.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                            solInternacao.Item().Text($"Tipo: {prontuario.SolicitacaoInternacao.Tipo}");
                                            solInternacao.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                            solInternacao.Item().Text($"Regime: {prontuario.SolicitacaoInternacao.Regime}");
                                            solInternacao.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                            solInternacao.Item().Text($"Caráter: {prontuario.SolicitacaoInternacao.Carater}");
                                            solInternacao.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                            solInternacao.Item().Text($"Usa OPME: {(prontuario.SolicitacaoInternacao.UsaOPME ? "Sim" : "Não")}");
                                            solInternacao.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                            solInternacao.Item().Text($"Local: {prontuario.SolicitacaoInternacao.Local}");
                                            solInternacao.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                            solInternacao.Item().Text($"Guia: {prontuario.SolicitacaoInternacao.Guia}");
                                            solInternacao.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);

                                            if (prontuario.SolicitacaoInternacao.Procedimentos != null && prontuario.SolicitacaoInternacao.Procedimentos.Any())
                                            {
                                                solInternacao.Item().Text("Procedimentos Solicitados:").SemiBold().FontSize(10);
                                                foreach (var proc in prontuario.SolicitacaoInternacao.Procedimentos)
                                                {
                                                    solInternacao.Item().PaddingLeft(80).Text($"- {proc}");
                                                }
                                                solInternacao.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten4);
                                            }
                                            else
                                            {
                                                solInternacao.Item().Text("Nenhum procedimento de internação registrado.");
                                            }
                                        }
                                        else
                                        {
                                            solInternacao.Item().Text("Nenhuma solicitação de internação registrada.");
                                        }
                                    });
                                prontuarioDetails.Item().PageBreak(); // Quebra de página entre prontuários, se houver muitos
                                });
                            }
                        }
                        else
                        {
                            column.Item().PaddingLeft(10).Text("Nenhum prontuário encontrado para este paciente.");
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ").FontSize(10);
                        x.CurrentPageNumber().FontSize(10);
                        x.Span(" de ").FontSize(10);
                        x.TotalPages().FontSize(10);
                    });
            });
        });

        var stream = new MemoryStream();
        document.GeneratePdf(stream);
        stream.Position = 0;
        return stream;
    }

    // Helper para calcular idade
    private int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }

    // Helper para obter o DisplayName do enum
    private string GetEnumDisplayName<T>(T enumValue) where T : Enum
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        var attribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));
        return attribute?.Name ?? enumValue.ToString();
    }
}

