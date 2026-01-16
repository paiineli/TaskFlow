using TaskFlow.Model;
using X.PagedList;

namespace TaskFlow.UI.Web.Models
{
    public class ChamadoViewMOD
    {
        public ChamadoMOD Chamado { get; set; }
        public IPagedList<ChamadoMOD> ListaChamadosPaginada { get; set; }
        public Int32 QtdTotalDeRegistros { get; set; }
        
        // Listas para dropdowns
        public List<CategoriaMOD> Categorias { get; set; }
        public List<StatusMOD> Status { get; set; }
        public List<FuncionarioMOD> Atendentes { get; set; }
        public List<JustificativaMOD> Justificativas { get; set; }
        
        // Dados relacionados ao chamado
        public List<ChamadoComentarioMOD> Comentarios { get; set; }
        public List<ChamadoAnexoMOD> Anexos { get; set; }
        public List<ChamadoHistoricoMOD> Historico { get; set; }
        public ChamadoAvaliacaoMOD Avaliacao { get; set; }
    }
}
