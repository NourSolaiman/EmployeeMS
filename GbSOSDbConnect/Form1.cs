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

        TextBox[] txtBoxesPerson;

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

            //Skapa en Array Ref för input fält
            txtBoxesPerson = new TextBox[] { txtName, txtAge };
        }

        private void InsertPersonToDB()
        {
            //Validering
            bool valid = true;

            foreach (TextBox txtBox in txtBoxesPerson)
            {
                //Trimmar test-innehållet
                txtBox.Text = txtBox.Text.Trim();

                //Kontrollera att txtBox har text
                if (txtBox.Text == "")
                {
                    //Validering har misslyckats
                    valid = false;
                    txtBox.BackColor = Color.IndianRed;
                } else
                {
                    txtBox.BackColor = TextBox.DefaultBackColor;
                }
            }

            //Kontrollera valid
            if (!valid)
            {
                MessageBox.Show("Felaktig validering. Kontrollera röda fält.");
                return;
            }

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

        private void SelectPersonsFromDB(string keyword = "")
        {
            //Skapa en SQL Querry
            string sqlQuerry;

            if (keyword == "") sqlQuerry = $"CALL selectPeople();";
            else               sqlQuerry = $"CALL searchPeople('{keyword}');";

            //Skapa ett MySQLCommand objekt
            MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);

            //Exekvera querry mot DB. Få data tillbaka
            try
            {
                //Öppnar koppling till DB
                conn.Open();

                //Exekvera cmd
                MySqlDataReader reader = cmd.ExecuteReader();

                //Placera data i en DataTable objekt
                DataTable dt = new DataTable();
                dt.Load(reader);

                //Koppla TD objekt som DataSource till Grid
                gridPeopleOutput.DataSource = dt;

                //Ladda Reader på Nytt
                reader = cmd.ExecuteReader();

                //Tömma statisk lista
                Person.people.Clear();

                //While loop för att spara datan lokalt i en lista
                while(reader.Read())
                {
                    //Hämta och spara data till variabler
                    int id = Convert.ToInt32(reader["people_id"]);
                    string name = reader["people_name"].ToString();
                    int age = Convert.ToInt32(reader["people_age"]);

                    //Skapa ett Person obejkt och spara i statisk lista
                    Person.people.Add(new Person(id, name, age));
                }

                //Stänga koppling till DB
                conn.Close();
            } catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            //Enabla knapp för Update och Delete
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SelectPersonsFromDB();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SelectPersonsFromDB( txtSearch.Text );
        }

        private void gridPeopleOutput_SelectionChanged(object sender, EventArgs e)
        {
            GetRowData();
        }

        private void GetRowData()
        {
            //Kontrollera att vi har en markerad rad i grid
            if (gridPeopleOutput.SelectedRows.Count != 1) return;

            //Hämta data från grid
            DataGridViewSelectedRowCollection row = gridPeopleOutput.SelectedRows;
            int id = Convert.ToInt32( row[0].Cells[0].Value );

            //Skriva in data från grid till formulär
            foreach(Person person in Person.people)
            {
                // Kontrollera ID property
                if (person.Id == id)
                {
                    //Rätt objekt hittat
                    txtName.Text = person.Name;
                    txtAge.Text = person.Age.ToString();
                    break;
                }
            }
        }

        private void UpdatePersonToDB()
        {
            //Kontrollera att vi har en markerad rad i grid
            if (gridPeopleOutput.SelectedRows.Count != 1) return;

            //Hämta data från grid
            DataGridViewSelectedRowCollection row = gridPeopleOutput.SelectedRows;
            int id = Convert.ToInt32(row[0].Cells[0].Value);

            //hämtar värden från textfält
            string name = txtName.Text;
            int age = Convert.ToInt32(txtAge.Text);

            //Skapar en SQL Querry
            string SqlQuerry = $"CALL updatePeople({id}, '{name}', {age});";

            //MySqlCommand
            MySqlCommand cmd = new MySqlCommand(SqlQuerry, conn);

            try
            {
                //Öppna koppling till DB
                conn.Open();

                //Exekverar commando
                cmd.ExecuteReader();

                conn.Close();
            } catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            //Hämta den nya datan
            SelectPersonsFromDB();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdatePersonToDB();
        }
    }
}
