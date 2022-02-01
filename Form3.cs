using System;
using System.Windows.Forms;
using System.Drawing;

namespace biljardipoytateoria
{
    public partial class Form3 : Form
    {
        Tv teevee = new Tv();

        float lasku;
        int poytanro;

        public Form3(int width)
        {
            InitializeComponent();

            teevee.initsql();

            teksti.Top = 0;
            tiedot.Top = 0;
            teksti.Left = 0;
            teksti.Text = "Pöydän nro: \nAjalta: \nPeliaika: ";

            this.KeyPreview = true;
            this.ActiveControl = palautusnum;

            this.Text = "Palauta pallot";
            this.Width = width - (width/3);
            hintalistaus.Width = this.Width;

            Color clr = Color.FromArgb(255, 15,15,15);
            this.BackColor = clr;

            foreach (Control x in this.Controls)
            {
                x.Font = new Font(tiedot.Font.Name, width / 65);
                x.BackColor = clr;
                x.ForeColor = Color.White;
            }

            hintalistaus.Height = tiedot.Height * 7;

            tiedot.Left = teksti.Right;
            this.FormBorderStyle = FormBorderStyle.None;
            hintalistaus.Left = 0;
            hintalistaus.Top = teksti.Bottom + 15;
            hintalistaus.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            kysymys.Top = hintalistaus.Bottom + 15;
            palautusnum.Top = kysymys.Bottom + 15;

            varmistus.Top = kysymys.Top;
            varmistus.Left = kysymys.Right + 15;

            naytahinnat();
            this.Height = palautusnum.Bottom;

        }

        private void palautusnum_KeyUp(object sender, KeyEventArgs e)
        {
            if (!Int32.TryParse(palautusnum.Text, out int lol) || lol > 30)
            {
                palautusnum.Clear();
            }
            else if (e.KeyCode == Keys.Enter && lol != null)
            {
                nayttaydy(lol);
                naytahinnat();
                palautusnum.Visible = false;
                this.ActiveControl = varmistus;
            }
        }

        private void nayttaydy(int vuokrattava)
        {
            poytanro = vuokrattava;
            tiedot.Text = teevee.hae("poyta", "poydat", "poyta", vuokrattava.ToString()) + "\n";
            tiedot.Text = tiedot.Text + teevee.hae("aikavarattu", "poydat", "poyta", vuokrattava.ToString()) + " -> " + teevee.hae("aikapaatos", "poydat", "poyta", vuokrattava.ToString()) + "\n";

            string varaus = teevee.hae("varaus", "poydat", "poyta", vuokrattava.ToString());
            string varauspaatos = teevee.hae("varauspaatos", "poydat", "poyta", vuokrattava.ToString());

            if (varauspaatos == "") //katsotaan onko pöydässä varaus kesken.
            {
                varauspaatos = teevee.nykyhetki(); //katsotaan aika nykyhetkestä, eikä varauksen päätöstä, jos varaus ei ole päättynyt
                varmistus.Visible = true; //annetaan käyttäjälle mahdollisuus päättää varaus
                kysymys.Visible = true;
            }
            else
            {
                varmistus.Visible = false; //estetään käyttäjältä mahdollisuus päättää varaus, koska se on jo päättynyt.
                kysymys.Visible = false;
            }

            if(Int64.TryParse(varauspaatos, out Int64 aikapaatos) && Int64.TryParse(varaus, out Int64 varausaika))
            {
                lasku = (aikapaatos - varausaika) / 60;
                tiedot.Text = tiedot.Text + lasku.ToString() + " min";
            }
        }

        private void naytahinnat()
        {
            hintalistaus.Items.Clear();

            for(int x = 1; x < 6; x++)
            {
                ListViewItem hinta = new ListViewItem(teevee.hae("luokka", "hinnat", "ID", x.ToString()));
                string thishinta = teevee.hae("pertunti", "hinnat", "ID", x.ToString());
                hinta.SubItems.Add(thishinta + "€");

                float pertunti = float.Parse(thishinta) / 60;

                for(int i = 1; i < 5; i++)
                {
                    float eurot = (lasku * pertunti) / i;
                    hinta.SubItems.Add((Math.Round(eurot * 20) /20).ToString() + "€");
                }

                hintalistaus.Items.Add(hinta);
            }

            hintalistaus.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void varmistus_KeyUp(object sender, KeyEventArgs e)
        {
            if (varmistus.Text == "e" || varmistus.Text == "E")
            {
                this.Close();
            }

            if(varmistus.Text == "k" || varmistus.Text == "K")
            {
                    Form1.paivittaja.paatavaraus(poytanro.ToString(), " "); //lähetetään hinta funktioon, joka päättää varauksen.

                    Form1.paivittaja.paivita(); //Päivitetään näkymä pääikkunassa

                    this.Close();
            }
        }

        private void Form3_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close(); //Ikkunan voi sulkea myös ESCiä painamalla
            }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.paivittaja.Show();
        }
    }
}
