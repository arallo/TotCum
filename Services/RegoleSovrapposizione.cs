using TotCum.Models;

namespace TotCum.Services
{
    public interface IRegolaSovrapposizione
    {
        bool SiSovrappongono(Periodo periodo1, Periodo periodo2);
        void CorreggiSovrapposizione(Periodo periodo1, Periodo periodo2);
    }

    public class RegolaSovrapposizioneFPLD : IRegolaSovrapposizione
    {
        public bool SiSovrappongono(Periodo periodo1, Periodo periodo2)
        {
            return periodo1.Fine >= periodo2.Inizio && periodo1.Inizio <= periodo2.Fine;
        }

        public void CorreggiSovrapposizione(Periodo periodo1, Periodo periodo2)
        {
            if (periodo1.TipoContribuzione == TipoContribuzione.Obbligatoria && (periodo2.TipoContribuzione == TipoContribuzione.Obbligatoria | periodo2.TipoContribuzione == TipoContribuzione.Figurativa))
            {
                if (periodo1.Inizio == periodo2.Inizio && periodo1.Fine == periodo2.Fine)
                {
                    periodo1.Settimane += periodo2.Settimane;
                    periodo1.Settimane = periodo1.Settimane > 52 ? 52 : periodo1.Settimane;
                    periodo2.Neutralizzato = true;
                }
                else 
                {
                    periodo2.Inizio = periodo1.Fine.AddDays(1);
                    int capienza1 = UtilitaDate.Settimane(periodo1.Inizio, periodo1.Fine);
                    int capienza2 = UtilitaDate.Settimane(periodo2.Inizio, periodo2.Fine);
                    periodo1.Settimane = periodo1.Settimane > capienza1 ? capienza1 : periodo1.Settimane;
                    periodo2.Settimane = periodo2.Settimane > capienza2 ? capienza2 : periodo2.Settimane;

                    // Verifica che il totale delle settimane per l'anno non superi 52
                    var settimaneTotaliAnno = periodo1.Settimane + periodo2.Settimane;
                    if (settimaneTotaliAnno > 52)
                    {
                        periodo2.Settimane = 52 - periodo1.Settimane;
                    }
                }
            }

            if (periodo1.TipoContribuzione == TipoContribuzione.Figurativa && periodo2.TipoContribuzione == TipoContribuzione.Obbligatoria)
            {
                if (periodo1.Inizio == periodo2.Inizio && periodo1.Fine == periodo2.Fine)
                {
                    periodo2.Settimane += periodo1.Settimane;
                    periodo2.Settimane = periodo2.Settimane > 52 ? 52 : periodo2.Settimane;
                    periodo1.Neutralizzato = true;
                }
                else
                {
                    periodo1.Fine = periodo2.Inizio.AddDays(-1);
                    int capienza1 = UtilitaDate.Settimane(periodo1.Inizio, periodo1.Fine);
                    int capienza2 = UtilitaDate.Settimane(periodo2.Inizio, periodo2.Fine);
                    periodo1.Settimane = periodo1.Settimane > capienza1 ? capienza1 : periodo1.Settimane;
                    periodo2.Settimane = periodo2.Settimane > capienza2 ? capienza2 : periodo2.Settimane;

                    // Verifica che il totale delle settimane per l'anno non superi 52
                    var settimaneTotaliAnno = periodo1.Settimane + periodo2.Settimane;
                    if (settimaneTotaliAnno > 52)
                    {
                        periodo1.Settimane = 52 - periodo2.Settimane;
                    }
                }
            }

            if (periodo1.TipoContribuzione == TipoContribuzione.Figurativa && periodo2.TipoContribuzione == TipoContribuzione.Figurativa)
            {
                if (periodo1.Inizio == periodo2.Inizio && periodo1.Fine == periodo2.Fine)
                {
                    periodo1.Settimane += periodo2.Settimane;
                    periodo1.Settimane = periodo1.Settimane > 52 ? 52 : periodo1.Settimane;
                    periodo2.Neutralizzato = true;
                }
                else
                {
                    periodo2.Inizio = periodo1.Fine.AddDays(1);
                    int capienza1 = UtilitaDate.Settimane(periodo1.Inizio, periodo1.Fine);
                    int capienza2 = UtilitaDate.Settimane(periodo2.Inizio, periodo2.Fine);
                    periodo1.Settimane = periodo1.Settimane > capienza1 ? capienza1 : periodo1.Settimane;
                    periodo2.Settimane = periodo2.Settimane > capienza2 ? capienza2 : periodo2.Settimane;

                    // Verifica che il totale delle settimane per l'anno non superi 52
                    var settimaneTotaliAnno = periodo1.Settimane + periodo2.Settimane;
                    if (settimaneTotaliAnno > 52)
                    {
                        periodo2.Settimane = 52 - periodo1.Settimane;
                    }
                }
            }
        }
    }

