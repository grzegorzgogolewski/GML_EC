namespace GML_EC
{
    public static class Dict
    {
        public static string DC_RodzajDokumentuType(string val)
        {
            string wartosc = "";

            switch (val)
            {
                case "1":
                    wartosc = "umowaAktNotarialny";
                    break;
                case "2":
                    wartosc = "aktWlasnosciZiemi";
                    break;
                case "3":
                    wartosc = "decyzjaAdminInnaNizAWZ";
                    break;
                case "4":
                    wartosc = "orzeczenieSaduPostanowienieWyrok";
                    break;
                case "5":
                    wartosc = "wyciagOdpisZKsiegiWieczystej";
                    break;
                case "6":
                    wartosc = "wyciagOdpisZKsiegiHipotecznej";
                    break;
                case "7":
                    wartosc = "odpisAktKWLubZbioruDokumentu";
                    break;
                case "8":
                    wartosc = "zawiadomienieZWydzialuKW";
                    break;
                case "9":
                    wartosc = "wniosekWSprawieZmiany";
                    break;
                case "10":
                    wartosc = "wyciagZDokumentacjiBudowyBudynku";
                    break;
                case "11":
                    wartosc = "protokol";
                    break;
                case "12":
                    wartosc = "ustawa";
                    break;
                case "13":
                    wartosc = "rozporzadzenie";
                    break;
                case "14":
                    wartosc = "uchwala";
                    break;
                case "15":
                    wartosc = "zarzadzenie";
                    break;
                case "16":
                    wartosc = "odpisWyciagZInnegoRejestruPublicznego";
                    break;
                case "17":
                    wartosc = "pelnomocnictwo";
                    break;
                case "18":
                    wartosc = "wyciagZOperatuSzacunkowego";
                    break;
                case "19":
                    wartosc = "innyDokument";
                    break;
                case "20":
                    wartosc = "dokArchitektoniczoBud";
                    break;
                case "21":
                    wartosc = "dokPlanistyczny";
                    break;
                case "22":
                    wartosc = "protokolNaradyKoordynacyjnej";
                    break;
                case "23":
                    wartosc = "umowaDzierzawy";
                    break;
            }

            return wartosc;
        }

        public static string EGB_RodzajBlokuType(string val)
        {
            string wartosc = "";

            switch (val)
            {
                case "1":
                    wartosc = "kondygnacjeNadziemne";
                    break;
                case "2":
                    wartosc = "kondygnacjePodziemne";
                    break;
                case "3":
                    wartosc = "lacznik";
                    break;
                case "4":
                    wartosc = "nawis";
                    break;
                case "5":
                    wartosc = "przejazdPrzezBudynek";
                    break;
                case "6":
                    wartosc = "inny";
                    break;
            }

            return wartosc;
        }

        public static string EGB_RodzajObiektuZwiazanegoZBudynkiemType(string val)
        {
            string wartosc="";

            switch (val)
            {
                case "1":
                    wartosc = "taras";
                    break;
                case "2":
                    wartosc = "werandaGanek";
                    break;
                case "3":
                    wartosc = "wiatrolap";
                    break;
                case "4":
                    wartosc = "schody";
                    break;
                case "5":
                    wartosc = "podpora";
                    break;
                case "6":
                    wartosc = "rampa";
                    break;
                case "7":
                    wartosc = "wjazdDoPodziemia";
                    break;
                case "8":
                    wartosc = "podjazdDlaOsobNiepelnosprawnych";
                    break;
                case "9":
                    wartosc = "inny";
                    break;
            }

            return wartosc;
        }
    
        public static string EGB_RodzajWladaniaType(string val)
        {
            string wartosc = "";

            switch (val)
            {
                case "1":
                    wartosc = "uzytkowanieWieczyste";
                    break;
                case "2":
                    wartosc = "trwalyZarzad";
                    break;
                case "3":
                    wartosc = "zarzad";
                    break;
                case "4":
                    wartosc = "uzytkowanie";
                    break;
                case "5":
                    wartosc = "innyRodzajWladania";
                    break;
            }

            return wartosc;
        }

        public static string EGB_RodzajUprawnienType(string val)
        {
            string wartosc = "";

            switch (val)
            {
                case "1":
                    wartosc = "wykonywaniePrawaWlasnosciSPIInnychPrawRzeczowych";
                    break;
                case "2":
                    wartosc = "gospodarowanieZasobemNieruchomosciSPLubGmPowWoj";
                    break;
                case "3":
                    wartosc = "gospodarowanieGruntemSPPokrytymWodamiPowierzchniowymi";
                    break;
                case "4":
                    wartosc = "wykonywanieZadanZarzadcyDrogPub";
                    break;
            }

            return wartosc;
        }

        public static string EGB_StatusPodmiotuEwidType(string val)
        {
            string wartosc = "";

            switch (val)
            {
                case "1":
                    wartosc = "osobaFizycznaLegitymujacaSieObywatelstwemPolskim";
                    break;
                case "2":
                    wartosc = "osobaFizycznaBezObywatelstwaPolskiego";
                    break;
                case "3":
                    wartosc = "skarbPanstwa";
                    break;
                case "4":
                    wartosc = "gminaLubZwiazekMiedzygminny";
                    break;
                case "5":
                    wartosc = "solectwo";
                    break;
                case "6":
                    wartosc = "panstwowaOsobaPrawnaLubJednoosobowaSpolkaSkarbuPanstwa";
                    break;
                case "7":
                    wartosc = "panstwoweGospodarstwoLesneLasyPanstwowe";
                    break;
                case "8":
                    wartosc = "agencjaNieruchomosciRolnych";
                    break;
                case "9":
                    wartosc = "agencjaMieniaWojskowego";
                    break;
                case "10":
                    wartosc = "wojskowaAgencjaMieszkaniowa";
                    break;
                case "11":
                    wartosc = "panstwowaJednostkaOrganizacyjnaBezOsobowosciPrawnej";
                    break;
                case "12":
                    wartosc = "gminnaJednostkaOrganizacyjnaBezOsobowosciPrawnej";
                    break;
                case "13":
                    wartosc = "powiatowaJednostkaOrganizacyjnaBezOsobowosciPrawnej";
                    break;
                case "14":
                    wartosc = "wojewodzkaJednostkaOrganizacyjnaBezOsobowosciPrawnej";
                    break;
                case "15":
                    wartosc = "gminnaOsobaPrawnaLubJednoosobowaSpolkaGminy";
                    break;
                case "16":
                    wartosc = "powiatowaOsobaPrawnaLubJednoosobowaSpolkaPowiatu";
                    break;
                case "17":
                    wartosc = "wojewodzkaOsobaPrawnaLubJednoosobowaSpolkaWojewodztwa";
                    break;
                case "18":
                    wartosc = "ministerSkarbuPanstwa";
                    break;
                case "19":
                    wartosc = "starosta";
                    break;
                case "20":
                    wartosc = "wojtBurmistrzPrezydentMiasta";
                    break;
                case "21":
                    wartosc = "zarzadPowiatu";
                    break;
                case "22":
                    wartosc = "zarzadWojewodztwa";
                    break;
                case "23":
                    wartosc = "spoldzielniaMieszkaniowa";
                    break;
                case "24":
                    wartosc = "spoldzielniaLubZwiazekSpoldzielni";
                    break;
                case "25":
                    wartosc = "kosciolyLubZwiazkiWyznaniowe";
                    break;
                case "26":
                    wartosc = "spolkaHandlowaNieBedacaCudzoziemcem";
                    break;
                case "27":
                    wartosc = "spolkaHandlowaBedacaCudzoziemcem";
                    break;
                case "28":
                    wartosc = "osobaPrawnaInnaNizSpolkaHandlowaBedacaCudzoziemcem";
                    break;
                case "29":
                    wartosc = "partiaPolityczna";
                    break;
                case "30":
                    wartosc = "stowarzyszenie";
                    break;
                case "31":
                    wartosc = "jednOrganizacyjnaNieBedacaOsobaPrawnaZeZdolnosciaPrawna";
                    break;
                case "32":
                    wartosc = "podmiotyPozostajaceWeWspolwlasnosciLacznej";
                    break;
                case "33":
                    wartosc = "spolkaCywilna";
                    break;
                case "34":
                    wartosc = "malzenstwoObywateliPolskich";
                    break;
                case "35":
                    wartosc = "malzenstwoJedenCudzoziemiec";
                    break;
                case "36":
                    wartosc = "wojewodztwo";
                    break;
                case "37":
                    wartosc = "powiat";
                    break;
                case "38":
                    wartosc = "spoldzielniaRolnicza";
                    break;
                case "39":
                    wartosc = "";
                    break;
                case "40":
                    wartosc = "wlascicielNieustalony";
                    break;
                case "41":
                    wartosc = "wspolnotaGruntowa";
                    break;
                case "42":
                    wartosc = "ministerGospodarkiMorskiej";
                    break;
                case "43":
                    wartosc = "prezesKrajowegoZarzaduGospodarkiWodnej";
                    break;
                case "44":
                    wartosc = "generalnyDyrektorDrogKrajowychIAutostrad";
                    break;
                case "45":
                    wartosc = "dyrektorParkuNarodowego";
                    break;
                case "46":
                    wartosc = "marszalekWojewodztwa";
                    break;
                case "47":
                    wartosc = "inne";
                    break;
            }

            return wartosc;
        }

        public static string EGB_PlecType(string val)
        {
            string wartosc = "";

            switch (val)
            {
                case "1":
                    wartosc = "meska";
                    break;
                case "2":
                    wartosc = "zenska";
                    break;
            }

            return wartosc;
        }

        public static string EGB_RodzajPomieszczeniaPrzynaleznegoDoLokaluType(string val)
        {
            string wartosc = "";

            switch (val)
            {
                case "1":
                    wartosc = "piwnica";
                    break;
                case "2":
                    wartosc = "garaz";
                    break;
                case "3":
                    wartosc = "miejscePostojoweWGarazuWielostanowiskowym";
                    break;
                case "4":
                    wartosc = "strych";
                    break;
                case "5":
                    wartosc = "komorka";
                    break;
                case "6":
                    wartosc = "inne";
                    break;
            }

            return wartosc;
        }

        public static string EGB_KodStabilizacjiType(string val)
        {
            string wartosc = "";

            switch (val)
            {
                case "1":
                    wartosc = "brakInformacji";
                    break;
                case "2":
                    wartosc = "niestabilizowany";
                    break;
                case "3":
                    wartosc = "znakNaziemny";
                    break;
                case "4":
                    wartosc = "znakNaziemnyIPodziemny";
                    break;
                case "5":
                    wartosc = "znakPodziemny";
                    break;
            }

            return wartosc;
        }

        public static string EGB_ZrodloDanychZRDType(string val)
        {
            string wartosc = "";

            switch (val)
            {
                case "1":
                    wartosc = "ZRD1";
                    break;
                case "2":
                    wartosc = "ZRD2";
                    break;
                case "3":
                    wartosc = "ZRD3";
                    break;
                case "4":
                    wartosc = "ZRD4";
                    break;
                case "5":
                    wartosc = "ZRD5";
                    break;
                case "6":
                    wartosc = "ZRD6";
                    break;
                case "7":
                    wartosc = "ZRD7";
                    break;
                case "8":
                    wartosc = "ZRD8";
                    break;
                case "9":
                    wartosc = "ZRD9";
                    break;
            }

            return wartosc;
        }

        public static string EGB_BladPolozeniaWzgledemOsnowyType(string val)
        {
            string wartosc = "";

            switch (val)
            {
                case "1":
                    wartosc = "0_00_0_10";
                    break;
                case "2":
                    wartosc = "0_11_0_30";
                    break;
                case "3":
                    wartosc = "0_31_0_60";
                    break;
                case "4":
                    wartosc = "0_61_1_50";
                    break;
                case "5":
                    wartosc = "1_51_3_00";
                    break;
                case "6":
                    wartosc = "powyzej_3_00";
                    break;
            }

            return wartosc;
        }
    }
}
