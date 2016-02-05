using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;

namespace StrategyGame
{
    public partial class Form3 : Form
    {
        string sqlcon = Variables.sqlstring;
        Dictionary<string, string> vars = Variables.vars();

        public Form3()
        {
            InitializeComponent();
            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" &&  pictureBox1.BackgroundImage != null)
            {
                MySqlConnection connection = new MySqlConnection(sqlcon);
                MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM player", connection);
                connection.Open();
                MySqlCommand command2 = connection.CreateCommand();
                MySqlDataReader reader;

                command2.CommandText = "SELECT username FROM player where username=@p12";
                command2.Prepare();
                command2.Parameters.AddWithValue("@p12", textBox3.Text);
                reader = command2.ExecuteReader();

                if (!reader.Read())
                {
                    reader.Close();
                    FileStream fs;
                    BinaryReader br;
                    string FileName = openFileDialog1.FileName;
                    byte[] ImageData;
                    fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                    br = new BinaryReader(fs);
                    ImageData = br.ReadBytes((int)fs.Length);
                    br.Close();
                    fs.Close();
                    string temp = command.ExecuteScalar().ToString();
                    int players = Convert.ToInt32(temp);


                    command.CommandText = "insert into player (id,name,picture,username,password,gold,military,political,diplomatic,trade,surname) values (@p1, @p2, @p11, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10)";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p1", players + 1);
                    command.Parameters.AddWithValue("@p2", textBox1.Text);
                    command.Parameters.AddWithValue("@p3", textBox3.Text);
                    command.Parameters.AddWithValue("@p4", textBox4.Text);
                    command.Parameters.AddWithValue("@p5", Convert.ToDecimal(vars["ini_gold"]));
                    command.Parameters.AddWithValue("@p6", Convert.ToDecimal(vars["ini_military"]));
                    command.Parameters.AddWithValue("@p7", Convert.ToDecimal(vars["ini_political"]));
                    command.Parameters.AddWithValue("@p8", Convert.ToDecimal(vars["ini_diplomatic"]));
                    command.Parameters.AddWithValue("@p9", Convert.ToDecimal(vars["ini_trade"]));
                    command.Parameters.AddWithValue("@p10", textBox2.Text);
                    command.Parameters.AddWithValue("@p11", ImageData);
                    command.ExecuteNonQuery();
                    connection.Close();

                    createRegions();
                    //SumCalculate();

                    MessageBox.Show("Επιτυχής Εγγραφή");
                }
                else
                {
                    MessageBox.Show("Το συγκεκριμένο username χρησιμοποιείται από άλλο παίκτη");
                    reader.Close();
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Συμπληρώστε τα απαραίτητα πεδία και μια φωτοφραφία λογαριασμού");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 frm = new Form2();
            frm.Show();
        }

        public void createRegions()
        {
            decimal defence;
            int offence = 0;
            int revenue;
            int box_id = Convert.ToInt32(vars["box_id"]);

            MySqlConnection connection1 = new MySqlConnection(sqlcon);
            MySqlCommand command1 = new MySqlCommand("SELECT COUNT(*) FROM regions", connection1);

            connection1.Open();
            string temp3 = command1.ExecuteScalar().ToString();
            connection1.Close();

            int num_cit = Convert.ToInt32(temp3);
            int box = num_cit / Convert.ToInt32(vars["ini_regions"]);
            int reg_box = box + 1;

            for (int i = num_cit + 1; i <= (Convert.ToInt32(vars["ini_regions"]) + num_cit); i++)
            {
                if (i == num_cit + 1)
                {

                    if (num_cit < Convert.ToInt32(vars["ini_regions"]))
                    {
                        Random randomizer1 = new Random();
                        int p = Convert.ToInt32(vars["reg_dev_ab"]) - Convert.ToInt32(vars["dev_margin"]) + randomizer1.Next(0, 20);
                        int craft = p * (Convert.ToInt32(vars["craft_per"]) - Convert.ToInt32(vars["dev_margin"]) + randomizer1.Next(0, 20)) / Convert.ToInt32(vars["reg_dev_ab"]);
                        int dealer = p * (Convert.ToInt32(vars["dealer_per"]) - Convert.ToInt32(vars["dev_margin"]) + randomizer1.Next(0, 20)) / Convert.ToInt32(vars["reg_dev_ab"]);
                        int farm = p - craft - dealer;
                        defence = Math.Round(Convert.ToDecimal(vars["def_fact"]) * (farm + craft + dealer) + offence);
                        revenue = (farm * Convert.ToInt32(vars["farm_production"])) + (craft * Convert.ToInt32(vars["craft_production"])) + (dealer * Convert.ToInt32(vars["dealer_production"]));

                        MySqlConnection connection2 = new MySqlConnection(sqlcon);
                        connection2.Open();
                        MySqlCommand command2 = connection2.CreateCommand();
                        command2.CommandText = "insert into regions (id,name,owner,farmcon,craftcon,dealercon,farm,craft,dealer,defence,level,revenue,immune,pol_stab,x,y,gold,reg_box,ini,box_id,def_fact) values (@p2, @p3, @p22, @p4, @p5, @p6, @p7, @p8, @p9, @p20, @p10, @p21, @p11, @p12, @p13, @p14, @p15, @p16, @p17, @p18, @p19)";
                        command2.Prepare();
                        command2.Parameters.AddWithValue("@p2", i);
                        command2.Parameters.AddWithValue("@p3", GenerateNames());
                        command2.Parameters.AddWithValue("@p4", farm);
                        command2.Parameters.AddWithValue("@p5", craft);
                        command2.Parameters.AddWithValue("@p6", dealer);
                        command2.Parameters.AddWithValue("@p7", farm);
                        command2.Parameters.AddWithValue("@p8", craft);
                        command2.Parameters.AddWithValue("@p9", dealer);
                        command2.Parameters.AddWithValue("@p10", Convert.ToInt32(vars["level"]));
                        command2.Parameters.AddWithValue("@p11", Convert.ToInt32(vars["immune"]));
                        command2.Parameters.AddWithValue("@p12", Convert.ToDecimal(vars["pol_stab"]));
                        command2.Parameters.AddWithValue("@p13", Convert.ToInt32(vars["ini_x"]));
                        command2.Parameters.AddWithValue("@p14", Convert.ToInt32(vars["ini_y"]));
                        command2.Parameters.AddWithValue("@p15", Convert.ToInt32(vars["ini_gold"]));
                        command2.Parameters.AddWithValue("@p16", reg_box);
                        command2.Parameters.AddWithValue("@p17", Convert.ToInt32(vars["ini"]));
                        command2.Parameters.AddWithValue("@p18", box_id);
                        command2.Parameters.AddWithValue("@p19", Convert.ToDecimal(vars["def_fact"]));
                        command2.Parameters.AddWithValue("@p20", Convert.ToInt32(Math.Round(defence)));
                        command2.Parameters.AddWithValue("@p21", revenue);
                        command2.Parameters.AddWithValue("@p22", textBox3.Text);
                        command2.ExecuteNonQuery();
                        connection2.Close();
                    }
                    else
                    {
                        int arxiki_perioxi = num_cit - (Convert.ToInt32(vars["ini_regions"]) - 1);
                        Random randomizer1 = new Random();
                        int p = Convert.ToInt32(vars["reg_dev_ab"]) - Convert.ToInt32(vars["dev_margin"]) + randomizer1.Next(0, 20);
                        int craft = p * (Convert.ToInt32(vars["craft_per"]) - Convert.ToInt32(vars["dev_margin"]) + randomizer1.Next(0, 20)) / Convert.ToInt32(vars["reg_dev_ab"]);
                        int dealer = p * (Convert.ToInt32(vars["dealer_per"]) - Convert.ToInt32(vars["dev_margin"]) + randomizer1.Next(0, 20)) / Convert.ToInt32(vars["reg_dev_ab"]);
                        int farm = p - craft - dealer;
                        defence = Math.Round(Convert.ToDecimal(vars["def_fact"]) * (farm + craft + dealer) + offence);
                        revenue = (farm * Convert.ToInt32(vars["farm_production"])) + (craft * Convert.ToInt32(vars["craft_production"])) + (dealer * Convert.ToInt32(vars["dealer_production"]));

                        MySqlConnection connection2 = new MySqlConnection(sqlcon);
                        connection2.Open();
                        MySqlCommand command2 = connection2.CreateCommand();
                        MySqlDataReader reader1;//Create prepared statement
                        command2.CommandText = "SELECT * FROM regions where id=@p2";
                        command2.Prepare();
                        command2.Parameters.AddWithValue("@p2", arxiki_perioxi);
                        reader1 = command2.ExecuteReader();
                        reader1.Read();
                        string temp6 = reader1["x"].ToString();
                        string temp7 = reader1["y"].ToString();

                        int sx = Convert.ToInt32(temp6);
                        int sy = Convert.ToInt32(temp7);
                        int x = 0;
                        int y = 0;

                        if (Math.Abs(sx) < Math.Abs(sy))
                        {
                            x = sx - 2 * Convert.ToInt32(vars["diaspora"]) * sy / Math.Abs(sy);
                            y = sy;
                        }
                        else
                        {
                            x = sx;
                            y = sy + 2 * Convert.ToInt32(vars["diaspora"]) * sx / Math.Abs(sx);
                        }

                        if (sx == sy)
                        {
                            x = sx - 2 * Convert.ToInt32(vars["diaspora"]) * sy / Math.Abs(sy);
                            y = sy;
                        }

                        if ((Math.Abs(sx) == Math.Abs(sy)) && sy > 0 && sx < 0)
                        {
                            x = sx;
                            y = sy - 2 * Convert.ToInt32(vars["diaspora"]);
                        }

                        if ((Math.Abs(sx) == Math.Abs(sy)) && sy < 0 && sx > 0)
                        {
                            x = sx + 2 * Convert.ToInt32(vars["diaspora"]);
                            y = sy;
                        }
                        reader1.Close();
                        //elegxos perioxis
                        int territory_exists = 0;

                        command2.CommandText = "SELECT * FROM regions where x=@p3 AND y=@p4";
                        command2.Prepare();
                        command2.Parameters.AddWithValue("@p3", x);
                        command2.Parameters.AddWithValue("@p4", y);
                        reader1 = command2.ExecuteReader();

                        if (reader1.Read())
                        {
                            territory_exists = 1;
                        }
                        reader1.Close();

                        if (territory_exists == 0)
                        {
                            command2.CommandText = "insert into regions (id,name,owner,farmcon,craftcon,dealercon,farm,craft,dealer,defence,level,revenue,immune,pol_stab,x,y,gold,reg_box,ini,box_id,def_fact) values (@p5, @p6, @p23, @p7, @p8, @p9, @p10, @p11, @p12, @p20, @p13, @p21, @p14, @p15, @p3, @p4, @p22, @p16, @p17, @p18, @p19)";
                            command2.Prepare();
                            command2.Parameters.AddWithValue("@p5", i);
                            command2.Parameters.AddWithValue("@p6", GenerateNames());
                            command2.Parameters.AddWithValue("@p7", farm);
                            command2.Parameters.AddWithValue("@p8", craft);
                            command2.Parameters.AddWithValue("@p9", dealer);
                            command2.Parameters.AddWithValue("@p10", farm);
                            command2.Parameters.AddWithValue("@p11", craft);
                            command2.Parameters.AddWithValue("@p12", dealer);
                            command2.Parameters.AddWithValue("@p13", Convert.ToInt32(vars["level"]));
                            command2.Parameters.AddWithValue("@p14", Convert.ToInt32(vars["immune"]));
                            command2.Parameters.AddWithValue("@p15", Convert.ToDecimal(vars["pol_stab"]));
                            command2.Parameters.AddWithValue("@p16", reg_box);
                            command2.Parameters.AddWithValue("@p17", Convert.ToInt32(vars["ini"]));
                            command2.Parameters.AddWithValue("@p18", box_id);
                            command2.Parameters.AddWithValue("@p19", Convert.ToDecimal(vars["def_fact"]));
                            command2.Parameters.AddWithValue("@p20", defence);
                            command2.Parameters.AddWithValue("@p21", revenue);
                            command2.Parameters.AddWithValue("@p22", Convert.ToInt32(vars["ini_gold"]));
                            command2.Parameters.AddWithValue("@p23", textBox3.Text);
                            command2.ExecuteNonQuery();
                            connection2.Close();
                        }
                        else
                        {
                            i = i - 1;
                        }
                    }
                }
                else
                {
                    int arxiki = num_cit + 1;
                    box_id++;
                    Random randomizer1 = new Random();
                    int p = Convert.ToInt32(vars["reg_dev_ab"]) - Convert.ToInt32(vars["dev_margin"]) + randomizer1.Next(0, 20);
                    int craft = p * (Convert.ToInt32(vars["craft_per"]) - Convert.ToInt32(vars["dev_margin"]) + randomizer1.Next(0, 20)) / Convert.ToInt32(vars["reg_dev_ab"]);
                    int dealer = p * (Convert.ToInt32(vars["dealer_per"]) - Convert.ToInt32(vars["dev_margin"]) + randomizer1.Next(0, 20)) / Convert.ToInt32(vars["reg_dev_ab"]);
                    int farm = p - craft - dealer;
                    defence = Math.Round(Convert.ToDecimal(vars["def_fact"]) * (farm + craft + dealer) + offence);
                    revenue = (farm * Convert.ToInt32(vars["farm_production"])) + (craft * Convert.ToInt32(vars["craft_production"])) + (dealer * Convert.ToInt32(vars["dealer_production"]));

                    MySqlConnection connection3 = new MySqlConnection(sqlcon);
                    connection3.Open();
                    MySqlCommand command3 = connection3.CreateCommand();
                    MySqlDataReader reader2;//Create prepared statement
                    command3.CommandText = "SELECT * FROM regions where id=@p4";
                    command3.Prepare();
                    command3.Parameters.AddWithValue("@p4", arxiki);
                    reader2 = command3.ExecuteReader();
                    reader2.Read();
                    string temp4 = reader2["x"].ToString();
                    string temp5 = reader2["y"].ToString();
                    Random randomizer = new Random();
                    int x = Convert.ToInt32(temp4) + (randomizer.Next(0, 20) - Convert.ToInt32(vars["ini_regions"]));
                    int y = Convert.ToInt32(temp5) + (randomizer.Next(0, 20) - Convert.ToInt32(vars["ini_regions"]));
                    reader2.Close();

                    int territory_exists = 0;

                    command3.CommandText = "SELECT * FROM regions where x=@p5 AND y=@p6";
                    command3.Prepare();
                    command3.Parameters.AddWithValue("@p5", x);
                    command3.Parameters.AddWithValue("@p6", y);
                    reader2 = command3.ExecuteReader();

                    if (reader2.Read())
                    {
                        territory_exists = 1;
                    }
                    reader2.Close();

                    if (territory_exists == 0)
                    {
                        command3.CommandText = "insert into regions (id,name,farmcon,craftcon,dealercon,farm,craft,dealer,defence,level,revenue,immune,pol_stab,x,y,gold,reg_box,box_id, def_fact) values (@p7, @p10, @p12, @p13, @p14, @p15, @p16, @p17, @p21, @p8, @p22, @p9, @p11, @p5, @p6, @p23, @p18, @p19, @p20)";
                        command3.Prepare();
                        command3.Parameters.AddWithValue("@p7", i);
                        command3.Parameters.AddWithValue("@p10", GenerateNames());
                        command3.Parameters.AddWithValue("@p12", farm);
                        command3.Parameters.AddWithValue("@p13", craft);
                        command3.Parameters.AddWithValue("@p14", dealer);
                        command3.Parameters.AddWithValue("@p15", farm);
                        command3.Parameters.AddWithValue("@p16", craft);
                        command3.Parameters.AddWithValue("@p17", dealer);
                        command3.Parameters.AddWithValue("@p8", Convert.ToInt32(vars["level"]));
                        command3.Parameters.AddWithValue("@p9", Convert.ToInt32(vars["immune"]));
                        command3.Parameters.AddWithValue("@p11", Convert.ToDecimal(vars["pol_stab"]));
                        command3.Parameters.AddWithValue("@p18", reg_box);
                        command3.Parameters.AddWithValue("@p19", box_id);
                        command3.Parameters.AddWithValue("@p20", Convert.ToDecimal(vars["def_fact"]));
                        command3.Parameters.AddWithValue("@p21", defence);
                        command3.Parameters.AddWithValue("@p22", revenue);
                        command3.Parameters.AddWithValue("@p23", Convert.ToInt32(vars["ini_gold"]));
                        command3.ExecuteNonQuery();
                        connection3.Close();
                    }
                    else
                    {
                        i = i - 1;
                        box_id--;
                    }
                }
            }
        }

        public void SumCalculate()
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            MySqlCommand command2 = new MySqlCommand("SELECT COUNT(*) FROM player", connection);
            string temp18 = command2.ExecuteScalar().ToString();
            int num_player = Convert.ToInt32(temp18);

            for (int j = 1; j <= num_player; j++)
            {
                int flag = 0;
                int flag1 = 0;
                int sum_rev = 0;
                int sum_cost = 0;

                command.CommandText = "SELECT * FROM player where id=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", j);
                reader = command.ExecuteReader();
                reader.Read();
                string temp19 = reader["username"].ToString();
                string temp20 = reader["gold"].ToString();
                int gold = Convert.ToInt32(temp20);
                reader.Close();

                MySqlCommand command3 = new MySqlCommand("SELECT SUM(revenue) FROM regions where owner='" + temp19 + "'", connection);
                reader = command3.ExecuteReader();
                if (reader.Read())
                {
                    if (reader[0] != System.DBNull.Value)
                    {
                        sum_rev = Convert.ToInt32(reader[0]);
                        flag = 1;
                    }
                }
                reader.Close();

                MySqlCommand command4 = new MySqlCommand("SELECT SUM(cost) FROM regions where owner='" + temp19 + "'", connection);
                reader = command4.ExecuteReader();
                if (reader.Read())
                {
                    if (reader[0] != System.DBNull.Value)
                    {
                        sum_cost = Convert.ToInt32(reader[0]);
                        flag1 = 1;
                    }
                }
                reader.Close();
                if (flag == 1)
                {
                    command.CommandText = "UPDATE player SET rev_sum=@p2 Where id=@p1 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p2", sum_rev);
                    command.ExecuteNonQuery();
                }

                if (flag1 == 1)
                {
                    command.CommandText = "UPDATE player SET cost_sum=@p3 Where id=@p1 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p3", sum_cost);
                    command.ExecuteNonQuery();
                }

