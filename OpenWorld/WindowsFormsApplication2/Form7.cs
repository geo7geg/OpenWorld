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
    public partial class Form7 : Form
    {
        Dictionary<string, string> vars = Variables.vars();
        string sqlcon = Variables.sqlstring;

        public Form7(string username)
        {
            InitializeComponent();

            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.None;
            textBox1.Text = username;
            
            listView1.View = View.Details;
            listView1.Columns.Add("Κωδικός Αίτησης", 70, HorizontalAlignment.Left);
            listView1.Columns.Add("Αιτών", 110, HorizontalAlignment.Left);
            listView1.Columns.Add("Αποδέκτης", 110, HorizontalAlignment.Left);

            filldatagrid();
            fillrequest();

            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM player where username=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", username);
            reader = command.ExecuteReader();

            string enwsi = "";
            string sunaspismos = "";
            string enwsi_vote = "";

            if (reader.Read())
            {
                enwsi = reader["enwsi"].ToString();
                enwsi_vote = reader["enwsi_vote"].ToString();
                sunaspismos = reader["sunasp"].ToString();
            }

            reader.Close();

            int flag = 0;
            int flag1 = 0;

            command.CommandText = "SELECT * FROM sunaspismos where name=@p2";
            command.Prepare();
            command.Parameters.AddWithValue("@p2", sunaspismos);
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                pictureBox2.Visible = false;
                pictureBox3.Visible = true;
                flag = 1;
            }

            reader.Close();

            command.CommandText = "SELECT * FROM agreement where player_c=@p4 and sunaspismos!=''";
            command.Prepare();
            command.Parameters.AddWithValue("@p4", username);
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                pictureBox2.Visible = false;
            }

            reader.Close();

            command.CommandText = "SELECT * FROM enwsi where name=@p3";
            command.Prepare();
            command.Parameters.AddWithValue("@p3", enwsi);
            reader = command.ExecuteReader();
            string players = "";
            int status = 0;
            int leader_rem_rounds = 0;
            if (reader.Read())
            {
                string leader = reader["leader"].ToString();
                players = reader["players"].ToString();
                status = Convert.ToInt32(reader["status"]);
                leader_rem_rounds = Convert.ToInt32(reader["leader_rem_rounds"]);
                if (leader == username && leader_rem_rounds != 0 && status == 1)
                {
                    label3.Visible = true;
                    label5.Visible = true;
                    label6.Visible = true;
                    label8.Visible = true;
                    label9.Visible = true;
                    label10.Visible = true;
                    label11.Visible = true;
                    label12.Visible = true;
                    label13.Visible = true;
                    label14.Visible = true;
                    label15.Visible = true;
                    label16.Visible = true;
                    button4.Visible = true;
                    numericUpDown1.Visible = true;
                    numericUpDown2.Visible = true;
                    numericUpDown3.Visible = true;
                    numericUpDown4.Visible = true;
                    numericUpDown5.Visible = true;
                }
                pictureBox4.Visible = false;
                pictureBox5.Visible = true;
                flag1 = 1;
            }
            string[] players_arr = players.Split(',');

            reader.Close();

            if ((enwsi_vote == "" && enwsi != "" && status == 1 && leader_rem_rounds == 0) || (enwsi_vote == "" && enwsi != "" && status == 0))
            {
                comboBox1.Visible = true;
                button5.Visible = true;
                button6.Visible = false;
                label16.Visible = true;

                //players_arr = players_arr.Where(w => w != username).ToArray(); 

                comboBox1.DataSource = players_arr;
            }
            else if (enwsi_vote != "" && enwsi != "" && status == 1 && leader_rem_rounds != 0)
            {
                comboBox1.Visible = false;
                button5.Visible = false;
                button6.Visible = true;
                label16.Visible = true;
            }
            else if (enwsi == "")
            {
                comboBox1.Visible = false;
                button5.Visible = false;
                button6.Visible = false;
                label16.Visible = false;
            }

            command.CommandText = "SELECT * FROM agreement where player_c=@p5 and enwsi!=''";
            command.Prepare();
            command.Parameters.AddWithValue("@p5", username);
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                pictureBox4.Visible = false;
            }

            if ((flag + flag1) == 2)
            {
                pictureBox1.Visible = false;
            }
        }

        public void fillrequest()
        {
            string str = sqlcon;
            MySqlConnection con = new MySqlConnection(str);
           

            string com = "Select * from agreement where player_b='" + textBox1.Text + "'";
            MySqlDataAdapter adpt = new MySqlDataAdapter(com, con);
            DataSet myDataSet = new DataSet();
            adpt.Fill(myDataSet, "agreement");
            DataTable myDataTable = myDataSet.Tables[0];
            DataRow tempRow = null;

            string com1 = "Select * from agreement where leader='" + textBox1.Text + "' and sunaspismos!=''";
            MySqlDataAdapter adpt1 = new MySqlDataAdapter(com1, con);
            DataSet myDataSet1 = new DataSet();
            adpt1.Fill(myDataSet1, "agreement");
            DataTable myDataTable1 = myDataSet1.Tables[0];
            DataRow tempRow1 = null;

            string com2 = "Select * from agreement where leader='" + textBox1.Text + "' and enwsi!=''";
            MySqlDataAdapter adpt2 = new MySqlDataAdapter(com2, con);
            DataSet myDataSet2 = new DataSet();
            adpt2.Fill(myDataSet2, "agreement");
            DataTable myDataTable2 = myDataSet2.Tables[0];
            DataRow tempRow2 = null;

            listView1.Items.Clear();

            con.Open();
            foreach (DataRow tempRow_Variable in myDataTable.Rows)
            {
                
                tempRow = tempRow_Variable;
                //listBox5.Items.Add((tempRow["id"]));

                ListViewItem lvi = new ListViewItem(tempRow["id"].ToString());
                if (tempRow["region_a"].ToString() != "")
                {
                    lvi.SubItems.Add(tempRow["region_a"].ToString());
                    lvi.SubItems.Add(tempRow["region_b"].ToString());
                }
                else
                {
                    lvi.SubItems.Add(tempRow["player_a"].ToString());
                    lvi.SubItems.Add(tempRow["player_b"].ToString());
                }
                
                
                lvi.ForeColor = Color.Red;

                // You also have access to the list view's SubItems collection
                lvi.SubItems[0].ForeColor = Color.Blue;
                lvi.SubItems[1].ForeColor = Color.Blue;
                lvi.SubItems[2].ForeColor = Color.Blue;
                
                // Add the list items to the ListView
                listView1.Items.Add(lvi);
            }

            foreach (DataRow tempRow_Variable1 in myDataTable1.Rows)
            {
                tempRow1 = tempRow_Variable1;
                //listBox5.Items.Add((tempRow["id"]));

                ListViewItem lvi = new ListViewItem(tempRow1["id"].ToString());
                lvi.SubItems.Add(tempRow1["player_c"].ToString());
                lvi.SubItems.Add(tempRow1["sunaspismos"].ToString());
             
                lvi.ForeColor = Color.Red;

                // You also have access to the list view's SubItems collection
                lvi.SubItems[0].ForeColor = Color.Blue;
                lvi.SubItems[1].ForeColor = Color.Blue;
                lvi.SubItems[2].ForeColor = Color.Blue;

                // Add the list items to the ListView
                listView1.Items.Add(lvi);
            }

            foreach (DataRow tempRow_Variable2 in myDataTable2.Rows)
            {
                tempRow2 = tempRow_Variable2;
                //listBox5.Items.Add((tempRow["id"]));

                ListViewItem lvi = new ListViewItem(tempRow2["id"].ToString());
                lvi.SubItems.Add(tempRow2["player_c"].ToString());
                lvi.SubItems.Add(tempRow2["enwsi"].ToString());

                lvi.ForeColor = Color.Red;

                // You also have access to the list view's SubItems collection
                lvi.SubItems[0].ForeColor = Color.Blue;
                lvi.SubItems[1].ForeColor = Color.Blue;
                lvi.SubItems[2].ForeColor = Color.Blue;

                // Add the list items to the ListView
                listView1.Items.Add(lvi);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0 )
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;

                string item = listView1.SelectedItems[0].Text;

                command.CommandText = "SELECT * FROM agreement where id=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", item);
                reader = command.ExecuteReader();
                reader.Read();
                string player_a = reader["player_a"].ToString();
                string player_b = reader["player_b"].ToString();
                string region_a = reader["region_a"].ToString();
                string region_b = reader["region_b"].ToString();
                string player_c = reader["player_c"].ToString();
                string sunaspismos = reader["sunaspismos"].ToString();
                string enwsi = reader["enwsi"].ToString();
                string leader = reader["leader"].ToString();
                reader.Close();

                if (region_a != "")
                {
                    command.CommandText = "insert into reg_treaties (region_a, region_b) values (@p2, @p3)";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p2", region_a);
                    command.Parameters.AddWithValue("@p3", region_b);
                    command.ExecuteNonQuery();

                    command.CommandText = "insert into reg_treaties (region_a, region_b) values (@p4, @p5)";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p4", region_b);
                    command.Parameters.AddWithValue("@p5", region_a);
                    command.ExecuteNonQuery();
                }
                else if(player_a != "")
                {
                    command.CommandText = "insert into treaties (player_a, player_b) values (@p6, @p7)";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p6", player_a);
                    command.Parameters.AddWithValue("@p7", player_b);
                    command.ExecuteNonQuery();

                    command.CommandText = "insert into treaties (player_a, player_b) values (@p8, @p9)";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p8", player_b);
                    command.Parameters.AddWithValue("@p9", player_a);
                    command.ExecuteNonQuery();
                }
                else if(sunaspismos != "")
                {
                    command.CommandText = "SELECT * FROM sunaspismos where name=@p13";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p13", sunaspismos);
                    reader = command.ExecuteReader();
                    reader.Read();
                    string players = reader["players"].ToString();
                    int status = Convert.ToInt32(reader["status"]);
                    reader.Close();

                    command.CommandText = "UPDATE sunaspismos SET players=@p11 Where name=@p12 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p11", players + "," + player_c);
                    command.Parameters.AddWithValue("@p12", sunaspismos);
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE player SET sunasp=@p16 Where username=@p17 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p16", sunaspismos);
                    command.Parameters.AddWithValue("@p17", player_c);
                    command.ExecuteNonQuery();

                    if(status == 1)
                    {
                        command.Parameters.Clear();

                        command.CommandText = "UPDATE regions SET def_fact= def_fact + 1 Where owner=@p15 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p15", player_c);
                        command.ExecuteNonQuery();

                        Variables.fix_defence(player_c);
                    }
                }
                else
                {
                    command.CommandText = "SELECT * FROM enwsi where name=@p13";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p13", enwsi);
                    reader = command.ExecuteReader();
                    reader.Read();
                    string players = reader["players"].ToString();
                    int status = Convert.ToInt32(reader["status"]);
                    reader.Close();

                    command.CommandText = "UPDATE enwsi SET players=@p11 Where name=@p12 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p11", players + "," + player_c);
                    command.Parameters.AddWithValue("@p12", enwsi);
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE player SET enwsi=@p16, enwsi_vote=@p18 Where username=@p17 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p16", enwsi);
                    command.Parameters.AddWithValue("@p17", player_c);
                    command.Parameters.AddWithValue("@p18", textBox1.Text);
                    command.ExecuteNonQuery();

                    if (status == 1)
                    {
                        command.Parameters.Clear();

                        command.CommandText = "UPDATE regions SET def_fact= def_fact + 2 Where owner=@p15 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p15", player_c);
                        command.ExecuteNonQuery();

                        Variables.fix_defence(player_c);
                    }
                }

                command.CommandText = "DELETE FROM agreement Where id=@p10";
                command.Prepare();
                command.Parameters.AddWithValue("@p10", item);
                command.ExecuteNonQuery();

                connection.Close();
                MessageBox.Show("Η Aίτηση Συμφωνίας επικυρώθηκε");

                filldatagrid();
                fillrequest(); 
            }else
            {
                MessageBox.Show("Επιλέξτε τον κωδικό αιτήματος");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0 )
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();

                string item = listView1.SelectedItems[0].Text;

                command.CommandText = "DELETE FROM agreement Where id=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", item);
                command.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Η Aίτηση Συμφωνίας ακυρώθηκε");
                fillrequest(); 
            }else
            {
                MessageBox.Show("Επιλέξτε τον κωδικό αιτήματος");
            }
        }

        public void filldatagrid()
        {
            string str = sqlcon;
            MySqlConnection con = new MySqlConnection(str);

            string com = "Select * from treaties where player_a='" + textBox1.Text + "'";
            MySqlDataAdapter adpt = new MySqlDataAdapter(com, con);
            DataSet myDataSet = new DataSet();
            adpt.Fill(myDataSet, "treaties");
            DataTable myDataTable = myDataSet.Tables[0];
            //DataRow tempRow = null;

            dataGridView1.DataSource = myDataSet;
            dataGridView1.DataMember = "treaties";
            dataGridView1.Columns[0].HeaderText = "Παίκτης Α";
            dataGridView1.Columns[1].HeaderText = "Παίκτης Β";

            string com1 = "Select * from reg_treaties";
            MySqlDataAdapter adpt1 = new MySqlDataAdapter(com1, con);
            DataSet myDataSet1 = new DataSet();
            adpt1.Fill(myDataSet1, "reg_treaties");
            DataTable myDataTable1 = myDataSet1.Tables[0];
            DataRow tempRow1;

            string com2 = "Select * from reg_treaties where region_a='bgh'";
            MySqlDataAdapter adpt2 = new MySqlDataAdapter(com2, con);
            DataSet myDataSet2 = new DataSet();
            adpt2.Fill(myDataSet2, "reg_treaties");
            DataTable myDataTable2 = myDataSet2.Tables[0];

            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            int j = myDataTable1.Rows.Count - 1;
            for (int i = 0; i <= j; i++)
            {

                tempRow1 = myDataTable1.Rows[i];
                string region = Convert.ToString(tempRow1["region_a"]);

                command.CommandText = "SELECT * FROM regions where name=@p1 and owner=@p2";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", region);
                command.Parameters.AddWithValue("@p2", textBox1.Text);
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    myDataTable2.ImportRow(myDataTable1.Rows[i]);
                    //myDataSet2.Tables.Add(myDataTable2);     
                }

                reader.Close();
                command.Parameters.Clear();
            }


            connection.Close();
            dataGridView2.DataSource = myDataSet2;
            dataGridView2.DataMember = "reg_treaties";
            dataGridView2.Columns[0].HeaderText = "Περιοχή Α";
            dataGridView2.Columns[1].HeaderText = "Περιοχή Β";

            string com3 = "Select * from sunaspismos where 1";
            MySqlDataAdapter adpt3 = new MySqlDataAdapter(com3, con);
            DataSet myDataSet3 = new DataSet();
            adpt3.Fill(myDataSet3, "sunaspismos");
            DataTable myDataTable3 = myDataSet3.Tables[0];
            //DataRow tempRow = null;

            dataGridView3.DataSource = myDataSet3;
            dataGridView3.DataMember = "sunaspismos";
            dataGridView3.Columns[0].HeaderText = "Κωδικός";
            dataGridView3.Columns[1].HeaderText = "Όνομα";
            dataGridView3.Columns[2].HeaderText = "Ηγέτης";
            dataGridView3.Columns[3].HeaderText = "Παίκτες";
            dataGridView3.Columns[4].HeaderText = "Ένοπλοι";
            dataGridView3.Columns[5].HeaderText = "Υπόλοιπο Ενόπλων";
            dataGridView3.Columns[6].HeaderText = "Κατάσταση";

            dataGridView3.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            string com4 = "Select * from enwsi where 1";
            MySqlDataAdapter adpt4 = new MySqlDataAdapter(com4, con);
            DataSet myDataSet4 = new DataSet();
            adpt4.Fill(myDataSet4, "enwsi");
            DataTable myDataTable4 = myDataSet4.Tables[0];
            //DataRow tempRow = null;

            dataGridView4.DataSource = myDataSet4;
            dataGridView4.DataMember = "enwsi";
            dataGridView4.Columns[0].HeaderText = "Κωδικός";
            dataGridView4.Columns[1].HeaderText = "Όνομα";
            dataGridView4.Columns[2].HeaderText = "Κυβερνήτης";
            dataGridView4.Columns[3].HeaderText = "Υπόλοιπο Περιόδου Διακυβέρνησης \n (Γύροι)";
            dataGridView4.Columns[4].HeaderText = "Υπόλοιπο Περιόδων Ψηφοφορίας \n (Γύροι)";
            dataGridView4.Columns[5].HeaderText = "Υπόλοιπο Χρόνου Διάσπασης Ένωσης \n (Γύροι)";
            dataGridView4.Columns[6].HeaderText = "Παίκτες";
            dataGridView4.Columns[7].HeaderText = "Ένοπλοι";
            dataGridView4.Columns[8].HeaderText = "Υπόλοιπο Ενόπλων";
            dataGridView4.Columns[9].HeaderText = "Ποσοστό Παρακράτησης \n Χρυσού";
            dataGridView4.Columns[10].HeaderText = "Ποσοστό Αναδιανομής \n Αγροτών";
            dataGridView4.Columns[11].HeaderText = "Ποσοστό Αναδιανομής \n Τεχνιτών";
            dataGridView4.Columns[12].HeaderText = "Ποσοστό Αναδιανομής \n Εμπόρων";
            dataGridView4.Columns[13].HeaderText = "Ποσοστό Αναδιανομής \n Ενόπλων";
            dataGridView4.Columns[14].HeaderText = "Υπόλοιπο Αποθέματος \n Χρυσού";
            dataGridView4.Columns[15].HeaderText = "Κατάσταση";

            dataGridView4.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
            string player = dataGridView1.CurrentCell.Value.ToString();
            string me = textBox1.Text;
            //MySqlConnection connection = new MySqlConnection(sqlcon);
            //connection.Open();
            //MySqlCommand command = connection.CreateCommand();
            //MySqlDataReader reader;

            //command.CommandText = "SELECT * FROM regions where name=@p1";
            //command.Prepare();
            //command.Parameters.AddWithValue("@p1", city);
            //reader = command.ExecuteReader();
            //reader.Read();

            //string owner = Convert.ToString(reader["owner"]);
            //reader.Close();
                
            Form9 frm = new Form9(player,me);
            frm.Show();
                
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM player where username=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", textBox1.Text);
            reader = command.ExecuteReader();

            string enwsi = "";
            string sunaspismos = "";

            if (reader.Read())
            {
                enwsi = reader["enwsi"].ToString();
                sunaspismos = reader["sunasp"].ToString();
            }

            reader.Close();

            command.CommandText = "SELECT * FROM agreement where player_c=@p7";
            command.Prepare();
            command.Parameters.AddWithValue("@p7", textBox1.Text);
            reader = command.ExecuteReader();

            string enws = "";
            string sunasp = "";

            if (reader.Read())
            {
                enws = reader["enwsi"].ToString();
                sunasp = reader["sunaspismos"].ToString();
            }

            reader.Close();

            if (tabControl1.SelectedTab == tabPage3)
            {
                if (dataGridView3.CurrentCell != null)
                {
                    if (sunaspismos == "")
                    {
                        if (sunasp == "")
                        {
                            int i = 1;
                            int flag = 0;
                            command.Parameters.Clear();
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

                            command.CommandText = "SELECT * FROM sunaspismos where id=@p6";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p6", dataGridView3.CurrentCell.Value);
                            reader = command.ExecuteReader();
                            string name = "";
                            string leader = "";
                            if (reader.Read())
                            {
                                name = reader["name"].ToString();
                                leader = reader["leader"].ToString();
                                reader.Close();

                                command.CommandText = "insert into agreement (id,player_c, sunaspismos,leader) values (@p2,@p3, @p4, @p5)";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p2", i);
                                command.Parameters.AddWithValue("@p3", textBox1.Text);
                                command.Parameters.AddWithValue("@p4", name);
                                command.Parameters.AddWithValue("@p5", leader);
                                command.ExecuteNonQuery();

                                connection.Close();

                                textBox2.Visible = false;
                                pictureBox2.Visible = false;

                                MessageBox.Show("Η Aίτηση Συμφωνίας στάλθηκε στον " + leader);
                            }
                            else
                            {
                                MessageBox.Show("Επιλέξτε κωδικό συνασπισμού");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Έχετε στείλει ήδη αίτημα σε συνασπισμό");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Είστε ήδη μέλος ενός συνασπισμού");
                    }

                }
                else
                {
                    MessageBox.Show("Επιλέξτε κωδικό συνασπισμού");
                }
            }
            else if (tabControl1.SelectedTab == tabPage4)
            {
                if (dataGridView4.CurrentCell != null)
                {
                    if (enwsi == "")
                    {
                        if (enws == "")
                        {
                            int i = 1;
                            int flag = 0;
                            command.Parameters.Clear();
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

                            command.CommandText = "SELECT * FROM enwsi where id=@p6";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p6", dataGridView4.CurrentCell.Value);
                            reader = command.ExecuteReader();
                            string name = "";
                            string leader = "";
                            if (reader.Read())
                            {
                                name = reader["name"].ToString();
                                leader = reader["leader"].ToString();
                                reader.Close();

                                command.CommandText = "insert into agreement (id,player_c,enwsi,leader) values (@p2,@p3, @p4, @p5)";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p2", i);
                                command.Parameters.AddWithValue("@p3", textBox1.Text);
                                command.Parameters.AddWithValue("@p4", name);
                                command.Parameters.AddWithValue("@p5", leader);
                                command.ExecuteNonQuery();

                                connection.Close();

                                textBox3.Visible = false;
                                pictureBox4.Visible = false;

                                MessageBox.Show("Η Aίτηση Συμφωνίας στάλθηκε στον " + leader);
                            }
                            else
                            {
                                MessageBox.Show("Επιλέξτε κωδικό ένωσης");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Έχετε στείλει ήδη αίτημα σε άλλη ένωση");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Είστε ήδη μέλος μιας ένωσης");
                    }
                }
                else
                {
                    MessageBox.Show("Επιλέξτε κωδικό ένωσης");
                }
            }
            else
            {
                MessageBox.Show("Επιλέξτε κωδικό συνασπισμού ή ένωσης");
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (textBox2.Visible == true && textBox2.Text != "")
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;

                int i = 1;
                int flag = 0;

                while (flag == 0)
                {
                    command.CommandText = "SELECT * FROM sunaspismos where id=@p1";
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

                command.CommandText = "insert into sunaspismos (id,name,leader,players,army,army_live,status) values (@p2,@p3, @p4, @p5, @p6, @p7, @p8)";
                command.Prepare();
                command.Parameters.AddWithValue("@p2", i);
                command.Parameters.AddWithValue("@p3", textBox2.Text);
                command.Parameters.AddWithValue("@p4", textBox1.Text);
                command.Parameters.AddWithValue("@p5", textBox1.Text);
                command.Parameters.AddWithValue("@p6", 0);
                command.Parameters.AddWithValue("@p7", 0);
                command.Parameters.AddWithValue("@p8", 0);
                command.ExecuteNonQuery();

                command.CommandText = "UPDATE player SET sunasp=@p11 Where username=@p12 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p11", textBox2.Text);
                command.Parameters.AddWithValue("@p12", textBox1.Text);
                command.ExecuteNonQuery();
                connection.Close();

                textBox2.Visible = false;
                pictureBox2.Visible = false;

                filldatagrid();

                MessageBox.Show("Δημιουργήσατε Συνασπισμό με όνομα " + textBox2.Text);
            }
            else
            {
                textBox2.Visible = true;
                MessageBox.Show("Δώστε όνομα συνασπισμού");
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;
            connection.Open();

            string com4 = "Select * from sunaspismos where 1";
            MySqlDataAdapter adpt4 = new MySqlDataAdapter(com4, connection);
            DataSet myDataSet4 = new DataSet();
            adpt4.Fill(myDataSet4, "attacks");
            DataTable myDataTable4 = myDataSet4.Tables[0];
            DataRow tempRow4 = null;

            foreach (DataRow tempRow4_Variable in myDataTable4.Rows)
            {
                command.Parameters.Clear();
                tempRow4 = tempRow4_Variable;

                string players = tempRow4["players"].ToString();
                string name = tempRow4["name"].ToString();
                string leader = tempRow4["leader"].ToString();
                int army_live = Convert.ToInt32(tempRow4["army_live"]);
                int army = Convert.ToInt32(tempRow4["army"]);
                int status = Convert.ToInt32(tempRow4["status"]);
                string[] players_arr = players.Split(',');

                foreach (string player in players_arr)
                {
                    command.Parameters.Clear();
                    if (player == textBox1.Text)
                    {
                        pictureBox1.Visible = true;
                        pictureBox2.Visible = true;
                        pictureBox3.Visible = false;

                        if (status == 1)
                        {
                            command.CommandText = "UPDATE regions SET def_fact= def_fact - 1 Where owner=@p16 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p16", player);
                            command.ExecuteNonQuery();

                            Variables.fix_defence(player);

                            MySqlCommand command1 = new MySqlCommand("SELECT SUM(army_sunasp) FROM attacks where att_player='"+ player + "'", connection);
                            reader = command1.ExecuteReader();
                            int army_live4 = 0;
                            if (reader.Read())
                            {
                                if (reader[0] != System.DBNull.Value)
                                {
                                    army_live4 = Convert.ToInt32(reader[0]);
                                    
                                }
                            }
                            reader.Close();

                            army_live = army_live + army_live4;

                            command.CommandText = "UPDATE sunaspismos SET army_live=@p29  Where name=@p12 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p12", name);
                            command.Parameters.AddWithValue("@p29", army_live);
                            command.ExecuteNonQuery();

                            command.CommandText = "UPDATE attacks SET army_sunasp=@p19, sunaspismos=@p20 Where att_player=@p21 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p19", 0);
                            command.Parameters.AddWithValue("@p20", "");
                            command.Parameters.AddWithValue("@p21", player);
                            command.ExecuteNonQuery();

                            int free_army = 0;
                            int army_given_sunasp = 0;
                            int army_debt_sunasp = 0;

                            command.CommandText = "SELECT * FROM player where username=@p24";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p24", player);
                            reader = command.ExecuteReader();
                            if (reader.Read())
                            {
                                free_army = Convert.ToInt32(reader["free_army"]);
                                army_given_sunasp = Convert.ToInt32(reader["army_given_sunasp"]);
                                army_debt_sunasp = Convert.ToInt32(reader["army_debt_sunasp"]);
                            }
                            reader.Close();

                            command.CommandText = "UPDATE player SET free_army=@p25, army_debt_sunasp=@p26, army_given_sunasp=@p27, sunasp=@p35 Where username=@p24 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p25", free_army + army_given_sunasp);
                            command.Parameters.AddWithValue("@p26", 0);
                            command.Parameters.AddWithValue("@p27", 0);
                            command.Parameters.AddWithValue("@p35", "");
                            command.ExecuteNonQuery();

                            players_arr = players_arr.Where(val => val != player).ToArray();

                            string new_players = string.Join(",", players_arr.ToArray());

                            if (army_live >= army_given_sunasp)
                            {
                                command.Parameters.Clear();
                                if (leader != player)
                                {
                                    command.CommandText = "UPDATE sunaspismos SET players=@p11, army=@p28, army_live=@p29  Where name=@p12 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p11", new_players);
                                    command.Parameters.AddWithValue("@p12", name);
                                    command.Parameters.AddWithValue("@p28", army - army_debt_sunasp);
                                    command.Parameters.AddWithValue("@p29", army_live - army_given_sunasp);
                                    command.ExecuteNonQuery();
                                }
                                else
                                {
                                    command.Parameters.Clear();

                                    string chosen_leader = "";

                                    string com3 = "SELECT username,military FROM player";
                                    MySqlDataAdapter adpt3 = new MySqlDataAdapter(com3, connection);
                                    DataSet myDataSet3 = new DataSet();
                                    adpt3.Fill(myDataSet3, "attacks");
                                    DataTable myDataTable3 = myDataSet3.Tables[0];
                                    DataRow tempRow3 = null;

                                    decimal max = 0;

                                    foreach (DataRow tempRow3_Variable in myDataTable3.Rows)
                                    {
                                        command.Parameters.Clear();
                                        tempRow3 = tempRow3_Variable;

                                        string username = tempRow3["username"].ToString();
                                        decimal military = Convert.ToDecimal(tempRow3["military"]);

                                        foreach (string player2 in players_arr)
                                        {
                                            if (player2 == username)
                                            {
                                                if (military > max)
                                                {
                                                    max = military;
                                                    chosen_leader = username;
                                                }
                                            }
                                        }
                                    }

                                    command.CommandText = "UPDATE sunaspismos SET leader=@p1, players=@p11, army=@p28, army_live=@p29  Where name=@p12 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p1", chosen_leader);
                                    command.Parameters.AddWithValue("@p11", new_players);
                                    command.Parameters.AddWithValue("@p12", name);
                                    command.Parameters.AddWithValue("@p28", army - army_debt_sunasp);
                                    command.Parameters.AddWithValue("@p29", army_live - army_given_sunasp);
                                    command.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                command.Parameters.Clear();
                                if (leader != player)
                                {
                                    command.CommandText = "UPDATE sunaspismos SET players=@p11, army=@p28, army_live=@p29  Where name=@p12 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p11", new_players);
                                    command.Parameters.AddWithValue("@p12", name);
                                    command.Parameters.AddWithValue("@p28", army - army_debt_sunasp);
                                    command.Parameters.AddWithValue("@p29", 0);
                                    command.ExecuteNonQuery();
                                }
                                else
                                {
                                    command.Parameters.Clear();

                                    string chosen_leader = "";

                                    string com3 = "SELECT username,military FROM player";
                                    MySqlDataAdapter adpt3 = new MySqlDataAdapter(com3, connection);
                                    DataSet myDataSet3 = new DataSet();
                                    adpt3.Fill(myDataSet3, "attacks");
                                    DataTable myDataTable3 = myDataSet3.Tables[0];
                                    DataRow tempRow3 = null;

                                    decimal max = 0;

                                    foreach (DataRow tempRow3_Variable in myDataTable3.Rows)
                                    {
                                        command.Parameters.Clear();
                                        tempRow3 = tempRow3_Variable;

                                        string username = tempRow3["username"].ToString();
                                        decimal military = Convert.ToDecimal(tempRow3["military"]);

                                        foreach (string player2 in players_arr)
                                        {
                                            if (player2 == username)
                                            {
                                                if (military > max)
                                                {
                                                    max = military;
                                                    chosen_leader = username;
                                                }
                                            }
                                        }
                                    }

                                    command.CommandText = "UPDATE sunaspismos SET leader=@p1, players=@p11, army=@p28, army_live=@p29  Where name=@p12 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p1", chosen_leader);
                                    command.Parameters.AddWithValue("@p11", new_players);
                                    command.Parameters.AddWithValue("@p12", name);
                                    command.Parameters.AddWithValue("@p28", army - army_debt_sunasp);
                                    command.Parameters.AddWithValue("@p29", 0);
                                    command.ExecuteNonQuery();
                                }

                                int change = Math.Abs(army_live - army_given_sunasp);
                                foreach (string player1 in players_arr)
                                {
                                    command.Parameters.Clear();
                                    if (change != 0)
                                    {
                                        string com3 = "Select * from attacks where att_player='" + player1 + "' and army_sunasp>0";
                                        MySqlDataAdapter adpt3 = new MySqlDataAdapter(com3, connection);
                                        DataSet myDataSet3 = new DataSet();
                                        adpt3.Fill(myDataSet3, "attacks");
                                        DataTable myDataTable3 = myDataSet3.Tables[0];
                                        DataRow tempRow3 = null;

                                        foreach (DataRow tempRow3_Variable in myDataTable3.Rows)
                                        {
                                            command.Parameters.Clear();
                                            if (change != 0)
                                            {
                                                tempRow3 = tempRow3_Variable;

                                                int id = Convert.ToInt32(tempRow3["id"]);
                                                int army_sunasp = Convert.ToInt32(tempRow3["army_sunasp"]);

                                                if ((change - army_sunasp) >= 0)
                                                {
                                                    change = change - army_sunasp;
                                                    army_sunasp = 0;

                                                    command.CommandText = "UPDATE attacks SET army_sunasp=@p30, sunaspismos=@p31  Where id=@p32 ";
                                                    command.Prepare();
                                                    command.Parameters.AddWithValue("@p30", army_sunasp);
                                                    command.Parameters.AddWithValue("@p31", "");
                                                    command.Parameters.AddWithValue("@p32", id);
                                                    command.ExecuteNonQuery();
                                                }
                                                else
                                                {
                                                    army_sunasp = army_sunasp - change;
                                                    change = 0;

                                                    command.CommandText = "UPDATE attacks SET army_sunasp=@p30  Where id=@p32 ";
                                                    command.Prepare();
                                                    command.Parameters.AddWithValue("@p30", army_sunasp);
                                                    command.Parameters.AddWithValue("@p32", id);
                                                    command.ExecuteNonQuery();
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                            }
                        }
                        else
                        {
                            players_arr = players_arr.Where(val => val != player).ToArray();

                            string new_players = string.Join(",", players_arr.ToArray());

                            command.Parameters.Clear();

                            command.CommandText = "UPDATE player SET sunasp=@p35 Where username=@p24 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p35", "");
                            command.Parameters.AddWithValue("@p24", player);
                            command.ExecuteNonQuery();

                            if (leader != player)
                            {
                                command.CommandText = "UPDATE sunaspismos SET players=@p11  Where name=@p12 ";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p11", new_players);
                                command.Parameters.AddWithValue("@p12", name);
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                string chosen_leader = "";

                                string com3 = "SELECT username,military FROM player";
                                        MySqlDataAdapter adpt3 = new MySqlDataAdapter(com3, connection);
                                        DataSet myDataSet3 = new DataSet();
                                        adpt3.Fill(myDataSet3, "attacks");
                                        DataTable myDataTable3 = myDataSet3.Tables[0];
                                        DataRow tempRow3 = null;

                                        decimal max = 0;

                                        foreach (DataRow tempRow3_Variable in myDataTable3.Rows)
                                        {
                                            command.Parameters.Clear();
                                            tempRow3 = tempRow3_Variable;

                                            string username = tempRow3["username"].ToString();
                                            decimal military = Convert.ToDecimal(tempRow3["military"]);

                                            foreach (string player2 in players_arr)
                                            {
                                                if (player2 == username)
                                                {
                                                    if (military > max)
                                                    {
                                                        max = military;
                                                        chosen_leader = username; 
                                                    }
                                                } 
                                            }
                                        }

                                command.CommandText = "UPDATE sunaspismos SET leader=@p1, players=@p11  Where name=@p12 ";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p1", chosen_leader);
                                command.Parameters.AddWithValue("@p11", new_players);
                                command.Parameters.AddWithValue("@p12", name);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }

            }
            MessageBox.Show("Η Συμφωνία ακυρώθηκε");

            filldatagrid();
            fillrequest(); 

            connection.Close();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (textBox3.Visible == true && textBox3.Text != "")
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;

                int i = 1;
                int flag = 0;

                while (flag == 0)
                {
                    command.CommandText = "SELECT * FROM enwsi where id=@p1";
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

                command.CommandText = "insert into enwsi (id,name,leader,leader_rem_rounds,vote_rem_rounds,break_rem_rounds,players,army,army_live,gold_percent,farm_percent,craft_percent,dealer_percent,army_percent,gold_live,status) values (@p2,@p3, @p4, @p18, @p19, @p20, @p5, @p6, @p7, @p13, @p14, @p15, @p16, @p17, @p8, @p9)";
                command.Prepare();
                command.Parameters.AddWithValue("@p2", i);
                command.Parameters.AddWithValue("@p3", textBox3.Text);
                command.Parameters.AddWithValue("@p4", textBox1.Text);
                command.Parameters.AddWithValue("@p5", textBox1.Text);
                command.Parameters.AddWithValue("@p6", 0);
                command.Parameters.AddWithValue("@p13", Convert.ToDecimal(vars["gold_percent"]));
                command.Parameters.AddWithValue("@p14", Convert.ToDecimal(vars["farm_percent"]));
                command.Parameters.AddWithValue("@p15", Convert.ToDecimal(vars["craft_percent"]));
                command.Parameters.AddWithValue("@p16", Convert.ToDecimal(vars["dealer_percent"]));
                command.Parameters.AddWithValue("@p17", Convert.ToDecimal(vars["army_percent"]));
                command.Parameters.AddWithValue("@p18", Convert.ToInt32(vars["enwsi_leader_rounds"]));
                command.Parameters.AddWithValue("@p19", Convert.ToInt32(vars["enwsi_vote_rounds"]));
                command.Parameters.AddWithValue("@p20", Convert.ToInt32(vars["enwsi_break_rounds"]));
                command.Parameters.AddWithValue("@p7", 0);
                command.Parameters.AddWithValue("@p8", 0);
                command.Parameters.AddWithValue("@p9", 0);
                command.ExecuteNonQuery();

                command.CommandText = "UPDATE player SET enwsi=@p11, enwsi_vote=@p21 Where username=@p12 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p11", textBox3.Text);
                command.Parameters.AddWithValue("@p12", textBox1.Text);
                command.Parameters.AddWithValue("@p21", textBox1.Text);
                command.ExecuteNonQuery();

                connection.Close();

                textBox3.Visible = false;
                pictureBox4.Visible = false;

                filldatagrid();

                MessageBox.Show("Δημιουργήσατε Ένωση με όνομα " + textBox3.Text);
            }
            else
            {
                textBox3.Visible = true;
                MessageBox.Show("Δώστε όνομα ένωσης");
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;
            connection.Open();

            command.CommandText = "SELECT * FROM player where username=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", textBox1.Text);
            reader = command.ExecuteReader();
            reader.Read();

            string enwsi = reader["enwsi"].ToString();

            reader.Close();

            if(enwsi != "")
            {
                pictureBox4.Visible = true;
                pictureBox5.Visible = false;

                command.CommandText = "SELECT * FROM enwsi where name=@p2";
                command.Prepare();
                command.Parameters.AddWithValue("@p2", enwsi);
                reader = command.ExecuteReader();
                reader.Read();

                string players = reader["players"].ToString();
                string name = reader["name"].ToString();
                string leader = reader["leader"].ToString();
                int army_live = Convert.ToInt32(reader["army_live"]);
                int army = Convert.ToInt32(reader["army"]);
                int status = Convert.ToInt32(reader["status"]);
                string[] players_arr = players.Split(',');

                reader.Close();

                if (status == 1)
                {
                    command.CommandText = "UPDATE regions SET def_fact= def_fact - 2 Where owner=@p16 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p16", textBox1.Text);
                    command.ExecuteNonQuery();

                    Variables.fix_defence(textBox1.Text);

                    MySqlCommand command1 = new MySqlCommand("SELECT SUM(army_enwsi) FROM attacks where att_player='"+ textBox1.Text + "'", connection);
                    reader = command1.ExecuteReader();
                    int army_live4 = 0;
                    if (reader.Read())
                    {
                        if (reader[0] != System.DBNull.Value)
                        {
                            army_live4 = Convert.ToInt32(reader[0]);
                                    
                        }
                    }
                    reader.Close();

                    army_live = army_live + army_live4;

                    command.CommandText = "UPDATE enwsi SET army_live=@p29  Where name=@p12 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p12", name);
                    command.Parameters.AddWithValue("@p29", army_live);
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE attacks SET army_enwsi=@p19, enwsi=@p20 Where att_player=@p21 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p19", 0);
                    command.Parameters.AddWithValue("@p20", "");
                    command.Parameters.AddWithValue("@p21", textBox1.Text);
                    command.ExecuteNonQuery();

                    int free_army = 0;
                    int army_given_enwsi = 0;
                    int army_debt_enwsi = 0;

                    command.CommandText = "SELECT * FROM player where username=@p24";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p24", textBox1.Text);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        free_army = Convert.ToInt32(reader["free_army"]);
                        army_given_enwsi = Convert.ToInt32(reader["army_given_enwsi"]);
                        army_debt_enwsi = Convert.ToInt32(reader["army_debt_enwsi"]);
                    }
                    reader.Close();

                    command.CommandText = "UPDATE player SET free_army=@p25, army_debt_enwsi=@p26, army_given_enwsi=@p27, enwsi=@p35, enwsi_vote=@p36 Where username=@p24 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p25", free_army + army_given_enwsi);
                    command.Parameters.AddWithValue("@p26", 0);
                    command.Parameters.AddWithValue("@p27", 0);
                    command.Parameters.AddWithValue("@p35", "");
                    command.Parameters.AddWithValue("@p36", "");
                    command.ExecuteNonQuery();

                    string player = textBox1.Text;
                    players_arr = players_arr.Where(val => val != player).ToArray();

                    string new_players = string.Join(",", players_arr.ToArray());

                    if (army_live >= army_given_enwsi)
                    {
                        command.Parameters.Clear();
                        if (leader != player)
                        {
                            command.CommandText = "UPDATE enwsi SET players=@p11, army=@p28, army_live=@p29  Where name=@p12 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p11", new_players);
                            command.Parameters.AddWithValue("@p12", name);
                            command.Parameters.AddWithValue("@p28", army - army_debt_enwsi);
                            command.Parameters.AddWithValue("@p29", army_live - army_given_enwsi);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            command.Parameters.Clear();

                            string chosen_leader = "";

                            string com3 = "SELECT username,military FROM player";
                            MySqlDataAdapter adpt3 = new MySqlDataAdapter(com3, connection);
                            DataSet myDataSet3 = new DataSet();
                            adpt3.Fill(myDataSet3, "attacks");
                            DataTable myDataTable3 = myDataSet3.Tables[0];
                            DataRow tempRow3 = null;

                            decimal max = 0;

                            foreach (DataRow tempRow3_Variable in myDataTable3.Rows)
                            {
                                command.Parameters.Clear();
                                tempRow3 = tempRow3_Variable;

                                string username = tempRow3["username"].ToString();
                                decimal military = Convert.ToDecimal(tempRow3["military"]);

                                foreach (string player2 in players_arr)
                                {
                                    if (player2 == username)
                                    {
                                        if (military > max)
                                        {
                                            max = military;
                                            chosen_leader = username;
                                        }
                                    }
                                }
                            }

                            command.Parameters.Clear();

                            command.CommandText = "UPDATE agreement SET leader=@p1  Where leader=@p2 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p1", chosen_leader);
                            command.Parameters.AddWithValue("@p2", player);
                            command.ExecuteNonQuery();

                            command.Parameters.Clear();

                            command.CommandText = "UPDATE enwsi SET leader=@p1, leader_rem_rounds=@p37, players=@p11, army=@p28, army_live=@p29  Where name=@p12 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p1", chosen_leader);
                            command.Parameters.AddWithValue("@p11", new_players);
                            command.Parameters.AddWithValue("@p12", name);
                            command.Parameters.AddWithValue("@p28", army - army_debt_enwsi);
                            command.Parameters.AddWithValue("@p29", army_live - army_given_enwsi);
                            command.Parameters.AddWithValue("@p37", 0);
                            command.ExecuteNonQuery();

                            label3.Visible = false;
                            label5.Visible = false;
                            label6.Visible = false;
                            label8.Visible = false;
                            label9.Visible = false;
                            label10.Visible = false;
                            label11.Visible = false;
                            label12.Visible = false;
                            label13.Visible = false;
                            label14.Visible = false;
                            label15.Visible = false;
                            button4.Visible = false;
                            numericUpDown1.Visible = false;
                            numericUpDown2.Visible = false;
                            numericUpDown3.Visible = false;
                            numericUpDown4.Visible = false;
                            numericUpDown5.Visible = false;
                            label16.Visible = false;
                        }
                    }
                    else
                    {
                        command.Parameters.Clear();
                        if (leader != player)
                        {
                            command.CommandText = "UPDATE enwsi SET players=@p11, army=@p28, army_live=@p29  Where name=@p12 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p11", new_players);
                            command.Parameters.AddWithValue("@p12", name);
                            command.Parameters.AddWithValue("@p28", army - army_debt_enwsi);
                            command.Parameters.AddWithValue("@p29", 0);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            command.Parameters.Clear();

                            string chosen_leader = "";

                            string com3 = "SELECT username,military FROM player";
                            MySqlDataAdapter adpt3 = new MySqlDataAdapter(com3, connection);
                            DataSet myDataSet3 = new DataSet();
                            adpt3.Fill(myDataSet3, "attacks");
                            DataTable myDataTable3 = myDataSet3.Tables[0];
                            DataRow tempRow3 = null;

                            decimal max = 0;

                            foreach (DataRow tempRow3_Variable in myDataTable3.Rows)
                            {
                                command.Parameters.Clear();
                                tempRow3 = tempRow3_Variable;

                                string username = tempRow3["username"].ToString();
                                decimal military = Convert.ToDecimal(tempRow3["military"]);

                                foreach (string player2 in players_arr)
                                {
                                    if (player2 == username)
                                    {
                                        if (military > max)
                                        {
                                            max = military;
                                            chosen_leader = username;
                                        }
                                    }
                                }
                            }

                            command.Parameters.Clear();

                            command.CommandText = "UPDATE agreement SET leader=@p1  Where leader=@p2 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p1", chosen_leader);
                            command.Parameters.AddWithValue("@p2", player);
                            command.ExecuteNonQuery();

                            command.Parameters.Clear();

                            command.CommandText = "UPDATE enwsi SET leader=@p1, leader_rem_rounds=@p37, players=@p11, army=@p28, army_live=@p29  Where name=@p12 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p1", chosen_leader);
                            command.Parameters.AddWithValue("@p11", new_players);
                            command.Parameters.AddWithValue("@p12", name);
                            command.Parameters.AddWithValue("@p28", army - army_debt_enwsi);
                            command.Parameters.AddWithValue("@p29", 0);
                            command.Parameters.AddWithValue("@p37", 0);
                            command.ExecuteNonQuery();
                        }

                        int change = Math.Abs(army_live - army_given_enwsi);
                        foreach (string player1 in players_arr)
                        {
                            command.Parameters.Clear();
                            if (change != 0)
                            {
                                string com3 = "Select * from attacks where att_player='" + player1 + "' and army_enwsi>0";
                                MySqlDataAdapter adpt3 = new MySqlDataAdapter(com3, connection);
                                DataSet myDataSet3 = new DataSet();
                                adpt3.Fill(myDataSet3, "attacks");
                                DataTable myDataTable3 = myDataSet3.Tables[0];
                                DataRow tempRow3 = null;

                                foreach (DataRow tempRow3_Variable in myDataTable3.Rows)
                                {
                                    command.Parameters.Clear();
                                    if (change != 0)
                                    {
                                        tempRow3 = tempRow3_Variable;

                                        int id = Convert.ToInt32(tempRow3["id"]);
                                        int army_enwsi = Convert.ToInt32(tempRow3["army_enwsi"]);

                                        if ((change - army_enwsi) >= 0)
                                        {
                                            change = change - army_enwsi;
                                            army_enwsi = 0;

                                            command.CommandText = "UPDATE attacks SET army_enwsi=@p30, enwsi=@p31  Where id=@p32 ";
                                            command.Prepare();
                                            command.Parameters.AddWithValue("@p30", army_enwsi);
                                            command.Parameters.AddWithValue("@p31", "");
                                            command.Parameters.AddWithValue("@p32", id);
                                            command.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            army_enwsi = army_enwsi - change;
                                            change = 0;

                                            command.CommandText = "UPDATE attacks SET army_enwsi=@p30  Where id=@p32 ";
                                            command.Prepare();
                                            command.Parameters.AddWithValue("@p30", army_enwsi);
                                            command.Parameters.AddWithValue("@p32", id);
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                }
                else
                {
                    string player = textBox1.Text;
                    players_arr = players_arr.Where(val => val != player).ToArray();

                    string new_players = string.Join(",", players_arr.ToArray());

                    command.Parameters.Clear();

                    command.CommandText = "UPDATE player SET enwsi=@p35 Where username=@p24 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p35", "");
                    command.Parameters.AddWithValue("@p24", player);
                    command.ExecuteNonQuery();

                    if (leader != player)
                    {
                        command.CommandText = "UPDATE enwsi SET players=@p11  Where name=@p12 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p11", new_players);
                        command.Parameters.AddWithValue("@p12", name);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        string chosen_leader = "";

                        string com3 = "SELECT username,military FROM player";
                        MySqlDataAdapter adpt3 = new MySqlDataAdapter(com3, connection);
                        DataSet myDataSet3 = new DataSet();
                        adpt3.Fill(myDataSet3, "attacks");
                        DataTable myDataTable3 = myDataSet3.Tables[0];
                        DataRow tempRow3 = null;

                        decimal max = 0;

                        foreach (DataRow tempRow3_Variable in myDataTable3.Rows)
                        {
                            command.Parameters.Clear();
                            tempRow3 = tempRow3_Variable;

                            string username = tempRow3["username"].ToString();
                            decimal military = Convert.ToDecimal(tempRow3["military"]);

                            foreach (string player2 in players_arr)
                            {
                                if (player2 == username)
                                {
                                    if (military > max)
                                    {
                                        max = military;
                                        chosen_leader = username; 
                                    }
                                } 
                            }
                        }

                        command.Parameters.Clear();

                        command.CommandText = "UPDATE agreement SET leader=@p1  Where leader=@p2 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p1", chosen_leader);
                        command.Parameters.AddWithValue("@p2", player);
                        command.ExecuteNonQuery();

                        command.Parameters.Clear();

                        command.CommandText = "UPDATE enwsi SET leader=@p1, players=@p11  Where name=@p12 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p1", chosen_leader);
                        command.Parameters.AddWithValue("@p11", new_players);
                        command.Parameters.AddWithValue("@p12", name);
                        command.ExecuteNonQuery();

                        label3.Visible = false;
                        label5.Visible = false;
                        label6.Visible = false;
                        label8.Visible = false;
                        label9.Visible = false;
                        label10.Visible = false;
                        label11.Visible = false;
                        label12.Visible = false;
                        label13.Visible = false;
                        label14.Visible = false;
                        label15.Visible = false;
                        button4.Visible = false;
                        numericUpDown1.Visible = false;
                        numericUpDown2.Visible = false;
                        numericUpDown3.Visible = false;
                        numericUpDown4.Visible = false;
                        numericUpDown5.Visible = false;
                    }
                }

                string com8 = "SELECT * FROM attacks WHERE att_player='" + textBox1.Text + "' and army_coplayer!=''";
                MySqlDataAdapter adpt8 = new MySqlDataAdapter(com8, connection);
                DataSet myDataSet8 = new DataSet();
                adpt8.Fill(myDataSet8, "attacks");
                DataTable myDataTable8 = myDataSet8.Tables[0];
                DataRow tempRow8 = null;

                foreach (DataRow tempRow8_Variable in myDataTable8.Rows)
	            {
                    command.Parameters.Clear();

                    tempRow8 = tempRow8_Variable;

                    string army_coplayer = tempRow8["army_coplayer"].ToString();
                    string region_coplayer = tempRow8["region_coplayer"].ToString();
                    string[] army_co = army_coplayer.Split(',');
                    string[] region_co = region_coplayer.Split(',');

                    List<int> list = new List<int>();
                    if (army_coplayer != "")
                    {
                        foreach (string army1 in army_co)
                        {
                            int army_co1 = Int32.Parse(army1);
                            list.Add(army_co1);
                        }
                    }

		            if (army_coplayer != "")
                    {
                        Dictionary<string, int> coplayers = new Dictionary<string, int>();
                        coplayers = Enumerable.Range(0, region_co.Length).ToDictionary(x => region_co[x], x => list[x]);

                        foreach (KeyValuePair<string, int> item in coplayers)
                        {
                            command.Parameters.Clear();

                            command.CommandText = "SELECT * FROM regions where name='" + item.Key + "'";
                            command.Prepare();
                            reader = command.ExecuteReader();

                            reader.Read();

                            int co_id = Convert.ToInt32(reader["id"]);
                            string co_owner = reader["owner"].ToString();
                            int co_farm = Convert.ToInt32(reader["farm"]);
                            int co_craft = Convert.ToInt32(reader["craft"]);
                            int co_dealer = Convert.ToInt32(reader["dealer"]);
                            int co_army = Convert.ToInt32(reader["army"]);
                            decimal co_def_fact = Convert.ToDecimal(reader["def_fact"]);

                            reader.Close();

                            command.CommandText = "SELECT * FROM player where username=@p1";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p1", co_owner);
                            reader = command.ExecuteReader();
                            reader.Read();

                            decimal co_military = Convert.ToDecimal(reader["military"]);

                            reader.Close();

                            int co_offense = Convert.ToInt32(Math.Round((co_army + item.Value) * (co_military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                            decimal co_defence = Math.Round(co_def_fact * (co_farm + co_craft + co_dealer) + co_offense);

                            command.CommandText = "UPDATE regions SET army=@p11, defence=@p12, cost=@p13, offense=@p14 Where name=@p16 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p11", co_army + item.Value);
                            command.Parameters.AddWithValue("@p12", Convert.ToInt32(Math.Round(co_defence)));
                            command.Parameters.AddWithValue("@p13", co_army + item.Value);
                            command.Parameters.AddWithValue("@p14", co_offense);
                            command.Parameters.AddWithValue("@p16", item.Key);
                            command.ExecuteNonQuery();

                            Variables.calculate_cost(co_id);
                        }
                    } 
	            }

                MessageBox.Show("Η Συμφωνία ακυρώθηκε");

                MySqlCommand command2 = new MySqlCommand("SELECT COUNT(*) FROM regions where owner='" + textBox1.Text + "'", connection);
                string temp3 = command2.ExecuteScalar().ToString();
                decimal num_cit = Convert.ToDecimal(temp3);

                int number_lost_regions = Convert.ToInt32(Math.Round(num_cit - (num_cit * Convert.ToDecimal(vars["enwsi_abandon_percent"]))));

                string com7 = "SELECT * FROM regions WHERE owner='" + textBox1.Text + "' ORDER BY RAND() LIMIT " + number_lost_regions;
                MySqlDataAdapter adpt7 = new MySqlDataAdapter(com7, connection);
                DataSet myDataSet7 = new DataSet();
                adpt7.Fill(myDataSet7, "attacks");
                DataTable myDataTable7 = myDataSet7.Tables[0];
                DataRow tempRow7 = null;

                foreach (DataRow tempRow7_Variable in myDataTable7.Rows)
                {
                    command.Parameters.Clear();

                    tempRow7 = tempRow7_Variable;

                    string reg_name = tempRow7["name"].ToString();
                    int id1 = Convert.ToInt32(tempRow7["id"]);

                    Variables.search_around_fix(id1);

                    command.CommandText = "UPDATE regions SET owner=@p10, def_fact= def_fact + 2 Where name=@p24 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p10", leader);
                    command.Parameters.AddWithValue("@p24", reg_name);
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE attacks SET def_player=@p11 Where def_region=@p25 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p11", leader);
                    command.Parameters.AddWithValue("@p25", reg_name);
                    command.ExecuteNonQuery();

                    Variables.fix_defence(leader);
                }

                filldatagrid();
                fillrequest();

                if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
                {
                    (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).PlayerGold();
                }
                if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
                {
                    (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).check_sunaspismos();
                }

                connection.Close();    
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if ((numericUpDown2.Value + numericUpDown3.Value + numericUpDown4.Value + numericUpDown5.Value) == 100)
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;

                command.CommandText = "SELECT * FROM player where username=@p7";
                command.Prepare();
                command.Parameters.AddWithValue("@p7", textBox1.Text);
                reader = command.ExecuteReader();

                string enwsi = "";

                if (reader.Read())
                {
                    enwsi = reader["enwsi"].ToString();
                }

                reader.Close();

                command.CommandText = "UPDATE enwsi SET gold_percent=@p1, farm_percent=@p2, craft_percent=@p3, dealer_percent=@p4, army_percent=@p5  Where name=@p6 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", Convert.ToDecimal(numericUpDown1.Value) / 100M);
                command.Parameters.AddWithValue("@p2", Convert.ToDecimal(numericUpDown2.Value) / 100M);
                command.Parameters.AddWithValue("@p3", Convert.ToDecimal(numericUpDown3.Value) / 100M);
                command.Parameters.AddWithValue("@p4", Convert.ToDecimal(numericUpDown4.Value) / 100M);
                command.Parameters.AddWithValue("@p5", Convert.ToDecimal(numericUpDown5.Value) / 100M);
                command.Parameters.AddWithValue("@p6", enwsi);
                command.ExecuteNonQuery();

                MessageBox.Show("Τα ποσοστά της παρακράτησης και της αναδιανομής του αποθεματικού \n της ένωσης " + enwsi + " ενημερώθηκαν επιτυχώς");

                connection.Close();
                filldatagrid();
            }
            else
            {
                MessageBox.Show("To άθροισμα των ποσοστών των αγροτών, των τεχνιτών , \n των εμπόρων και των στρατιωτών δεν πρέπει να ξεπερνά ή να είναι λιγότερο του 100%");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;

                command.CommandText = "SELECT * FROM player where username=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", textBox1.Text);
                reader = command.ExecuteReader();

                string enwsi = "";
                string sunaspismos = "";
                string enwsi_vote = "";

                if (reader.Read())
                {
                    enwsi = reader["enwsi"].ToString();
                    enwsi_vote = reader["enwsi_vote"].ToString();
                    sunaspismos = reader["sunasp"].ToString();
                }

                reader.Close();

                command.CommandText = "UPDATE player SET enwsi_vote=@p2 Where username=@p1 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p2", comboBox1.SelectedItem);
                command.ExecuteNonQuery();

                MessageBox.Show("Ψηφίσατε επιτυχώς τον " + comboBox1.SelectedItem + " για κυβερνήτη της ένωσης");

                connection.Close();

                button5.Visible = false;
            }
            else
            {
                MessageBox.Show("Επιλέξτε το όνομα του παίκτη που θέλετε \n να ψηφίσετε για κυβερνήτης της ένωσης");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM player where username=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", textBox1.Text);
            reader = command.ExecuteReader();

            string enwsi = "";
            string sunaspismos = "";
            string enwsi_vote = "";

            if (reader.Read())
            {
                enwsi = reader["enwsi"].ToString();
                enwsi_vote = reader["enwsi_vote"].ToString();
                sunaspismos = reader["sunasp"].ToString();
            }

            reader.Close();

            command.CommandText = "UPDATE player SET enwsi_vote=@p2 Where username=@p1 ";
            command.Prepare();
            command.Parameters.AddWithValue("@p2", "");
            command.ExecuteNonQuery();

            MessageBox.Show("Αποσύρατε την ψήφο σας απο τον " + enwsi_vote);

            connection.Close();

            button6.Visible = false;
        }
    }
}
