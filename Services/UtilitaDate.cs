namespace TotCum.Services
{
    public static class UtilitaDate
    {
        public static int Settimane(DateTime inizio, DateTime fine)
        {
            var dataDal = inizio;
            var dataAl = fine;

            if (dataAl.Year > dataDal.Year)
            {
                throw new Exception($"Il periodo {dataDal.ToShortDateString()}-{dataAl.ToShortDateString()} copre più anni e va spacchettato");
            }

            if (dataAl < dataDal)
            {
                //throw new Exception($"La data {dataDal.ToShortDateString()} è successiva alla data {dataAl.ToShortDateString()}");
                var tmp = dataAl;
                dataAl = dataDal;
                dataDal = tmp;
            }

            int settimane = 0;
            int sett1 = 0;
            int sett2 = 0;
            int sett3 = 0;

            //1 - Caso mesi interi
            if (dataDal.Day == 1 && dataAl.Day == DateTime.DaysInMonth(dataAl.Year, dataAl.Month))
            {
                int mesi = dataAl.Month - dataDal.Month + 1;
                settimane = Convert.ToInt32(Math.Ceiling(mesi * 4.333));
            }


            // 2 - Caso con mesi non interi (si contano i sabati)
            if (dataDal.Day > 1 && dataAl.Day < DateTime.DaysInMonth(dataAl.Year, dataAl.Month))
            {
                int sabati = Sabati(dataDal, dataAl);
                int settdagiornidiv7 = ((dataAl - dataDal).Days + 1) / 7;
                if (sabati > settdagiornidiv7)
                {
                    settimane = sabati;
                }
                else
                {
                    settimane = settdagiornidiv7;
                }
            }


            // 3.1 - situazione ibrida
            if (dataDal.Day > 1 && dataAl.Day == DateTime.DaysInMonth(dataAl.Year, dataAl.Month))
            {
                int mesi = dataAl.Month - dataDal.Month + 1;
                sett1 = Convert.ToInt32(Math.Ceiling(mesi * 4.333));
                //conto i sabati che intercorrono tra la data iniziale e l’ultimo giorno del mese relativo alla data iniziale
                int appo_sett = Sabati(dataDal, new DateTime(dataDal.Year, dataDal.Month, DateTime.DaysInMonth(dataDal.Year, dataDal.Month)));
                // procedo al conteggio dei mesi che intercorrono dal primo giorno del mese successivo alla data inizio fino alla fine 
                // del periodo e si convertono i mesi In settimane moltiplicando per 4, 333
                int appo_sett2 = Convert.ToInt32(Math.Ceiling((dataAl.Month - PrimoGiornoMeseSuccessivoDi(dataDal).Month + 1) * 4.333));
                sett2 = appo_sett + appo_sett2;
                // conto i sabati e quindi le settimane che intercorrono tra la data iniziale e la data finale del periodo in oggetto
                int sabati = Sabati(dataDal, dataAl);
                int settdagiornidiv7 = ((dataAl - dataDal).Days + 1) / 7;
                if (sabati > settdagiornidiv7)
                {
                    sett3 = sabati;
                }
                else
                {
                    sett3 = settdagiornidiv7;
                }

                // restituisco il valore minore
                int min1 = Math.Min(sett1, sett2);
                settimane = Math.Min(sett3, min1);
            }

            // 3.2 - situazione ibrida

            if (dataDal.Day == 1 && dataAl.Day < DateTime.DaysInMonth(dataAl.Year, dataAl.Month))
            {
                int mesi = dataAl.Month - dataDal.Month + 1;
                sett1 = Convert.ToInt32(Math.Ceiling(mesi * 4.333));
                // conto i sabati che intercorrono tra l'inizio del mese della data iniziale e l’ultimo giorno del mese relativo alla data finale
                int appo_sett = Sabati(new DateTime(dataAl.Year, dataAl.Month, 1), dataAl);
                // procedo al conteggio dei mesi che intercorrono dal primo giorno del mese inizio fino alla fine 
                // del mese antecedente la fine del periodo e si convertono i mesi In settimane moltiplicando per 4, 333
                int appo_sett2 = Convert.ToInt32(Math.Ceiling((dataAl.AddMonths(-1).Month - dataDal.Month + 1) * 4.333));
                sett2 = appo_sett + appo_sett2;
                // conto i sabati e quindi le settimane che intercorrono tra la data iniziale e la data finale del periodo in oggetto
                int sabati = Sabati(dataDal, dataAl);
                int settdagiornidiv7 = ((dataAl - dataDal).Days + 1) / 7;
                if (sabati > settdagiornidiv7)
                {
                    sett3 = sabati;
                }
                else
                {
                    sett3 = settdagiornidiv7;
                }
                // restituisco il valore minore
                int minimo_parziale = Math.Min(sett1, sett2);
                int minimo = Math.Min(sett3, minimo_parziale);
                settimane = minimo;
            }

            if (settimane > 52)
            {
                settimane = 52;
            }
            return settimane;

        }

        public static int Sabati(DateTime FromDate, DateTime ToDate)
        {
            var sabati = 0;
            DateTime dCheck = FromDate;
            TimeSpan tsToAdd = TimeSpan.FromDays(1);

            do
            {
                if (dCheck.DayOfWeek == DayOfWeek.Saturday)
                    sabati += 1;
                dCheck += tsToAdd;
                if (dCheck > ToDate)
                    break;
            }
            while (true);
            return sabati;
        }

        public static int Domeniche(DateTime FromDate, DateTime ToDate)
        {
            var domeniche = 0;
            DateTime dCheck = FromDate;
            TimeSpan tsToAdd = TimeSpan.FromDays(1);

            do
            {
                if (dCheck.DayOfWeek == DayOfWeek.Sunday)
                    domeniche += 1;
                dCheck += tsToAdd;
                if (dCheck > ToDate)
                    break;
            }
            while (true);
            return domeniche;
        }

        public static (int Anni, int Mesi, int Giorni, int Settimane)? GetDateOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            DateTime overlapStart = start1 > start2 ? start1 : start2;
            DateTime overlapEnd = end1 < end2 ? end1 : end2;

            if (overlapStart < overlapEnd)
            {
                var differenza = CalcolaDifferenzaDate(overlapStart, overlapEnd);
                return (differenza.Anni, differenza.Mesi, differenza.Giorni, differenza.Settimane);
            }
            else
            {
                return null;
            }
        }

        public static (int Anni, int Mesi, int Giorni, int Settimane) CalcolaDifferenzaDate(DateTime inizio, DateTime fine)
        {
            int anni = fine.Year - inizio.Year;
            int mesi = fine.Month - inizio.Month;
            int giorni = fine.Day - inizio.Day;

            if (giorni < 0)
            {
                mesi--;
                giorni += DateTime.DaysInMonth(inizio.Year, inizio.Month);
            }

            if (mesi < 0)
            {
                anni--;
                mesi += 12;
            }

            int settimane = UtilitaDate.Settimane(inizio, fine);

            return (anni, mesi, giorni, settimane);
        }

        public static bool DateOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            DateTime overlapStart = start1 > start2 ? start1 : start2;
            DateTime overlapEnd = end1 < end2 ? end1 : end2;

            if (overlapStart < overlapEnd)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static DateTime PrimoGiornoMeseSuccessivoDi(DateTime dt)
        {
            DateTime tempDt = dt;
            tempDt = tempDt.AddMonths(1);
            return new DateTime(tempDt.Year, tempDt.Month, 1);
        }

        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }


        public static TipoOverlapData2 TestOverlapDates(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            // 1) ev2 contenuto in ev1
            //     ev1: +---------------+
            //     ev2:       +---+

            // 2) ev1 contenuto in ev2 (sovrapposti)
            //     ev1:       +--------+
            //     ev2: +-----------------+

            // 3) parzialmente contenuto, ev1 inizia prima di ev2 (sovrapposti)
            //     ev1:   +--------+
            //     ev2:        +---------------+

            // 4) parzialmente contenuto, ev1 inizia dopo ev2 (sovrapposti)
            //     ev1:       +--------+
            //     ev2: +--------+

            // 5) eventi esattamente sovrapposti (coincidenti)
            //     ev1: +--------+
            //     ev2: +--------+

            // 6) ev1 contenuto in ev2 con data inizio uguale (sovrapposti)
            //     ev1: +--------+
            //     ev2: +---------------+

            // 7) ev1 contenuto in ev2 con data fine uguale (sovrapposti)
            //     ev1:      +---+
            //     ev2: +--------+

            // 8) ev2 contenuto in ev1 con data inizio uguale (sovrapposti)
            //     ev1: +---------------+
            //     ev2: +--------+

            // 9) ev2 contenuto in ev1 con data fine uguale (sovrapposti)
            //     ev1: +--------------+
            //     ev2:       +--------+

            if (start1 > end1 || start2 > end2)
            {
                throw new ArgumentException("Date invertite");
            }

            if ((start1 < start2 && end1 > end2)) // caso 1 ev2 contenuto in ev1
            {
                return TipoOverlapData2.periodo2_contenuto_in_periodo1;
            }
            else if ((start1 > start2 && end1 < end2)) // caso 2 ev1 contenuto in ev2
            {
                return TipoOverlapData2.periodo1_contenuto_in_periodo2;
            }
            else if ((start1 < start2 && end1 > start2)) // caso 3 parzialmente contenuto, ev1 inizia prima di ev2
            {
                return TipoOverlapData2.periodo1_inizia_prima_di_periodo2;
            }
            else if ((start1 < end2 && end1 > end2)) // caso 4 parzialmente contenuto, ev1 inizia dopo ev2
            {
                return TipoOverlapData2.periodo1_inizia_dopo_periodo2;
            }
            else if ((start1 == start2 && end1 == end2)) // caso 5 eventi esattamente sovrapposti (coincidenti)
            {
                return TipoOverlapData2.periodi_coincidenti;
            }
            else if ((start1 == start2 && end1 < end2)) // caso 6 ev1 contenuto in ev2 con data inizio uguale 
            {
                return TipoOverlapData2.periodo1_contenuto_in_periodo2_inizio_uguale;
            }
            else if ((start2 < start1 && end1 == end2)) // caso 7 ev1 contenuto in ev2 con data fine uguale 
            {
                return TipoOverlapData2.periodo1_contenuto_in_periodo2_fine_uguale;
            }
            else if ((start1 == start2 && end2 < end1)) // caso 8 ev2 contenuto in ev1 con data inizio uguale 
            {
                return TipoOverlapData2.periodo2_contenuto_in_periodo1_inizio_uguale;
            }
            else if ((start1 < start2 && end2 == end1)) // caso 9 ev2 contenuto in ev1 con data fine uguale
            {
                return TipoOverlapData2.periodo2_contenuto_in_periodo1_fine_uguale;
            }
            else 
            {
                return TipoOverlapData2.periodi_non_sovrapposti;
            }
        }

        public enum TipoOverlapData2
        {
            periodi_non_sovrapposti,
            periodo2_contenuto_in_periodo1,
            periodo1_contenuto_in_periodo2,
            periodo1_inizia_prima_di_periodo2,
            periodo1_inizia_dopo_periodo2,
            periodi_coincidenti,
            periodo1_contenuto_in_periodo2_inizio_uguale,
            periodo1_contenuto_in_periodo2_fine_uguale,
            periodo2_contenuto_in_periodo1_inizio_uguale,
            periodo2_contenuto_in_periodo1_fine_uguale,

        }

    }


}
