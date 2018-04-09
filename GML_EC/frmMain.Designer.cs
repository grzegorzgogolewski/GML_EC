namespace GML_EC
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOpenGML = new System.Windows.Forms.Button();
            this.gBoxGML = new System.Windows.Forms.GroupBox();
            this.btnSaveGMLData = new System.Windows.Forms.Button();
            this.btnReadGML = new System.Windows.Forms.Button();
            this.txtInputGML = new System.Windows.Forms.TextBox();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.dataSetGML = new System.Data.DataSet();
            this.gBoxCSV = new System.Windows.Forms.GroupBox();
            this.lblLiczbaRekordow = new System.Windows.Forms.Label();
            this.chboxKrytyczne = new System.Windows.Forms.CheckBox();
            this.btnSaveErrorList = new System.Windows.Forms.Button();
            this.btnReadCSV = new System.Windows.Forms.Button();
            this.txtInputCSV = new System.Windows.Forms.TextBox();
            this.btnOpenCSV = new System.Windows.Forms.Button();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wybierzGMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wybierzPlikCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wyjścieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pomocToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oProgramieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataSetCSV = new System.Data.DataSet();
            this.lblBT_Dokument = new System.Windows.Forms.Label();
            this.gboxStatystykaGML = new System.Windows.Forms.GroupBox();
            this.lblEGB_PomieszczeniePrzynalezneDoLokalu = new System.Windows.Forms.Label();
            this.lblEGB_PunktGraniczny = new System.Windows.Forms.Label();
            this.lblEGB_UdzialDzierzawy = new System.Windows.Forms.Label();
            this.lblEGB_UdzialGospodarowaniaNieruchomosciaSPLubJST = new System.Windows.Forms.Label();
            this.lblEGB_UdzialWeWladaniuNieruchomosciaSPLubJST = new System.Windows.Forms.Label();
            this.lblEGB_UdzialWlasnosci = new System.Windows.Forms.Label();
            this.lblEGB_ZarzadSpolkiWspolnotyGruntowej = new System.Windows.Forms.Label();
            this.lblEGB_Zmiana = new System.Windows.Forms.Label();
            this.lblEGB_PodmiotGrupowy = new System.Windows.Forms.Label();
            this.lblEGB_OsobaFizyczna = new System.Windows.Forms.Label();
            this.lblEGB_OperatTechniczny = new System.Windows.Forms.Label();
            this.lblEGB_ObrebEwidencyjny = new System.Windows.Forms.Label();
            this.lblEGB_ObiektTrwaleZwiazanyZBudynkiem = new System.Windows.Forms.Label();
            this.lblEGB_Malzenstwo = new System.Windows.Forms.Label();
            this.lblEGB_LokalSamodzielny = new System.Windows.Forms.Label();
            this.lblEGB_Instytucja = new System.Windows.Forms.Label();
            this.lblEGB_JednostkaEwidencyjna = new System.Windows.Forms.Label();
            this.lblEGB_JednostkaRejestrowaBudynkow = new System.Windows.Forms.Label();
            this.lblEGB_JednostkaRejestrowaGruntow = new System.Windows.Forms.Label();
            this.lblEGB_JednostkaRejestrowaLokali = new System.Windows.Forms.Label();
            this.lblEGB_Klasouzytek = new System.Windows.Forms.Label();
            this.lblEGB_KonturKlasyfikacyjny = new System.Windows.Forms.Label();
            this.lblEGB_KonturUzytkuGruntowego = new System.Windows.Forms.Label();
            this.lblEGB_Dzierzawa = new System.Windows.Forms.Label();
            this.lblEGB_DzialkaEwidencyjna = new System.Windows.Forms.Label();
            this.lblEGB_Budynek = new System.Windows.Forms.Label();
            this.lblEGB_BlokBudynku = new System.Windows.Forms.Label();
            this.lblEGB_ArkuszEwidencyjny = new System.Windows.Forms.Label();
            this.lblEGB_Adres = new System.Windows.Forms.Label();
            this.stbMainStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.stbMainProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.stbMain = new System.Windows.Forms.StatusStrip();
            this.lblLicencja = new System.Windows.Forms.Label();
            this.lblClient = new System.Windows.Forms.Label();
            this.pictureBoxClient = new System.Windows.Forms.PictureBox();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.gBoxGML.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetGML)).BeginInit();
            this.gBoxCSV.SuspendLayout();
            this.mnuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCSV)).BeginInit();
            this.gboxStatystykaGML.SuspendLayout();
            this.stbMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxClient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOpenGML
            // 
            this.btnOpenGML.Location = new System.Drawing.Point(6, 45);
            this.btnOpenGML.Name = "btnOpenGML";
            this.btnOpenGML.Size = new System.Drawing.Size(130, 30);
            this.btnOpenGML.TabIndex = 0;
            this.btnOpenGML.Text = "Otwórz &GML";
            this.btnOpenGML.UseVisualStyleBackColor = true;
            this.btnOpenGML.Click += new System.EventHandler(this.ButtonOpenGML_Click);
            // 
            // gBoxGML
            // 
            this.gBoxGML.Controls.Add(this.btnSaveGMLData);
            this.gBoxGML.Controls.Add(this.btnReadGML);
            this.gBoxGML.Controls.Add(this.txtInputGML);
            this.gBoxGML.Controls.Add(this.btnOpenGML);
            this.gBoxGML.Location = new System.Drawing.Point(11, 27);
            this.gBoxGML.Name = "gBoxGML";
            this.gBoxGML.Size = new System.Drawing.Size(861, 88);
            this.gBoxGML.TabIndex = 1;
            this.gBoxGML.TabStop = false;
            this.gBoxGML.Text = "GML";
            // 
            // btnSaveGMLData
            // 
            this.btnSaveGMLData.Location = new System.Drawing.Point(278, 45);
            this.btnSaveGMLData.Name = "btnSaveGMLData";
            this.btnSaveGMLData.Size = new System.Drawing.Size(130, 30);
            this.btnSaveGMLData.TabIndex = 3;
            this.btnSaveGMLData.Text = "Zapisz dane z GML";
            this.btnSaveGMLData.UseVisualStyleBackColor = true;
            this.btnSaveGMLData.Click += new System.EventHandler(this.ButtonSaveGMLData_Click);
            // 
            // btnReadGML
            // 
            this.btnReadGML.Location = new System.Drawing.Point(142, 45);
            this.btnReadGML.Name = "btnReadGML";
            this.btnReadGML.Size = new System.Drawing.Size(130, 30);
            this.btnReadGML.TabIndex = 2;
            this.btnReadGML.Text = "Czytaj GML";
            this.btnReadGML.UseVisualStyleBackColor = true;
            this.btnReadGML.Click += new System.EventHandler(this.ButtonReadGML_Click);
            // 
            // txtInputGML
            // 
            this.txtInputGML.Location = new System.Drawing.Point(6, 19);
            this.txtInputGML.Name = "txtInputGML";
            this.txtInputGML.ReadOnly = true;
            this.txtInputGML.Size = new System.Drawing.Size(849, 20);
            this.txtInputGML.TabIndex = 2;
            // 
            // dlgOpen
            // 
            this.dlgOpen.RestoreDirectory = true;
            // 
            // dataSetGML
            // 
            this.dataSetGML.DataSetName = "dataSetGML";
            // 
            // gBoxCSV
            // 
            this.gBoxCSV.Controls.Add(this.lblLiczbaRekordow);
            this.gBoxCSV.Controls.Add(this.chboxKrytyczne);
            this.gBoxCSV.Controls.Add(this.btnSaveErrorList);
            this.gBoxCSV.Controls.Add(this.btnReadCSV);
            this.gBoxCSV.Controls.Add(this.txtInputCSV);
            this.gBoxCSV.Controls.Add(this.btnOpenCSV);
            this.gBoxCSV.Location = new System.Drawing.Point(12, 371);
            this.gBoxCSV.Name = "gBoxCSV";
            this.gBoxCSV.Size = new System.Drawing.Size(860, 109);
            this.gBoxCSV.TabIndex = 4;
            this.gBoxCSV.TabStop = false;
            this.gBoxCSV.Text = "CSV";
            // 
            // lblLiczbaRekordow
            // 
            this.lblLiczbaRekordow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLiczbaRekordow.Location = new System.Drawing.Point(448, 45);
            this.lblLiczbaRekordow.Name = "lblLiczbaRekordow";
            this.lblLiczbaRekordow.Size = new System.Drawing.Size(200, 30);
            this.lblLiczbaRekordow.TabIndex = 4;
            this.lblLiczbaRekordow.Text = "Liczba błędów: 0";
            this.lblLiczbaRekordow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chboxKrytyczne
            // 
            this.chboxKrytyczne.Checked = true;
            this.chboxKrytyczne.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chboxKrytyczne.Location = new System.Drawing.Point(6, 81);
            this.chboxKrytyczne.Name = "chboxKrytyczne";
            this.chboxKrytyczne.Size = new System.Drawing.Size(130, 23);
            this.chboxKrytyczne.TabIndex = 3;
            this.chboxKrytyczne.Text = "Tylko błędy krytyczne";
            this.chboxKrytyczne.UseVisualStyleBackColor = true;
            // 
            // btnSaveErrorList
            // 
            this.btnSaveErrorList.Location = new System.Drawing.Point(654, 45);
            this.btnSaveErrorList.Name = "btnSaveErrorList";
            this.btnSaveErrorList.Size = new System.Drawing.Size(200, 30);
            this.btnSaveErrorList.TabIndex = 6;
            this.btnSaveErrorList.Text = "Zapisz dane o błędach";
            this.btnSaveErrorList.UseVisualStyleBackColor = true;
            this.btnSaveErrorList.Click += new System.EventHandler(this.ButtonSaveErrorList_Click);
            // 
            // btnReadCSV
            // 
            this.btnReadCSV.Location = new System.Drawing.Point(142, 45);
            this.btnReadCSV.Name = "btnReadCSV";
            this.btnReadCSV.Size = new System.Drawing.Size(130, 30);
            this.btnReadCSV.TabIndex = 2;
            this.btnReadCSV.Text = "Czytaj CSV";
            this.btnReadCSV.UseVisualStyleBackColor = true;
            this.btnReadCSV.Click += new System.EventHandler(this.ButtonReadCSV_Click);
            // 
            // txtInputCSV
            // 
            this.txtInputCSV.Location = new System.Drawing.Point(6, 19);
            this.txtInputCSV.Name = "txtInputCSV";
            this.txtInputCSV.ReadOnly = true;
            this.txtInputCSV.Size = new System.Drawing.Size(848, 20);
            this.txtInputCSV.TabIndex = 2;
            // 
            // btnOpenCSV
            // 
            this.btnOpenCSV.Location = new System.Drawing.Point(6, 45);
            this.btnOpenCSV.Name = "btnOpenCSV";
            this.btnOpenCSV.Size = new System.Drawing.Size(130, 30);
            this.btnOpenCSV.TabIndex = 0;
            this.btnOpenCSV.Text = "Otwórz &CSV";
            this.btnOpenCSV.UseVisualStyleBackColor = true;
            this.btnOpenCSV.Click += new System.EventHandler(this.ButtonOpenCSV_Click);
            // 
            // mnuMain
            // 
            this.mnuMain.BackColor = System.Drawing.SystemColors.Menu;
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.pomocToolStripMenuItem});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(881, 24);
            this.mnuMain.TabIndex = 5;
            this.mnuMain.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wybierzGMLToolStripMenuItem,
            this.wybierzPlikCSVToolStripMenuItem,
            this.wyjścieToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(38, 20);
            this.toolStripMenuItem1.Text = "Plik";
            // 
            // wybierzGMLToolStripMenuItem
            // 
            this.wybierzGMLToolStripMenuItem.Name = "wybierzGMLToolStripMenuItem";
            this.wybierzGMLToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.wybierzGMLToolStripMenuItem.Text = "Wybierz plik GML";
            this.wybierzGMLToolStripMenuItem.Click += new System.EventHandler(this.WybierzGMLToolStripMenuItem_Click);
            // 
            // wybierzPlikCSVToolStripMenuItem
            // 
            this.wybierzPlikCSVToolStripMenuItem.Name = "wybierzPlikCSVToolStripMenuItem";
            this.wybierzPlikCSVToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.wybierzPlikCSVToolStripMenuItem.Text = "Wybierz plik CSV";
            this.wybierzPlikCSVToolStripMenuItem.Click += new System.EventHandler(this.WybierzPlikCSVToolStripMenuItem_Click);
            // 
            // wyjścieToolStripMenuItem
            // 
            this.wyjścieToolStripMenuItem.Name = "wyjścieToolStripMenuItem";
            this.wyjścieToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.wyjścieToolStripMenuItem.Text = "Wyjście";
            this.wyjścieToolStripMenuItem.Click += new System.EventHandler(this.WyjścieToolStripMenuItem_Click);
            // 
            // pomocToolStripMenuItem
            // 
            this.pomocToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.oProgramieToolStripMenuItem});
            this.pomocToolStripMenuItem.Name = "pomocToolStripMenuItem";
            this.pomocToolStripMenuItem.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.pomocToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.pomocToolStripMenuItem.Text = "Pomoc";
            // 
            // oProgramieToolStripMenuItem
            // 
            this.oProgramieToolStripMenuItem.Name = "oProgramieToolStripMenuItem";
            this.oProgramieToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.oProgramieToolStripMenuItem.Text = "O programie";
            this.oProgramieToolStripMenuItem.Click += new System.EventHandler(this.OProgramieToolStripMenuItem_Click);
            // 
            // dataSetCSV
            // 
            this.dataSetCSV.DataSetName = "dataSetCSV";
            // 
            // lblBT_Dokument
            // 
            this.lblBT_Dokument.AutoSize = true;
            this.lblBT_Dokument.Location = new System.Drawing.Point(6, 20);
            this.lblBT_Dokument.Name = "lblBT_Dokument";
            this.lblBT_Dokument.Size = new System.Drawing.Size(88, 13);
            this.lblBT_Dokument.TabIndex = 4;
            this.lblBT_Dokument.Text = "BT_Dokument: 0";
            // 
            // gboxStatystykaGML
            // 
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_PomieszczeniePrzynalezneDoLokalu);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_PunktGraniczny);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_UdzialDzierzawy);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_UdzialGospodarowaniaNieruchomosciaSPLubJST);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_UdzialWeWladaniuNieruchomosciaSPLubJST);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_UdzialWlasnosci);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_ZarzadSpolkiWspolnotyGruntowej);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_Zmiana);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_PodmiotGrupowy);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_OsobaFizyczna);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_OperatTechniczny);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_ObrebEwidencyjny);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_ObiektTrwaleZwiazanyZBudynkiem);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_Malzenstwo);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_LokalSamodzielny);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_Instytucja);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_JednostkaEwidencyjna);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_JednostkaRejestrowaBudynkow);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_JednostkaRejestrowaGruntow);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_JednostkaRejestrowaLokali);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_Klasouzytek);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_KonturKlasyfikacyjny);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_KonturUzytkuGruntowego);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_Dzierzawa);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_DzialkaEwidencyjna);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_Budynek);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_BlokBudynku);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_ArkuszEwidencyjny);
            this.gboxStatystykaGML.Controls.Add(this.lblEGB_Adres);
            this.gboxStatystykaGML.Controls.Add(this.lblBT_Dokument);
            this.gboxStatystykaGML.Location = new System.Drawing.Point(12, 121);
            this.gboxStatystykaGML.Name = "gboxStatystykaGML";
            this.gboxStatystykaGML.Size = new System.Drawing.Size(860, 231);
            this.gboxStatystykaGML.TabIndex = 7;
            this.gboxStatystykaGML.TabStop = false;
            this.gboxStatystykaGML.Text = "Statystyka pliku GML";
            // 
            // lblEGB_PomieszczeniePrzynalezneDoLokalu
            // 
            this.lblEGB_PomieszczeniePrzynalezneDoLokalu.AutoSize = true;
            this.lblEGB_PomieszczeniePrzynalezneDoLokalu.Location = new System.Drawing.Point(534, 60);
            this.lblEGB_PomieszczeniePrzynalezneDoLokalu.Name = "lblEGB_PomieszczeniePrzynalezneDoLokalu";
            this.lblEGB_PomieszczeniePrzynalezneDoLokalu.Size = new System.Drawing.Size(220, 13);
            this.lblEGB_PomieszczeniePrzynalezneDoLokalu.TabIndex = 33;
            this.lblEGB_PomieszczeniePrzynalezneDoLokalu.Text = "EGB_PomieszczeniePrzynalezneDoLokalu: 0";
            // 
            // lblEGB_PunktGraniczny
            // 
            this.lblEGB_PunktGraniczny.AutoSize = true;
            this.lblEGB_PunktGraniczny.Location = new System.Drawing.Point(534, 80);
            this.lblEGB_PunktGraniczny.Name = "lblEGB_PunktGraniczny";
            this.lblEGB_PunktGraniczny.Size = new System.Drawing.Size(122, 13);
            this.lblEGB_PunktGraniczny.TabIndex = 32;
            this.lblEGB_PunktGraniczny.Text = "EGB_PunktGraniczny: 0";
            // 
            // lblEGB_UdzialDzierzawy
            // 
            this.lblEGB_UdzialDzierzawy.AutoSize = true;
            this.lblEGB_UdzialDzierzawy.Location = new System.Drawing.Point(534, 100);
            this.lblEGB_UdzialDzierzawy.Name = "lblEGB_UdzialDzierzawy";
            this.lblEGB_UdzialDzierzawy.Size = new System.Drawing.Size(124, 13);
            this.lblEGB_UdzialDzierzawy.TabIndex = 31;
            this.lblEGB_UdzialDzierzawy.Text = "EGB_UdzialDzierzawy: 0";
            // 
            // lblEGB_UdzialGospodarowaniaNieruchomosciaSPLubJST
            // 
            this.lblEGB_UdzialGospodarowaniaNieruchomosciaSPLubJST.Location = new System.Drawing.Point(534, 120);
            this.lblEGB_UdzialGospodarowaniaNieruchomosciaSPLubJST.Name = "lblEGB_UdzialGospodarowaniaNieruchomosciaSPLubJST";
            this.lblEGB_UdzialGospodarowaniaNieruchomosciaSPLubJST.Size = new System.Drawing.Size(320, 13);
            this.lblEGB_UdzialGospodarowaniaNieruchomosciaSPLubJST.TabIndex = 30;
            this.lblEGB_UdzialGospodarowaniaNieruchomosciaSPLubJST.Text = "EGB_UdzialGospodarowaniaNieruchomosciaSPLubJST: 0";
            // 
            // lblEGB_UdzialWeWladaniuNieruchomosciaSPLubJST
            // 
            this.lblEGB_UdzialWeWladaniuNieruchomosciaSPLubJST.AutoSize = true;
            this.lblEGB_UdzialWeWladaniuNieruchomosciaSPLubJST.Location = new System.Drawing.Point(534, 140);
            this.lblEGB_UdzialWeWladaniuNieruchomosciaSPLubJST.Name = "lblEGB_UdzialWeWladaniuNieruchomosciaSPLubJST";
            this.lblEGB_UdzialWeWladaniuNieruchomosciaSPLubJST.Size = new System.Drawing.Size(265, 13);
            this.lblEGB_UdzialWeWladaniuNieruchomosciaSPLubJST.TabIndex = 29;
            this.lblEGB_UdzialWeWladaniuNieruchomosciaSPLubJST.Text = "EGB_UdzialWeWladaniuNieruchomosciaSPLubJST: 0";
            // 
            // lblEGB_UdzialWlasnosci
            // 
            this.lblEGB_UdzialWlasnosci.AutoSize = true;
            this.lblEGB_UdzialWlasnosci.Location = new System.Drawing.Point(534, 160);
            this.lblEGB_UdzialWlasnosci.Name = "lblEGB_UdzialWlasnosci";
            this.lblEGB_UdzialWlasnosci.Size = new System.Drawing.Size(125, 13);
            this.lblEGB_UdzialWlasnosci.TabIndex = 28;
            this.lblEGB_UdzialWlasnosci.Text = "EGB_UdzialWlasnosci: 0";
            // 
            // lblEGB_ZarzadSpolkiWspolnotyGruntowej
            // 
            this.lblEGB_ZarzadSpolkiWspolnotyGruntowej.AutoSize = true;
            this.lblEGB_ZarzadSpolkiWspolnotyGruntowej.Location = new System.Drawing.Point(534, 180);
            this.lblEGB_ZarzadSpolkiWspolnotyGruntowej.Name = "lblEGB_ZarzadSpolkiWspolnotyGruntowej";
            this.lblEGB_ZarzadSpolkiWspolnotyGruntowej.Size = new System.Drawing.Size(207, 13);
            this.lblEGB_ZarzadSpolkiWspolnotyGruntowej.TabIndex = 27;
            this.lblEGB_ZarzadSpolkiWspolnotyGruntowej.Text = "EGB_ZarzadSpolkiWspolnotyGruntowej: 0";
            // 
            // lblEGB_Zmiana
            // 
            this.lblEGB_Zmiana.AutoSize = true;
            this.lblEGB_Zmiana.Location = new System.Drawing.Point(534, 200);
            this.lblEGB_Zmiana.Name = "lblEGB_Zmiana";
            this.lblEGB_Zmiana.Size = new System.Drawing.Size(82, 13);
            this.lblEGB_Zmiana.TabIndex = 26;
            this.lblEGB_Zmiana.Text = "EGB_Zmiana: 0";
            // 
            // lblEGB_PodmiotGrupowy
            // 
            this.lblEGB_PodmiotGrupowy.AutoSize = true;
            this.lblEGB_PodmiotGrupowy.Location = new System.Drawing.Point(534, 40);
            this.lblEGB_PodmiotGrupowy.Name = "lblEGB_PodmiotGrupowy";
            this.lblEGB_PodmiotGrupowy.Size = new System.Drawing.Size(127, 13);
            this.lblEGB_PodmiotGrupowy.TabIndex = 25;
            this.lblEGB_PodmiotGrupowy.Text = "EGB_PodmiotGrupowy: 0";
            // 
            // lblEGB_OsobaFizyczna
            // 
            this.lblEGB_OsobaFizyczna.AutoSize = true;
            this.lblEGB_OsobaFizyczna.Location = new System.Drawing.Point(534, 20);
            this.lblEGB_OsobaFizyczna.Name = "lblEGB_OsobaFizyczna";
            this.lblEGB_OsobaFizyczna.Size = new System.Drawing.Size(119, 13);
            this.lblEGB_OsobaFizyczna.TabIndex = 24;
            this.lblEGB_OsobaFizyczna.Text = "EGB_OsobaFizyczna: 0";
            // 
            // lblEGB_OperatTechniczny
            // 
            this.lblEGB_OperatTechniczny.AutoSize = true;
            this.lblEGB_OperatTechniczny.Location = new System.Drawing.Point(270, 200);
            this.lblEGB_OperatTechniczny.Name = "lblEGB_OperatTechniczny";
            this.lblEGB_OperatTechniczny.Size = new System.Drawing.Size(134, 13);
            this.lblEGB_OperatTechniczny.TabIndex = 23;
            this.lblEGB_OperatTechniczny.Text = "EGB_OperatTechniczny: 0";
            // 
            // lblEGB_ObrebEwidencyjny
            // 
            this.lblEGB_ObrebEwidencyjny.AutoSize = true;
            this.lblEGB_ObrebEwidencyjny.Location = new System.Drawing.Point(270, 180);
            this.lblEGB_ObrebEwidencyjny.Name = "lblEGB_ObrebEwidencyjny";
            this.lblEGB_ObrebEwidencyjny.Size = new System.Drawing.Size(135, 13);
            this.lblEGB_ObrebEwidencyjny.TabIndex = 22;
            this.lblEGB_ObrebEwidencyjny.Text = "EGB_ObrebEwidencyjny: 0";
            // 
            // lblEGB_ObiektTrwaleZwiazanyZBudynkiem
            // 
            this.lblEGB_ObiektTrwaleZwiazanyZBudynkiem.AutoSize = true;
            this.lblEGB_ObiektTrwaleZwiazanyZBudynkiem.Location = new System.Drawing.Point(270, 160);
            this.lblEGB_ObiektTrwaleZwiazanyZBudynkiem.Name = "lblEGB_ObiektTrwaleZwiazanyZBudynkiem";
            this.lblEGB_ObiektTrwaleZwiazanyZBudynkiem.Size = new System.Drawing.Size(214, 13);
            this.lblEGB_ObiektTrwaleZwiazanyZBudynkiem.TabIndex = 21;
            this.lblEGB_ObiektTrwaleZwiazanyZBudynkiem.Text = "EGB_ObiektTrwaleZwiazanyZBudynkiem: 0";
            // 
            // lblEGB_Malzenstwo
            // 
            this.lblEGB_Malzenstwo.AutoSize = true;
            this.lblEGB_Malzenstwo.Location = new System.Drawing.Point(270, 140);
            this.lblEGB_Malzenstwo.Name = "lblEGB_Malzenstwo";
            this.lblEGB_Malzenstwo.Size = new System.Drawing.Size(103, 13);
            this.lblEGB_Malzenstwo.TabIndex = 20;
            this.lblEGB_Malzenstwo.Text = "EGB_Malzenstwo: 0";
            // 
            // lblEGB_LokalSamodzielny
            // 
            this.lblEGB_LokalSamodzielny.AutoSize = true;
            this.lblEGB_LokalSamodzielny.Location = new System.Drawing.Point(270, 120);
            this.lblEGB_LokalSamodzielny.Name = "lblEGB_LokalSamodzielny";
            this.lblEGB_LokalSamodzielny.Size = new System.Drawing.Size(132, 13);
            this.lblEGB_LokalSamodzielny.TabIndex = 19;
            this.lblEGB_LokalSamodzielny.Text = "EGB_LokalSamodzielny: 0";
            // 
            // lblEGB_Instytucja
            // 
            this.lblEGB_Instytucja.AutoSize = true;
            this.lblEGB_Instytucja.Location = new System.Drawing.Point(6, 160);
            this.lblEGB_Instytucja.Name = "lblEGB_Instytucja";
            this.lblEGB_Instytucja.Size = new System.Drawing.Size(92, 13);
            this.lblEGB_Instytucja.TabIndex = 18;
            this.lblEGB_Instytucja.Text = "EGB_Instytucja: 0";
            // 
            // lblEGB_JednostkaEwidencyjna
            // 
            this.lblEGB_JednostkaEwidencyjna.AutoSize = true;
            this.lblEGB_JednostkaEwidencyjna.Location = new System.Drawing.Point(6, 180);
            this.lblEGB_JednostkaEwidencyjna.Name = "lblEGB_JednostkaEwidencyjna";
            this.lblEGB_JednostkaEwidencyjna.Size = new System.Drawing.Size(156, 13);
            this.lblEGB_JednostkaEwidencyjna.TabIndex = 17;
            this.lblEGB_JednostkaEwidencyjna.Text = "EGB_JednostkaEwidencyjna: 0";
            // 
            // lblEGB_JednostkaRejestrowaBudynkow
            // 
            this.lblEGB_JednostkaRejestrowaBudynkow.AutoSize = true;
            this.lblEGB_JednostkaRejestrowaBudynkow.Location = new System.Drawing.Point(6, 200);
            this.lblEGB_JednostkaRejestrowaBudynkow.Name = "lblEGB_JednostkaRejestrowaBudynkow";
            this.lblEGB_JednostkaRejestrowaBudynkow.Size = new System.Drawing.Size(199, 13);
            this.lblEGB_JednostkaRejestrowaBudynkow.TabIndex = 16;
            this.lblEGB_JednostkaRejestrowaBudynkow.Text = "EGB_JednostkaRejestrowaBudynkow: 0";
            // 
            // lblEGB_JednostkaRejestrowaGruntow
            // 
            this.lblEGB_JednostkaRejestrowaGruntow.Location = new System.Drawing.Point(270, 20);
            this.lblEGB_JednostkaRejestrowaGruntow.Name = "lblEGB_JednostkaRejestrowaGruntow";
            this.lblEGB_JednostkaRejestrowaGruntow.Size = new System.Drawing.Size(320, 13);
            this.lblEGB_JednostkaRejestrowaGruntow.TabIndex = 15;
            this.lblEGB_JednostkaRejestrowaGruntow.Text = "EGB_JednostkaRejestrowaGruntow: 0";
            // 
            // lblEGB_JednostkaRejestrowaLokali
            // 
            this.lblEGB_JednostkaRejestrowaLokali.AutoSize = true;
            this.lblEGB_JednostkaRejestrowaLokali.Location = new System.Drawing.Point(270, 40);
            this.lblEGB_JednostkaRejestrowaLokali.Name = "lblEGB_JednostkaRejestrowaLokali";
            this.lblEGB_JednostkaRejestrowaLokali.Size = new System.Drawing.Size(177, 13);
            this.lblEGB_JednostkaRejestrowaLokali.TabIndex = 14;
            this.lblEGB_JednostkaRejestrowaLokali.Text = "EGB_JednostkaRejestrowaLokali: 0";
            // 
            // lblEGB_Klasouzytek
            // 
            this.lblEGB_Klasouzytek.AutoSize = true;
            this.lblEGB_Klasouzytek.Location = new System.Drawing.Point(270, 60);
            this.lblEGB_Klasouzytek.Name = "lblEGB_Klasouzytek";
            this.lblEGB_Klasouzytek.Size = new System.Drawing.Size(104, 13);
            this.lblEGB_Klasouzytek.TabIndex = 13;
            this.lblEGB_Klasouzytek.Text = "EGB_Klasouzytek: 0";
            // 
            // lblEGB_KonturKlasyfikacyjny
            // 
            this.lblEGB_KonturKlasyfikacyjny.AutoSize = true;
            this.lblEGB_KonturKlasyfikacyjny.Location = new System.Drawing.Point(270, 80);
            this.lblEGB_KonturKlasyfikacyjny.Name = "lblEGB_KonturKlasyfikacyjny";
            this.lblEGB_KonturKlasyfikacyjny.Size = new System.Drawing.Size(144, 13);
            this.lblEGB_KonturKlasyfikacyjny.TabIndex = 12;
            this.lblEGB_KonturKlasyfikacyjny.Text = "EGB_KonturKlasyfikacyjny: 0";
            // 
            // lblEGB_KonturUzytkuGruntowego
            // 
            this.lblEGB_KonturUzytkuGruntowego.AutoSize = true;
            this.lblEGB_KonturUzytkuGruntowego.Location = new System.Drawing.Point(270, 100);
            this.lblEGB_KonturUzytkuGruntowego.Name = "lblEGB_KonturUzytkuGruntowego";
            this.lblEGB_KonturUzytkuGruntowego.Size = new System.Drawing.Size(169, 13);
            this.lblEGB_KonturUzytkuGruntowego.TabIndex = 11;
            this.lblEGB_KonturUzytkuGruntowego.Text = "EGB_KonturUzytkuGruntowego: 0";
            // 
            // lblEGB_Dzierzawa
            // 
            this.lblEGB_Dzierzawa.AutoSize = true;
            this.lblEGB_Dzierzawa.Location = new System.Drawing.Point(6, 140);
            this.lblEGB_Dzierzawa.Name = "lblEGB_Dzierzawa";
            this.lblEGB_Dzierzawa.Size = new System.Drawing.Size(96, 13);
            this.lblEGB_Dzierzawa.TabIndex = 10;
            this.lblEGB_Dzierzawa.Text = "EGB_Dzierzawa: 0";
            // 
            // lblEGB_DzialkaEwidencyjna
            // 
            this.lblEGB_DzialkaEwidencyjna.AutoSize = true;
            this.lblEGB_DzialkaEwidencyjna.Location = new System.Drawing.Point(6, 120);
            this.lblEGB_DzialkaEwidencyjna.Name = "lblEGB_DzialkaEwidencyjna";
            this.lblEGB_DzialkaEwidencyjna.Size = new System.Drawing.Size(142, 13);
            this.lblEGB_DzialkaEwidencyjna.TabIndex = 9;
            this.lblEGB_DzialkaEwidencyjna.Text = "EGB_DzialkaEwidencyjna: 0";
            // 
            // lblEGB_Budynek
            // 
            this.lblEGB_Budynek.AutoSize = true;
            this.lblEGB_Budynek.Location = new System.Drawing.Point(6, 100);
            this.lblEGB_Budynek.Name = "lblEGB_Budynek";
            this.lblEGB_Budynek.Size = new System.Drawing.Size(89, 13);
            this.lblEGB_Budynek.TabIndex = 8;
            this.lblEGB_Budynek.Text = "EGB_Budynek: 0";
            // 
            // lblEGB_BlokBudynku
            // 
            this.lblEGB_BlokBudynku.AutoSize = true;
            this.lblEGB_BlokBudynku.Location = new System.Drawing.Point(6, 80);
            this.lblEGB_BlokBudynku.Name = "lblEGB_BlokBudynku";
            this.lblEGB_BlokBudynku.Size = new System.Drawing.Size(110, 13);
            this.lblEGB_BlokBudynku.TabIndex = 7;
            this.lblEGB_BlokBudynku.Text = "EGB_BlokBudynku: 0";
            // 
            // lblEGB_ArkuszEwidencyjny
            // 
            this.lblEGB_ArkuszEwidencyjny.AutoSize = true;
            this.lblEGB_ArkuszEwidencyjny.Location = new System.Drawing.Point(6, 60);
            this.lblEGB_ArkuszEwidencyjny.Name = "lblEGB_ArkuszEwidencyjny";
            this.lblEGB_ArkuszEwidencyjny.Size = new System.Drawing.Size(138, 13);
            this.lblEGB_ArkuszEwidencyjny.TabIndex = 6;
            this.lblEGB_ArkuszEwidencyjny.Text = "EGB_ArkuszEwidencyjny: 0";
            // 
            // lblEGB_Adres
            // 
            this.lblEGB_Adres.AutoSize = true;
            this.lblEGB_Adres.Location = new System.Drawing.Point(6, 40);
            this.lblEGB_Adres.Name = "lblEGB_Adres";
            this.lblEGB_Adres.Size = new System.Drawing.Size(74, 13);
            this.lblEGB_Adres.TabIndex = 5;
            this.lblEGB_Adres.Text = "EGB_Adres: 0";
            // 
            // stbMainStatus
            // 
            this.stbMainStatus.AutoSize = false;
            this.stbMainStatus.Margin = new System.Windows.Forms.Padding(10, 3, 0, 2);
            this.stbMainStatus.Name = "stbMainStatus";
            this.stbMainStatus.Size = new System.Drawing.Size(270, 20);
            this.stbMainStatus.Text = "stbMainStatus";
            this.stbMainStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // stbMainProgress
            // 
            this.stbMainProgress.Name = "stbMainProgress";
            this.stbMainProgress.Size = new System.Drawing.Size(590, 19);
            this.stbMainProgress.Step = 1;
            // 
            // stbMain
            // 
            this.stbMain.AutoSize = false;
            this.stbMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stbMainStatus,
            this.stbMainProgress});
            this.stbMain.Location = new System.Drawing.Point(0, 557);
            this.stbMain.Name = "stbMain";
            this.stbMain.Size = new System.Drawing.Size(881, 25);
            this.stbMain.SizingGrip = false;
            this.stbMain.TabIndex = 2;
            this.stbMain.Text = "statusStrip1";
            // 
            // lblLicencja
            // 
            this.lblLicencja.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblLicencja.ForeColor = System.Drawing.Color.Red;
            this.lblLicencja.Location = new System.Drawing.Point(285, 486);
            this.lblLicencja.Name = "lblLicencja";
            this.lblLicencja.Size = new System.Drawing.Size(174, 68);
            this.lblLicencja.TabIndex = 10;
            this.lblLicencja.Text = "lblLicencja";
            this.lblLicencja.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblClient
            // 
            this.lblClient.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblClient.Location = new System.Drawing.Point(460, 486);
            this.lblClient.Name = "lblClient";
            this.lblClient.Size = new System.Drawing.Size(340, 68);
            this.lblClient.TabIndex = 11;
            this.lblClient.Text = "lblClient";
            this.lblClient.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBoxClient
            // 
            this.pictureBoxClient.Location = new System.Drawing.Point(813, 486);
            this.pictureBoxClient.Name = "pictureBoxClient";
            this.pictureBoxClient.Size = new System.Drawing.Size(59, 68);
            this.pictureBoxClient.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxClient.TabIndex = 9;
            this.pictureBoxClient.TabStop = false;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = global::GML_EC.Properties.Resources.logo;
            this.pictureBoxLogo.Location = new System.Drawing.Point(12, 486);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(195, 68);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLogo.TabIndex = 8;
            this.pictureBoxLogo.TabStop = false;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 582);
            this.Controls.Add(this.lblClient);
            this.Controls.Add(this.lblLicencja);
            this.Controls.Add(this.pictureBoxClient);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.gboxStatystykaGML);
            this.Controls.Add(this.gBoxCSV);
            this.Controls.Add(this.stbMain);
            this.Controls.Add(this.mnuMain);
            this.Controls.Add(this.gBoxGML);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.mnuMain;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.Text = "FrmMain";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.Shown += new System.EventHandler(this.FrmMain_Shown);
            this.gBoxGML.ResumeLayout(false);
            this.gBoxGML.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetGML)).EndInit();
            this.gBoxCSV.ResumeLayout(false);
            this.gBoxCSV.PerformLayout();
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCSV)).EndInit();
            this.gboxStatystykaGML.ResumeLayout(false);
            this.gboxStatystykaGML.PerformLayout();
            this.stbMain.ResumeLayout(false);
            this.stbMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxClient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenGML;
        private System.Windows.Forms.GroupBox gBoxGML;
        private System.Windows.Forms.TextBox txtInputGML;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.Button btnReadGML;
        private System.Data.DataSet dataSetGML;
        private System.Windows.Forms.Button btnSaveGMLData;
        private System.Windows.Forms.GroupBox gBoxCSV;
        private System.Windows.Forms.Button btnReadCSV;
        private System.Windows.Forms.TextBox txtInputCSV;
        private System.Windows.Forms.Button btnOpenCSV;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem wybierzGMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wybierzPlikCSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wyjścieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pomocToolStripMenuItem;
        private System.Data.DataSet dataSetCSV;
        private System.Windows.Forms.Button btnSaveErrorList;
        private System.Windows.Forms.Label lblBT_Dokument;
        private System.Windows.Forms.GroupBox gboxStatystykaGML;
        private System.Windows.Forms.Label lblEGB_Instytucja;
        private System.Windows.Forms.Label lblEGB_JednostkaEwidencyjna;
        private System.Windows.Forms.Label lblEGB_JednostkaRejestrowaBudynkow;
        private System.Windows.Forms.Label lblEGB_JednostkaRejestrowaGruntow;
        private System.Windows.Forms.Label lblEGB_JednostkaRejestrowaLokali;
        private System.Windows.Forms.Label lblEGB_Klasouzytek;
        private System.Windows.Forms.Label lblEGB_KonturKlasyfikacyjny;
        private System.Windows.Forms.Label lblEGB_KonturUzytkuGruntowego;
        private System.Windows.Forms.Label lblEGB_Dzierzawa;
        private System.Windows.Forms.Label lblEGB_DzialkaEwidencyjna;
        private System.Windows.Forms.Label lblEGB_Budynek;
        private System.Windows.Forms.Label lblEGB_BlokBudynku;
        private System.Windows.Forms.Label lblEGB_ArkuszEwidencyjny;
        private System.Windows.Forms.Label lblEGB_Adres;
        private System.Windows.Forms.Label lblEGB_PomieszczeniePrzynalezneDoLokalu;
        private System.Windows.Forms.Label lblEGB_PunktGraniczny;
        private System.Windows.Forms.Label lblEGB_UdzialDzierzawy;
        private System.Windows.Forms.Label lblEGB_UdzialGospodarowaniaNieruchomosciaSPLubJST;
        private System.Windows.Forms.Label lblEGB_UdzialWeWladaniuNieruchomosciaSPLubJST;
        private System.Windows.Forms.Label lblEGB_UdzialWlasnosci;
        private System.Windows.Forms.Label lblEGB_ZarzadSpolkiWspolnotyGruntowej;
        private System.Windows.Forms.Label lblEGB_Zmiana;
        private System.Windows.Forms.Label lblEGB_PodmiotGrupowy;
        private System.Windows.Forms.Label lblEGB_OsobaFizyczna;
        private System.Windows.Forms.Label lblEGB_OperatTechniczny;
        private System.Windows.Forms.Label lblEGB_ObrebEwidencyjny;
        private System.Windows.Forms.Label lblEGB_ObiektTrwaleZwiazanyZBudynkiem;
        private System.Windows.Forms.Label lblEGB_Malzenstwo;
        private System.Windows.Forms.Label lblEGB_LokalSamodzielny;
        private System.Windows.Forms.CheckBox chboxKrytyczne;
        private System.Windows.Forms.Label lblLiczbaRekordow;
        private System.Windows.Forms.ToolStripStatusLabel stbMainStatus;
        private System.Windows.Forms.ToolStripProgressBar stbMainProgress;
        private System.Windows.Forms.StatusStrip stbMain;
        private System.Windows.Forms.ToolStripMenuItem oProgramieToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.PictureBox pictureBoxClient;
        private System.Windows.Forms.Label lblLicencja;
        private System.Windows.Forms.Label lblClient;
    }
}

