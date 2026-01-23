using DocFront.Models.ViewModels;

public class AgendamentoState
{
    public List<AgendamentoViewModel>? Lista { get; private set; }
    public AgendamentoViewModel? Selecionado { get; private set; }

    public bool IsLoaded => Lista != null;

    public void SetLista(List<AgendamentoViewModel> lista)
        => Lista = lista;

    public void SetSelecionado(AgendamentoViewModel item)
        => Selecionado = item;

    public void Clear()
    {
        Lista = null;
        Selecionado = null;
    }
}
