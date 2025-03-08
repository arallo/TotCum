using TotCum.Data;
using TotCum.Models;
using System.Globalization;
using System.Text.RegularExpressions;

/* Per la determinazione del sistema di calcolo del trattamento in cumulo e dei relativi istituti applicabili, 
 * si deve considerare l’anzianità contributiva complessivamente maturata nelle gestioni assicurative interessate dal cumulo al 31 dicembre 1995 
 * (ad esclusione dei periodi accreditati presso le Casse professionali, 
 * dei periodi riscattati presso la Gestione Separata riferiti a periodi antecedenti al 1996, 
 * della contribuzione derivante da riscatto del corso di studi universitario richiesto dai soggetti “inoccupati”). */

namespace TotCum.Services
{
    public class GeneraContoGrezzo
    {
        public readonly List<Periodo> ContoGrezzo = [];
        public readonly List<RigaContributi> Scartati = [];
        public GeneraContoGrezzo(EstrattoConto estrattoConto) 
        {
            var datianagrafici = estrattoConto.DatiAnagrafici;
            var regimegenerale = estrattoConto.RegimeGenerale;
            var regimeparasubordinati = estrattoConto.RegimeParasubordinati;
            var fondospettacolosport = estrattoConto.FondoSpettacoloSport;
            var gestionepubblica = estrattoConto.GestionePubblica;

            if (regimeparasubordinati != null)
            {
                string iscrizionegestioneseparata = regimeparasubordinati.SegnalazioniPersonalizzate!.SingleOrDefault(x => x!.Contains("Gestione Separata dal"))!;
                Regex regexDataIscrizione = new(@"(\d{2}\/\d{2}\/\d{4})");
                DateTime dataiscrizionegs = new();
                var annoprimocontributoversato = regimeparasubordinati.Parasubordinati?.RigaParasubordinati?.FirstOrDefault();
                int primoannoctr = int.Parse(annoprimocontributoversato?.AnnoSolare!);

                if (iscrizionegestioneseparata is not null)
                {
                    var iscrizionematch = regexDataIscrizione.Match(iscrizionegestioneseparata);
                    if (iscrizionematch.Success)
                    {
                        dataiscrizionegs = DateTime.Parse(iscrizionematch.Groups[1].Value).Date;
                    }
                }
                else
                {
                    dataiscrizionegs = new DateTime(primoannoctr, 1, 1);
                }

                foreach (var item in regimeparasubordinati.Parasubordinati?.RigaParasubordinati!)
                {
                    int mesi = 0;
                    var aa_test = Convert.ToInt32(item.AnnoSolare);
                    if (Convert.ToInt32(item.AnnoSolare) > 1995 && item.TipoAttivitaOContribuzione!.Descrizione != "Riscatti")
                    {
                        var minimale = Minimali.ListaMinimali.Single(x => x.Anno == Convert.ToInt32(item.AnnoSolare)).Valore;
                        var aliquota = float.Parse(item.AliquotaContributiva!, CultureInfo.InvariantCulture.NumberFormat);
                        if (item.ContributiVersati is not null)
                        {
                            double contributi_versati = float.Parse(item.ContributiVersati!, CultureInfo.InvariantCulture.NumberFormat);
                            double contributi_sul_minimale = minimale * (aliquota / 100);
                            // 12 : contributiminimale = x : ctrversati  -> x = (12*ctrversati)/contributiminimale
                            var mesi_da_arrotondare = (contributi_versati * 12) / contributi_sul_minimale;
                            var decimali = mesi_da_arrotondare - Math.Truncate(mesi_da_arrotondare);
                            mesi = (int)((decimali >= 0.999) ? Math.Ceiling(mesi_da_arrotondare) : Math.Floor(mesi_da_arrotondare));
                            if (dataiscrizionegs.Year == Convert.ToInt32(item.AnnoSolare))
                            {
                                int mm = 13 - dataiscrizionegs.Month;
                                if (mesi > mm)
                                {
                                    mesi = mm;
                                }
                            }
                            if (mesi > 12)
                            {
                                mesi = 12;
                            }
                            if (mesi == 0)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            mesi = 0;
                        }
                    }
                    else
                    {
                        mesi = regimeparasubordinati.MontanteParasubordinati!.RigaMontanteParasubordinati!
                            .Where(x => Convert.ToInt32(x.Anno) == Convert.ToInt32(item.AnnoSolare) && x.TipoAttivitaOContribuzione == "Riscatti")
                            .Sum(x => Convert.ToInt32(x.Mesi));
                    }

                    TipoContribuzione tipoContributo = new();

                    if (item.TipoAttivitaOContribuzione!.Codice == "C" | item.TipoAttivitaOContribuzione!.Codice == "P"| item.TipoAttivitaOContribuzione!.Codice == "a")
                    {
                        tipoContributo = TipoContribuzione.Obbligatoria;
                    }
                    else
                    {
                        tipoContributo = TipoContribuzione.Figurativa; // generalmente codice Y
                    }
                    bool isDisColl = item.TipoAttivitaOContribuzione.Descrizione!.Contains("dis-coll", StringComparison.InvariantCultureIgnoreCase);

                    DateTime datainiziale = GetDataInizioPeriodo(Convert.ToInt32(item.AnnoSolare), dataiscrizionegs, item.TipoAttivitaOContribuzione?.Codice!, primoannoctr);

                    ContoGrezzo.Add(new Periodo
                    {
                        Id = Guid.NewGuid(),
                        Fondo = TipoFondo.GestioneSeparata,
                        TipoContribuzione = tipoContributo,
                        CodiceGlobale = 900,
                        Inizio = datainiziale,
                        Fine = new DateTime(datainiziale.Year, datainiziale.AddMonths(mesi-1).Month, DateTime.DaysInMonth(datainiziale.Year, datainiziale.AddMonths(mesi-1).Month)),
                        Mesi = mesi,
                        DescrizioneContributo = item.TipoAttivitaOContribuzione?.Descrizione ?? "descrizione assente",
                        Si35Anni = !isDisColl,
                        Segnalazioni = ""
                    });

                }
            }

            if (regimegenerale != null)
            {
                var listaRigaContributi = estrattoConto?.RegimeGenerale?.Contributi?.RigaContributi;
                if (listaRigaContributi != null)
                {
                    RigaContributi? rigaContributiCorrente = listaRigaContributi.First();
                    Periodo periodoCorrente = new();

                    foreach (var rigaContributi in listaRigaContributi)
                    {
                        if ((rigaContributi.Dal is null | rigaContributi.Al is null) && rigaContributi.TipoContribuzione is not null)
                        {
                            Scartati.Add(rigaContributi);
                            continue;
                        }
                        if ((rigaContributi.Dal is null | rigaContributi.Al is null) && rigaContributi.TipoContribuzione is null)
                        {
                            if (rigaContributi.Azienda!.Codice == rigaContributiCorrente.Azienda!.Codice)
                            {
                                if (rigaContributi.TipoContributo == "Settimane")
                                {
                                    periodoCorrente.Settimane = Convert.ToInt32(rigaContributi.ContributiUtiliDiritto);
                                }
                                if (rigaContributi.TipoContributo == "Anni")
                                {
                                    periodoCorrente.Anni = Convert.ToInt32(rigaContributi.ContributiUtiliDiritto);
                                }
                                if (rigaContributi.TipoContributo == "Mesi")
                                {
                                    periodoCorrente.Mesi = Convert.ToInt32(rigaContributi.ContributiUtiliDiritto);
                                }
                                if (rigaContributi.TipoContributo == "Giorni")
                                {
                                    periodoCorrente.Giorni = Convert.ToInt32(rigaContributi.ContributiUtiliDiritto);
                                }
                            }
                            else
                            {
                                Scartati.Add(rigaContributi);
                            }

                            continue;
                        }
                        else
                        {
                            if (periodoCorrente.Inizio > DateTime.MinValue)
                            {
                                ContoGrezzo.Add(periodoCorrente);
                            }
                            
                        }

                        Periodo periodo = new()
                        {
                            Id = Guid.NewGuid(),
                            Inizio = new DateTime(Convert.ToInt32(rigaContributi.Dal?.Anno), Convert.ToInt32(rigaContributi.Dal?.Mese), Convert.ToInt32(rigaContributi.Dal?.Giorno)),
                            Fine = new DateTime(Convert.ToInt32(rigaContributi.Al?.Anno), Convert.ToInt32(rigaContributi.Al?.Mese), Convert.ToInt32(rigaContributi.Al?.Giorno)),
                            DescrizioneContributo = rigaContributi.TipoContribuzione! ?? "descrizione assente"
                        };

                        if (rigaContributi.TipoContribuzione != null)
                        {
                            if (rigaContributi.TipoContribuzione.ToLower().Contains("fondo") | rigaContributi.TipoContribuzione.ToLower().Contains("obbligatoria") | rigaContributi.TipoContribuzione.ToLower().Contains("p.t.") | rigaContributi.TipoContribuzione.ToLower().Contains("f.s.") | rigaContributi.TipoContribuzione.ToLower().Contains("d.i.") | rigaContributi.TipoContribuzione.ToLower().Contains("v.l."))
                            {
                                periodo.Fondo = TipoFondo.FondoSpeciale;
                                periodo.TipoContribuzione = TipoContribuzione.Obbligatoria;
                                periodo.Si35Anni = true;
                                periodo.CodiceGlobale = 500;
                            }
                            else if (rigaContributi.TipoContribuzione.ToLower().Contains("dipend") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("apprendista") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("familiare") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("agricolo") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("time") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("ind. sost. preavviso") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("cassa") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("riscatto") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("risc. periodi scoperti") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("ricongiunzione") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("volontaria") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("ass.ord. fis") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("telef") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("p.a.l.s. obblig. cong.") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("contratto a tempo") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("computo dpr 1092/73") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("lavoro in albania"))
                            {
                                if (Regex.IsMatch(rigaContributi.TipoContribuzione, @"\(int\)", RegexOptions.IgnoreCase) ||
                                    Regex.IsMatch(rigaContributi.TipoContribuzione, @"ad int", RegexOptions.IgnoreCase) ||
                                    Regex.IsMatch(rigaContributi.TipoContribuzione, @"ad integ", RegexOptions.IgnoreCase))
                                {
                                    periodo.Segnalazioni = "periodo neutralizzato";
                                    periodo.Neutralizzato = true;
                                }
                                else
                                {
                                    periodo.Fondo = TipoFondo.FPLD;
                                    periodo.TipoContribuzione = TipoContribuzione.Obbligatoria;
                                    periodo.Si35Anni = true;
                                    periodo.CodiceGlobale = 100;
                                }
                            }
                            else if(rigaContributi.TipoContribuzione.ToLower().Contains("disoccupazione") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("aspi") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("naspi") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("trattamento speciale") && !rigaContributi.TipoContribuzione.ToLower().Contains("estero"))
                            {
                                periodo.Fondo = TipoFondo.FPLD;
                                periodo.TipoContribuzione = TipoContribuzione.Figurativa;
                                periodo.Si35Anni = false;
                                periodo.CodiceGlobale = 100;
                            }
                            else if (rigaContributi.TipoContribuzione.ToLower().Contains("mobilita") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("malattia") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("servizio militare") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("maternita") | 
                                rigaContributi.TipoContribuzione.ToLower().Contains("ast.obbl.extra rapp.lav.") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("mater/pater/congedi") |
                                rigaContributi.TipoContribuzione.ToLower().Contains("donaz.sangue")
                                )
                            {
                                periodo.Fondo = TipoFondo.FPLD;
                                if (Regex.IsMatch(rigaContributi.TipoContribuzione, @"\(int\)", RegexOptions.IgnoreCase) ||
                                    Regex.IsMatch(rigaContributi.TipoContribuzione, @"ad int", RegexOptions.IgnoreCase) ||
                                    Regex.IsMatch(rigaContributi.TipoContribuzione, @"ad integ", RegexOptions.IgnoreCase))
                                {
                                    //periodo.TipoContribuzione = TipoContribuzione.Integrazione;
                                    periodo.Segnalazioni = "periodo neutralizzato";
                                    periodo.Neutralizzato = true;
                                }
                                else
                                {
                                    periodo.TipoContribuzione = TipoContribuzione.Figurativa;
                                    periodo.Si35Anni = true;
                                    periodo.CodiceGlobale = 100;
                                }
                            }
                            else if (rigaContributi.TipoContribuzione.ToLower().Contains("art."))
                            {
                                periodo.Fondo = TipoFondo.ART;
                                periodo.TipoContribuzione = TipoContribuzione.Obbligatoria;
                                periodo.Si35Anni = true;
                                periodo.CodiceGlobale = 200;
                            }
                            else if (rigaContributi.TipoContribuzione.ToLower().Contains("com."))
                            {
                                periodo.Fondo = TipoFondo.COM;
                                periodo.TipoContribuzione = TipoContribuzione.Obbligatoria;
                                periodo.Si35Anni = true;
                                periodo.CodiceGlobale = 300;
                            }
                            else if (rigaContributi.TipoContribuzione.ToLower().Contains("coltiv.diretto"))
                            {
                                periodo.Fondo = TipoFondo.CDCM;
                                periodo.TipoContribuzione = TipoContribuzione.Obbligatoria;
                                periodo.Si35Anni = true;
                                periodo.CodiceGlobale = 400;
                            }
                            else if (rigaContributi.TipoContribuzione.ToLower().Contains("estero:lavoro"))
                            {
                                periodo.Fondo = TipoFondo.Estero;
                                periodo.TipoContribuzione = TipoContribuzione.Obbligatoria;
                                periodo.Si35Anni = true;
                                periodo.CodiceGlobale = 700;
                            }
                            else if (rigaContributi.TipoContribuzione.ToLower().Contains("estero:vers.volontari"))
                            {
                                periodo.Fondo = TipoFondo.Estero;
                                periodo.TipoContribuzione = TipoContribuzione.Obbligatoria;
                                periodo.Si35Anni = true;
                                periodo.CodiceGlobale = 700;
                            }
                            else if (rigaContributi.TipoContribuzione.ToLower().Contains("bonif.enfant/mere famil."))
                            {
                                periodo.Fondo = TipoFondo.Estero;
                                periodo.TipoContribuzione = TipoContribuzione.Figurativa;
                                periodo.Si35Anni = false;
                                periodo.CodiceGlobale = 700;
                            }
                            else if (rigaContributi.TipoContribuzione.ToLower().Contains("estero:disoccupazione"))
                            {
                                periodo.Fondo = TipoFondo.Estero;
                                periodo.TipoContribuzione = TipoContribuzione.Figurativa;
                                periodo.Si35Anni = false;
                                periodo.CodiceGlobale = 700;
                            }
                            else if (rigaContributi.TipoContribuzione.ToLower().Contains("estero:malattia"))
                            {
                                periodo.Fondo = TipoFondo.Estero;
                                periodo.TipoContribuzione = TipoContribuzione.Figurativa;
                                periodo.Si35Anni = false;
                                periodo.CodiceGlobale = 700;
                            }
                            else if (rigaContributi.TipoContribuzione.ToLower().Contains("estero:fig.per magg.anz."))
                            {
                                periodo.Fondo = TipoFondo.Estero;
                                periodo.TipoContribuzione = TipoContribuzione.Figurativa;
                                periodo.Si35Anni = false;
                                periodo.CodiceGlobale = 700;
                            }
                            else if (rigaContributi.TipoContribuzione.ToLower().Contains("estero:educazione figli"))
                            {
                                periodo.Fondo = TipoFondo.Estero;
                                periodo.TipoContribuzione = TipoContribuzione.Figurativa;
                                periodo.Si35Anni = false;
                                periodo.CodiceGlobale = 700;
                            }
                            else if (rigaContributi.TipoContribuzione.ToLower().Contains("estero:figurat"))
                            {
                                periodo.Fondo = TipoFondo.Estero;
                                periodo.TipoContribuzione = TipoContribuzione.Figurativa;
                                periodo.Si35Anni = false;
                                periodo.CodiceGlobale = 700;
                            }
                            else
                            {
                                periodo.Fondo = TipoFondo.FondoNonPrevisto;
                                periodo.TipoContribuzione = TipoContribuzione.Sconosciuto;
                                periodo.CodiceGlobale = 0;
                            }
                        }

                        if (rigaContributi.ContributiUtiliDiritto == "0")
                        {
                            periodo.Segnalazioni = "periodo neutralizzato";
                            periodo.Neutralizzato = true;
                            
                        }
                        else
                        {
                            if (rigaContributi.TipoContributo == "Settimane")
                            {
                                periodo.Settimane = Convert.ToInt32(rigaContributi.ContributiUtiliDiritto);
                            }
                            if (rigaContributi.TipoContributo == "Anni")
                            {
                                periodo.Anni = Convert.ToInt32(rigaContributi.ContributiUtiliDiritto);
                            }
                            if (rigaContributi.TipoContributo == "Mesi")
                            {
                                periodo.Mesi = Convert.ToInt32(rigaContributi.ContributiUtiliDiritto);
                            }
                            if (rigaContributi.TipoContributo == "Giorni")
                            {
                                periodo.Giorni = Convert.ToInt32(rigaContributi.ContributiUtiliDiritto);
                            }
                        }

                        periodoCorrente = periodo;
                        rigaContributiCorrente = rigaContributi;
                    }
                }
            }

            if (fondospettacolosport != null)
            {
                var records = estrattoConto?.FondoSpettacoloSport?.ContributiSpettacoloSport?.RigaContributiSpettacoloSport;
                if (records != null)
                {
                    foreach (var record in records)
                    {
                        Periodo periodo = new()
                        {
                            Id = Guid.NewGuid(),
                            Fondo = TipoFondo.PALS,
                            Inizio = new DateTime(int.Parse(record.Dal?.Anno!), int.Parse(record.Dal?.Mese!), int.Parse(record.Dal?.Giorno!)),
                            Fine = new DateTime(int.Parse(record.Al?.Anno!), int.Parse(record.Al?.Mese!), int.Parse(record.Al?.Giorno!)),
                            Giorni = int.TryParse(record.Giorni, out int g) == false ? 0 : int.Parse(record.Giorni),
                            DescrizioneContributo = record.TipoDiContribuzione!.Descrizione! ?? "descrizione assente",
                            Si35Anni = true,
                            CodiceGlobale = 100
                        };
                        ContoGrezzo.Add(periodo);
                    }
                }
            }

            if (gestionepubblica != null)
            {
                var records = estrattoConto?.GestionePubblica?.ContributiGestionePubblica?.RigaContributiGestionePubblica;
                if (records != null)
                {
                    foreach (var record in records)
                    {
                        int aa = int.Parse(record.AnniContribUtiliDiritto!);
                        int mm = int.Parse(record.MesiContribUtiliDiritto!);
                        int gg = int.Parse(record.GiorniContribUtiliDiritto!);
                        if (aa == 0 && mm == 0 && gg == 0)
                        {
                            continue;
                        }
                        Periodo periodo = new()
                        {
                            Id = Guid.NewGuid(),
                            Fondo = TipoFondo.GestionePubblica,
                            Inizio = DateTime.Parse(record.Dal!),
                            Fine = DateTime.Parse(record.Al!),
                            Anni = aa,
                            Mesi = mm,
                            Giorni = gg,
                            DescrizioneContributo = record.TipoDiContribuzione ?? "descrizione assente",
                            Si35Anni = true,
                            CodiceGlobale = 600
                        };
                        ContoGrezzo.Add(periodo);
                    }

                }
            }
            ContoGrezzo = ContoGrezzo.OrderBy(x => x.Inizio.Date).ToList();

        }

        private DateTime GetDataInizioPeriodo(int anno, DateTime dataiscrizionegs, string tipoCodiceContribuzione, int annoPrimoContributo)
        {
            if (anno == dataiscrizionegs.Year && tipoCodiceContribuzione != "a" && annoPrimoContributo == dataiscrizionegs.Year)
            {
                return dataiscrizionegs;
            }
            else
            {
                return new DateTime(anno, 1, 1);
            }
        }

    }
}
