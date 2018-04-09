using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using OfficeOpenXml;
using System.Drawing;
using GML_EC.Properties;

namespace GML_EC
{
    public partial class FrmMain : Form
    {
        // struktura do przzechowywania danych o błedach dla danego obiektu
        private struct CsvError
        {
            public string SzczegolyWeryfikacji;
            public string WynikWeryfikacji;
            public string LokalnyId;
            public string Opis;
            public string Linia;
        }


        private readonly BackgroundWorker _bwReadGml = new BackgroundWorker();     // definicja wątku wczytywanie pliku GML
        private readonly BackgroundWorker _bwSaveGmlData = new BackgroundWorker(); // definicja wątku zapisywania danyc z pliku GML
        private readonly BackgroundWorker _bwReadCsv = new BackgroundWorker();     // definicja wątku czytania pliku z błędami CSV
        private readonly BackgroundWorker _bwSaveErrors = new BackgroundWorker();  // definicja wątku zapisywania błedów 

        private string _outputDirectory = ""; // zmienna określająca katalog wyjściowy dla plików GML
        private string _errorDirectory = "";  // zmienna określająca katalog dla plików błędów

        private string _gmlFile = "";         // zmienna okreslajaca nazwę badanego pliku GML

        public FrmMain()
        {

            InitializeComponent();

            _bwReadGml.WorkerReportsProgress = true;
            _bwReadGml.WorkerSupportsCancellation = true;
            _bwReadGml.DoWork += BW_ReadGMLDoWork;
            _bwReadGml.ProgressChanged += BW_ReadGMLProgressChanged;
            _bwReadGml.RunWorkerCompleted += BW_ReadGMLRunWorkerCompleted;

            _bwSaveGmlData.WorkerReportsProgress = true;
            _bwSaveGmlData.WorkerSupportsCancellation = true;
            _bwSaveGmlData.DoWork += BW_SaveGMLDataDoWork;
            _bwSaveGmlData.ProgressChanged += BW_SaveGMLDataProgressChanged;
            _bwSaveGmlData.RunWorkerCompleted += BW_SaveGMLDataRunWorkerCompleted;

            _bwReadCsv.WorkerReportsProgress = true;
            _bwReadCsv.WorkerSupportsCancellation = true;
            _bwReadCsv.DoWork += BW_ReadCSVDoWork;
            _bwReadCsv.ProgressChanged += BW_ReadCSVProgressChanged;
            _bwReadCsv.RunWorkerCompleted += BW_ReadCSVRunWorkerCompleted;

            _bwSaveErrors.WorkerReportsProgress = true;
            _bwSaveErrors.WorkerSupportsCancellation = true;
            _bwSaveErrors.DoWork += BW_bwSaveErrorsDoWork;
            _bwSaveErrors.ProgressChanged += BW_bwSaveErrorsProgressChanged;
            _bwSaveErrors.RunWorkerCompleted += BW_bwSaveErrorsRunWorkerCompleted;

            btnOpenGML.Enabled = true;
            btnReadGML.Enabled = false;
            btnSaveGMLData.Enabled = false;

            btnOpenCSV.Enabled = true;
            btnReadCSV.Enabled = false;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

            // ====================================================================================
            // TABELE GML
            // ====================================================================================

            // BT_Dokument
            DataTable btDokument = new DataTable("BT_Dokument");
            btDokument.Columns.Add("lokalnyId", typeof(string));
            btDokument.Columns.Add("przestrzenNazw", typeof(string));
            btDokument.Columns.Add("nazwaTworcyDokumentu", typeof(string));
            btDokument.Columns.Add("rodzajDokumentu", typeof(string));
            btDokument.Columns.Add("sygnaturaDokumentu", typeof(string));
            dataSetGML.Tables.Add(btDokument);

            // EGB_Adres
            DataTable egbAdres = new DataTable("EGB_Adres");
            egbAdres.Columns.Add("lokalnyId", typeof(string));
            egbAdres.Columns.Add("przestrzenNazw", typeof(string));
            egbAdres.Columns.Add("miejscowosc", typeof(string));
            egbAdres.Columns.Add("nrLokalu", typeof(string));
            egbAdres.Columns.Add("numerPorzadkowy", typeof(string));
            egbAdres.Columns.Add("ulica", typeof(string));
            dataSetGML.Tables.Add(egbAdres);

            // EGB_ArkuszEwidencyjny
            DataTable egbArkuszEwidencyjny = new DataTable("EGB_ArkuszEwidencyjny");
            egbArkuszEwidencyjny.Columns.Add("lokalnyId", typeof(string));
            egbArkuszEwidencyjny.Columns.Add("przestrzenNazw", typeof(string));
            egbArkuszEwidencyjny.Columns.Add("idArkusza", typeof(string));
            dataSetGML.Tables.Add(egbArkuszEwidencyjny);

            // EGB_BlokBudynku
            DataTable egbBlokBudynku = new DataTable("EGB_BlokBudynku");
            egbBlokBudynku.Columns.Add("lokalnyId", typeof(string));
            egbBlokBudynku.Columns.Add("przestrzenNazw", typeof(string));
            egbBlokBudynku.Columns.Add("rodzajBloku", typeof(string));
            egbBlokBudynku.Columns.Add("budynekZWyodrebnionymBlokiemBudynku", typeof(string));
            dataSetGML.Tables.Add(egbBlokBudynku);

            // EGB_Budynek
            DataTable egbBudynek = new DataTable("EGB_Budynek");
            egbBudynek.Columns.Add("lokalnyId", typeof(string));
            egbBudynek.Columns.Add("przestrzenNazw", typeof(string));
            egbBudynek.Columns.Add("idBudynku", typeof(string));
            dataSetGML.Tables.Add(egbBudynek);

            // EGB_DzialkaEwidencyjna
            DataTable egbDzialkaEwidencyjna = new DataTable("EGB_DzialkaEwidencyjna");
            egbDzialkaEwidencyjna.Columns.Add("lokalnyId", typeof(string));
            egbDzialkaEwidencyjna.Columns.Add("przestrzenNazw", typeof(string));
            egbDzialkaEwidencyjna.Columns.Add("idDzialki", typeof(string));
            dataSetGML.Tables.Add(egbDzialkaEwidencyjna);

            // EGB_Dzierzawa
            DataTable egbDzierzawa = new DataTable("EGB_Dzierzawa");
            egbDzierzawa.Columns.Add("lokalnyId", typeof(string));
            egbDzierzawa.Columns.Add("przestrzenNazw", typeof(string));
            egbDzierzawa.Columns.Add("idDzierzawy", typeof(string));
            egbDzierzawa.Columns.Add("budynekObjetyDzierzawa", typeof(string));
            egbDzierzawa.Columns.Add("lokalObjetyDzierzawa", typeof(string));
            egbDzierzawa.Columns.Add("dzialkaObjetaDzierzawa", typeof(string));
            egbDzierzawa.Columns.Add("czescDzialkiObjetaDzierzawa", typeof(string));
            dataSetGML.Tables.Add(egbDzierzawa);

            // EGB_Instytucja
            DataTable egbInstytucja = new DataTable("EGB_Instytucja");
            egbInstytucja.Columns.Add("lokalnyId", typeof(string));
            egbInstytucja.Columns.Add("przestrzenNazw", typeof(string));
            egbInstytucja.Columns.Add("status", typeof(string));
            egbInstytucja.Columns.Add("nazwaPelna", typeof(string));
            dataSetGML.Tables.Add(egbInstytucja);

            // EGB_JednostkaEwidencyjna
            DataTable egbJednostkaEwidencyjna = new DataTable("EGB_JednostkaEwidencyjna");
            egbJednostkaEwidencyjna.Columns.Add("lokalnyId", typeof(string));
            egbJednostkaEwidencyjna.Columns.Add("przestrzenNazw", typeof(string));
            egbJednostkaEwidencyjna.Columns.Add("idJednostkiEwid", typeof(string));
            egbJednostkaEwidencyjna.Columns.Add("nazwaWlasna", typeof(string));
            dataSetGML.Tables.Add(egbJednostkaEwidencyjna);

            // EGB_JednostkaRejestrowaBudynkow
            DataTable egbJednostkaRejestrowaBudynkow = new DataTable("EGB_JednostkaRejestrowaBudynkow");
            egbJednostkaRejestrowaBudynkow.Columns.Add("lokalnyId", typeof(string));
            egbJednostkaRejestrowaBudynkow.Columns.Add("przestrzenNazw", typeof(string));
            egbJednostkaRejestrowaBudynkow.Columns.Add("idJednostkiRejestrowej", typeof(string));
            dataSetGML.Tables.Add(egbJednostkaRejestrowaBudynkow);

            // EGB_JednostkaRejestrowaGruntow
            DataTable egbJednostkaRejestrowaGruntow = new DataTable("EGB_JednostkaRejestrowaGruntow");
            egbJednostkaRejestrowaGruntow.Columns.Add("lokalnyId", typeof(string));
            egbJednostkaRejestrowaGruntow.Columns.Add("przestrzenNazw", typeof(string));
            egbJednostkaRejestrowaGruntow.Columns.Add("idJednostkiRejestrowej", typeof(string));
            dataSetGML.Tables.Add(egbJednostkaRejestrowaGruntow);

            // EGB_JednostkaRejestrowaLokali
            DataTable egbJednostkaRejestrowaLokali = new DataTable("EGB_JednostkaRejestrowaLokali");
            egbJednostkaRejestrowaLokali.Columns.Add("lokalnyId", typeof(string));
            egbJednostkaRejestrowaLokali.Columns.Add("przestrzenNazw", typeof(string));
            egbJednostkaRejestrowaLokali.Columns.Add("idJednostkiRejestrowej", typeof(string));
            dataSetGML.Tables.Add(egbJednostkaRejestrowaLokali);

            // EGB_Klasouzytek
            DataTable egbKlasouzytek = new DataTable("EGB_Klasouzytek");
            egbKlasouzytek.Columns.Add("lokalnyId", typeof(string));
            egbKlasouzytek.Columns.Add("przestrzenNazw", typeof(string));
            egbKlasouzytek.Columns.Add("OFU", typeof(string));
            egbKlasouzytek.Columns.Add("OZU", typeof(string));
            egbKlasouzytek.Columns.Add("OZK", typeof(string));
            dataSetGML.Tables.Add(egbKlasouzytek);

            // EGB_KonturKlasyfikacyjny
            DataTable egbKonturKlasyfikacyjny = new DataTable("EGB_KonturKlasyfikacyjny");
            egbKonturKlasyfikacyjny.Columns.Add("lokalnyId", typeof(string));
            egbKonturKlasyfikacyjny.Columns.Add("przestrzenNazw", typeof(string));
            egbKonturKlasyfikacyjny.Columns.Add("idKonturu", typeof(string));
            egbKonturKlasyfikacyjny.Columns.Add("OZU", typeof(string));
            egbKonturKlasyfikacyjny.Columns.Add("OZK", typeof(string));
            dataSetGML.Tables.Add(egbKonturKlasyfikacyjny);

            // EGB_KonturUzytkuGruntowego
            DataTable egbKonturUzytkuGruntowego = new DataTable("EGB_KonturUzytkuGruntowego");
            egbKonturUzytkuGruntowego.Columns.Add("lokalnyId", typeof(string));
            egbKonturUzytkuGruntowego.Columns.Add("przestrzenNazw", typeof(string));
            egbKonturUzytkuGruntowego.Columns.Add("idUzytku", typeof(string));
            egbKonturUzytkuGruntowego.Columns.Add("OFU", typeof(string));
            dataSetGML.Tables.Add(egbKonturUzytkuGruntowego);

            // EGB_LokalSamodzielny
            DataTable egbLokalSamodzielny = new DataTable("EGB_LokalSamodzielny");
            egbLokalSamodzielny.Columns.Add("lokalnyId", typeof(string));
            egbLokalSamodzielny.Columns.Add("przestrzenNazw", typeof(string));
            egbLokalSamodzielny.Columns.Add("idLokalu", typeof(string));
            dataSetGML.Tables.Add(egbLokalSamodzielny);

            // EGB_Malzenstwo
            DataTable egbMalzenstwo = new DataTable("EGB_Malzenstwo");
            egbMalzenstwo.Columns.Add("lokalnyId", typeof(string));
            egbMalzenstwo.Columns.Add("przestrzenNazw", typeof(string));
            egbMalzenstwo.Columns.Add("status", typeof(string));
            egbMalzenstwo.Columns.Add("osobaFizyczna2", typeof(string));
            egbMalzenstwo.Columns.Add("osobaFizyczna3", typeof(string));
            dataSetGML.Tables.Add(egbMalzenstwo);

            // EGB_ObiektTrwaleZwiazanyZBudynkiem
            DataTable egbObiektTrwaleZwiazanyZBudynkiem = new DataTable("EGB_ObiektTrwaleZwiazanyZBudynkiem");
            egbObiektTrwaleZwiazanyZBudynkiem.Columns.Add("lokalnyId", typeof(string));
            egbObiektTrwaleZwiazanyZBudynkiem.Columns.Add("przestrzenNazw", typeof(string));
            egbObiektTrwaleZwiazanyZBudynkiem.Columns.Add("rodzajObiektuZwiazanegoZBudynkiem", typeof(string));
            egbObiektTrwaleZwiazanyZBudynkiem.Columns.Add("budynekZElementamiTrwaleZwiazanymi", typeof(string));
            dataSetGML.Tables.Add(egbObiektTrwaleZwiazanyZBudynkiem);

            // EGB_ObrebEwidencyjny
            DataTable egbObrebEwidencyjny = new DataTable("EGB_ObrebEwidencyjny");
            egbObrebEwidencyjny.Columns.Add("lokalnyId", typeof(string));
            egbObrebEwidencyjny.Columns.Add("przestrzenNazw", typeof(string));
            egbObrebEwidencyjny.Columns.Add("idObrebu", typeof(string));
            egbObrebEwidencyjny.Columns.Add("nazwaWlasna", typeof(string));
            dataSetGML.Tables.Add(egbObrebEwidencyjny);

            // EGB_OperatTechniczny
            DataTable egbOperatTechniczny = new DataTable("EGB_OperatTechniczny");
            egbOperatTechniczny.Columns.Add("lokalnyId", typeof(string));
            egbOperatTechniczny.Columns.Add("przestrzenNazw", typeof(string));
            egbOperatTechniczny.Columns.Add("nazwaTworcy", typeof(string));
            egbOperatTechniczny.Columns.Add("identyfikatorOperatuWgPZGIK", typeof(string));
            dataSetGML.Tables.Add(egbOperatTechniczny);

            // EGB_OsobaFizyczna
            DataTable egbOsobaFizyczna = new DataTable("EGB_OsobaFizyczna");
            egbOsobaFizyczna.Columns.Add("lokalnyId", typeof(string));
            egbOsobaFizyczna.Columns.Add("przestrzenNazw", typeof(string));
            egbOsobaFizyczna.Columns.Add("pierwszeImie", typeof(string));
            egbOsobaFizyczna.Columns.Add("pierwszyCzlonNazwiska", typeof(string));
            egbOsobaFizyczna.Columns.Add("plec", typeof(string));
            egbOsobaFizyczna.Columns.Add("pesel", typeof(string));
            egbOsobaFizyczna.Columns.Add("drugiCzlonNazwiska", typeof(string));
            egbOsobaFizyczna.Columns.Add("drugieImie", typeof(string));
            egbOsobaFizyczna.Columns.Add("imieMatki", typeof(string));
            egbOsobaFizyczna.Columns.Add("imieOjca", typeof(string));
            dataSetGML.Tables.Add(egbOsobaFizyczna);

            // EGB_PodmiotGrupowy
            DataTable egbPodmiotGrupowy = new DataTable("EGB_PodmiotGrupowy");
            egbPodmiotGrupowy.Columns.Add("lokalnyId", typeof(string));
            egbPodmiotGrupowy.Columns.Add("przestrzenNazw", typeof(string));
            egbPodmiotGrupowy.Columns.Add("status", typeof(string));
            egbPodmiotGrupowy.Columns.Add("nazwaPelna", typeof(string));
            dataSetGML.Tables.Add(egbPodmiotGrupowy);

            // EGB_PomieszczeniePrzynalezneDoLokalu
            DataTable egbPomieszczeniePrzynalezneDoLokalu = new DataTable("EGB_PomieszczeniePrzynalezneDoLokalu");
            egbPomieszczeniePrzynalezneDoLokalu.Columns.Add("lokalnyId", typeof(string));
            egbPomieszczeniePrzynalezneDoLokalu.Columns.Add("przestrzenNazw", typeof(string));
            egbPomieszczeniePrzynalezneDoLokalu.Columns.Add("rodzajPomieszczeniaPrzynaleznego", typeof(string));
            egbPomieszczeniePrzynalezneDoLokalu.Columns.Add("lokalizacjaPomieszczeniaPrzynaleznego", typeof(string));
            dataSetGML.Tables.Add(egbPomieszczeniePrzynalezneDoLokalu);

            // EGB_PunktGraniczny
            DataTable egbPunktGraniczny = new DataTable("EGB_PunktGraniczny");
            egbPunktGraniczny.Columns.Add("lokalnyId", typeof(string));
            egbPunktGraniczny.Columns.Add("przestrzenNazw", typeof(string));
            egbPunktGraniczny.Columns.Add("posX", typeof(string));
            egbPunktGraniczny.Columns.Add("posY", typeof(string));
            egbPunktGraniczny.Columns.Add("idPunktu", typeof(string));
            egbPunktGraniczny.Columns.Add("dodatkoweInformacje", typeof(string));
            egbPunktGraniczny.Columns.Add("oznWMaterialeZrodlowym", typeof(string));
            egbPunktGraniczny.Columns.Add("zrodloDanychZRD", typeof(string));
            egbPunktGraniczny.Columns.Add("bladPolozeniaWzgledemOsnowy", typeof(string));
            egbPunktGraniczny.Columns.Add("kodStabilizacji", typeof(string));
            dataSetGML.Tables.Add(egbPunktGraniczny);

            // EGB_UdzialDzierzawy
            DataTable egbUdzialDzierzawy = new DataTable("EGB_UdzialDzierzawy");
            egbUdzialDzierzawy.Columns.Add("lokalnyId", typeof(string));
            egbUdzialDzierzawy.Columns.Add("przestrzenNazw", typeof(string));
            egbUdzialDzierzawy.Columns.Add("udzial", typeof(string));
            egbUdzialDzierzawy.Columns.Add("przedmiotUdzialuDzierzawy", typeof(string));
            dataSetGML.Tables.Add(egbUdzialDzierzawy);

            // EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST
            DataTable egbUdzialGospodarowaniaNieruchomosciaSpLubJst =
                new DataTable("EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST");
            egbUdzialGospodarowaniaNieruchomosciaSpLubJst.Columns.Add("lokalnyId", typeof(string));
            egbUdzialGospodarowaniaNieruchomosciaSpLubJst.Columns.Add("przestrzenNazw", typeof(string));
            egbUdzialGospodarowaniaNieruchomosciaSpLubJst.Columns.Add("rodzajUprawnien", typeof(string));
            egbUdzialGospodarowaniaNieruchomosciaSpLubJst.Columns.Add("licznikUlamkaOkreslajacegoWartoscUdzialu",
                typeof(string));
            egbUdzialGospodarowaniaNieruchomosciaSpLubJst.Columns.Add("mianownikUlamkaOkreslajacegoWartoscUdzialu",
                typeof(string));
            egbUdzialGospodarowaniaNieruchomosciaSpLubJst.Columns.Add("podgrupaRej", typeof(string));
            egbUdzialGospodarowaniaNieruchomosciaSpLubJst.Columns.Add("przedmiotUdzialuGZ1", typeof(string));
            egbUdzialGospodarowaniaNieruchomosciaSpLubJst.Columns.Add("idPrzedmiotUdzialuGZ1", typeof(string));
            dataSetGML.Tables.Add(egbUdzialGospodarowaniaNieruchomosciaSpLubJst);

            // EGB_UdzialWeWladaniuNieruchomosciaSPLubJST
            DataTable egbUdzialWeWladaniuNieruchomosciaSpLubJst =
                new DataTable("EGB_UdzialWeWladaniuNieruchomosciaSPLubJST");
            egbUdzialWeWladaniuNieruchomosciaSpLubJst.Columns.Add("lokalnyId", typeof(string));
            egbUdzialWeWladaniuNieruchomosciaSpLubJst.Columns.Add("przestrzenNazw", typeof(string));
            egbUdzialWeWladaniuNieruchomosciaSpLubJst.Columns.Add("rodzajWladania", typeof(string));
            egbUdzialWeWladaniuNieruchomosciaSpLubJst.Columns.Add("licznikUlamkaOkreslajacegoWartoscUdzialu",
                typeof(string));
            egbUdzialWeWladaniuNieruchomosciaSpLubJst.Columns.Add("mianownikUlamkaOkreslajacegoWartoscUdzialu",
                typeof(string));
            egbUdzialWeWladaniuNieruchomosciaSpLubJst.Columns.Add("podgrupaRej", typeof(string));
            egbUdzialWeWladaniuNieruchomosciaSpLubJst.Columns.Add("przedmiotUdzialuWladania", typeof(string));
            egbUdzialWeWladaniuNieruchomosciaSpLubJst.Columns.Add("idPrzedmiotUdzialuWladania", typeof(string));
            dataSetGML.Tables.Add(egbUdzialWeWladaniuNieruchomosciaSpLubJst);

            // EGB_UdzialWlasnosci
            DataTable egbUdzialWlasnosci = new DataTable("EGB_UdzialWlasnosci");
            egbUdzialWlasnosci.Columns.Add("lokalnyId", typeof(string));
            egbUdzialWlasnosci.Columns.Add("przestrzenNazw", typeof(string));
            egbUdzialWlasnosci.Columns.Add("rodzajPrawa", typeof(string));
            egbUdzialWlasnosci.Columns.Add("licznikUlamkaOkreslajacegoWartoscUdzialu", typeof(string));
            egbUdzialWlasnosci.Columns.Add("mianownikUlamkaOkreslajacegoWartoscUdzialu", typeof(string));
            egbUdzialWlasnosci.Columns.Add("podgrupaRej", typeof(string));
            egbUdzialWlasnosci.Columns.Add("przedmiotUdzialuWlasnosci", typeof(string));
            egbUdzialWlasnosci.Columns.Add("idPrzedmiotUdzialuWlasnosci", typeof(string));
            dataSetGML.Tables.Add(egbUdzialWlasnosci);

            // EGB_ZarzadSpolkiWspolnotyGruntowej
            DataTable egbZarzadSpolkiWspolnotyGruntowej = new DataTable("EGB_ZarzadSpolkiWspolnotyGruntowej");
            egbZarzadSpolkiWspolnotyGruntowej.Columns.Add("lokalnyId", typeof(string));
            egbZarzadSpolkiWspolnotyGruntowej.Columns.Add("przestrzenNazw", typeof(string));
            egbZarzadSpolkiWspolnotyGruntowej.Columns.Add("nazwaSpolkiPowolanejDoZarzadzaniaWspolnotaGruntowa",
                typeof(string));
            egbZarzadSpolkiWspolnotyGruntowej.Columns.Add("wspolnotaGruntowa", typeof(string));
            dataSetGML.Tables.Add(egbZarzadSpolkiWspolnotyGruntowej);

            // EGB_Zmiana
            DataTable egbZmiana = new DataTable("EGB_Zmiana");
            egbZmiana.Columns.Add("lokalnyId", typeof(string));
            egbZmiana.Columns.Add("przestrzenNazw", typeof(string));
            egbZmiana.Columns.Add("nrZmiany", typeof(string));
            dataSetGML.Tables.Add(egbZmiana);

            // ====================================================================================
            // TABELE POŚREDNIE
            // ====================================================================================

            //// klasouzytekWGranicachDzialki
            //DataTable klasouzytekWGranicachDzialki = new DataTable("klasouzytekWGranicachDzialki");
            //klasouzytekWGranicachDzialki.Columns.Add("idDzialka", typeof(string));
            //klasouzytekWGranicachDzialki.Columns.Add("idKlasouzytek", typeof(string));
            //dataSetGML.Tables.Add(klasouzytekWGranicachDzialki);

            // ====================================================================================
            // RAPORT CSV
            // ====================================================================================

            DataTable raport = new DataTable("Raport");
            raport.Columns.Add("typWeryfikacji", typeof(string));
            raport.Columns.Add("rodzajWeryfikacji", typeof(string));
            raport.Columns.Add("szczegolyWeryfikacji", typeof(string));
            raport.Columns.Add("rodzajObiektu", typeof(string));
            raport.Columns.Add("statusWeryfikacji", typeof(string));
            raport.Columns.Add("wynikWeryfikacji", typeof(string));
            raport.Columns.Add("idObiektu", typeof(string));
            raport.Columns.Add("lokalnyId", typeof(string));
            raport.Columns.Add("przestrzenNazw", typeof(string));
            raport.Columns.Add("opis", typeof(string));
            raport.Columns.Add("linia", typeof(string));
            dataSetCSV.Tables.Add(raport);

            // ====================================================================================

        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            // ====================================================================================
            // zabezpiczenie przez XP, który nie potrafi podłączyć ikony
            try
            {
                Icon = Resources.gml;
            }
            catch (Exception)
            {
                // ignored
            }
            // ====================================================================================

            Text = Application.ProductName + @" " + Application.ProductVersion;

            stbMainStatus.Text = @"Gotowy";

            // ====================================================================================
            // obliczanie czasu licencji czasowej
            TimeSpan licensePeriod = new DateTime(2019, 02, 28, 23, 59, 59) - DateTime.Now;

            //lblLicencja.Text = Resources.lblLicencja_Czasowa + Math.Round(licensePeriod.TotalHours / 24, 2);
            //lblLicencja.ForeColor = Color.Red;
            //lblClient.Text = Resources.lblClient_Puck;
            //pictureBoxClient.Image = Resources.pucki;

            lblLicencja.ForeColor = SystemColors.ControlText;
            lblLicencja.Text = @"Licencja bezterminowa";
            lblClient.Text = @"Wersja demonstracyjna";


            // ====================================================================================

            if (licensePeriod.Days <= 0)
            {
                MessageBox.Show(@"Okres testowy minął: " + licensePeriod.Days, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Application.Exit();
            }
        }

        //  ---------------------------------------------------------------------------------------
        //  obsluga przycisku wybierania pliku GML
        private void ButtonOpenGML_Click(object sender, EventArgs e)
        {
            dlgOpen.FileName = "*.gml";
            dlgOpen.Filter = @"GML (*.gml)|*.gml";

            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                txtInputGML.Text = dlgOpen.FileName;

                btnOpenGML.Enabled = true;
                btnReadGML.Enabled = true;
                btnSaveGMLData.Enabled = false;

                // wyczysc tabele GML
                ClearGmlData();

                // ustaw folder roboczy
                string tempDirectory = dlgOpen.FileName.Substring(0, dlgOpen.FileName.LastIndexOf("\\", StringComparison.Ordinal) + 1);

                string workingFile = dlgOpen.FileName.Substring(dlgOpen.FileName.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                workingFile = workingFile.Substring(0, workingFile.LastIndexOf(".", StringComparison.Ordinal));


                _outputDirectory = tempDirectory + workingFile + "\\gml_output\\";
                _errorDirectory = tempDirectory + workingFile + "\\gml_error\\";
                _gmlFile = workingFile;

                // ================================================================================
                // jeśli w katalogu z plikiem GML jest tylko jeden raport dotyczący tego pliku jest
                // on automatycznie wybierany o wczytywany
                string[] csvFiles = Directory.GetFiles(tempDirectory, "*" + workingFile + "*.csv");

                if (csvFiles.Length == 1)
                {
                    txtInputCSV.Text = csvFiles[0];
                    btnReadCSV.Enabled = true;
                }
                else
                {
                    txtInputCSV.Text = @"Wskaż plik CSV z zestawieniem błędów";
                    btnReadCSV.Enabled = false;
                }
                // ================================================================================

                stbMainStatus.Text = @"Gotowy";
                UseWaitCursor = false;
            }
        }

        //  ---------------------------------------------------------------------------------------
        //  obsluga przycisku wczytywania pliku GML 
        private void ButtonReadGML_Click(object sender, EventArgs e)
        {
            if ((_bwReadCsv.IsBusy != true) && (_bwReadGml.IsBusy != true))
            {
                btnOpenGML.Enabled = false;
                btnReadGML.Enabled = false;
                btnSaveGMLData.Enabled = false;

                UseWaitCursor = true;

                ClearGmlData();              // wyczysc tabele GML

                _bwReadGml.RunWorkerAsync(); // uruchom wątek wczytywania pliku GML
            }
        }

        //  ---------------------------------------------------------------------------------------
        //  funkcja wątku głownego wczytujaca dane z pliku GML do bazy danych 
        private void BW_ReadGMLDoWork(object sender, DoWorkEventArgs e)
        {
            stbMainStatus.Text = @"Zliczanie liczby obiektów...";

            int elementCount = 0, elementCounter = 0; // zmienne do obsługi paska postępu

            XmlTextReader gmlReader = null;

            // ====================================================================================
            // zliczanie elementów typu "featureMember" w pliku GML
            try
            {
                gmlReader = new XmlTextReader(txtInputGML.Text);

                while (gmlReader.Read())
                {
                    if (gmlReader.NodeType == XmlNodeType.Element && gmlReader.Name == "gml:featureMember") elementCount++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                gmlReader?.Close();
            }
            // ====================================================================================

            stbMainStatus.Text = @"Wczytywanie pliku GML [" + elementCount + @"]";

            // ====================================================================================
            // wczytywanie pliku GML do stryuktur programu

            gmlReader = new XmlTextReader(txtInputGML.Text);

            while (gmlReader.Read())
            {

                if (gmlReader.NodeType == XmlNodeType.Element)
                {

                    switch (gmlReader.Name)
                    {
                        case "gml:featureMember":

                            elementCounter++;

                            // Report progress as a percentage of the total task. 
                            int percentComplete = (int)(elementCounter / (float)elementCount * 100);
                            _bwReadGml.ReportProgress(percentComplete);

                            break;

                        case "bt:BT_Dokument":

                            DataRow rowBtDokument = dataSetGML.Tables["BT_Dokument"].NewRow();

                            XmlDocument docBtDokument = new XmlDocument();
                            docBtDokument.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmBtDokument = docBtDokument.DocumentElement;

                            foreach (XmlNode node in elmBtDokument.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "bt:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowBtDokument["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowBtDokument["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "bt:nazwaTworcyDokumentu":
                                        rowBtDokument["nazwaTworcyDokumentu"] = node.InnerText;
                                        break;

                                    case "bt:rodzajDokumentu":
                                        rowBtDokument["rodzajDokumentu"] = node.InnerText;
                                        break;

                                    case "bt:sygnaturaDokumentu":
                                        rowBtDokument["sygnaturaDokumentu"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["BT_Dokument"].Rows.Add(rowBtDokument);

                            break; // BT_Dokument

                        case "egb:EGB_Adres":

                            DataRow rowEgbAdres = dataSetGML.Tables["EGB_Adres"].NewRow();

                            XmlDocument docEgbAdres = new XmlDocument();
                            docEgbAdres.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbAdres = docEgbAdres.DocumentElement;

                            foreach (XmlNode node in elmEgbAdres.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbAdres["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbAdres["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:miejscowosc":
                                        rowEgbAdres["miejscowosc"] = node.InnerText;
                                        break;

                                    case "egb:nrLokalu":
                                        rowEgbAdres["nrLokalu"] = node.InnerText;
                                        break;

                                    case "egb:numerPorzadkowy":
                                        rowEgbAdres["numerPorzadkowy"] = node.InnerText;
                                        break;
                                    case "egb:ulica":
                                        rowEgbAdres["ulica"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_Adres"].Rows.Add(rowEgbAdres);

                            break; // EGB_Adres

                        case "egb:EGB_ArkuszEwidencyjny":

                            DataRow rowEgbArkuszEwidencyjny = dataSetGML.Tables["EGB_ArkuszEwidencyjny"].NewRow();

                            XmlDocument docEgbArkuszEwidencyjny = new XmlDocument();
                            docEgbArkuszEwidencyjny.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbArkuszEwidencyjny = docEgbArkuszEwidencyjny.DocumentElement;

                            foreach (XmlNode node in elmEgbArkuszEwidencyjny.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbArkuszEwidencyjny["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbArkuszEwidencyjny["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:idArkusza":
                                        rowEgbArkuszEwidencyjny["idArkusza"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_ArkuszEwidencyjny"].Rows.Add(rowEgbArkuszEwidencyjny);

                            break; // EGB_ArkuszEwidencyjny

                        case "egb:EGB_BlokBudynku":

                            DataRow rowEgbBlokBudynku = dataSetGML.Tables["EGB_BlokBudynku"].NewRow();

                            XmlDocument docEgbBlokBudynku = new XmlDocument();
                            docEgbBlokBudynku.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbBlokBudynku = docEgbBlokBudynku.DocumentElement;

                            foreach (XmlNode node in elmEgbBlokBudynku.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbBlokBudynku["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbBlokBudynku["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:rodzajBloku":
                                        rowEgbBlokBudynku["rodzajBloku"] = node.InnerText;
                                        break;

                                    case "egb:budynekZWyodrebnionymBlokiemBudynku":
                                        string idBudynku = node.Attributes["xlink:href"].InnerText;
                                        rowEgbBlokBudynku["budynekZWyodrebnionymBlokiemBudynku"] = idBudynku.Substring(idBudynku.LastIndexOf(':') + 1);
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_BlokBudynku"].Rows.Add(rowEgbBlokBudynku);

                            break; // egb:EGB_BlokBudynku

                        case "egb:EGB_Budynek":

                            DataRow rowEgbBudynek = dataSetGML.Tables["EGB_Budynek"].NewRow();

                            XmlDocument docEgbBudynek = new XmlDocument();
                            docEgbBudynek.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbBudynek = docEgbBudynek.DocumentElement;

                            foreach (XmlNode node in elmEgbBudynek.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbBudynek["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbBudynek["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:idBudynku":
                                        rowEgbBudynek["idBudynku"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_Budynek"].Rows.Add(rowEgbBudynek);

                            break; // egb:EGB_Budynek

                        case "egb:EGB_DzialkaEwidencyjna":

                            DataRow rowEgbDzialkaEwidencyjna = dataSetGML.Tables["EGB_DzialkaEwidencyjna"].NewRow();

                            XmlDocument docEgbDzialkaEwidencyjna = new XmlDocument();
                            docEgbDzialkaEwidencyjna.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbDzialkaEwidencyjna = docEgbDzialkaEwidencyjna.DocumentElement;

                            foreach (XmlNode node in elmEgbDzialkaEwidencyjna.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbDzialkaEwidencyjna["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbDzialkaEwidencyjna["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:idDzialki":
                                        rowEgbDzialkaEwidencyjna["idDzialki"] = node.InnerText;
                                        break;

                                }
                            }

                            dataSetGML.Tables["EGB_DzialkaEwidencyjna"].Rows.Add(rowEgbDzialkaEwidencyjna);

                            break; // egb:EGB_DzialkaEwidencyjna

                        case "egb:EGB_Dzierzawa":

                            DataRow rowEgbDzierzawa = dataSetGML.Tables["EGB_Dzierzawa"].NewRow();

                            XmlDocument docEgbDzierzawa = new XmlDocument();
                            docEgbDzierzawa.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbDzierzawa = docEgbDzierzawa.DocumentElement;

                            foreach (XmlNode node in elmEgbDzierzawa.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbDzierzawa["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbDzierzawa["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:idDzierzawy":
                                        rowEgbDzierzawa["idDzierzawy"] = node.InnerText;
                                        break;

                                    case "egb:budynekObjetyDzierzawa":
                                        string budynekObjetyDzierzawa = node.Attributes["xlink:href"].InnerText;
                                        rowEgbDzierzawa["budynekObjetyDzierzawa"] = budynekObjetyDzierzawa.Substring(budynekObjetyDzierzawa.LastIndexOf(':') + 1);
                                        break;

                                    case "egb:lokalObjetyDzierzawa":
                                        string lokalObjetyDzierzawa = node.Attributes["xlink:href"].InnerText;
                                        rowEgbDzierzawa["lokalObjetyDzierzawa"] = lokalObjetyDzierzawa.Substring(lokalObjetyDzierzawa.LastIndexOf(':') + 1);
                                        break;

                                    case "egb:dzialkaObjetaDzierzawa":
                                        string dzialkaObjetaDzierzawa = node.Attributes["xlink:href"].InnerText;
                                        rowEgbDzierzawa["dzialkaObjetaDzierzawa"] = dzialkaObjetaDzierzawa.Substring(dzialkaObjetaDzierzawa.LastIndexOf(':') + 1);
                                        break;

                                    case "egb:czescDzialkiObjetaDzierzawa":
                                        string czescDzialkiObjetaDzierzawa = node.Attributes["xlink:href"].InnerText;
                                        rowEgbDzierzawa["czescDzialkiObjetaDzierzawa"] = czescDzialkiObjetaDzierzawa.Substring(czescDzialkiObjetaDzierzawa.LastIndexOf(':') + 1);
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_Dzierzawa"].Rows.Add(rowEgbDzierzawa);

                            break; // egb:EGB_Dzierzawa

                        case "egb:EGB_Instytucja":

                            DataRow rowEgbInstytucja = dataSetGML.Tables["EGB_Instytucja"].NewRow();

                            XmlDocument docEgbInstytucja = new XmlDocument();
                            docEgbInstytucja.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbInstytucja = docEgbInstytucja.DocumentElement;

                            foreach (XmlNode node in elmEgbInstytucja.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbInstytucja["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbInstytucja["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:status":
                                        rowEgbInstytucja["status"] = node.InnerText;
                                        break;

                                    case "egb:nazwaPelna":
                                        rowEgbInstytucja["nazwaPelna"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_Instytucja"].Rows.Add(rowEgbInstytucja);

                            break; // egb:EGB_Instytucja

                        case "egb:EGB_JednostkaEwidencyjna":

                            DataRow rowEgbJednostkaEwidencyjna = dataSetGML.Tables["EGB_JednostkaEwidencyjna"].NewRow();

                            XmlDocument docEgbJednostkaEwidencyjna = new XmlDocument();
                            docEgbJednostkaEwidencyjna.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbJednostkaEwidencyjna = docEgbJednostkaEwidencyjna.DocumentElement;

                            foreach (XmlNode node in elmEgbJednostkaEwidencyjna.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbJednostkaEwidencyjna["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbJednostkaEwidencyjna["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:idJednostkiEwid":
                                        rowEgbJednostkaEwidencyjna["idJednostkiEwid"] = node.InnerText;
                                        break;

                                    case "egb:nazwaWlasna":
                                        rowEgbJednostkaEwidencyjna["nazwaWlasna"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_JednostkaEwidencyjna"].Rows.Add(rowEgbJednostkaEwidencyjna);

                            break; // egb:EGB_JednostkaEwidencyjna

                        case "egb:EGB_JednostkaRejestrowaBudynkow":

                            DataRow rowEgbJednostkaRejestrowaBudynkow = dataSetGML.Tables["EGB_JednostkaRejestrowaBudynkow"].NewRow();

                            XmlDocument docEgbJednostkaRejestrowaBudynkow = new XmlDocument();
                            docEgbJednostkaRejestrowaBudynkow.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbJednostkaRejestrowaBudynkow = docEgbJednostkaRejestrowaBudynkow.DocumentElement;

                            foreach (XmlNode node in elmEgbJednostkaRejestrowaBudynkow.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbJednostkaRejestrowaBudynkow["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbJednostkaRejestrowaBudynkow["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:idJednostkiRejestrowej":
                                        rowEgbJednostkaRejestrowaBudynkow["idJednostkiRejestrowej"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_JednostkaRejestrowaBudynkow"].Rows.Add(rowEgbJednostkaRejestrowaBudynkow);

                            break; // egb:EGB_JednostkaRejestrowaBudynkow

                        case "egb:EGB_JednostkaRejestrowaGruntow":

                            DataRow rowEgbJednostkaRejestrowaGruntow = dataSetGML.Tables["EGB_JednostkaRejestrowaGruntow"].NewRow();

                            XmlDocument docEgbJednostkaRejestrowaGruntow = new XmlDocument();
                            docEgbJednostkaRejestrowaGruntow.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbJednostkaRejestrowaGruntow = docEgbJednostkaRejestrowaGruntow.DocumentElement;

                            foreach (XmlNode node in elmEgbJednostkaRejestrowaGruntow.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbJednostkaRejestrowaGruntow["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbJednostkaRejestrowaGruntow["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:idJednostkiRejestrowej":
                                        rowEgbJednostkaRejestrowaGruntow["idJednostkiRejestrowej"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_JednostkaRejestrowaGruntow"].Rows.Add(rowEgbJednostkaRejestrowaGruntow);

                            break; // egb:EGB_JednostkaRejestrowaGruntow

                        case "egb:EGB_JednostkaRejestrowaLokali":

                            DataRow rowEgbJednostkaRejestrowaLokali = dataSetGML.Tables["EGB_JednostkaRejestrowaLokali"].NewRow();

                            XmlDocument docEgbJednostkaRejestrowaLokali = new XmlDocument();
                            docEgbJednostkaRejestrowaLokali.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbJednostkaRejestrowaLokali = docEgbJednostkaRejestrowaLokali.DocumentElement;

                            foreach (XmlNode node in elmEgbJednostkaRejestrowaLokali.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbJednostkaRejestrowaLokali["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbJednostkaRejestrowaLokali["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:idJednostkiRejestrowej":
                                        rowEgbJednostkaRejestrowaLokali["idJednostkiRejestrowej"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_JednostkaRejestrowaLokali"].Rows.Add(rowEgbJednostkaRejestrowaLokali);

                            break; // egb:EGB_JednostkaRejestrowaLokali

                        case "egb:EGB_Klasouzytek":

                            DataRow rowEgbKlasouzytek = dataSetGML.Tables["EGB_Klasouzytek"].NewRow();

                            XmlDocument docEgbKlasouzytek = new XmlDocument();
                            docEgbKlasouzytek.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbKlasouzytek = docEgbKlasouzytek.DocumentElement;

                            foreach (XmlNode node in elmEgbKlasouzytek.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbKlasouzytek["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbKlasouzytek["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:oznaczenieKlasouzytku":
                                        XmlNode egbOznaczenieKlasouzytku = node.FirstChild;

                                        if (egbOznaczenieKlasouzytku.ChildNodes.Count == 1)
                                        {
                                            XmlNode ofu = egbOznaczenieKlasouzytku.FirstChild;
                                            rowEgbKlasouzytek["OFU"] = ofu.InnerText;
                                        }
                                        else if (egbOznaczenieKlasouzytku.ChildNodes.Count == 3)
                                        {
                                            XmlNode ofu = egbOznaczenieKlasouzytku.FirstChild;
                                            rowEgbKlasouzytek["OFU"] = ofu.InnerText;
                                            XmlNode ozu = ofu.NextSibling;
                                            rowEgbKlasouzytek["OZU"] = ozu.InnerText;
                                            XmlNode ozk = ozu.NextSibling;
                                            rowEgbKlasouzytek["OZK"] = ozk.InnerText;
                                        }
                                        else
                                        {
                                            MessageBox.Show(@"Błąd w EGB_Klasouzytek! Zgłoś to autorowi programu.");
                                        }

                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_Klasouzytek"].Rows.Add(rowEgbKlasouzytek);

                            break; // egb:EGB_Klasouzytek

                        case "egb:EGB_KonturKlasyfikacyjny":

                            DataRow rowEgbKonturKlasyfikacyjny = dataSetGML.Tables["EGB_KonturKlasyfikacyjny"].NewRow();

                            XmlDocument docEgbKonturKlasyfikacyjny = new XmlDocument();
                            docEgbKonturKlasyfikacyjny.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbKonturKlasyfikacyjny = docEgbKonturKlasyfikacyjny.DocumentElement;

                            foreach (XmlNode node in elmEgbKonturKlasyfikacyjny.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbKonturKlasyfikacyjny["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbKonturKlasyfikacyjny["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:idKonturu":
                                        rowEgbKonturKlasyfikacyjny["idKonturu"] = node.InnerText;
                                        break;

                                    case "egb:OZU":
                                        rowEgbKonturKlasyfikacyjny["OZU"] = node.InnerText;
                                        break;

                                    case "egb:OZK":
                                        rowEgbKonturKlasyfikacyjny["OZK"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_KonturKlasyfikacyjny"].Rows.Add(rowEgbKonturKlasyfikacyjny);

                            break; // egb:EGB_KonturKlasyfikacyjny

                        case "egb:EGB_KonturUzytkuGruntowego":

                            DataRow rowEgbKonturUzytkuGruntowego = dataSetGML.Tables["EGB_KonturUzytkuGruntowego"].NewRow();

                            XmlDocument docEgbKonturUzytkuGruntowego = new XmlDocument();
                            docEgbKonturUzytkuGruntowego.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbKonturUzytkuGruntowego = docEgbKonturUzytkuGruntowego.DocumentElement;

                            foreach (XmlNode node in elmEgbKonturUzytkuGruntowego.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbKonturUzytkuGruntowego["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbKonturUzytkuGruntowego["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:idUzytku":
                                        rowEgbKonturUzytkuGruntowego["idUzytku"] = node.InnerText;
                                        break;

                                    case "egb:OFU":
                                        rowEgbKonturUzytkuGruntowego["OFU"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_KonturUzytkuGruntowego"].Rows.Add(rowEgbKonturUzytkuGruntowego);

                            break; // egb:EGB_KonturUzytkuGruntowego


                        case "egb:EGB_LokalSamodzielny":

                            DataRow rowEgbLokalSamodzielny = dataSetGML.Tables["EGB_LokalSamodzielny"].NewRow();

                            XmlDocument docEgbLokalSamodzielny = new XmlDocument();
                            docEgbLokalSamodzielny.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbLokalSamodzielny = docEgbLokalSamodzielny.DocumentElement;

                            foreach (XmlNode node in elmEgbLokalSamodzielny.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbLokalSamodzielny["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbLokalSamodzielny["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:idLokalu":
                                        rowEgbLokalSamodzielny["idLokalu"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_LokalSamodzielny"].Rows.Add(rowEgbLokalSamodzielny);

                            break; // egb:EGB_LokalSamodzielny

                        case "egb:EGB_Malzenstwo":

                            DataRow rowEgbMalzenstwo = dataSetGML.Tables["EGB_Malzenstwo"].NewRow();

                            XmlDocument docEgbMalzenstwo = new XmlDocument();
                            docEgbMalzenstwo.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbMalzenstwo = docEgbMalzenstwo.DocumentElement;

                            foreach (XmlNode node in elmEgbMalzenstwo.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbMalzenstwo["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbMalzenstwo["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:status":
                                        rowEgbMalzenstwo["status"] = node.InnerText;
                                        break;

                                    case "egb:osobaFizyczna2":
                                        string osobaFizyczna2 = node.Attributes["xlink:href"].InnerText;
                                        rowEgbMalzenstwo["osobaFizyczna2"] = osobaFizyczna2.Substring(osobaFizyczna2.LastIndexOf(':') + 1);
                                        break;

                                    case "egb:osobaFizyczna3":
                                        string osobaFizyczna3 = node.Attributes["xlink:href"].InnerText;
                                        rowEgbMalzenstwo["osobaFizyczna3"] = osobaFizyczna3.Substring(osobaFizyczna3.LastIndexOf(':') + 1);
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_Malzenstwo"].Rows.Add(rowEgbMalzenstwo);

                            break; // egb:EGB_Malzenstwo

                        case "egb:EGB_ObiektTrwaleZwiazanyZBudynkiem":

                            DataRow rowEgbObiektTrwaleZwiazanyZBudynkiem = dataSetGML.Tables["EGB_ObiektTrwaleZwiazanyZBudynkiem"].NewRow();

                            XmlDocument docEgbObiektTrwaleZwiazanyZBudynkiem = new XmlDocument();
                            docEgbObiektTrwaleZwiazanyZBudynkiem.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbObiektTrwaleZwiazanyZBudynkiem = docEgbObiektTrwaleZwiazanyZBudynkiem.DocumentElement;

                            foreach (XmlNode node in elmEgbObiektTrwaleZwiazanyZBudynkiem.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbObiektTrwaleZwiazanyZBudynkiem["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbObiektTrwaleZwiazanyZBudynkiem["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:rodzajObiektuZwiazanegoZBudynkiem":
                                        rowEgbObiektTrwaleZwiazanyZBudynkiem["rodzajObiektuZwiazanegoZBudynkiem"] = node.InnerText;
                                        break;

                                    case "egb:budynekZElementamiTrwaleZwiazanymi":
                                        string idBudynku = node.Attributes["xlink:href"].InnerText;
                                        rowEgbObiektTrwaleZwiazanyZBudynkiem["budynekZElementamiTrwaleZwiazanymi"] = idBudynku.Substring(idBudynku.LastIndexOf(':') + 1);
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_ObiektTrwaleZwiazanyZBudynkiem"].Rows.Add(rowEgbObiektTrwaleZwiazanyZBudynkiem);

                            break; // egb:EGB_ObiektTrwaleZwiazanyZBudynkiem

                        case "egb:EGB_ObrebEwidencyjny":

                            DataRow rowEgbObrebEwidencyjny = dataSetGML.Tables["EGB_ObrebEwidencyjny"].NewRow();

                            XmlDocument docEgbObrebEwidencyjny = new XmlDocument();
                            docEgbObrebEwidencyjny.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbObrebEwidencyjny = docEgbObrebEwidencyjny.DocumentElement;

                            foreach (XmlNode node in elmEgbObrebEwidencyjny.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbObrebEwidencyjny["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbObrebEwidencyjny["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:idObrebu":
                                        rowEgbObrebEwidencyjny["idObrebu"] = node.InnerText;
                                        break;

                                    case "egb:nazwaWlasna":
                                        rowEgbObrebEwidencyjny["nazwaWlasna"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_ObrebEwidencyjny"].Rows.Add(rowEgbObrebEwidencyjny);

                            break; // egb:EGB_ObrebEwidencyjny

                        case "egb:EGB_OperatTechniczny":

                            DataRow rowEgbOperatTechniczny = dataSetGML.Tables["EGB_OperatTechniczny"].NewRow();

                            XmlDocument docEgbOperatTechniczny = new XmlDocument();
                            docEgbOperatTechniczny.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbOperatTechniczny = docEgbOperatTechniczny.DocumentElement;

                            foreach (XmlNode node in elmEgbOperatTechniczny.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbOperatTechniczny["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbOperatTechniczny["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:nazwaTworcy":
                                        rowEgbOperatTechniczny["nazwaTworcy"] = node.InnerText;
                                        break;

                                    case "egb:identyfikatorOperatuWgPZGIK":
                                        rowEgbOperatTechniczny["identyfikatorOperatuWgPZGIK"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_OperatTechniczny"].Rows.Add(rowEgbOperatTechniczny);

                            break; // egb:EGB_OperatTechniczny

                        case "egb:EGB_OsobaFizyczna":

                            DataRow rowEgbOsobaFizyczna = dataSetGML.Tables["EGB_OsobaFizyczna"].NewRow();

                            XmlDocument docEgbOsobaFizyczna = new XmlDocument();
                            docEgbOsobaFizyczna.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbOsobaFizyczna = docEgbOsobaFizyczna.DocumentElement;

                            foreach (XmlNode node in elmEgbOsobaFizyczna.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbOsobaFizyczna["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbOsobaFizyczna["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:pierwszeImie":
                                        rowEgbOsobaFizyczna["pierwszeImie"] = node.InnerText;
                                        break;

                                    case "egb:pierwszyCzlonNazwiska":
                                        rowEgbOsobaFizyczna["pierwszyCzlonNazwiska"] = node.InnerText;
                                        break;

                                    case "egb:plec":
                                        rowEgbOsobaFizyczna["plec"] = node.InnerText;
                                        break;

                                    case "egb:pesel":
                                        rowEgbOsobaFizyczna["pesel"] = node.InnerText;
                                        break;

                                    case "egb:drugiCzlonNazwiska":
                                        rowEgbOsobaFizyczna["drugiCzlonNazwiska"] = node.InnerText;
                                        break;

                                    case "egb:drugieImie":
                                        rowEgbOsobaFizyczna["drugieImie"] = node.InnerText;
                                        break;

                                    case "egb:imieMatki":
                                        rowEgbOsobaFizyczna["imieMatki"] = node.InnerText;
                                        break;

                                    case "egb:imieOjca":
                                        rowEgbOsobaFizyczna["imieOjca"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_OsobaFizyczna"].Rows.Add(rowEgbOsobaFizyczna);

                            break; // egb:EGB_OsobaFizyczna

                        case "egb:EGB_PodmiotGrupowy":

                            DataRow rowEgbPodmiotGrupowy = dataSetGML.Tables["EGB_PodmiotGrupowy"].NewRow();

                            XmlDocument docEgbPodmiotGrupowy = new XmlDocument();
                            docEgbPodmiotGrupowy.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbPodmiotGrupowy = docEgbPodmiotGrupowy.DocumentElement;

                            foreach (XmlNode node in elmEgbPodmiotGrupowy.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbPodmiotGrupowy["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbPodmiotGrupowy["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:status":
                                        rowEgbPodmiotGrupowy["status"] = node.InnerText;
                                        break;

                                    case "egb:nazwaPelna":
                                        rowEgbPodmiotGrupowy["nazwaPelna"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_PodmiotGrupowy"].Rows.Add(rowEgbPodmiotGrupowy);

                            break; // egb:EGB_PodmiotGrupowy

                        case "egb:EGB_PomieszczeniePrzynalezneDoLokalu":

                            DataRow rowEgbPomieszczeniePrzynalezneDoLokalu = dataSetGML.Tables["EGB_PomieszczeniePrzynalezneDoLokalu"].NewRow();

                            XmlDocument docEgbPomieszczeniePrzynalezneDoLokalu = new XmlDocument();
                            docEgbPomieszczeniePrzynalezneDoLokalu.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbPomieszczeniePrzynalezneDoLokalu = docEgbPomieszczeniePrzynalezneDoLokalu.DocumentElement;

                            foreach (XmlNode node in elmEgbPomieszczeniePrzynalezneDoLokalu.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbPomieszczeniePrzynalezneDoLokalu["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbPomieszczeniePrzynalezneDoLokalu["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:rodzajPomieszczeniaPrzynaleznego":
                                        rowEgbPomieszczeniePrzynalezneDoLokalu["rodzajPomieszczeniaPrzynaleznego"] = node.InnerText;
                                        break;

                                    case "egb:lokalizacjaPomieszczeniaPrzynaleznego":
                                        string lokalizacjaPomieszczeniaPrzynaleznego = node.Attributes["xlink:href"].InnerText;
                                        rowEgbPomieszczeniePrzynalezneDoLokalu["lokalizacjaPomieszczeniaPrzynaleznego"] = lokalizacjaPomieszczeniaPrzynaleznego.Substring(lokalizacjaPomieszczeniaPrzynaleznego.LastIndexOf(':') + 1);
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_PomieszczeniePrzynalezneDoLokalu"].Rows.Add(rowEgbPomieszczeniePrzynalezneDoLokalu);

                            break; // egb:EGB_PomieszczeniePrzynalezneDoLokalu

                        case "egb:EGB_PunktGraniczny":

                            DataRow rowEgbPunktGraniczny = dataSetGML.Tables["EGB_PunktGraniczny"].NewRow();

                            XmlDocument docEgbPunktGraniczny = new XmlDocument();
                            docEgbPunktGraniczny.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbPunktGraniczny = docEgbPunktGraniczny.DocumentElement;

                            foreach (XmlNode node in elmEgbPunktGraniczny.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbPunktGraniczny["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbPunktGraniczny["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:geometria":
                                        XmlNode point = node.FirstChild;
                                        XmlNode pos = point.FirstChild;
                                        string posXy = pos.InnerText;
                                        rowEgbPunktGraniczny["posX"] = posXy.Substring(0, posXy.IndexOf(' '));
                                        rowEgbPunktGraniczny["posY"] = posXy.Substring(posXy.IndexOf(' ') + 1);
                                        break;

                                    case "egb:idPunktu":
                                        rowEgbPunktGraniczny["idPunktu"] = node.InnerText;
                                        break;

                                    case "egb:dodatkoweInformacje":
                                        rowEgbPunktGraniczny["dodatkoweInformacje"] = node.InnerText;
                                        break;

                                    case "egb:oznWMaterialeZrodlowym":
                                        rowEgbPunktGraniczny["oznWMaterialeZrodlowym"] = node.InnerText;
                                        break;

                                    case "egb:zrodloDanychZRD":
                                        rowEgbPunktGraniczny["zrodloDanychZRD"] = node.InnerText;
                                        break;

                                    case "egb:bladPolozeniaWzgledemOsnowy":
                                        rowEgbPunktGraniczny["bladPolozeniaWzgledemOsnowy"] = node.InnerText;
                                        break;

                                    case "egb:kodStabilizacji":
                                        rowEgbPunktGraniczny["kodStabilizacji"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_PunktGraniczny"].Rows.Add(rowEgbPunktGraniczny);

                            break; // egb:EGB_PunktGraniczny

                        case "egb:EGB_UdzialDzierzawy":

                            DataRow rowEgbUdzialDzierzawy = dataSetGML.Tables["EGB_UdzialDzierzawy"].NewRow();

                            XmlDocument docEgbUdzialDzierzawy = new XmlDocument();
                            docEgbUdzialDzierzawy.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbUdzialDzierzawy = docEgbUdzialDzierzawy.DocumentElement;

                            foreach (XmlNode node in elmEgbUdzialDzierzawy.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbUdzialDzierzawy["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbUdzialDzierzawy["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:udzial":
                                        rowEgbUdzialDzierzawy["udzial"] = node.InnerText;
                                        break;

                                    case "egb:przedmiotUdzialuDzierzawy":
                                        string przedmiotUdzialuDzierzawy = node.Attributes["xlink:href"].InnerText;
                                        rowEgbUdzialDzierzawy["przedmiotUdzialuDzierzawy"] = przedmiotUdzialuDzierzawy.Substring(przedmiotUdzialuDzierzawy.LastIndexOf(':') + 1);
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_UdzialDzierzawy"].Rows.Add(rowEgbUdzialDzierzawy);

                            break; // egb:EGB_UdzialDzierzawy

                        case "egb:EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST":

                            DataRow rowEgbUdzialGospodarowaniaNieruchomosciaSpLubJst = dataSetGML.Tables["EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST"].NewRow();

                            XmlDocument docEgbUdzialGospodarowaniaNieruchomosciaSpLubJst = new XmlDocument();
                            docEgbUdzialGospodarowaniaNieruchomosciaSpLubJst.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbUdzialGospodarowaniaNieruchomosciaSpLubJst = docEgbUdzialGospodarowaniaNieruchomosciaSpLubJst.DocumentElement;

                            foreach (XmlNode node in elmEgbUdzialGospodarowaniaNieruchomosciaSpLubJst.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbUdzialGospodarowaniaNieruchomosciaSpLubJst["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbUdzialGospodarowaniaNieruchomosciaSpLubJst["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:rodzajUprawnien":
                                        rowEgbUdzialGospodarowaniaNieruchomosciaSpLubJst["rodzajUprawnien"] = node.InnerText;
                                        break;

                                    case "egb:licznikUlamkaOkreslajacegoWartoscUdzialu":
                                        rowEgbUdzialGospodarowaniaNieruchomosciaSpLubJst["licznikUlamkaOkreslajacegoWartoscUdzialu"] = node.InnerText;
                                        break;

                                    case "egb:mianownikUlamkaOkreslajacegoWartoscUdzialu":
                                        rowEgbUdzialGospodarowaniaNieruchomosciaSpLubJst["mianownikUlamkaOkreslajacegoWartoscUdzialu"] = node.InnerText;
                                        break;

                                    case "egb:podgrupaRej":
                                        rowEgbUdzialGospodarowaniaNieruchomosciaSpLubJst["podgrupaRej"] = node.InnerText;
                                        break;

                                    case "egb:przedmiotUdzialuGZ1":
                                        XmlNode egbJednostkaRejestrowa = node.FirstChild;
                                        XmlNode jr = egbJednostkaRejestrowa.FirstChild;

                                        rowEgbUdzialGospodarowaniaNieruchomosciaSpLubJst["przedmiotUdzialuGZ1"] = jr.Name;

                                        string idJr = jr.Attributes["xlink:href"].InnerText;
                                        rowEgbUdzialGospodarowaniaNieruchomosciaSpLubJst["idPrzedmiotUdzialuGZ1"] = idJr.Substring(idJr.LastIndexOf(':') + 1);
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST"].Rows.Add(rowEgbUdzialGospodarowaniaNieruchomosciaSpLubJst);

                            break; // egb:EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST

                        case "egb:EGB_UdzialWeWladaniuNieruchomosciaSPLubJST":

                            DataRow rowEgbUdzialWeWladaniuNieruchomosciaSpLubJst = dataSetGML.Tables["EGB_UdzialWeWladaniuNieruchomosciaSPLubJST"].NewRow();

                            XmlDocument docEgbUdzialWeWladaniuNieruchomosciaSpLubJst = new XmlDocument();
                            docEgbUdzialWeWladaniuNieruchomosciaSpLubJst.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbUdzialWeWladaniuNieruchomosciaSpLubJst = docEgbUdzialWeWladaniuNieruchomosciaSpLubJst.DocumentElement;

                            foreach (XmlNode node in elmEgbUdzialWeWladaniuNieruchomosciaSpLubJst.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbUdzialWeWladaniuNieruchomosciaSpLubJst["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbUdzialWeWladaniuNieruchomosciaSpLubJst["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:rodzajWladania":
                                        rowEgbUdzialWeWladaniuNieruchomosciaSpLubJst["rodzajWladania"] = node.InnerText;
                                        break;

                                    case "egb:licznikUlamkaOkreslajacegoWartoscUdzialu":
                                        rowEgbUdzialWeWladaniuNieruchomosciaSpLubJst["licznikUlamkaOkreslajacegoWartoscUdzialu"] = node.InnerText;
                                        break;

                                    case "egb:mianownikUlamkaOkreslajacegoWartoscUdzialu":
                                        rowEgbUdzialWeWladaniuNieruchomosciaSpLubJst["mianownikUlamkaOkreslajacegoWartoscUdzialu"] = node.InnerText;
                                        break;

                                    case "egb:podgrupaRej":
                                        rowEgbUdzialWeWladaniuNieruchomosciaSpLubJst["podgrupaRej"] = node.InnerText;
                                        break;

                                    case "egb:przedmiotUdzialuWladania":
                                        XmlNode egbJednostkaRejestrowa = node.FirstChild;
                                        XmlNode jr = egbJednostkaRejestrowa.FirstChild;

                                        rowEgbUdzialWeWladaniuNieruchomosciaSpLubJst["przedmiotUdzialuWladania"] = jr.Name;

                                        string idJr = jr.Attributes["xlink:href"].InnerText;
                                        rowEgbUdzialWeWladaniuNieruchomosciaSpLubJst["idPrzedmiotUdzialuWladania"] = idJr.Substring(idJr.LastIndexOf(':') + 1);
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_UdzialWeWladaniuNieruchomosciaSPLubJST"].Rows.Add(rowEgbUdzialWeWladaniuNieruchomosciaSpLubJst);

                            break; // egb:EGB_UdzialWeWladaniuNieruchomosciaSPLubJST

                        case "egb:EGB_UdzialWlasnosci":

                            DataRow rowEgbUdzialWlasnosci = dataSetGML.Tables["EGB_UdzialWlasnosci"].NewRow();

                            XmlDocument docEgbUdzialWlasnosci = new XmlDocument();
                            docEgbUdzialWlasnosci.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbUdzialWlasnosci = docEgbUdzialWlasnosci.DocumentElement;

                            foreach (XmlNode node in elmEgbUdzialWlasnosci.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbUdzialWlasnosci["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbUdzialWlasnosci["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:rodzajPrawa":
                                        rowEgbUdzialWlasnosci["rodzajPrawa"] = node.InnerText;
                                        break;

                                    case "egb:licznikUlamkaOkreslajacegoWartoscUdzialu":
                                        rowEgbUdzialWlasnosci["licznikUlamkaOkreslajacegoWartoscUdzialu"] = node.InnerText;
                                        break;

                                    case "egb:mianownikUlamkaOkreslajacegoWartoscUdzialu":
                                        rowEgbUdzialWlasnosci["mianownikUlamkaOkreslajacegoWartoscUdzialu"] = node.InnerText;
                                        break;

                                    case "egb:podgrupaRej":
                                        rowEgbUdzialWlasnosci["podgrupaRej"] = node.InnerText;
                                        break;

                                    case "egb:przedmiotUdzialuWlasnosci":
                                        XmlNode egbJednostkaRejestrowa = node.FirstChild;
                                        XmlNode jr = egbJednostkaRejestrowa.FirstChild;

                                        rowEgbUdzialWlasnosci["przedmiotUdzialuWlasnosci"] = jr.Name;

                                        string idJr = jr.Attributes["xlink:href"].InnerText;
                                        rowEgbUdzialWlasnosci["idPrzedmiotUdzialuWlasnosci"] = idJr.Substring(idJr.LastIndexOf(':') + 1);
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_UdzialWlasnosci"].Rows.Add(rowEgbUdzialWlasnosci);

                            break; // egb:EGB_UdzialWlasnosci

                        case "egb:EGB_ZarzadSpolkiWspolnotyGruntowej":

                            DataRow rowEgbZarzadSpolkiWspolnotyGruntowej = dataSetGML.Tables["EGB_ZarzadSpolkiWspolnotyGruntowej"].NewRow();

                            XmlDocument docEgbZarzadSpolkiWspolnotyGruntowej = new XmlDocument();
                            docEgbZarzadSpolkiWspolnotyGruntowej.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbZarzadSpolkiWspolnotyGruntowej = docEgbZarzadSpolkiWspolnotyGruntowej.DocumentElement;

                            foreach (XmlNode node in elmEgbZarzadSpolkiWspolnotyGruntowej.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbZarzadSpolkiWspolnotyGruntowej["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbZarzadSpolkiWspolnotyGruntowej["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:nazwaSpolkiPowolanejDoZarzadzaniaWspolnotaGruntowa":
                                        rowEgbZarzadSpolkiWspolnotyGruntowej["nazwaSpolkiPowolanejDoZarzadzaniaWspolnotaGruntowa"] = node.InnerText;
                                        break;

                                    case "egb:wspolnotaGruntowa":
                                        string wspolnotaGruntowa = node.Attributes["xlink:href"].InnerText;
                                        rowEgbZarzadSpolkiWspolnotyGruntowej["wspolnotaGruntowa"] = wspolnotaGruntowa.Substring(wspolnotaGruntowa.LastIndexOf(':') + 1);
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_ZarzadSpolkiWspolnotyGruntowej"].Rows.Add(rowEgbZarzadSpolkiWspolnotyGruntowej);

                            break; // egb:EGB_ZarzadSpolkiWspolnotyGruntowej

                        case "egb:EGB_Zmiana":

                            DataRow rowEgbZmiana = dataSetGML.Tables["EGB_Zmiana"].NewRow();

                            XmlDocument docEgbZmiana = new XmlDocument();
                            docEgbZmiana.LoadXml(gmlReader.ReadOuterXml());
                            XmlElement elmEgbZmiana = docEgbZmiana.DocumentElement;

                            foreach (XmlNode node in elmEgbZmiana.ChildNodes)
                            {
                                switch (node.Name)
                                {
                                    case "egb:idIIP":
                                        XmlNode btIdentyfikator = node.FirstChild;
                                        XmlNode lokalnyId = btIdentyfikator.FirstChild;
                                        rowEgbZmiana["lokalnyId"] = lokalnyId.InnerText;
                                        XmlNode przestrzenNazw = lokalnyId.NextSibling;
                                        rowEgbZmiana["przestrzenNazw"] = przestrzenNazw.InnerText;
                                        break;

                                    case "egb:nrZmiany":
                                        rowEgbZmiana["nrZmiany"] = node.InnerText;
                                        break;
                                }
                            }

                            dataSetGML.Tables["EGB_Zmiana"].Rows.Add(rowEgbZmiana);

                            break; // egb:EGB_Zmiana
                    }
                }

            } // GMLReader.Read()

            gmlReader?.Close();

        }

        // funkcja obsługująca postęp wykonywania czytania pliku GML
        private void BW_ReadGMLProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            stbMainProgress.Value = e.ProgressPercentage;
        }

        // ----------------------------------------------------------------------------------------
        // funkcja wywoływana po zakończeniu wczytywania pliku GML
        private void BW_ReadGMLRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //this.tbProgress.Text = "Canceled!";
                MessageBox.Show(e.Error.Message, @"Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Error != null)
            {
                //this.tbProgress.Text = ("Error: " + e.Error.Message);
                MessageBox.Show(e.Error.Message, @"Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            { // DONE
                btnOpenGML.Enabled = true;
                btnReadGML.Enabled = true;
                btnSaveGMLData.Enabled = true;

                lblBT_Dokument.Text = @"BT_Dokument: " + dataSetGML.Tables["BT_Dokument"].Rows.Count;
                lblEGB_Adres.Text = @"EGB_Adres: " + dataSetGML.Tables["EGB_Adres"].Rows.Count;
                lblEGB_ArkuszEwidencyjny.Text = @"EGB_ArkuszEwidencyjny: " + dataSetGML.Tables["EGB_ArkuszEwidencyjny"].Rows.Count;
                lblEGB_BlokBudynku.Text = @"EGB_BlokBudynku: " + dataSetGML.Tables["EGB_BlokBudynku"].Rows.Count;
                lblEGB_Budynek.Text = @"EGB_Budynek: " + dataSetGML.Tables["EGB_Budynek"].Rows.Count;
                lblEGB_DzialkaEwidencyjna.Text = @"EGB_DzialkaEwidencyjna: " + dataSetGML.Tables["EGB_DzialkaEwidencyjna"].Rows.Count;
                lblEGB_Dzierzawa.Text = @"EGB_Dzierzawa: " + dataSetGML.Tables["EGB_Dzierzawa"].Rows.Count;
                lblEGB_Instytucja.Text = @"EGB_Instytucja: " + dataSetGML.Tables["EGB_Instytucja"].Rows.Count;
                lblEGB_JednostkaEwidencyjna.Text = @"EGB_JednostkaEwidencyjna: " + dataSetGML.Tables["EGB_JednostkaEwidencyjna"].Rows.Count;
                lblEGB_JednostkaRejestrowaBudynkow.Text = @"EGB_JednostkaRejestrowaBudynkow: " + dataSetGML.Tables["EGB_JednostkaRejestrowaBudynkow"].Rows.Count;
                lblEGB_JednostkaRejestrowaGruntow.Text = @"EGB_JednostkaRejestrowaGruntow: " + dataSetGML.Tables["EGB_JednostkaRejestrowaGruntow"].Rows.Count;
                lblEGB_JednostkaRejestrowaLokali.Text = @"EGB_JednostkaRejestrowaLokali: " + dataSetGML.Tables["EGB_JednostkaRejestrowaLokali"].Rows.Count;
                lblEGB_Klasouzytek.Text = @"EGB_Klasouzytek: " + dataSetGML.Tables["EGB_Klasouzytek"].Rows.Count;
                lblEGB_KonturKlasyfikacyjny.Text = @"EGB_KonturKlasyfikacyjny: " + dataSetGML.Tables["EGB_KonturKlasyfikacyjny"].Rows.Count;
                lblEGB_KonturUzytkuGruntowego.Text = @"EGB_KonturUzytkuGruntowego: " + dataSetGML.Tables["EGB_KonturUzytkuGruntowego"].Rows.Count;
                lblEGB_LokalSamodzielny.Text = @"EGB_LokalSamodzielny: " + dataSetGML.Tables["EGB_LokalSamodzielny"].Rows.Count;
                lblEGB_Malzenstwo.Text = @"EGB_Malzenstwo: " + dataSetGML.Tables["EGB_Malzenstwo"].Rows.Count;
                lblEGB_ObiektTrwaleZwiazanyZBudynkiem.Text = @"EGB_ObiektTrwaleZwiazanyZBudynkiem: " + dataSetGML.Tables["EGB_ObiektTrwaleZwiazanyZBudynkiem"].Rows.Count;
                lblEGB_ObrebEwidencyjny.Text = @"EGB_ObrebEwidencyjny: " + dataSetGML.Tables["EGB_ObrebEwidencyjny"].Rows.Count;
                lblEGB_OperatTechniczny.Text = @"EGB_OperatTechniczny: " + dataSetGML.Tables["EGB_OperatTechniczny"].Rows.Count;
                lblEGB_OsobaFizyczna.Text = @"EGB_OsobaFizyczna: " + dataSetGML.Tables["EGB_OsobaFizyczna"].Rows.Count;
                lblEGB_PodmiotGrupowy.Text = @"EGB_PodmiotGrupowy: " + dataSetGML.Tables["EGB_PodmiotGrupowy"].Rows.Count;
                lblEGB_PomieszczeniePrzynalezneDoLokalu.Text = @"EGB_PomieszczeniePrzynalezneDoLokalu: " + dataSetGML.Tables["EGB_PomieszczeniePrzynalezneDoLokalu"].Rows.Count;
                lblEGB_PunktGraniczny.Text = @"EGB_PunktGraniczny: " + dataSetGML.Tables["EGB_PunktGraniczny"].Rows.Count;
                lblEGB_UdzialDzierzawy.Text = @"EGB_UdzialDzierzawy: " + dataSetGML.Tables["EGB_UdzialDzierzawy"].Rows.Count;
                lblEGB_UdzialGospodarowaniaNieruchomosciaSPLubJST.Text = @"EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST: " + dataSetGML.Tables["EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST"].Rows.Count;
                lblEGB_UdzialWeWladaniuNieruchomosciaSPLubJST.Text = @"EGB_UdzialWeWladaniuNieruchomosciaSPLubJST: " + dataSetGML.Tables["EGB_UdzialWeWladaniuNieruchomosciaSPLubJST"].Rows.Count;
                lblEGB_UdzialWlasnosci.Text = @"EGB_UdzialWlasnosci: " + dataSetGML.Tables["EGB_UdzialWlasnosci"].Rows.Count;
                lblEGB_ZarzadSpolkiWspolnotyGruntowej.Text = @"EGB_ZarzadSpolkiWspolnotyGruntowej: " + dataSetGML.Tables["EGB_ZarzadSpolkiWspolnotyGruntowej"].Rows.Count;
                lblEGB_Zmiana.Text = @"EGB_Zmiana: " + dataSetGML.Tables["EGB_Zmiana"].Rows.Count;

                UseWaitCursor = false;

                stbMainStatus.Text = @"Wczytano plik GML";

                System.Media.SystemSounds.Beep.Play();

                MessageBox.Show(@"Wczytano plik GML", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                //if (txtInputCSV.Text != @"Wskaż plik CSV z zestawieniem błędów")
                //{
                //    ButtonReadCSV_Click(this, EventArgs.Empty);
                //}
            }
        }

        //  ---------------------------------------------------------------------------------------
        //  obsluga przycisku zapisaujacego dane do GML 
        private void ButtonSaveGMLData_Click(object sender, EventArgs e)
        {
            if (_bwSaveGmlData.IsBusy != true)
            {
                btnOpenGML.Enabled = false;
                btnReadGML.Enabled = false;
                btnSaveGMLData.Enabled = false;

                stbMainStatus.Text = @"Zapisywanie danych GML na dysk...";
                UseWaitCursor = true;

                _bwSaveGmlData.RunWorkerAsync();
            }
        }

        //  ---------------------------------------------------------------------------------------
        //  funkcja zapisujaca dane z pliku GML na dysk do CSV
        private void BW_SaveGMLDataDoWork(object sender, DoWorkEventArgs e)
        {
            #region utworzenie katalogu lub kasowanie plików
            if (Directory.Exists(_outputDirectory))
            {
                DirectoryInfo di = new DirectoryInfo(_outputDirectory);

                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(ex.Message, @"Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }
            }
            else
            {
                Directory.CreateDirectory(_outputDirectory);
            }
            #endregion

            int tableCounter = 0;

            foreach (DataTable dataTable in dataSetGML.Tables)
            {
                tableCounter++;
                int percentComplete = (int)(tableCounter / (float)dataSetGML.Tables.Count * 100);
                _bwSaveGmlData.ReportProgress(percentComplete);

                stbMainStatus.Text = @"Zapisywanie: " + dataTable.TableName;

                FileInfo xlsFile = new FileInfo(_outputDirectory + dataTable.TableName + ".xlsx");

                ExcelPackage xlsWorkbook = new ExcelPackage(xlsFile);
                xlsWorkbook.Workbook.Properties.Title = dataTable.TableName;
                xlsWorkbook.Workbook.Properties.Author = "Grzegorz Gogolewski";
                xlsWorkbook.Workbook.Properties.Comments = "Raport wygenerowany przez aplikację: " + Application.ProductName + " [" + Application.ProductVersion + "]";
                xlsWorkbook.Workbook.Properties.Company = "GISNET";

                ExcelWorksheet xlsSheet = xlsWorkbook.Workbook.Worksheets.Add(dataTable.TableName);

                for (int columCounter = 1; columCounter <= dataTable.Columns.Count; columCounter++)
                {
                    xlsSheet.Cells[1, columCounter].Value = dataTable.Columns[columCounter - 1].ColumnName;
                }

                using (var range = xlsSheet.Cells[1, 1, 1, dataTable.Columns.Count])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    range.Style.Font.Color.SetColor(Color.Black);
                    range.AutoFilter = true;
                }

                for (int rowCounter = 0; rowCounter < dataTable.Rows.Count; rowCounter++)
                {
                    for (int columCounter = 0; columCounter < dataTable.Columns.Count; columCounter++)
                    {
                        if (dataTable.TableName == "BT_Dokument" && dataTable.Columns[columCounter].ColumnName == "rodzajDokumentu")
                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = Dict.DC_RodzajDokumentuType(dataTable.Rows[rowCounter][columCounter].ToString());
                        }
                        else

                        if (dataTable.TableName == "EGB_BlokBudynku" && dataTable.Columns[columCounter].ColumnName == "rodzajBloku")
                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = Dict.EGB_RodzajBlokuType(dataTable.Rows[rowCounter][columCounter].ToString());
                        }
                        else

                        if (dataTable.TableName == "EGB_Instytucja" && dataTable.Columns[columCounter].ColumnName == "status")
                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = Dict.EGB_StatusPodmiotuEwidType(dataTable.Rows[rowCounter][columCounter].ToString());
                        }
                        else

                        if (dataTable.TableName == "EGB_Malzenstwo" && dataTable.Columns[columCounter].ColumnName == "status")
                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = Dict.EGB_StatusPodmiotuEwidType(dataTable.Rows[rowCounter][columCounter].ToString());
                        }
                        else

                        if (dataTable.TableName == "EGB_ObiektTrwaleZwiazanyZBudynkiem" && dataTable.Columns[columCounter].ColumnName == "rodzajObiektuZwiazanegoZBudynkiem")
                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = Dict.EGB_RodzajObiektuZwiazanegoZBudynkiemType(dataTable.Rows[rowCounter][columCounter].ToString());
                        }
                        else

                        if (dataTable.TableName == "EGB_OsobaFizyczna" && dataTable.Columns[columCounter].ColumnName == "plec")
                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = Dict.EGB_PlecType(dataTable.Rows[rowCounter][columCounter].ToString());
                        }
                        else

                        if (dataTable.TableName == "EGB_PodmiotGrupowy" && dataTable.Columns[columCounter].ColumnName == "status")
                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = Dict.EGB_StatusPodmiotuEwidType(dataTable.Rows[rowCounter][columCounter].ToString());
                        }
                        else

                        if (dataTable.TableName == "EGB_PomieszczeniePrzynalezneDoLokalu" && dataTable.Columns[columCounter].ColumnName == "rodzajPomieszczeniaPrzynaleznego")
                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = Dict.EGB_RodzajPomieszczeniaPrzynaleznegoDoLokaluType(dataTable.Rows[rowCounter][columCounter].ToString());
                        }
                        else

                        if (dataTable.TableName == "EGB_PunktGraniczny" && dataTable.Columns[columCounter].ColumnName == "zrodloDanychZRD")
                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = Dict.EGB_ZrodloDanychZRDType(dataTable.Rows[rowCounter][columCounter].ToString());
                        }
                        else

                        if (dataTable.TableName == "EGB_PunktGraniczny" && dataTable.Columns[columCounter].ColumnName == "bladPolozeniaWzgledemOsnowy")
                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = Dict.EGB_BladPolozeniaWzgledemOsnowyType(dataTable.Rows[rowCounter][columCounter].ToString());
                        }
                        else

                        if (dataTable.TableName == "EGB_PunktGraniczny" && dataTable.Columns[columCounter].ColumnName == "kodStabilizacji")
                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = Dict.EGB_KodStabilizacjiType(dataTable.Rows[rowCounter][columCounter].ToString());
                        }
                        else

                        if (dataTable.TableName == "EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST" && dataTable.Columns[columCounter].ColumnName == "rodzajUprawnien")
                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = Dict.EGB_RodzajUprawnienType(dataTable.Rows[rowCounter][columCounter].ToString());
                        }
                        else

                        if (dataTable.TableName == "EGB_UdzialWeWladaniuNieruchomosciaSPLubJST" && dataTable.Columns[columCounter].ColumnName == "rodzajWladania")
                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = Dict.EGB_RodzajWladaniaType(dataTable.Rows[rowCounter][columCounter].ToString());
                        }
                        else

                        if (dataTable.TableName == "EGB_UdzialWlasnosci" && dataTable.Columns[columCounter].ColumnName == "rodzajPrawa")
                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = Dict.EGB_RodzajWladaniaType(dataTable.Rows[rowCounter][columCounter].ToString());
                        }
                        else

                        {
                            xlsSheet.Cells[rowCounter + 2, columCounter + 1].Value = dataTable.Rows[rowCounter][columCounter].ToString();
                        }

                    }

                }

                xlsSheet.View.FreezePanes(2, 1);
                xlsSheet.Cells.Style.Font.Size = 10;
                xlsSheet.Cells.AutoFitColumns(0);
                xlsWorkbook.Save();

            }

        }

        // ----------------------------------------------------------------------------------------
        // funkcja obsługująca postęp zapisywania dnaych z pliku GML do CSV
        private void BW_SaveGMLDataProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            stbMainProgress.Value = e.ProgressPercentage;
        }

        // ----------------------------------------------------------------------------------------
        // funkcja wywoływana po zakończeniu zapisywania danych GML na dysk do CSV
        private void BW_SaveGMLDataRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //this.tbProgress.Text = "Canceled!";
                MessageBox.Show(e.Error.Message, @"Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Error != null)
            {
                //this.tbProgress.Text = ("Error: " + e.Error.Message);
                MessageBox.Show(e.Error.Message, @"Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            { // DONE
                btnOpenGML.Enabled = true;
                btnReadGML.Enabled = true;
                btnSaveGMLData.Enabled = true;

                UseWaitCursor = false;

                stbMainStatus.Text = @"Zapisano dane z pliku GML.";

                //System.Media.SystemSounds.Beep.Play();

                MessageBox.Show(@"Zapisano dane z pliku GML.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                Process.Start("explorer.exe", _outputDirectory);
            }
        }

        // ----------------------------------------------------------------------------------------
        // funkcja obsługująca wcisniecie przycisku otwarcia pliku csv
        private void ButtonOpenCSV_Click(object sender, EventArgs e)
        {
            dlgOpen.Filter = @"CSV (*.csv)|*.csv";
            dlgOpen.FileName = _gmlFile + "*.csv";

            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                txtInputCSV.Text = dlgOpen.FileName;

                btnOpenCSV.Enabled = true;
                btnReadCSV.Enabled = true;

                // czyszczenie tabel raportu
                ClearCsvData();
            }
        }

        // ----------------------------------------------------------------------------------------
        // funkcja obsługująca wcisniecie przycisku czytania pliku csv
        private void ButtonReadCSV_Click(object sender, EventArgs e)
        {
            if ((_bwReadCsv.IsBusy != true) && (_bwReadGml.IsBusy != true))
            {
                btnOpenCSV.Enabled = false;
                btnReadCSV.Enabled = false;

                stbMainStatus.Text = @"Wczytywanie pliku CSV...";
                UseWaitCursor = true;

                // czyszczenie tabel raportu
                ClearCsvData();

                _bwReadCsv.RunWorkerAsync();
            }
        }

        // główna funkcja obłsugi wątku wczytywania pliku csv
        private void BW_ReadCSVDoWork(object sender, DoWorkEventArgs e)
        {
            StreamReader fileIn;
            StreamWriter fileOut;

            try
            {
                fileIn = new StreamReader(txtInputCSV.Text, Encoding.GetEncoding(1250));
            }
            catch (IOException ex)
            {

                MessageBox.Show(ex.Message, @"Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            try
            {
                fileOut = new StreamWriter(new FileStream(txtInputCSV.Text + ".tmp", FileMode.Create), Encoding.GetEncoding(1250));
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, @"Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            bool openQ = false;

            while (fileIn.Peek() >= 0)
            {
                char character = (char)fileIn.Read();

                if (character == '"')
                {
                    if (openQ == false)
                    {
                        openQ = true;
                    }
                    else
                    {
                        openQ = false;
                    }
                }

                if ((character == '\n') && openQ) fileOut.Write(',');
                else if ((character == ';') && openQ) fileOut.Write('|');
                else if ((character == '\r') && openQ) { }
                else fileOut.Write(character);
            }

            fileOut.Close();
            fileIn.Close();

            fileIn = new StreamReader(txtInputCSV.Text + ".tmp", Encoding.GetEncoding(1250));

            // ------------------------------------------------------------------------------------

            string line;

            while ((line = fileIn.ReadLine()) != null)
            {
                string[] vals = line.Split(';');

                if (vals[5] == "Błąd krytyczny" || (vals[5] == "Błąd niekrytyczny" && chboxKrytyczne.Checked == false))
                {
                    DataRow r = dataSetCSV.Tables["Raport"].NewRow();
                    r["typWeryfikacji"] = vals[0];
                    r["rodzajWeryfikacji"] = vals[1];
                    r["szczegolyWeryfikacji"] = vals[2];

                    if (vals[3] == "")
                    {
                        r["rodzajObiektu"] = "inne";
                    }
                    else
                    {
                        r["rodzajObiektu"] = vals[3];
                    }

                    r["statusWeryfikacji"] = vals[4];
                    r["wynikWeryfikacji"] = vals[5];
                    r["idObiektu"] = vals[6];

                    string idObiektu = vals[6];

                    if (idObiektu.StartsWith("\""))
                    {
                        idObiektu = idObiektu.Replace("\"Przestrzeń nazw: ", "");
                        int posComma = idObiektu.IndexOf(',');
                        r["przestrzenNazw"] = idObiektu.Substring(0, posComma);

                        idObiektu = idObiektu.Substring(posComma + 1);
                        idObiektu = idObiektu.Replace("Lokalny ID: ", "");
                        posComma = idObiektu.IndexOf(',');
                        r["lokalnyId"] = idObiektu.Substring(0, posComma);
                    }

                    int ifLinia = vals[7].IndexOf(",Linia:", StringComparison.Ordinal);

                    if (ifLinia > 0)
                    {
                        r["opis"] = vals[7].Substring(0, ifLinia) + "\"";

                        string lineNumer = vals[7].Substring(ifLinia + 8);

                        lineNumer = lineNumer.Substring(0, lineNumer.IndexOf(','));

                        r["linia"] = lineNumer;
                    }
                    else
                    {
                        r["opis"] = vals[7];
                        r["linia"] = null;
                    }

                    dataSetCSV.Tables["Raport"].Rows.Add(r);
                }
            }

            fileIn.Close();

            File.Delete(txtInputCSV.Text + ".tmp");

        }

        // ----------------------------------------------------------------------------------------
        // funkcja obsługująca postęp wczytywania pliku CSV
        private void BW_ReadCSVProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //this.tbProgress.Text = (e.ProgressPercentage.ToString() + "%");
        }

        // ----------------------------------------------------------------------------------------
        // funkcja wywoływana po zakończeniu wczytywania pliku CSV do CSV
        private void BW_ReadCSVRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //this.tbProgress.Text = "Canceled!";
                MessageBox.Show(e.Error.Message, @"Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Error != null)
            {
                //this.tbProgress.Text = ("Error: " + e.Error.Message);
                MessageBox.Show(e.Error.Message, @"Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            { // DONE
                btnOpenCSV.Enabled = true;
                btnReadCSV.Enabled = true;

                lblLiczbaRekordow.Text = @"Liczba błędów: " + dataSetCSV.Tables["Raport"].Rows.Count.ToString();

                UseWaitCursor = false;

                stbMainStatus.Text = @"Wczytano plik CSV";

                System.Media.SystemSounds.Beep.Play();

                MessageBox.Show(@"Wczytano plik CSV", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                //if (txtInputCSV.Text != @"Wskaż plik CSV z zestawieniem błędów")
                //{
                //    ButtonSaveErrorList_Click(this, EventArgs.Empty);
                //}

            }
        }

        // ----------------------------------------------------------------------------------------
        // funkcja obsługująca wcisnięcie przysisku zapisu błedów
        private void ButtonSaveErrorList_Click(object sender, EventArgs e)
        {
            if ((_bwReadCsv.IsBusy != true) && (_bwReadGml.IsBusy != true))
            {
                stbMainStatus.Text = @"Zapisywanie danych o błędach...";
                UseWaitCursor = true;

                _bwSaveErrors.RunWorkerAsync();
            }
        }


        //  ---------------------------------------------------------------------------------------
        //  funkcja wątku głownego zapisu raportu błędów na dysk 
        private void BW_bwSaveErrorsDoWork(object sender, DoWorkEventArgs e)
        {
            #region tworzenie katalogu lub usuwanie plików
            if (Directory.Exists(_errorDirectory))
            {
                DirectoryInfo di = new DirectoryInfo(_errorDirectory);

                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(ex.Message, @"Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(_errorDirectory);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, @"Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            #endregion

            DataView vRaport = new DataView(dataSetCSV.Tables["Raport"]);
            DataTable distinctValues = vRaport.ToTable(true, "rodzajObiektu");

            int rowCounter = 0;

            foreach (DataRow rowObiektyBlad in distinctValues.Rows)
            {
                StreamWriter fileSql;

                DataRow[] gmlRows;
                int errorsCount;

                rowCounter++;
                int percentComplete = (int)(rowCounter / (float)distinctValues.Rows.Count * 100);
                _bwSaveErrors.ReportProgress(percentComplete);

                string rodzajObiektu = rowObiektyBlad["rodzajObiektu"].ToString();

                stbMainStatus.Text = @"Zapisywanie: " + rodzajObiektu;

                FileInfo xlsFile = new FileInfo(_errorDirectory + rodzajObiektu + "_errors.xlsx");

                ExcelPackage xlsWorkbook = new ExcelPackage(xlsFile);
                xlsWorkbook.Workbook.Properties.Title = rodzajObiektu;
                xlsWorkbook.Workbook.Properties.Author = "Grzegorz Gogolewski";
                xlsWorkbook.Workbook.Properties.Comments = "Raport wygenerowany przez aplikację: " + Application.ProductName + " [" + Application.ProductVersion + "]";
                xlsWorkbook.Workbook.Properties.Company = "GISNET";

                ExcelWorksheet xlsSheet = xlsWorkbook.Workbook.Worksheets.Add(rodzajObiektu);

                switch (rodzajObiektu)
                {
                    /* ------------------------------------------------------------------------------------
                       BT_Dokument
                    ------------------------------------------------------------------------------------ */
                    case "BT_Dokument":
                    case "BT_Dokument_zal":
                    case "BT_Dokument_zasSiec":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "nazwaTworcyDokumentu";
                        xlsSheet.Cells[1, 3].Value = "rodzajDokumentu";
                        xlsSheet.Cells[1, 4].Value = "sygnaturaDokumentu";
                        xlsSheet.Cells[1, 5].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 7].Value = "opis";
                        xlsSheet.Cells[1, 8].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 8])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami dokumentu
                            gmlRows = dataSetGML.Tables["BT_Dokument"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string nazwaTworcyDokumentu = gmlRows[0]["nazwaTworcyDokumentu"].ToString();
                            string rodzajDokumentu = Dict.DC_RodzajDokumentuType(gmlRows[0]["rodzajDokumentu"].ToString());
                            string sygnaturaDokumentu = gmlRows[0]["sygnaturaDokumentu"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = nazwaTworcyDokumentu;
                            xlsSheet.Cells[i + 2, 3].Value = rodzajDokumentu;
                            xlsSheet.Cells[i + 2, 4].Value = sygnaturaDokumentu;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : nazwaTworcyDokumentu;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : rodzajDokumentu;
                            xlsSheet.Cells[i + 2, 4].Value = i > 2 ? "DEMO_" + i.ToString() : sygnaturaDokumentu;
#endif
                            xlsSheet.Cells[i + 2, 5].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 7].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 8].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_Adres
                    ------------------------------------------------------------------------------------ */
                    case "EGB_Adres":
                    case "EGB_Adres_dok":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "miejscowosc";
                        xlsSheet.Cells[1, 3].Value = "ulica";
                        xlsSheet.Cells[1, 4].Value = "numerPorzadkowy";
                        xlsSheet.Cells[1, 5].Value = "nrLokalu";
                        xlsSheet.Cells[1, 6].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 7].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 8].Value = "opis";
                        xlsSheet.Cells[1, 9].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 9])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami adresu
                            gmlRows = dataSetGML.Tables["EGB_Adres"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string miejscowosc = gmlRows[0]["miejscowosc"].ToString();
                            string nrLokalu = gmlRows[0]["nrLokalu"].ToString();
                            string numerPorzadkowy = gmlRows[0]["numerPorzadkowy"].ToString();
                            string ulica = gmlRows[0]["ulica"].ToString();

#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = miejscowosc;
                            xlsSheet.Cells[i + 2, 3].Value = ulica;
                            xlsSheet.Cells[i + 2, 4].Value = numerPorzadkowy;
                            xlsSheet.Cells[i + 2, 5].Value = nrLokalu;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : miejscowosc;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : ulica;
                            xlsSheet.Cells[i + 2, 4].Value = i > 2 ? "DEMO_" + i.ToString() : numerPorzadkowy;
                            xlsSheet.Cells[i + 2, 5].Value = i > 2 ? "DEMO_" + i.ToString() : nrLokalu;
#endif
                            xlsSheet.Cells[i + 2, 6].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 7].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 8].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 9].Value = err.Linia;

                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_ArkuszEwidencyjny
                    ------------------------------------------------------------------------------------ */
                    case "EGB_ArkuszEwidencyjny":
                    case "EGB_Arkusz_dok":
                    case "EGB_Arkusz_pktGrArkusza":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idArkusza";
                        xlsSheet.Cells[1, 3].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 4].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "opis";
                        xlsSheet.Cells[1, 6].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 6])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami arkusza
                            gmlRows = dataSetGML.Tables["EGB_ArkuszEwidencyjny"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string idArkusza = gmlRows[0]["idArkusza"].ToString();

#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idArkusza;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idArkusza;
#endif
                            xlsSheet.Cells[i + 2, 3].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 4].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 6].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                       EGB_BlokBudynku
                    ------------------------------------------------------------------------------------ */
                    case "EGB_BlokBudynku":
                    case "EGB_BlokBudynku_dok":

                        fileSql = new StreamWriter(new FileStream(_errorDirectory + rodzajObiektu + "_errors.sql", FileMode.Create), Encoding.GetEncoding(1250));
                        fileSql.WriteLine("AND EW_BUD_IDG5(kbudynek.mslink) in (");

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idBudynku";
                        xlsSheet.Cells[1, 3].Value = "rodzajBloku";
                        xlsSheet.Cells[1, 4].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "opis";
                        xlsSheet.Cells[1, 7].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 7])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami bloku
                            gmlRows = dataSetGML.Tables["EGB_BlokBudynku"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string rodzajBloku = Dict.EGB_RodzajBlokuType(gmlRows[0]["rodzajBloku"].ToString());
                            string budynekZWyodrebnionymBlokiemBudynku = gmlRows[0]["budynekZWyodrebnionymBlokiemBudynku"].ToString();

                            // wczytaj wiersz z atrybutami budynku
                            gmlRows = dataSetGML.Tables["EGB_Budynek"].Select("lokalnyId = '" + budynekZWyodrebnionymBlokiemBudynku + "'");

                            string idBudynku = "brak";
                            if (gmlRows.Length != 0) idBudynku = gmlRows[0]["idBudynku"].ToString();

#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idBudynku;
                            xlsSheet.Cells[i + 2, 3].Value = rodzajBloku;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idBudynku;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : rodzajBloku;
#endif
                            xlsSheet.Cells[i + 2, 4].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 7].Value = err.Linia;
#if !DEMO
                            if (i != errorsCount - 1) fileSql.WriteLine("'" + idBudynku + "',"); else fileSql.WriteLine("'" + idBudynku + "'");
#endif
                        }

                        fileSql.WriteLine(")");
                        fileSql.Close();

                        break;

                    /* ------------------------------------------------------------------------------------
                       EGB_Budynek
                    ------------------------------------------------------------------------------------ */
                    case "EGB_Budynek":
                    case "EGB_Budynek_adres":
                    case "EGB_Budynek_dok":
                    case "EGB_Budynek_dzialZabBud":
                    case "EGB_Budynek_infoDotCzBud":
                    case "EGB_Budynek_innaFcjaBud":
                    case "EGB_Budynek_numerEKW":
                    case "EGB_Budynek_numerKW":

                        fileSql = new StreamWriter(new FileStream(_errorDirectory + rodzajObiektu + "_errors.sql", FileMode.Create), Encoding.GetEncoding(1250));
                        fileSql.WriteLine("AND EW_BUD_IDG5(kbudynek.mslink) in (");

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idBudynku";
                        xlsSheet.Cells[1, 3].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 4].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "opis";
                        xlsSheet.Cells[1, 6].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 6])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami 
                            gmlRows = dataSetGML.Tables["EGB_Budynek"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string idBudynku = gmlRows[0]["idBudynku"].ToString();

#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idBudynku;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idBudynku;
#endif

                            xlsSheet.Cells[i + 2, 3].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 4].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 6].Value = err.Linia;
#if !DEMO
                            if (i != errorsCount - 1) fileSql.WriteLine("'" + idBudynku + "',"); else fileSql.WriteLine("'" + idBudynku + "'");
#endif
                            }

                        fileSql.WriteLine(")");
                        fileSql.Close();

                        break;

                    /* ------------------------------------------------------------------------------------
                       EGB_DzialkaEwidencyjna
                    ------------------------------------------------------------------------------------ */
                    case "EGB_DzialkaEwidencyjna":
                    case "EGB_Dzialka_adres":
                    case "EGB_Dzialka_dok":
                    case "EGB_Dzialka_kluzWGrDzial":
                    case "EGB_Dzialka_numerEKW":
                    case "EGB_Dzialka_numerKW":
                    case "EGB_Dzialka_pktGrDz":

                        fileSql = new StreamWriter(new FileStream(_errorDirectory + rodzajObiektu + "_errors.sql", FileMode.Create), Encoding.GetEncoding(1250));
                        fileSql.WriteLine("AND EW_DLK_IDG5(kdzialka.mslink) in (");

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idDzialki";
                        xlsSheet.Cells[1, 3].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 4].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "opis";
                        xlsSheet.Cells[1, 6].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 6])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami 
                            gmlRows = dataSetGML.Tables["EGB_DzialkaEwidencyjna"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string idDzialki = gmlRows[0]["idDzialki"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idDzialki;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idDzialki;
#endif
                            xlsSheet.Cells[i + 2, 3].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 4].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 6].Value = err.Linia;
#if !DEMO
                            if (i != errorsCount - 1) fileSql.WriteLine("'" + idDzialki + "',"); else fileSql.WriteLine("'" + idDzialki + "'");
#endif
                        }

                        fileSql.WriteLine(")");
                        fileSql.Close();

                        break;

                    /* ------------------------------------------------------------------------------------
                       EGB_Dzierzawa
                    ------------------------------------------------------------------------------------ */
                    case "EGB_Dzierzawa":
                    case "EGB_Dzierzawa_budynek":
                    case "EGB_Dzierzawa_czescDz":
                    case "EGB_Dzierzawa_dok":
                    case "EGB_Dzierzawa_dzialka":
                    case "EGB_Dzierzawa_lokal":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idDzierzawy";
                        xlsSheet.Cells[1, 3].Value = "budynekObjetyDzierzawa";
                        xlsSheet.Cells[1, 4].Value = "lokalObjetyDzierzawa";
                        xlsSheet.Cells[1, 5].Value = "dzialkaObjetaDzierzawa";
                        xlsSheet.Cells[1, 6].Value = "czescDzialkiObjetaDzierzawa";
                        xlsSheet.Cells[1, 7].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 8].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 9].Value = "opis";
                        xlsSheet.Cells[1, 10].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 10])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami
                            gmlRows = dataSetGML.Tables["EGB_Dzierzawa"].Select("lokalnyId = '" + err.LokalnyId + "'");

                            string idDzierzawy = gmlRows[0]["idDzierzawy"].ToString();
                            string budynekObjetyDzierzawa = gmlRows[0]["budynekObjetyDzierzawa"].ToString();
                            string lokalObjetyDzierzawa = gmlRows[0]["lokalObjetyDzierzawa"].ToString();
                            string dzialkaObjetaDzierzawa = gmlRows[0]["dzialkaObjetaDzierzawa"].ToString();
                            string czescDzialkiObjetaDzierzawa = gmlRows[0]["czescDzialkiObjetaDzierzawa"].ToString();

                            // wczytaj wiersz z atrybutami budynku
                            string idBudynku = "brak";
                            gmlRows = dataSetGML.Tables["EGB_Budynek"].Select("lokalnyId = '" + budynekObjetyDzierzawa + "'");

                            if (gmlRows.Length != 0) idBudynku = gmlRows[0]["idBudynku"].ToString();

                            // wczytaj wiersz z atrybutami lokalu
                            string idLokalu = "brak";
                            gmlRows = dataSetGML.Tables["EGB_LokalSamodzielny"].Select("lokalnyId = '" + lokalObjetyDzierzawa + "'");

                            if (gmlRows.Length != 0) idLokalu = gmlRows[0]["idLokalu"].ToString();

                            // wczytaj wiersz z atrybutami działki
                            string idDzialki = "brak";
                            gmlRows = dataSetGML.Tables["EGB_DzialkaEwidencyjna"].Select("lokalnyId = '" + dzialkaObjetaDzierzawa + "'");

                            if (gmlRows.Length != 0) idDzialki = gmlRows[0]["idDzialki"].ToString();

                            // wczytaj wiersz z atrybutami częsci działki
                            string idDzialkiCzesc = "brak";
                            gmlRows = dataSetGML.Tables["EGB_DzialkaEwidencyjna"].Select("lokalnyId = '" + czescDzialkiObjetaDzierzawa + "'");

                            if (gmlRows.Length != 0) idDzialkiCzesc = gmlRows[0]["idDzialki"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idDzierzawy;
                            xlsSheet.Cells[i + 2, 3].Value = idBudynku;
                            xlsSheet.Cells[i + 2, 4].Value = idLokalu;
                            xlsSheet.Cells[i + 2, 5].Value = idDzialki;
                            xlsSheet.Cells[i + 2, 6].Value = idDzialkiCzesc;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idDzierzawy;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : idBudynku;
                            xlsSheet.Cells[i + 2, 4].Value = i > 2 ? "DEMO_" + i.ToString() : idLokalu;
                            xlsSheet.Cells[i + 2, 5].Value = i > 2 ? "DEMO_" + i.ToString() : idDzialki;
                            xlsSheet.Cells[i + 2, 6].Value = i > 2 ? "DEMO_" + i.ToString() : idDzialkiCzesc;
#endif
                            xlsSheet.Cells[i + 2, 7].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 8].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 9].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 10].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                       EGB_Instytucja
                    ------------------------------------------------------------------------------------ */
                    case "EGB_Instytucja":
                    case "EGB_Instytucja_dok":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "status";
                        xlsSheet.Cells[1, 3].Value = "nazwaPelna";
                        xlsSheet.Cells[1, 4].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "opis";
                        xlsSheet.Cells[1, 7].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 7])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami
                            gmlRows = dataSetGML.Tables["EGB_Instytucja"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string status = Dict.EGB_StatusPodmiotuEwidType(gmlRows[0]["status"].ToString());
                            string nazwaPelna = gmlRows[0]["nazwaPelna"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = status;
                            xlsSheet.Cells[i + 2, 3].Value = nazwaPelna;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : status;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : nazwaPelna;
#endif
                            xlsSheet.Cells[i + 2, 4].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 7].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                       EGB_JednostkaEwidencyjna
                    ------------------------------------------------------------------------------------ */
                    case "EGB_JednostkaEwidencyjna":
                    case "EGB_JedEwid_dok":
                    case "EGB_JedEwid_pktGrJedEwid":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idJednostkiEwid";
                        xlsSheet.Cells[1, 3].Value = "nazwaWlasna";
                        xlsSheet.Cells[1, 4].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "opis";
                        xlsSheet.Cells[1, 7].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 7])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami
                            gmlRows = dataSetGML.Tables["EGB_JednostkaEwidencyjna"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string idJednostkiEwid = gmlRows[0]["idJednostkiEwid"].ToString();
                            string nazwaWlasna = gmlRows[0]["nazwaWlasna"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idJednostkiEwid;
                            xlsSheet.Cells[i + 2, 3].Value = nazwaWlasna;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idJednostkiEwid;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : nazwaWlasna;
#endif
                            xlsSheet.Cells[i + 2, 4].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 7].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                       EGB_JednostkaRejestrowaBudynkow
                    ------------------------------------------------------------------------------------ */
                    case "EGB_JednostkaRejestrowaBudynkow":
                    case "EGB_JRB_dok":
                    case "EGB_JRB_JRGZwiazanaZJRB":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idJednostkiRejestrowej";
                        xlsSheet.Cells[1, 3].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 4].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "opis";
                        xlsSheet.Cells[1, 6].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 6])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami bloku
                            gmlRows = dataSetGML.Tables["EGB_JednostkaRejestrowaBudynkow"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string idJednostkiRejestrowej = gmlRows[0]["idJednostkiRejestrowej"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idJednostkiRejestrowej;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idJednostkiRejestrowej;
#endif
                            xlsSheet.Cells[i + 2, 3].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 4].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 6].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                       EGB_JednostkaRejestrowaGruntow
                    ------------------------------------------------------------------------------------ */
                    case "EGB_JednostkaRejestrowaGruntow":
                    case "EGB_JRG_dok":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idJednostkiRejestrowej";
                        xlsSheet.Cells[1, 3].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 4].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "opis";
                        xlsSheet.Cells[1, 6].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 6])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami bloku
                            gmlRows = dataSetGML.Tables["EGB_JednostkaRejestrowaGruntow"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string idJednostkiRejestrowej = gmlRows[0]["idJednostkiRejestrowej"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idJednostkiRejestrowej;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idJednostkiRejestrowej;
#endif
                            xlsSheet.Cells[i + 2, 3].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 4].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 6].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_JednostkaRejestrowaLokali
                    ------------------------------------------------------------------------------------ */
                    case "EGB_JednostkaRejestrowaLokali":
                    case "EGB_JRL_dok":
                    case "EGB_JRL_JRGZwiazanaZJRL":
                    case "EGB_JRL_nierWspDlaLokalu":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idJednostkiRejestrowej";
                        xlsSheet.Cells[1, 3].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 4].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "opis";
                        xlsSheet.Cells[1, 6].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 6])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami bloku
                            gmlRows = dataSetGML.Tables["EGB_JednostkaRejestrowaLokali"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string idJednostkiRejestrowej = gmlRows[0]["idJednostkiRejestrowej"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idJednostkiRejestrowej;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idJednostkiRejestrowej;
#endif
                            xlsSheet.Cells[i + 2, 3].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 4].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 6].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_Klasouzytek
                    ------------------------------------------------------------------------------------ */
                    case "EGB_Klasouzytek":
                    case "EGB_Klasouzytek_dok":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "OFU";
                        xlsSheet.Cells[1, 3].Value = "OZU";
                        xlsSheet.Cells[1, 4].Value = "OZK";
                        xlsSheet.Cells[1, 5].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 7].Value = "opis";
                        xlsSheet.Cells[1, 8].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 8])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami 
                            gmlRows = dataSetGML.Tables["EGB_Klasouzytek"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string ofu = gmlRows[0]["OFU"].ToString();
                            string ozu = gmlRows[0]["OZU"].ToString();
                            string ozk = gmlRows[0]["OZK"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = ofu;
                            xlsSheet.Cells[i + 2, 3].Value = ozu;
                            xlsSheet.Cells[i + 2, 4].Value = ozk;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : ofu;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : ozu;
                            xlsSheet.Cells[i + 2, 4].Value = i > 2 ? "DEMO_" + i.ToString() : ozk;
#endif
                            xlsSheet.Cells[i + 2, 5].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 7].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 8].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_KonturKlasyfikacyjny
                    ------------------------------------------------------------------------------------ */
                    case "EGB_KonturKlasyfikacyjny":
                    case "EGB_KonturKlas_dok":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idKonturu";
                        xlsSheet.Cells[1, 3].Value = "OZU";
                        xlsSheet.Cells[1, 4].Value = "OZK";
                        xlsSheet.Cells[1, 5].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 7].Value = "opis";
                        xlsSheet.Cells[1, 8].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 8])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami 
                            gmlRows = dataSetGML.Tables["EGB_KonturKlasyfikacyjny"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string idKonturu = gmlRows[0]["idKonturu"].ToString();
                            string ozu = gmlRows[0]["OZU"].ToString();
                            string ozk = gmlRows[0]["OZK"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idKonturu;
                            xlsSheet.Cells[i + 2, 3].Value = ozu;
                            xlsSheet.Cells[i + 2, 4].Value = ozk;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idKonturu;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : ozu;
                            xlsSheet.Cells[i + 2, 4].Value = i > 2 ? "DEMO_" + i.ToString() : ozk;
#endif
                            xlsSheet.Cells[i + 2, 5].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 7].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 8].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_KonturUzytkuGruntowego
                    ------------------------------------------------------------------------------------ */
                    case "EGB_KonturUzytkuGruntowego":
                    case "EGB_KonturUzytGrunt_dok":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idUzytku";
                        xlsSheet.Cells[1, 3].Value = "OFU";
                        xlsSheet.Cells[1, 4].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "opis";
                        xlsSheet.Cells[1, 7].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 7])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami 
                            gmlRows = dataSetGML.Tables["EGB_KonturUzytkuGruntowego"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string idUzytku = gmlRows[0]["idUzytku"].ToString();
                            string ofu = gmlRows[0]["OFU"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idUzytku;
                            xlsSheet.Cells[i + 2, 3].Value = ofu;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idUzytku;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : ofu;
#endif
                            xlsSheet.Cells[i + 2, 4].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 7].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_LokalSamodzielny
                    ------------------------------------------------------------------------------------ */
                    case "EGB_LokalSamodzielny":
                    case "EGB_Lokal_dok":
                    case "EGB_Lokal_numerEKW":
                    case "EGB_Lokal_numerKW":
                    case "EGB_Lokal_pomPrzynal":

                        fileSql = new StreamWriter(new FileStream(_errorDirectory + rodzajObiektu + "_errors.sql", FileMode.Create), Encoding.GetEncoding(1250));
                        fileSql.WriteLine("AND EW_LOK_IDG5(klokal.mslink) in (");

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idLokalu";
                        xlsSheet.Cells[1, 3].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 4].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "opis";
                        xlsSheet.Cells[1, 6].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 6])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami 
                            gmlRows = dataSetGML.Tables["EGB_LokalSamodzielny"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string idLokalu = gmlRows[0]["idLokalu"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idLokalu;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idLokalu;
#endif
                            xlsSheet.Cells[i + 2, 3].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 4].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 6].Value = err.Linia;
#if !DEMO
                            if (i != errorsCount - 1) fileSql.WriteLine("'" + idLokalu + "',"); else fileSql.WriteLine("'" + idLokalu + "'");
#endif
                            }

                        fileSql.WriteLine(")");
                        fileSql.Close();

                        break;

                    /* ------------------------------------------------------------------------------------
                       EGB_Malzenstwo
                    ------------------------------------------------------------------------------------ */
                    case "EGB_Malzenstwo":
                    case "EGB_Malzenstwo_dok":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "status";
                        xlsSheet.Cells[1, 3].Value = "osobaFizyczna2pierwszeImie";
                        xlsSheet.Cells[1, 4].Value = "osobaFizyczna2pierwszyCzlonNazwiska";
                        xlsSheet.Cells[1, 5].Value = "osobaFizyczna3pierwszeImie";
                        xlsSheet.Cells[1, 6].Value = "osobaFizyczna3pierwszyCzlonNazwiska";
                        xlsSheet.Cells[1, 7].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 8].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 9].Value = "opis";
                        xlsSheet.Cells[1, 10].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 10])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami
                            gmlRows = dataSetGML.Tables["EGB_Malzenstwo"].Select("lokalnyId = '" + err.LokalnyId + "'");

                            string status = Dict.EGB_StatusPodmiotuEwidType(gmlRows[0]["status"].ToString());



                            string osobaFizyczna2 = gmlRows[0]["osobaFizyczna2"].ToString();
                            string osobaFizyczna3 = gmlRows[0]["osobaFizyczna3"].ToString();

                            // wczytaj wiersz z atrybutami osobaFizyczna2
                            gmlRows = dataSetGML.Tables["EGB_OsobaFizyczna"].Select("lokalnyId = '" + osobaFizyczna2 + "'");

                            string osobaFizyczna2PierwszeImie = gmlRows[0]["pierwszeImie"].ToString();
                            string osobaFizyczna2PierwszyCzlonNazwiska = gmlRows[0]["pierwszyCzlonNazwiska"].ToString();

                            // wczytaj wiersz z atrybutami osobaFizyczna3
                            gmlRows = dataSetGML.Tables["EGB_OsobaFizyczna"].Select("lokalnyId = '" + osobaFizyczna3 + "'");

                            string osobaFizyczna3PierwszeImie = gmlRows[0]["pierwszeImie"].ToString();
                            string osobaFizyczna3PierwszyCzlonNazwiska = gmlRows[0]["pierwszyCzlonNazwiska"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = status;
                            xlsSheet.Cells[i + 2, 3].Value = osobaFizyczna2PierwszeImie;
                            xlsSheet.Cells[i + 2, 4].Value = osobaFizyczna2PierwszyCzlonNazwiska;
                            xlsSheet.Cells[i + 2, 5].Value = osobaFizyczna3PierwszeImie;
                            xlsSheet.Cells[i + 2, 6].Value = osobaFizyczna3PierwszyCzlonNazwiska;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : status;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : osobaFizyczna2PierwszeImie;
                            xlsSheet.Cells[i + 2, 4].Value = i > 2 ? "DEMO_" + i.ToString() : osobaFizyczna2PierwszyCzlonNazwiska;
                            xlsSheet.Cells[i + 2, 5].Value = i > 2 ? "DEMO_" + i.ToString() : osobaFizyczna3PierwszeImie;
                            xlsSheet.Cells[i + 2, 6].Value = i > 2 ? "DEMO_" + i.ToString() : osobaFizyczna3PierwszyCzlonNazwiska;
#endif
                            xlsSheet.Cells[i + 2, 7].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 8].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 9].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 10].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_ObiektTrwaleZwiazanyZBudynkiem
                    ------------------------------------------------------------------------------------ */
                    case "EGB_ObiektTrwaleZwiazanyZBudynkiem":
                    case "EGB_OTZZB_dok":

                        fileSql = new StreamWriter(new FileStream(_errorDirectory + rodzajObiektu + "_errors.sql", FileMode.Create), Encoding.GetEncoding(1250));
                        fileSql.WriteLine("AND EW_BUD_IDG5(kbudynek.mslink) in (");

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idBudynku";
                        xlsSheet.Cells[1, 3].Value = "rodzajObiektuZwiazanegoZBudynkiem";
                        xlsSheet.Cells[1, 4].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "opis";
                        xlsSheet.Cells[1, 7].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 7])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami
                            gmlRows = dataSetGML.Tables["EGB_ObiektTrwaleZwiazanyZBudynkiem"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string rodzajObiektuZwiazanegoZBudynkiem = Dict.EGB_RodzajObiektuZwiazanegoZBudynkiemType(gmlRows[0]["rodzajObiektuZwiazanegoZBudynkiem"].ToString());
                            string budynekZElementamiTrwaleZwiazanymi = gmlRows[0]["budynekZElementamiTrwaleZwiazanymi"].ToString();

                            // wczytaj wiersz z atrybutami budynku
                            gmlRows = dataSetGML.Tables["EGB_Budynek"].Select("lokalnyId = '" + budynekZElementamiTrwaleZwiazanymi + "'");
                            string idBudynku = gmlRows[0]["idBudynku"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idBudynku;
                            xlsSheet.Cells[i + 2, 3].Value = rodzajObiektuZwiazanegoZBudynkiem;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idBudynku;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : rodzajObiektuZwiazanegoZBudynkiem;
#endif
                            xlsSheet.Cells[i + 2, 4].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 7].Value = err.Linia;
#if !DEMO
                            if (i != errorsCount - 1) fileSql.WriteLine("'" + idBudynku + "',"); else fileSql.WriteLine("'" + idBudynku + "'");
#endif
                            }

                        fileSql.WriteLine(")");
                        fileSql.Close();

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_ObrebEwidencyjny
                    ------------------------------------------------------------------------------------ */
                    case "EGB_ObrebEwidencyjny":
                    case "EGB_Obreb_dok":
                    case "EGB_Obreb_pktGrObrebu":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idObrebu";
                        xlsSheet.Cells[1, 3].Value = "nazwaWlasna";
                        xlsSheet.Cells[1, 3].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 4].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "opis";
                        xlsSheet.Cells[1, 7].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 7])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami obrębu
                            gmlRows = dataSetGML.Tables["EGB_ObrebEwidencyjny"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string idObrebu = gmlRows[0]["idObrebu"].ToString();
                            string nazwaWlasna = gmlRows[0]["nazwaWlasna"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idObrebu;
                            xlsSheet.Cells[i + 2, 3].Value = nazwaWlasna;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idObrebu;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : nazwaWlasna;
#endif
                            xlsSheet.Cells[i + 2, 4].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 7].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_OperatTechniczny
                    ------------------------------------------------------------------------------------ */
                    case "EGB_OperatTechniczny":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "nazwaTworcy";
                        xlsSheet.Cells[1, 3].Value = "identyfikatorOperatuWgPZGIK";
                        xlsSheet.Cells[1, 4].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "opis";
                        xlsSheet.Cells[1, 7].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 7])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami
                            gmlRows = dataSetGML.Tables["EGB_OperatTechniczny"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string nazwaTworcy = gmlRows[0]["nazwaTworcy"].ToString();
                            string identyfikatorOperatuWgPzgik = gmlRows[0]["identyfikatorOperatuWgPZGIK"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = nazwaTworcy;
                            xlsSheet.Cells[i + 2, 3].Value = identyfikatorOperatuWgPzgik;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : nazwaTworcy;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : identyfikatorOperatuWgPzgik;
#endif
                            xlsSheet.Cells[i + 2, 4].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 7].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_OsobaFizyczna
                    ------------------------------------------------------------------------------------ */
                    case "EGB_OsobaFizyczna":
                    case "EGB_OsobaFizyczna_dok":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "pierwszeImie";
                        xlsSheet.Cells[1, 3].Value = "pierwszyCzlonNazwiska";
                        xlsSheet.Cells[1, 4].Value = "plec";
                        xlsSheet.Cells[1, 5].Value = "pesel";
                        xlsSheet.Cells[1, 6].Value = "drugiCzlonNazwiska";
                        xlsSheet.Cells[1, 7].Value = "drugieImie";
                        xlsSheet.Cells[1, 8].Value = "imieMatki";
                        xlsSheet.Cells[1, 9].Value = "imieOjca";
                        xlsSheet.Cells[1, 10].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 11].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 12].Value = "opis";
                        xlsSheet.Cells[1, 13].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 13])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami osoby
                            gmlRows = dataSetGML.Tables["EGB_OsobaFizyczna"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string pierwszeImie = gmlRows[0]["pierwszeImie"].ToString();
                            string pierwszyCzlonNazwiska = gmlRows[0]["pierwszyCzlonNazwiska"].ToString();
                            string plec = Dict.EGB_PlecType(gmlRows[0]["plec"].ToString());
                            string pesel = gmlRows[0]["pesel"].ToString();
                            string drugiCzlonNazwiska = gmlRows[0]["drugiCzlonNazwiska"].ToString();
                            string drugieImie = gmlRows[0]["drugieImie"].ToString();
                            string imieMatki = gmlRows[0]["imieMatki"].ToString();
                            string imieOjca = gmlRows[0]["imieOjca"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = pierwszeImie;
                            xlsSheet.Cells[i + 2, 3].Value = pierwszyCzlonNazwiska;
                            xlsSheet.Cells[i + 2, 4].Value = plec;
                            xlsSheet.Cells[i + 2, 5].Value = pesel;
                            xlsSheet.Cells[i + 2, 6].Value = drugiCzlonNazwiska;
                            xlsSheet.Cells[i + 2, 7].Value = drugieImie;
                            xlsSheet.Cells[i + 2, 8].Value = imieMatki;
                            xlsSheet.Cells[i + 2, 9].Value = imieOjca;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : pierwszeImie;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : pierwszyCzlonNazwiska;
                            xlsSheet.Cells[i + 2, 4].Value = i > 2 ? "DEMO_" + i.ToString() : plec;
                            xlsSheet.Cells[i + 2, 5].Value = i > 2 ? "DEMO_" + i.ToString() : pesel;
                            xlsSheet.Cells[i + 2, 6].Value = i > 2 ? "DEMO_" + i.ToString() : drugiCzlonNazwiska;
                            xlsSheet.Cells[i + 2, 7].Value = i > 2 ? "DEMO_" + i.ToString() : drugieImie;
                            xlsSheet.Cells[i + 2, 8].Value = i > 2 ? "DEMO_" + i.ToString() : imieMatki;
                            xlsSheet.Cells[i + 2, 9].Value = i > 2 ? "DEMO_" + i.ToString() : imieOjca;
#endif
                            xlsSheet.Cells[i + 2, 10].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 11].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 12].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 13].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_PodmiotGrupowy
                    ------------------------------------------------------------------------------------ */
                    case "EGB_PodmiotGrupowy":
                    case "EGB_PodGrup_dok":
                    case "EGB_PodGrup_instytucja":
                    case "EGB_PodGrup_mal":
                    case "EGB_PodGrup_osFiz":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idArkusza";
                        xlsSheet.Cells[1, 3].Value = "idArkusza";
                        xlsSheet.Cells[1, 4].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "opis";
                        xlsSheet.Cells[1, 7].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 7])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami
                            gmlRows = dataSetGML.Tables["EGB_PodmiotGrupowy"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string status = Dict.EGB_StatusPodmiotuEwidType(gmlRows[0]["status"].ToString());
                            string nazwaPelna = gmlRows[0]["nazwaPelna"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = status;
                            xlsSheet.Cells[i + 2, 3].Value = nazwaPelna;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : status;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : nazwaPelna;
#endif
                            xlsSheet.Cells[i + 2, 4].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 7].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_PomieszczeniePrzynalezneDoLokalu
                    ------------------------------------------------------------------------------------ */
                    case "EGB_PomieszczeniePrzynalezneDoLokalu":
                    case "EGB_PomPrzynDoLokalu_dok":

                        fileSql = new StreamWriter(new FileStream(_errorDirectory + rodzajObiektu + "_errors.sql", FileMode.Create), Encoding.GetEncoding(1250));
                        fileSql.WriteLine("AND EW_BUD_IDG5(kbudynek.mslink) in (");

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idBudynku";
                        xlsSheet.Cells[1, 3].Value = "rodzajPomieszczeniaPrzynaleznego";
                        xlsSheet.Cells[1, 4].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "opis";
                        xlsSheet.Cells[1, 7].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 7])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami
                            gmlRows = dataSetGML.Tables["EGB_PomieszczeniePrzynalezneDoLokalu"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string rodzajPomieszczeniaPrzynaleznego = Dict.EGB_RodzajPomieszczeniaPrzynaleznegoDoLokaluType(gmlRows[0]["rodzajPomieszczeniaPrzynaleznego"].ToString());
                            string lokalizacjaPomieszczeniaPrzynaleznego = gmlRows[0]["lokalizacjaPomieszczeniaPrzynaleznego"].ToString();

                            // wczytaj wiersz z atrybutami budynku
                            gmlRows = dataSetGML.Tables["EGB_Budynek"].Select("lokalnyId = '" + lokalizacjaPomieszczeniaPrzynaleznego + "'");

                            string idBudynku = "BRAK!";

                            if (gmlRows.Length != 0) idBudynku = gmlRows[0]["idBudynku"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idBudynku;
                            xlsSheet.Cells[i + 2, 3].Value = rodzajPomieszczeniaPrzynaleznego;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idBudynku;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : rodzajPomieszczeniaPrzynaleznego;
#endif
                            xlsSheet.Cells[i + 2, 4].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 7].Value = err.Linia;
#if !DEMO
                            if (i != errorsCount - 1) fileSql.WriteLine("'" + idBudynku + "',"); else fileSql.WriteLine("'" + idBudynku + "'");
#endif
                        }

