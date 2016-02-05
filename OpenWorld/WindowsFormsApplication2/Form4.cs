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
    public partial class Form4 : Form
    {

        int zoom = 8;
        int Panx = 0;
        int Pany = 0;
        string sqlcon = Variables.sqlstring;
        Dictionary<string, string> vars = Variables.vars();

        public Form4(string username)
        {
            InitializeComponent();

            //display2.MouseWheel += new MouseEventHandler(pictureBox1_MouseWheel);
            //display2.MouseHover += new EventHandler(pictureBox1_MouseHover);
            //display2.MouseWheel += new MouseEventHandler(display2_MouseWheel);
            this.CenterToScreen();
            //this.HScroll = true;
            //this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            textBox1.Text = username;
            numericUpDown1.Minimum = 0;
            pictureBox8.Visible = false;
            pictureBox9.Visible = false;
            filldatagrid();

            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM player where username='" + username + "'";
            command.Prepare();
            reader = command.ExecuteReader();
            reader.Read();

            textBox4.Text = Convert.ToString(reader["military"]);

            reader.Close();
            connection.Close();
        }

        public void filldatagrid()
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            string str = sqlcon;
            MySqlConnection con = new MySqlConnection(str);

            string com = "Select name,owner,farm,craft,dealer,army,defence,level,offense,pol_stab from regions ORDER BY owner DESC";
            MySqlDataAdapter adpt = new MySqlDataAdapter(com, con);
            DataSet myDataSet = new DataSet();
            adpt.Fill(myDataSet, "regions");
            DataTable myDataTable = myDataSet.Tables[0];
            //DataRow tempRow = null;

            for (int i = 0; i < myDataSet.Tables[0].Rows.Count; i++)
            {
                command.CommandText = "SELECT * FROM def_othomanoi where name='" + myDataSet.Tables[0].Rows[i].ItemArray[0].ToString() + "'";
                command.Prepare();
                reader = command.ExecuteReader();
                int factor1 = 0;
                int flag = 0;
                if (reader.Read())
                {
                    factor1 = Convert.ToInt32(reader["factor"]);
                    flag = Convert.ToInt32(reader["flag"]);
                    if (flag == 1)
                    {
                        Int32 defence = Convert.ToInt32(myDataSet.Tables[0].Rows[i][6].ToString());
                        int new_defence = defence * factor1;
                        myDataSet.Tables[0].Rows[i][6] = Convert.ToString(new_defence);
                    }
                }

                reader.Close();

                command.CommandText = "SELECT * FROM player where username='" + myDataSet.Tables[0].Rows[i].ItemArray[1].ToString() + "'";
                command.Prepare();
                reader = command.ExecuteReader();
                string sunaspismos = "";
                string enwsi = "";
                if (reader.Read())
                {
                    sunaspismos = reader["sunasp"].ToString();
                    enwsi = reader["enwsi"].ToString();
                    if (sunaspismos != "" && enwsi != "")
                    {
                        myDataSet.Tables[0].Rows[i][1] = myDataSet.Tables[0].Rows[i][1] + " (" + sunaspismos + "," + enwsi + ")";
                    }
                    else
                    {
                        if (sunaspismos != "")
                        {
                            myDataSet.Tables[0].Rows[i][1] = myDataSet.Tables[0].Rows[i][1] +" (" + sunaspismos + ")";
                        }
                        if (enwsi != "")
                        {
                            myDataSet.Tables[0].Rows[i][1] = myDataSet.Tables[0].Rows[i][1] + " (" + enwsi + ")";
                        }
                    }
                }

                reader.Close();
            }

            myDataSet.Tables[0].AcceptChanges();

            dataGridView1.DataSource = myDataSet;
            dataGridView1.DataMember = "regions";
            
            dataGridView1.Columns[0].HeaderText = "Όνομα Περιοχής";
            dataGridView1.Columns[1].HeaderText = "Ιδιοκτήτης";
            dataGridView1.Columns[2].HeaderText = "Αγρότες";
            dataGridView1.Columns[3].HeaderText = "Τεχνίτες";
            dataGridView1.Columns[4].HeaderText = "Έμποροι";
            dataGridView1.Columns[5].HeaderText = "Αριθμός Στρατού";
            dataGridView1.Columns[6].HeaderText = "Αμυντική Ισχύς";
            dataGridView1.Columns[7].HeaderText = "Επίπεδο Ανάπτυξης";
            dataGridView1.Columns[8].HeaderText = "Επιθετική Ισχύς";
            dataGridView1.Columns[9].HeaderText = "Πολιτική Σταθερότητα";

            dataGridView1.Columns[8].DisplayIndex = 7;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //foreach (DataRow tempRow_Variable in myDataTable.Rows)
            //{
            //    tempRow = tempRow_Variable;
            //    listBox1.Items.Add((tempRow["username"]));
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox4_MouseClick(object sender, MouseEventArgs e)
        {
            if (label2.Text != "" && label3.Text != "" && numericUpDown1.Value != 0)
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;

                command.CommandText = "SELECT * FROM regions where name=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", label2.Text);
                reader = command.ExecuteReader();
                reader.Read();

                string att_name = reader["owner"].ToString();
                string att_reg_name = reader["name"].ToString();
                int x = Convert.ToInt32(reader["x"]);
                int y = Convert.ToInt32(reader["y"]);
                int att_army = Convert.ToInt32(reader["army"]);
                int att_def = Convert.ToInt32(reader["defence"]);
                int att_farm = Convert.ToInt32(reader["farm"]);
                int att_craft = Convert.ToInt32(reader["craft"]);
                int att_dealer = Convert.ToInt32(reader["dealer"]);
                decimal att_def_fact = Convert.ToDecimal(reader["def_fact"]);
                
                string neighbor = reader["neighbor"].ToString();
                string neighbor1 = reader["neighbor1"].ToString();
                string neighbor2 = reader["neighbor2"].ToString();
                string neighbor3 = reader["neighbor3"].ToString();
                string[] words = neighbor.Split(',');
                string[] words1 = neighbor1.Split(',');
                string[] words2 = neighbor2.Split(',');
                string[] words3 = neighbor3.Split(',');

                reader.Close();

                command.CommandText = "SELECT * FROM regions where name=@p2";
                command.Prepare();
                command.Parameters.AddWithValue("@p2", label3.Text);
                reader = command.ExecuteReader();
                reader.Read();

                string def_name = reader["owner"].ToString();
                string def_reg_name = reader["name"].ToString();
                int x1 = Convert.ToInt32(reader["x"]);
                int y1 = Convert.ToInt32(reader["y"]);
                int immune = Convert.ToInt32(reader["immune"]);

                reader.Close();

                command.CommandText = "SELECT * FROM player where username=@p21";
                command.Prepare();
                command.Parameters.AddWithValue("@p21", textBox1.Text);
                reader = command.ExecuteReader();
                reader.Read();

                decimal military = Convert.ToDecimal(reader["military"]);
                string sunaspismos = reader["sunasp"].ToString();
                string enwsi = reader["enwsi"].ToString();

                reader.Close();

                command.CommandText = "SELECT * FROM player where username=@p23";
                command.Prepare();
                command.Parameters.AddWithValue("@p23", def_name);
                reader = command.ExecuteReader();
                string sunaspismos1 = "";
                string enwsi1 = "";
                if(reader.Read())
                {
                    sunaspismos1 = reader["sunasp"].ToString();
                    enwsi1 = reader["enwsi"].ToString();
                }
                reader.Close();

                command.CommandText = "SELECT * FROM enwsi where name=@p25";
                command.Prepare();
                command.Parameters.AddWithValue("@p25", enwsi);
                reader = command.ExecuteReader();
                int enwsi_status = 0;
                if (reader.Read())
                {
                    enwsi_status = Convert.ToInt32(reader["status"]);
                }
                reader.Close();

                int distance = Convert.ToInt32(Math.Round(Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1))));

                if (enwsi != "" && enwsi_status == 1)
                {
                    int distance1 = get_distance(sunaspismos, enwsi, def_reg_name);
                    if (distance1 < distance)
                    {
                        distance = distance1;
                    }
                }
                
                if (att_name != "")
                {
                    if (att_name == textBox1.Text)
                    {
                        if (att_name != def_name)
                        {
                            if (enwsi != enwsi1 || (enwsi == "" && enwsi1 ==""))
                            {
                                if (sunaspismos != sunaspismos1 || (sunaspismos == "" && sunaspismos1 == ""))
                                {
                                    if (immune == 0)
                                    {
                                        command.CommandText = "SELECT * FROM treaties where player_a=@p12 and player_b=@p13";
                                        command.Prepare();
                                        command.Parameters.AddWithValue("@p12", att_name);
                                        command.Parameters.AddWithValue("@p13", def_name);
                                        reader = command.ExecuteReader();


                                        if (reader.Read())
                                        {
                                            reader.Close();
                                            MessageBox.Show("Δεν μπορείτε να κάνετε επίθεση στην " + def_reg_name + " καθώς ανήκετε στην ίδια ομοσπονδία με τον " + def_name + ".");
                                        }
                                        else
                                        {
                                            reader.Close();
                                            command.CommandText = "SELECT * FROM reg_treaties where region_a=@p14 and region_b=@p15";
                                            command.Prepare();
                                            command.Parameters.AddWithValue("@p14", att_reg_name);
                                            command.Parameters.AddWithValue("@p15", def_reg_name);
                                            reader = command.ExecuteReader();


                                            if (reader.Read())
                                            {
                                                reader.Close();
                                                MessageBox.Show("Δεν μπορείτε να κάνετε επίθεση στην " + def_reg_name + " καθώς ανήκετε στην ίδια ομοσπονδία με την " + att_reg_name);
                                            }
                                            else
                                            {
                                                if (att_army >= numericUpDown1.Value)
                                                {
                                                    int i = 1;
                                                    int flag = 0;
                                                    reader.Close();
                                                    while (flag == 0)
                                                    {
                                                        command.CommandText = "SELECT * FROM attacks where id=@p11";
                                                        command.Prepare();
                                                        command.Parameters.AddWithValue("@p11", i);
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
                                                    MySqlCommand command2 = new MySqlCommand("SELECT COUNT(*) FROM attacks", connection);
                                                    string temp = command2.ExecuteScalar().ToString();
                                                    int num_attacks = Convert.ToInt32(temp);

                                                    command.CommandText = "insert into attacks (id,att_player,def_player,att_region,def_region,distance,turn,army,kind) values (@p3, @p10, @p16, @p4, @p5, @p6, @p7, @p8, @p9)";
                                                    command.Prepare();
                                                    command.Parameters.AddWithValue("@p3", i);
                                                    command.Parameters.AddWithValue("@p4", att_reg_name);
                                                    command.Parameters.AddWithValue("@p5", def_reg_name);
                                                    command.Parameters.AddWithValue("@p6", distance);
                                                    command.Parameters.AddWithValue("@p7", distance);
                                                    command.Parameters.AddWithValue("@p8", numericUpDown1.Value);
                                                    command.Parameters.AddWithValue("@p9", 1);
                                                    command.Parameters.AddWithValue("@p10", textBox1.Text);
                                                    command.Parameters.AddWithValue("@p16", def_name);
                                                    command.ExecuteNonQuery();

                                                    //listBox4.Items.Clear();
                                                    //listBox5.Items.Clear();
                                                    int offense = Convert.ToInt32(Math.Round((att_army - Convert.ToInt32(numericUpDown1.Value)) * (military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                                                    decimal defence = Math.Round(att_def_fact * (att_farm + att_craft + att_dealer) + offense);

                                                    command.CommandText = "UPDATE regions SET army=@p17, defence=@p18, cost=@p19, offense=@p20 Where name=@p22 ";
                                                    command.Prepare();
                                                    command.Parameters.AddWithValue("@p17", att_army - Convert.ToInt32(numericUpDown1.Value));
                                                    command.Parameters.AddWithValue("@p18", Convert.ToInt32(Math.Round(defence)));
                                                    command.Parameters.AddWithValue("@p19", att_army - Convert.ToInt32(numericUpDown1.Value));
                                                    command.Parameters.AddWithValue("@p20", offense);
                                                    command.Parameters.AddWithValue("@p22", label2.Text);
                                                    command.ExecuteNonQuery();

                                                    MessageBox.Show("Η επίθεση προστέθηκε επιτυχώς.");

                                                    label2.Text = "";
                                                    label3.Text = "";
                                                    label6.Text = "";
                                                    label7.Text = "";
                                                    label9.Text = "";
                                                    numericUpDown1.Value = 0;
                                                    filldatagrid();

                                                    if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
                                                    {
                                                        (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).FillAttack();
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Οι στρατιώτες που δηλώσατε είναι περισσότεροι από αυτούς που έχετε.");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Η " + def_reg_name + " έχει ακόμη " + immune + " γύρους ασυλίας.");
                                    }
                                }
                                else
                                {
                                    label3.Text = "";
                                    MessageBox.Show("Δεν μπορείτε να κάνετε επίθεση στην " + def_reg_name + " καθώς ανήκετε στον ίδιο συνασπισμό με την " + att_reg_name);
                                }
                            }
                            else
                            {
                                label3.Text = "";
                                MessageBox.Show("Δεν μπορείτε να κάνετε επίθεση στην " + def_reg_name + " καθώς ανήκετε στην ίδια ένωση με την " + att_reg_name);
                            }

                        }
                        else
                        {
                            label3.Text = "";
                            //textBox6.Text = "";
                            MessageBox.Show("Η " + def_reg_name + " σας ανήκει και δεν μπορείτε να επιτεθείτε.");
                        }
                    }
                    else
                    {
                        label2.Text = "";
                        label3.Text = "";
                        label6.Text = "";
                        label7.Text = "";
                        label9.Text = "";
                        numericUpDown1.Value = 0;
                        //textBox6.Text = "";
                        MessageBox.Show("Η επιτιθέμενη περιοχή δεν σας ανήκει.");
                    }
                }
                else
                {
                    label2.Text = "";
                    label3.Text = "";
                    label6.Text = "";
                    label7.Text = "";
                    label9.Text = "";
                    numericUpDown1.Value = 0;
                    //textBox6.Text = "";
                    MessageBox.Show("Η επιτιθέμενη περιοχή δεν σας ανήκει.");
                }

                connection.Close();
            }
            else
            {
                MessageBox.Show("Συμπληρώστε όλα τα απαραίτητα πεδία.");
            }
        }

        public int get_distance(string sunaspismos, string enwsi, string def_reg_name)
        {
            Dictionary<string, int> distances = new Dictionary<string, int>();

            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            //command.CommandText = "Select * from sunaspismos where name=@p1";
            //command.Prepare();
            //command.Parameters.AddWithValue("@p1", sunaspismos);
            //reader = command.ExecuteReader();

            //string players = "";
            //string[] players_arr;

            //if(reader.Read())
            //{
            //    players = reader["players"].ToString();
            //}

            //players_arr = players.Split(',');
            //reader.Close();

            command.CommandText = "Select * from enwsi where name=@p2";
            command.Prepare();
            command.Parameters.AddWithValue("@p2", enwsi);
            reader = command.ExecuteReader();

            string players1 = "";
            string[] players_arr1;

            if (reader.Read())
            {
                players1 = reader["players"].ToString();
            }

            players_arr1 = players1.Split(',');
            reader.Close();

            command.CommandText = "Select * from regions where name=@p3";
            command.Prepare();
            command.Parameters.AddWithValue("@p3", def_reg_name);
            reader = command.ExecuteReader();

            int x = 0;
            int y = 0;

            if (reader.Read())
            {
                x = Convert.ToInt32(reader["x"]);
                y = Convert.ToInt32(reader["y"]);
            }
            reader.Close();

            //if (players_arr.Length >= 3)
            //{
            //    foreach (string player in players_arr)
            //    {
            //        command.Parameters.Clear();

            //        string com = "Select * from regions where owner='" + player + "'";
            //        MySqlDataAdapter adpt = new MySqlDataAdapter(com, connection);
            //        DataSet myDataSet = new DataSet();
            //        adpt.Fill(myDataSet, "attacks");
            //        DataTable myDataTable = myDataSet.Tables[0];
            //        DataRow tempRow = null;

            //        foreach (DataRow tempRow_Variable in myDataTable.Rows)
            //        {
            //            command.Parameters.Clear();
            //            tempRow = tempRow_Variable;

            //            string name = tempRow["name"].ToString();

            //            string neighbor = tempRow["neighbor"].ToString();
            //            string neighbor1 = tempRow["neighbor1"].ToString();
            //            string neighbor2 = tempRow["neighbor2"].ToString();
            //            string neighbor3 = tempRow["neighbor3"].ToString();
            //            string[] words = neighbor.Split(',');
            //            string[] words1 = neighbor1.Split(',');
            //            string[] words2 = neighbor2.Split(',');
            //            string[] words3 = neighbor3.Split(',');

            //            int flag1 = 20;
            //            int distance = 0;
            //            foreach (string n in words)
            //            {
            //                if (n == def_reg_name)
            //                {
            //                    flag1 = 0;
            //                    break;
            //                }
            //            }
            //            foreach (string h in words1)
            //            {
            //                if (h == def_reg_name)
            //                {
            //                    flag1 = 1;
            //                    break;
            //                }
            //            }
            //            foreach (string w in words2)
            //            {
            //                if (w == def_reg_name)
            //                {
            //                    flag1 = 2;
            //                    break;
            //                }
            //            }
            //            foreach (string o in words3)
            //            {
            //                if (o == def_reg_name)
            //                {
            //                    flag1 = 3;
            //                    break;
            //                }
            //            }
            //            if (flag1 == 0)
            //            {
            //                distance = 3;
            //            }
            //            else if (flag1 == 1)
            //            {
            //                distance = 6;
            //            }
            //            else if (flag1 == 2)
            //            {
            //                distance = 9;
            //            }
            //            else if (flag1 == 3)
            //            {
            //                distance = 12;
            //            }

            //            distances.Add(name, distance);
            //        }
            //    } 
            //}

            if (players_arr1.Length >= Convert.ToInt32(vars["enwsi_player_limit"]))
            {
                foreach (string player1 in players_arr1)
                {
                    command.Parameters.Clear();

                    string com1 = "Select * from regions where owner='" + player1 + "'";
                    MySqlDataAdapter adpt1 = new MySqlDataAdapter(com1, connection);
                    DataSet myDataSet1 = new DataSet();
                    adpt1.Fill(myDataSet1, "attacks");
                    DataTable myDataTable1 = myDataSet1.Tables[0];
                    DataRow tempRow1 = null;

                    foreach (DataRow tempRow1_Variable in myDataTable1.Rows)
                    {
                        command.Parameters.Clear();
                        tempRow1 = tempRow1_Variable;

                        string name = tempRow1["name"].ToString();

                        int x1 = Convert.ToInt32(tempRow1["x"]);
                        int y1 = Convert.ToInt32(tempRow1["y"]);

                        int distance = Convert.ToInt32(Math.Round(Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1))));

                        distances.Add(name, distance);
                    }
                } 
            }

            connection.Close();

            return distances.Values.Min();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = 0;
            int flag = 0;
            string s = Convert.ToString(dataGridView1.CurrentCell.Value);
            if(int.TryParse(s, out i))
            {
                flag = 1;
            }

            if (label2.Text != "" && dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.Value != "" && dataGridView1.CurrentCell.Value != null && flag == 0 && e.ColumnIndex == 0 && label2.Text != s)
            {
                label3.Text = dataGridView1.CurrentCell.Value.ToString();

                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;

                command.CommandText = "SELECT * FROM regions where name=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", label2.Text);
                reader = command.ExecuteReader();
                reader.Read();

                //string att_name = reader["owner"].ToString();
                //string att_reg_name = reader["name"].ToString();
                string att_name = Convert.ToString(reader["owner"]);
                int x = Convert.ToInt32(reader["x"]);
                int y = Convert.ToInt32(reader["y"]);
                int offense = Convert.ToInt32(reader["offense"]);
                int att_army = Convert.ToInt32(reader["army"]);

                int attack = Convert.ToInt32(Math.Round((Convert.ToDecimal(textBox4.Text) * Convert.ToDecimal(vars["off_fact"]) + 1) * att_army));
                string neighbor = reader["neighbor"].ToString();
                string neighbor1 = reader["neighbor1"].ToString();
                string neighbor2 = reader["neighbor2"].ToString();
                string neighbor3 = reader["neighbor3"].ToString();
                string[] words = neighbor.Split(',');
                string[] words1 = neighbor1.Split(',');
                string[] words2 = neighbor2.Split(',');
                string[] words3 = neighbor3.Split(',');
                //int att_army = Convert.ToInt32(reader["army"]);

                reader.Close();

                if (att_name == "")
                {
                    pictureBox8.Visible = false;
                    pictureBox9.Visible = false;
                }
                
                command.CommandText = "SELECT * FROM regions where name=@p2";
                command.Prepare();
                command.Parameters.AddWithValue("@p2", label3.Text);
                reader = command.ExecuteReader();
                reader.Read();

                string def_name = reader["owner"].ToString();
                string def_reg_name = reader["name"].ToString();
                int x1 = Convert.ToInt32(reader["x"]);
                int y1 = Convert.ToInt32(reader["y"]);
                int army = Convert.ToInt32(reader["army"]);
                int defence = Convert.ToInt32(reader["defence"]);

                reader.Close();

                command.CommandText = "SELECT * FROM player where username=@p21";
                command.Prepare();
                command.Parameters.AddWithValue("@p21", textBox1.Text);
                reader = command.ExecuteReader();
                reader.Read();

                string sunaspismos = reader["sunasp"].ToString();
                string enwsi = reader["enwsi"].ToString();

                reader.Close();

                if (def_name == att_name)
                {
                    pictureBox8.Visible = false;
                    pictureBox9.Visible = false;
                }

                command.CommandText = "SELECT * FROM enwsi where name=@p25";
                command.Prepare();
                command.Parameters.AddWithValue("@p25", enwsi);
                reader = command.ExecuteReader();
                int enwsi_status = 0;
                if (reader.Read())
                {
                    enwsi_status = Convert.ToInt32(reader["status"]);
                }
                reader.Close();

                int distance = Convert.ToInt32(Math.Round(Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1))));

                if (enwsi != "" && enwsi_status == 1)
                {
                    int distance1 = get_distance(sunaspismos, enwsi, def_reg_name);
                    if (distance1 < distance)
                    {
                        distance = distance1;
                    }
                }

                command.CommandText = "SELECT * FROM def_othomanoi where name='" + def_reg_name + "'";
                command.Prepare();
                reader = command.ExecuteReader();
                int factor1 = 0;
                int flag2 = 0;
                if (reader.Read())
                {
                    factor1 = Convert.ToInt32(reader["factor"]);
                    flag2 = Convert.ToInt32(reader["flag"]);
                    if (flag2 == 1)
                    {
                        defence = defence * factor1;
                    }
                }

                reader.Close();

                label6.Text = Convert.ToString(distance);
                label7.Text = Convert.ToString(defence);
                label9.Text = Convert.ToString(attack);
                if(attack > defence)
                {
                    label9.ForeColor = Color.Green;
                }
                else
                {
                    label9.ForeColor = Color.Red;
                }

                if (att_name == textBox1.Text)
                {
                    label9.Visible = true;
                }
                else
                {
                    label9.Visible = false;
                }

                command.CommandText = "SELECT * FROM reg_treaties where region_a=@p3 and region_b=@p4";
                command.Prepare();
                command.Parameters.AddWithValue("@p3", label2.Text);
                command.Parameters.AddWithValue("@p4", label3.Text);
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    pictureBox8.Visible = false;
                    pictureBox9.Visible = true;
                }
                else if(att_name == "")
                {
                    pictureBox8.Visible = false;
                    pictureBox9.Visible = false;
                }
                else
                {
                    pictureBox8.Visible = true;
                    pictureBox9.Visible = false;
                }

                reader.Close();

                command.CommandText = "SELECT * FROM agreement where region_a=@p5 and region_b=@p6";
                command.Prepare();
                command.Parameters.AddWithValue("@p5", label2.Text);
                command.Parameters.AddWithValue("@p6", label3.Text);
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    pictureBox8.Visible = false;
                    pictureBox9.Visible = false;
                }

                reader.Close();
                
                
                connection.Close();

                DrawMap();
                display2.Visible = true;
            }
            else
            {
                if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.Value != "" && dataGridView1.CurrentCell.Value != null && flag == 0 && e.ColumnIndex == 0)
                {
                    label2.Text = dataGridView1.CurrentCell.Value.ToString();

                    MySqlConnection connection = new MySqlConnection(sqlcon);
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    MySqlDataReader reader;

                    command.CommandText = "SELECT * FROM regions where name=@p1";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p1", label2.Text);
                    reader = command.ExecuteReader();
                    reader.Read();
                    int att_army = Convert.ToInt32(reader["army"]);
                    reader.Close();
                    connection.Close();

                    textBox5.Text = Convert.ToString(att_army);
                    numericUpDown1.Maximum = att_army;
                }
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (label2.Text != "" && label3.Text != "" && numericUpDown1.Value != 0)
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;

                command.CommandText = "SELECT * FROM regions where name=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", label2.Text);
                reader = command.ExecuteReader();
                reader.Read();

                string att_name = reader["owner"].ToString();
                string att_reg_name = reader["name"].ToString();
                int x = Convert.ToInt32(reader["x"]);
                int y = Convert.ToInt32(reader["y"]);
                int att_army = Convert.ToInt32(reader["army"]);
                int att_farm = Convert.ToInt32(reader["farm"]);
                decimal att_def_fact = Convert.ToDecimal(reader["def_fact"]);
                int att_craft = Convert.ToInt32(reader["craft"]);
                int att_dealer = Convert.ToInt32(reader["dealer"]);
                string neighbor = reader["neighbor"].ToString();
                string neighbor1 = reader["neighbor1"].ToString();
                string neighbor2 = reader["neighbor2"].ToString();
                string neighbor3 = reader["neighbor3"].ToString();
                string[] words = neighbor.Split(',');
                string[] words1 = neighbor1.Split(',');
                string[] words2 = neighbor2.Split(',');
                string[] words3 = neighbor3.Split(',');

                reader.Close();

                command.CommandText = "SELECT * FROM player where username=@p15";
                command.Prepare();
                command.Parameters.AddWithValue("@p15", textBox1.Text);
                reader = command.ExecuteReader();
                reader.Read();

                decimal military = Convert.ToDecimal(reader["military"]);

                reader.Close();

                if (att_name != "")
                {
                    if (att_name == textBox1.Text)
                    {
                        if (numericUpDown1.Value <= att_army)
                        {

                            command.CommandText = "SELECT * FROM regions where name=@p2";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p2", label3.Text);
                            reader = command.ExecuteReader();
                            reader.Read();

                            string def_name = reader["owner"].ToString();
                            string def_reg_name = reader["name"].ToString();
                            int x1 = Convert.ToInt32(reader["x"]);
                            int y1 = Convert.ToInt32(reader["y"]);
                            int immune = Convert.ToInt32(reader["immune"]);

                            reader.Close();

                            int distance = Convert.ToInt32(Math.Round(Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1))));

                            if (immune == 0)
                            {
                                int i = 1;
                                int flag = 0;

                                while (flag == 0)
                                {
                                    command.CommandText = "SELECT * FROM attacks where id=@p11";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p11", i);
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
                                MySqlCommand command2 = new MySqlCommand("SELECT COUNT(*) FROM attacks", connection);
                                string temp = command2.ExecuteScalar().ToString();
                                int num_attacks = Convert.ToInt32(temp);

                                command.CommandText = "insert into attacks (id,att_player,def_player,att_region,def_region,distance,turn,army,kind) values (@p3, @p10, @p17, @p4, @p5, @p6, @p7, @p8, @p9)";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p3", i);
                                command.Parameters.AddWithValue("@p4", att_reg_name);
                                command.Parameters.AddWithValue("@p5", def_reg_name);
                                command.Parameters.AddWithValue("@p6", distance);
                                command.Parameters.AddWithValue("@p7", distance);
                                command.Parameters.AddWithValue("@p8", numericUpDown1.Value);
                                command.Parameters.AddWithValue("@p9", 2);
                                command.Parameters.AddWithValue("@p10", textBox1.Text);
                                command.Parameters.AddWithValue("@p17", def_name);
                                command.ExecuteNonQuery();


                                int offense = Convert.ToInt32(Math.Round((att_army - Convert.ToInt32(numericUpDown1.Value)) * (military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                                decimal defence = Math.Round(att_def_fact * (att_farm + att_craft + att_dealer) + offense);

                                command.CommandText = "UPDATE regions SET army=@p11, defence=@p12, cost=@p13, offense=@p14 Where name=@p16 ";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p11", att_army - Convert.ToInt32(numericUpDown1.Value));
                                command.Parameters.AddWithValue("@p12", Convert.ToInt32(Math.Round(defence)));
                                command.Parameters.AddWithValue("@p13", att_army - Convert.ToInt32(numericUpDown1.Value));
                                command.Parameters.AddWithValue("@p14", offense);
                                command.Parameters.AddWithValue("@p16", label2.Text);
                                command.ExecuteNonQuery();

                                MessageBox.Show("Η υποστήριξη προστέθηκε επιτυχώς.");

                                label2.Text = "";
                                label3.Text = "";
                                label6.Text = "";
                                label7.Text = "";
                                label9.Text = "";
                                numericUpDown1.Value = 0;
                                filldatagrid();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ο αριθμός των στρατιωτών που έχετε εισάγει είναι μεγαλύτερος απο αυτόν που διαθέτει η περιοχή σας.");
                        }
                    }
                    else
                    {
                        label2.Text = "";
                        label3.Text = "";
                        label6.Text = "";
                        label7.Text = "";
                        label9.Text = "";
                        numericUpDown1.Value = 0;
                        //textBox6.Text = "";
                        MessageBox.Show("Η επιτιθέμενη περιοχή δεν σας ανήκει.");
                    }
                }
                else
                {
                    label2.Text = "";
                    label3.Text = "";
                    label6.Text = "";
                    label7.Text = "";
                    label9.Text = "";
                    numericUpDown1.Value = 0;
                    //textBox6.Text = "";
                    MessageBox.Show("Η επιτιθέμενη περιοχή δεν σας ανήκει.");
                }

                connection.Close();
            }
            else
            {
                MessageBox.Show("Συμπληρώστε όλα τα απαραίτητα πεδία.");
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            label2.Text = "";
            label3.Text = "";
            label6.Text = "";
            label7.Text = "";
            label9.Text = "";
            numericUpDown1.Value = 0;
            pictureBox8.Visible = false;
            pictureBox9.Visible = false;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                string str = sqlcon;
                MySqlConnection con = new MySqlConnection(str);

                string com1 = "Select name,owner,farm,craft,dealer,army,defence,level,offense,pol_stab,x,y from regions where name='" + dataGridView1.CurrentCell.Value.ToString() + "'";
                MySqlDataAdapter adpt1 = new MySqlDataAdapter(com1, con);
                DataSet myDataSet1 = new DataSet();
                adpt1.Fill(myDataSet1, "regions");
                DataTable myDataTable1 = myDataSet1.Tables[0];
                DataRow tempRow1 = null;

                //string[] words = new string[]{};
                //string[] words1 = new string[] { };
                //string[] words2 = new string[] { };
                //string[] words3 = new string[] { };

                foreach (DataRow tempRow1_Variable in myDataTable1.Rows)
                {
                    tempRow1 = tempRow1_Variable;
                    int x = Convert.ToInt32(tempRow1["x"]);
                    int y = Convert.ToInt32(tempRow1["y"]);
                    textBox2.Text = Convert.ToString(x);
                    textBox3.Text = Convert.ToString(y);
                }
                myDataTable1.Columns.Remove("x");
                myDataTable1.Columns.Remove("y");
                //myDataTable1.Columns.Remove("neighbor2");
                //myDataTable1.Columns.Remove("neighbor3");
                //myDataSet1.Clear();
                string com = "Select name,owner,farm,craft,dealer,army,defence,level,offense,pol_stab from regions where x<" + Convert.ToString(Convert.ToInt32(textBox2.Text) + Convert.ToInt32(comboBox1.SelectedItem)) + " and x>" + Convert.ToString(Convert.ToInt32(textBox2.Text) - Convert.ToInt32(comboBox1.SelectedItem)) + " and y<" + Convert.ToString(Convert.ToInt32(textBox3.Text) + Convert.ToInt32(comboBox1.SelectedItem)) + " and y>" + Convert.ToString(Convert.ToInt32(textBox3.Text) - Convert.ToInt32(comboBox1.SelectedItem));

                //string com = "Select name,owner,farm,craft,dealer,army,defence,level,offense,pol_stab,neighbor,neighbor1,neighbor2,neighbor3 from regions";
                MySqlDataAdapter adpt = new MySqlDataAdapter(com, con);
                DataSet myDataSet = new DataSet();
                adpt.Fill(myDataSet, "regions");
                DataTable myDataTable = myDataSet.Tables[0];
                //DataRow tempRow = null;

                //if (Convert.ToInt32(comboBox1.SelectedItem) == 3)
                //{
                //    for (int i = 0; i < myDataSet.Tables[0].Rows.Count; i++)
                //    { 
                //        foreach(string s in words)
                //        {
                //            if (myDataSet.Tables[0].Rows[i][0].ToString() == s)
                //            {
                //                myDataTable1.ImportRow(myDataTable.Rows[i]);
                //            }
                //        }
                //    }
                //}
                //else if (Convert.ToInt32(comboBox1.SelectedItem) == 6)
                //{
                //    for (int i = 0; i < myDataSet.Tables[0].Rows.Count; i++)
                //    {
                //        foreach (string s in words1)
                //        {
                //            if (myDataSet.Tables[0].Rows[i][0].ToString() == s)
                //            {
                //                myDataTable1.ImportRow(myDataTable.Rows[i]);
                //            }
                //        }
                //    }
                //}
                //else if (Convert.ToInt32(comboBox1.SelectedItem) == 9)
                //{
                //    for (int i = 0; i < myDataSet.Tables[0].Rows.Count; i++)
                //    {
                //        foreach (string s in words2)
                //        {
                //            if (myDataSet.Tables[0].Rows[i][0].ToString() == s)
                //            {
                //                myDataTable1.ImportRow(myDataTable.Rows[i]);
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    for (int i = 0; i < myDataSet.Tables[0].Rows.Count; i++)
                //    {
                //        foreach (string s in words3)
                //        {
                //            if (myDataSet.Tables[0].Rows[i][0].ToString() == s)
                //            {
                //                myDataTable1.ImportRow(myDataTable.Rows[i]);
                //            }
                //        }
                //    }
                //}
                

                dataGridView1.DataSource = myDataSet;
                dataGridView1.DataMember = "regions";
                
                dataGridView1.Columns[0].HeaderText = "Όνομα Περιοχής";
                dataGridView1.Columns[1].HeaderText = "Ιδιοκτήτης";  
                dataGridView1.Columns[2].HeaderText = "Αγρότες";
                dataGridView1.Columns[3].HeaderText = "Τεχνίτες";
                dataGridView1.Columns[4].HeaderText = "Έμποροι";
                dataGridView1.Columns[5].HeaderText = "Αριθμός Στρατού";
                dataGridView1.Columns[6].HeaderText = "Αμυντική Ισχύς";
                dataGridView1.Columns[7].HeaderText = "Επίπεδο Ανάπτυξης";
                dataGridView1.Columns[8].HeaderText = "Επιθετική Ισχύς";
                dataGridView1.Columns[9].HeaderText = "Πολιτική Σταθερότητα";
                

                //foreach (DataRow tempRow_Variable in myDataTable.Rows)
                //{
                //    tempRow = tempRow_Variable;
                //    listBox1.Items.Add((tempRow["username"]));
                //}
            }
            else
            {
                filldatagrid();
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            if (label2.Text != "" && label3.Text != "")
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;

                command.CommandText = "SELECT * FROM regions where name=@p5";
                command.Prepare();
                command.Parameters.AddWithValue("@p5", label3.Text);
                reader = command.ExecuteReader();
                reader.Read();

                string player_b = reader["owner"].ToString();
                reader.Close();
                string player_a = textBox1.Text;

                int i = 1;
                int flag = 0;

                while (flag == 0)
                {
                    command.CommandText = "SELECT * FROM agreement where id=@p6";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p6", i);
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
                
                string region1 = label2.Text;
                string region2 = label3.Text;

                command.CommandText = "insert into agreement (id,player_a,player_b,region_a, region_b) values (@p7,@p1, @p2, @p3, @p4)";
                command.Prepare();
                command.Parameters.AddWithValue("@p7", i);
                command.Parameters.AddWithValue("@p1", player_a);
                command.Parameters.AddWithValue("@p2", player_b);
                command.Parameters.AddWithValue("@p3", region1);
                command.Parameters.AddWithValue("@p4", region2);
                command.ExecuteNonQuery();

                //string region1 = label2.Text;
                //string region2 = label3.Text;

                //command.CommandText = "insert into reg_treaties (region_a, region_b) values (@p3, @p4)";
                //command.Prepare();
                //command.Parameters.AddWithValue("@p3", region1);
                //command.Parameters.AddWithValue("@p4", region2);
                //command.ExecuteNonQuery();

                //command.CommandText = "insert into reg_treaties (region_a, region_b) values (@p5, @p6)";
                //command.Prepare();
                //command.Parameters.AddWithValue("@p5", region2);
                //command.Parameters.AddWithValue("@p6", region1);
                //command.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Η Aίτηση Συμφωνίας στάλθηκε στην " + label3.Text);
                pictureBox8.Visible = false;
                pictureBox9.Visible = false;
            }
            else
            {
                MessageBox.Show("Εισάγετε τις πόλεις που θέλετε να κάνουν συμφωνία.");
            }
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            if (label2.Text != "" && label3.Text != "")
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();

                string region1 = label2.Text;
                string region2 = label3.Text;

                command.CommandText = "DELETE FROM reg_treaties Where region_a=@p3 and region_b=@p4";
                command.Prepare();
                command.Parameters.AddWithValue("@p3", region1);
                command.Parameters.AddWithValue("@p4", region2);
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM reg_treaties Where region_a=@p5 and region_b=@p6";
                command.Prepare();
                command.Parameters.AddWithValue("@p5", region2);
                command.Parameters.AddWithValue("@p6", region1);
                command.ExecuteNonQuery();


                connection.Close();

                MessageBox.Show("Η συμφωνία ακυρώθηκε.");
                pictureBox8.Visible = true;
                pictureBox9.Visible = false;
            }
            else
            {
                MessageBox.Show("Εισάγετε τις πόλεις που θέλετε να κάνουν συμφωνία.");
            }
        }

        public void DrawMap()
        {
            display2.Controls.Clear();
            int XOff1 = display2.Width/2;
            int Yoff1 = display2.Height/2;
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM regions", connection);
            string temp3 = command.ExecuteScalar().ToString();
            int num_cit = Convert.ToInt32(temp3);
            MySqlDataReader reader;//Create prepared statement
            //δίνω το ερώτημα

            //for (int i = 1; i <= (num_cit / 10); i++)
            //{

            //    command.CommandText = "SELECT * FROM regions where reg_box=@p1 and ini=@p2";
            //    command.Prepare();
            //    //και τις παραμέτρους
            //    command.Parameters.AddWithValue("@p1", i);
            //    command.Parameters.AddWithValue("@p2", 1);
            //    reader = command.ExecuteReader();

            //    string temp4 = "";
            //    string temp5 = "";
            //    if (reader.Read())//αν έχω βρει αποτέλεσμα
            //    {

            //       temp4 = reader["x"].ToString();
            //       temp5 = reader["y"].ToString();
            //    }

            //    int x1 = Convert.ToInt32(temp4);
            //    int y1 = Convert.ToInt32(temp5);

            //    //PictureBox display = new PictureBox();
            //    display.Width = 20;
            //    display.Height = 20;
            //    //display.ImageLocation = @"C:\Users\George\Desktop\map2.png";
            //    display.BackgroundImage = Image.FromFile(@"C:\Users\"+ Environment.UserName +@"\Desktop\map2.png");
            //    display.SizeMode = PictureBoxSizeMode.StretchImage;
            //    display.BackgroundImageLayout = ImageLayout.Stretch;
            //    display.Location = new Point(XOff1 + (x1 - 10),Yoff1 - (y1 - 10));
            //    reader.Close();
            //    command.Parameters.Clear();

            //    this.Controls.Add(display);



            //    //display.BackColor = Color.Transparent;

            //}

            Bitmap bmp = new Bitmap(400, 400);
            Graphics g = Graphics.FromImage(bmp);

            
            command.CommandText = "SELECT * FROM regions where name=@p1";
            command.Prepare();
            //και τις παραμέτρους
            command.Parameters.AddWithValue("@p1", label2.Text);
            reader = command.ExecuteReader();

            string temp1 = "";
            string temp2 = "";
            if (reader.Read())//αν έχω βρει αποτέλεσμα
            {

                temp1 = reader["x"].ToString();
                temp2 = reader["y"].ToString();
                temp3 = reader["name"].ToString();
            }

            int x = Convert.ToInt32(temp1);
            int y = Convert.ToInt32(temp2);

            reader.Close();

            command.CommandText = "SELECT * FROM regions where name=@p2";
            command.Prepare();
            //και τις παραμέτρους
            command.Parameters.AddWithValue("@p2", label3.Text);
            reader = command.ExecuteReader();

            string temp4 = "";
            string temp5 = "";
            string temp6 = "";
            if (reader.Read())//αν έχω βρει αποτέλεσμα
            {

                temp4 = reader["x"].ToString();
                temp5 = reader["y"].ToString();
                temp6 = reader["name"].ToString();
            }

            int x1 = Convert.ToInt32(temp4);
            int y1 = Convert.ToInt32(temp5);

            reader.Close();
            //int x2 = 0;
            //int y2 = 0;           
            if (zoom >= 6 && zoom >= 0)
            {
                Label LB = new Label();
                LB.Name = temp3;
                //LB.Name = Convert.ToString(j);
                LB.Location = new Point((XOff1 + zoom * (Panx + x/2) - 10), (Yoff1 - zoom * (Pany + y/2) + -20));
                LB.Size = new Size(32, 37);
                LB.BackColor = Color.Transparent;
                LB.Font = new Font("Calibri", 15, FontStyle.Bold);
                //LB.Text = "X";
                LB.Image = new Bitmap(WindowsFormsApplication2.Properties.Resources.tower);
                LB.Click += new EventHandler(LB_Click); //assign click handler

                display2.Controls.Add(LB);

                Label LB1 = new Label();
                LB1.Name = temp6;
                //LB.Name = Convert.ToString(j);
                LB1.Location = new Point((XOff1 + zoom * (Panx + x1/2) - 10), (Yoff1 - zoom * (Pany + y1/2) - 20));
                LB1.Size = new Size(32, 37);
                LB1.BackColor = Color.Transparent;
                LB1.Font = new Font("Calibri", 15, FontStyle.Bold);
                LB1.Image = new Bitmap(WindowsFormsApplication2.Properties.Resources.citywalls3);
                //LB1.Text = "X";
                LB1.Click += new EventHandler(LB_Click); //assign click handler

                display2.Controls.Add(LB1);

                Point point1 = new Point((XOff1 + zoom * (Panx + x/2)) + 10, (Yoff1 - zoom * (Pany + y/2)) + 10);
                Point point2 = new Point((XOff1 + zoom * (Panx + x1/2)) + 10, (Yoff1 - zoom * (Pany + y1/2)) + 10);
        
                g.DrawLine(new Pen(Color.ForestGreen, 3), point1, point2);
                //labels[j-1].Text = "X";
                //labels[j-1].Location = new Point((XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y)));
            }
            else if (zoom < 0)
            {
                zoom = 0;
                Label LB = new Label();
                LB.Name = temp3;
                LB.Location = new Point((XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y)));
                LB.Size = new Size(20, 20);
                LB.BackColor = Color.Transparent;
                LB.Font = new Font("Calibri", 15, FontStyle.Bold);
                LB.Text = "X";
                LB.Click += new EventHandler(LB_Click); //assign click handler

                display2.Controls.Add(LB);

                Label LB1 = new Label();
                LB1.Name = temp6;
                //LB.Name = Convert.ToString(j);
                LB1.Location = new Point((XOff1 + zoom * (Panx + x1)), (Yoff1 - zoom * (Pany + y1)));
                LB1.Size = new Size(20, 20);
                LB1.BackColor = Color.Transparent;
                LB1.Font = new Font("Calibri", 15, FontStyle.Bold);
                LB1.Text = "X";
                LB1.Click += new EventHandler(LB_Click); //assign click handler

                display2.Controls.Add(LB1);

                Point point1 = new Point((XOff1 + zoom * (Panx + x)) + 10, (Yoff1 - zoom * (Pany + y)) + 10);
                Point point2 = new Point((XOff1 + zoom * (Panx + x1)) + 10, (Yoff1 - zoom * (Pany + y1)) + 10);
                
                g.DrawLine(new Pen(Color.Red, 3), point1, point2);
                //g.DrawString("x", new Font("Calibri", 15), new SolidBrush(Color.Black), (XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y)));
                //display2.Controls.Add(new Label { Text = "X", Font = new Font("Calibri", 15, FontStyle.Bold), Height = 20, Width = 20, Name = "lable" + j, BackColor = Color.Transparent, Location = new Point((XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y))) });
                //labels[j-1].Text = "X";
                //labels[j-1].Location = new Point((XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y)));
            }
            else
            {
                Label LB = new Label();
                LB.Name = temp3;
                LB.Location = new Point((XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y)));
                LB.Size = new Size(100, 30);
                LB.BackColor = Color.Transparent;
                LB.Font = new Font("Calibri", 15, FontStyle.Bold);
                LB.Text = temp3;
                LB.Click += new EventHandler(LB_Click); //assign click handler

                display2.Controls.Add(LB);

                Label LB1 = new Label();
                LB1.Name = temp6;
                //LB.Name = Convert.ToString(j);
                LB1.Location = new Point((XOff1 + zoom * (Panx + x1)), (Yoff1 - zoom * (Pany + y1)));
                LB1.Size = new Size(100, 30);
                LB1.BackColor = Color.Transparent;
                LB1.Font = new Font("Calibri", 15, FontStyle.Bold);
                LB1.Text = temp6;
                LB1.Click += new EventHandler(LB_Click); //assign click handler

                display2.Controls.Add(LB1);

                Point point1 = new Point((XOff1 + zoom * (Panx + x)) + 10, (Yoff1 - zoom * (Pany + y)) + 10);
                Point point2 = new Point((XOff1 + zoom * (Panx + x1)) + 10, (Yoff1 - zoom * (Pany + y1)) + 10);
                
                g.DrawLine(new Pen(Color.Red, 3), point1, point2);
                //g.DrawString(temp3, new Font("Calibri", 8), new SolidBrush(Color.Black), (XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y)));
                //display2.Controls.Add(new Label { Text = temp3, Font = new Font("Calibri", 15, FontStyle.Bold), Height = 20, Width = 20, Name = "lable" + j, BackColor = Color.Transparent, Location = new Point((XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y))) });
                //labels[j-1].Text = "X";
                //labels[j-1].Location = new Point((XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y)));
            }
            //g.DrawLine (new Pen(Color.Red, 2), 5,250, 300,250);
            //let's draw a coordinate equivalent to (20,30) (20 up, 30 across)
            
            //g.DrawString("X", new Font("Calibri", 15), new SolidBrush(Color.Black), (XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y)));
            
           // g.DrawString("O", new Font("Calibri", 15), new SolidBrush(Color.Black), (XOff1 + zoom * (Panx + x1)), (Yoff1 - zoom * (Pany + y1)));
            
            //g.DrawString(temp3, new Font("Calibri", 8), new SolidBrush(Color.Black), (XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y)));
            
            //g.DrawString("O", new Font("Calibri", 20), new SolidBrush(Color.Black), (XOff + x), (YOff - y));
            //double distance = Math.Round(Math.Sqrt((Math.Pow(x - x2, 2) + Math.Pow(y - y2, 2))));
            //g.DrawString(distance.ToString(), new Font("Calibri", 40), new SolidBrush(Color.Black), 0, 0);
            
            //command.Parameters.Clear();
 
            //display2.Width = 50;
            //display2.Height = 50;
            display2.Location = new Point(300, 0);
            //display2.BackgroundImage = Image.FromFile(@"C:\Users\" + Environment.UserName + @"\Desktop\middle.jpg");
            //display2.ImageLocation = @"C:\Users\" + Environment.UserName + @"\Desktop\middle.jpg";
            display2.SizeMode = PictureBoxSizeMode.StretchImage;
            display2.Image = bmp;

            connection.Close();
            
            Application.DoEvents();
        }

        protected void LB_Click(object sender, EventArgs e)
        {
            //attempt to cast the sender as a label
            Label lbl = sender as Label;

            //if the cast was successful
            if (lbl != null)
            {
                lbl.Image = null;
                lbl.Text = lbl.Name;
                lbl.Size = new Size(100, 30); 
            }
        }

        void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                if (e.Delta <= 0)
                {
                    if (zoom < 0)
                        return;
                    zoom -= 2;
                    //set minimum size to zoom
                    if (display2.Width < 200)
                        return;
                }
                else
                {
                    zoom += 2;
                    //set maximum size to zoom
                    if (display2.Width > 2000)
                        return;
                }
                display2.Width += Convert.ToInt32(display2.Width * e.Delta / 1000);
                display2.Height += Convert.ToInt32(display2.Height * e.Delta / 1000);
                
                DrawMap();
            }
        }

        void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            display2.Focus();
        }

        void display2_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        public void DrawLinePoint(Point point1, Point point2 , PaintEventArgs e)
        {

            // Create pen.
            Pen blackPen = new Pen(Color.Red, 3);

            // Draw line to screen.
            e.Graphics.DrawLine(blackPen, point1, point2);
        }

        private void dataGridView1_MouseHover(object sender, EventArgs e)
        {
            dataGridView1.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (label2.Text != "" && label3.Text != "" && numericUpDown1.Value != 0)
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;

                command.CommandText = "SELECT * FROM regions where name=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", label2.Text);
                reader = command.ExecuteReader();
                reader.Read();

                string att_name = reader["owner"].ToString();
                string att_reg_name = reader["name"].ToString();
                int x = Convert.ToInt32(reader["x"]);
                int y = Convert.ToInt32(reader["y"]);
                int att_army = Convert.ToInt32(reader["army"]);
                int att_farm = Convert.ToInt32(reader["farm"]);
                decimal att_def_fact = Convert.ToDecimal(reader["def_fact"]);
                int att_craft = Convert.ToInt32(reader["craft"]);
                int att_dealer = Convert.ToInt32(reader["dealer"]);
                int att_cost = Convert.ToInt32(reader["cost"]);
                
                string neighbor = reader["neighbor"].ToString();
                string neighbor1 = reader["neighbor1"].ToString();
                string neighbor2 = reader["neighbor2"].ToString();
                string neighbor3 = reader["neighbor3"].ToString();
                string[] words = neighbor.Split(',');
                string[] words1 = neighbor1.Split(',');
                string[] words2 = neighbor2.Split(',');
                string[] words3 = neighbor3.Split(',');

                reader.Close();

                if (att_name != "")
                {
                    if (att_name == textBox1.Text)
                    {
                        if (numericUpDown1.Value <= att_army)
                        {

                            command.CommandText = "SELECT * FROM regions where name=@p2";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p2", label3.Text);
                            reader = command.ExecuteReader();
                            reader.Read();

                            string def_name = reader["owner"].ToString();
                            string def_reg_name = reader["name"].ToString();
                            int x1 = Convert.ToInt32(reader["x"]);
                            int y1 = Convert.ToInt32(reader["y"]);
                            int immune = Convert.ToInt32(reader["immune"]);

                            reader.Close();

                            if (def_name == textBox1.Text)
                            {
                                int distance = Convert.ToInt32(Math.Round(Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1))));

                                if (immune == 0)
                                {
                                    int i = 1;
                                    int flag = 0;

                                    while (flag == 0)
                                    {
                                        command.CommandText = "SELECT * FROM attacks where id=@p11";
                                        command.Prepare();
                                        command.Parameters.AddWithValue("@p11", i);
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
                                    MySqlCommand command2 = new MySqlCommand("SELECT COUNT(*) FROM attacks", connection);
                                    string temp = command2.ExecuteScalar().ToString();
                                    int num_attacks = Convert.ToInt32(temp);

                                    command.CommandText = "insert into attacks (id,att_player,def_player,att_region,def_region,distance,turn,army,kind) values (@p3, @p10, @p11, @p4, @p5, @p6, @p7, @p8, @p9)";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p3", i);
                                    command.Parameters.AddWithValue("@p4", att_reg_name);
                                    command.Parameters.AddWithValue("@p5", def_reg_name);
                                    command.Parameters.AddWithValue("@p6", distance);
                                    command.Parameters.AddWithValue("@p7", distance);
                                    command.Parameters.AddWithValue("@p8", numericUpDown1.Value);
                                    command.Parameters.AddWithValue("@p9", 3);
                                    command.Parameters.AddWithValue("@p10", textBox1.Text);
                                    command.Parameters.AddWithValue("@p11", def_name);
                                    command.ExecuteNonQuery();

                                    command.CommandText = "SELECT * FROM player where username=@p12";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p12", att_name);
                                    reader = command.ExecuteReader();
                                    reader.Read();

                                    string temp20 = reader["military"].ToString();
                                    decimal att_military = Convert.ToDecimal(temp20);
                                    reader.Close();
                                    int att_offense = Convert.ToInt32((att_army - numericUpDown1.Value) * (att_military * Convert.ToDecimal(vars["off_fact"]) + 1));
                                    decimal att_defence = Math.Round(att_def_fact * (att_farm + att_craft + att_dealer) + att_offense);

                                    command.CommandText = "UPDATE regions SET army=@p13, defence=@p14, cost=@p15, offense=@p16 Where name=@p17 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p17", att_reg_name);
                                    command.Parameters.AddWithValue("@p13", att_army - numericUpDown1.Value);
                                    command.Parameters.AddWithValue("@p14", Convert.ToInt32(Math.Round( att_defence)));
                                    command.Parameters.AddWithValue("@p16", att_offense);
                                    command.Parameters.AddWithValue("@p15", att_cost - numericUpDown1.Value);
                                    command.ExecuteNonQuery();

                                    MessageBox.Show("Η μετακίνηση στρατού προστέθηκε επιτυχώς.");

                                    filldatagrid();
                                    label2.Text = "";
                                    label3.Text = "";
                                    label6.Text = "";
                                    label7.Text = "";
                                    label9.Text = "";
                                    numericUpDown1.Value = 0;
                                    filldatagrid();
                                }

                            }
                            else
                            {
                                label2.Text = "";
                                label3.Text = "";
                                label6.Text = "";
                                label7.Text = "";
                                label9.Text = "";
                                numericUpDown1.Value = 0;
                                //textBox6.Text = "";
                                MessageBox.Show("Η αμυνόμενη περιοχή δεν σας ανήκει.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ο αριθμός των στρατιωτών που έχετε εισάγει είναι μεγαλύτερος απο αυτόν που διαθέτει η περιοχή σας.");
                        }
                    }
                    else
                    {
                        label2.Text = "";
                        label3.Text = "";
                        label6.Text = "";
                        label7.Text = "";
                        label9.Text = "";
                        numericUpDown1.Value = 0;
                        //textBox6.Text = "";
                        MessageBox.Show("Η επιτιθέμενη περιοχή δεν σας ανήκει.");
                    }
                }
                else
                {
                    label2.Text = "";
                    label3.Text = "";
                    label6.Text = "";
                    label7.Text = "";
                    label9.Text = "";
                    numericUpDown1.Value = 0;
                    //textBox6.Text = "";
                    MessageBox.Show("Η επιτιθέμενη περιοχή δεν σας ανήκει.");
                }

                connection.Close();
            }
            else
            {
                MessageBox.Show("Συμπληρώστε όλα τα απαραίτητα πεδία.");
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if(textBox5.Text != "")
            {
                int attack = Convert.ToInt32(Math.Round((Convert.ToDecimal(textBox4.Text) * Convert.ToDecimal(vars["off_fact"]) + 1) * numericUpDown1.Value));
                label9.Text = Convert.ToString(attack);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                string city = dataGridView1.CurrentCell.Value.ToString();

                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;

                command.CommandText = "SELECT * FROM regions where name=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", city);
                reader = command.ExecuteReader();
                reader.Read();

                string owner = Convert.ToString(reader["owner"]);
                reader.Close();

                if(owner == textBox1.Text)
                {
                    Form8 frm = new Form8(city);
                    frm.Show();
                }
            }
        }
    }
}
