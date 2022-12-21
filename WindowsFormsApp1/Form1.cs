using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();            
        }

        private void button1_Click(object sender, EventArgs e)
        {          
            eventBindingSource.DataSource = (new bdEntities1()).events.ToList();
            MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();
            sb.Server = "127.0.0.1";
            sb.Database = "bd";
            sb.UserID = "root";
            sb.Password = "1234";

            string query = @"select * from employee;";
            using (MySqlConnection connection = new MySqlConnection())
            {
                connection.ConnectionString = sb.ConnectionString;
                MySqlCommand com = new MySqlCommand(query, connection);
                query = @"select ed.id, concat(e.surname, ' ', e.name, ' ', e.middle_name, ' ') as `ФИО`, d.name as `Отдел`, p.name as `Должность`
                        from employee_department as ed
                        join employee as e on e.id = ed.employee_id
                        join department as d on d.id = ed.department_id
                        join position as p on p.id = ed.position_id;";
                MySqlCommand com1 = new MySqlCommand(query, connection);
                try
                {
                    connection.Open();
                    // чтение в цикле
                    using (MySqlDataReader dr = com.ExecuteReader())
                    {
                        dataGridView1.Columns.Clear();
                        dataGridView1.Rows.Clear();
                        if (dr.HasRows)
                        {
                            for (int i = 0; i < dr.FieldCount; i++)
                                dataGridView1.Columns.Add(dr.GetName(i), dr.GetName(i));
                        }
                        while (dr.Read())
                        {
                            StringBuilder builder = new StringBuilder();
                            for (int i = 0; i < dr.FieldCount; i++)
                                builder.Append(dr[i].ToString() + ";");
                            dataGridView1.Rows.Add(builder.ToString().Split(';'));
                        }
                    }
                    // подключение источника данных к data aware компоненту
                    using (MySqlDataReader dr = com1.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        if (dr.HasRows)
                        {
                            dt.Load(dr);
                        }
                        dataGridView2.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                connection.Close();

            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridViewEntity.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }
    }
}
