using DocFront.Models.ViewModels;

public class ProntuarioState
{
    public List<ProntuarioViewModel>? Lista { get; private set; }
    public ProntuarioViewModel? Selecionado { get; private set; }

    public bool IsLoaded => Lista != null;

    public void SetLista(List<ProntuarioViewModel> lista)
        => Lista = lista;

    public void SetSelecionado(ProntuarioViewModel prontuario)
        => Selecionado = prontuario;

    public void Clear()
    {
        Lista = null;
        Selecionado = null;
    }
}