                        fileSql.WriteLine(")");
                        fileSql.Close();

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_PunktGraniczny
                    ------------------------------------------------------------------------------------ */
                    case "EGB_PunktGraniczny":
                    case "EGB_PktGraniczny_dok":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idPunktu";
                        xlsSheet.Cells[1, 3].Value = "oznWMaterialeZrodlowym";
                        xlsSheet.Cells[1, 4].Value = "posX";
                        xlsSheet.Cells[1, 5].Value = "posY";
                        xlsSheet.Cells[1, 6].Value = "dodatkoweInformacje";
                        xlsSheet.Cells[1, 7].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 8].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 9].Value = "opis";
                        xlsSheet.Cells[1, 10].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 10])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami 
                            gmlRows = dataSetGML.Tables["EGB_PunktGraniczny"].Select("lokalnyId = '" + err.LokalnyId + "'");

                            string idPunktu = gmlRows[0]["idPunktu"].ToString();
                            string oznWMaterialeZrodlowym = gmlRows[0]["oznWMaterialeZrodlowym"].ToString();
                            string posX = gmlRows[0]["posX"].ToString();
                            string posY = gmlRows[0]["posY"].ToString();
                            string dodatkoweInformacje = gmlRows[0]["dodatkoweInformacje"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idPunktu;
                            xlsSheet.Cells[i + 2, 3].Value = oznWMaterialeZrodlowym;
                            xlsSheet.Cells[i + 2, 4].Value = posX;
                            xlsSheet.Cells[i + 2, 5].Value = posY;
                            xlsSheet.Cells[i + 2, 6].Value = dodatkoweInformacje;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idPunktu;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : oznWMaterialeZrodlowym;
                            xlsSheet.Cells[i + 2, 4].Value = i > 2 ? "DEMO_" + i.ToString() : posX;
                            xlsSheet.Cells[i + 2, 5].Value = i > 2 ? "DEMO_" + i.ToString() : posY;
                            xlsSheet.Cells[i + 2, 6].Value = i > 2 ? "DEMO_" + i.ToString() : dodatkoweInformacje;
#endif
                            xlsSheet.Cells[i + 2, 7].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 8].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 9].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 10].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_UdzialDzierzawy
                    ------------------------------------------------------------------------------------ */
                    case "EGB_UdzialDzierzawy":
                    case "EGB_UdzialDzierzawy_dok":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idDzierzawy";
                        xlsSheet.Cells[1, 3].Value = "udzial";
                        xlsSheet.Cells[1, 4].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "opis";
                        xlsSheet.Cells[1, 7].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 7])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami
                            gmlRows = dataSetGML.Tables["EGB_UdzialDzierzawy"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string udzial = gmlRows[0]["udzial"].ToString();
                            string przedmiotUdzialuDzierzawy = gmlRows[0]["przedmiotUdzialuDzierzawy"].ToString();

                            // wczytaj wiersz z atrybutami dzierżawy
                            gmlRows = dataSetGML.Tables["EGB_Dzierzawa"].Select("lokalnyId = '" + przedmiotUdzialuDzierzawy + "'");
                            string idDzierzawy = gmlRows[0]["idDzierzawy"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idDzierzawy;
                            xlsSheet.Cells[i + 2, 3].Value = udzial;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idDzierzawy;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : udzial;
#endif
                            xlsSheet.Cells[i + 2, 4].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 7].Value = err.Linia;
                        }

                        break;


                    /* ------------------------------------------------------------------------------------
                       EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST
                    ------------------------------------------------------------------------------------ */
                    case "EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST":
                    case "EGB_UdzGosNiSPLubJST_dok":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idJednostkiRejestrowej";
                        xlsSheet.Cells[1, 3].Value = "rodzajUprawnien";
                        xlsSheet.Cells[1, 4].Value = "podgrupaRej";
                        xlsSheet.Cells[1, 5].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 7].Value = "opis";
                        xlsSheet.Cells[1, 8].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 8])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST
                            gmlRows = dataSetGML.Tables["EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string przedmiotUdzialuGz1 = gmlRows[0]["przedmiotUdzialuGZ1"].ToString();
                            string idPrzedmiotUdzialuGz1 = gmlRows[0]["idPrzedmiotUdzialuGZ1"].ToString();
                            string rodzajUprawnien = Dict.EGB_RodzajUprawnienType(gmlRows[0]["rodzajUprawnien"].ToString());
                            string podgrupaRej = gmlRows[0]["podgrupaRej"].ToString();

                            // wczytaj wiersz z atrybutami jednostki rejestrowej
                            switch (przedmiotUdzialuGz1)
                            {
                                case "egb:JRG":
                                    gmlRows = dataSetGML.Tables["EGB_JednostkaRejestrowaGruntow"].Select("lokalnyId = '" + idPrzedmiotUdzialuGz1 + "'");
                                    break;
                                case "egb:JRB":
                                    gmlRows = dataSetGML.Tables["EGB_JednostkaRejestrowaBudynkow"].Select("lokalnyId = '" + idPrzedmiotUdzialuGz1 + "'");
                                    break;
                                case "egb:JRL":
                                    gmlRows = dataSetGML.Tables["EGB_JednostkaRejestrowaLokali"].Select("lokalnyId = '" + idPrzedmiotUdzialuGz1 + "'");
                                    break;
                            }

                            string idJednostkiRejestrowej = "brak jednostki w pliku";
                            if (gmlRows.Length != 0) idJednostkiRejestrowej = gmlRows[0]["idJednostkiRejestrowej"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idJednostkiRejestrowej;
                            xlsSheet.Cells[i + 2, 3].Value = rodzajUprawnien;
                            xlsSheet.Cells[i + 2, 4].Value = podgrupaRej;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idJednostkiRejestrowej;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : rodzajUprawnien;
                            xlsSheet.Cells[i + 2, 4].Value = i > 2 ? "DEMO_" + i.ToString() : podgrupaRej;
#endif
                            xlsSheet.Cells[i + 2, 5].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 7].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 8].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                    EGB_UdzialWeWladaniuNieruchomosciaSPLubJST
                    ------------------------------------------------------------------------------------ */
                    case "EGB_UdzialWeWladaniuNieruchomosciaSPLubJST":
                    case "EGB_UdzWeWladNiSPJST_dok":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "idJednostkiRejestrowej";
                        xlsSheet.Cells[1, 3].Value = "rodzajWladania";
                        xlsSheet.Cells[1, 4].Value = "podgrupaRej";
                        xlsSheet.Cells[1, 5].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 7].Value = "opis";
                        xlsSheet.Cells[1, 8].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 8])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST
                            gmlRows = dataSetGML.Tables["EGB_UdzialWeWladaniuNieruchomosciaSPLubJST"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string przedmiotUdzialuWladania = gmlRows[0]["przedmiotUdzialuWladania"].ToString();
                            string idPrzedmiotUdzialuWladania = gmlRows[0]["idPrzedmiotUdzialuWladania"].ToString();
                            string rodzajWladania = Dict.EGB_RodzajWladaniaType(gmlRows[0]["rodzajWladania"].ToString());
                            string podgrupaRej = gmlRows[0]["podgrupaRej"].ToString();

                            // wczytaj wiersz z atrybutami jednostki rejestrowej
                            switch (przedmiotUdzialuWladania)
                            {
                                case "egb:JRG":
                                    gmlRows = dataSetGML.Tables["EGB_JednostkaRejestrowaGruntow"].Select("lokalnyId = '" + idPrzedmiotUdzialuWladania + "'");
                                    break;
                                case "egb:JRB":
                                    gmlRows = dataSetGML.Tables["EGB_JednostkaRejestrowaBudynkow"].Select("lokalnyId = '" + idPrzedmiotUdzialuWladania + "'");
                                    break;
                                case "egb:JRL":
                                    gmlRows = dataSetGML.Tables["EGB_JednostkaRejestrowaLokali"].Select("lokalnyId = '" + idPrzedmiotUdzialuWladania + "'");
                                    break;
                            }

                            string idJednostkiRejestrowej = "brak jednostki w pliku";
                            if (gmlRows.Length != 0) idJednostkiRejestrowej = gmlRows[0]["idJednostkiRejestrowej"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = idJednostkiRejestrowej;
                            xlsSheet.Cells[i + 2, 3].Value = rodzajWladania;
                            xlsSheet.Cells[i + 2, 4].Value = podgrupaRej;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : idJednostkiRejestrowej;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : rodzajWladania;
                            xlsSheet.Cells[i + 2, 4].Value = i > 2 ? "DEMO_" + i.ToString() : podgrupaRej;
#endif
                            xlsSheet.Cells[i + 2, 5].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 7].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 8].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_UdzialWlasnosci
                    ------------------------------------------------------------------------------------ */
                    case "EGB_UdzialWlasnosci":
                    case "EGB_UdzWlas_dok":
                    case "EGB_UdzWlas_GoNiSPLubJST":
                    case "EGB_UdzWlas_udzWeWladGr":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "przedmiotUdzialuWlasnosci";
                        xlsSheet.Cells[1, 3].Value = "idJednostkiRejestrowej";
                        xlsSheet.Cells[1, 4].Value = "rodzajPrawa";
                        xlsSheet.Cells[1, 5].Value = "licznikUlamkaOkreslajacegoWartoscUdzialu";
                        xlsSheet.Cells[1, 6].Value = "mianownikUlamkaOkreslajacegoWartoscUdzialu";
                        xlsSheet.Cells[1, 7].Value = "podgrupaRej";
                        xlsSheet.Cells[1, 8].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 9].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 10].Value = "opis";
                        xlsSheet.Cells[1, 11].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 11])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }
                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami
                            gmlRows = dataSetGML.Tables["EGB_UdzialWlasnosci"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string rodzajPrawa = Dict.EGB_RodzajWladaniaType(gmlRows[0]["rodzajPrawa"].ToString());
                            string licznikUlamkaOkreslajacegoWartoscUdzialu = gmlRows[0]["licznikUlamkaOkreslajacegoWartoscUdzialu"].ToString();
                            string mianownikUlamkaOkreslajacegoWartoscUdzialu = gmlRows[0]["mianownikUlamkaOkreslajacegoWartoscUdzialu"].ToString();
                            string podgrupaRej = gmlRows[0]["podgrupaRej"].ToString();
                            string przedmiotUdzialuWlasnosci = gmlRows[0]["przedmiotUdzialuWlasnosci"].ToString();
                            string idPrzedmiotUdzialuWlasnosci = gmlRows[0]["idPrzedmiotUdzialuWlasnosci"].ToString();

                            switch (przedmiotUdzialuWlasnosci)
                            {
                                case "egb:JRL":
                                    gmlRows = dataSetGML.Tables["EGB_JednostkaRejestrowaLokali"].Select("lokalnyId = '" + idPrzedmiotUdzialuWlasnosci + "'");
                                    break;
                                case "egb:JRG":
                                    gmlRows = dataSetGML.Tables["EGB_JednostkaRejestrowaGruntow"].Select("lokalnyId = '" + idPrzedmiotUdzialuWlasnosci + "'");
                                    break;
                                case "egb:JRB":
                                    gmlRows = dataSetGML.Tables["EGB_JednostkaRejestrowaBudynkow"].Select("lokalnyId = '" + idPrzedmiotUdzialuWlasnosci + "'");
                                    break;
                            }

                            string idJednostkiRejestrowej = "brak jednostki w pliku";
                            if (gmlRows.Length != 0) idJednostkiRejestrowej = gmlRows[0]["idJednostkiRejestrowej"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = przedmiotUdzialuWlasnosci;
                            xlsSheet.Cells[i + 2, 3].Value = idJednostkiRejestrowej;
                            xlsSheet.Cells[i + 2, 4].Value = rodzajPrawa;
                            xlsSheet.Cells[i + 2, 5].Value = licznikUlamkaOkreslajacegoWartoscUdzialu;
                            xlsSheet.Cells[i + 2, 6].Value = mianownikUlamkaOkreslajacegoWartoscUdzialu;
                            xlsSheet.Cells[i + 2, 7].Value = podgrupaRej;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : przedmiotUdzialuWlasnosci;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : idJednostkiRejestrowej;
                            xlsSheet.Cells[i + 2, 4].Value = i > 2 ? "DEMO_" + i.ToString() : rodzajPrawa;
                            xlsSheet.Cells[i + 2, 5].Value = i > 2 ? "DEMO_" + i.ToString() : licznikUlamkaOkreslajacegoWartoscUdzialu;
                            xlsSheet.Cells[i + 2, 6].Value = i > 2 ? "DEMO_" + i.ToString() : mianownikUlamkaOkreslajacegoWartoscUdzialu;
                            xlsSheet.Cells[i + 2, 7].Value = i > 2 ? "DEMO_" + i.ToString() : podgrupaRej;
#endif
                            xlsSheet.Cells[i + 2, 8].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 9].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 10].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 11].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_ZarzadSpolkiWspolnotyGruntowej
                    ------------------------------------------------------------------------------------ */
                    case "EGB_ZarzadSpolkiWspolnotyGruntowej":
                    case "EGB_ZarzSpolWspGrunt_dok":
                    case "EGB_ZarzSpolWspGrunt_osF":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "nazwaSpolkiPowolanejDoZarzadzaniaWspolnotaGruntowa";
                        xlsSheet.Cells[1, 3].Value = "wspolnotaGruntowa";
                        xlsSheet.Cells[1, 4].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 6].Value = "opis";
                        xlsSheet.Cells[1, 7].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 7])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami 
                            gmlRows = dataSetGML.Tables["EGB_ZarzadSpolkiWspolnotyGruntowej"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string nazwaSpolkiPowolanejDoZarzadzaniaWspolnotaGruntowa = gmlRows[0]["nazwaSpolkiPowolanejDoZarzadzaniaWspolnotaGruntowa"].ToString();
                            string wspolnotaGruntowa = gmlRows[0]["wspolnotaGruntowa"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = nazwaSpolkiPowolanejDoZarzadzaniaWspolnotaGruntowa;
                            xlsSheet.Cells[i + 2, 3].Value = wspolnotaGruntowa;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : nazwaSpolkiPowolanejDoZarzadzaniaWspolnotaGruntowa;
                            xlsSheet.Cells[i + 2, 3].Value = i > 2 ? "DEMO_" + i.ToString() : wspolnotaGruntowa;
#endif
                            xlsSheet.Cells[i + 2, 4].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 6].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 7].Value = err.Linia;
                        }

                        break;

                    /* ------------------------------------------------------------------------------------
                        EGB_Zmiana
                    ------------------------------------------------------------------------------------ */
                    case "EGB_Zmiana":
                    case "EGB_Zmiana_dok":
                    case "EGB_Zmiana_OT":

                        xlsSheet.Cells[1, 1].Value = "lokalnyId";
                        xlsSheet.Cells[1, 2].Value = "nrZmiany";
                        xlsSheet.Cells[1, 3].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 4].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 5].Value = "opis";
                        xlsSheet.Cells[1, 6].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 6])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            // wczytaj wiersz z atrybutami 
                            gmlRows = dataSetGML.Tables["EGB_Zmiana"].Select("lokalnyId = '" + err.LokalnyId + "'");
                            string nrZmiany = gmlRows[0]["nrZmiany"].ToString();
