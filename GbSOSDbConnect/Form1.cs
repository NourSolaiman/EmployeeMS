using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GbSOSDbConnect
{
    public partial class Form1 : Form
    {
        // Skapa ref för MySQLConnection objekt
        MySqlConnection conn;

        public Form1()
        {
            InitializeComponent();

            //Bygg upp MySQLCOnneection obejkt
            string server = "localhost";
            string database = "gbsosdbapplikation";
            string user = "root";
            string password = "SokrateS13";

            string connString = $"SERVER={server};DATABASE={database};UID={user};PASSWORD={password};";

            conn = new MySqlConnection(connString);
        }

        private void InsertPersonToDB()
        {
            //Hämta värden från textfält
            string name = txtName.Text;
            int age = Convert.ToInt32( txtAge.Text);

            //Bygg upp SQL querry
            string sqlQuerry = $"CALL insertPeople('{name}', {age});";

            //Skapar ett MySqlCOmmand objekt
            MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);

            //Skapa ett TryCatch Block
            try
            {
                //Öppna Connection
                conn.Open();

                //Exekvera kommando
                cmd.ExecuteReader();

                //Stänga Connection
                conn.Close();
            } catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            //Bekräftelse till användare
            MessageBox.Show("Insert Finished Successfully!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InsertPersonToDB();
        }
    }
}
