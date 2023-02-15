using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployyeManageSys
{
    public partial class Form1 : Form
    {
        // Skapa ref för MySQLConnection objekt
        MySqlConnection conn;

        TextBox[] txtBoxesDepartments;
        TextBox[] txtBoxesEmployees;
        TextBox[] txtBoxesDep_Emp;
        public Form1()
        {
            InitializeComponent();

            //Bygg upp MySQLCOnneection obejkt
            string server = "localhost";
            string database = "gbsosdbapplikation";
            string user = "root";
            string password = "Mns@19741111";

            string connString = $"SERVER={server};DATABASE={database};UID={user};PASSWORD={password};";

            conn = new MySqlConnection(connString);

            //Skapa en Array Ref för input fält
            txtBoxesDepartments = new TextBox[] { txtName, txtFloor };
            txtBoxesEmployees = new TextBox[] { txtEmployeeName, txtEmployyeAddress, txtPhoneNumber,txtEmail};
            txtBoxesDep_Emp = txtBoxesDepartments.Concat(txtBoxesEmployees).ToArray();
        }

        private void InsertDepartmentToDB()
        {
            //Validering
            bool valid = true;

            foreach (TextBox txtBox in txtBoxesDepartments)
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
            string depName = txtName.Text;
            int floor = Convert.ToInt32( txtFloor.Text);

            //Bygg upp SQL querry
            string sqlQuerry = $"CALL insertDepartment('{depName}', {floor});";

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
            SelectDepartmentFromDB();
        }

        private void btnInsertDepToDB(object sender, EventArgs e)
        {
            InsertDepartmentToDB();
        }

        private void SelectDepartmentFromDB(string keyword = "")
        {
            //Skapa en SQL Querry
            string sqlQuerry;

            if (keyword == "") sqlQuerry = $"CALL selectDepartment();";
            else               sqlQuerry = $"CALL searchDepartment('{keyword}');";

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
                gridDepartmentOutput.DataSource = dt;

                //Ladda Reader på Nytt
                reader = cmd.ExecuteReader();

                //Tömma statisk lista
                Department.department.Clear();

                //While loop för att spara datan lokalt i en lista
                while(reader.Read())
                {
                    //Hämta och spara data till variabler
                    int id = Convert.ToInt32(reader["departments_id"]);
                    string name = reader["departments_name"].ToString();
                    int floor = Convert.ToInt32(reader["departments_floor"]);

                    //Skapa ett Department obejkt och spara i statisk lista
                    Department.department.Add(new Department(id, name, floor));
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
            btnUpdateEmployee.Enabled = true;
        }
        private void btnSelectDepFromDB(object sender, EventArgs e)
        {
            SelectDepartmentFromDB();
           
        }

        private void SelectEmployeesFromDB(string keyword = "")
        {
            //Skapa en SQL Querry
            string sqlQuerry;

            if (keyword == "") sqlQuerry = $"CALL selectEmployees();";
            else sqlQuerry = $"CALL searchEmployee('{keyword}');";
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
                gridEmployeeOutput.DataSource = dt;

                //Ladda Reader på Nytt
                reader = cmd.ExecuteReader();

                //Tömma statisk lista
                Employee.employee.Clear();


                //While loop för att spara datan lokalt i en lista
                while (reader.Read())
                {
                    //Hämta och spara data till variabler
                    int id = Convert.ToInt32(reader["employees_id"]);
                    string name = reader["employees_name"].ToString();
                    string address = reader["employees_address"].ToString();
                    string phoneNumber = reader["employees_phone_number"].ToString();
                    string email = reader["employees_email"].ToString();                    

                    //Skapa ett Department obejkt och spara i statisk lista
                    Employee.employee.Add(new Employee(id, name,address, phoneNumber,email));
                }

                //Stänga koppling till DB
                conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            } 
            
        }

        
        private void btnSearchDep(object sender, EventArgs e)
        {
            SelectDepartmentFromDB( txtSearch.Text );
        }

        private void btnSearchEmp(object sender, EventArgs e)
        {
            SelectEmployeesFromDB(txtSearch2.Text);
        }

        private void gridDepartmentsOutput_SelectionChanged(object sender, EventArgs e)
        {
            GetRowData();
        }
      
        private void GetRowData()
        {
            //Kontrollera att vi har en markerad rad i grid
            if (gridDepartmentOutput.SelectedRows.Count != 1) return;

            //Hämta data från grid
            DataGridViewSelectedRowCollection row = gridDepartmentOutput.SelectedRows;
            int id = Convert.ToInt32( row[0].Cells[0].Value );

            //Skriva in data från grid till formulär
            foreach(Department dep in Department.department)
            {
                // Kontrollera ID property
                if (dep.DepId == id)
                {
                    //Rätt objekt hittat
                    txtName.Text = dep.DepName;
                    txtFloor.Text = dep.Floor.ToString();
                    break;
                }
            }

            //Uppdatera Employees grid via Departments ID
            GetEmployeesByDepartments(id);
        }

       
        private void UpdateDepartmentsToDB()
        {
            //Kontrollera att vi har en markerad rad i grid
            if (gridDepartmentOutput.SelectedRows.Count != 1) return;

            //Hämta data från grid
            DataGridViewSelectedRowCollection row = gridDepartmentOutput.SelectedRows;
            int id = Convert.ToInt32(row[0].Cells[0].Value);

            //hämtar värden från textfält
            string name = txtName.Text;
            int floor = Convert.ToInt32(txtFloor.Text);

            //Skapar en SQL Querry
            string SqlQuerry = $"CALL updateDepartment({id}, '{name}', {floor});";

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
            SelectDepartmentFromDB();
        }

        private void UpdateEmployeesToDB()
        {
            //Kontrollera att vi har en markerad rad i grid
            if (gridEmployeeOutput.SelectedRows.Count != 1) return;

            //Hämta data från grid
            DataGridViewSelectedRowCollection row = gridEmployeeOutput.SelectedRows;
            int id = Convert.ToInt32(row[0].Cells[0].Value);

            //hämtar värden från textfält

            string employeesName = txtEmployeeName.Text;
            string employeesAddress = txtEmployyeAddress.Text;
            string employeesPhoneNumber = txtPhoneNumber.Text;
            string employeesEmail = txtEmail.Text;
          

            //Skapar en SQL Querry
            string SqlQuerry = $"CALL updateEmployee({id}, '{employeesName}', '{employeesAddress}', '{employeesPhoneNumber}', '{employeesEmail}');";

            //MySqlCommand
            MySqlCommand cmd = new MySqlCommand(SqlQuerry, conn);

            try
            {
                //Öppna koppling till DB
                conn.Open();

                //Exekverar commando
                cmd.ExecuteReader();

                conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            

            //Hämta den nya datan
           SelectEmployeesFromDB();
        }
        private void btnUpdateDep(object sender, EventArgs e)
        {
            UpdateDepartmentsToDB();
        }

        private void btnDeleteDep(object sender, EventArgs e)
        {
            DeleteDepartments();
        }
        private void DeleteDepartments()
        {
            //Kontrollera att vi har en markerad rad i grid
            if (gridDepartmentOutput.SelectedRows.Count != 1) return;

            //Hämta data från grid
            DataGridViewSelectedRowCollection row = gridDepartmentOutput.SelectedRows;
            int id = Convert.ToInt32(row[0].Cells[0].Value);

            //Skapar en SQL Querry
            string SqlQuerry = $"CALL deleteDepartment({id});";

            //MySqlCommand
            MySqlCommand cmd = new MySqlCommand(SqlQuerry, conn);

            try
            {
                //Öppna koppling till DB
                conn.Open();

                //Exekverar commando
                cmd.ExecuteReader();

                conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            //Hämta den nya datan
            SelectDepartmentFromDB();
        }

        private void GetEmployeesByDepartments(int id)
        {
            //Sql querry (för att hitta employee by department)
            string SqlQuerry = $"CALL selectEmployeeByDepartment({id});";

            //Command
            MySqlCommand cmd = new MySqlCommand(SqlQuerry, conn);

            try
            {
                //Öppna koppling
                conn.Open();

                //Exekvera kommando
                MySqlDataReader reader = cmd.ExecuteReader();

                //Skapa och fyll upp dataTable
                DataTable dt = new DataTable();
                dt.Load(reader);

                //Koppla dt till DatSource i Grid
                gridEmployeeOutput.DataSource = dt;

                //Stäng koppling
                conn.Close();
            } catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void btnAddEmployeeToD(object sender, EventArgs e)
        {
            AddEmployeesToDepartments();
        }

        private void AddEmployeesToDepartments()
        {
            //Kontrollera att vi har en markerad rad i grid
            if (gridDepartmentOutput.SelectedRows.Count != 1) return;
         

            //Kontrollera att Employee formuläret har inmatade värden
            bool valid = true;

            foreach (TextBox txtBox in txtBoxesEmployees)
            {
                //Trimmar test-innehållet
                txtBox.Text = txtBox.Text.Trim();

                //Kontrollera att txtBox har text
                if (txtBox.Text == "")
                {
                    //Validering har misslyckats
                    valid = false;
                    txtBox.BackColor = Color.IndianRed;
                }
                else
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
                       
            //Hämta data från grid
            DataGridViewSelectedRowCollection row = gridDepartmentOutput.SelectedRows;
            int id = Convert.ToInt32(row[0].Cells[0].Value);

            string employeesName = txtEmployeeName.Text;
            string employeesAddress = txtEmployyeAddress.Text;
            string employeesPhoneNumber = txtPhoneNumber.Text;
            string employeesEmail = txtEmail.Text;
            



            //Skapar en SQL Querry

            string SqlQuerry = $"CALL insertemployeeToDepartment( {id}, '{employeesName}', '{employeesAddress}', '{employeesPhoneNumber}', '{employeesEmail}');";         

                      
            //MySqlCommand
            MySqlCommand cmd = new MySqlCommand(SqlQuerry, conn);

            try
            {
                //Öppna koppling till DB
                conn.Open();

                //Exekverar commando
                cmd.ExecuteReader();

                conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            //Uppdatera EmployeeGrid
            GetEmployeesByDepartments(id);

            MessageBox.Show("Insert finished successfully!");

            txtEmployeeName.Clear();
            txtEmployyeAddress.Clear();
            txtPhoneNumber.Clear();
            txtEmail.Clear();
        }

        private void btnSelectemployee(object sender, EventArgs e)
        {
            SelectEmployeesFromDB();
        }

        private void AddEmployeesToNewDepartment()
        {
            //Validering
            //Kontrollera att både Department och employyes formuläret har inmatade värden
            bool valid = true;

            foreach (TextBox txtBox in txtBoxesDepartments)
            {
                //Trimmar test-innehållet
                txtBox.Text = txtBox.Text.Trim();

                //Kontrollera att txtBox har text
                if (txtBox.Text == "")
                {
                    //Validering har misslyckats
                    valid = false;
                    txtBox.BackColor = Color.IndianRed;
                }
                else
                {
                    txtBox.BackColor = TextBox.DefaultBackColor;
                }
            }

            foreach (TextBox txtBox in txtBoxesDep_Emp)
            {
                //Trimmar test-innehållet
                txtBox.Text = txtBox.Text.Trim();

                //Kontrollera att txtBox har text
                if (txtBox.Text == "")
                {
                    //Validering har misslyckats
                    valid = false;
                    txtBox.BackColor = Color.IndianRed;
                }
                else
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
                     
                       
            //Hämta data och exekvera SQL
            string departmentsName = txtName.Text;
            int departmentsFloor = Convert.ToInt32(txtFloor.Text);
            string employeesName = txtEmployeeName.Text;
            string employeesAddress = txtEmployyeAddress.Text;
            string employeesPhoneNumber = txtPhoneNumber.Text;
            string employeesEmail = txtEmail.Text;


            //Skapa SQL querry
            string sqlQuerry = $"CALL InsertNewEmployeeToNewDepartment('{departmentsName}', '{departmentsFloor}', '{employeesName}', '{employeesAddress}', '{employeesPhoneNumber}', '{employeesEmail}');";

            //Skapa Command objekt
            MySqlCommand cmd = new MySqlCommand(sqlQuerry, conn);

            try
            {
                //Öppna koppling, exekvera och stäng koppling
                conn.Open();
                cmd.ExecuteReader();
                conn.Close();
            } catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            //Hämta data till Department Tabellen
            SelectDepartmentFromDB();

            //Markera den nya personen i grid
            gridDepartmentOutput.Rows[gridDepartmentOutput.Rows.Count - 2].Selected = true;

            //Hämta data till Pet Tabellen
            GetRowData();
            txtName.Clear();
            txtFloor.Clear();
            txtEmployeeName.Clear();
            txtEmployyeAddress.Clear();
            txtPhoneNumber.Clear();
            txtEmail.Clear();
        }

        private void btnAddEmployeeToNewDep(object sender, EventArgs e)
        {
            AddEmployeesToNewDepartment();
        }

        private void btnUpdateEmployeeToDB(object sender, EventArgs e)
        {
            UpdateEmployeesToDB ();
        }

        
    }
}
