using System;
using System.Windows.Forms;
using System.Drawing;

namespace biljardipoytateoria
{
    public partial class Form2 : Form
    {
        Tv teevee = new Tv();

        public Form2(int width)
        {
            InitializeComponent();

            teevee.initsql();

            this.KeyPreview = true;
            this.ActiveControl = vuokranum; //Tässä ikkunassa tekstilaatikko vuokranum on aina keskitetty

            this.Text = "vuokraa pöytä";

            this.FormBorderStyle = FormBorderStyle.None;
            foreach (Control x in this.Controls)
            {
                x.Font = new Font(Otsikko.Font.Name, width / 30);
                x.BackColor = Color.DimGray;
                x.ForeColor = Color.White;
            }
            Otsikko.Top = 0;
            Otsikko.Left = 0;
            this.Width = Otsikko.Width;
            this.Height = Otsikko.Height + vuokranum.Height;
            vuokranum.Top = Otsikko.Bottom;
            vuokranum.Left = 0;
            vuokranum.Width = Otsikko.Width / 3;
        }

        private void vuokranum_KeyUp(object sender, KeyEventArgs e) //Kun vuokranumissa tapahtuu napinpainallus
        {
            if(!Int32.TryParse(vuokranum.Text, out int lol) || lol > 30) //katsoo eikö annettua tekstiä voi kääntää numeroksi tai onko jäsennetty numero yli 30
            {
                vuokranum.Clear(); //Tekstilaatikko vuokranum tyhjennetään jos tosi
            }
            else if((e.KeyCode == Keys.Enter && lol != null) || lol.ToString().Length > 1)
            {
                vuokraus(lol); //Käsketään ohjelmaa vuokraamaan tietokantaan pöytä, jos numero on annettu ja enteriä on painettu
            }
        }

        private void vuokraus(int vuokrattava) //Funktio, jossa vuokrataan pöytä tietokantaan
        {
            string varauspaatos = teevee.hae("varauspaatos", "poydat", "poyta", vuokrattava.ToString());

            if (varauspaatos == "")
            {
                var laatikko = MessageBox.Show("Pöytä " + vuokrattava + " on jo varattu", "Ongelma", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (laatikko == DialogResult.OK)
                {
                    this.Close();
                }
            }
            else
            {
                string[] muutokset = {"'"+teevee.nykytunti()+"'", teevee.nykyhetki(), teevee.nykyhetki(), "''", "''" };
                string[] muunnettavat = { "aikavarattu", "varaus", "vaihtovaraus", "aikapaatos", "varauspaatos" };

                for(int i = 0; i < 5; i++)
                {
                teevee.muunna("poydat", muunnettavat[i], muutokset[i], "poyta", vuokrattava.ToString());
                }
            }

            Form1.paivittaja.paivita(); //Päivitetään näkymä pääikkunassa

            this.Close(); //Suljetaan tänä ikkuna
        }

        private void Form2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close(); //Ikkunan voi sulkea myös ESCiä painamalla
            }
        }
    }
}