                command.CommandText = "UPDATE player SET gold=@p4 Where id=@p1 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p4", gold + (sum_rev - sum_cost));
                command.ExecuteNonQuery();

                command.Parameters.Clear();
            }
            connection.Close();

        }

        public string GenerateNames()
        {
            List<string> PlaceNames = new List<string>()
                {
                    "Bell",
                    "Char",
                    "Harl",
                    "Good",
                    "Rei",
                    "Bran",
                    "Cul",
                    "Lang",
                    "Darf",
                    "Fith",
                    "Old",
                    "Cub",
                    "Bur",
                    "Fas",
                    "Long",
                    "River",
                    "Sea",
                    "Big",
                    "Small",
                    "Light"
                };

            List<string> PlaceNamesA = new List<string>()
                {
                    "Lake",
                    "Wish",
                    "Perl",
                    "Eagl",
                    "Yul",
                    "Clap",
                    "Bark",
                    "Ham",
                    "Dom",
                    "Ram",
                    "Vill",
                    "Ford",
                    "Worth",
                    "Wood",
                    "Iron",
                    "Strong",
                    "Hold",
                    "Mill",
                    "Ship",
                    "Dorn"
                };

            List<string> PlaceNamesB = new List<string>()
                {
                    "Art",
                    "Darw",
                    "Calf",
                    "Swan",
                    "Black",
                    "Water",
                    "Storm",
                    "Sly",
                    "Born",
                    "Far",
                    "Dragon",
                    "Pierce",
                    "Lyon",
                    "Bell",
                    "Stone",
                    "King",
                    "Queen",
                    "Spark",
                    "High",
                    "Ray",
                };

            List<string> PlaceNamesC = new List<string>()
                {
                    "Horn",
                    "Lake",
                    "Well",
                    "Hard",
                    "Arch",
                    "Gold",
                    "Silver",
                    "Head",
                    "Fort",
                    "Elf",
                    "Keep",
                    "Oak",
                    "Rise",
                    "Heart",
                    "Leg",
                    "Shin",
                    "Sand",
                    "Beach",
                    "Sword",
                    "Rock"
                };

            var permutations = new List<Tuple<int, int, int, int>>();
            List<string> generatedNames = new List<string>();

            Random random = new Random();
            int a, b, c, d;

            //We want to generate 500 names.
            //while (permutations.Count < 10)
            //{
            int flag = 0;
            while (flag != 1)
            {
                a = random.Next(0, PlaceNames.Count);
                b = random.Next(0, PlaceNamesA.Count);
                c = random.Next(0, PlaceNamesB.Count);
                d = random.Next(0, PlaceNamesC.Count);

                Tuple<int, int, int, int> tuple = new Tuple<int, int, int, int>(a, b, c, d);
                permutations.Add(tuple);

                //}

                //foreach (var tuple in permutations)
                //{
                Random rnd = new Random();
                int num = rnd.Next(1, 4);

                if (num == 1)
                {
                    generatedNames.Add(string.Format("{0}{1} {2}{3}", PlaceNames[tuple.Item1], PlaceNamesA[tuple.Item2].ToLower(), PlaceNamesB[tuple.Item3], PlaceNamesC[tuple.Item4].ToLower()));
                }
                else if (num == 2)
                {
                    generatedNames.Add(string.Format("{0}{1} {2}{3}", PlaceNames[tuple.Item2], PlaceNamesA[tuple.Item1].ToLower(), PlaceNamesB[tuple.Item4], PlaceNamesC[tuple.Item3].ToLower()));
                }
                else if (num == 3)
                {
                    generatedNames.Add(string.Format("{0}{1} {2}{3}", PlaceNames[tuple.Item3], PlaceNamesA[tuple.Item1].ToLower(), PlaceNamesB[tuple.Item2], PlaceNamesC[tuple.Item4].ToLower()));
                }
                else if (num == 4)
                {
                    generatedNames.Add(string.Format("{0}{1} {2}{3}", PlaceNames[tuple.Item4], PlaceNamesA[tuple.Item2].ToLower(), PlaceNamesB[tuple.Item1], PlaceNamesC[tuple.Item3].ToLower()));
                }


                //}

                //foreach (var n in generatedNames)
                //{
                //    string name = n;
                //}
                MySqlConnection connection3 = new MySqlConnection(sqlcon);
                connection3.Open();
                MySqlCommand command3 = connection3.CreateCommand();
                MySqlDataReader reader2;//Create prepared statement
                command3.CommandText = "SELECT * FROM regions where name=@p4";
                command3.Prepare();
                command3.Parameters.AddWithValue("@p4", generatedNames[0]);
                reader2 = command3.ExecuteReader();
                reader2.Read();

                if (reader2.Read())//αν έχω βρει αποτέλεσμα
                {
                    generatedNames.RemoveAt(0);
                }
                else
                {
                    flag = 1;
                    reader2.Close();
                    connection3.Close();
                }

            }

            return generatedNames[0];
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "Image files | *.jpg";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.BackgroundImage = Image.FromFile(openFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
