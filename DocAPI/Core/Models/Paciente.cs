using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.ConstrainedExecution;
using System.Text.Json.Serialization;

namespace DocAPI.Core.Models;

public class Paciente
{
    // public Paciente(){} contructor antigo, que mantinha apra o funcionamento de alguma coisa que não lembro
    protected Paciente() { } // EF, me explique pq isso é necessário e pq precisa ser protected( o que isso signigica?)

    public Paciente(string nome, DateTime nascimento, string cpf) // esse contructor seria utilizado nos repositórios por exemplo?
    {
        ID = Guid.NewGuid();
        Nome = nome;
        Nascimento = Nascimento;
        CPF = cpf;
    }
    // [Key] //  assm estava antes
    // [Required(ErrorMessage = "Este campo é obrigatório")]
    // public string? ID { get; set; } = string.Empty;
    [Key]// queria entendeet melhor essa datanotation, para onde é necessária? 
    [Required(ErrorMessage = "Este campo é obrigatório")]//queria entender melhor aqui esse uso, quando é acionado?
    public Guid? ID { get; private set; } // preciso entender melhor o pq vc sugeriu que todas props fosse private set
    [Required(ErrorMessage = "O nome da paciente é obrigatório")]
    public string? Nome { get; set; }
    [Required(ErrorMessage = "O Nascimento da paciente é obrigatório")]
    public DateOnly Nascimento { get; set; }
    [NotMapped] 
    public int Idade
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var idade = today.Year - Nascimento.Year;
            if (Nascimento > today.AddYears(-idade)) idade--;
            return idade;
        }
    }
    [Required(ErrorMessage = "O CPF da paciente é obrigatório")]
    [MaxLength(11, ErrorMessage = "O máximo de caracteres é 11")]
    public string? CPF { get; set; }
    [MaxLength(11, ErrorMessage = "O máximo de caracteres é 11")]
    public string? RG { get; set; }
    [Required(ErrorMessage = "O email da paciente é obrigatório")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "O telefone da paciente é obrigatório")]
    public string? Telefone { get; set; }
    [Required(ErrorMessage = "O Plano de saúde do paciente é obrigatório")]
    public string? Plano { get; set; }// possível enum
    [Required(ErrorMessage = "A carteira da paciente é obrigatório")]
    public string? Carteira { get; set; }
    public Endereco? Endereco { get; set; } 

    // Navegações
    public ICollection<Prontuario> Prontuarios { get; private set; } = new List<Prontuario>();
    public ICollection<Agendamento> Agendamentos { get; private set; } = new List<Agendamento>();
    public ICollection<Atendimento> Atendimentos { get; private set; } = new List<Atendimento>();
    
}