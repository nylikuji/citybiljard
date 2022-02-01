using System;
using System.Windows.Forms;
using System.Drawing;

namespace biljardipoytateoria
{
    public partial class Form4 : Form
    {
        Tv teevee = new Tv();

        int ekapoyta;

        public Form4(int width)
        {
            InitializeComponent();

            teevee.initsql();

            this.KeyPreview = true;
            this.ActiveControl = vaihdettava;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Text = "vaihda pöytää";
            foreach (Control x in this.Controls)
            {
                x.Font = new Font(Otsikko.Font.Name, width / 30);
                x.BackColor = Color.DimGray;
                x.ForeColor = Color.White;
            }
            Otsikko.Top = 0;
            Otsikko.Left = 0;
            vaihdettava.Width = Otsikko.Width / 3;
            tuohon.Width = Otsikko.Width / 3;
            vaihdettava.Left = 0;
            nuoli.Left = vaihdettava.Right;
            tuohon.Left = nuoli.Right;
            vaihdettava.Top = Otsikko.Bottom;
            nuoli.Top = Otsikko.Bottom;
            tuohon.Top = Otsikko.Bottom;
            this.Width = Otsikko.Width;
            this.Height = Otsikko.Height + tuohon.Height;
        }

        private void vaihdettava_KeyUp(object sender, KeyEventArgs e)
        {
            if (!Int32.TryParse(vaihdettava.Text, out ekapoyta) || ekapoyta > 30)
            {
                vaihdettava.Clear();
            }
            else if (e.KeyCode == Keys.Enter && ekapoyta != null || vaihdettava.Text.Length == 2)
            {
                this.ActiveControl = tuohon;
            }
        }

        private void tuohon_KeyUp(object sender, KeyEventArgs e)
        {
            if (!Int32.TryParse(tuohon.Text, out int lol) || lol > 30)
            {
                tuohon.Clear();
            }
            else if (e.KeyCode == Keys.Enter && lol != null)
            {
                vaihdapoytaa(ekapoyta, lol);
                this.Close();
            }
            if ( tuohon.Text == "" && e.KeyCode == Keys.Back)
            {
                this.ActiveControl = vaihdettava;
            }
        }

        private void vaihdapoytaa(int vaihdatamapoyta, int paamaara)
        {
            string[] sanat = new string[4];
            string[] queryt = { "aikavarattu", "varaus", "aikapaatos", "varauspaatos" };

            string varauspaatos = teevee.hae("varauspaatos", "poydat", "poyta", paamaara.ToString());
            string varauspaatos2 = teevee.hae("varauspaatos", "poydat", "poyta", vaihdatamapoyta.ToString());

            if (varauspaatos == "" || varauspaatos2 != "")
            {
                var laatikko = MessageBox.Show("Pöytä " + paamaara + " on jo varattu", "Ongelma", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //ei anneta siirtää varausta pöytään, joka on jo varattu.
                if (laatikko == DialogResult.OK)
                {
                    this.Close();
                }
            }
            else
            {
                for(int i = 0; i < 4; i++)
                {
                    sanat[i] = teevee.hae(queryt[i], "poydat", "poyta", vaihdatamapoyta.ToString());
                }

                for (int i = 0; i < 4; i++) //päivitetään tiedot päämäärässä
                {
                    teevee.muunna("poydat", "'"+queryt[i]+"'", "'"+sanat[i]+"'", "poyta", paamaara.ToString());
                }

                teevee.muunna("poydat", "vaihtovaraus", "'"+teevee.nykyhetki()+"'", "poyta", paamaara.ToString());

                Form1.paivittaja.paatavaraus(vaihdatamapoyta.ToString(), "->" + paamaara.ToString()); 
                // palautetussa "hinnassa" on nuoli, koska se merkitsee sitä että pöydän varaus on siirretty toiseen pöytään.
                Form1.paivittaja.paivita();
            }
        }

        private void Form4_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
