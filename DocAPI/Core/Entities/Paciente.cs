namespace DocAPI.Core.Entities;

public class Paciente
{
    protected Paciente() {}
    public Paciente(string nome, DateOnly nascimento, string cpf, string email, string telefone) 
    {
        ID = Guid.NewGuid();
        Nome = nome;
        Nascimento = nascimento;
        CPF = cpf;
        Email = email;
        Telefone = telefone;
        
    }
 

    public Guid ID { get; private set; } 
    public string Nome { get; private set; } = string.Empty;
    public DateOnly Nascimento { get; private set; }
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
    public string CPF { get; private set; } = string.Empty;
    public string? RG { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Telefone { get; private set; } = string.Empty;
    public string? Plano { get; private set; }
    public string? Carteira { get; private set; }
    public Endereco? Endereco { get; private set; } 

//Novas props de audt criadas, 
    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }
    public string? AtualizadoPor { get; private set; }
    public bool Deletado { get; private set; }
    public DateTime? DeletadoEm { get; private set; }

  
    public ICollection<Prontuario> Prontuarios { get; private set; } = new List<Prontuario>();
    public ICollection<Agendamento> Agendamentos { get; private set; } = new List<Agendamento>();
    public ICollection<Atendimento> Atendimentos { get; private set; } = new List<Atendimento>();

    public void AplicarCriacao()
    {
        if (CriadoEm == default)
            CriadoEm = DateTime.UtcNow;
    }

    public void ComplementarCadastro(string? rg, string? plano, string? carteira, Endereco? endereco)
    {
        RG = rg;
        Plano = plano;
        Carteira = carteira;
        Endereco = endereco;
    }

    public void AplicarAtualizacao(
        string nome,
        DateOnly nascimento,
        string cpf,
        string? rg,
        string email,
        string telefone,
        string? plano,
        string? carteira,
        Endereco? endereco,
        string? atualizadoPor = null)
    {
        Nome = nome;
        Nascimento = nascimento;
        CPF = cpf;
        RG = rg;
        Email = email;
        Telefone = telefone;
        Plano = plano;
        Carteira = carteira;
        Endereco = endereco;
        AtualizadoEm = DateTime.UtcNow;
        AtualizadoPor = atualizadoPor;
    }

    public void MarcarComoExcluido()
    {
        Deletado = true;
        DeletadoEm = DateTime.UtcNow;
    }
}