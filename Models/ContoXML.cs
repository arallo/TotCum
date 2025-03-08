using System.Xml.Serialization;

namespace TotCum.Models
{
    [XmlRoot(ElementName = "DataNascita")]
    public class DataNascita
    {
        [XmlElement(ElementName = "Giorno")]
        public string? Giorno { get; set; }
        [XmlElement(ElementName = "Mese")]
        public string? Mese { get; set; }
        [XmlElement(ElementName = "Anno")]
        public string? Anno { get; set; }
    }

    [XmlRoot(ElementName = "Indirizzo")]
    public class Indirizzo
    {
        [XmlElement(ElementName = "Via")]
        public string? Via { get; set; }
        [XmlElement(ElementName = "Comune")]
        public string? Comune { get; set; }
        [XmlElement(ElementName = "Provincia")]
        public string? Provincia { get; set; }
        [XmlElement(ElementName = "Cap")]
        public string? Cap { get; set; }
    }

    [XmlRoot(ElementName = "DatiAnagrafici")]
    public class DatiAnagrafici
    {
        [XmlElement(ElementName = "Cognome")]
        public string? Cognome { get; set; }
        [XmlElement(ElementName = "Nome")]
        public string? Nome { get; set; }
        [XmlElement(ElementName = "DataNascita")]
        public DataNascita? DataNascita { get; set; }
        [XmlElement(ElementName = "Sesso")]
        public string? Sesso { get; set; }
        [XmlElement(ElementName = "LuogoNascita")]
        public string? LuogoNascita { get; set; }
        [XmlElement(ElementName = "ProvinciaNascita")]
        public string? ProvinciaNascita { get; set; }
        [XmlElement(ElementName = "CodiceFiscale")]
        public string? CodiceFiscale { get; set; }
        [XmlElement(ElementName = "Indirizzo")]
        public Indirizzo? Indirizzo { get; set; }
    }

    [XmlRoot(ElementName = "DataEmissioneEstratto")]
    public class DataEmissioneEstratto
    {
        [XmlElement(ElementName = "Giorno")]
        public string? Giorno { get; set; }
        [XmlElement(ElementName = "Mese")]
        public string? Mese { get; set; }
        [XmlElement(ElementName = "Anno")]
        public string? Anno { get; set; }
    }

    [XmlRoot(ElementName = "Aggiornamento")]
    public class Aggiornamento
    {
        [XmlElement(ElementName = "DataEmissioneEstratto")]
        public DataEmissioneEstratto? DataEmissioneEstratto { get; set; }
        [XmlElement(ElementName = "EsitoElaborazione")]
        public string? EsitoElaborazione { get; set; }
    }

    [XmlRoot(ElementName = "Dal")]
    public class Dal
    {
        [XmlElement(ElementName = "Giorno")]
        public string? Giorno { get; set; }
        [XmlElement(ElementName = "Mese")]
        public string? Mese { get; set; }
        [XmlElement(ElementName = "Anno")]
        public string? Anno { get; set; }
    }

    [XmlRoot(ElementName = "Al")]
    public class Al
    {
        [XmlElement(ElementName = "Giorno")]
        public string? Giorno { get; set; }
        [XmlElement(ElementName = "Mese")]
        public string? Mese { get; set; }
        [XmlElement(ElementName = "Anno")]
        public string? Anno { get; set; }
    }

    [XmlRoot(ElementName = "Azienda")]
    public class Azienda
    {
        [XmlElement(ElementName = "Codice")]
        public string? Codice { get; set; }
        [XmlElement(ElementName = "Descrizione")]
        public string? Descrizione { get; set; }
    }

