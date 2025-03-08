namespace TotCum.Models
{
    public class Periodo
    {
        public Guid Id { get; set; }    
        public TipoFondo? Fondo { get; set; }
        public TipoContribuzione? TipoContribuzione { get; set; }
        public bool Si35Anni { get; set; }
        public int CodiceUnex { get; set; }
        public int CodiceGlobale { get; set; }
        public DateTime Inizio { get; set; }
        public DateTime Fine { get; set; }

        public int Anni { get; set; }
        public int Mesi { get; set; }
        public int Giorni { get; set; }
        public int Settimane { get; set; }
        public string? DescrizioneContributo { get; set; }
        public int AnniDiritto { get; set; }
        public int MesiDiritto { get; set; }
        public int GiorniDiritto { get; set; }
        public int AnniMaggioreAnz { get; set; }
        public int MesiMaggioreAnz { get; set; }
        public int GiorniMaggioreAnz { get; set; }

        public bool PeriodoSovrapposto { get; set; }
        public string? Segnalazioni { get; set; }
        public bool Neutralizzato { get; set; }

    }

    public enum TipoFondo
    {
        ND,
        GestioneSeparata,
        FPLD,
        ART,
        COM,
        CDCM,
        PALS,
        GestionePubblica,
        FondoSpeciale,
        Estero,
        FondoNonPrevisto = 99
    }

    public enum TipoContribuzione
    {
        Sconosciuto,
        Obbligatoria,
        Figurativa,
        //Integrazione
    }

}
