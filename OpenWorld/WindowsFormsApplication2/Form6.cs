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
    public partial class Form6 : Form
    {
        string sqlcon = Variables.sqlstring;
        Dictionary<string, string> vars = Variables.vars();

        public Form6(string username)
        {
            InitializeComponent();

            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.None;

            string str = sqlcon;
            MySqlConnection con = new MySqlConnection(str);
            textBox1.Text = username;

            string com = "Select id,att_player,att_region,def_region,distance,turn,army from attacks where def_player='" + username + "'";
            MySqlDataAdapter adpt = new MySqlDataAdapter(com, con);
            DataSet myDataSet = new DataSet();
            adpt.Fill(myDataSet, "attacks");
            DataTable myDataTable = myDataSet.Tables[0];
            //DataRow tempRow = null;

            dataGridView1.DataSource = myDataSet;
            dataGridView1.DataMember = "attacks";
            dataGridView1.Columns[0].HeaderText = "Κωδικός Επίθεσης";
            dataGridView1.Columns[1].HeaderText = "Επιτιθέμενος Παίχτης";
            dataGridView1.Columns[2].HeaderText = "Επιτιθέμενη Περιοχή";
            dataGridView1.Columns[3].HeaderText = "Αμυνόμενη Περιοχή";
            dataGridView1.Columns[4].HeaderText = "Απόσταση";
            dataGridView1.Columns[5].HeaderText = "Γύροι";
            dataGridView1.Columns[6].HeaderText = "Αριθμός Στρατού";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
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

                    string def_region = reader["def_region"].ToString();
                    int turn = Convert.ToInt32(reader["turn"]);

                    reader.Close();

                    command.CommandText = "SELECT * FROM regions where name=@p2";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p2", def_region);
                    reader = command.ExecuteReader();
                    reader.Read();

                    string def_reg_owner = textBox1.Text;
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
                            if (def_reg_owner == reg_owner)
                            {
                                myDataTable1.ImportRow(myDataTable2.Rows[i]);
                            }
                            else
                            {
                                command.CommandText = "SELECT * FROM reg_treaties where region_a=@p3 and region_b=@p4";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p3", def_region);
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
                                    command.Parameters.AddWithValue("@p5", def_reg_owner);
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
                                        command.Parameters.AddWithValue("@p7", def_reg_owner);
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


                    dataGridView2.DataSource = myDataSet1;
                    dataGridView2.DataMember = "regions";
                    dataGridView2.Columns[6].Visible = false;
                    dataGridView2.Columns[7].Visible = false;
                    dataGridView2.Columns[0].HeaderText = "Κωδικός Περιοχής";
                    dataGridView2.Columns[1].HeaderText = "Όνομα Περιοχής";
                    dataGridView2.Columns[2].HeaderText = "Ιδιοκτήτης";
                    dataGridView2.Columns[3].HeaderText = "Στρατός";
                    dataGridView2.Columns[4].HeaderText = "Αμυντική Ισχύς";
                    dataGridView2.Columns[5].HeaderText = "Επιθετική Ισχύς";
                    dataGridView2.Columns[6].HeaderText = "x";
                    dataGridView2.Columns[7].HeaderText = "y";
                    dataGridView2.Visible = true;
                    button2.Visible = true;
                }
                reader.Close();

                connection.Close();
            }
            else
            {
                MessageBox.Show("Επιλέξτε τον κωδικό της επίθεσης");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentCell != null && dataGridView2.CurrentCell.Value != "" && dataGridView2.CurrentCell.Value != null)
            {
                int selectedcolumnindex = dataGridView2.SelectedCells[0].ColumnIndex;
                if (selectedcolumnindex == 0)
                {
                    string text = dataGridView1.CurrentCell.Value.ToString();
                    string text1 = dataGridView2.CurrentCell.Value.ToString();

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
                    string req_region = reader["def_region"].ToString();
                    int turn = Convert.ToInt32(reader["turn"]);
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

                    reader.Close();

                    command.CommandText = "Select * from regions where name=@p11";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p11", req_region);
                    reader = command.ExecuteReader();
                    reader.Read();

                    int defence = Convert.ToInt32(reader["defence"]);

                    reader.Close();

                    command.CommandText = "Select * from player where username=@p12";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p12", att_player);
                    reader = command.ExecuteReader();
                    reader.Read();

                    decimal military = Convert.ToDecimal(reader["military"]);

                    reader.Close();

                    command.CommandText = "Select * from regions where id=@p9";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p9", text1);
                    reader = command.ExecuteReader();
                    reader.Read();

                    string supporter = reader["owner"].ToString();
                    string sup_region = reader["name"].ToString();

                    reader.Close();

                    int sup_army = Convert.ToInt32(Math.Round(((army + army_enwsi + army_sunasp + sum_co_army) * (military * Convert.ToDecimal(vars["off_fact"]) + 1)))) - defence;

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

                        command.CommandText = "insert into support (id,requestor,supporter,req_region,sup_region,turn,army) values (@p2, @p3, @p4, @p5, @p6, @p7, @p8)";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p2", i);
                        command.Parameters.AddWithValue("@p3", textBox1.Text);
                        command.Parameters.AddWithValue("@p4", supporter);
                        command.Parameters.AddWithValue("@p5", req_region);
                        command.Parameters.AddWithValue("@p6", sup_region);
                        command.Parameters.AddWithValue("@p7", turn);
                        command.Parameters.AddWithValue("@p8", sup_army);
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
            }
        }
    }
}
