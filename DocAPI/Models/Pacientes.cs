namespace DocAPI.Models;

public class Pacientes
{
    private static List<Paciente> ListaPaciente = new();

    public void AdicionarNovoPaciente(Paciente paciente)
    {
        
        ListaPaciente.Add(paciente);
    }
}