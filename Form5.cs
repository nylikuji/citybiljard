using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Drawing;

namespace biljardipoytateoria
{
    public partial class Form5 : Form
    {
        SQLiteConnection tietoyhteys;
        Tv teevee = new Tv();
        public Form5()
        {
            InitializeComponent();
            tietoyhteys = new SQLiteConnection("Data Source=poydat.sqlite;Version=3");
            tietoyhteys.Open();

            teevee.initsql();

            this.KeyPreview = true;
            this.Text = "";
            this.TopMost = true;

            this.FormBorderStyle = FormBorderStyle.None;

            foreach (Control x in this.Controls)
            {
                x.BackColor = Color.DimGray;
                x.ForeColor = Color.White;
            }

            paivita();
        }
        private void paivita()
        {
            kaikkipoydat.Items.Clear();
            kaikkipelit.Items.Clear();
            editablepoydat.Items.Clear();
            editablehinnat.Items.Clear();

            string sql = "SELECT * FROM pelit";
            SQLiteCommand komento = new SQLiteCommand(sql, tietoyhteys); //Määritellään käsky tietokantayhteyteen
            SQLiteDataReader lukija = komento.ExecuteReader(); //Tällä luetaan haettu tieto myöhemmin

            while (lukija.Read())
            {
                ListViewItem biljardipeli = new ListViewItem(lukija["poyta"].ToString() + "-" + lukija["tyyppi"].ToString());
                biljardipeli.SubItems.Add(lukija["alku"].ToString() + " -> " + lukija["loppu"].ToString());
                biljardipeli.SubItems.Add(lukija["vaihto"].ToString());

                string pituus = lukija["pituus"].ToString();
                int middle = pituus.IndexOf("/"); //haetaan pituuden keskeltä kauttamerkki
                int sessiomin = Int32.Parse(pituus.Substring(0, middle)) % 60;
                int sessioh = (Int32.Parse(pituus.Substring(0, middle)) - sessiomin) / 60; //muunnetaan minuutit tunneiksi
                int allmin = Int32.Parse(pituus.Substring(middle+1, pituus.Length-middle-1)) % 60;
                int allh = (Int32.Parse(pituus.Substring(middle+1, pituus.Length-middle-1)) - allmin) / 60;
                biljardipeli.SubItems.Add(sessioh + "h " + sessiomin + "min" + "/" + allh + "h " + allmin + "min");

                kaikkipelit.Items.Insert(0,biljardipeli); //lisätään esine aina indeksiin 0, jotta se näkyy ylimpänä
            }

            sql = "SELECT * FROM poytainfo";
            komento = new SQLiteCommand(sql, tietoyhteys); //Määritellään käsky tietokantayhteyteen
            lukija = komento.ExecuteReader(); //Tällä luetaan haettu tieto myöhemmin

            while (lukija.Read())
            {
                ListViewItem biljardipoyta = new ListViewItem(lukija["poyta"].ToString() + "-" + lukija["tyyppi"].ToString());
                biljardipoyta.SubItems.Add(lukija["pelikerrat"].ToString());
                double minuutittunteina = float.Parse(lukija["peliminuutit"].ToString());
                biljardipoyta.SubItems.Add(Math.Round((minuutittunteina / 60),1).ToString()+"h");
                //luodaan olio, joka lisätään ListViewiin

                kaikkipoydat.Items.Add(biljardipoyta); //Lisätään tieto poytainfosta listaan, jossa näkyy tiedot kaikista pöydistä.
                editablepoydat.Items.Add(lukija["poyta"].ToString());
            }
            kaikkipelit.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            kaikkipelit.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.HeaderSize);

            for(int i = 1; i < 6; i++)
            {
                editablehinnat.Items.Add(i.ToString()); //hintoja on 5 erilaista, lisätään ne listaan josta niitä voi valita muokattavaksi
            }

            Form1.paivittaja.paivita();
        }

        private void Editablepoydat_SelectedIndexChanged(object sender, EventArgs e)
        {
            uusityyppi.Text = teevee.hae("tyyppi", "poytainfo", "poyta", editablepoydat.Text); //näytetään pöydän tyyppi laatikossa, jossa sitä voi muokata
            uusityyppi.Visible = true;
            emptygames.Visible = true;
            emptytables.Visible = true;
            label1.Visible = true;
        }