    [XmlRoot(ElementName = "RigaContributi")]
    public class RigaContributi
    {
        [XmlElement(ElementName = "Dal")]
        public Dal? Dal { get; set; }
        [XmlElement(ElementName = "Al")]
        public Al? Al { get; set; }
        [XmlElement(ElementName = "TipoContribuzione")]
        public string? TipoContribuzione { get; set; }
        [XmlElement(ElementName = "TipoContributo")] // Anni Mesi Giorni Settimane
        public string? TipoContributo { get; set; }
        [XmlElement(ElementName = "ContributiUtiliDiritto")]
        public string? ContributiUtiliDiritto { get; set; }
        [XmlElement(ElementName = "ContributiUtiliCalcolo")]
        public string? ContributiUtiliCalcolo { get; set; }
        [XmlElement(ElementName = "RetribuzioneLire")]
        public string? RetribuzioneLire { get; set; }
        [XmlElement(ElementName = "RetribuzioneEuro")]
        public string? RetribuzioneEuro { get; set; }
        [XmlElement(ElementName = "Azienda")]
        public Azienda? Azienda { get; set; }
        [XmlElement(ElementName = "PrimaNota")]
        public PrimaNota? PrimaNota { get; set; }
    }

    [XmlRoot(ElementName = "PrimaNota")]
    public class PrimaNota
    {
        [XmlElement(ElementName = "Codice")]
        public string? Codice { get; set; }
        [XmlElement(ElementName = "Descrizione")]
        public string? Descrizione { get; set; }
    }

    [XmlRoot(ElementName = "Contributi")]
    public class Contributi
    {
        [XmlElement(ElementName = "RigaContributi")]
        public List<RigaContributi>? RigaContributi { get; set; }
    }

    [XmlRoot(ElementName = "RegimeGenerale")]
    public class RegimeGenerale
    {
        [XmlElement(ElementName = "Contributi")]
        public Contributi? Contributi { get; set; }
        [XmlElement(ElementName = "Avvertenze")]
        public List<string?>? Avvertenze { get; set; }
        [XmlElement(ElementName = "SegnalazioniPersonalizzate")]
        public List<string?>? SegnalazioniPersonalizzate { get; set; }
    }

    [XmlRoot(ElementName = "TipoAttivitaOContribuzione")]
    public class TipoAttivitaOContribuzione
    {
        [XmlElement(ElementName = "Codice")]
        public string? Codice { get; set; }
        [XmlElement(ElementName = "Descrizione")]
        public string? Descrizione { get; set; }
    }

    [XmlRoot(ElementName = "RigaParasubordinati")]
    public class RigaParasubordinati
    {
        [XmlElement(ElementName = "AnnoSolare")]
        public string? AnnoSolare { get; set; }
        [XmlElement(ElementName = "RedditoImponibile")]
        public string? RedditoImponibile { get; set; }
        [XmlElement(ElementName = "DescrizioneCommittente")]
        public string? DescrizioneCommittente { get; set; }
        [XmlElement(ElementName = "TipoAttivitaOContribuzione")]
        public TipoAttivitaOContribuzione? TipoAttivitaOContribuzione { get; set; }
        [XmlElement(ElementName = "ContributiVersati")]
        public string? ContributiVersati { get; set; }
        [XmlElement(ElementName = "AliquotaContributiva")]
        public string? AliquotaContributiva { get; set; }
    }

    [XmlRoot(ElementName = "Parasubordinati")]
    public class Parasubordinati
    {
        [XmlElement(ElementName = "RigaParasubordinati")]
        public List<RigaParasubordinati>? RigaParasubordinati { get; set; }
    }

    [XmlRoot(ElementName = "RigaMontanteParasubordinati")]
    public class RigaMontanteParasubordinati
    {
        [XmlElement(ElementName = "Anno")]
        public string? Anno { get; set; }
        [XmlElement(ElementName = "TipoAttivitaOContribuzione")]
        public string? TipoAttivitaOContribuzione { get; set; }
        [XmlElement(ElementName = "RedditoImponibile")]
        public string? RedditoImponibile { get; set; }
        [XmlElement(ElementName = "Mesi")]
        public string? Mesi { get; set; }
        [XmlElement(ElementName = "Contributi")]
        public string? Contributi { get; set; }
        [XmlElement(ElementName = "MontanteContributivo")]
        public string? MontanteContributivo { get; set; }
    }