    public class RegolaSovrapposizioneFPLDAutonomi : IRegolaSovrapposizione
    {
        public bool SiSovrappongono(Periodo periodo1, Periodo periodo2)
        {
            return periodo1.Fine >= periodo2.Inizio && periodo1.Inizio <= periodo2.Fine;
        }

        public void CorreggiSovrapposizione(Periodo periodo1, Periodo periodo2)
        {
            if (periodo1.TipoContribuzione == TipoContribuzione.Obbligatoria && periodo2.TipoContribuzione == TipoContribuzione.Obbligatoria)
            {
                if (periodo1.Settimane > 26)
                {
                    periodo2.Neutralizzato = true;
                }
                else 
                {
                    periodo2.Inizio = periodo1.Fine.AddDays(1);
                    int capienza1 = UtilitaDate.Settimane(periodo1.Inizio, periodo1.Fine);
                    int capienza2 = UtilitaDate.Settimane(periodo2.Inizio, periodo2.Fine);
                    periodo1.Settimane = periodo1.Settimane > capienza1 ? capienza1 : periodo1.Settimane;
                    periodo2.Settimane = periodo2.Settimane > capienza2 ? capienza2 : periodo2.Settimane;

                    // Verifica che il totale delle settimane per l'anno non superi 52
                    var settimaneTotaliAnno = periodo1.Settimane + periodo2.Settimane;
                    if (settimaneTotaliAnno > 52)
                    {
                        periodo2.Settimane = 52 - periodo1.Settimane;
                    }
                }
            }

            if (periodo1.TipoContribuzione == TipoContribuzione.Figurativa && periodo2.TipoContribuzione == TipoContribuzione.Obbligatoria)
            {
                periodo1.Fine = periodo2.Inizio.AddDays(-1);
                int capienza1 = UtilitaDate.Settimane(periodo1.Inizio, periodo1.Fine);
                int capienza2 = UtilitaDate.Settimane(periodo2.Inizio, periodo2.Fine);
                periodo1.Settimane = periodo1.Settimane > capienza1 ? capienza1 : periodo1.Settimane;
                periodo2.Settimane = periodo2.Settimane > capienza2 ? capienza2 : periodo2.Settimane;

                // Verifica che il totale delle settimane per l'anno non superi 52
                var settimaneTotaliAnno = periodo1.Settimane + periodo2.Settimane;
                if (settimaneTotaliAnno > 52)
                {
                    periodo1.Settimane = 52 - periodo2.Settimane;
                }
            }
        }
    }

    public class RegolaSovrapposizioneAutonomiFPLD : IRegolaSovrapposizione
    {
        public bool SiSovrappongono(Periodo periodo1, Periodo periodo2)
        {
            return periodo1.Fine >= periodo2.Inizio && periodo1.Inizio <= periodo2.Fine;
        }

        public void CorreggiSovrapposizione(Periodo periodo1, Periodo periodo2)
        {
            if (periodo1.TipoContribuzione == TipoContribuzione.Obbligatoria && periodo2.TipoContribuzione == TipoContribuzione.Obbligatoria)
            {
                if (periodo2.Settimane > 26)
                {
                    periodo1.Neutralizzato = true;
                }
                else
                {
                    periodo1.Fine = periodo2.Inizio.AddDays(-1);
                    int capienza1 = UtilitaDate.Settimane(periodo1.Inizio, periodo1.Fine);
                    int capienza2 = UtilitaDate.Settimane(periodo2.Inizio, periodo2.Fine);
                    periodo1.Settimane = periodo1.Settimane > capienza1 ? capienza1 : periodo1.Settimane;
                    periodo2.Settimane = periodo2.Settimane > capienza2 ? capienza2 : periodo2.Settimane;

                    // Verifica che il totale delle settimane per l'anno non superi 52
                    var settimaneTotaliAnno = periodo1.Settimane + periodo2.Settimane;
                    if (settimaneTotaliAnno > 52)
                    {
                        periodo1.Settimane = 52 - periodo2.Settimane;
                    }
                }
            }

            if (periodo1.TipoContribuzione == TipoContribuzione.Obbligatoria && periodo2.TipoContribuzione == TipoContribuzione.Figurativa)
            {
                periodo2.Inizio = periodo1.Fine.AddDays(1);
                int capienza1 = UtilitaDate.Settimane(periodo1.Inizio, periodo1.Fine);
                int capienza2 = UtilitaDate.Settimane(periodo2.Inizio, periodo2.Fine);
                periodo1.Settimane = periodo1.Settimane > capienza1 ? capienza1 : periodo1.Settimane;
                periodo2.Settimane = periodo2.Settimane > capienza2 ? capienza2 : periodo2.Settimane;

                // Verifica che il totale delle settimane per l'anno non superi 52
                var settimaneTotaliAnno = periodo1.Settimane + periodo2.Settimane;
                if (settimaneTotaliAnno > 52)
                {
                    periodo2.Settimane = 52 - periodo1.Settimane;
                }
            }

        }
    }

