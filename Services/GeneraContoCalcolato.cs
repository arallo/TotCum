using TotCum.Models;
using TotCum.Services;
using System.Diagnostics;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TotCum.Services
{
    public interface IGeneraContoCalcolato
    {
        List<Periodo> ElaboraDati(List<Periodo> contobase);
    }

    public class GeneraContoCalcolato : IGeneraContoCalcolato
    {
        public List<Periodo> ElaboraDati(List<Periodo> contobase)
        {
            var contoCumulo = new List<Periodo>();   

            // test per presenza agricoli e autonomi
            
            var presenzaAgricoli = contobase.Where(a => (bool)(a.DescrizioneContributo!.Contains("agricol", StringComparison.InvariantCultureIgnoreCase))).Any();
            var presenzaAutonomi = contobase
                .Where(a =>
                a.DescrizioneContributo!.Contains("artigiano", StringComparison.InvariantCultureIgnoreCase) |
                a.DescrizioneContributo.Contains("commerciante", StringComparison.InvariantCultureIgnoreCase) |
                a.DescrizioneContributo.Contains("coltivatore", StringComparison.InvariantCultureIgnoreCase))
                .Any();

            List<Periodo> contoBaseFiltrato = new();
            foreach (var item in contobase)
            {
                if (item.Neutralizzato)
                {
                    continue;
                }
                var periodo = new Periodo
                {
                    Id = item.Id,
                    Fondo = item.Fondo,
                    Inizio = item.Inizio,
                    Fine = item.Fine,
                    DescrizioneContributo = item.DescrizioneContributo,
                    TipoContribuzione = item.TipoContribuzione,
                    CodiceGlobale = item.CodiceGlobale,
                    Anni = item.Anni,
                    Mesi = item.Mesi,
                    Giorni = item.Giorni,
                    Settimane = item.Settimane,
                    Segnalazioni = item.Segnalazioni,
                    Si35Anni = item.Si35Anni,
                    PeriodoSovrapposto = false,
                    Neutralizzato = item.Neutralizzato
                };
                contoBaseFiltrato.Add(periodo);
            }

            if (presenzaAutonomi)
            {
                foreach (var p in contoBaseFiltrato)
                {
                    if (p.DescrizioneContributo!.Contains("art.") | p.DescrizioneContributo!.Contains("com."))
                    {
                        int capienza = p.Fine.Month - p.Inizio.Month + 1;
                        if (p.Mesi < capienza)
                        {
                            p.Fine = new DateTime(p.Inizio.Year, p.Mesi, DateTime.DaysInMonth(p.Inizio.Year, p.Mesi));
                        }
                    }
                }
            }

            var contoPreCumulo = RielaboraEstratto(contoBaseFiltrato);
            contoCumulo = ConversionePeriodi(contoPreCumulo.OrderBy(p => p.Inizio.Year).ToList());  

            return contoCumulo;
        }


        private List<Periodo> RielaboraEstratto(List<Periodo> periodiDaRielaborare)
        {
            // Raggruppa i periodi per anno in un dizionario
            var periodiRaggruppatiPerAnno = periodiDaRielaborare.OrderBy(p => p.Inizio.Year)
                                                                .GroupBy(p => p.Inizio.Year)
                                                                .ToDictionary(g => g.Key, g => g.ToList());

            var rielaborati = new List<Periodo>();

            foreach (var item in periodiRaggruppatiPerAnno) 
            { 
                var periodiStessoAnno = item.Value;  // Value contiene i periodi (Key contiene l'anno)

                //DateTime primaData = periodiStessoAnno.First().Inizio;
                //DateTime ultimaData = periodiStessoAnno.Last().Fine;

                Periodo? periodoCorrente = null;

                foreach (var periodo in periodiStessoAnno)
                {
                    if (periodoCorrente == null)
                    {
                        periodoCorrente = periodo;
                    }
                    else if(periodo.Inizio <= periodoCorrente.Fine && periodo.CodiceGlobale == periodoCorrente.CodiceGlobale)
                    {
                        periodoCorrente.Fine = periodo.Fine > periodoCorrente.Fine ? periodo.Fine : periodoCorrente.Fine;
                        periodoCorrente.Settimane += periodo.Settimane;
                        if (periodoCorrente.Settimane > 52)
                        {
                            periodoCorrente.Settimane = 52;
                        }
                        periodo.Neutralizzato = true;
                        periodoCorrente.Neutralizzato = true;
                    }
                    else if (periodo.Inizio <= periodoCorrente.Fine && periodo.CodiceGlobale != periodoCorrente.CodiceGlobale) 
                    {
                        periodoCorrente.Segnalazioni = "Periodo sovrapposto da verificare";
                        rielaborati.Add(periodoCorrente);
                        periodoCorrente = periodo;
                    }
                    else
                    {
                        rielaborati.Add(periodoCorrente);
                        periodoCorrente = periodo;
                    }
                }

                if (periodoCorrente != null)
                {
                    rielaborati.Add(periodoCorrente);
                }
            }
            return rielaborati;
        }

        private List<Periodo> ConversionePeriodi(List<Periodo> conto)
        {
            List<Periodo> nuovoconto = new();

            foreach (Periodo rec in conto)
            {
                if (rec.Settimane > 0 && rec.Settimane < 53)
                {
                    // verifico se il rec.periodo è completo (è completo se il numero di settimane accreditate è >= al numero di sabati del rec.periodo)
                    var sabati = UtilitaDate.Sabati(rec.Inizio, rec.Fine);
                    if (rec.Settimane >= sabati)
                    {
                        // se il rec.periodo è completo riconosco la capienza massima
                        var (Anni, Mesi, Giorni) = CapienzaMassimaInCumulo(rec);
                        if (rec.Si35Anni)
                        {
                            rec.AnniDiritto = Anni;
                            rec.MesiDiritto = Mesi;
                            rec.GiorniDiritto = Giorni;
                            rec.AnniMaggioreAnz = Anni;
                            rec.MesiMaggioreAnz = Mesi;
                            rec.GiorniMaggioreAnz = Giorni;
                        }
                        else
                        {
                            rec.AnniMaggioreAnz = Anni;
                            rec.MesiMaggioreAnz = Mesi;
                            rec.GiorniMaggioreAnz = Giorni;
                        }
                    }
                    else
                    {
                        // se il rec.periodo non è completo riconosco il rec.periodo accreditato in estratto rapportato all'unità di misura della tabella di conversione
                        int gg = rec.Settimane * 6;
                        int mm = (int)Double.Floor(gg / 26);
                        int ggBase26 = mm * 26;
                        int giorni = gg - ggBase26;
                        if (giorni == 0 && mm == 12)
                        {
                            if (rec.Si35Anni)
                            {
                                rec.AnniDiritto = 1;
                                rec.MesiDiritto = 0;
                                rec.GiorniDiritto = 0;
                                rec.AnniMaggioreAnz = 1;
                                rec.MesiMaggioreAnz = 0;
                                rec.GiorniMaggioreAnz = 0;
                            }
                            else
                            {
                                rec.AnniMaggioreAnz = 1;
                                rec.MesiMaggioreAnz = 0;
                                rec.GiorniMaggioreAnz = 0;
                            }
                        }
                        else
                        {
                            if (rec.Si35Anni)
                            {
                                rec.AnniDiritto = 0;
                                rec.MesiDiritto = mm;
                                rec.GiorniDiritto = giorni;
                                rec.AnniMaggioreAnz = 0;
                                rec.MesiMaggioreAnz = mm;
                                rec.GiorniMaggioreAnz = giorni;
                            }
                            else
                            {
                                rec.AnniMaggioreAnz = 0;
                                rec.MesiMaggioreAnz = mm;
                                rec.GiorniMaggioreAnz = giorni;
                            }
                        }
                    }
                }
                else // periodo espresso in gg mm aa
                {
                    if (rec.Anni > 0 | rec.Mesi > 0 | rec.Giorni > 0)
                    {
                        var (Anni, Mesi, Giorni) = Capienza(rec);
                        if (rec.Anni >= Anni && rec.Mesi >= Mesi && rec.Giorni >= Giorni)
                        {
                            // se il periodo è completo riconosco la capienza massima
                            var (AnniMax, MesiMax, GiorniMax) = CapienzaMassimaInCumulo(rec);
                            if (rec.Si35Anni)
                            {
                                rec.AnniDiritto = AnniMax;
                                rec.MesiDiritto = MesiMax;
                                rec.GiorniDiritto = GiorniMax;
                                rec.AnniMaggioreAnz = AnniMax;
                                rec.MesiMaggioreAnz = MesiMax;
                                rec.GiorniMaggioreAnz = GiorniMax;
                            }
                            else
                            {
                                rec.AnniMaggioreAnz = AnniMax;
                                rec.MesiMaggioreAnz = MesiMax;
                                rec.GiorniMaggioreAnz = GiorniMax;
                            }
                        }
                        else
                        {
                            if (rec.Si35Anni)
                            {
                                rec.AnniDiritto = Anni;
                                rec.MesiDiritto = Mesi;
                                rec.GiorniDiritto = Giorni;
                                rec.AnniMaggioreAnz = Anni;
                                rec.MesiMaggioreAnz = Mesi;
                                rec.GiorniMaggioreAnz = Giorni;
                            }
                            else
                            {
                                rec.AnniMaggioreAnz = Anni;
                                rec.MesiMaggioreAnz = Mesi;
                                rec.GiorniMaggioreAnz = Giorni;
                            }
                        }
                    }
                }
                nuovoconto.Add(rec);
            }

            return nuovoconto;
        }


        //private static bool VerificaContinuita(List<Periodo> periodi)
        //{
        //    periodi.Sort((x, y) => x.Inizio.CompareTo(y.Inizio));

        //    for (int i = 1; i < periodi.Count; i++)
        //    {
        //        if (periodi[i].Inizio > periodi[i - 1].Fine.AddDays(1))
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}


        private (int Anni, int Mesi, int Giorni) CapienzaMassimaInCumulo(Periodo periodo)
        {
            // Date di inizio e fine
            DateTime dataInizio = periodo.Inizio;// 12/01/1987
            DateTime dataFine = periodo.Fine;    // 20/05/1987

            int anni = dataFine.Year - dataInizio.Year;  // 0
            int mesi = dataFine.Month - dataInizio.Month;// 4 in questo modo si esclude il mese di partenza
            int giorni = dataFine.Day - dataInizio.Day;

            // Aggiusta i valori se necessario
            if (giorni < 0)
            {
                mesi--; // = 0
                giorni += DateTime.DaysInMonth(dataFine.Year, dataFine.Month - 1); // = -29 + 30 = 1
            }

            if (mesi < 0)
            {
                anni--;
                mesi += 12;
            }

            int ggMeseFinale = DateTime.DaysInMonth(dataFine.Year, dataFine.Month); // 31

            DateTime dataraggiunta;

            dataraggiunta = dataInizio.AddYears(anni).AddMonths(mesi);
            int rimanenzagg = (dataFine - dataraggiunta).Days + 1;
            //riproporziono su base 26
            var rimanenzaGGbase26 = (int)Double.Floor(rimanenzagg * 26 / 30);
            //calcolo correttivo per le domeniche
            int domenicheNelPeriodo = UtilitaDate.Domeniche(dataraggiunta, dataFine);
            var rimanenzaConCorrettivo = rimanenzagg - domenicheNelPeriodo;
            if (rimanenzaConCorrettivo > rimanenzaGGbase26)
            {
                giorni = rimanenzaConCorrettivo;
            }
            else
            {
                giorni = rimanenzaGGbase26;
            }

            if (giorni >= 26)
            {
                giorni = 0;
                mesi += 1;
                if (mesi == 12)
                {
                    mesi = 0;
                    anni += 1;
                }
            }

            return (anni, mesi, giorni);

        }
        private (int Anni, int Mesi, int Giorni) Capienza(Periodo periodo)
        {
            DateTime startDate = periodo.Inizio;
            DateTime endDate = periodo.Fine;

            int totalDays = (endDate.Year - startDate.Year) * 360 + (endDate.Month - startDate.Month) * 30 + (endDate.Day - startDate.Day);

            // Calcoliamo gli anni
            int years = totalDays / 360;
            totalDays %= 360;

            // Calcoliamo i mesi
            int months = totalDays / 30;
            totalDays %= 30;

            // Calcoliamo i giorni rimanenti
            int days = totalDays;

            return (years, months, days);

        }

        static List<string> CalcolaPeriodiScoperti(DateTime startDate, DateTime endDate, List<(DateTime start, DateTime end)> ranges)
        {
            // Ordina i range per data di inizio
            ranges = ranges.OrderBy(r => r.start).ToList();

            List<string> periodiScoperti = new List<string>();

            // Verifica se c'è un periodo scoperto prima del primo range
            if (ranges[0].start > startDate)
            {
                periodiScoperti.Add($"{startDate.ToString("yyyy-MM-dd")} to {ranges[0].start.AddDays(-1).ToString("yyyy-MM-dd")}");
            }

            // Verifica i periodi scoperti tra i range
            for (int i = 1; i < ranges.Count; i++)
            {
                if (ranges[i].start > ranges[i - 1].end.AddDays(1))
                {
                    periodiScoperti.Add($"{ranges[i - 1].end.AddDays(1).ToString("yyyy-MM-dd")} to {ranges[i].start.AddDays(-1).ToString("yyyy-MM-dd")}");
                }
            }

            // Verifica se c'è un periodo scoperto dopo l'ultimo range
            if (ranges[^1].end < endDate)
            {
                periodiScoperti.Add($"{ranges[^1].end.AddDays(1).ToString("yyyy-MM-dd")} to {endDate.ToString("yyyy-MM-dd")}");
            }

            return periodiScoperti;
        }
    }
}