    [XmlRoot(ElementName = "MontanteParasubordinati")]
    public class MontanteParasubordinati
    {
        [XmlElement(ElementName = "InfoGeneraliMontanteParasubordinati")]
        public string? InfoGeneraliMontanteParasubordinati { get; set; }
        [XmlElement(ElementName = "RigaMontanteParasubordinati")]
        public List<RigaMontanteParasubordinati>? RigaMontanteParasubordinati { get; set; }
        [XmlElement(ElementName = "InfoCalcoloContributivo")]
        public string? InfoCalcoloContributivo { get; set; }
    }

    [XmlRoot(ElementName = "RegimeParasubordinati")]
    public class RegimeParasubordinati
    {
        [XmlElement(ElementName = "Parasubordinati")]
        public Parasubordinati? Parasubordinati { get; set; }
        [XmlElement(ElementName = "Avvertenze")]
        public List<string?>? Avvertenze { get; set; }
        [XmlElement(ElementName = "SegnalazioniPersonalizzate")]
        public List<string?>? SegnalazioniPersonalizzate { get; set; }
        [XmlElement(ElementName = "MontanteParasubordinati")]
        public MontanteParasubordinati? MontanteParasubordinati { get; set; }
    }

    [XmlRoot(ElementName = "TipiContribuzione")]
    public class TipiContribuzione
    {
        [XmlElement(ElementName = "Codice")]
        public string? Codice { get; set; }
        [XmlElement(ElementName = "Descrizione")]
        public string? Descrizione { get; set; }
    }

    [XmlRoot(ElementName = "DenominazioniAzienda")]
    public class DenominazioniAzienda
    {
        [XmlElement(ElementName = "Codice")]
        public string? Codice { get; set; }
        [XmlElement(ElementName = "Descrizione")]
        public string? Descrizione { get; set; }
    }

    [XmlRoot(ElementName = "InformazioniAccessorie")]
    public class InformazioniAccessorie
    {
        [XmlElement(ElementName = "TipiContribuzione")]
        public List<TipiContribuzione>? TipiContribuzione { get; set; }
        //[XmlElement(ElementName = "DenominazioniAzienda")]
        //public List<DenominazioniAzienda> DenominazioniAzienda { get; set; }
    }

    [XmlRoot(ElementName = "RigaContributiSpettacoloSport")]
    public class RigaContributiSpettacoloSport
    {
        [XmlElement(ElementName = "Dal")]
        public Dal? Dal { get; set; }
        [XmlElement(ElementName = "Al")]
        public Al? Al { get; set; }
        [XmlElement(ElementName = "TipoDiContribuzione")]
        public TipoDiContribuzione? TipoDiContribuzione { get; set; }
        [XmlElement(ElementName = "Giorni")]
        public string? Giorni { get; set; }
        [XmlElement(ElementName = "Retribuzione")]
        public string? Retribuzione { get; set; }
    }

    [XmlRoot(ElementName = "TipoDiContribuzione")]
    public class TipoDiContribuzione
    {
        [XmlElement(ElementName = "Codice")]
        public string? Codice { get; set; }
        [XmlElement(ElementName = "Descrizione")]
        public string? Descrizione { get; set; }
    }

    [XmlRoot(ElementName = "ContributiSpettacoloSport")]
    public class ContributiSpettacoloSport
    {
        [XmlElement(ElementName = "RigaContributiSpettacoloSport")]
        public List<RigaContributiSpettacoloSport>? RigaContributiSpettacoloSport { get; set; }
    }

    [XmlRoot(ElementName = "FondoSpettacoloSport")]
    public class FondoSpettacoloSport
    {
        [XmlElement(ElementName = "ContributiSpettacoloSport")]
        public ContributiSpettacoloSport? ContributiSpettacoloSport { get; set; }
        [XmlElement(ElementName = "Avvertenze")]
        public List<string?>? Avvertenze { get; set; }
        [XmlElement(ElementName = "SegnalazioniPersonalizzate")]
        public string? SegnalazioniPersonalizzate { get; set; }
    }

    [XmlRoot(ElementName = "AmministrazioneOEnte")]
    public class AmministrazioneOEnte
    {
        [XmlElement(ElementName = "Codice")]
        public string? Codice { get; set; }
        [XmlElement(ElementName = "Descrizione")]
        public string? Descrizione { get; set; }
    }

