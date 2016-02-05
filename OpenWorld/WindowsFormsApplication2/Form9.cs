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

namespace StrategyGame
{
    public partial class Form9 : Form
    {
        string sqlcon = Variables.sqlstring;

        public Form9(string username, string me)
        {
            InitializeComponent();

            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.None;
            label2.Text = username;
            label3.Text = me;

            filldatagrid();

            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM agreement where (player_a=@p3 and player_b=@p4 and region_a='' and region_b='') or (player_a=@p4 and player_b=@p3 and region_a='' and region_b='')";
            command.Prepare();
            command.Parameters.AddWithValue("@p3", username);
            command.Parameters.AddWithValue("@p4", me);
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                pictureBox1.Visible = false;
                pictureBox2.Visible = false;
            }
            else
            {
                reader.Close();

                command.CommandText = "SELECT * FROM treaties where player_a=@p1 and player_b=@p2";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", username);
                command.Parameters.AddWithValue("@p2", me);
                reader = command.ExecuteReader();


                if (username == me)
                {
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = false;
                }
                else if (reader.Read())
                {
                    pictureBox1.Visible = false;
                }
                else
                {
                    pictureBox2.Visible = false;
                }

                reader.Close();
            }
            reader.Close();
            connection.Close();
        }

        public void filldatagrid()
        {
            string str = sqlcon;
            MySqlConnection con = new MySqlConnection(str);

            string com = "Select id,name,surname,username,gold,military,political,diplomatic,trade,rev_sum,cost_sum from player where username='" + label2.Text + "'" ;
            MySqlDataAdapter adpt = new MySqlDataAdapter(com, con);
            DataSet myDataSet = new DataSet();
            adpt.Fill(myDataSet, "player");
            DataTable myDataTable = myDataSet.Tables[0];
            //DataRow tempRow = null;

            DataRow tempRow1 = null;

            foreach (DataRow tempRow1_Variable in myDataTable.Rows)
            {
                tempRow1 = tempRow1_Variable;
                label1.Text = Convert.ToString(tempRow1["name"] + " " + tempRow1["surname"]);
            }

            dataGridView1.DataSource = myDataSet;
            dataGridView1.DataMember = "player";
            dataGridView1.Columns[0].HeaderText = "Κωδικός Παίκτη";
            dataGridView1.Columns[1].HeaderText = "Όνομα Παίκτη";
            dataGridView1.Columns[2].HeaderText = "Επώνυμο Παίκτη";
            dataGridView1.Columns[3].HeaderText = "Username";
            dataGridView1.Columns[4].HeaderText = "Χρύσος";
            dataGridView1.Columns[5].HeaderText = "Στρατιωτική Ικανότητα";
            dataGridView1.Columns[6].HeaderText = "Πολιτική Ικανότητα";
            dataGridView1.Columns[7].HeaderText = "Διπλωματική Ικανότητα";
            dataGridView1.Columns[8].HeaderText = "Εμπορική Ικανότητα";
            dataGridView1.Columns[9].HeaderText = "Έσοδα";
            dataGridView1.Columns[10].HeaderText = "Έξοδα";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            string player1 = label3.Text;
            string player2 = label2.Text;

            int i = 1;
            int flag = 0;

            while (flag == 0)
            {
                command.CommandText = "SELECT * FROM agreement where id=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", i);
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    i = i + 1;
                }
                else
                {
                    flag = 1;
                }
                reader.Close();
                command.Parameters.Clear();
            }

            command.CommandText = "insert into agreement (id,player_a, player_b) values (@p2,@p3, @p4)";
            command.Prepare();
            command.Parameters.AddWithValue("@p2", i);
            command.Parameters.AddWithValue("@p3", player1);
            command.Parameters.AddWithValue("@p4", player2);
            command.ExecuteNonQuery();

            //string player1 = label2.Text;
            //string player2 = label3.Text;

            //command.CommandText = "insert into treaties (player_a, player_b) values (@p3, @p4)";
            //command.Prepare();
            //command.Parameters.AddWithValue("@p3", player1);
            //command.Parameters.AddWithValue("@p4", player2);
            //command.ExecuteNonQuery();

            //command.CommandText = "insert into treaties (player_a, player_b) values (@p5, @p6)";
            //command.Prepare();
            //command.Parameters.AddWithValue("@p5", player2);
            //command.Parameters.AddWithValue("@p6", player1);
            //command.ExecuteNonQuery();

            connection.Close();

            MessageBox.Show("Η Aίτηση Συμφωνίας στάλθηκε στον " + label1.Text);
            pictureBox1.Visible = false;
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();

            string player1 = label2.Text;
            string player2 = label3.Text;

            command.CommandText = "DELETE FROM treaties Where player_a=@p3 and player_b=@p4";
            command.Prepare();
            command.Parameters.AddWithValue("@p3", player1);
            command.Parameters.AddWithValue("@p4", player2);
            command.ExecuteNonQuery();

            command.CommandText = "DELETE FROM treaties Where player_a=@p5 and player_b=@p6";
            command.Prepare();
            command.Parameters.AddWithValue("@p5", player2);
            command.Parameters.AddWithValue("@p6", player1);
            command.ExecuteNonQuery();


            connection.Close();

            MessageBox.Show("Η συμφωνία ακυρώθηκε.");
            pictureBox2.Visible = false;
            pictureBox1.Visible = true;
        }
    }
}
