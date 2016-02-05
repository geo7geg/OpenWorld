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
using System.IO;
using System.Windows;
using System.Windows.Input;


namespace StrategyGame
{
    class Variables
    {
        public static string sqlstringtext()
        {
            string appPath = Path.GetDirectoryName(Application.ExecutablePath) + @"\sql.txt";
            //string appPath = @"c:\Users\" + Environment.UserName + @"\Desktop\sql.txt";
            List<string> lines = new List<string>();
            
            using (StreamReader r = new StreamReader(appPath, Encoding.Default))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            string sqltext = "";
            foreach (string s in lines)
            {
                //string[] words = s;

                sqltext = s.Trim();

                //words[0] = "";
                //words[1] = "";
            }

            return sqltext;
        }
        public static string sqlstring = sqlstringtext();
        //public static decimal factor = Convert.ToDecimal(0.1);
        //public static double def_fact = 0.3;
        //"server=localhost;user id=root;Password=;database=mapgame1;persist security info=False"
        //"server=94.70.246.130;database=mapgame1;uid=gameuser;Password=123456;"
        //"server=192.168.1.230;database=mapgame1;uid=gameuser;Password=123456;"
        
        public static void stop_attack(int id)
        {
            MySqlConnection connection = new MySqlConnection(sqlstring);
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;
            connection.Open();

            command.CommandText = "SELECT * FROM attacks where id=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", id);
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
            decimal def_fact = Convert.ToDecimal(reader["def_fact"]);
            reader.Close();

            command.CommandText = "SELECT * FROM player where username=@p22";
            command.Prepare();
            command.Parameters.AddWithValue("@p22", att_player);
            reader = command.ExecuteReader();
            reader.Read();

            decimal att_military = Convert.ToDecimal(reader["military"]);

            reader.Close();

            command.CommandText = "SELECT * FROM variables";
            command.Prepare();
            reader = command.ExecuteReader();
            reader.Read();

            decimal factor = Convert.ToDecimal(reader["off_fact"]);

            reader.Close();

            int att_offense1 = Convert.ToInt32(Math.Round((army + att_army) * (att_military * factor + 1)));
            decimal defence = Math.Round(def_fact * (farm + craft + dealer) + att_offense1);
            command.CommandText = "UPDATE regions SET army=@p36, defence=@p37, cost=@p38, offense=@p39 Where name=@p33 ";
            command.Prepare();
            command.Parameters.AddWithValue("@p36", army + att_army);
            command.Parameters.AddWithValue("@p37", Convert.ToInt32(defence));
            command.Parameters.AddWithValue("@p38", army + att_army);
            command.Parameters.AddWithValue("@p39", att_offense1);
            command.Parameters.AddWithValue("@p33", att_region);
            command.ExecuteNonQuery();

            command.CommandText = "DELETE FROM attacks Where id='" + id + "'";
            command.Prepare();
            command.ExecuteNonQuery();
        }

