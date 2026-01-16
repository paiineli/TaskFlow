namespace TaskFlow.UI.Web.Models
{
    public class BuscaChamadoViewMOD
    {
        public string? NrChamado { get; set; }
        public string? TxTitulo { get; set; }
        public Int32? CdCategoria { get; set; }
        public Int32? CdStatus { get; set; }
        public Int32? CdPrioridade { get; set; }
        public Int32? CdResponsavel { get; set; }
        public DateTime? DtAberturaInicio { get; set; }
        public DateTime? DtAberturaFim { get; set; }
        public string? NmSolicitante { get; set; }
        
        // Flag para indicar tipo de visualização
        public bool ApenasMeusChamados { get; set; }
        public bool ApenasAbertos { get; set; }
    }
}
