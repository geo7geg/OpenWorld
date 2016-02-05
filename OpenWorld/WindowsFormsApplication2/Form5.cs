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
    public partial class Form5 : Form
    {
        string sqlcon = Variables.sqlstring;
        Dictionary<string, string> vars = Variables.vars();

        public Form5(string username)
        {
            InitializeComponent();

            this.CenterToScreen();
            //this.FormBorderStyle = FormBorderStyle.None;


            listView1.View = View.Details;
            listView1.Columns.Add("Κωδικός Αίτησης", 30, HorizontalAlignment.Left);
            listView1.Columns.Add("Αιτών", 70, HorizontalAlignment.Left);
            listView1.Columns.Add("Αιτούσα Περιοχή", 110, HorizontalAlignment.Left);
            listView1.Columns.Add("Περιοχή Υποστήριξης", 110, HorizontalAlignment.Left);
            listView1.Columns.Add("Γύροι", 50, HorizontalAlignment.Left);
            listView1.Columns.Add("Στρατός", 50, HorizontalAlignment.Left);

            textBox1.Text = username;
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

            if (reader.Read())
            {
                enwsi = reader["enwsi"].ToString();
                sunaspismos = reader["sunasp"].ToString();
            }

            reader.Close();

            command.CommandText = "SELECT * FROM sunaspismos where name=@p2";
            command.Prepare();
            command.Parameters.AddWithValue("@p2", sunaspismos);
            reader = command.ExecuteReader();
            int status = 0;
            int army_live = 0;
            int army = 0;
            if (reader.Read())
            {
                status = Convert.ToInt32(reader["status"]);
                army = Convert.ToInt32(reader["army"]);
                army_live = Convert.ToInt32(reader["army_live"]);
            }

            reader.Close();
            
            if (status == 1)
            {
                pictureBox3.Visible = true;
                pictureBox8.Visible = true;
                label7.Visible = true;
                textBox2.Visible = true;
                numericUpDown5.Visible = true;
                textBox3.Text = sunaspismos;

                textBox2.Text = Convert.ToString(army_live);

                if ((army * Convert.ToDecimal(vars["army_percent_sunasp"])) >= army_live)
                {
                    numericUpDown5.Maximum = army_live;
                }
                else
                {
                    numericUpDown5.Maximum = army * Convert.ToDecimal(vars["army_percent_sunasp"]);
                }
            }

            command.CommandText = "SELECT * FROM enwsi where name=@p3";
            command.Prepare();
            command.Parameters.AddWithValue("@p3", enwsi);
            reader = command.ExecuteReader();
            int status1 = 0;
            int army_live1 = 0;
            int army1 = 0;
            if (reader.Read())
            {
                status1 = Convert.ToInt32(reader["status"]);
                army1 = Convert.ToInt32(reader["army"]);
                army_live1 = Convert.ToInt32(reader["army_live"]);
            }

            reader.Close();

            if (status1 == 1)
            {
                pictureBox4.Visible = true;
                pictureBox5.Visible = true;
                label4.Visible = true;
                textBox4.Visible = true;
                numericUpDown1.Visible = true;
                textBox5.Text = enwsi;

                textBox4.Text = Convert.ToString(army_live1);

                if ((army1 * Convert.ToDecimal(vars["army_percent_enwsi"])) >= army_live1)
                {
                    numericUpDown1.Maximum = army_live1;
                }
                else
                {
                    numericUpDown1.Maximum = army1 * Convert.ToDecimal(vars["army_percent_enwsi"]);
                }
            }
                
            connection.Close();
        }

        private void filldatagrid()
        {
            string str = sqlcon;
            MySqlConnection con = new MySqlConnection(str);

            string com = "Select id,att_player,att_region,def_region,distance,turn,army,army_sunasp,sunaspismos,army_enwsi,enwsi,army_coplayer,region_coplayer from attacks where att_player='" + textBox1.Text + "' and kind=1";
            MySqlDataAdapter adpt = new MySqlDataAdapter(com, con);
            DataSet myDataSet = new DataSet();
            adpt.Fill(myDataSet, "attacks");
            DataTable myDataTable = myDataSet.Tables[0];
            //DataRow tempRow = null;

            string com1 = "Select id,att_player,att_region,def_region,distance,turn,army from attacks where att_player='" + textBox1.Text + "' and kind=2";
            MySqlDataAdapter adpt1 = new MySqlDataAdapter(com1, con);
            DataSet myDataSet1 = new DataSet();
            adpt1.Fill(myDataSet1, "attacks");
            DataTable myDataTable1 = myDataSet1.Tables[0];

            for (int i = 0; i < myDataSet.Tables[0].Rows.Count; i++)
            {
                string army_coplayer = myDataSet.Tables[0].Rows[i].ItemArray[11].ToString();
                string region_coplayer = myDataSet.Tables[0].Rows[i].ItemArray[12].ToString();
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
                int[] armyco2 = list.ToArray();
                int sum_co_army = armyco2.Sum();

                myDataSet.Tables[0].Rows[i][11] = Convert.ToString(sum_co_army);
            }

            dataGridView1.DataSource = myDataSet;
            dataGridView1.DataMember = "attacks";
            dataGridView1.Columns[0].HeaderText = "Κωδικός Περιοχής";
            dataGridView1.Columns[1].HeaderText = "Επιτιθέμενος Παίχτης";
            dataGridView1.Columns[2].HeaderText = "Επιτιθέμενη Περιοχή";
            dataGridView1.Columns[3].HeaderText = "Αμυνόμενη Περιοχή";
            dataGridView1.Columns[4].HeaderText = "Απόσταση";
            dataGridView1.Columns[5].HeaderText = "Γύροι";
            dataGridView1.Columns[6].HeaderText = "Αριθμός Στρατού";
            dataGridView1.Columns[7].HeaderText = "Στρατός Συνασπισμού";
            dataGridView1.Columns[8].HeaderText = "Συνασπισμός";
            dataGridView1.Columns[9].HeaderText = "Στρατός Ένωσης";
            dataGridView1.Columns[10].HeaderText = "Ένωση";
            dataGridView1.Columns[11].HeaderText = "Στρατός Υποστήριξης";
            dataGridView1.Columns[12].HeaderText = "Περιοχές Υποστήριξης";

            dataGridView1.Columns[12].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dataGridView2.DataSource = myDataSet1;
            dataGridView2.DataMember = "attacks";
            dataGridView2.Columns[0].HeaderText = "Κωδικός Περιοχής";
            dataGridView2.Columns[1].HeaderText = "Υποστηρικτητικός Παίχτης";
            dataGridView2.Columns[2].HeaderText = "Υποστηριζόμενη Περιοχή";
            dataGridView2.Columns[3].HeaderText = "Αμυνόμενη Περιοχή";
            dataGridView2.Columns[4].HeaderText = "Απόσταση";
            dataGridView2.Columns[5].HeaderText = "Γύροι";
            dataGridView2.Columns[6].HeaderText = "Αριθμός Στρατού";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            label13.Text = dataGridView1.SelectedCells[0].Value.ToString();
            
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.Value != "" && dataGridView1.CurrentCell.Value != null)
            {
                string text = dataGridView1.SelectedCells[0].Value.ToString();
                MySqlConnection connection = new MySqlConnection(sqlcon);
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;
                connection.Open();

                command.CommandText = "SELECT * FROM attacks where id=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", text);
                reader = command.ExecuteReader();
                reader.Read();
                string att_player = reader["att_player"].ToString();
                string sunaspismos = reader["sunaspismos"].ToString();
                string enwsi = reader["enwsi"].ToString();
                string att_region = reader["att_region"].ToString();
                string def_region = reader["def_region"].ToString();
                int distance = Convert.ToInt32(reader["distance"]);
                int army_sunasp = Convert.ToInt32(reader["army_sunasp"]);
                int army_enwsi = Convert.ToInt32(reader["army_enwsi"]);
                int turn = Convert.ToInt32(reader["turn"]);
                int army = Convert.ToInt32(reader["army"]);
                reader.Close();

                command.CommandText = "SELECT * FROM regions where name=@p2";
                command.Prepare();
                command.Parameters.AddWithValue("@p2", att_region);
                reader = command.ExecuteReader();
                reader.Read();
                int farm = Convert.ToInt32(reader["farm"]);
                int craft = Convert.ToInt32(reader["craft"]);
                int dealer = Convert.ToInt32(reader["dealer"]);
                int att_army = Convert.ToInt32(reader["army"]);
                decimal att_def_fact = Convert.ToDecimal(reader["def_fact"]);
                reader.Close();

                command.CommandText = "SELECT * FROM player where username=@p22";
                command.Prepare();
                command.Parameters.AddWithValue("@p22", att_player);
                reader = command.ExecuteReader();
                reader.Read();

                decimal att_military = Convert.ToDecimal(reader["military"]);

                reader.Close();

                int att_offense1 = Convert.ToInt32(Math.Round((army + att_army) * (att_military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                decimal defence = Math.Round(att_def_fact * (farm + craft + dealer) + att_offense1);
                command.CommandText = "UPDATE regions SET army=@p36, defence=@p37, cost=@p38, offense=@p39 Where name=@p33 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p36", army + att_army);
                command.Parameters.AddWithValue("@p37", Convert.ToInt32(Math.Round(defence)));
                command.Parameters.AddWithValue("@p38", army + att_army);
                command.Parameters.AddWithValue("@p39", att_offense1);
                command.Parameters.AddWithValue("@p33", att_region);
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM attacks Where id='" + text + "'";
                command.Prepare();
                command.ExecuteNonQuery();

                if (army_sunasp != 0)
                {
                    command.Parameters.Clear();

                    command.CommandText = "SELECT * FROM sunaspismos where name=@p1";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p1", sunaspismos);
                    reader = command.ExecuteReader();
                    reader.Read();
                    int army_live = Convert.ToInt32(reader["army_live"]);
                    reader.Close();

                    command.CommandText = "UPDATE sunaspismos SET army_live=@p2 Where name=@p3";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p2", army_live + army_sunasp);
                    command.Parameters.AddWithValue("@p3", sunaspismos);
                    command.ExecuteNonQuery();

                    command.CommandText = "SELECT * FROM sunaspismos where name=@p6";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p6", textBox3.Text);
                    reader = command.ExecuteReader();
                    reader.Read();

                    int army_live1 = Convert.ToInt32(reader["army_live"]);
                    int army1 = Convert.ToInt32(reader["army"]);

                    reader.Close();

                    textBox2.Text = Convert.ToString(army_live1);


                    if ((army1 * Convert.ToDecimal(vars["army_percent_sunasp"])) >= army_live1)
                    {
                        numericUpDown5.Maximum = army_live1;
                    }
                    else
                    {
                        numericUpDown5.Maximum = army1 * Convert.ToDecimal(vars["army_percent_sunasp"]);
                    }
                }

                if (army_enwsi != 0)
                {
                    command.Parameters.Clear();

                    command.CommandText = "SELECT * FROM enwsi where name=@p1";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p1", enwsi);
                    reader = command.ExecuteReader();
                    reader.Read();
                    int army_live = Convert.ToInt32(reader["army_live"]);
                    reader.Close();

                    command.CommandText = "UPDATE enwsi SET army_live=@p2 Where name=@p3";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p2", army_live + army_enwsi);
                    command.Parameters.AddWithValue("@p3", enwsi);
                    command.ExecuteNonQuery();

                    command.CommandText = "SELECT * FROM enwsi where name=@p6";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p6", enwsi);
                    reader = command.ExecuteReader();
                    reader.Read();

                    int army_live1 = Convert.ToInt32(reader["army_live"]);
                    int army1 = Convert.ToInt32(reader["army"]);

                    reader.Close();

                    textBox4.Text = Convert.ToString(army_live1);


                    if ((army1 * Convert.ToDecimal(vars["army_percent_enwsi"])) >= army_live1)
                    {
                        numericUpDown1.Maximum = army_live1;
                    }
                    else
                    {
                        numericUpDown1.Maximum = army1 * Convert.ToDecimal(vars["army_percent_enwsi"]);
                    }
                }

                connection.Close();
                MessageBox.Show("Ο στρατός σας ανακλήθηκε επιτυχώς.");
                filldatagrid();
                if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
                {
                    (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).FillAttack();
                }
            }else
            {
                MessageBox.Show("Επιλέξτε τον κωδικό της επίθεσης");
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentCell != null && dataGridView2.CurrentCell.Value != "" && dataGridView2.CurrentCell.Value != null)
            {
                string text = dataGridView2.CurrentCell.Value.ToString();
                MySqlConnection connection = new MySqlConnection(sqlcon);
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;
                connection.Open();

                command.CommandText = "SELECT * FROM attacks where id=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", text);
                reader = command.ExecuteReader();
                reader.Read();
                string att_player = reader["att_player"].ToString();

                string att_region = reader["att_region"].ToString();
                string def_region = reader["def_region"].ToString();
                int distance = Convert.ToInt32(reader["distance"]);
                int turn = Convert.ToInt32(reader["turn"]);
                int army = Convert.ToInt32(reader["army"]);
                reader.Close();

                command.CommandText = "SELECT * FROM regions where name=@p2";
                command.Prepare();
                command.Parameters.AddWithValue("@p2", att_region);
                reader = command.ExecuteReader();
                reader.Read();
                int farm = Convert.ToInt32(reader["farm"]);
                int craft = Convert.ToInt32(reader["craft"]);
                int dealer = Convert.ToInt32(reader["dealer"]);
                int att_army = Convert.ToInt32(reader["army"]);
                decimal att_def_fact = Convert.ToDecimal(reader["def_fact"]);
                reader.Close();

                command.CommandText = "SELECT * FROM player where username=@p22";
                command.Prepare();
                command.Parameters.AddWithValue("@p22", att_player);
                reader = command.ExecuteReader();
                reader.Read();

                decimal att_military = Convert.ToDecimal(reader["military"]);

                reader.Close();

                int att_offense1 = Convert.ToInt32(Math.Round((army + att_army) * (att_military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                decimal defence = Math.Round(att_def_fact * (farm + craft + dealer) + att_offense1);
                command.CommandText = "UPDATE regions SET army=@p36, defence=@p37, cost=@p38, offense=@p39 Where name=@p33 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p36", army + att_army);
                command.Parameters.AddWithValue("@p37", Convert.ToInt32(Math.Round(defence)));
                command.Parameters.AddWithValue("@p38", army + att_army);
                command.Parameters.AddWithValue("@p39", att_offense1);
                command.Parameters.AddWithValue("@p33", att_region);
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM attacks Where id='" + text + "'";
                command.Prepare();
                command.ExecuteNonQuery();

                command.CommandText = "SELECT * FROM support_check where id=@p3";
                command.Prepare();
                command.Parameters.AddWithValue("@p3", text);
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    string supporter = reader["supporter"].ToString();
                    reader.Close();

                    command.CommandText = "DELETE FROM support_check Where id='" + text + "'";
                    command.Prepare();
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE player SET diplomatic= diplomatic - 1 Where username=@p5 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p5", supporter);
                    command.ExecuteNonQuery();
                }

                connection.Close();
                MessageBox.Show("Ο στρατός σας ανακλήθηκε επιτυχώς.");
                filldatagrid(); 
            }else
            {
                MessageBox.Show("Επιλέξτε τον κωδικό της επίθεσης");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;

                string item = listView1.SelectedItems[0].Text;

                command.CommandText = "SELECT * FROM support where id=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", item);
                reader = command.ExecuteReader();
                reader.Read();
                string requestor = reader["requestor"].ToString();
                string supporter = reader["supporter"].ToString();
                string req_region = reader["req_region"].ToString();
                string sup_region = reader["sup_region"].ToString();
                int turn = Convert.ToInt32(reader["turn"]);
                int army = Convert.ToInt32(reader["army"]);
                int attack_id = Convert.ToInt32(reader["attack_id"]);
                reader.Close();

                command.Parameters.Clear();

                command.CommandText = "Select * from attacks where id=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", attack_id);
                reader = command.ExecuteReader();
                int id = 0;
                int army_sunasp = 0;
                int army_enwsi = 0;
                string army_coplayer = "";
                string region_coplayer = "";
                string def_region = "";
                if(reader.Read())
                {
                    id = Convert.ToInt32(reader["id"]);
                    army_sunasp = Convert.ToInt32(reader["army_sunasp"]);
                    army_enwsi = Convert.ToInt32(reader["army_enwsi"]);
                    army_coplayer = reader["army_coplayer"].ToString();
                    region_coplayer = reader["region_coplayer"].ToString();
                    def_region = reader["def_region"].ToString();
                }
                reader.Close();

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
                int[] armyco2 = list.ToArray();
                int sum_co_army = armyco2.Sum();

                command.CommandText = "SELECT * FROM regions where name=@p2";
                command.Prepare();
                command.Parameters.AddWithValue("@p2", req_region);
                reader = command.ExecuteReader();
                reader.Read();
                int x = Convert.ToInt32(reader["x"]);
                int y = Convert.ToInt32(reader["y"]);
                string neighbor = reader["neighbor"].ToString();
                string neighbor1 = reader["neighbor1"].ToString();
                string neighbor2 = reader["neighbor2"].ToString();
                string neighbor3 = reader["neighbor3"].ToString();
                string[] words = neighbor.Split(',');
                string[] words1 = neighbor1.Split(',');
                string[] words2 = neighbor2.Split(',');
                string[] words3 = neighbor3.Split(',');
                reader.Close();

                command.CommandText = "SELECT * FROM regions where name=@p3";
                command.Prepare();
                command.Parameters.AddWithValue("@p3", sup_region);
                reader = command.ExecuteReader();
                reader.Read();
                int sup_id = Convert.ToInt32(reader["id"]);
                int x1 = Convert.ToInt32(reader["x"]);
                int y1 = Convert.ToInt32(reader["y"]);
                int sup_farm = Convert.ToInt32(reader["farm"]);
                int sup_craft = Convert.ToInt32(reader["craft"]);
                int sup_dealer = Convert.ToInt32(reader["dealer"]);
                int sup_army = Convert.ToInt32(reader["army"]);
                decimal sup_def_fact = Convert.ToDecimal(reader["def_fact"]);
                reader.Close();

                command.CommandText = "SELECT * FROM regions where name=@p30";
                command.Prepare();
                command.Parameters.AddWithValue("@p30", def_region);
                reader = command.ExecuteReader();

                int x2 = 0;
                int y2 = 0;

                if(reader.Read())
                {
                    x2 = Convert.ToInt32(reader["x"]);
                    y2 = Convert.ToInt32(reader["y"]);
                }
                
                reader.Close();

                int distance = 0;

                if (attack_id == 0)
                {
                    distance = Convert.ToInt32(Math.Round(Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1))));
                }
                else
                {
                    distance = Convert.ToInt32(Math.Round(Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2))));
                }

                int i = 1;
                int flag = 0;

                while (flag == 0)
                {
                    command.CommandText = "SELECT * FROM attacks where id=@p13";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p13", i);
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

                command.CommandText = "SELECT * FROM player where username=@p22";
                command.Prepare();
                command.Parameters.AddWithValue("@p22", supporter);
                reader = command.ExecuteReader();
                reader.Read();

                decimal military = Convert.ToDecimal(reader["military"]);

                reader.Close();

                if (turn >= distance)
                {
                    if (sup_army >= army && attack_id == 0)
                    {
                        command.CommandText = "insert into attacks (id,att_player,def_player,att_region,def_region,distance,turn,army,kind) values (@p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12)";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p4", i);
                        command.Parameters.AddWithValue("@p5", textBox1.Text);
                        command.Parameters.AddWithValue("@p6", requestor);
                        command.Parameters.AddWithValue("@p7", sup_region);
                        command.Parameters.AddWithValue("@p8", req_region);
                        command.Parameters.AddWithValue("@p9", distance);
                        command.Parameters.AddWithValue("@p10", turn);
                        command.Parameters.AddWithValue("@p11", army);
                        command.Parameters.AddWithValue("@p12", 2);
                        command.ExecuteNonQuery();

                        int offense = Convert.ToInt32(Math.Round((sup_army - army) * (military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                        decimal defence = Math.Round(sup_def_fact * (sup_farm + sup_craft + sup_dealer) + offense);

                        command.CommandText = "UPDATE regions SET army=@p23, defence=@p24, cost=@p25, offense=@p26 Where name=@p27 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p23", sup_army - army);
                        command.Parameters.AddWithValue("@p24", Convert.ToInt32(Math.Round(defence)));
                        command.Parameters.AddWithValue("@p25", sup_army - army);
                        command.Parameters.AddWithValue("@p26", offense);
                        command.Parameters.AddWithValue("@p27", sup_region);
                        command.ExecuteNonQuery();

                        Variables.calculate_cost(sup_id);

                        command.CommandText = "insert into support_check (id,requestor,supporter,req_region,sup_region,turn) values (@p16, @p17, @p18, @p19, @p20, @p21)";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p16", i);
                        command.Parameters.AddWithValue("@p17", requestor);
                        command.Parameters.AddWithValue("@p18", textBox1.Text);
                        command.Parameters.AddWithValue("@p19", req_region);
                        command.Parameters.AddWithValue("@p20", sup_region);
                        command.Parameters.AddWithValue("@p21", turn);
                        command.ExecuteNonQuery();

                        command.CommandText = "DELETE FROM support Where id=@p14";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p14", item);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Η υποστήριξη προστέθηκε επιτυχώς με " + army + " στρατιώτες");
                    }
                    else if (sup_army < army && sup_army != 0 && attack_id == 0)
                    {
                        army = sup_army;
                        command.CommandText = "insert into attacks (id,att_player,def_player,att_region,def_region,distance,turn,army,kind) values (@p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12)";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p4", i);
                        command.Parameters.AddWithValue("@p5", textBox1.Text);
                        command.Parameters.AddWithValue("@p6", requestor);
                        command.Parameters.AddWithValue("@p7", sup_region);
                        command.Parameters.AddWithValue("@p8", req_region);
                        command.Parameters.AddWithValue("@p9", distance);
                        command.Parameters.AddWithValue("@p10", turn);
                        command.Parameters.AddWithValue("@p11", army);
                        command.Parameters.AddWithValue("@p12", 2);
                        command.ExecuteNonQuery();

                        decimal defence = Math.Round(sup_def_fact * (sup_farm + sup_craft + sup_dealer) + 0);

                        command.CommandText = "UPDATE regions SET army=@p23, defence=@p24, cost=@p25, offense=@p26 Where name=@p27 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p23", 0);
                        command.Parameters.AddWithValue("@p24", Convert.ToInt32(Math.Round(defence)));
                        command.Parameters.AddWithValue("@p25", 0);
                        command.Parameters.AddWithValue("@p26", 0);
                        command.Parameters.AddWithValue("@p27", sup_region);
                        command.ExecuteNonQuery();

                        Variables.calculate_cost(sup_id);

                        command.CommandText = "insert into support_check (id,requestor,supporter,req_region,sup_region,turn) values (@p16, @p17, @p18, @p19, @p20, @p21)";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p16", i);
                        command.Parameters.AddWithValue("@p17", requestor);
                        command.Parameters.AddWithValue("@p18", textBox1.Text);
                        command.Parameters.AddWithValue("@p19", req_region);
                        command.Parameters.AddWithValue("@p20", sup_region);
                        command.Parameters.AddWithValue("@p21", turn);
                        command.ExecuteNonQuery();

                        command.CommandText = "DELETE FROM support Where id=@p14";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p14", item);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Η υποστήριξη προστέθηκε επιτυχώς με " + army + " στρατιώτες");
                    }
                    else if (sup_army < army && sup_army == 0 && attack_id == 0)
                    {
                        command.CommandText = "DELETE FROM support Where id=@p15";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p15", item);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Δεν έχετε στρατό για να υποστηρίξετε την άμυνα στην " + req_region);
                    }
                    else if (sup_army >= army && attack_id != 0)
                    {
                        Dictionary<string, int> coplayers = new Dictionary<string, int>();
                        if (list.Any())
                        {
                            coplayers = Enumerable.Range(0, region_co.Length).ToDictionary(w => region_co[w], w => list[w]);
                        }

                        string army_coplayer1 = "";
                        if (army_coplayer == "")
                        {
                            army_coplayer1 = Convert.ToString(army);
                        }
                        else
                        {
                            if (!coplayers.ContainsKey(sup_region))
                            {
                                army_coplayer1 = army_coplayer + "," + Convert.ToString(army);
                            }
                            else
                            {
                                coplayers[sup_region] = coplayers[sup_region] + army;
                                int[] co_armys = coplayers.Values.ToArray();
                                string[] co_armys1 = Array.ConvertAll(co_armys, element => element.ToString());
                                army_coplayer1 = string.Join(",", co_armys1);
                            }
                        }
                        string region_coplayer1 = "";
                        if (region_coplayer == "")
                        {
                            region_coplayer1 = sup_region;
                        }
                        else
                        {
                            if (!coplayers.ContainsKey(sup_region))
                            {
                                region_coplayer1 = region_coplayer + "," + sup_region;
                            }
                            else
                            {
                                region_coplayer1 = region_coplayer;
                            }
                            
                        }
                        command.CommandText = "UPDATE attacks SET army_coplayer=@p23, region_coplayer=@p24 Where id=@p27 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p23", army_coplayer1);
                        command.Parameters.AddWithValue("@p24", region_coplayer1);
                        command.Parameters.AddWithValue("@p27", attack_id);
                        command.ExecuteNonQuery();

                        int offense = Convert.ToInt32(Math.Round((sup_army - army) * (military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                        decimal defence = Math.Round(sup_def_fact * (sup_farm + sup_craft + sup_dealer) + offense);

                        command.Parameters.Clear();

                        command.CommandText = "UPDATE regions SET army=@p23, defence=@p24, cost=@p25, offense=@p26 Where name=@p27 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p23", sup_army - army);
                        command.Parameters.AddWithValue("@p24", Convert.ToInt32(Math.Round(defence)));
                        command.Parameters.AddWithValue("@p25", sup_army - army);
                        command.Parameters.AddWithValue("@p26", offense);
                        command.Parameters.AddWithValue("@p27", sup_region);
                        command.ExecuteNonQuery();

                        Variables.calculate_cost(sup_id);

                        command.CommandText = "insert into support_check (id,requestor,supporter,req_region,sup_region,turn) values (@p16, @p17, @p18, @p19, @p20, @p21)";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p16", id);
                        command.Parameters.AddWithValue("@p17", requestor);
                        command.Parameters.AddWithValue("@p18", textBox1.Text);
                        command.Parameters.AddWithValue("@p19", req_region);
                        command.Parameters.AddWithValue("@p20", sup_region);
                        command.Parameters.AddWithValue("@p21", turn);
                        command.ExecuteNonQuery();

                        command.CommandText = "DELETE FROM support Where id=@p14";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p14", item);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Η υποστήριξη προστέθηκε επιτυχώς με " + army + " στρατιώτες");
                    }
                    else if (sup_army < army && sup_army != 0 && attack_id != 0)
                    {
                        Dictionary<string, int> coplayers = new Dictionary<string, int>();
                        if (list.Any())
                        {
                            coplayers = Enumerable.Range(0, region_co.Length).ToDictionary(w => region_co[w], w => list[w]);
                        }

                        army = sup_army;
                        string army_coplayer1 = "";
                        if (army_coplayer == "")
                        {
                            army_coplayer1 = Convert.ToString(army);
                        }
                        else
                        {
                            if (!coplayers.ContainsKey(sup_region))
                            {
                                army_coplayer1 = army_coplayer + "," + Convert.ToString(army);
                            }
                            else
                            {
                                coplayers[sup_region] = coplayers[sup_region] + army;
                                int[] co_armys = coplayers.Values.ToArray();
                                string[] co_armys1 = Array.ConvertAll(co_armys, element => element.ToString());
                                army_coplayer1 = string.Join(",", co_armys1);
                            }
                        }
                        string region_coplayer1 = "";
                        if (region_coplayer == "")
                        {
                            region_coplayer1 = sup_region;
                        }
                        else
                        {
                            if (!coplayers.ContainsKey(sup_region))
                            {
                                region_coplayer1 = region_coplayer + "," + sup_region;
                            }
                            else
                            {
                                region_coplayer1 = region_coplayer;
                            }
                        }
                        command.CommandText = "UPDATE attacks SET army_coplayer=@p23, region_coplayer=@p24 Where id=@p27 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p23", army_coplayer1);
                        command.Parameters.AddWithValue("@p24", region_coplayer1);
                        command.Parameters.AddWithValue("@p27", attack_id);
                        command.ExecuteNonQuery();

                        decimal defence = Math.Round(sup_def_fact * (sup_farm + sup_craft + sup_dealer) + 0);

                        command.Parameters.Clear();

                        command.CommandText = "UPDATE regions SET army=@p23, defence=@p24, cost=@p25, offense=@p26 Where name=@p27 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p23", 0);
                        command.Parameters.AddWithValue("@p24", Convert.ToInt32(Math.Round(defence)));
                        command.Parameters.AddWithValue("@p25", 0);
                        command.Parameters.AddWithValue("@p26", 0);
                        command.Parameters.AddWithValue("@p27", sup_region);
                        command.ExecuteNonQuery();

                        Variables.calculate_cost(sup_id);

                        command.CommandText = "insert into support_check (id,requestor,supporter,req_region,sup_region,turn) values (@p16, @p17, @p18, @p19, @p20, @p21)";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p16", id);
                        command.Parameters.AddWithValue("@p17", requestor);
                        command.Parameters.AddWithValue("@p18", textBox1.Text);
                        command.Parameters.AddWithValue("@p19", req_region);
                        command.Parameters.AddWithValue("@p20", sup_region);
                        command.Parameters.AddWithValue("@p21", turn);
                        command.ExecuteNonQuery();

                        command.CommandText = "DELETE FROM support Where id=@p14";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p14", item);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Η υποστήριξη προστέθηκε επιτυχώς με " + army + " στρατιώτες");
                    }
                    else if (sup_army < army && sup_army == 0 && attack_id != 0)
                    {
                        command.CommandText = "DELETE FROM support Where id=@p15";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p15", item);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Δεν έχετε στρατό για να υποστηρίξετε την επίθεση της " + req_region);
                    }
                }
                else if(attack_id == 0)
                {
                    command.CommandText = "DELETE FROM support Where id=@p15";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p15", item);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Δεν προλαβαίνετε να αναχαιτίσετε την επίθεση στην " + req_region);
                }
                else
                {
                    command.CommandText = "DELETE FROM support Where id=@p15";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p15", item);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Δεν προλαβαίνετε να βοηθήσετε την επίθεση της " + req_region);
                }
                connection.Close();
                filldatagrid();
                fillrequest();
            }
            else
            {
                MessageBox.Show("Επιλέξτε κωδικό αιτήματος");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();

                string item = listView1.SelectedItems[0].Text;

                command.CommandText = "DELETE FROM support Where id=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", item);
                command.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Η Aίτηση Υποστήριξης ακυρώθηκε");
                filldatagrid();
                fillrequest();
            }
            else
            {
                MessageBox.Show("Επιλέξτε κωδικό αιτήματος");
            }
        }

        public void fillrequest()
        {
            string str = sqlcon;
            MySqlConnection con = new MySqlConnection(str);


            string com = "Select * from support where supporter='" + textBox1.Text + "'";
            MySqlDataAdapter adpt = new MySqlDataAdapter(com, con);
            DataSet myDataSet = new DataSet();
            adpt.Fill(myDataSet, "support");
            DataTable myDataTable = myDataSet.Tables[0];
            DataRow tempRow = null;

            listView1.Items.Clear();

            con.Open();
            foreach (DataRow tempRow_Variable in myDataTable.Rows)
            {

                tempRow = tempRow_Variable;
                //listBox5.Items.Add((tempRow["id"]));

                ListViewItem lvi = new ListViewItem(tempRow["id"].ToString());

                lvi.SubItems.Add(tempRow["requestor"].ToString());
                lvi.SubItems.Add(tempRow["req_region"].ToString());
                lvi.SubItems.Add(tempRow["sup_region"].ToString());
                lvi.SubItems.Add(tempRow["turn"].ToString());
                lvi.SubItems.Add(tempRow["army"].ToString());

                lvi.ForeColor = Color.Red;

                // You also have access to the list view's SubItems collection
                lvi.SubItems[0].ForeColor = Color.Blue;
                lvi.SubItems[1].ForeColor = Color.Blue;
                lvi.SubItems[2].ForeColor = Color.Blue;

                // Add the list items to the ListView
                listView1.Items.Add(lvi);
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.Value != "" && dataGridView1.CurrentCell.Value != null)
            {
                string text = dataGridView1.SelectedCells[0].Value.ToString();
                MySqlConnection connection = new MySqlConnection(sqlcon);
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;
                connection.Open();

                command.CommandText = "SELECT * FROM sunaspismos where name=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", textBox3.Text);
                reader = command.ExecuteReader();
                reader.Read();

                int army_live = Convert.ToInt32(reader["army_live"]);
                string name = Convert.ToString(reader["name"]);

                reader.Close();

                if (army_live >= numericUpDown5.Value)
                {
                    command.CommandText = "UPDATE attacks SET army_sunasp= army_sunasp + @p2,sunaspismos=@p7 Where id=@p3 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p2", numericUpDown5.Value);
                    command.Parameters.AddWithValue("@p3", text);
                    command.Parameters.AddWithValue("@p7", name);
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE sunaspismos SET army_live=@p4 Where name=@p5 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p4", army_live - numericUpDown5.Value);
                    command.Parameters.AddWithValue("@p5", textBox3.Text);
                    command.ExecuteNonQuery();

                    command.CommandText = "SELECT * FROM sunaspismos where name=@p6";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p6", textBox3.Text);
                    reader = command.ExecuteReader();
                    reader.Read();

                    int army_live1 = Convert.ToInt32(reader["army_live"]);
                    int army = Convert.ToInt32(reader["army"]);

                    reader.Close();

                    textBox2.Text = Convert.ToString(army_live1);

                    filldatagrid();

                    MessageBox.Show("Η επίθεση σας ενισχύθηκε με " + numericUpDown5.Value + " στρατιώτες συνασπισμού");

                    if ((army * Convert.ToDecimal(vars["army_percent_sunasp"])) >= army_live1)
                    {
                        numericUpDown5.Maximum = army_live1;
                    }
                    else
                    {
                        numericUpDown5.Maximum = army * Convert.ToDecimal(vars["army_percent_sunasp"]);
                    }

                    if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
                    {
                        (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).FillAttack();
                    }
                }
                else
                {
                    command.CommandText = "SELECT * FROM sunaspismos where name=@p6";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p6", textBox3.Text);
                    reader = command.ExecuteReader();
                    reader.Read();

                    int army_live1 = Convert.ToInt32(reader["army_live"]);
                    int army = Convert.ToInt32(reader["army"]);

                    reader.Close();

                    textBox2.Text = Convert.ToString(army_live1);

                    if ((army * Convert.ToDecimal(vars["army_percent_sunasp"])) >= army_live1)
                    {
                        numericUpDown5.Maximum = army_live1;
                    }
                    else
                    {
                        numericUpDown5.Maximum = army * Convert.ToDecimal(vars["army_percent_sunasp"]);
                    }
                    MessageBox.Show("Δεν επαρκούν οι στρατιώτες που ζητήσατε");
                }

                connection.Close();
            }
            else
            {
                MessageBox.Show("Επιλέξτε τον κωδικό της επίθεσης της οποίας θέλετε να \n υποστηρίξετε με στρατιώτες απο τον συνασπισμό");
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.Value != "" && dataGridView1.CurrentCell.Value != null)
            {
                string text = dataGridView1.SelectedCells[0].Value.ToString();
                MySqlConnection connection = new MySqlConnection(sqlcon);
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;
                connection.Open();

                command.CommandText = "SELECT * FROM enwsi where name=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", textBox5.Text);
                reader = command.ExecuteReader();
                reader.Read();

                int army_live = Convert.ToInt32(reader["army_live"]);
                string name = Convert.ToString(reader["name"]);

                reader.Close();

                if (army_live >= numericUpDown1.Value)
                {
                    command.CommandText = "UPDATE attacks SET army_enwsi= army_enwsi + @p2,enwsi=@p7 Where id=@p3 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p2", numericUpDown1.Value);
                    command.Parameters.AddWithValue("@p3", text);
                    command.Parameters.AddWithValue("@p7", name);
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE enwsi SET army_live=@p4 Where name=@p5 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p4", army_live - numericUpDown1.Value);
                    command.Parameters.AddWithValue("@p5", textBox5.Text);
                    command.ExecuteNonQuery();

                    command.CommandText = "SELECT * FROM enwsi where name=@p6";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p6", textBox5.Text);
                    reader = command.ExecuteReader();
                    reader.Read();

                    int army_live1 = Convert.ToInt32(reader["army_live"]);
                    int army = Convert.ToInt32(reader["army"]);

                    reader.Close();

                    textBox4.Text = Convert.ToString(army_live1);

                    filldatagrid();

                    MessageBox.Show("Η επίθεση σας ενισχύθηκε με " + numericUpDown1.Value + " στρατιώτες ένωσης");

                    if ((army * Convert.ToDecimal(vars["army_percent_enwsi"])) >= army_live1)
                    {
                        numericUpDown1.Maximum = army_live1;
                    }
                    else
                    {
                        numericUpDown1.Maximum = army * Convert.ToDecimal(vars["army_percent_enwsi"]);
                    }

                    if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
                    {
                        (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).FillAttack();
                    }
                }
                else
                {
                    command.CommandText = "SELECT * FROM enwsi where name=@p6";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p6", textBox5.Text);
                    reader = command.ExecuteReader();
                    reader.Read();

                    int army_live1 = Convert.ToInt32(reader["army_live"]);
                    int army = Convert.ToInt32(reader["army"]);

                    reader.Close();

                    textBox4.Text = Convert.ToString(army_live1);

                    if ((army * Convert.ToDecimal(vars["army_percent_enwsi"])) >= army_live1)
                    {
                        numericUpDown1.Maximum = army_live1;
                    }
                    else
                    {
                        numericUpDown1.Maximum = army * Convert.ToDecimal(vars["army_percent_enwsi"]);
                    }
                    MessageBox.Show("Δεν επαρκούν οι στρατιώτες που ζητήσατε");
                }

                connection.Close();
            }
            else
            {
                MessageBox.Show("Επιλέξτε τον κωδικό της επίθεσης της οποίας θέλετε να \n υποστηρίξετε με στρατιώτες απο την ένωση");
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
             if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.Value != "" && dataGridView1.CurrentCell.Value != null)
            {
                string text = dataGridView1.CurrentCell.Value.ToString();

                MySqlConnection connection = new MySqlConnection(sqlcon);
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;
                connection.Open();

                command.CommandText = "SELECT * FROM attacks where id=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", Convert.ToInt32(text));
                reader = command.ExecuteReader();

                if (reader.Read())
                {

                    string att_region = reader["def_region"].ToString();
                    int turn = Convert.ToInt32(reader["turn"]);

                    reader.Close();

                    command.CommandText = "SELECT * FROM regions where name=@p2";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p2", att_region);
                    reader = command.ExecuteReader();
                    reader.Read();

                    string att_reg_owner = textBox1.Text;
                    int x = Convert.ToInt32(reader["x"]);
                    int y = Convert.ToInt32(reader["y"]);
                    string neighbor = reader["neighbor"].ToString();
                    string neighbor1 = reader["neighbor1"].ToString();
                    string neighbor2 = reader["neighbor2"].ToString();
                    string neighbor3 = reader["neighbor3"].ToString();
                    string[] words = neighbor.Split(',');
                    string[] words1 = neighbor1.Split(',');
                    string[] words2 = neighbor2.Split(',');
                    string[] words3 = neighbor3.Split(',');

                    reader.Close();

                    string com1 = "Select id,name,owner,army,defence,offense,x,y from regions where name='bgh'";
                    MySqlDataAdapter adpt1 = new MySqlDataAdapter(com1, connection);
                    DataSet myDataSet1 = new DataSet();
                    adpt1.Fill(myDataSet1, "regions");
                    DataTable myDataTable1 = myDataSet1.Tables[0];

                    string com = "Select id,name,owner,army,defence,offense,x,y from regions";
                    MySqlDataAdapter adpt2 = new MySqlDataAdapter(com, connection);
                    DataSet myDataSet2 = new DataSet();

                    adpt2.Fill(myDataSet2, "regions");
                    DataTable myDataTable2 = myDataSet2.Tables[0];

                    DataRow tempRow1;

                    int j = myDataTable2.Rows.Count - 1;
                    for (int i = 0; i <= j; i++)
                    {

                        tempRow1 = myDataTable2.Rows[i];

                        string reg_name = Convert.ToString(tempRow1["name"]);
                        string reg_owner = Convert.ToString(tempRow1["owner"]);
                        int x1 = Convert.ToInt32(tempRow1["x"]);
                        int y1 = Convert.ToInt32(tempRow1["y"]);

                        int distance = Convert.ToInt32(Math.Round(Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1))));

                        if (distance <= turn)
                        {
                            if (att_reg_owner == reg_owner)
                            {
                                myDataTable1.ImportRow(myDataTable2.Rows[i]);
                            }
                            else
                            {
                                command.CommandText = "SELECT * FROM reg_treaties where region_a=@p3 and region_b=@p4";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p3", att_region);
                                command.Parameters.AddWithValue("@p4", reg_name);
                                reader = command.ExecuteReader();

                                if (reader.Read())
                                {
                                    myDataTable1.ImportRow(myDataTable2.Rows[i]);
                                }
                                else
                                {
                                    reader.Close();
                                    command.CommandText = "SELECT * FROM treaties where player_a=@p5 and player_b=@p6";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p5", att_reg_owner);
                                    command.Parameters.AddWithValue("@p6", reg_owner);
                                    reader = command.ExecuteReader();
                                    if (reader.Read())
                                    {
                                        myDataTable1.ImportRow(myDataTable2.Rows[i]);
                                    }
                                    else
                                    {
                                        reader.Close();

                                        command.CommandText = "SELECT * FROM player where username=@p7";
                                        command.Prepare();
                                        command.Parameters.AddWithValue("@p7", att_reg_owner);
                                        reader = command.ExecuteReader();
                                        string enwsi = "";
                                        string sunaspismos = "";
                                        if (reader.Read())
                                        {
                                            enwsi = reader["enwsi"].ToString();
                                            sunaspismos = reader["sunasp"].ToString();
                                        }
                                        reader.Close();

                                        command.CommandText = "SELECT * FROM player where username=@p8";
                                        command.Prepare();
                                        command.Parameters.AddWithValue("@p8", reg_owner);
                                        reader = command.ExecuteReader();
                                        string enwsi1 = "";
                                        string sunaspismos1 = "";
                                        if (reader.Read())
                                        {
                                            enwsi1 = reader["enwsi"].ToString();
                                            sunaspismos1 = reader["sunasp"].ToString();
                                        }
                                        reader.Close();

                                        if (sunaspismos == sunaspismos1 && sunaspismos != "" && sunaspismos1 != "")
                                        {
                                            myDataTable1.ImportRow(myDataTable2.Rows[i]);
                                        }
                                        if (enwsi == enwsi1 && enwsi != "" && enwsi1 != "")
                                        {
                                            myDataTable1.ImportRow(myDataTable2.Rows[i]);
                                        }
                                    }
                                }
                                reader.Close();
                            }
                        }

                        command.Parameters.Clear();

                    }


                    dataGridView3.DataSource = myDataSet1;
                    dataGridView3.DataMember = "regions";
                    dataGridView3.Columns[6].Visible = false;
                    dataGridView3.Columns[7].Visible = false;
                    dataGridView3.Columns[0].HeaderText = "Κωδικός Περιοχής";
                    dataGridView3.Columns[1].HeaderText = "Όνομα Περιοχής";
                    dataGridView3.Columns[2].HeaderText = "Ιδιοκτήτης";
                    dataGridView3.Columns[3].HeaderText = "Στρατός";
                    dataGridView3.Columns[4].HeaderText = "Αμυντική Ισχύς";
                    dataGridView3.Columns[5].HeaderText = "Επιθετική Ισχύς";
                    dataGridView3.Columns[6].HeaderText = "x";
                    dataGridView3.Columns[7].HeaderText = "y";
                    dataGridView3.Visible = true;
                    button4.Visible = true;
                }
                reader.Close();

                connection.Close();
            }
            else
            {
                MessageBox.Show("Επιλέξτε τον κωδικό της επίθεσης");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView3.CurrentCell != null && dataGridView3.CurrentCell.Value != "" && dataGridView3.CurrentCell.Value != null)
            {
                int selectedcolumnindex = dataGridView3.SelectedCells[0].ColumnIndex;
                if (selectedcolumnindex == 0)
                {
                    string text = dataGridView1.CurrentCell.Value.ToString();
                    string text1 = dataGridView3.CurrentCell.Value.ToString();

                    MySqlConnection connection = new MySqlConnection(sqlcon);
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    MySqlDataReader reader;

                    command.CommandText = "Select * from attacks where id=@p1";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p1", text);
                    reader = command.ExecuteReader();
                    reader.Read();

                    string att_player = reader["att_player"].ToString();
                    string def_player = reader["def_player"].ToString();
                    string req_region = reader["att_region"].ToString();
                    string def_region = reader["def_region"].ToString();
                    int turn = Convert.ToInt32(reader["turn"]);
                    int id = Convert.ToInt32(reader["id"]);
                    int army = Convert.ToInt32(reader["army"]);
                    int army_sunasp = Convert.ToInt32(reader["army_sunasp"]);
                    int army_enwsi = Convert.ToInt32(reader["army_enwsi"]);
                    string army_coplayer = reader["army_coplayer"].ToString();
                    string region_coplayer = reader["region_coplayer"].ToString();
                    string[] army_co = army_coplayer.Split(',');

                    List<int> list = new List<int>();
                    if (army_coplayer != "")
                    {
                        foreach (string army1 in army_co)
                        {
                            int army_co1 = Int32.Parse(army1);
                            list.Add(army_co1);
                        }
                    }
                    int[] armyco2 = list.ToArray();
                    int sum_co_army = armyco2.Sum();
                    reader.Close();

                    command.CommandText = "Select * from regions where name=@p11";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p11", def_region);
                    reader = command.ExecuteReader();
                    reader.Read();

                    int defence = Convert.ToInt32(reader["defence"]);
                    int def_farm = Convert.ToInt32(reader["farm"]);
                    int def_craft = Convert.ToInt32(reader["craft"]);
                    int def_dealer = Convert.ToInt32(reader["dealer"]);
                    int def_army = Convert.ToInt32(reader["army"]);
                    decimal def_fact = Convert.ToDecimal(reader["def_fact"]);

                    reader.Close();

                    command.CommandText = "Select * from player where username=@p12";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p12", att_player);
                    reader = command.ExecuteReader();
                    reader.Read();

                    decimal military = Convert.ToDecimal(reader["military"]);

                    reader.Close();

                    command.Parameters.Clear();

                    command.CommandText = "Select * from player where username=@p12";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p12", att_player);
                    reader = command.ExecuteReader();
                    reader.Read();

                    decimal def_military = Convert.ToDecimal(reader["military"]);

                    reader.Close();

                    command.CommandText = "Select * from regions where id=@p9";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p9", text1);
                    reader = command.ExecuteReader();
                    reader.Read();

                    string supporter = reader["owner"].ToString();
                    string sup_region = reader["name"].ToString();

                    reader.Close();

                    int def_offense = Convert.ToInt32(Math.Round(def_army * (def_military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                    int def_defence = Convert.ToInt32(Math.Round(def_fact * (def_farm + def_craft + def_dealer) + def_offense));

                    command.CommandText = "SELECT * FROM def_othomanoi where name='" + def_region + "'";
                    command.Prepare();
                    reader = command.ExecuteReader();
                    int factor1 = 0;
                    int flag5 = 0;
                    if (reader.Read())
                    {
                        factor1 = Convert.ToInt32(reader["factor"]);
                        flag5 = Convert.ToInt32(reader["flag"]);
                        if (flag5 == 1)
                        {
                            def_defence = def_defence * factor1;
                        }
                    }

                    reader.Close();

                    int sup_army = def_defence - Convert.ToInt32(Math.Round(((army + army_enwsi + army_sunasp + sum_co_army) * (military * Convert.ToDecimal(vars["off_fact"]) + 1))));

                    if (sup_army > 0 && textBox1.Text != supporter)
                    {
                        int i = 1;
                        int flag = 0;

                        while (flag == 0)
                        {
                            command.CommandText = "SELECT * FROM support where id=@p10";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p10", i);
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

                        command.CommandText = "insert into support (id,attack_id,requestor,supporter,req_region,sup_region,turn,army) values (@p2, @p13, @p3, @p4, @p5, @p6, @p7, @p8)";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p2", i);
                        command.Parameters.AddWithValue("@p3", textBox1.Text);
                        command.Parameters.AddWithValue("@p4", supporter);
                        command.Parameters.AddWithValue("@p5", req_region);
                        command.Parameters.AddWithValue("@p6", sup_region);
                        command.Parameters.AddWithValue("@p7", turn);
                        command.Parameters.AddWithValue("@p8", sup_army);
                        command.Parameters.AddWithValue("@p13", id);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Το αίτημα βοηθείας στάλθηκε επιτυχώς");
                    }
                    else if (textBox1.Text == supporter)
                    {
                        MessageBox.Show("Η βοήθεια που ζητάτε είναι από δική σας περιοχή");
                    }
                    else
                    {
                        MessageBox.Show("Δεν χρειάζεστε βοήθεια για να αντιμετωπίσετε αυτήν την επίθεση");
                    }

                    connection.Close();
                }
                else
                {
                    MessageBox.Show("Επιλέξτε τον κωδικό της περιοχής");
                }
            }
            else
            {
                MessageBox.Show("Επιλέξτε τον κωδικό της περιοχής");
            }
        }
    }
}