        public static void remove_army(string name, int missing)
        {
            MySqlConnection connection = new MySqlConnection(sqlstring);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM player where username='" + name + "'";
            command.Prepare();
            reader = command.ExecuteReader();

            decimal def_military = 0;
            if (reader.Read())
            {
                def_military = Convert.ToDecimal(reader["military"]);
            }

            reader.Close();

            int missing_con = missing;

            int sum_army = 0;
            MySqlCommand command3 = new MySqlCommand("SELECT SUM(army) FROM regions where owner='" + name + "'", connection);
            reader = command3.ExecuteReader();
            if (reader.Read())
            {
                if (reader[0] != System.DBNull.Value)
                {
                    sum_army = Convert.ToInt32(reader[0]);
                }
            }
            reader.Close();

            command.CommandText = "SELECT * FROM variables";
            command.Prepare();
            reader = command.ExecuteReader();
            reader.Read();

            decimal factor = Convert.ToDecimal(reader["off_fact"]);

            reader.Close();

            string com = "Select * from regions where owner='" + name + "'";
            MySqlDataAdapter adpt = new MySqlDataAdapter(com, connection);
            DataSet myDataSet = new DataSet();
            adpt.Fill(myDataSet, "regions");
            DataTable myDataTable = myDataSet.Tables[0];
            DataRow tempRow = null;

            foreach (DataRow tempRow_Variable in myDataTable.Rows)
            {
                tempRow = tempRow_Variable;
                int army3 = Convert.ToInt32(tempRow["army"]);
                int farm3 = Convert.ToInt32(tempRow["farm"]);
                int craft3 = Convert.ToInt32(tempRow["craft"]);
                int dealer3 = Convert.ToInt32(tempRow["dealer"]);
                decimal def_fact = Convert.ToInt32(tempRow["def_fact"]);
                string city = Convert.ToString(tempRow["name"]);

                if (sum_army != 0)
                {
                    missing = missing - ((army3 / sum_army) * missing_con);
                    
                    army3 = army3 - ((army3 / sum_army) * missing_con);
                    
                    int offense4 = Convert.ToInt32(Math.Round(army3 * (def_military * factor + 1)));
                    decimal defence4 = Math.Round(def_fact * (farm3 + craft3 + dealer3) + offense4);

                    command.CommandText = "UPDATE regions SET army=@p43, defence=@p44, cost=@p45, offense=@p46 Where name=@p47 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p43", army3);
                    command.Parameters.AddWithValue("@p44", Convert.ToInt32(defence4));
                    command.Parameters.AddWithValue("@p45", army3);
                    command.Parameters.AddWithValue("@p46", offense4);
                    command.Parameters.AddWithValue("@p47", city);
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                }
            }
        }

        public static void search_around_fix(int id)
        {
            MySqlConnection connection = new MySqlConnection(sqlstring);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM regions where id=@p2";
            command.Prepare();
            command.Parameters.AddWithValue("@p2", id);
            reader = command.ExecuteReader();
            reader.Read();

            string def_name = reader["owner"].ToString();
            int def_id = Convert.ToInt32(reader["id"]);
            string def_reg_name = reader["name"].ToString();
            int def_def = Convert.ToInt32(reader["defence"]);
            int farmcon = Convert.ToInt32(reader["farmcon"]);
            int craftcon = Convert.ToInt32(reader["craftcon"]);
            int dealercon = Convert.ToInt32(reader["dealercon"]);
            int def_farm = Convert.ToInt32(reader["farm"]);
            int def_craft = Convert.ToInt32(reader["craft"]);
            int def_dealer = Convert.ToInt32(reader["dealer"]);
            int def_cost = Convert.ToInt32(reader["cost"]);
            int def_army = Convert.ToInt32(reader["army"]);
            int level = Convert.ToInt32(reader["level"]);
            decimal def_fact = Convert.ToDecimal(reader["def_fact"]);

            reader.Close();

            command.CommandText = "SELECT * FROM player where username='" + def_name + "'";
            command.Prepare();
            reader = command.ExecuteReader();
            int free_army = 0;
            decimal def_military = 0;
            int def_army_debt_sunasp = 0;
            int def_army_given_sunasp = 0;
            int def_army_given_enwsi = 0;
            string def_sunasp = "";
            int def_army_debt_enwsi = 0;
            string def_enwsi = "";
            if (reader.Read())
            {
                def_army_debt_sunasp = Convert.ToInt32(reader["army_debt_sunasp"]);
                def_army_given_sunasp = Convert.ToInt32(reader["army_given_sunasp"]);
                def_army_given_enwsi = Convert.ToInt32(reader["army_given_enwsi"]);
                free_army = Convert.ToInt32(reader["free_army"]);
                def_military = Convert.ToDecimal(reader["military"]);
                def_sunasp = reader["sunasp"].ToString();
                def_army_debt_enwsi = Convert.ToInt32(reader["army_debt_enwsi"]);
                def_enwsi = reader["enwsi"].ToString();
            }

            reader.Close();

            command.CommandText = "SELECT * FROM variables";
            command.Prepare();
            reader = command.ExecuteReader();
            reader.Read();

            int farm_production = Convert.ToInt32(reader["farm_production"]);
            int craft_production = Convert.ToInt32(reader["craft_production"]);
            int dealer_production = Convert.ToInt32(reader["dealer_production"]);
            int ini_gold = Convert.ToInt32(reader["ini_gold"]);
            int level_ini = Convert.ToInt32(reader["level"]);
            decimal pol_stab = Convert.ToDecimal(reader["pol_stab"]);
            decimal levelup_fact = Convert.ToDecimal(reader["levelup_fact"]);

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
                farm_max = Convert.ToInt32(farmcon + (farmcon * levelup_fact) * (level - 1));
                craft_max = Convert.ToInt32(craftcon + (craftcon * levelup_fact) * (level - 1));
                dealer_max = Convert.ToInt32(dealercon + (dealercon * levelup_fact) * (level - 1));
            }

            int missing_population = (farm_max + craft_max + dealer_max) - (def_farm + def_craft + def_dealer);
            int change = 0;

            if (missing_population > 0)
            {
                change = def_army - missing_population;
                if (change > 0)
                {
                    def_army = change;
                }
                else if (change == 0)
                {
                    def_army = 0;
                }
                else
                {
                    def_army = 0;
                    missing_population = Math.Abs(change);
                    int missing_con = missing_population;

                    decimal sum_army = 0;
                    MySqlCommand command3 = new MySqlCommand("SELECT SUM(army) FROM regions where owner='" + def_name + "' and id!='" + def_id + "'", connection);
                    reader = command3.ExecuteReader();
                    if (reader.Read())
                    {
                        if (reader[0] != System.DBNull.Value)
                        {
                            sum_army = Convert.ToDecimal(reader[0]);
                        }
                    }
                    reader.Close();

                    command.CommandText = "SELECT * FROM variables";
                    command.Prepare();
                    reader = command.ExecuteReader();
                    reader.Read();

                    decimal factor = Convert.ToDecimal(reader["off_fact"]);

                    reader.Close();

                    string com = "Select * from regions where owner='" + def_name + "' and id!='" + def_id + "'";
                    MySqlDataAdapter adpt = new MySqlDataAdapter(com, connection);
                    DataSet myDataSet = new DataSet();
                    adpt.Fill(myDataSet, "regions");
                    DataTable myDataTable = myDataSet.Tables[0];
                    DataRow tempRow = null;

                    foreach (DataRow tempRow_Variable in myDataTable.Rows)
                    {
                        tempRow = tempRow_Variable;
                        decimal army3 = Convert.ToDecimal(tempRow["army"]);
                        int farm3 = Convert.ToInt32(tempRow["farm"]);
                        int craft3 = Convert.ToInt32(tempRow["craft"]);
                        int dealer3 = Convert.ToInt32(tempRow["dealer"]);
                        string city = Convert.ToString(tempRow["name"]);

                        if (sum_army != 0)
                        {
                            if (Convert.ToInt32(Math.Round((army3 / sum_army) * missing_con)) >= army3)
                            {
                                missing_population = missing_population - Convert.ToInt32(army3);
                                army3 = 0;
                                //army3 = army3 - Convert.ToInt32(((army3 / sum_army) * missing_con));
                            }
                            else
                            {
                                missing_population = missing_population - Convert.ToInt32(Math.Round((army3 / sum_army) * missing_con));
                                army3 = army3 - Convert.ToInt32(Math.Round((army3 / sum_army) * missing_con));
                            }

                            int offense4 = Convert.ToInt32(Math.Round(army3 * (def_military * factor + 1)));
                            decimal defence4 = Math.Round(def_fact * (farm3 + craft3 + dealer3) + offense4);

                            command.CommandText = "UPDATE regions SET army=@p43, defence=@p44, cost=@p45, offense=@p46 Where name=@p47 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p43", army3);
                            command.Parameters.AddWithValue("@p44", Convert.ToInt32(defence4));
                            command.Parameters.AddWithValue("@p45", army3);
                            command.Parameters.AddWithValue("@p46", offense4);
                            command.Parameters.AddWithValue("@p47", city);
                            command.ExecuteNonQuery();

                            if (missing_population == 0)
                            {
                                break;
                            }
                            command.Parameters.Clear();
                        }
                    }

                    if (missing_population > 0)
                    {
                        string com1 = "Select * from attacks where att_player='" + def_name + "' && army>='" + missing_population + "' ORDER BY army ASC";
                        MySqlDataAdapter adpt1 = new MySqlDataAdapter(com1, connection);
                        DataSet myDataSet1 = new DataSet();
                        adpt1.Fill(myDataSet1, "attacks");
                        DataTable myDataTable1 = myDataSet1.Tables[0];
                        DataRow tempRow1 = null;

                        foreach (DataRow tempRow1_Variable in myDataTable1.Rows)
                        {
                            tempRow1 = tempRow1_Variable;
                            int army4 = Convert.ToInt32(tempRow1["army"]);
                            int code = Convert.ToInt32(tempRow1["id"]);
                            string city1 = Convert.ToString(tempRow1["att_region"]);

                            army4 = army4 - missing_population;


                            if (army4 >= 0)
                            {
                                missing_population = 0;
                                free_army = free_army + army4;

                                command.CommandText = "DELETE FROM attacks Where id='" + code + "'";
                                command.Prepare();
                                command.ExecuteNonQuery();

                                command.Parameters.Clear();
                                //command.CommandText = "SELECT * FROM regions where name='" + city1 + "'";
                                //command.Prepare();
                                //reader = command.ExecuteReader();
                                //reader.Read();

                                //int army5 = Convert.ToInt32(reader["army"]);
                                //int farm4 = Convert.ToInt32(reader["farm"]);
                                //int craft4 = Convert.ToInt32(reader["craft"]);
                                //int dealer4 = Convert.ToInt32(reader["dealer"]);

                                //reader.Close();

                                //int offense5 = Convert.ToInt32(Math.Round((army4 + army5) * (def_military * factor + 1)));
                                //double defence5 = Math.Round(def_fact * (farm4 + craft4 + dealer4) + offense5);

                                //command.CommandText = "UPDATE regions SET army=@p43, defence=@p44, cost=@p45, offense=@p46 Where name=@p47 ";
                                //command.Prepare();
                                //command.Parameters.AddWithValue("@p43", army5 + army4);
                                //command.Parameters.AddWithValue("@p44", defence5);
                                //command.Parameters.AddWithValue("@p45", army5 + army4);
                                //command.Parameters.AddWithValue("@p46", offense5);
                                //command.Parameters.AddWithValue("@p47", city1);
                                //command.ExecuteNonQuery();
                            }
                            break;
                        }
                    }
                    if (missing_population > 0)
                    {
                        string com2 = "Select * from attacks where att_player='" + def_name + "' ORDER BY army DESC";
                        MySqlDataAdapter adpt2 = new MySqlDataAdapter(com2, connection);
                        DataSet myDataSet2 = new DataSet();
                        adpt2.Fill(myDataSet2, "attacks");
                        DataTable myDataTable2 = myDataSet2.Tables[0];
                        DataRow tempRow2 = null;

                        foreach (DataRow tempRow2_Variable in myDataTable2.Rows)
                        {
                            tempRow2 = tempRow2_Variable;
                            int code1 = Convert.ToInt32(tempRow2["id"]);
                            int army7 = Convert.ToInt32(tempRow2["army"]);
                            string att_city = Convert.ToString(tempRow["name"]);

                            missing_population = missing_population - army7;
                            if (missing_population <= 0)
                            {
                                army7 = Math.Abs(missing_population);
                                missing_population = 0;
                                free_army = free_army + army7;

                                command.CommandText = "DELETE FROM attacks Where id='" + code1 + "'";
                                command.Prepare();
                                command.ExecuteNonQuery();
                                break;
                            }
                            else
                            {
                                army7 = 0;

                                command.CommandText = "DELETE FROM attacks Where id='" + code1 + "'";
                                command.Prepare();
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    if (missing_population > 0)
                    {
                        command.Parameters.Clear();

                        string com6 = "Select * from attacks where region_coplayer like '%" + def_reg_name + "%'";
                        MySqlDataAdapter adpt6 = new MySqlDataAdapter(com6, connection);
                        DataSet myDataSet6 = new DataSet();
                        adpt6.Fill(myDataSet6, "attacks");
                        DataTable myDataTable6 = myDataSet6.Tables[0];
                        DataRow tempRow6 = null;

                        foreach (DataRow tempRow6_Variable in myDataTable6.Rows)
                        {
                            command.Parameters.Clear();

                            tempRow6 = tempRow6_Variable;

                            int id1 = Convert.ToInt32(tempRow6["id"]);
                            string army_coplayer1 = tempRow6["army_coplayer"].ToString();
                            string region_coplayer1 = tempRow6["region_coplayer"].ToString();
                            string[] army_co1 = army_coplayer1.Split(',');
                            string[] region_co1 = region_coplayer1.Split(',');

                            List<int> list1 = new List<int>();

                            foreach (string army1 in army_co1)
                            {
                                int army_co2 = Int32.Parse(army1);
                                list1.Add(army_co2);
                            }

                            Dictionary<string, int> coplayers = new Dictionary<string, int>();
                            coplayers = Enumerable.Range(0, region_co1.Length).ToDictionary(w => region_co1[w], w => list1[w]);

                            if (coplayers.ContainsKey(def_reg_name))
                            {
                                missing_population = missing_population - coplayers[def_reg_name];
                                int change1 = 0;
                                if (missing_population <= 0)
                                {
                                    change1 = Math.Abs(missing_population);
                                    missing_population = 0;
                                    free_army = free_army + change1;

                                    break;
                                }
                                coplayers.Remove(def_reg_name);
                            }

                            int[] co_armys = coplayers.Values.ToArray();
                            string[] co_play = coplayers.Keys.ToArray();
                            string[] co_armys1 = Array.ConvertAll(co_armys, element => element.ToString());
                            if (co_armys1.Length > 1)
                            {
                                army_coplayer1 = string.Join(",", co_armys1);
                                region_coplayer1 = string.Join(",", co_play);
                            }
                            else if (co_armys1.Length == 0)
                            {
                                army_coplayer1 = "";
                                region_coplayer1 = "";
                            }
                            else
                            {
                                army_coplayer1 = Convert.ToString(co_armys[0]);
                                region_coplayer1 = co_play[0];
                            }

                            command.CommandText = "UPDATE attacks SET army_coplayer=@p23, region_coplayer=@p24 Where id=@p27 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p23", army_coplayer1);
                            command.Parameters.AddWithValue("@p24", region_coplayer1);
                            command.Parameters.AddWithValue("@p27", id1);
                            command.ExecuteNonQuery();

                            command.CommandText = "DELETE FROM support_check Where sup_region=@p14";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p14", def_reg_name);
                            command.ExecuteNonQuery();
                        }
                    }

                    if (missing_population > 0 && def_army_debt_sunasp > 0 && def_army_given_sunasp > 0)
                    {
                        command.Parameters.Clear();

                        command.CommandText = "SELECT * FROM sunaspismos where name='" + def_sunasp + "'";
                        command.Prepare();
                        reader = command.ExecuteReader();
                        reader.Read();
                        int army_live = Convert.ToInt32(reader["army_live"]);
                        reader.Close();

                        missing_population = missing_population - def_army_given_sunasp;

                        if (army_live >= def_army_given_sunasp)
                        {
                            army_live = army_live - def_army_given_sunasp;
                            def_army_given_sunasp = 0;

                            command.CommandText = "UPDATE sunaspismos SET army_live=@p34 Where name=@p35 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p34", army_live);
                            command.Parameters.AddWithValue("@p35", def_sunasp);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            army_live = army_live - def_army_given_sunasp;
                            def_army_given_sunasp = Math.Abs(army_live);
                            army_live = 0;

                            command.CommandText = "UPDATE sunaspismos SET army_live=@p34 Where name=@p35 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p34", army_live);
                            command.Parameters.AddWithValue("@p35", def_sunasp);
                            command.ExecuteNonQuery();

                            string com5 = "Select * from attacks where sunaspismos='" + def_sunasp + "'";
                            MySqlDataAdapter adpt5 = new MySqlDataAdapter(com5, connection);
                            DataSet myDataSet5 = new DataSet();
                            adpt5.Fill(myDataSet5, "attacks");
                            DataTable myDataTable5 = myDataSet5.Tables[0];
                            DataRow tempRow5 = null;

                            foreach (DataRow tempRow5_Variable in myDataTable5.Rows)
                            {
                                command.Parameters.Clear();
                                tempRow5 = tempRow5_Variable;

                                int army_sunasp1 = Convert.ToInt32(tempRow5["army_sunasp"]);
                                int id2 = Convert.ToInt32(tempRow5["id"]);

                                def_army_given_sunasp = def_army_given_sunasp - army_sunasp1;

                                if (def_army_given_sunasp <= 0)
                                {
                                    army_sunasp1 = Math.Abs(def_army_given_sunasp);
                                    def_army_given_sunasp = 0;

                                    command.CommandText = "UPDATE attacks SET army_sunasp=@p36 Where id=@p37 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p36", army_sunasp1);
                                    command.Parameters.AddWithValue("@p37", id2);
                                    command.ExecuteNonQuery();

                                    break;
                                }
                                else
                                {
                                    army_sunasp1 = 0;

                                    command.CommandText = "UPDATE attacks SET army_sunasp=@p36, sunaspismos=@p38 Where id=@p37 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p36", army_sunasp1);
                                    command.Parameters.AddWithValue("@p37", id2);
                                    command.Parameters.AddWithValue("@p38", "");
                                    command.ExecuteNonQuery();
                                }

                            }
                        }
                        command.Parameters.Clear();

                        command.CommandText = "UPDATE player SET army_given_sunasp=@p36 Where username=@p37 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p36", 0);
                        command.Parameters.AddWithValue("@p37", def_name);
                        command.ExecuteNonQuery();
                    }

                    if (missing_population > 0 && def_army_debt_enwsi > 0 && def_army_given_enwsi > 0)
                    {
                        command.Parameters.Clear();

                        command.CommandText = "SELECT * FROM enwsi where name='" + def_enwsi + "'";
                        command.Prepare();
                        reader = command.ExecuteReader();
                        reader.Read();
                        int army_live = Convert.ToInt32(reader["army_live"]);
                        reader.Close();

                        missing_population = missing_population - def_army_given_enwsi;

                        if (army_live >= def_army_given_enwsi)
                        {
                            army_live = army_live - def_army_given_enwsi;
                            def_army_given_enwsi = 0;

                            command.CommandText = "UPDATE enwsi SET army_live=@p34 Where name=@p35 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p34", army_live);
                            command.Parameters.AddWithValue("@p35", def_enwsi);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            army_live = army_live - def_army_given_enwsi;
                            def_army_given_enwsi = Math.Abs(army_live);
                            army_live = 0;

                            command.CommandText = "UPDATE enwsi SET army_live=@p34 Where name=@p35 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p34", army_live);
                            command.Parameters.AddWithValue("@p35", def_enwsi);
                            command.ExecuteNonQuery();

                            string com5 = "Select * from attacks where enwsi='" + def_enwsi + "'";
                            MySqlDataAdapter adpt5 = new MySqlDataAdapter(com5, connection);
                            DataSet myDataSet5 = new DataSet();
                            adpt5.Fill(myDataSet5, "attacks");
                            DataTable myDataTable5 = myDataSet5.Tables[0];
                            DataRow tempRow5 = null;

                            foreach (DataRow tempRow5_Variable in myDataTable5.Rows)
                            {
                                command.Parameters.Clear();

                                tempRow5 = tempRow5_Variable;

                                int army_enwsi1 = Convert.ToInt32(tempRow5["army_enwsi"]);
                                int id2 = Convert.ToInt32(tempRow5["id"]);

                                def_army_given_enwsi = def_army_given_enwsi - army_enwsi1;

                                if (def_army_given_enwsi <= 0)
                                {
                                    army_enwsi1 = Math.Abs(def_army_given_enwsi);
                                    def_army_given_enwsi = 0;

                                    command.CommandText = "UPDATE attacks SET army_enwsi=@p36 Where id=@p37 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p36", army_enwsi1);
                                    command.Parameters.AddWithValue("@p37", id2);
                                    command.ExecuteNonQuery();

                                    break;
                                }
                                else
                                {
                                    army_enwsi1 = 0;

                                    command.CommandText = "UPDATE attacks SET army_enwsi=@p36, enwsi=@p38 Where id=@p37 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p36", army_enwsi1);
                                    command.Parameters.AddWithValue("@p37", id2);
                                    command.Parameters.AddWithValue("@p38", "");
                                    command.ExecuteNonQuery();
                                }

                            }
                        }
                        command.Parameters.Clear();

                        command.CommandText = "UPDATE player SET army_given_enwsi=@p36 Where username=@p37 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p36", 0);
                        command.Parameters.AddWithValue("@p37", def_name);
                        command.ExecuteNonQuery();
                    }
                }
            }

            if (missing_population > 0)
            {
                free_army = free_army - missing_population;
                if (free_army < 0)
                {
                    missing_population = Math.Abs(free_army);
                    free_army = 0;
                }
                else
                {
                    missing_population = 0;
                }
            }

            command.Parameters.Clear();
            int leftovers = 0;
            string com3 = "Select * from attacks where att_region='" + def_reg_name + "'";
            MySqlDataAdapter adpt3 = new MySqlDataAdapter(com3, connection);
            DataSet myDataSet3 = new DataSet();
            adpt3.Fill(myDataSet3, "attacks");
            DataTable myDataTable3 = myDataSet3.Tables[0];
            DataRow tempRow3 = null;

            foreach (DataRow tempRow3_Variable in myDataTable3.Rows)
            {
                tempRow3 = tempRow3_Variable;
                int army6 = Convert.ToInt32(tempRow3["army"]);
                leftovers = leftovers + army6;
            }

            free_army = free_army + leftovers;

            command.CommandText = "DELETE FROM attacks Where att_region='" + def_reg_name + "'";
            command.Prepare();
            command.ExecuteNonQuery();

            command.Parameters.Clear();

            int revenue = (farmcon * farm_production) + (craftcon * craft_production) + (dealercon * dealer_production);
            decimal defence = Math.Round(def_fact * (farmcon + craftcon + dealercon));// to mhden einai to offence

            command.CommandText = "UPDATE regions SET farm=@p5, craft=@p6, dealer=@p7, army=@p4, defence=@p13, level=@p33, revenue=@p11, cost=@p15, offense=@p14, pol_stab=@p1, gold=@p16 Where name=@p50 ";
            command.Prepare();
            command.Parameters.AddWithValue("@p4", 0);
            command.Parameters.AddWithValue("@p5", farmcon);
            command.Parameters.AddWithValue("@p6", craftcon);
            command.Parameters.AddWithValue("@p7", dealercon);
            command.Parameters.AddWithValue("@p11", revenue);
            command.Parameters.AddWithValue("@p13", Convert.ToInt32(defence));
            command.Parameters.AddWithValue("@p14", 0);
            command.Parameters.AddWithValue("@p15", 0);
            command.Parameters.AddWithValue("@p16", ini_gold);
            command.Parameters.AddWithValue("@p50", def_reg_name);
            command.Parameters.AddWithValue("@p1", pol_stab);
            command.Parameters.AddWithValue("@p33", level_ini);
            command.ExecuteNonQuery();

            command.CommandText = "UPDATE player SET free_army=@p41 Where username=@p42 ";
            command.Prepare();
            command.Parameters.AddWithValue("@p41", free_army + def_army);
            command.Parameters.AddWithValue("@p42", def_name);
            command.ExecuteNonQuery();
        }

        public static int level_fix(int city_id, int missing_population, int army_new)
        {
            MySqlConnection connection = new MySqlConnection(sqlstring);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM regions where id=@p2";
            command.Prepare();
            command.Parameters.AddWithValue("@p2", city_id);
            reader = command.ExecuteReader();
            reader.Read();

            string def_name = reader["owner"].ToString();
            int def_id = Convert.ToInt32(reader["id"]);
            string def_reg_name = reader["name"].ToString();
            int def_def = Convert.ToInt32(reader["defence"]);
            int farmcon = Convert.ToInt32(reader["farmcon"]);
            int craftcon = Convert.ToInt32(reader["craftcon"]);
            int dealercon = Convert.ToInt32(reader["dealercon"]);
            int def_farm = Convert.ToInt32(reader["farm"]);
            int def_craft = Convert.ToInt32(reader["craft"]);
            int def_dealer = Convert.ToInt32(reader["dealer"]);
            int def_cost = Convert.ToInt32(reader["cost"]);
            int def_army = army_new;
            int level = Convert.ToInt32(reader["level"]);

            reader.Close();

            command.CommandText = "SELECT * FROM player where username='" + def_name + "'";
            command.Prepare();
            reader = command.ExecuteReader();
            int free_army = 0;
            decimal def_military = 0;
            int def_army_debt_sunasp = 0;
            int def_army_given_sunasp = 0;
            string def_sunasp = "";
            int def_army_debt_enwsi = 0;
            int def_army_given_enwsi = 0;
            string def_enwsi = "";
            if (reader.Read())
            {
                free_army = Convert.ToInt32(reader["free_army"]);
                def_military = Convert.ToDecimal(reader["military"]);
                def_army_debt_sunasp = Convert.ToInt32(reader["army_debt_sunasp"]);
                def_army_given_sunasp = Convert.ToInt32(reader["army_given_sunasp"]);
                def_army_given_enwsi = Convert.ToInt32(reader["army_given_enwsi"]);
                def_sunasp = reader["sunasp"].ToString();
                def_army_debt_enwsi = Convert.ToInt32(reader["army_debt_enwsi"]);
                def_enwsi = reader["enwsi"].ToString();
            }

            reader.Close();

            int change = 0;

            if (missing_population > 0)
            {
                change = def_army - missing_population;
                if (change > 0)
                {
                    def_army = change;
                }
                else if (change == 0)
                {
                    def_army = 0;
                }
                else
                {
                    def_army = 0;
                    missing_population = Math.Abs(change);
                    int missing_con = missing_population;

                    decimal sum_army = 0;
                    MySqlCommand command3 = new MySqlCommand("SELECT SUM(army) FROM regions where owner='" + def_name + "' and id!='" + def_id + "'", connection);
                    reader = command3.ExecuteReader();
                    if (reader.Read())
                    {
                        if (reader[0] != System.DBNull.Value)
                        {
                            sum_army = Convert.ToDecimal(reader[0]);
                        }
                    }
                    reader.Close();

                    string com = "Select * from regions where owner='" + def_name + "' and id!='" + def_id + "'";
                    MySqlDataAdapter adpt = new MySqlDataAdapter(com, connection);
                    DataSet myDataSet = new DataSet();
                    adpt.Fill(myDataSet, "regions");
                    DataTable myDataTable = myDataSet.Tables[0];
                    DataRow tempRow = null;

                    foreach (DataRow tempRow_Variable in myDataTable.Rows)
                    {
                        tempRow = tempRow_Variable;
                        decimal army3 = Convert.ToDecimal(tempRow["army"]);
                        int farm3 = Convert.ToInt32(tempRow["farm"]);
                        int craft3 = Convert.ToInt32(tempRow["craft"]);
                        int dealer3 = Convert.ToInt32(tempRow["dealer"]);
                        string city4 = Convert.ToString(tempRow["name"]);
                        decimal def_fact = Convert.ToDecimal(tempRow["def_fact"]);

                        if (sum_army != 0)
                        {
                            
                            if (Convert.ToInt32(Math.Round((army3 / sum_army) * missing_con)) >= army3)
                            {
                                missing_population = missing_population - Convert.ToInt32(army3);
                                army3 = 0;
                                //army3 = army3 - Convert.ToInt32(((army3 / sum_army) * missing_con));
                            }
                            else
                            {
                                missing_population = missing_population - Convert.ToInt32(Math.Round((army3 / sum_army) * missing_con));
                                army3 = army3 - Convert.ToInt32(Math.Round((army3 / sum_army) * missing_con));
                            }

                            command.CommandText = "SELECT * FROM variables";
                            command.Prepare();
                            reader = command.ExecuteReader();
                            reader.Read();

                            decimal factor = Convert.ToDecimal(reader["off_fact"]);

                            reader.Close();

                            int offense4 = Convert.ToInt32(Math.Round(army3 * (def_military * factor + 1)));
                            decimal defence4 = Math.Round(def_fact * (farm3 + craft3 + dealer3) + offense4);

                            command.CommandText = "UPDATE regions SET army=@p43, defence=@p44, cost=@p45, offense=@p46 Where name=@p47 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p43", army3);
                            command.Parameters.AddWithValue("@p44", Convert.ToInt32(defence4));
                            command.Parameters.AddWithValue("@p45", army3);
                            command.Parameters.AddWithValue("@p46", offense4);
                            command.Parameters.AddWithValue("@p47", city4);
                            command.ExecuteNonQuery();

                            if(missing_population == 0)
                            {
                                break;
                            }
                            
                            command.Parameters.Clear();
                        }
                    }

                    if (missing_population > 0)
                    {
                        string com1 = "Select * from attacks where att_player='" + def_name + "' && army>='" + missing_population + "' ORDER BY army ASC";
                        MySqlDataAdapter adpt1 = new MySqlDataAdapter(com1, connection);
                        DataSet myDataSet1 = new DataSet();
                        adpt1.Fill(myDataSet1, "attacks");
                        DataTable myDataTable1 = myDataSet1.Tables[0];
                        DataRow tempRow1 = null;

                        foreach (DataRow tempRow1_Variable in myDataTable1.Rows)
                        {
                            tempRow1 = tempRow1_Variable;
                            int army4 = Convert.ToInt32(tempRow1["army"]);
                            int code = Convert.ToInt32(tempRow1["id"]);
                            string city1 = Convert.ToString(tempRow1["att_region"]);

                            army4 = army4 - missing_population;


                            if (army4 >= 0)
                            {
                                missing_population = 0;
                                free_army = free_army + army4;

                                command.CommandText = "DELETE FROM attacks Where id='" + code + "'";
                                command.Prepare();
                                command.ExecuteNonQuery();

                                command.Parameters.Clear();

                                //command.CommandText = "SELECT * FROM regions where name='" + city1 + "'";
                                //command.Prepare();
                                //reader = command.ExecuteReader();
                                //reader.Read();

                                //int army5 = Convert.ToInt32(reader["army"]);
                                //int farm4 = Convert.ToInt32(reader["farm"]);
                                //int craft4 = Convert.ToInt32(reader["craft"]);
                                //int dealer4 = Convert.ToInt32(reader["dealer"]);

                                //reader.Close();

                                //int offense5 = Convert.ToInt32(Math.Round((army4 + army5) * (def_military * factor + 1)));
                                //double defence5 = Math.Round(def_fact * (farm4 + craft4 + dealer4) + offense5);

                                //command.CommandText = "UPDATE regions SET army=@p43, defence=@p44, cost=@p45, offense=@p46 Where name=@p47 ";
                                //command.Prepare();
                                //command.Parameters.AddWithValue("@p43", army5 + army4);
                                //command.Parameters.AddWithValue("@p44", defence5);
                                //command.Parameters.AddWithValue("@p45", army5 + army4);
                                //command.Parameters.AddWithValue("@p46", offense5);
                                //command.Parameters.AddWithValue("@p47", city1);
                                //command.ExecuteNonQuery();
                            }
                            break;
                        }
                    }
                    if (missing_population > 0)
                    {
                        string com2 = "Select * from attacks where att_player='" + def_name + "' ORDER BY army DESC";
                        MySqlDataAdapter adpt2 = new MySqlDataAdapter(com2, connection);
                        DataSet myDataSet2 = new DataSet();
                        adpt2.Fill(myDataSet2, "attacks");
                        DataTable myDataTable2 = myDataSet2.Tables[0];
                        DataRow tempRow2 = null;

                        foreach (DataRow tempRow2_Variable in myDataTable2.Rows)
                        {
                            tempRow2 = tempRow2_Variable;
                            int code1 = Convert.ToInt32(tempRow2["id"]);
                            int army7 = Convert.ToInt32(tempRow2["army"]);
                            string att_city = Convert.ToString(tempRow["name"]);

                            missing_population = missing_population - army7;
                            if (missing_population <= 0)
                            {
                                army7 = Math.Abs(missing_population);
                                missing_population = 0;
                                free_army = free_army + army7;

                                command.CommandText = "DELETE FROM attacks Where id='" + code1 + "'";
                                command.Prepare();
                                command.ExecuteNonQuery();
                                break;
                            }
                            else
                            {
                                army7 = 0;

                                command.CommandText = "DELETE FROM attacks Where id='" + code1 + "'";
                                command.Prepare();
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    if (missing_population > 0)
                    {
                        command.Parameters.Clear();

                        string com6 = "Select * from attacks where region_coplayer like '%" + def_reg_name + "%'";
                        MySqlDataAdapter adpt6 = new MySqlDataAdapter(com6, connection);
                        DataSet myDataSet6 = new DataSet();
                        adpt6.Fill(myDataSet6, "attacks");
                        DataTable myDataTable6 = myDataSet6.Tables[0];
                        DataRow tempRow6 = null;

                        foreach (DataRow tempRow6_Variable in myDataTable6.Rows)
                        {
                            command.Parameters.Clear();

                            tempRow6 = tempRow6_Variable;

                            int id1 = Convert.ToInt32(tempRow6["id"]);
                            string army_coplayer1 = tempRow6["army_coplayer"].ToString();
                            string region_coplayer1 = tempRow6["region_coplayer"].ToString();
                            string[] army_co1 = army_coplayer1.Split(',');
                            string[] region_co1 = region_coplayer1.Split(',');

                            List<int> list1 = new List<int>();

                            foreach (string army1 in army_co1)
                            {
                                int army_co2 = Int32.Parse(army1);
                                list1.Add(army_co2);
                            }

                            Dictionary<string, int> coplayers = new Dictionary<string, int>();
                            coplayers = Enumerable.Range(0, region_co1.Length).ToDictionary(w => region_co1[w], w => list1[w]);

                            if (coplayers.ContainsKey(def_reg_name))
                            {
                                missing_population = missing_population - coplayers[def_reg_name];
                                int change1 = 0;
                                if (missing_population <= 0)
                                {
                                    change1 = Math.Abs(missing_population);
                                    missing_population = 0;
                                    free_army = free_army + change1;

                                    break;
                                }
                                coplayers.Remove(def_reg_name);
                            }

                            int[] co_armys = coplayers.Values.ToArray();
                            string[] co_play = coplayers.Keys.ToArray();
                            string[] co_armys1 = Array.ConvertAll(co_armys, element => element.ToString());
                            if (co_armys1.Length > 1)
                            {
                                army_coplayer1 = string.Join(",", co_armys1);
                                region_coplayer1 = string.Join(",", co_play);
                            }
                            else if (co_armys1.Length == 0)
                            {
                                army_coplayer1 = "";
                                region_coplayer1 = "";
                            }
                            else
                            {
                                army_coplayer1 = Convert.ToString(co_armys[0]);
                                region_coplayer1 = co_play[0];
                            }

                            command.CommandText = "UPDATE attacks SET army_coplayer=@p23, region_coplayer=@p24 Where id=@p27 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p23", army_coplayer1);
                            command.Parameters.AddWithValue("@p24", region_coplayer1);
                            command.Parameters.AddWithValue("@p27", id1);
                            command.ExecuteNonQuery();

                            command.CommandText = "DELETE FROM support_check Where sup_region=@p14";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p14", def_reg_name);
                            command.ExecuteNonQuery();
                        }
                    }

                    if (missing_population > 0 && def_army_debt_sunasp > 0 && def_army_given_sunasp > 0)
                    {
                        command.Parameters.Clear();

                        command.CommandText = "SELECT * FROM sunaspismos where name='" + def_sunasp + "'";
                        command.Prepare();
                        reader = command.ExecuteReader();
                        reader.Read();
                        int army_live = Convert.ToInt32(reader["army_live"]);
                        reader.Close();

                        missing_population = missing_population - def_army_given_sunasp;

                        if (army_live >= def_army_given_sunasp)
                        {
                            army_live = army_live - def_army_given_sunasp;
                            def_army_given_sunasp = 0;
                            
                            command.CommandText = "UPDATE sunaspismos SET army_live=@p34 Where name=@p35 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p34", army_live);
                            command.Parameters.AddWithValue("@p35", def_sunasp);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            army_live = army_live - def_army_given_sunasp;
                            def_army_given_sunasp = Math.Abs(army_live);
                            army_live = 0;

                            command.CommandText = "UPDATE sunaspismos SET army_live=@p34 Where name=@p35 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p34", army_live);
                            command.Parameters.AddWithValue("@p35", def_sunasp);
                            command.ExecuteNonQuery();

                            string com5 = "Select * from attacks where sunaspismos='" + def_sunasp + "'";
                            MySqlDataAdapter adpt5 = new MySqlDataAdapter(com5, connection);
                            DataSet myDataSet5 = new DataSet();
                            adpt5.Fill(myDataSet5, "attacks");
                            DataTable myDataTable5 = myDataSet5.Tables[0];
                            DataRow tempRow5 = null;

                            foreach (DataRow tempRow5_Variable in myDataTable5.Rows)
                            {
                                command.Parameters.Clear();
                                tempRow5 = tempRow5_Variable;

                                int army_sunasp1 = Convert.ToInt32(tempRow5["army_sunasp"]);
                                int id2 = Convert.ToInt32(tempRow5["id"]);

                                def_army_given_sunasp = def_army_given_sunasp - army_sunasp1;

                                if (def_army_given_sunasp <= 0)
                                {
                                    army_sunasp1 = Math.Abs(def_army_given_sunasp);
                                    def_army_given_sunasp = 0;

                                    command.CommandText = "UPDATE attacks SET army_sunasp=@p36 Where id=@p37 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p36", army_sunasp1);
                                    command.Parameters.AddWithValue("@p37", id2);
                                    command.ExecuteNonQuery();

                                    break;
                                }
                                else
                                {
                                    army_sunasp1 = 0;

                                    command.CommandText = "UPDATE attacks SET army_sunasp=@p36, sunaspismos=@p38 Where id=@p37 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p36", army_sunasp1);
                                    command.Parameters.AddWithValue("@p37", id2);
                                    command.Parameters.AddWithValue("@p38", "");
                                    command.ExecuteNonQuery();
                                }

                            }
                        }
                        command.Parameters.Clear();

                        command.CommandText = "UPDATE player SET army_given_sunasp=@p36 Where username=@p37 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p36", 0);
                        command.Parameters.AddWithValue("@p37", def_name);
                        command.ExecuteNonQuery();
                    }

                    if (missing_population > 0 && def_army_debt_enwsi > 0 && def_army_given_enwsi > 0)
                    {
                        command.Parameters.Clear();

                        command.CommandText = "SELECT * FROM enwsi where name='" + def_enwsi + "'";
                        command.Prepare();
                        reader = command.ExecuteReader();
                        reader.Read();
                        int army_live = Convert.ToInt32(reader["army_live"]);
                        reader.Close();

                        missing_population = missing_population - def_army_given_enwsi;

                        if (army_live >= def_army_given_enwsi)
                        {
                            army_live = army_live - def_army_given_enwsi;
                            def_army_given_enwsi = 0;

                            command.CommandText = "UPDATE enwsi SET army_live=@p34 Where name=@p35 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p34", army_live);
                            command.Parameters.AddWithValue("@p35", def_enwsi);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            army_live = army_live - def_army_given_enwsi;
                            def_army_given_enwsi = Math.Abs(army_live);
                            army_live = 0;

                            command.CommandText = "UPDATE enwsi SET army_live=@p34 Where name=@p35 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p34", army_live);
                            command.Parameters.AddWithValue("@p35", def_enwsi);
                            command.ExecuteNonQuery();

                            string com5 = "Select * from attacks where enwsi='" + def_enwsi + "'";
                            MySqlDataAdapter adpt5 = new MySqlDataAdapter(com5, connection);
                            DataSet myDataSet5 = new DataSet();
                            adpt5.Fill(myDataSet5, "attacks");
                            DataTable myDataTable5 = myDataSet5.Tables[0];
                            DataRow tempRow5 = null;

                            foreach (DataRow tempRow5_Variable in myDataTable5.Rows)
                            {
                                command.Parameters.Clear();

                                tempRow5 = tempRow5_Variable;

                                int army_enwsi1 = Convert.ToInt32(tempRow5["army_enwsi"]);
                                int id2 = Convert.ToInt32(tempRow5["id"]);

                                def_army_given_enwsi = def_army_given_enwsi - army_enwsi1;

                                if (def_army_given_enwsi <= 0)
                                {
                                    army_enwsi1 = Math.Abs(def_army_given_enwsi);
                                    def_army_given_enwsi = 0;

                                    command.CommandText = "UPDATE attacks SET army_enwsi=@p36 Where id=@p37 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p36", army_enwsi1);
                                    command.Parameters.AddWithValue("@p37", id2);
                                    command.ExecuteNonQuery();

                                    break;
                                }
                                else
                                {
                                    army_enwsi1 = 0;

                                    command.CommandText = "UPDATE attacks SET army_enwsi=@p36, enwsi=@p38 Where id=@p37 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p36", army_enwsi1);
                                    command.Parameters.AddWithValue("@p37", id2);
                                    command.Parameters.AddWithValue("@p38", "");
                                    command.ExecuteNonQuery();
                                }

                            }
                        }
                        command.Parameters.Clear();

                        command.CommandText = "UPDATE player SET army_given_enwsi=@p36 Where username=@p37 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p36", 0);
                        command.Parameters.AddWithValue("@p37", def_name);
                        command.ExecuteNonQuery();
                    }
                }
            }

            if (missing_population > 0)
            {
                free_army = free_army - missing_population;
                if (free_army < 0)
                {
                    missing_population = Math.Abs(free_army);
                    free_army = 0;
                }
                else
                {
                    missing_population = 0;
                }
            }

            command.CommandText = "UPDATE player SET free_army=@p41 Where username=@p42 ";
            command.Prepare();
            command.Parameters.AddWithValue("@p41", free_army + def_army);
            command.Parameters.AddWithValue("@p42", def_name);
            command.ExecuteNonQuery();

            return def_army;
        }

        public static Dictionary<string, string> vars()
        {
            MySqlConnection connection = new MySqlConnection(Variables.sqlstring);
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;
            Dictionary<string, string> var = new Dictionary<string, string>();
        
            connection.Open();

            command.CommandText = "SELECT * FROM variables LIMIT 0,1";
            command.Prepare();
            reader = command.ExecuteReader();
            reader.Read();

            var.Add("def_fact", Convert.ToString(reader["def_fact"]));
            var.Add("off_fact", Convert.ToString(reader["off_fact"]));
            var.Add("diaspora", Convert.ToString(reader["diaspora"]));
            var.Add("immune", Convert.ToString(reader["immune"]));
            var.Add("level", Convert.ToString(reader["level"]));
            var.Add("levelup_fact", Convert.ToString(reader["levelup_fact"]));
            var.Add("craft_per", Convert.ToString(reader["craft_per"]));
            var.Add("dealer_per", Convert.ToString(reader["dealer_per"]));
            var.Add("pol_stab", Convert.ToString(reader["pol_stab"]));
            var.Add("ini_regions", Convert.ToString(reader["ini_regions"]));
            var.Add("reg_dev_ab", Convert.ToString(reader["reg_dev_ab"]));
            var.Add("dev_margin", Convert.ToString(reader["dev_margin"]));
            var.Add("ini", Convert.ToString(reader["ini"]));
            var.Add("box_id", Convert.ToString(reader["box_id"]));
            var.Add("enwsi_gold_contribution", Convert.ToString(reader["enwsi_gold_contribution"]));
            var.Add("enwsi_player_limit", Convert.ToString(reader["enwsi_player_limit"]));
            var.Add("enwsi_abandon_percent", Convert.ToString(reader["enwsi_abandon_percent"]));
            var.Add("sunasp_army_contribution", Convert.ToString(reader["sunasp_army_contribution"]));
            var.Add("enwsi_army_contribution", Convert.ToString(reader["enwsi_army_contribution"]));
            var.Add("edres_demand_percent", Convert.ToString(reader["edres_demand_percent"]));
            var.Add("army_percent_sunasp", Convert.ToString(reader["army_percent_sunasp"]));
            var.Add("army_percent_enwsi", Convert.ToString(reader["army_percent_enwsi"]));
            var.Add("gold_percent", Convert.ToString(reader["gold_percent"]));
            var.Add("farm_percent", Convert.ToString(reader["farm_percent"]));
            var.Add("craft_percent", Convert.ToString(reader["craft_percent"]));
            var.Add("dealer_percent", Convert.ToString(reader["dealer_percent"]));
            var.Add("army_percent", Convert.ToString(reader["army_percent"]));
            var.Add("enwsi_leader_rounds", Convert.ToString(reader["enwsi_leader_rounds"]));
            var.Add("enwsi_vote_rounds", Convert.ToString(reader["enwsi_vote_rounds"]));
            var.Add("enwsi_break_rounds", Convert.ToString(reader["enwsi_break_rounds"]));
            var.Add("ini_gold", Convert.ToString(reader["ini_gold"]));
            var.Add("ini_military", Convert.ToString(reader["ini_military"]));
            var.Add("ini_trade", Convert.ToString(reader["ini_trade"]));
            var.Add("ini_political", Convert.ToString(reader["ini_political"]));
            var.Add("ini_diplomatic", Convert.ToString(reader["ini_diplomatic"]));
            var.Add("farm_production", Convert.ToString(reader["farm_production"]));
            var.Add("craft_production", Convert.ToString(reader["craft_production"]));
            var.Add("dealer_production", Convert.ToString(reader["dealer_production"]));
            var.Add("army_cost", Convert.ToString(reader["army_cost"]));
            var.Add("farm_margin", Convert.ToString(reader["farm_margin"]));
            var.Add("craft_margin", Convert.ToString(reader["craft_margin"]));
            var.Add("dealer_margin", Convert.ToString(reader["dealer_margin"]));
            var.Add("trade_fact", Convert.ToString(reader["trade_fact"]));
            var.Add("levelup_cost", Convert.ToString(reader["levelup_cost"]));
            var.Add("ini_x", Convert.ToString(reader["ini_x"]));
            var.Add("ini_y", Convert.ToString(reader["ini_y"]));
            
            reader.Close();
            connection.Close();

            return var;
        }

        public static void calculate_cost(int id)
        {
            MySqlConnection connection = new MySqlConnection(Variables.sqlstring);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM regions where id=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", id);
            reader = command.ExecuteReader();
            reader.Read();

            string region = reader["name"].ToString();
            string owner = reader["owner"].ToString();
            int imm = Convert.ToInt32(reader["immune"]);
            int rev = Convert.ToInt32(reader["revenue"]);
            int farmcon = Convert.ToInt32(reader["farmcon"]);
            int craftcon = Convert.ToInt32(reader["craftcon"]);
            int dealercon = Convert.ToInt32(reader["dealercon"]);
            int def_farm = Convert.ToInt32(reader["farm"]);
            int def_craft = Convert.ToInt32(reader["craft"]);
            int def_dealer = Convert.ToInt32(reader["dealer"]);
            int level = Convert.ToInt32(reader["level"]);
            int gold_region = Convert.ToInt32(reader["gold"]);

            reader.Close();

            command.CommandText = "SELECT * FROM player where username=@p6";
            command.Prepare();
            command.Parameters.AddWithValue("@p6", owner);
            reader = command.ExecuteReader();
            string enwsi = "";
            int army_debt_enwsi = 0;
            if (reader.Read())
            {
                enwsi = reader["enwsi"].ToString();
                army_debt_enwsi = Convert.ToInt32(reader["army_debt_enwsi"]);
            }

            reader.Close();

            int farm_max = 0;
            int craft_max = 0;
            int dealer_max = 0;

            command.CommandText = "SELECT * FROM variables";
            command.Prepare();
            reader = command.ExecuteReader();
            reader.Read();

            decimal levelup_fact = Convert.ToDecimal(reader["levelup_fact"]);
            decimal enwsi_army_contribution = Convert.ToDecimal(reader["enwsi_army_contribution"]);

            reader.Close();

            if (level <= 1)
            {
                farm_max = farmcon;
                craft_max = craftcon;
                dealer_max = dealercon;
            }
            else
            {
                farm_max = Convert.ToInt32(farmcon + (farmcon * levelup_fact) * (level - 1));
                craft_max = Convert.ToInt32(craftcon + (craftcon * levelup_fact) * (level - 1));
                dealer_max = Convert.ToInt32(dealercon + (dealercon * levelup_fact) * (level - 1));
            }

            int cost = 0;

            if (enwsi == "" && army_debt_enwsi == 0) 
	        {
                cost = (farm_max + craft_max + dealer_max) - (def_farm + def_craft + def_dealer);
            }
            else
            {
                cost = (farm_max + craft_max + dealer_max) - (def_farm + def_craft + def_dealer) + Convert.ToInt32(rev * enwsi_army_contribution);
            }

            command.CommandText = "UPDATE regions SET cost=@p5 Where id=@p1 ";
            command.Prepare();
            command.Parameters.AddWithValue("@p5", cost);
            command.ExecuteNonQuery();

            connection.Close();
        }

        public static void fix_defence(string player)
        {
            Dictionary<string, string> vars1 = vars();

            MySqlConnection connection = new MySqlConnection(Variables.sqlstring);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            string com4 = "Select * from regions where owner='" + player + "'";
            MySqlDataAdapter adpt4 = new MySqlDataAdapter(com4, connection);
            DataSet myDataSet4 = new DataSet();
            adpt4.Fill(myDataSet4, "attacks");
            DataTable myDataTable4 = myDataSet4.Tables[0];
            DataRow tempRow4 = null;

            decimal military = 0;

            command.CommandText = "SELECT * FROM player where username=@p24";
            command.Prepare();
            command.Parameters.AddWithValue("@p24", player);
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                military = Convert.ToDecimal(reader["military"]);
            }
            reader.Close();

            foreach (DataRow tempRow4_Variable in myDataTable4.Rows)
            {
                command.Parameters.Clear();
                tempRow4 = tempRow4_Variable;

                int id = Convert.ToInt32(tempRow4["id"]);
                int farm = Convert.ToInt32(tempRow4["farm"]);
                int craft = Convert.ToInt32(tempRow4["craft"]);
                int dealer = Convert.ToInt32(tempRow4["dealer"]);
                decimal def_fact = Convert.ToDecimal(tempRow4["def_fact"]);
                int army = Convert.ToInt32(tempRow4["army"]);

                int offense = Convert.ToInt32(Math.Round(army * (military * Convert.ToDecimal(vars1["off_fact"]) + 1)));
                decimal defence = Math.Round(def_fact * (farm + craft + dealer) + offense);

                command.CommandText = "UPDATE regions SET defence=@p1, offense=@p2 Where id=@p15 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", Convert.ToInt32(Math.Round(defence)));
                command.Parameters.AddWithValue("@p2", offense);
                command.Parameters.AddWithValue("@p15", id);
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }
}