#if !DEMO
                            xlsSheet.Cells[i + 2, 1].Value = err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = nrZmiany;
#else
                            xlsSheet.Cells[i + 2, 1].Value = i > 2 ? "DEMO_" + i.ToString() : err.LokalnyId;
                            xlsSheet.Cells[i + 2, 2].Value = i > 2 ? "DEMO_" + i.ToString() : nrZmiany;
#endif
                            xlsSheet.Cells[i + 2, 3].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 4].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 5].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 6].Value = err.Linia;
                        }

                        break;

                    case "inne":

                        xlsSheet.Cells[1, 1].Value = "wynikWeryfikacji";
                        xlsSheet.Cells[1, 2].Value = "szczegolyWeryfikacji";
                        xlsSheet.Cells[1, 3].Value = "opis";
                        xlsSheet.Cells[1, 4].Value = "linia";

                        using (var range = xlsSheet.Cells[1, 1, 1, 4])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                            range.Style.Font.Color.SetColor(Color.Black);
                            range.AutoFilter = true;
                        }

                        errorsCount = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + rodzajObiektu + "'").Length;

                        for (int i = 0; i < errorsCount; i++)
                        {
                            // pobierz z csv kolejno wiersz z błędem
                            CsvError err = GetCsvError(i, rodzajObiektu);

                            xlsSheet.Cells[i + 2, 1].Value = err.WynikWeryfikacji;
                            xlsSheet.Cells[i + 2, 2].Value = err.SzczegolyWeryfikacji;
                            xlsSheet.Cells[i + 2, 3].Value = err.Opis;
                            xlsSheet.Cells[i + 2, 4].Value = err.Linia;
                        }

                        break;

                    default:
                        MessageBox.Show(rodzajObiektu);
                        break;

                } // switch (rodzajObiektu)

                xlsSheet.View.FreezePanes(2, 1);
                xlsSheet.Cells.Style.Font.Size = 10;
                xlsSheet.Cells.AutoFitColumns(0);
                xlsWorkbook.Save();

            } //foreach (DataRow rowObiektyBlad in distinctValues.Rows)

        }

        // ----------------------------------------------------------------------------------------
        // funkcja obsługująca postęp wczytywania pliku CSV
        private void BW_bwSaveErrorsProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            stbMainProgress.Value = e.ProgressPercentage;
        }

        // ----------------------------------------------------------------------------------------
        // funkcja wywoływana po zakończeniu wczytywania pliku CSV do CSV
        private void BW_bwSaveErrorsRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //this.tbProgress.Text = "Canceled!";
                MessageBox.Show(e.Error.Message, @"Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Error != null)
            {
                //this.tbProgress.Text = ("Error: " + e.Error.Message);
                MessageBox.Show(e.Error.Message, @"Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            { // DONE

                UseWaitCursor = false;

                stbMainStatus.Text = @"Zapisano dane o błędach.";

                //System.Media.SystemSounds.Beep.Play();

                MessageBox.Show(@"Zapisano dane o błędach", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                //Process.Start("explorer.exe", _errorDirectory);

            }
        }

        // ----------------------------------------------------------------------------------------
        // czyszczenie tabel GML i etykiet na formularzu
        private void ClearGmlData()
        {
            dataSetGML.Tables["BT_Dokument"].Rows.Clear();
            lblBT_Dokument.Text = @"BT_Dokument: " + dataSetGML.Tables["BT_Dokument"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_Adres"].Rows.Clear();
            lblEGB_Adres.Text = @"EGB_Adres: " + dataSetGML.Tables["EGB_Adres"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_ArkuszEwidencyjny"].Rows.Clear();
            lblEGB_ArkuszEwidencyjny.Text = @"EGB_ArkuszEwidencyjny: " + dataSetGML.Tables["EGB_ArkuszEwidencyjny"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_BlokBudynku"].Rows.Clear();
            lblEGB_BlokBudynku.Text = @"EGB_BlokBudynku: " + dataSetGML.Tables["EGB_BlokBudynku"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_Budynek"].Rows.Clear();
            lblEGB_Budynek.Text = @"EGB_Budynek: " + dataSetGML.Tables["EGB_Budynek"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_DzialkaEwidencyjna"].Rows.Clear();
            lblEGB_DzialkaEwidencyjna.Text = @"EGB_DzialkaEwidencyjna: " + dataSetGML.Tables["EGB_DzialkaEwidencyjna"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_Dzierzawa"].Rows.Clear();
            lblEGB_Dzierzawa.Text = @"EGB_Dzierzawa: " + dataSetGML.Tables["EGB_Dzierzawa"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_Instytucja"].Rows.Clear();
            lblEGB_Instytucja.Text = @"EGB_Instytucja: " + dataSetGML.Tables["EGB_Instytucja"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_JednostkaEwidencyjna"].Rows.Clear();
            lblEGB_JednostkaEwidencyjna.Text = @"EGB_JednostkaEwidencyjna: " + dataSetGML.Tables["EGB_JednostkaEwidencyjna"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_JednostkaRejestrowaBudynkow"].Rows.Clear();
            lblEGB_JednostkaRejestrowaBudynkow.Text = @"EGB_JednostkaRejestrowaBudynkow: " + dataSetGML.Tables["EGB_JednostkaRejestrowaBudynkow"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_JednostkaRejestrowaGruntow"].Rows.Clear();
            lblEGB_JednostkaRejestrowaGruntow.Text = @"EGB_JednostkaRejestrowaGruntow: " + dataSetGML.Tables["EGB_JednostkaRejestrowaGruntow"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_JednostkaRejestrowaLokali"].Rows.Clear();
            lblEGB_JednostkaRejestrowaLokali.Text = @"EGB_JednostkaRejestrowaLokali: " + dataSetGML.Tables["EGB_JednostkaRejestrowaLokali"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_Klasouzytek"].Rows.Clear();
            lblEGB_Klasouzytek.Text = @"EGB_Klasouzytek: " + dataSetGML.Tables["EGB_Klasouzytek"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_KonturKlasyfikacyjny"].Rows.Clear();
            lblEGB_KonturKlasyfikacyjny.Text = @"EGB_KonturKlasyfikacyjny: " + dataSetGML.Tables["EGB_KonturKlasyfikacyjny"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_KonturUzytkuGruntowego"].Rows.Clear();
            lblEGB_KonturUzytkuGruntowego.Text = @"EGB_KonturUzytkuGruntowego: " + dataSetGML.Tables["EGB_KonturUzytkuGruntowego"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_LokalSamodzielny"].Rows.Clear();
            lblEGB_LokalSamodzielny.Text = @"EGB_LokalSamodzielny: " + dataSetGML.Tables["EGB_LokalSamodzielny"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_Malzenstwo"].Rows.Clear();
            lblEGB_Malzenstwo.Text = @"EGB_Malzenstwo: " + dataSetGML.Tables["EGB_Malzenstwo"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_ObiektTrwaleZwiazanyZBudynkiem"].Rows.Clear();
            lblEGB_ObiektTrwaleZwiazanyZBudynkiem.Text = @"EGB_ObiektTrwaleZwiazanyZBudynkiem: " + dataSetGML.Tables["EGB_ObiektTrwaleZwiazanyZBudynkiem"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_ObrebEwidencyjny"].Rows.Clear();
            lblEGB_ObrebEwidencyjny.Text = @"EGB_ObrebEwidencyjny: " + dataSetGML.Tables["EGB_ObrebEwidencyjny"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_OperatTechniczny"].Rows.Clear();
            lblEGB_OperatTechniczny.Text = @"EGB_OperatTechniczny: " + dataSetGML.Tables["EGB_OperatTechniczny"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_OsobaFizyczna"].Rows.Clear();
            lblEGB_OsobaFizyczna.Text = @"EGB_OsobaFizyczna: " + dataSetGML.Tables["EGB_OsobaFizyczna"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_PodmiotGrupowy"].Rows.Clear();
            lblEGB_PodmiotGrupowy.Text = @"EGB_PodmiotGrupowy: " + dataSetGML.Tables["EGB_PodmiotGrupowy"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_PomieszczeniePrzynalezneDoLokalu"].Rows.Clear();
            lblEGB_PomieszczeniePrzynalezneDoLokalu.Text = @"EGB_PomieszczeniePrzynalezneDoLokalu: " + dataSetGML.Tables["EGB_PomieszczeniePrzynalezneDoLokalu"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_PunktGraniczny"].Rows.Clear();
            lblEGB_PunktGraniczny.Text = @"EGB_PunktGraniczny: " + dataSetGML.Tables["EGB_PunktGraniczny"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_UdzialDzierzawy"].Rows.Clear();
            lblEGB_UdzialDzierzawy.Text = @"EGB_UdzialDzierzawy: " + dataSetGML.Tables["EGB_UdzialDzierzawy"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST"].Rows.Clear();
            lblEGB_UdzialGospodarowaniaNieruchomosciaSPLubJST.Text = @"EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST: " + dataSetGML.Tables["EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_UdzialWeWladaniuNieruchomosciaSPLubJST"].Rows.Clear();
            lblEGB_UdzialWeWladaniuNieruchomosciaSPLubJST.Text = @"EGB_UdzialWeWladaniuNieruchomosciaSPLubJST: " + dataSetGML.Tables["EGB_UdzialWeWladaniuNieruchomosciaSPLubJST"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_UdzialWlasnosci"].Rows.Clear();
            lblEGB_UdzialWlasnosci.Text = @"EGB_UdzialWlasnosci: " + dataSetGML.Tables["EGB_UdzialWlasnosci"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_ZarzadSpolkiWspolnotyGruntowej"].Rows.Clear();
            lblEGB_ZarzadSpolkiWspolnotyGruntowej.Text = @"EGB_ZarzadSpolkiWspolnotyGruntowej: " + dataSetGML.Tables["EGB_ZarzadSpolkiWspolnotyGruntowej"].Rows.Count.ToString();

            dataSetGML.Tables["EGB_Zmiana"].Rows.Clear();
            lblEGB_Zmiana.Text = @"EGB_Zmiana: " + dataSetGML.Tables["EGB_Zmiana"].Rows.Count.ToString();

            //dataSetGML.Tables["klasouzytekWGranicachDzialki"].Rows.Clear();

        }

        // ----------------------------------------------------------------------------------------
        // czyszczenie tabel raportu CSV i etykiet na formularzu
        private void ClearCsvData()
        {
            dataSetCSV.Tables["Raport"].Rows.Clear();
            lblLiczbaRekordow.Text = @"Liczba rekordów: " + dataSetCSV.Tables["Raport"].Rows.Count.ToString();
        }

        // pobarnie danych o określonym błedzie
        private CsvError GetCsvError(int id, string obiekt)
        {
            CsvError err;

            DataRow[] csvRows = dataSetCSV.Tables["Raport"].Select("rodzajObiektu = '" + obiekt + "'");

            // pobierz z csv kolejno szukaną wartość
            //err.TypWeryfikacji = csvRows[id]["typWeryfikacji"].ToString();
            //err.RodzajWeryfikacji = csvRows[id]["rodzajWeryfikacji"].ToString();
            err.SzczegolyWeryfikacji = csvRows[id]["szczegolyWeryfikacji"].ToString();
            err.WynikWeryfikacji = csvRows[id]["wynikWeryfikacji"].ToString();
            //err.IdObiektu = csvRows[id]["idObiektu"].ToString();
            err.LokalnyId = csvRows[id]["lokalnyId"].ToString();
            //err.PrzestrzenNazw = csvRows[id]["przestrzenNazw"].ToString();
            err.Opis = csvRows[id]["opis"].ToString();
            err.Linia = csvRows[id]["linia"].ToString();

            return err;
        }

        // ======================================
        //              OBSŁUGA MENU
        // ======================================
        private void WyjścieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void WybierzGMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ButtonOpenGML_Click(sender, e);
        }

        private void WybierzPlikCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ButtonOpenCSV_Click(sender, e);
        }

        private void OProgramieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAbout frm = new FrmAbout();

            frm.ShowDialog(this);
        }

    } // class frmMain : Form

}