    [XmlRoot(ElementName = "RigaContributiGestionePubblica")]
    public class RigaContributiGestionePubblica
    {
        [XmlElement(ElementName = "Dal")]
        public string? Dal { get; set; }
        [XmlElement(ElementName = "Al")]
        public string? Al { get; set; }
        [XmlElement(ElementName = "GestioneOFondo")]
        public string? GestioneOFondo { get; set; }
        [XmlElement(ElementName = "TipoDiContribuzione")]
        public string? TipoDiContribuzione { get; set; }
        [XmlElement(ElementName = "AnniContribUtiliDiritto")]
        public string? AnniContribUtiliDiritto { get; set; }
        [XmlElement(ElementName = "MesiContribUtiliDiritto")]
        public string? MesiContribUtiliDiritto { get; set; }
        [XmlElement(ElementName = "GiorniContribUtiliDiritto")]
        public string? GiorniContribUtiliDiritto { get; set; }
        [XmlElement(ElementName = "AnniContribUtiliMisura")]
        public string? AnniContribUtiliMisura { get; set; }
        [XmlElement(ElementName = "MesiContribUtiliMisura")]
        public string? MesiContribUtiliMisura { get; set; }
        [XmlElement(ElementName = "GiorniContribUtiliMisura")]
        public string? GiorniContribUtiliMisura { get; set; }
        [XmlElement(ElementName = "RetribuzioneEuro")]
        public string? RetribuzioneEuro { get; set; }
        [XmlElement(ElementName = "AmministrazioneOEnte")]
        public AmministrazioneOEnte? AmministrazioneOEnte { get; set; }
        [XmlElement(ElementName = "Nota")]
        public List<Nota>? Nota { get; set; }
    }

    [XmlRoot(ElementName = "ContributiGestionePubblica")]
    public class ContributiGestionePubblica
    {
        [XmlElement(ElementName = "RigaContributiGestionePubblica")]
        public List<RigaContributiGestionePubblica>? RigaContributiGestionePubblica { get; set; }
    }

    [XmlRoot(ElementName = "AggiornamentoGP")]
    public class AggiornamentoGP
    {
        [XmlElement(ElementName = "DataEmissioneEstrattoGP")]
        public string? DataEmissioneEstrattoGP { get; set; }
    }

    [XmlRoot(ElementName = "Nota")]
    public class Nota
    {
        [XmlElement(ElementName = "Codice")]
        public string? Codice { get; set; }
        [XmlElement(ElementName = "Descrizione")]
        public string? Descrizione { get; set; }
    }

    [XmlRoot(ElementName = "GestionePubblica")]
    public class GestionePubblica
    {
        [XmlElement(ElementName = "ContributiGestionePubblica")]
        public ContributiGestionePubblica? ContributiGestionePubblica { get; set; }
        [XmlElement(ElementName = "AggiornamentoGP")]
        public AggiornamentoGP? AggiornamentoGP { get; set; }
        [XmlElement(ElementName = "Avvertenze")]
        public List<string?>? Avvertenze { get; set; }
        [XmlElement(ElementName = "SegnalazioniPersonalizzate")]
        public string? SegnalazioniPersonalizzate { get; set; }
    }


    [XmlRoot(ElementName = "EstrattoConto")]
    public class EstrattoConto
    {
        [XmlElement(ElementName = "DatiAnagrafici")]
        public DatiAnagrafici? DatiAnagrafici { get; set; }
        [XmlElement(ElementName = "Aggiornamento")]
        public Aggiornamento? Aggiornamento { get; set; }
        [XmlElement(ElementName = "RegimeGenerale")]
        public RegimeGenerale? RegimeGenerale { get; set; }
        [XmlElement(ElementName = "RegimeParasubordinati")]
        public RegimeParasubordinati? RegimeParasubordinati { get; set; }
        [XmlElement(ElementName = "FondoSpettacoloSport")]
        public FondoSpettacoloSport? FondoSpettacoloSport { get; set; }
        [XmlElement(ElementName = "GestionePubblica")]
        public GestionePubblica? GestionePubblica { get; set; }

        [XmlElement(ElementName = "InformazioniAccessorie")]
        public InformazioniAccessorie? InformazioniAccessorie { get; set; }
    }
}
