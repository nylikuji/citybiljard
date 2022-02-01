using System;
using System.Windows.Forms;
using System.Drawing;

namespace biljardipoytateoria
{
    public partial class Form1 : Form
    {

        public static Form1 paivittaja = null; //muut formit voivat tätä kautta kutsua paivita() funktiota.
        Tv teevee = new Tv();

        public Form1()
        {
            teevee.initsql(); //olio, jossa on funktioita SQL-käskyille, jottei koodi olisi liian redundant

            InitializeComponent();
            paivita();
            this.KeyPreview = true; //Formi kuuntelee napinpainalluksia ennenkuin ne menevät perille

            paivittaja = this;

            this.Text = "biljardipöytälaskutusohjelma";

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
        }

        public void paivita() //Erittäin tärkeä funktio, joka listaa pöydät pääikkunaan
        {
            poydat15.Text = "";
            poydat30.Text = "";
            vapaat15.Text = "";
            vapaat30.Text = "";

            for (int i = 1; i < 31; i++) //haetaan tietokannasta kaikkien 30 pöydän tiedot
            {
                string tyyppi = teevee.hae("tyyppi", "poytainfo", "poyta", i.ToString());
                string aikapaatos = teevee.hae("aikapaatos", "poydat", "poyta", i.ToString());
                string aikavarattu = " ";
                string occupied = " ";

                if(aikapaatos == "") //pöytä on varattu, koska siellä ollut peli ei ole päättynyt vielä
                {
                    occupied = "VARATTU"; 
                    aikavarattu = teevee.hae("aikavarattu", "poydat", "poyta", i.ToString());
                }

                if (i < 16)
                {
                    poydat15.Text = poydat15.Text + i.ToString() + ". " + tyyppi + "\n";
                    vapaat15.Text = vapaat15.Text + aikavarattu + "-" + occupied + "\n";
                }

                else if (i < 31)
                {
                    poydat30.Text = poydat30.Text + i.ToString() + ". " + tyyppi + "\n";
                    vapaat30.Text = vapaat30.Text + aikavarattu + "-" + occupied + "\n";
                }
                    

            }
            poydat15.Left = 15;
            vapaat15.Left = poydat15.Right + 15;
            poydat30.Left = vapaat15.Right + 15;
            vapaat30.Left = poydat30.Right + 15;
        }

        private void varaapoyta_Click(object sender, EventArgs e) //Kun painaa nappia "Varaa Pöytä (F1)"
        {
            Form2 varaaja = new Form2(this.Width); //Luodaan uusi formi
            varaaja.Show(); //Näytetään uusi formi
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e) //Funktio pikanäppäimiä varten
        {
            if (e.KeyCode == Keys.F1) //Jos painettu näppäin on F1
            {
                varaapoyta.PerformClick(); //Painetaan nappia "Varaa Pöytä (F1)"
            }
            else if(e.KeyCode == Keys.F2)
            {
                palautapallot.PerformClick();
            }
            else if (e.KeyCode == Keys.F3)
            {
                vaihdapoytaa.PerformClick();
            }
            else if (e.KeyCode == Keys.F5)
            {
                Form5 asetukset = new Form5();
                asetukset.Show();
            }
        }

        private void palautapallot_Click(object sender, EventArgs e)
        {
            Form3 palauttaja = new Form3(this.Width);
            palauttaja.Show();
        }

        private void vaihdapoytaa_Click(object sender, EventArgs e)
        {
            Form4 vaihtaja = new Form4(this.Width);
            vaihtaja.Show();
        }

        public void paatavaraus(string paatettava, string vaihto) //Funktio, jolla päivitetään varatun pöydän varauksen päätösaika. Julkinen, koska tätä tarvitaan pöytien vaihtoon myös
        {
            string nykypaiva = teevee.nykypaiva();
            string nykyhetki = teevee.nykyhetki();
            string nykytunti = teevee.nykytunti();

            teevee.muunna("poydat","aikapaatos","'"+nykytunti+"'","poyta",paatettava); //päätetään varaus pöydästä
            teevee.muunna("poydat","varauspaatos",nykyhetki,"poyta",paatettava);

            string tyyppi = teevee.hae("tyyppi", "poytainfo", "poyta", paatettava);
            string poyta = paatettava;
            string alku = teevee.hae("aikavarattu", "poydat", "poyta", paatettava);
            string loppu = teevee.hae("aikapaatos", "poydat", "poyta", paatettava) + " || " + nykypaiva;

            Int64 minalku = Int64.Parse(teevee.hae("varaus", "poydat", "poyta", paatettava));
            Int64 minpaatos = Int64.Parse(teevee.hae("varauspaatos", "poydat", "poyta", paatettava));
            Int64 vaihtovaraus = Int64.Parse(teevee.hae("vaihtovaraus", "poydat", "poyta", paatettava));

            Int64 pituus = (minpaatos - minalku) / 60; //koko varauksen pituus, mukaanlukien pöydät, joissa varaus on ollut. Hinta maksetaan riippuen tästä.
            Int64 sessiopituus = (minpaatos - vaihtovaraus) / 60; //session pituus lyhenee aina kun varaus siirretään pöydästä toiseen.

            int pelikerrat = Int32.Parse(teevee.hae("pelikerrat", "poytainfo", "poyta", paatettava));
            Int64 peliminuutit = Int64.Parse(teevee.hae("peliminuutit", "poytainfo", "poyta", paatettava));

            pelikerrat++;
            peliminuutit = peliminuutit + sessiopituus; //lisätään pelatut minuutit myöhemmin taulukkoon, jossa on pöytien info

            teevee.muunna("poytainfo", "pelikerrat", pelikerrat.ToString(), "poyta", paatettava);
            teevee.muunna("poytainfo", "peliminuutit", peliminuutit.ToString(), "poyta", paatettava);

            teevee.lisaa("pelit", "(poyta,alku,loppu,tyyppi,vaihto,pituus)", "('"+poyta+"', '"+alku+"', '"+loppu+"', '"+tyyppi+"', '"+vaihto+"', '"+sessiopituus+"/"+pituus+"');");
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            varaapoyta.Top = 15;
            vaihdapoytaa.Top = 15;
            palautapallot.Top = 15;
            foreach (Control x in this.Controls)
            {
                x.Font = new Font(vapaat15.Font.Name, this.Width / 50);
                x.BackColor = Color.Black;
                x.ForeColor = Color.White;
                if (x is Button)
                {
                    x.Height = this.Height / 15;
                    x.Width = this.Width / 4;
                    x.Top = poydat15.Bottom + 15;
                }
            }
            varaapoyta.Left = 15;
            palautapallot.Left = varaapoyta.Right + 15;
            vaihdapoytaa.Left = palautapallot.Right + 15;

            poydat15.Left = 15;
            vapaat15.Left = poydat15.Right + 15;
            poydat30.Left = vapaat15.Right + 15;
            vapaat30.Left = poydat30.Right + 15;
        }
    }
}
