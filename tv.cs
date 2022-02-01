using System.Data.SQLite; //SQLite kirjasto, jota käytetään tietokantayhteyksiin


namespace biljardipoytateoria
{
    class Tv //lyhenne sanoista tiedon välittäjä
    {
        SQLiteConnection tietoyhteys; //yhteys tietokantaan

        public void initsql()
        {
        tietoyhteys = new SQLiteConnection("Data Source=poydat.sqlite;Version=3"); //tietokantayhteys luodaan
        tietoyhteys.Open(); //tietokantayhteys avataan
        }

        public string hae(string haettava, string taulu, string paamaara, string tahto)
        {
            string haeinfo = "SELECT "+haettava+" FROM "+taulu+" WHERE "+paamaara+" = " + tahto + ";";
            SQLiteCommand hakija = new SQLiteCommand(haeinfo, tietoyhteys);
            string info = hakija.ExecuteScalar().ToString();

            return info;
        }

        public string nykyhetki()
        {
            string haeinfo = "SELECT strftime('%s', 'now')";
            SQLiteCommand hakija = new SQLiteCommand(haeinfo, tietoyhteys);
            string info = hakija.ExecuteScalar().ToString();

            return info;
        }

        public string nykypaiva()
        {
            string haeinfo = "SELECT strftime('%d/%m','now','localtime')";
            SQLiteCommand hakija = new SQLiteCommand(haeinfo, tietoyhteys);
            string info = hakija.ExecuteScalar().ToString();

            return info;
        }

        public string nykytunti()
        {
            string haeinfo = "SELECT strftime('%H:%M','now','localtime')";
            SQLiteCommand hakija = new SQLiteCommand(haeinfo, tietoyhteys);
            string info = hakija.ExecuteScalar().ToString();

            return info;
        }

        public void muunna(string taulu, string muunnettava, string muutos, string tahto, string paamaara)
        {
            string paivitys = "UPDATE "+taulu+" SET "+muunnettava+" = "+muutos+" WHERE "+tahto+" = " + paamaara + ";";
            SQLiteCommand komento = new SQLiteCommand(paivitys, tietoyhteys);
            komento.ExecuteNonQuery();
        }

        public void rajayta(string taulu)
        {
            string rmsql = "DELETE FROM "+taulu+";";
            SQLiteCommand rajayttaja = new SQLiteCommand(rmsql, tietoyhteys);
            rajayttaja.ExecuteNonQuery();
        }
        
        public void seekanddestroy(string taulu, string paamaara, string kohde)
        {
            string rmsql = "DELETE FROM " + taulu + " WHERE " + paamaara + " = " + kohde + ";";
            SQLiteCommand poistaja = new SQLiteCommand(rmsql, tietoyhteys);
            poistaja.ExecuteNonQuery();
        }

        public void lisaa(string taulu, string paamaara, string tahto)
        {
            string lisays = "INSERT INTO " + taulu + paamaara + " VALUES " + tahto + "";
            SQLiteCommand lisaaja = new SQLiteCommand(lisays, tietoyhteys);
            lisaaja.ExecuteNonQuery();
        }
    }
}
