using System;

namespace TotCum.Data
{
    public class Minimale
    {
        public short Anno { get; set; }
        public double Valore { get; set; }
    }

    public static class Minimali
    {
        public static List<Minimale> ListaMinimali { set; get; }

        static Minimali()
        {
            ListaMinimali =
            [
                new() { Anno = 1996, Valore = 10779.20383},
                new() { Anno = 1997, Valore = 11173.33843},
                new() { Anno = 1998, Valore = 11351.71438},
                new() { Anno = 1999, Valore = 11543.78677},
                new() { Anno = 2000, Valore = 11717.48981},
                new() { Anno = 2001, Valore = 12004.47045},
                new() { Anno = 2002, Valore = 12312},
                new() { Anno = 2003, Valore = 12590},
                new() { Anno = 2004, Valore = 12889},
                new() { Anno = 2005, Valore = 13133},
                new() { Anno = 2006, Valore = 13345},
                new() { Anno = 2007, Valore = 13598},
                new() { Anno = 2008, Valore = 13819},
                new() { Anno = 2009, Valore = 14240},
                new() { Anno = 2010, Valore = 14334},
                new() { Anno = 2011, Valore = 14552},
                new() { Anno = 2012, Valore = 14930},
                new() { Anno = 2013, Valore = 15357},
                new() { Anno = 2014, Valore = 15516},
                new() { Anno = 2015, Valore = 15548},
                new() { Anno = 2016, Valore = 15548}, // circolare n.13 del 29/01/2016
                new() { Anno = 2017, Valore = 15548}, // circolare n.21 del 31/01/2017
                new() { Anno = 2018, Valore = 15710}, // circolare n.18 del 31/01/2018
                new() { Anno = 2019, Valore = 15878}, // circolare n.19 del 06/02/2019
                new() { Anno = 2020, Valore = 15953}, // circolare n.12 del 03/02/2020
                new() { Anno = 2021, Valore = 15953}, // circolare n.12 del 05/02/2021
                new() { Anno = 2022, Valore = 16243}, // circolare n.25 del 11/02/2022
                new() { Anno = 2023, Valore = 17504}, // circolare n.19 del 10/02/2023
                new() { Anno = 2024, Valore = 18415}, // circolare n.24 del 29/01/2024

            ];
        }
    }


}