        private void Uusityyppi_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                teevee.muunna("poytainfo", "tyyppi", "'"+uusityyppi.Text+"'", "poyta", editablepoydat.Text);
                paivita();
            }
            if(e.KeyCode == Keys.Up && editablepoydat.Text != "30")
            {
                editablepoydat.Text = (Int32.Parse(editablepoydat.Text) + 1).ToString();
            }
            else if(e.KeyCode == Keys.Down && editablepoydat.Text != "0")
            {
                editablepoydat.Text = (Int32.Parse(editablepoydat.Text) - 1).ToString();
            }
        }

        private void Form5_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            if(e.KeyCode == Keys.F10)
            {
                resetti.Visible = true;
            }
        }

        private void Hinnanpaivittaja_Click(object sender, EventArgs e)
        {
            string[] hinnat = { hintaustyyppi.Text.ToString(), hintapertunti.Text.ToString() };
            string[] haut = { "Luokka", "pertunti" };

            bool success = true;

            if(!float.TryParse(hinnat[1], out float fail))
            {
                success = false;
            }

            if (success)
            {
                teevee.muunna("hinnat", haut[0], "'" + hinnat[0] + "'", "ID", editablehinnat.Text);
                teevee.muunna("hinnat", haut[1], "'" + hinnat[1] + "'", "ID", editablehinnat.Text);
            }

            else
            {
                var result = MessageBox.Show("Antamaasi numeroa ei voitu prosessoida.\nDesimaalit merkitään pilkulla.", "virhe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            paivita();
        }

        private void Editablehinnat_SelectedIndexChanged(object sender, EventArgs e)
        {
            hintaustyyppi.Text = teevee.hae("Luokka", "hinnat", "ID", editablehinnat.Text);
            hintapertunti.Text = teevee.hae("pertunti", "hinnat", "ID", editablehinnat.Text); //kun valitaan hinnan ID, näytetään hinta laatikossa, jossa sitä voidaan muokata.

            hinnanpaivittaja.Visible = true;
        }

        private void emptygames_Click(object sender, EventArgs e)
        {
            string decision = "Haluatko varmasti nollata pöydän " + editablepoydat.Text + " tiedot?";
            var varmistus = MessageBox.Show(decision, "varmistus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (varmistus == DialogResult.Yes)
            {
                teevee.seekanddestroy("pelit", "poyta", editablepoydat.Text);
            }
            paivita();
        }

        private void emptytables_Click(object sender, EventArgs e)
        {
            string decision = "Haluatko varmasti nollata pöydän " + editablepoydat.Text + " tiedot?";
            var varmistus = MessageBox.Show(decision, "varmistus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (varmistus == DialogResult.Yes)
            {
                teevee.muunna("poytainfo", "pelikerrat", "0", "poyta", editablepoydat.Text);
                teevee.muunna("poytainfo", "peliminuutit", "0", "poyta", editablepoydat.Text);
            }
            paivita();
        }

        private void kaikkipoydat_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach(ListViewItem poyta in kaikkipoydat.Items)
            {
                if(poyta.Selected)
                {
                    string valittu = poyta.Text;
                    int viiva = valittu.IndexOf("-");
                    editablepoydat.SelectedIndex = Int32.Parse(valittu.Substring(0, viiva)) - 1;
                }
            }
        }

        private void Form5_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.paivittaja.Show();
        }

        private void Resetti_Click(object sender, EventArgs e)
        {
            string decision = "Haluatko varmasti nollata kaikkien pöytien tiedot?";
            var varmistus = MessageBox.Show(decision, "varmistus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (varmistus == DialogResult.Yes)
            {
                for (int i = 1; i < 31; i++)
                {
                    teevee.seekanddestroy("pelit", "poyta", i.ToString());
                    teevee.muunna("poytainfo", "pelikerrat", "0", "poyta", i.ToString());
                    teevee.muunna("poytainfo", "peliminuutit", "0", "poyta", i.ToString());
                }
                paivita();
            }
        }
    }
}