    public class RegolaSovrapposizioneAutonomi : IRegolaSovrapposizione
    {
        public bool SiSovrappongono(Periodo periodo1, Periodo periodo2)
        {
            return periodo1.Fine >= periodo2.Inizio && periodo1.Inizio <= periodo2.Fine;
        }

        public void CorreggiSovrapposizione(Periodo periodo1, Periodo periodo2)
        {
            if (periodo1.Fondo == TipoFondo.COM && periodo2.Fondo == TipoFondo.ART)
            {
                periodo2.Inizio = periodo1.Fine.AddDays(1);
                int capienza1 = UtilitaDate.Settimane(periodo1.Inizio, periodo1.Fine);
                int capienza2 = UtilitaDate.Settimane(periodo2.Inizio, periodo2.Fine);
                periodo1.Settimane = periodo1.Settimane > capienza1 ? capienza1 : periodo1.Settimane;
                periodo2.Settimane = periodo2.Settimane > capienza2 ? capienza2 : periodo2.Settimane;

                // Verifica che il totale delle settimane per l'anno non superi 52
                var settimaneTotaliAnno = periodo1.Settimane + periodo2.Settimane;
                if (settimaneTotaliAnno > 52)
                {
                    periodo2.Settimane = 52 - periodo1.Settimane;
                }

            }
            if (periodo1.Fondo == TipoFondo.ART && periodo2.Fondo == TipoFondo.COM)
            {
                periodo1.Fine = periodo2.Inizio.AddDays(-1);
                int capienza1 = UtilitaDate.Settimane(periodo1.Inizio, periodo1.Fine);
                int capienza2 = UtilitaDate.Settimane(periodo2.Inizio, periodo2.Fine);
                periodo1.Settimane = periodo1.Settimane > capienza1 ? capienza1 : periodo1.Settimane;
                periodo2.Settimane = periodo2.Settimane > capienza2 ? capienza2 : periodo2.Settimane;

                // Verifica che il totale delle settimane per l'anno non superi 52
                var settimaneTotaliAnno = periodo1.Settimane + periodo2.Settimane;
                if (settimaneTotaliAnno > 52)
                {
                    periodo1.Settimane = 52 - periodo2.Settimane;
                }

            }

            if (periodo1.Fondo == TipoFondo.ART | periodo1.Fondo == TipoFondo.COM && periodo2.Fondo == TipoFondo.CDCM)
            {
                periodo2.Inizio = periodo1.Fine.AddDays(1);
                int capienza1 = UtilitaDate.Settimane(periodo1.Inizio, periodo1.Fine);
                int capienza2 = UtilitaDate.Settimane(periodo2.Inizio, periodo2.Fine);
                periodo1.Settimane = periodo1.Settimane > capienza1 ? capienza1 : periodo1.Settimane;
                periodo2.Settimane = periodo2.Settimane > capienza2 ? capienza2 : periodo2.Settimane;

                // Verifica che il totale delle settimane per l'anno non superi 52
                var settimaneTotaliAnno = periodo1.Settimane + periodo2.Settimane;
                if (settimaneTotaliAnno > 52)
                {
                    periodo2.Settimane = 52 - periodo1.Settimane;
                }
            }
        }
    }

    public class RegolaSovrapposizioneAGOAltriFondi : IRegolaSovrapposizione
    {
        public bool SiSovrappongono(Periodo periodo1, Periodo periodo2)
        {
            return periodo1.Fine >= periodo2.Inizio && periodo1.Inizio <= periodo2.Fine;
        }

        public void CorreggiSovrapposizione(Periodo periodo1, Periodo periodo2)
        {
            if (periodo1.Inizio == periodo2.Inizio && periodo1.Fine == periodo2.Fine)
            {
                periodo2.Neutralizzato = true;
            }
            else
            {
                periodo1.Fine = periodo2.Inizio.AddDays(-1);
            }

        }
    }
}
