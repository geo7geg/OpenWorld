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
    public partial class Form8 : Form
    {
        Dictionary<string, string> vars = Variables.vars();
        string sqlcon = Variables.sqlstring;

        public Form8(string reg_name)
        {
            InitializeComponent();
            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.None;
            textBox1.Visible = false;
            textBox1.Text = reg_name;

            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM regions where name=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", reg_name);
            reader = command.ExecuteReader();
            reader.Read();
            int army = Convert.ToInt32(reader["army"]);

            textBox3.Text = Convert.ToString(reader["farm"]);
            textBox4.Text = Convert.ToString(reader["craft"]);
            textBox5.Text = Convert.ToString(reader["dealer"]);
            textBox7.Text = Convert.ToString(reader["farmcon"]);
            textBox8.Text = Convert.ToString(reader["craftcon"]);
            textBox9.Text = Convert.ToString(reader["dealercon"]);
            textBox10.Text = Convert.ToString(reader["level"]);
            int cost = Convert.ToInt32(reader["cost"]);
            int revenue = Convert.ToInt32(reader["revenue"]);
            int gold = Convert.ToInt32(reader["gold"]);

            numericUpDown1.Maximum = Convert.ToInt32(textBox3.Text);
            numericUpDown2.Maximum = Convert.ToInt32(textBox4.Text);
            numericUpDown3.Maximum = Convert.ToInt32(textBox5.Text);
            numericUpDown1.Minimum = 0;
            numericUpDown2.Minimum = 0;
            numericUpDown3.Minimum = 0;

            string owner = Convert.ToString(reader["owner"]);
            textBox11.Text = owner;
            reader.Close();

            command.CommandText = "SELECT * FROM player where username='" + owner + "'";
            command.Prepare();
            reader = command.ExecuteReader();
            reader.Read();

            decimal military = Convert.ToDecimal(reader["military"]);

            textBox2.Text = Convert.ToString(military);
            textBox6.Text = Convert.ToString(army);

            reader.Close();
            connection.Close();

            showdatagrid();

            if((revenue - cost) < 0)
            {
                int remain_rounds = gold / (revenue - cost);
                MessageBox.Show("Το κόστος είναι μεγαλύτερο απο τα έσοδα . Σας απομένουν " + Math.Abs(remain_rounds) + " γύροι");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;
                command.CommandText = "SELECT * FROM regions where name=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", textBox1.Text);
                reader = command.ExecuteReader();
                reader.Read();

                string temp4 = reader["owner"].ToString();
                int def_id = Convert.ToInt32(reader["id"]);
                int farm = Convert.ToInt32(reader["farm"]);
                int craft = Convert.ToInt32(reader["craft"]);
                int dealer = Convert.ToInt32(reader["dealer"]);
                int farmcon = Convert.ToInt32(reader["farmcon"]);
                int craftcon = Convert.ToInt32(reader["craftcon"]);
                int dealercon = Convert.ToInt32(reader["dealercon"]);
                int level = Convert.ToInt32(reader["level"]);
                int gold = Convert.ToInt32(reader["gold"]);
                int army = Convert.ToInt32(reader["army"]);
                decimal def_fact = Convert.ToDecimal(reader["def_fact"]);

                reader.Close();


                if (Convert.ToInt32(numericUpDown1.Value) <= farm && Convert.ToInt32(numericUpDown2.Value) <= craft && Convert.ToInt32(numericUpDown3.Value) <= dealer)
                {
                    command.CommandText = "SELECT * FROM player where username=@p2";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p2", temp4);
                    reader = command.ExecuteReader();
                    reader.Read();

                    decimal military = Convert.ToDecimal(reader["military"]);
                    int free_army = Convert.ToInt32(reader["free_army"]);

                    reader.Close();

                    int farm_new = farm - Convert.ToInt32(numericUpDown1.Value);
                    int craft_new = craft - Convert.ToInt32(numericUpDown2.Value);
                    int dealer_new = dealer - Convert.ToInt32(numericUpDown3.Value);
                    int army_new = army + Convert.ToInt32(numericUpDown1.Value) + Convert.ToInt32(numericUpDown2.Value) + Convert.ToInt32(numericUpDown3.Value);
                    int offense = Convert.ToInt32(Math.Round(army_new * (military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                    int revenue = (farm_new * Convert.ToInt32(vars["farm_production"])) + (craft_new * Convert.ToInt32(vars["craft_production"])) + (dealer_new * Convert.ToInt32(vars["dealer_production"]));
                    decimal defence = Math.Round(def_fact * (farm_new + craft_new + dealer_new) + offense);
                    decimal pol_stab = 1;
                    int cost_bill = 0;

                    int farm_max = 0;
                    int craft_max = 0;
                    int dealer_max = 0;

                    if (level <= 1)
                    {
                        farm_max = Convert.ToInt32(farmcon);
                        craft_max = Convert.ToInt32(craftcon);
                        dealer_max = Convert.ToInt32(dealercon);
                    }
                    else
                    {
                        farm_max = Convert.ToInt32(farmcon + (farmcon * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                        craft_max = Convert.ToInt32(craftcon + (craftcon * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                        dealer_max = Convert.ToInt32(dealercon + (dealercon * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                    }

                    int cost = (farm_max + craft_max + dealer_max) - (farm_new + craft_new + craft_new);

                    if (farm_new < farm_max * 0.5 || craft_new < craft_max * 0.5 || dealer_new < dealer_max * 0.5)
                    {
                        decimal farmnew = farm_new;
                        decimal craftnew = craft_new;
                        decimal dealernew = dealer_new;
                        decimal farmcons = farm_max;
                        decimal craftcons = craft_max;
                        decimal dealercons = dealer_max;
                        pol_stab = (farmnew + craftnew + dealernew) / (farmcons + craftcons + dealercons);
                        if (pol_stab == 0)
                        {
                            cost_bill = Convert.ToInt32(revenue);
                        }
                        else
                        {
                            cost_bill = Convert.ToInt32(Math.Round((revenue / pol_stab) - revenue));
                        }

                        decimal farm1 = 0;
                        decimal craft1 = 0;
                        decimal dealer1 = 0;

                        int missing_population = 0;
                        if (level > 1)
                        {
                            farm1 = Convert.ToDecimal(farm_new - farmcon * Convert.ToDecimal(vars["levelup_fact"]));
                            craft1 = Convert.ToDecimal(craft_new - craftcon * Convert.ToDecimal(vars["levelup_fact"]));
                            dealer1 = Convert.ToDecimal(dealer_new - dealercon * Convert.ToDecimal(vars["levelup_fact"]));
                            if (farm1 < 0)
                            {
                                missing_population = missing_population + Math.Abs(Convert.ToInt32(farm1));
                                farm1 = 0;
                            }
                            if (craft1 < 0)
                            {
                                missing_population = missing_population + Math.Abs(Convert.ToInt32(craft1));
                                craft1 = 0;
                            }
                            if (dealer1 < 0)
                            {
                                missing_population = missing_population + Math.Abs(Convert.ToInt32(dealer1));
                                dealer1 = 0;
                            }
                            level = level - 1;
                            if (missing_population > 0)
                            {
                                army_new = Variables.level_fix(def_id, missing_population, army_new);
                            }
                        }
                        else
                        {
                            farm1 = farm_new;
                            craft1 = craft_new;
                            dealer1 = dealer_new;
                        }

                        offense = Convert.ToInt32(Math.Round(army_new * (military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                        decimal defence1 = Math.Round(def_fact * (farm1 + craft1 + dealer1) + offense);
                        decimal revenue1 = (farm1 * Convert.ToInt32(vars["farm_production"])) + (craft1 * Convert.ToInt32(vars["craft_production"])) + (dealer1 * Convert.ToInt32(vars["dealer_production"]));


                        command.CommandText = "UPDATE regions SET farm=@p3, craft=@p4, dealer=@p5, army=@p6, defence=@p7, level=@p8, revenue=@p9, cost=@p10, offense=@p11, pol_stab=@p12, cost_bill=@p13 Where name=@p1 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p3", Convert.ToInt32(farm1));
                        command.Parameters.AddWithValue("@p4", Convert.ToInt32(craft1));
                        command.Parameters.AddWithValue("@p5", Convert.ToInt32(dealer1));
                        command.Parameters.AddWithValue("@p6", army_new);
                        command.Parameters.AddWithValue("@p7", Convert.ToInt32(defence1));
                        command.Parameters.AddWithValue("@p8", level);
                        command.Parameters.AddWithValue("@p9", Convert.ToInt32(Math.Round(revenue1)));
                        command.Parameters.AddWithValue("@p10", army_new);
                        command.Parameters.AddWithValue("@p11", offense);
                        command.Parameters.AddWithValue("@p12", pol_stab);
                        command.Parameters.AddWithValue("@p13", cost_bill);
                        command.ExecuteNonQuery();

                        numericUpDown1.Maximum = Convert.ToInt32(farm1);
                        numericUpDown2.Maximum = Convert.ToInt32(craft1);
                        numericUpDown3.Maximum = Convert.ToInt32(dealer1);

                        textBox3.Text = Convert.ToString(farm1);
                        textBox4.Text = Convert.ToString(craft1);
                        textBox5.Text = Convert.ToString(dealer1);
                        textBox6.Text = Convert.ToString(army_new);
                        textBox10.Text = Convert.ToString(level);

                        MessageBox.Show("Το Επίπεδο Ανάπτυξης και η Πολιτική Σταθερότητα σας μειώθηκε");

                        Variables.calculate_cost(def_id);

                        if ((revenue - cost) < 0)
                        {
                            int remain_rounds = gold / (revenue - cost);
                            MessageBox.Show("Το κόστος είναι μεγαλύτερο απο τα έσοδα . Σας απομένουν " + Math.Abs(remain_rounds) + " γύροι");
                        }

                        if (System.Windows.Forms.Application.OpenForms["Form4"] != null)
                        {
                            (System.Windows.Forms.Application.OpenForms["Form4"] as Form4).filldatagrid();
                        }
                    }
                    else
                    {
                        command.CommandText = "UPDATE regions SET farm=@p3, craft=@p4, dealer=@p5, army=@p6, defence=@p7, revenue=@p8, cost=@p9, offense=@p10, pol_stab=@p11, cost_bill=@p12 Where name=@p1 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p3", farm_new);
                        command.Parameters.AddWithValue("@p4", craft_new);
                        command.Parameters.AddWithValue("@p5", dealer_new);
                        command.Parameters.AddWithValue("@p6", army_new);
                        command.Parameters.AddWithValue("@p7", defence);
                        command.Parameters.AddWithValue("@p8", revenue);
                        command.Parameters.AddWithValue("@p9", army_new);
                        command.Parameters.AddWithValue("@p10", offense);
                        command.Parameters.AddWithValue("@p11", pol_stab);
                        command.Parameters.AddWithValue("@p12", cost_bill);
                        command.ExecuteNonQuery();
                        connection.Close();

                        numericUpDown1.Maximum = Convert.ToInt32(farm_new);
                        numericUpDown2.Maximum = Convert.ToInt32(craft_new);
                        numericUpDown3.Maximum = Convert.ToInt32(dealer_new);

                        textBox3.Text = Convert.ToString(farm_new);
                        textBox4.Text = Convert.ToString(craft_new);
                        textBox5.Text = Convert.ToString(dealer_new);
                        textBox6.Text = Convert.ToString(army_new);

                        MessageBox.Show("Επιτυχής Επιστράτευση");

                        Variables.calculate_cost(def_id);

                        if ((revenue - cost) < 0)
                        {
                            int remain_rounds = gold / (revenue - cost);
                            MessageBox.Show("Το κόστος είναι μεγαλύτερο απο τα έσοδα . Σας απομένουν " + Math.Abs(remain_rounds) + " γύροι");
                        }

                        if (System.Windows.Forms.Application.OpenForms["Form4"] != null)
                        {
                            (System.Windows.Forms.Application.OpenForms["Form4"] as Form4).filldatagrid();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Δεν επιτρέπεται η επιστράτευση παραπάνω προσωπικού απο το κανονικό.");
                }

                showdatagrid();
            }
            else
            {
                MessageBox.Show("Για να επιστρατεύσετε στρατιώτες ξετσεκάρετε το κουτάκι της αποστράτευσης");
            }
        }

        public void showdatagrid()
        {
            string str = sqlcon;
            MySqlConnection con = new MySqlConnection(str);

            string com = "Select farmcon,craftcon,dealercon,farm,craft,dealer,army,defence,level,revenue,cost,offense,pol_stab,gold,cost_bill from regions where name='" + textBox1.Text + "'";
            MySqlDataAdapter adpt = new MySqlDataAdapter(com, con);
            DataSet myDataSet = new DataSet();
            adpt.Fill(myDataSet, "regions");
            DataTable myDataTable = myDataSet.Tables[0];
            DataRow tempRow = null;

            foreach (DataRow tempRow_Variable in myDataTable.Rows)
            {
                tempRow = tempRow_Variable;
                int cost_bill = Convert.ToInt32(tempRow["cost_bill"]);

                if (cost_bill > 0)
                {
                    pictureBox6.Visible = true;
                }
            }
            DataTable dtClone = myDataTable.Clone();
            dtClone.Columns[0].DataType = typeof(string);
            dtClone.Columns[1].DataType = typeof(string);
            dtClone.Columns[2].DataType = typeof(string);
            foreach (DataRow row in myDataTable.Rows)
            {
                dtClone.ImportRow(row);
            }

            int farm_max = 0;
            int craft_max = 0;
            int dealer_max = 0;

            DataRow tempRow1 = null;
            foreach (DataRow tempRow1_Variable in dtClone.Rows)
            {
                tempRow1 = tempRow1_Variable;

                int level = Convert.ToInt32(tempRow1["level"]);
                int farmcon = Convert.ToInt32(tempRow1["farmcon"]);
                int craftcon = Convert.ToInt32(tempRow1["craftcon"]);
                int dealercon = Convert.ToInt32(tempRow1["dealercon"]);

                if (level <= 1)
                {
                    farm_max = Convert.ToInt32(farmcon);
                    craft_max = Convert.ToInt32(craftcon);
                    dealer_max = Convert.ToInt32(dealercon);
                }
                else
                {
                    farm_max = Convert.ToInt32(farmcon + (farmcon * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                    craft_max = Convert.ToInt32(craftcon + (craftcon * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                    dealer_max = Convert.ToInt32(dealercon + (dealercon * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                } 
            }
            
            dtClone.Rows[0][0] = dtClone.Rows[0][3].ToString() + "/" + Convert.ToString(farm_max);
            dtClone.Rows[0][1] = dtClone.Rows[0][4].ToString() + "/" + Convert.ToString(craft_max);
            dtClone.Rows[0][2] = dtClone.Rows[0][5].ToString() + "/" + Convert.ToString(dealer_max);

            dtClone.Columns.RemoveAt(3);
            dtClone.Columns.RemoveAt(3);
            dtClone.Columns.RemoveAt(3);

            myDataSet.Tables.Clear();
            myDataSet.Tables.Add(dtClone);

            label1.Text = textBox1.Text;
            label1.Text = label1.Text.ToUpper();

            dataGridView1.DataSource = myDataSet;
            dataGridView1.DataMember = "regions";
            dataGridView1.Columns[0].HeaderText = "Αγρότες";
            dataGridView1.Columns[1].HeaderText = "Τεχνίτες";
            dataGridView1.Columns[2].HeaderText = "Έμποροι";
            dataGridView1.Columns[3].HeaderText = "Αριθμός Στρατού";
            dataGridView1.Columns[4].HeaderText = "Άμυνα Περιοχής";
            dataGridView1.Columns[5].HeaderText = "Επίπεδο Ανάπτυξης";
            dataGridView1.Columns[6].HeaderText = "Έσοδα";
            dataGridView1.Columns[7].HeaderText = "Έξοδα";
            dataGridView1.Columns[8].HeaderText = "Επίθεση Περιοχής";
            dataGridView1.Columns[9].HeaderText = "Πολιτική Σταθερότητα";
            dataGridView1.Columns[10].HeaderText = "Χρυσός";
            dataGridView1.Columns[11].HeaderText = "Κόστος Ανάκτησης";
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;
                command.CommandText = "SELECT * FROM regions where name=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", textBox1.Text);
                reader = command.ExecuteReader();
                reader.Read();

                string temp = reader["owner"].ToString();
                int id = Convert.ToInt32(reader["id"]);
                int farmcon = Convert.ToInt32(reader["farmcon"]);
                int craftcon = Convert.ToInt32(reader["craftcon"]);
                int dealercon = Convert.ToInt32(reader["dealercon"]);
                int army = Convert.ToInt32(reader["army"]);
                int farm = Convert.ToInt32(reader["farm"]);
                int craft = Convert.ToInt32(reader["craft"]);
                int dealer = Convert.ToInt32(reader["dealer"]);
                int level = Convert.ToInt32(reader["level"]);
                decimal def_fact = Convert.ToDecimal(reader["def_fact"]);

                reader.Close();

                int farm_max = 0;
                int craft_max = 0;
                int dealer_max = 0;

                if (level <= 1)
                {
                    farm_max = Convert.ToInt32(farmcon);
                    craft_max = Convert.ToInt32(craftcon);
                    dealer_max = Convert.ToInt32(dealercon);
                }
                else
                {
                    farm_max = Convert.ToInt32(farmcon + (farmcon * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                    craft_max = Convert.ToInt32(craftcon + (craftcon * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                    dealer_max = Convert.ToInt32(dealercon + (dealercon * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                }

                
                    if ((Convert.ToInt32(numericUpDown1.Value) + Convert.ToInt32(numericUpDown2.Value) + Convert.ToInt32(numericUpDown3.Value)) <= army)
                    {
                        if (((Convert.ToInt32(numericUpDown1.Value) + farm) <= farm_max) && ((Convert.ToInt32(numericUpDown2.Value) + craft) <= craft_max) && ((Convert.ToInt32(numericUpDown3.Value) + dealer) <= dealer_max))
                        {
                            command.CommandText = "SELECT * FROM player where username=@p2";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p2", temp);
                            reader = command.ExecuteReader();
                            reader.Read();

                            decimal military = Convert.ToDecimal(reader["military"]);

                            reader.Close();

                            int farm_new = farm + Convert.ToInt32(numericUpDown1.Value);
                            int craft_new = craft + Convert.ToInt32(numericUpDown2.Value);
                            int dealer_new = dealer + Convert.ToInt32(numericUpDown3.Value);
                            int army_new = army - (Convert.ToInt32(numericUpDown1.Value) + Convert.ToInt32(numericUpDown2.Value) + Convert.ToInt32(numericUpDown3.Value));
                            int offense = Convert.ToInt32(Math.Round(army_new * (military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                            int revenue = (farm_new * Convert.ToInt32(vars["farm_production"])) + (craft_new * Convert.ToInt32(vars["craft_production"])) + (dealer_new * Convert.ToInt32(vars["dealer_production"]));
                            decimal defence = Math.Round(def_fact * (farm_new + craft_new + dealer_new) + offense);


                            command.CommandText = "UPDATE regions SET farm=@p3, craft=@p4, dealer=@p5, army=@p6, defence=@p7, revenue=@p8, cost=@p9, offense=@p10 Where name=@p1 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p3", farm_new);
                            command.Parameters.AddWithValue("@p4", craft_new);
                            command.Parameters.AddWithValue("@p5", dealer_new);
                            command.Parameters.AddWithValue("@p6", army_new);
                            command.Parameters.AddWithValue("@p7", Convert.ToInt32(Math.Round(defence)));
                            command.Parameters.AddWithValue("@p8", revenue);
                            command.Parameters.AddWithValue("@p9", army_new);
                            command.Parameters.AddWithValue("@p10", offense);
                            command.ExecuteNonQuery();
                            connection.Close();

                            numericUpDown1.Maximum = Convert.ToInt32(farm_new);
                            numericUpDown2.Maximum = Convert.ToInt32(craft_new);
                            numericUpDown3.Maximum = Convert.ToInt32(dealer_new);

                            textBox3.Text = Convert.ToString(farm_new);
                            textBox4.Text = Convert.ToString(craft_new);
                            textBox5.Text = Convert.ToString(dealer_new);
                            textBox6.Text = Convert.ToString(army_new);

                            MessageBox.Show("Επιτυχής Αποστράτευση");

                            Variables.calculate_cost(id);

                            showdatagrid();

                            if (System.Windows.Forms.Application.OpenForms["Form4"] != null)
                            {
                                (System.Windows.Forms.Application.OpenForms["Form4"] as Form4).filldatagrid();
                            }
                            
                        }
                        else
                        {
                            MessageBox.Show("Δεν επιτρέπεται η αποστράτευση παραπάνω στρατιωτών από το σταθερό αριθμό κάθε ομάδας.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Δεν επιτρέπεται η αποστράτευση παραπάνω στρατιωτών από όσους έχετε διαθέσιμους στην περιοχή.");
                    }
                
            
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM regions where name=@p5";
            command.Prepare();
            command.Parameters.AddWithValue("@p5", label1.Text);
            reader = command.ExecuteReader();
            reader.Read();

            int cost_bill = Convert.ToInt32(reader["cost_bill"]);
            int reg_id = Convert.ToInt32(reader["id"]);
            int revenue = Convert.ToInt32(reader["revenue"]);
            decimal pol_stab = Convert.ToDecimal(reader["pol_stab"]);
            int gold = Convert.ToInt32(reader["gold"]);

            reader.Close();

            if (gold >= cost_bill)
            {
                gold = gold - cost_bill;
                cost_bill = 0;
                pol_stab = 1;

                command.CommandText = "UPDATE regions SET pol_stab=@p1, gold=@p2, cost_bill=@p3 Where name=@p4 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", pol_stab);
                command.Parameters.AddWithValue("@p2", gold);
                command.Parameters.AddWithValue("@p3", cost_bill);
                command.Parameters.AddWithValue("@p4", label1.Text);
                command.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Το κόστος επαναφοράς πληρώθηκε και η πολιτική σταθερότητα επανήλθε.");
                pictureBox6.Visible = false;
                showdatagrid();
            }
            else
            {
                MessageBox.Show("Ο χρυσός δεν φτάνει για να πληρώσετε το κόστος επαναφοράς της πολιτικής σταθερότητας.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;
            command.CommandText = "SELECT * FROM regions where name=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", textBox1.Text);
            reader = command.ExecuteReader();
            reader.Read();

            int level =Convert.ToInt32(reader["level"]);
            int gold = Convert.ToInt32(reader["gold"]);
   
            int farm = Convert.ToInt32(reader["farm"]);
            int craft = Convert.ToInt32(reader["craft"]);
            int dealer = Convert.ToInt32(reader["dealer"]);
            int farmcon = Convert.ToInt32(reader["farmcon"]);
            int craftcon = Convert.ToInt32(reader["craftcon"]);
            int dealercon = Convert.ToInt32(reader["dealercon"]);
            int offense = Convert.ToInt32(reader["offense"]);
            decimal pol_stab = Convert.ToDecimal(reader["pol_stab"]);
            decimal def_fact = Convert.ToDecimal(reader["def_fact"]);

            reader.Close();

            int gold_bill = level * Convert.ToInt32(vars["levelup_cost"]);
            int farm_new = Convert.ToInt32(farm + farmcon * Convert.ToDecimal(vars["farm_production"]));
            int craft_new = Convert.ToInt32(craft + craftcon * Convert.ToDecimal(vars["craft_production"]));
            int dealer_new = Convert.ToInt32(dealer + dealercon * Convert.ToDecimal(vars["dealer_production"]));
            decimal defence = Math.Round(def_fact * (farm_new + craft_new + dealer_new) + offense);
            //decimal pol_stab_check = Convert.ToDecimal(1.00);

            if (pol_stab == 1)
            {
                if (gold > gold_bill)
                {
                    command.CommandText = "UPDATE regions SET farm=@p2, craft=@p3, dealer=@p4, defence=@p8, level=@p5, revenue=@p6, gold=@p7 Where name=@p1 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p2", farm_new);
                    command.Parameters.AddWithValue("@p3", craft_new);
                    command.Parameters.AddWithValue("@p4", dealer_new);
                    command.Parameters.AddWithValue("@p5", level + 1);
                    command.Parameters.AddWithValue("@p6", (farm_new * Convert.ToInt32(vars["farm_production"])) + (craft_new * Convert.ToInt32(vars["craft_production"])) + (dealer_new * Convert.ToInt32(vars["dealer_production"])));
                    command.Parameters.AddWithValue("@p7", gold - gold_bill);
                    command.Parameters.AddWithValue("@p8", Convert.ToInt32(Math.Round(defence)));
                    command.ExecuteNonQuery();

                    numericUpDown1.Maximum = Convert.ToInt32(farm_new);
                    numericUpDown2.Maximum = Convert.ToInt32(craft_new);
                    numericUpDown3.Maximum = Convert.ToInt32(dealer_new);

                    textBox3.Text = Convert.ToString(farm_new);
                    textBox4.Text = Convert.ToString(craft_new);
                    textBox5.Text = Convert.ToString(dealer_new);
                    textBox10.Text = Convert.ToString(level + 1);

                    MessageBox.Show("Το Επίπεδο Ανάπτυξης σας αυξήθηκε");
                    showdatagrid();

                    if (System.Windows.Forms.Application.OpenForms["Form4"] != null)
                    {
                        (System.Windows.Forms.Application.OpenForms["Form4"] as Form4).filldatagrid();
                    }
                }
                else
                {
                    MessageBox.Show("Ο χρυσός δεν επαρκεί για την αύξηση του επιπέδου ανάπτυξης");
                } 
            }
            else
            {
                MessageBox.Show("Η Πολιτική σας Σταθερότητα είναι κάτω του 1. Δεν επιτρέπεται αύξηση του Επιπέδου");
            } 
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                int farm_max = 0;
                int craft_max = 0;
                int dealer_max = 0;
                if (Convert.ToInt32(textBox10.Text) <= 1)
                {
                    farm_max = Convert.ToInt32(textBox7.Text);
                    craft_max = Convert.ToInt32(textBox8.Text);
                    dealer_max = Convert.ToInt32(textBox9.Text);
                }
                else
                {
                    farm_max = Convert.ToInt32(Convert.ToInt32(textBox7.Text) + (Convert.ToInt32(textBox7.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                    craft_max = Convert.ToInt32(Convert.ToInt32(textBox8.Text) + (Convert.ToInt32(textBox8.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                    dealer_max = Convert.ToInt32(Convert.ToInt32(textBox9.Text) + (Convert.ToInt32(textBox9.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                }
                
                numericUpDown1.Maximum = (farm_max - Convert.ToInt32(textBox3.Text));
                numericUpDown2.Maximum = (craft_max - Convert.ToInt32(textBox4.Text));
                numericUpDown3.Maximum = (dealer_max - Convert.ToInt32(textBox5.Text));

                int attack = Convert.ToInt32(Math.Round((Convert.ToDecimal(textBox2.Text) * Convert.ToDecimal(vars["off_fact"]) + 1) * (Convert.ToInt32(textBox6.Text) - (numericUpDown1.Value + numericUpDown2.Value + numericUpDown3.Value))));
                label2.Text = Convert.ToString(attack);
                label2.Visible = true;
                label3.Visible = true; 
            }
            else
            {
                numericUpDown1.Maximum = Convert.ToInt32(textBox3.Text);
                numericUpDown2.Maximum = Convert.ToInt32(textBox4.Text);
                numericUpDown3.Maximum = Convert.ToInt32(textBox5.Text);
                int attack = Convert.ToInt32(Math.Round((Convert.ToDecimal(textBox2.Text) * Convert.ToDecimal(vars["off_fact"]) + 1) * ((numericUpDown1.Value + numericUpDown2.Value + numericUpDown3.Value) + Convert.ToInt32(textBox6.Text))));

                int farm_new = Convert.ToInt32(textBox3.Text) - Convert.ToInt32(numericUpDown1.Value);
                int craft_new = Convert.ToInt32(textBox4.Text) - Convert.ToInt32(numericUpDown2.Value);
                int dealer_new = Convert.ToInt32(textBox5.Text) - Convert.ToInt32(numericUpDown3.Value);
                int farm_max = 0;
                int craft_max = 0;
                int dealer_max = 0;

                if (Convert.ToInt32(textBox10.Text) <= 1)
                {
                    farm_max = Convert.ToInt32(textBox7.Text);
                    craft_max = Convert.ToInt32(textBox8.Text);
                    dealer_max = Convert.ToInt32(textBox9.Text);
                }
                else
                {
                    farm_max = Convert.ToInt32(Convert.ToInt32(textBox7.Text) + (Convert.ToInt32(textBox7.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                    craft_max = Convert.ToInt32(Convert.ToInt32(textBox8.Text) + (Convert.ToInt32(textBox8.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                    dealer_max = Convert.ToInt32(Convert.ToInt32(textBox9.Text) + (Convert.ToInt32(textBox9.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                }

                if (farm_new < farm_max * 0.5 || craft_new < craft_max * 0.5 || dealer_new < dealer_max * 0.5)
                {
                    decimal farmnew = farm_new;
                    decimal craftnew = craft_new;
                    decimal dealernew = dealer_new;
                    decimal farmcons = farm_max;
                    decimal craftcons = craft_max;
                    decimal dealercons = dealer_max;
                    decimal pol_stab = (farmnew + craftnew + dealernew) / (farmcons + craftcons + dealercons);
                    label4.Visible = true;
                    label5.Visible = true;
                    label5.Text = Convert.ToString(decimal.Round(pol_stab, 2));
                }
                else
                {
                    label4.Visible = false;
                    label5.Visible = false;
                }
                label2.Text = Convert.ToString(attack);
                label2.Visible = true;
                label3.Visible = true; 
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                int farm_max = 0;
                int craft_max = 0;
                int dealer_max = 0;
                if (Convert.ToInt32(textBox10.Text) <= 1)
                {
                    farm_max = Convert.ToInt32(textBox7.Text);
                    craft_max = Convert.ToInt32(textBox8.Text);
                    dealer_max = Convert.ToInt32(textBox9.Text);
                }
                else
                {
                    farm_max = Convert.ToInt32(Convert.ToInt32(textBox7.Text) + (Convert.ToInt32(textBox7.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                    craft_max = Convert.ToInt32(Convert.ToInt32(textBox8.Text) + (Convert.ToInt32(textBox8.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                    dealer_max = Convert.ToInt32(Convert.ToInt32(textBox9.Text) + (Convert.ToInt32(textBox9.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                }

                numericUpDown1.Maximum = (farm_max - Convert.ToInt32(textBox3.Text));
                numericUpDown2.Maximum = (craft_max - Convert.ToInt32(textBox4.Text));
                numericUpDown3.Maximum = (dealer_max - Convert.ToInt32(textBox5.Text));

                int attack = Convert.ToInt32(Math.Round((Convert.ToDecimal(textBox2.Text) * Convert.ToDecimal(vars["off_fact"]) + 1) * (Convert.ToInt32(textBox6.Text) - (numericUpDown1.Value + numericUpDown2.Value + numericUpDown3.Value))));
                label2.Text = Convert.ToString(attack);
                label2.Visible = true;
                label3.Visible = true; 
            }
            else
            {
                numericUpDown1.Maximum = Convert.ToInt32(textBox3.Text);
                numericUpDown2.Maximum = Convert.ToInt32(textBox4.Text);
                numericUpDown3.Maximum = Convert.ToInt32(textBox5.Text);
                int attack = Convert.ToInt32(Math.Round((Convert.ToDecimal(textBox2.Text) * Convert.ToDecimal(vars["off_fact"]) + 1) * ((numericUpDown1.Value + numericUpDown2.Value + numericUpDown3.Value) + Convert.ToInt32(textBox6.Text))));

                int farm_new = Convert.ToInt32(textBox3.Text) - Convert.ToInt32(numericUpDown1.Value);
                int craft_new = Convert.ToInt32(textBox4.Text) - Convert.ToInt32(numericUpDown2.Value);
                int dealer_new = Convert.ToInt32(textBox5.Text) - Convert.ToInt32(numericUpDown3.Value);
                int farm_max = 0;
                int craft_max = 0;
                int dealer_max = 0;

                if (Convert.ToInt32(textBox10.Text) <= 1)
                {
                    farm_max = Convert.ToInt32(textBox7.Text);
                    craft_max = Convert.ToInt32(textBox8.Text);
                    dealer_max = Convert.ToInt32(textBox9.Text);
                }
                else
                {
                    farm_max = Convert.ToInt32(Convert.ToInt32(textBox7.Text) + (Convert.ToInt32(textBox7.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                    craft_max = Convert.ToInt32(Convert.ToInt32(textBox8.Text) + (Convert.ToInt32(textBox8.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                    dealer_max = Convert.ToInt32(Convert.ToInt32(textBox9.Text) + (Convert.ToInt32(textBox9.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                }

                if (farm_new < farm_max * 0.5 || craft_new < craft_max * 0.5 || dealer_new < dealer_max * 0.5)
                {
                    decimal farmnew = farm_new;
                    decimal craftnew = craft_new;
                    decimal dealernew = dealer_new;
                    decimal farmcons = farm_max;
                    decimal craftcons = craft_max;
                    decimal dealercons = dealer_max;
                    decimal pol_stab = (farmnew + craftnew + dealernew) / (farmcons + craftcons + dealercons);
                    label4.Visible = true;
                    label5.Visible = true;
                    label5.Text = Convert.ToString(decimal.Round(pol_stab, 2));
                }
                else
                {
                    label4.Visible = false;
                    label5.Visible = false;
                }

                label2.Text = Convert.ToString(attack);
                label2.Visible = true;
                label3.Visible = true; 
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                int farm_max = 0;
                int craft_max = 0;
                int dealer_max = 0;
                if (Convert.ToInt32(textBox10.Text) <= 1)
                {
                    farm_max = Convert.ToInt32(textBox7.Text);
                    craft_max = Convert.ToInt32(textBox8.Text);
                    dealer_max = Convert.ToInt32(textBox9.Text);
                }
                else
                {
                    farm_max = Convert.ToInt32(Convert.ToInt32(textBox7.Text) + (Convert.ToInt32(textBox7.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                    craft_max = Convert.ToInt32(Convert.ToInt32(textBox8.Text) + (Convert.ToInt32(textBox8.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                    dealer_max = Convert.ToInt32(Convert.ToInt32(textBox9.Text) + (Convert.ToInt32(textBox9.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                }

                numericUpDown1.Maximum = (farm_max - Convert.ToInt32(textBox3.Text));
                numericUpDown2.Maximum = (craft_max - Convert.ToInt32(textBox4.Text));
                numericUpDown3.Maximum = (dealer_max - Convert.ToInt32(textBox5.Text));

                //numericUpDown1.Maximum = Convert.ToInt32(textBox6.Text) - (numericUpDown2.Value + numericUpDown3.Value);
                //numericUpDown2.Maximum = Convert.ToInt32(textBox6.Text) - (numericUpDown1.Value + numericUpDown3.Value);
                //numericUpDown3.Maximum = Convert.ToInt32(textBox6.Text) - (numericUpDown2.Value + numericUpDown1.Value);
                int attack = Convert.ToInt32(Math.Round((Convert.ToDecimal(textBox2.Text) * Convert.ToDecimal(vars["off_fact"]) + 1) * (Convert.ToInt32(textBox6.Text) - (numericUpDown1.Value + numericUpDown2.Value + numericUpDown3.Value))));
                label2.Text = Convert.ToString(attack);
                label2.Visible = true;
                label3.Visible = true; 
            }
            else
            {
                numericUpDown1.Maximum = Convert.ToInt32(textBox3.Text);
                numericUpDown2.Maximum = Convert.ToInt32(textBox4.Text);
                numericUpDown3.Maximum = Convert.ToInt32(textBox5.Text);
                int attack = Convert.ToInt32(Math.Round((Convert.ToDecimal(textBox2.Text) * Convert.ToDecimal(vars["off_fact"]) + 1) * ((numericUpDown1.Value + numericUpDown2.Value + numericUpDown3.Value) + Convert.ToInt32(textBox6.Text))));

                int farm_new = Convert.ToInt32(textBox3.Text) - Convert.ToInt32(numericUpDown1.Value);
                int craft_new = Convert.ToInt32(textBox4.Text) - Convert.ToInt32(numericUpDown2.Value);
                int dealer_new = Convert.ToInt32(textBox5.Text) - Convert.ToInt32(numericUpDown3.Value);
                int farm_max = 0;
                int craft_max = 0;
                int dealer_max = 0;

                if (Convert.ToInt32(textBox10.Text) <= 1)
                {
                    farm_max = Convert.ToInt32(textBox7.Text);
                    craft_max = Convert.ToInt32(textBox8.Text);
                    dealer_max = Convert.ToInt32(textBox9.Text);
                }
                else
                {
                    farm_max = Convert.ToInt32(Convert.ToInt32(textBox7.Text) + (Convert.ToInt32(textBox7.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                    craft_max = Convert.ToInt32(Convert.ToInt32(textBox8.Text) + (Convert.ToInt32(textBox8.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                    dealer_max = Convert.ToInt32(Convert.ToInt32(textBox9.Text) + (Convert.ToInt32(textBox9.Text) * Convert.ToDecimal(vars["levelup_fact"])) * (Convert.ToInt32(textBox10.Text) - 1));
                }

                if (farm_new < farm_max * 0.5 || craft_new < craft_max * 0.5 || dealer_new < dealer_max * 0.5)
                {
                    decimal farmnew = farm_new;
                    decimal craftnew = craft_new;
                    decimal dealernew = dealer_new;
                    decimal farmcons = farm_max;
                    decimal craftcons = craft_max;
                    decimal dealercons = dealer_max;
                    decimal pol_stab = (farmnew + craftnew + dealernew) / (farmcons + craftcons + dealercons);
                    label4.Visible = true;
                    label5.Visible = true;
                    label5.Text = Convert.ToString(decimal.Round(pol_stab,2));
                }
                else
                {
                    label4.Visible = false;
                    label5.Visible = false;
                }

                label2.Text = Convert.ToString(attack);
                label2.Visible = true;
                label3.Visible = true; 
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                label2.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                label5.Visible = false; 
                label2.ForeColor = Color.DarkRed;

                numericUpDown1.Maximum = Convert.ToInt32(textBox6.Text) - (numericUpDown2.Value + numericUpDown3.Value);
                numericUpDown2.Maximum = Convert.ToInt32(textBox6.Text) - (numericUpDown1.Value + numericUpDown3.Value);
                numericUpDown3.Maximum = Convert.ToInt32(textBox6.Text) - (numericUpDown2.Value + numericUpDown1.Value);
            }
            else
            {
                //label2.Visible = false;
                //label3.Visible = false;
                label2.ForeColor = Color.ForestGreen;
                numericUpDown1.Maximum = Convert.ToInt32(textBox3.Text);
                numericUpDown2.Maximum = Convert.ToInt32(textBox4.Text);
                numericUpDown3.Maximum = Convert.ToInt32(textBox5.Text);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM regions where name=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", textBox1.Text);
            reader = command.ExecuteReader();
            reader.Read();

            int gold = Convert.ToInt32(reader["gold"]);

            reader.Close();

            command.CommandText = "SELECT * FROM player where username=@p2";
            command.Prepare();
            command.Parameters.AddWithValue("@p2", textBox11.Text);
            reader = command.ExecuteReader();
            reader.Read();

            int bank = Convert.ToInt32(reader["gold"]);

            reader.Close();


            if (gold >= numericUpDown4.Value)
            {
                command.CommandText = "UPDATE regions SET gold=@p11 Where name=@p16 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p11", gold - numericUpDown4.Value);
                command.Parameters.AddWithValue("@p16", textBox1.Text);
                command.ExecuteNonQuery();

                command.CommandText = "UPDATE player SET gold=@p5 Where username=@p6 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p5", bank + numericUpDown4.Value);
                command.Parameters.AddWithValue("@p6", textBox11.Text);
                command.ExecuteNonQuery();

                MessageBox.Show("Αυξήθηκε ο χρυσός της Τράπεζας κατά " + numericUpDown4.Value);

                showdatagrid();

                if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
                {
                    (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).PlayerGold();
                }
                if (System.Windows.Forms.Application.OpenForms["Form4"] != null)
                {
                    (System.Windows.Forms.Application.OpenForms["Form4"] as Form4).filldatagrid();
                }
            }
            else
            {
                MessageBox.Show("Η περιοχή δεν διαθέτει τόσα χρήματα");
            }

            connection.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM regions where name=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", textBox1.Text);
            reader = command.ExecuteReader();
            reader.Read();

            int id = Convert.ToInt32(reader["id"]);
            int army = Convert.ToInt32(reader["army"]);
            string owner = reader["owner"].ToString();
            int farm = Convert.ToInt32(reader["farm"]);
            int craft = Convert.ToInt32(reader["craft"]);
            int dealer = Convert.ToInt32(reader["dealer"]);
            decimal def_fact = Convert.ToDecimal(reader["def_fact"]);

            reader.Close();

            command.CommandText = "SELECT * FROM player where username=@p2";
            command.Prepare();
            command.Parameters.AddWithValue("@p2", textBox11.Text);
            reader = command.ExecuteReader();
            reader.Read();

            int free_army = Convert.ToInt32(reader["free_army"]);
            decimal military = Convert.ToDecimal(reader["military"]);

            reader.Close();


            if (army >= numericUpDown5.Value)
            {
                int offense = Convert.ToInt32(Math.Round((army - numericUpDown5.Value) * (military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                decimal defence = Math.Round(def_fact * (farm + craft + dealer) + offense);

                command.CommandText = "UPDATE regions SET army=@p11, defence=@p12, cost=@p13, offense=@p14 Where name=@p16 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p11", army - numericUpDown5.Value);
                command.Parameters.AddWithValue("@p12", Convert.ToInt32(Math.Round(defence)));
                command.Parameters.AddWithValue("@p13", army - numericUpDown5.Value);
                command.Parameters.AddWithValue("@p14", offense);
                command.Parameters.AddWithValue("@p16", textBox1.Text);
                command.ExecuteNonQuery();

                command.CommandText = "UPDATE player SET free_army=@p5 Where username=@p6 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p5", free_army + numericUpDown5.Value);
                command.Parameters.AddWithValue("@p6", owner);
                command.ExecuteNonQuery();

                MessageBox.Show("Αυξήθηκε ο ελεύθερος στρατός κατά " + numericUpDown5.Value);

                Variables.calculate_cost(id);

                showdatagrid();

                if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
                {
                    (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).PlayerGold();
                }
                if (System.Windows.Forms.Application.OpenForms["Form4"] != null)
                {
                    (System.Windows.Forms.Application.OpenForms["Form4"] as Form4).filldatagrid();
                }
            }
            else
            {
                MessageBox.Show("Η περιοχή δεν διαθέτει τόσους στρατιώτες");
            }

            connection.Close();
        }
    }
}
