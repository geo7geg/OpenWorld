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
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Threading;
using MoreLinq;
using System.IO;

namespace StrategyGame
{
    public partial class Form1 : Form
    {
        int zoom = 1;
        int Panx = 0;
        int Pany = 0;
        string sqlcon = Variables.sqlstring;
        Point _mousePt = new Point();
        bool _tracking = false;
        TcpClient clientSocket = new TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;
        int XOff1 = 400;
        int Yoff1 = 400;
        Dictionary<string, string> vars = Variables.vars();
        
        public Form1(string username)
        {
            InitializeComponent();
            this.CenterToScreen();
            //flowLayoutPanel1.VerticalScroll.Visible = false;
            //flowLayoutPanel1.HorizontalScroll.Visible = false;
            
            //this.MouseWheel += new MouseEventHandler(Panel1_MouseWheel);               
            //display.BackColor = Color.Transparent;
            display2.MouseWheel += new MouseEventHandler(pictureBox1_MouseWheel);
            //display2.MouseHover += new EventHandler(pictureBox1_MouseHover);
            //display2.MouseWheel += new MouseEventHandler(display2_MouseWheel);
            label33.Text = username;
            label33.Text = label33.Text.ToUpper();
            
            textBox23.Text = username;
            textBox23.Visible = false;

            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT username FROM player",connection);
            DataSet ds = new DataSet();
            adapter.Fill(ds, "player");
            comboBox1.DisplayMember = "username";
            comboBox1.ValueMember = "username";
            comboBox1.DataSource = ds.Tables["player"];

            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT picture FROM player where username=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", textBox23.Text);
            reader = command.ExecuteReader();
            if(reader.Read())
            {
                byte[] img = (byte[])reader["picture"];
                if (img.Length != 0)
                {
                    MemoryStream ms = new MemoryStream(img);
                    pictureBox2.BackgroundImage = Image.FromStream(ms); 
                }
            }

            reader.Close();
            reader.Dispose();

            connection.Close();

            listView1.View = View.List;
            listView1.View = View.Details;
            listView1.Columns.Add("Επιτιθέμενος", 110, HorizontalAlignment.Left);
            listView1.Columns.Add("Αμυνόμενος", 110, HorizontalAlignment.Left);
            listView1.Columns.Add("Γύροι", 45, HorizontalAlignment.Left);
            listView1.Columns.Add("Άμυνα", 50, HorizontalAlignment.Left);
            listView1.Columns.Add("Επίθεση", 50, HorizontalAlignment.Left);  
            listView1.Columns.Add("Στρατός", 45, HorizontalAlignment.Left);
            
            listView2.View = View.List;
            listView2.View = View.Details;
            listView2.Columns.Add("Επιτιθέμενος", 110, HorizontalAlignment.Left);
            listView2.Columns.Add("Αμυνόμενος", 110, HorizontalAlignment.Left);
            listView2.Columns.Add("Γύροι", 45, HorizontalAlignment.Left);
            listView2.Columns.Add("Άμυνα", 50, HorizontalAlignment.Left);
            listView2.Columns.Add("Επίθεση", 50, HorizontalAlignment.Left); 
            listView2.Columns.Add("Στρατός", 45, HorizontalAlignment.Left);
           
            listView3.View = View.List;
            listView3.View = View.Details;
            listView3.Columns.Add("Όνομα", 80, HorizontalAlignment.Left);
            listView3.Columns.Add("Ιδιοκτήτης", 70, HorizontalAlignment.Left);
            listView3.Columns.Add("Άμυνα", 70, HorizontalAlignment.Left);
            listView3.Columns.Add("Επίθεση", 70, HorizontalAlignment.Left);
            listView3.Columns.Add("Στρατός", 70, HorizontalAlignment.Left);
            listView3.Columns.Add("Χρυσός", 70, HorizontalAlignment.Left);
            listView3.Columns.Add("Αγρότες", 70, HorizontalAlignment.Left);
            listView3.Columns.Add("Τεχνίτες", 70, HorizontalAlignment.Left);
            listView3.Columns.Add("Έμποροι", 70, HorizontalAlignment.Left);

            listView4.View = View.Details;
    
            DrawMap();
            FillList();
            FillAttack();
            fillevents();
            PlayerGold();
            check_sunaspismos();

            //readData = "Connected to Chat Server ...";
            //msg();
            //clientSocket.Connect("192.168.1.208", 8888);
            //serverStream = clientSocket.GetStream();

            //byte[] outStream = System.Text.Encoding.ASCII.GetBytes(textBox23.Text + "$");
            //serverStream.Write(outStream, 0, outStream.Length);
            //serverStream.Flush();

            //Thread ctThread = new Thread(getMessage); 
            //ctThread.IsBackground = true;
            //ctThread.Start();
        }

        public void DrawMap()
        {
            display2.Controls.Clear();
            
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM regions", connection);
            string temp3 = command.ExecuteScalar().ToString();
            int num_cit = Convert.ToInt32(temp3);
            MySqlDataReader reader;

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
            //PictureBox display2 = new PictureBox();
            //Bitmap bmp = new Bitmap(800, 800);
            //Graphics g = Graphics.FromImage(bmp);
            //display2.Width = 800;
            //display2.Height = 800;
            display2.Location = new Point(300, 0);
            //display2.BackgroundImage = Image.FromFile(@"C:\Users\" + Environment.UserName + @"\Desktop\middle.jpg");
            //display2.ImageLocation = @"C:\Users\" + Environment.UserName + @"\Desktop\middle.jpg";
            display2.SizeMode = PictureBoxSizeMode.StretchImage;
            //display2.Image = bmp;

            for (int j = 1; j <= num_cit; j++)
            {
                command.CommandText = "SELECT * FROM regions where id=@p1";
                command.Prepare();
                //και τις παραμέτρους
                command.Parameters.AddWithValue("@p1", j);
                reader = command.ExecuteReader();

                string temp1 = "";
                string temp2 = "";
                string temp4 = "";
                if (reader.Read())//αν έχω βρει αποτέλεσμα
                {

                    temp1 = reader["x"].ToString();
                    temp2 = reader["y"].ToString();
                    temp3 = reader["name"].ToString();
                    temp4 = reader["owner"].ToString();
                }

                int x = Convert.ToInt32(temp1);
                int y = Convert.ToInt32(temp2);
                //int x2 = 0;
                //int y2 = 0;           

                //g.DrawLine (new Pen(Color.Red, 2), 5,250, 300,250);
                //let's draw a coordinate equivalent to (20,30) (20 up, 30 across)


                if (zoom <= 6 && zoom >= 0)
                {
                    //g.DrawString("x", new Font("Calibri", 15), new SolidBrush(Color.Black), (XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y)));
                    Label LB = new Label();
                    LB.Name = temp3;
                    //LB.Name = Convert.ToString(j);
                    LB.Location = new Point((XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y)));
                    LB.Size = new Size(26, 32);
                    LB.BackColor = Color.Transparent;
                    LB.Font = new Font("Calibri", 15, FontStyle.Bold);
                    //LB.Text = "X";
                    if (temp4 == textBox23.Text)
                    {
                        LB.Image = new Bitmap(WindowsFormsApplication2.Properties.Resources.tower2);
                    }
                    else
                    {
                        LB.Image = new Bitmap(WindowsFormsApplication2.Properties.Resources.tower);
                    }
                    LB.Click += new EventHandler(LB_Click);
                    LB.DoubleClick += new EventHandler(LB_DoubleClick);//assign click handler
                    LB.MouseHover += new EventHandler(LB_MouseHover);
                    LB.MouseLeave += new EventHandler(LB_MouseLeave);
                    //LB.Text = temp3;
                    display2.Controls.Add(LB);
                    //labels[j-1].Text = "X";
                    //labels[j-1].Location = new Point((XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y)));
                }
                else if (zoom < 0)
                {
                    zoom = 0;
                    Label LB = new Label();
                    LB.Name = temp3;
                    LB.Location = new Point((XOff1 + (Panx + x)), (Yoff1 - (Pany + y)));
                    LB.Size = new Size(20, 20);
                    LB.BackColor = Color.Transparent;
                    LB.Font = new Font("Calibri", 15, FontStyle.Bold);
                    if (temp4 == textBox23.Text)
                    {
                        LB.Image = new Bitmap(WindowsFormsApplication2.Properties.Resources.tower2);
                    }
                    else
                    {
                        LB.Image = new Bitmap(WindowsFormsApplication2.Properties.Resources.tower);
                    }
                    LB.Text = "X";
                    LB.Click += new EventHandler(LB_Click); //assign click handler
                    LB.DoubleClick += new EventHandler(LB_DoubleClick);//assign click handler
                    LB.MouseHover += new EventHandler(LB_MouseHover);
                    LB.MouseLeave += new EventHandler(LB_MouseLeave);

                    display2.Controls.Add(LB);
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
                    LB.Size = new Size(32, 37);
                    LB.BackColor = Color.Transparent;
                    LB.Font = new Font("Calibri", 15, FontStyle.Bold);
                    if (temp4 == textBox23.Text)
                    {
                        LB.Image = new Bitmap(WindowsFormsApplication2.Properties.Resources.tower2);
                    }
                    else
                    {
                        LB.Image = new Bitmap(WindowsFormsApplication2.Properties.Resources.tower);
                    }
                    //LB.Text = temp3;
                    LB.Click += new EventHandler(LB_Click); //assign click handler
                    LB.DoubleClick += new EventHandler(LB_DoubleClick);//assign click handler
                    LB.MouseHover += new EventHandler(LB_MouseHover);
                    LB.MouseLeave += new EventHandler(LB_MouseLeave);

                    display2.Controls.Add(LB);
                    //g.DrawString(temp3, new Font("Calibri", 8), new SolidBrush(Color.Black), (XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y)));
                    //display2.Controls.Add(new Label { Text = temp3, Font = new Font("Calibri", 15, FontStyle.Bold), Height = 20, Width = 20, Name = "lable" + j, BackColor = Color.Transparent, Location = new Point((XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y))) });
                    //labels[j-1].Text = "X";
                    //labels[j-1].Location = new Point((XOff1 + zoom * (Panx + x)), (Yoff1 - zoom * (Pany + y)));
                }
                //g.DrawString("O", new Font("Calibri", 20), new SolidBrush(Color.Black), (XOff + x), (YOff - y));
                //double distance = Math.Round(Math.Sqrt((Math.Pow(x - x2, 2) + Math.Pow(y - y2, 2))));
                //g.DrawString(distance.ToString(), new Font("Calibri", 40), new SolidBrush(Color.Black), 0, 0);
                reader.Close();
                command.Parameters.Clear();
            }
            
            connection.Close();
      
            flowLayoutPanel1.Refresh();
            Application.DoEvents();
        }

        private void button1_Click(object sender, EventArgs e)
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
                        command2.CommandText = "insert into regions (id,name,farmcon,craftcon,dealercon,farm,craft,dealer,defence,level,revenue,immune,pol_stab,x,y,gold,reg_box,ini,box_id,def_fact) values (@p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p20, @p10, @p21, @p11, @p12, @p13, @p14, @p15, @p16, @p17, @p18, @p19)";
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
                        MySqlDataReader reader1;

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
                            command2.CommandText = "insert into regions (id,name,farmcon,craftcon,dealercon,farm,craft,dealer,defence,level,revenue,immune,pol_stab,x,y,gold,reg_box,ini,box_id,def_fact) values (@p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p20, @p13, @p21, @p14, @p15, @p3, @p4, @p22, @p16, @p17, @p18, @p19)";
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
                    MySqlDataReader reader2;

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

            DrawMap();
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

                MySqlConnection connection3 = new MySqlConnection(sqlcon);
                connection3.Open();
                MySqlCommand command3 = connection3.CreateCommand();
                MySqlDataReader reader2;

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

        private void button2_Click(object sender, EventArgs e)
        {
            if (zoom >= 10)
            {
                zoom = 10;
                display2.Width = 2800;
                display2.Height = 2800;
                XOff1 = display2.Width / 2;
                Yoff1 = display2.Height / 2;
                display2.Location = new Point(300, 0);
            }
            else
            {
                zoom += 1;
                display2.Width += 200;
                display2.Height += 200;
                XOff1 = display2.Width / 2;
                Yoff1 = display2.Height / 2;
                display2.Location = new Point(300, 0);
            }

            DrawMap();
            //display2.Height += 10;
            //display2.Width += 10;        
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (zoom <= 0)
            {
                zoom = 0;
                display2.Width = 800;
                display2.Height = 800;
                XOff1 = display2.Width / 2;
                Yoff1 = display2.Height / 2;
                display2.Location = new Point(300, 0);
            }
            else
            {
                zoom -= 1;
                display2.Width -= 200;
                display2.Height -= 200;
                XOff1 = display2.Width / 2;
                Yoff1 = display2.Height / 2;
                display2.Location = new Point(300, 0);
            }
            DrawMap();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Panx++;
            DrawMap();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Panx--;
            DrawMap();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Pany++;
            DrawMap();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Pany--;
            DrawMap();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DrawMap();
        }

        public void NewTurn(int id)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM attacks where id=@p17";
            command.Prepare();
            command.Parameters.AddWithValue("@p17", id);
            reader = command.ExecuteReader();
            reader.Read();

            string temp = reader["att_region"].ToString();
            string temp1 = reader["def_region"].ToString();
            int distance = Convert.ToInt32(reader["distance"]);
            int turn = Convert.ToInt32(reader["turn"]);
            int army = Convert.ToInt32(reader["army"]);
            int army_sunasp = Convert.ToInt32(reader["army_sunasp"]);
            string sunaspismos = reader["sunaspismos"].ToString();
            int army_enwsi = Convert.ToInt32(reader["army_enwsi"]);
            string enwsi = reader["enwsi"].ToString();
            string army_coplayer = reader["army_coplayer"].ToString();
            string region_coplayer = reader["region_coplayer"].ToString();
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

            reader.Close();
            
            command.CommandText = "SELECT * FROM regions where name=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", temp);
            reader = command.ExecuteReader();
            reader.Read();
               
            string att_name = reader["owner"].ToString();
            int att_off = Convert.ToInt32(reader["offense"]);
            int att_army = Convert.ToInt32(reader["army"]);
            int att_cost = Convert.ToInt32(reader["cost"]);
            int att_farm = Convert.ToInt32(reader["farm"]);
            int att_craft = Convert.ToInt32(reader["craft"]);
            int att_dealer = Convert.ToInt32(reader["dealer"]);
            int att_revenue = Convert.ToInt32(reader["revenue"]);
            decimal att_def_fact = Convert.ToDecimal(reader["def_fact"]);
            reader.Close();

            command.CommandText = "SELECT * FROM regions where name=@p2";
            command.Prepare();
            command.Parameters.AddWithValue("@p2", temp1);
            reader = command.ExecuteReader();
            reader.Read();

            int def_id = Convert.ToInt32(reader["id"]);
            string def_name = reader["owner"].ToString();
            string def_reg_name = reader["name"].ToString();
            int farm_con = Convert.ToInt32(reader["farmcon"]);
            int craft_con = Convert.ToInt32(reader["craftcon"]);
            int dealer_con = Convert.ToInt32(reader["dealercon"]);
            int def_farm = Convert.ToInt32(reader["farm"]);
            int def_craft = Convert.ToInt32(reader["craft"]);
            int def_dealer = Convert.ToInt32(reader["dealer"]);
            int def_cost = Convert.ToInt32(reader["cost"]);
            int def_army = Convert.ToInt32(reader["army"]);
            int level = Convert.ToInt32(reader["level"]);
            decimal def_def_fact = Convert.ToDecimal(reader["def_fact"]);
            
            reader.Close();

            command.CommandText = "SELECT * FROM player where username='" + att_name + "'";
            command.Prepare();
            reader = command.ExecuteReader();
            reader.Read();
            decimal att_military = Convert.ToDecimal(reader["military"]);
            int army_debt_sunasp = Convert.ToInt32(reader["army_debt_sunasp"]);
            int army_debt_enwsi = Convert.ToInt32(reader["army_debt_enwsi"]);
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
            if(reader.Read())
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

            int def_off = Convert.ToInt32(Math.Round(def_army * (def_military * Convert.ToDecimal(vars["off_fact"]) + 1)));
            int def_def = Convert.ToInt32(Math.Round(def_def_fact * (def_farm + def_craft + def_dealer) + def_off));
            
            int att_offense = 0;
            
            att_offense = Convert.ToInt32(Math.Round((army + army_sunasp + army_enwsi + sum_co_army) * (att_military * Convert.ToDecimal(vars["off_fact"]) + 1)));

            command.CommandText = "SELECT * FROM def_othomanoi where name='" + def_reg_name + "'";
            command.Prepare();
            reader = command.ExecuteReader();
            int factor1 = 0;
            int flag = 0;
            if(reader.Read())
            {
                factor1 = Convert.ToInt32(reader["factor"]);
                flag = Convert.ToInt32(reader["flag"]);
                if(flag == 1)
                {
                    def_def = def_def * factor1;
                }  
            }
            reader.Close();
            
            if (turn > 0)
            {
                turn = turn - 1;
                if (turn == 0)
                { 
                    if (def_def < att_offense)
                    {
                        int farm_max = 0;
                        int craft_max = 0;
                        int dealer_max = 0;

                        if (level <= 1)
                        {
                            farm_max = Convert.ToInt32(farm_con);
                            craft_max = Convert.ToInt32(craft_con);
                            dealer_max = Convert.ToInt32(dealer_con);
                        }
                        else
                        {
                            farm_max = Convert.ToInt32(farm_con + (farm_con * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                            craft_max = Convert.ToInt32(craft_con + (craft_con * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                            dealer_max = Convert.ToInt32(dealer_con + (dealer_con * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                        }
                        
                        int missing_population = (farm_max + craft_max + dealer_max) - (def_farm + def_craft + def_dealer);  
                        int change = 0;

                        if(missing_population > 0)
                        {
                            change = def_army - missing_population;
                            if(change > 0)
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
                                MySqlCommand command3 = new MySqlCommand("SELECT SUM(army) FROM regions where owner='" + def_name + "' and id!='" + def_id +  "'", connection);
                                reader = command3.ExecuteReader();
                                if (reader.Read())
                                {
                                    if (reader[0] != System.DBNull.Value)
                                    {
                                        sum_army = Convert.ToDecimal(reader[0]);
                                    }
                                }
                                reader.Close();

                                string com = "Select * from regions where owner='" + def_name + "' and id!='" + def_id +"'";
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

                                        int offense4 = Convert.ToInt32(Math.Round(army3 * (def_military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                                        decimal defence4 = Math.Round(def_fact * (farm3 + craft3 + dealer3) + offense4);

                                        command.CommandText = "UPDATE regions SET army=@p43, defence=@p44, cost=@p45, offense=@p46 Where name=@p47 ";
                                        command.Prepare();
                                        command.Parameters.AddWithValue("@p43", army3);
                                        command.Parameters.AddWithValue("@p44", Convert.ToInt32(Math.Round(defence4)));
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

                                            command.CommandText = "DELETE FROM attacks Where id='" + code + "'";
                                            command.Prepare();
                                            command.ExecuteNonQuery();

                                            command.Parameters.Clear();
                                        }
                                        break;
                                    }
                                }

                                if(missing_population > 0)
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
                                        //Variables.stop_attack(code1);        
                                    }
                                    //Variables.remove_army(def_name, missing_population);
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
                        //else
                        //{
                        //    free_army = def_army;
                        //}     
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

                        command.CommandText = "DELETE FROM attacks Where id='" + id + "'";
                        command.Prepare();
                        command.ExecuteNonQuery();
                            
                        command.CommandText = "DELETE FROM attacks Where att_region='" + def_reg_name + "'";
                        command.Prepare();
                        command.ExecuteNonQuery();

                        command.CommandText = "UPDATE attacks SET def_player=@p34 Where def_region=@p35 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p34", att_name);
                        command.Parameters.AddWithValue("@p35", def_reg_name);
                        command.ExecuteNonQuery();

                        command.CommandText = "insert into events (att_player,def_player,att_region,def_region,result,inserton,date) values (@p22, @p28, @p23, @p24, @p25, @p26, @p27)";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p22", att_name);
                        command.Parameters.AddWithValue("@p28", def_name);
                        command.Parameters.AddWithValue("@p23", temp);
                        command.Parameters.AddWithValue("@p24", def_reg_name);
                        command.Parameters.AddWithValue("@p25", 1);
                        command.Parameters.AddWithValue("@p26", DateTime.Now.TimeOfDay);
                        command.Parameters.AddWithValue("@p27", DateTime.Now.Date);
                        command.ExecuteNonQuery();

                        decimal d_att_farm = Convert.ToDecimal(att_farm);
                        decimal d_att_craft = Convert.ToDecimal(att_craft);
                        decimal d_att_dealer = Convert.ToDecimal(att_dealer);
                        decimal d_army = Convert.ToDecimal(army);
                        decimal d_def_def = Convert.ToDecimal(def_def);
                            
                        decimal military_bonus = (1 + ((d_army * att_military) / (d_att_farm + d_att_craft + d_att_dealer)) - (((d_army * att_military) - d_def_def) / (d_att_farm + d_att_craft + d_att_dealer)));
                            
                        decimal military_update = att_military + military_bonus;

                        command.CommandText = "UPDATE player SET military=@p30 Where username=@p31 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p30", military_update);
                        command.Parameters.AddWithValue("@p31", textBox23.Text);
                        command.ExecuteNonQuery();

                        int att_offense1 = Convert.ToInt32(Math.Round(army * (military_update * Convert.ToDecimal(vars["off_fact"]) + 1)));
                        int revenue = (farm_con * Convert.ToInt32(vars["farm_production"])) + (craft_con * Convert.ToInt32(vars["craft_production"])) + (dealer_con * Convert.ToInt32(vars["dealer_production"]));
                        decimal defence = Math.Round(att_def_fact * (farm_con + craft_con + dealer_con) + att_offense1);// to mhden einai to offence

                        command.CommandText = "UPDATE regions SET owner=@p8, farm=@p5, craft=@p6, dealer=@p7, army=@p4, defence=@p13, level=@p32, revenue=@p11, cost=@p15, offense=@p14, gold=@p16, def_fact=@p53 Where name=@p50 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p4", army);
                        command.Parameters.AddWithValue("@p5", farm_con);
                        command.Parameters.AddWithValue("@p6", craft_con);
                        command.Parameters.AddWithValue("@p7", dealer_con);
                        command.Parameters.AddWithValue("@p8", att_name);    
                        command.Parameters.AddWithValue("@p11", revenue);
                        command.Parameters.AddWithValue("@p13", Convert.ToInt32(Math.Round(defence)));
                        command.Parameters.AddWithValue("@p14", att_offense1);
                        command.Parameters.AddWithValue("@p15", army);
                        command.Parameters.AddWithValue("@p16", Convert.ToInt32(vars["ini_gold"]));
                        command.Parameters.AddWithValue("@p32", Convert.ToInt32(vars["level"]));
                        command.Parameters.AddWithValue("@p50", def_reg_name);
                        command.Parameters.AddWithValue("@p53", att_def_fact);
                        command.ExecuteNonQuery();

                        command.CommandText = "UPDATE player SET free_army=@p41 Where username=@p42 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p41", free_army + def_army);
                        command.Parameters.AddWithValue("@p42", def_name);
                        command.ExecuteNonQuery();

                        if (flag == 1)
                        {
                            command.CommandText = "UPDATE def_othomanoi SET flag=0 Where name=@p40 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p40", def_reg_name);
                            command.ExecuteNonQuery();
                        }

                        if (army_sunasp != 0)
                        {
                            command.Parameters.Clear();

                            command.CommandText = "SELECT * FROM sunaspismos where name=@p17";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p17", sunaspismos);
                            reader = command.ExecuteReader();
                            reader.Read();
                            int army_live = Convert.ToInt32(reader["army_live"]);
                            reader.Close();

                            command.CommandText = "UPDATE sunaspismos SET army_live=@p51 Where name=@p52 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p51", army_live + army_sunasp);
                            command.Parameters.AddWithValue("@p52", sunaspismos);
                            command.ExecuteNonQuery();
                        }

                        if (army_enwsi != 0)
                        {
                            command.Parameters.Clear();

                            command.CommandText = "SELECT * FROM enwsi where name=@p17";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p17", enwsi);
                            reader = command.ExecuteReader();
                            reader.Read();
                            int army_live = Convert.ToInt32(reader["army_live"]);
                            reader.Close();

                            command.CommandText = "UPDATE enwsi SET army_live=@p51 Where name=@p52 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p51", army_live + army_enwsi);
                            command.Parameters.AddWithValue("@p52", enwsi);
                            command.ExecuteNonQuery();
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

                        command.Parameters.Clear();

                        string com7 = "SELECT * FROM support_check where id='" + id + "'";
                        MySqlDataAdapter adpt7 = new MySqlDataAdapter(com7, connection);
                        DataSet myDataSet7 = new DataSet();
                        adpt7.Fill(myDataSet7, "attacks");
                        DataTable myDataTable7 = myDataSet7.Tables[0];
                        DataRow tempRow7 = null;

                        foreach (DataRow tempRow7_Variable in myDataTable7.Rows)
                        {
                            command.Parameters.Clear();

                            tempRow7 = tempRow7_Variable;

                            string supporter = tempRow7["supporter"].ToString();

                            command.CommandText = "DELETE FROM support_check Where id='" + id + "'";
                            command.Prepare();
                            command.ExecuteNonQuery();

                            command.CommandText = "UPDATE player SET diplomatic= diplomatic + 1 Where username=@p24 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p24", supporter);
                            command.ExecuteNonQuery();
                        }

                        listBox3.Items.Clear();
                        FillList();
                        DrawMap();
                        if (att_name == textBox23.Text)
                        {
                            MessageBox.Show("Συγχαρητήρια.Κατακτήσατε την περιοχή " + def_reg_name + " .");
                        }else if(def_name == textBox23.Text)
                        {
                            MessageBox.Show("Η μάχη για την περιοχή " + def_reg_name + " χάθηκε.");
                        }
                    }
                    else
                    {
                        int att_offense1 = Convert.ToInt32(Math.Round((army + att_army) * (att_military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                        decimal defence = Math.Round(att_def_fact * (att_farm + att_craft + att_dealer) + att_offense1);
                        command.CommandText = "UPDATE regions SET army=@p36, defence=@p37, cost=@p38, offense=@p39 Where name=@p33 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p36", army + att_army);
                        command.Parameters.AddWithValue("@p37", Convert.ToInt32(Math.Round(defence)));
                        command.Parameters.AddWithValue("@p38", army + att_army);
                        command.Parameters.AddWithValue("@p39", att_offense1);
                        command.Parameters.AddWithValue("@p33", temp);
                        command.ExecuteNonQuery();

                        command.CommandText = "DELETE FROM attacks Where id='" + id + "'";
                        command.Prepare();
                        command.ExecuteNonQuery();
                        //    command.CommandText = "UPDATE regions SET army=@p9 Where id=@p1 ";
                        //    command.Prepare();
                        //    command.Parameters.AddWithValue("@p9", att_army + army);
                        //    command.ExecuteNonQuery();


                        command.CommandText = "insert into events (att_player,def_player,att_region,def_region,result,inserton,date) values (@p22, @p28, @p23, @p24, @p25, @p26, @p27)";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p22", att_name);
                        command.Parameters.AddWithValue("@p28", def_name);
                        command.Parameters.AddWithValue("@p23", temp);
                        command.Parameters.AddWithValue("@p24", def_reg_name);
                        command.Parameters.AddWithValue("@p25", 0);
                        command.Parameters.AddWithValue("@p26", DateTime.Now.TimeOfDay);
                        command.Parameters.AddWithValue("@p27", DateTime.Now.Date);
                        command.ExecuteNonQuery();

                        if (army_sunasp != 0)
                        {
                            command.Parameters.Clear();

                            command.CommandText = "SELECT * FROM sunaspismos where name=@p53";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p53", sunaspismos);
                            reader = command.ExecuteReader();
                            reader.Read();
                            int army_live = Convert.ToInt32(reader["army_live"]);
                            reader.Close();

                            command.CommandText = "UPDATE sunaspismos SET army_live=@p51 Where name=@p52";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p51", army_live + army_sunasp);
                            command.Parameters.AddWithValue("@p52", sunaspismos);
                            command.ExecuteNonQuery();
                        }

                        if (army_enwsi != 0)
                        {
                            command.Parameters.Clear();

                            command.CommandText = "SELECT * FROM enwsi where name=@p17";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p17", enwsi);
                            reader = command.ExecuteReader();
                            reader.Read();
                            int army_live = Convert.ToInt32(reader["army_live"]);
                            reader.Close();

                            command.CommandText = "UPDATE enwsi SET army_live=@p51 Where name=@p52 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p51", army_live + army_enwsi);
                            command.Parameters.AddWithValue("@p52", enwsi);
                            command.ExecuteNonQuery();
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

                        command.Parameters.Clear();

                        command.CommandText = "SELECT * FROM support_check where id=@p23";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p23", id);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            string supporter = reader["supporter"].ToString();
                            reader.Close();

                            command.CommandText = "DELETE FROM support_check Where id='" + id + "'";
                            command.Prepare();
                            command.ExecuteNonQuery();

                            command.CommandText = "UPDATE player SET diplomatic= diplomatic + 1 Where username=@p24 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p24", supporter);
                            command.ExecuteNonQuery();
                        }
                        reader.Close();

                        if (att_name == textBox23.Text)
                        {
                            MessageBox.Show("Η μάχη για την περιοχή " + def_reg_name + " χάθηκε.");
                        }
                        else if (def_name == textBox23.Text)
                        {
                            MessageBox.Show("Η μάχη για την προστασία της περιοχής " + def_reg_name + " κερδίθηκε.");
                        }
                    }
                }
                command.CommandText = "UPDATE attacks SET turn=@p18 Where id='" + id + "'";
                command.Prepare();
                command.Parameters.AddWithValue("@p18", turn);
                command.ExecuteNonQuery();
            }
            command.Parameters.Clear();
            connection.Close();
        }

        public void NewTurn1(int id)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM attacks where id=@p22";
            command.Prepare();
            command.Parameters.AddWithValue("@p22", id);
            reader = command.ExecuteReader();
            reader.Read();
            string temp = reader["att_region"].ToString();
            string temp1 = reader["def_region"].ToString();
            int distance = Convert.ToInt32(reader["distance"]);
            int turn = Convert.ToInt32(reader["turn"]);
            int army = Convert.ToInt32(reader["army"]);
            reader.Close();

            command.CommandText = "SELECT * FROM regions where name=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", temp);
            reader = command.ExecuteReader();
            reader.Read();

            string att_name = reader["owner"].ToString();
            int att_off = Convert.ToInt32(reader["offense"]);
            int att_army = Convert.ToInt32(reader["army"]);
            int att_cost = Convert.ToInt32(reader["cost"]);
            int att_farm = Convert.ToInt32(reader["farm"]);
            int att_craft = Convert.ToInt32(reader["craft"]);
            int att_dealer = Convert.ToInt32(reader["dealer"]);
            reader.Close();

            command.CommandText = "SELECT * FROM regions where name=@p2";
            command.Prepare();
            command.Parameters.AddWithValue("@p2", temp1);
            reader = command.ExecuteReader();
            reader.Read();

            string def_name = reader["owner"].ToString();
            string def_reg_name = reader["name"].ToString();
            int immune = Convert.ToInt32(reader["immune"]);
            int def_def = Convert.ToInt32(reader["defence"]);
            int farm_con = Convert.ToInt32(reader["farm"]);
            int craft_con = Convert.ToInt32(reader["craft"]);
            int dealer_con = Convert.ToInt32(reader["dealer"]);
            int def_cost = Convert.ToInt32(reader["cost"]);
            int def_army = Convert.ToInt32(reader["army"]);
            decimal def_def_fact = Convert.ToDecimal(reader["def_fact"]);
            reader.Close();

            if (turn > 0)
            {
                turn = turn - 1;
                if (turn == 0)
                {
                    if (def_name != "")
                    {
                        command.CommandText = "SELECT * FROM player where username=@p16";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p16", def_name);
                        reader = command.ExecuteReader();
                        reader.Read();
                        decimal military = Convert.ToDecimal(reader["military"]);
                        reader.Close();

                        int offense = Convert.ToInt32(Math.Round((army + def_army) * (military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                        decimal defence = Math.Round(def_def_fact * (farm_con + craft_con + dealer_con) + offense);

                        command.CommandText = "UPDATE regions SET army=@p5, defence=@p17, cost=@p18, offense=@p19 Where name=@p2 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p5", army + def_army);
                        command.Parameters.AddWithValue("@p17", Convert.ToInt32(Math.Round(defence)));
                        command.Parameters.AddWithValue("@p19", offense);
                        command.Parameters.AddWithValue("@p18", def_cost + army);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        int offense = army + def_army;
                        decimal defence = Math.Round(def_def_fact * (farm_con + craft_con + dealer_con) + offense);

                        command.CommandText = "UPDATE regions SET army=@p5, defence=@p17, cost=@p18, offense=@p19 Where name=@p2 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p5", army + def_army);
                        command.Parameters.AddWithValue("@p17", Convert.ToInt32(Math.Round(defence)));
                        command.Parameters.AddWithValue("@p19", offense);
                        command.Parameters.AddWithValue("@p18", def_cost + army);
                        command.ExecuteNonQuery();
                    }

                    command.CommandText = "SELECT * FROM support_check where id=@p23";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p23", id);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string supporter = reader["supporter"].ToString();
                        reader.Close();

                        command.CommandText = "DELETE FROM support_check Where id='" + id + "'";
                        command.Prepare();
                        command.ExecuteNonQuery();

                        command.CommandText = "UPDATE player SET diplomatic= diplomatic + 1 Where username=@p24 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p24", supporter);
                        command.ExecuteNonQuery();
                    }
                    reader.Close();
                    if (att_name == textBox23.Text)
                    {
                        MessageBox.Show("H κίνηση ολοκληρώθηκε. Υποστηρίξατε την περιοχή " + def_reg_name + " με " + army + " στρατιώτες .");
                    }
                }
                command.CommandText = "UPDATE attacks SET turn=@p21 Where id='" + id + "'";
                command.Prepare();
                command.Parameters.AddWithValue("@p21", turn);
                command.ExecuteNonQuery();
            }

            command.Parameters.Clear();
            connection.Close();
        }

        public void NewTurn2(int id)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM attacks where id=@p22";
            command.Prepare();
            command.Parameters.AddWithValue("@p22", id);
            reader = command.ExecuteReader();
            reader.Read();
            string temp = reader["att_region"].ToString();
            string temp1 = reader["def_region"].ToString();
            int distance = Convert.ToInt32(reader["distance"]);
            int turn = Convert.ToInt32(reader["turn"]);
            int army = Convert.ToInt32(reader["army"]);
            reader.Close();

            command.CommandText = "SELECT * FROM regions where name=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", temp);
            reader = command.ExecuteReader();
            reader.Read();

            string att_name = reader["owner"].ToString();
            int att_off = Convert.ToInt32(reader["offense"]);
            int att_army = Convert.ToInt32(reader["army"]);
            int att_cost = Convert.ToInt32(reader["cost"]);
            int att_farm = Convert.ToInt32(reader["farm"]);
            int att_craft = Convert.ToInt32(reader["craft"]);
            int att_dealer = Convert.ToInt32(reader["dealer"]);
            decimal off_def_fact = Convert.ToDecimal(reader["def_fact"]);
            reader.Close();

            command.CommandText = "SELECT * FROM regions where name=@p2";
            command.Prepare();
            command.Parameters.AddWithValue("@p2", temp1);
            reader = command.ExecuteReader();
            reader.Read();

            string def_name = reader["owner"].ToString();
            string def_reg_name = reader["name"].ToString();
            int immune = Convert.ToInt32(reader["immune"]);
            int def_def = Convert.ToInt32(reader["defence"]);
            int farm_con = Convert.ToInt32(reader["farm"]);
            int craft_con = Convert.ToInt32(reader["craft"]);
            int dealer_con = Convert.ToInt32(reader["dealer"]);
            int def_cost = Convert.ToInt32(reader["cost"]);
            int def_army = Convert.ToInt32(reader["army"]);
            decimal def_def_fact = Convert.ToDecimal(reader["def_fact"]);
            reader.Close();

            if (turn > 0)
            {
                turn = turn - 1;
                if (turn == 0)
                {
                    if (att_name == def_name)
                    {
                        command.CommandText = "SELECT * FROM player where username=@p16";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p16", def_name);
                        reader = command.ExecuteReader();
                        reader.Read();
                        decimal military = Convert.ToDecimal(reader["military"]);
                        reader.Close();

                        int offense = Convert.ToInt32(Math.Round((army + def_army) * (military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                        decimal defence = Math.Round(def_def_fact * (farm_con + craft_con + dealer_con) + offense);

                        command.CommandText = "UPDATE regions SET army=@p5, defence=@p17, cost=@p18, offense=@p19 Where name=@p2 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p5", army + def_army);
                        command.Parameters.AddWithValue("@p17", Convert.ToInt32(Math.Round(defence)));
                        command.Parameters.AddWithValue("@p19", offense);
                        command.Parameters.AddWithValue("@p18", def_cost + army);
                        command.ExecuteNonQuery();
                        
                        command.CommandText = "DELETE FROM attacks Where id='" + id + "'";
                        command.Prepare();
                        command.ExecuteNonQuery();

                        if (att_name == textBox23.Text)
                        {
                            MessageBox.Show("H κίνηση ολοκληρώθηκε. Η περιοχή " + def_reg_name + " ενισχύθηκε με " + army + " στρατιώτες .");
                        }
                    }
                    else
                    {
                        command.CommandText = "SELECT * FROM player where username=@p12";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p12", att_name);
                        reader = command.ExecuteReader();
                        reader.Read();
                        decimal att_military = Convert.ToDecimal(reader["military"]);
                        reader.Close();

                        int att_offense = Convert.ToInt32(Math.Round((att_army + army) * (att_military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                        decimal att_defence = Math.Round(off_def_fact * (att_farm + att_craft + att_dealer) + att_offense);

                        command.CommandText = "UPDATE regions SET army=@p4, defence=@p13, cost=@p15, offense=@p14 Where name=@p1 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p4", att_army + army);
                        command.Parameters.AddWithValue("@p13", Convert.ToInt32(Math.Round(att_defence)));
                        command.Parameters.AddWithValue("@p14", att_offense);
                        command.Parameters.AddWithValue("@p15", att_cost + army);
                        command.ExecuteNonQuery();

                        command.CommandText = "DELETE FROM attacks Where id='" + id + "'";
                        command.Prepare();
                        command.ExecuteNonQuery();
                        
                        MessageBox.Show("Η μετακίνηση στρατού στην περιοχή " + def_reg_name + " δεν μπορεί να πραγματοποιηθεί καθώς δεν ανήκει πια σε σας. Ο στρατός επιστρέφει πίσω στην " + temp);
                    }
                }
                command.CommandText = "UPDATE attacks SET turn=@p21 Where id='" + id + "'";
                command.Prepare();
                command.Parameters.AddWithValue("@p21", turn);
                command.ExecuteNonQuery();
            }

            command.Parameters.Clear();
            connection.Close();
        }

        public void Immune()
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            MySqlCommand command1 = new MySqlCommand("SELECT COUNT(*) FROM regions", connection);
            string temp13 = command1.ExecuteScalar().ToString();
            int num_cit = Convert.ToInt32(temp13);

            for (int i = 1; i <= num_cit; i++)
            {
                command.CommandText = "SELECT * FROM regions where id=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", i);
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

                if (level <= 1)
                {
                    farm_max = farmcon;
                    craft_max = craftcon;
                    dealer_max = dealercon;
                }
                else
                {
                    farm_max = Convert.ToInt32(farmcon + (farmcon * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                    craft_max = Convert.ToInt32(craftcon + (craftcon * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                    dealer_max = Convert.ToInt32(dealercon + (dealercon * Convert.ToDecimal(vars["levelup_fact"])) * (level - 1));
                }

                int cost = 0;

                if (enwsi == "" && army_debt_enwsi == 0) 
	            {
                    cost = (farm_max + craft_max + dealer_max) - (def_farm + def_craft + def_dealer);
                }
                else
                {
                    cost = (farm_max + craft_max + dealer_max) - (def_farm + def_craft + def_dealer) + Convert.ToInt32(rev * Convert.ToDecimal(vars["enwsi_gold_contribution"]));

                    command.CommandText = "UPDATE enwsi SET gold_live=gold_live + @p7 Where name=@p8 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p7", Convert.ToInt32(rev * Convert.ToDecimal(vars["enwsi_gold_contribution"])));
                    command.Parameters.AddWithValue("@p8", enwsi);
                    command.ExecuteNonQuery();
                }

                int gold = gold_region + (rev - cost);

                command.CommandText = "UPDATE regions SET cost=@p5, gold=@p3 Where id=@p1 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p5", cost);
                command.Parameters.AddWithValue("@p3", gold);
                command.ExecuteNonQuery();

                if((rev - cost) < 0 && gold < 0)
                {
                    Variables.search_around_fix(i);
                    MessageBox.Show("Η περιοχή " + region + " ρυθμίστηκε");
                }

                if (imm > 0)
                {
                    command.CommandText = "UPDATE regions SET immune= immune - 1 Where id=@p1 ";
                    command.Prepare();
                    command.ExecuteNonQuery();
                }
                

                command.Parameters.Clear();
            }
            connection.Close();
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

        public void PlayerGold()
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM player where username=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", textBox23.Text);
            reader = command.ExecuteReader();
            reader.Read();

            string temp = reader["gold"].ToString();
            string temp1 = reader["free_army"].ToString();
            int army_debt_sunasp = Convert.ToInt32(reader["army_debt_sunasp"]);
            int army_given_sunasp = Convert.ToInt32(reader["army_given_sunasp"]);
            int army_debt_enwsi = Convert.ToInt32(reader["army_debt_enwsi"]);
            int army_given_enwsi = Convert.ToInt32(reader["army_given_enwsi"]);

            reader.Close();

            int sum_gold = 0;
            MySqlCommand command3 = new MySqlCommand("SELECT SUM(gold) FROM regions where owner='" + textBox23.Text + "'", connection);
            reader = command3.ExecuteReader();
            if (reader.Read())
            {
                if (reader[0] != System.DBNull.Value)
                {
                    sum_gold = Convert.ToInt32(reader[0]);
                }
            }
            reader.Close();

            connection.Close();

            textBox22.Text = Convert.ToString(sum_gold);
            textBox42.Text = temp1;
            textBox43.Text = temp;
            textBox44.Text = Convert.ToString((army_debt_sunasp - army_given_sunasp) + (army_debt_enwsi - army_given_enwsi));
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string text = listBox1.GetItemText(listBox1.SelectedItem);
            string text2 = textBox23.Text;

            Form9 frm = new Form9(text, text2);
            frm.Show();

        }

        public void FillList()
        {
            string str = sqlcon;
            MySqlConnection con = new MySqlConnection(str);

            string com = "Select * from player";
            MySqlDataAdapter adpt = new MySqlDataAdapter(com, con);
            DataSet myDataSet = new DataSet();
            adpt.Fill(myDataSet, "player");
            DataTable myDataTable = myDataSet.Tables[0];
            DataRow tempRow = null;

            foreach (DataRow tempRow_Variable in myDataTable.Rows)
            {
                tempRow = tempRow_Variable;
                listBox1.Items.Add((tempRow["username"]));
            }

            string com1 = "Select name from regions where owner='" + textBox23.Text + "'";
            MySqlDataAdapter adpt1 = new MySqlDataAdapter(com1, con);
            DataSet myDataSet1 = new DataSet();
            adpt1.Fill(myDataSet1, "regions");
            DataTable myDataTable1 = myDataSet1.Tables[0];
            DataRow tempRow1 = null;

            foreach (DataRow tempRow1_Variable in myDataTable1.Rows)
            {
                tempRow1 = tempRow1_Variable;
                listBox3.Items.Add((tempRow1["name"]));
            }

        }

        public void fillevents()
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM events LIMIT 0,1";
            command.Prepare();
            
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                DateTime temp = Convert.ToDateTime(reader["date"]);
                DateTime date = DateTime.Now.Date;
                reader.Close();
                int i = DateTime.Compare(temp, date);
                if (i < 0)
                {
                    command.CommandText = "DELETE FROM events Where 1";
                    command.Prepare();
                    command.ExecuteNonQuery();
                }
            }
            
            listView4.Items.Clear();

            string str = sqlcon;
            MySqlConnection con = new MySqlConnection(str);

            string com = "Select att_player,def_player,att_region,def_region,army,kind,result,inserton from events ORDER BY inserton DESC";
            MySqlDataAdapter adpt = new MySqlDataAdapter(com, con);
            DataSet myDataSet = new DataSet();
            adpt.Fill(myDataSet, "events");
            DataTable myDataTable = myDataSet.Tables[0];
            DataRow tempRow = null;
            
            foreach (DataRow tempRow_Variable in myDataTable.Rows)
            {
                tempRow = tempRow_Variable;

                if (Convert.ToInt32( tempRow["army"]) > 0)
                {
                    if (Convert.ToInt32(tempRow["kind"]) == 1)
                    {
                        ListViewItem lvi = new ListViewItem(tempRow["att_player"].ToString());
                        lvi.SubItems.Add("attacks");
                        lvi.SubItems.Add(tempRow["def_player"].ToString());
                        lvi.SubItems.Add("in");
                        lvi.SubItems.Add(tempRow["def_region"].ToString());
                        lvi.SubItems.Add("from");
                        lvi.SubItems.Add(tempRow["att_region"].ToString());
                        lvi.SubItems.Add(tempRow["inserton"].ToString());

                        lvi.UseItemStyleForSubItems = false;

                        lvi.ForeColor = Color.ForestGreen;
                        lvi.SubItems[1].ForeColor = Color.DarkBlue;
                        lvi.SubItems[2].ForeColor = Color.ForestGreen;
                        lvi.SubItems[3].ForeColor = Color.DarkBlue;
                        lvi.SubItems[4].ForeColor = Color.DarkRed;
                        lvi.SubItems[5].ForeColor = Color.DarkBlue;
                        lvi.SubItems[6].ForeColor = Color.DarkRed;
                        lvi.SubItems[7].ForeColor = Color.DarkOrange;

                        listView4.Items.Add(lvi);
                    }
                    else
                    {
                        ListViewItem lvi = new ListViewItem(tempRow["att_player"].ToString());
                        lvi.SubItems.Add("supports");
                        lvi.SubItems.Add(tempRow["def_player"].ToString());
                        lvi.SubItems.Add("in");
                        lvi.SubItems.Add(tempRow["def_region"].ToString());
                        lvi.SubItems.Add("from");
                        lvi.SubItems.Add(tempRow["att_region"].ToString());
                        lvi.SubItems.Add(tempRow["inserton"].ToString());

                        lvi.UseItemStyleForSubItems = false;

                        lvi.ForeColor = Color.ForestGreen;
                        lvi.SubItems[1].ForeColor = Color.DarkBlue;
                        lvi.SubItems[2].ForeColor = Color.ForestGreen;
                        lvi.SubItems[3].ForeColor = Color.DarkBlue;
                        lvi.SubItems[4].ForeColor = Color.DarkRed;
                        lvi.SubItems[5].ForeColor = Color.DarkBlue;
                        lvi.SubItems[6].ForeColor = Color.DarkRed;
                        lvi.SubItems[7].ForeColor = Color.DarkOrange;

                        listView4.Items.Add(lvi);
                    }
                }
                else
                {
                    if (Convert.ToInt32(tempRow["result"]) == 0)
                    {
                        if (tempRow["def_player"].ToString() != "")
                        {
                            ListViewItem lvi = new ListViewItem(tempRow["def_player"].ToString());
                            lvi.SubItems.Add("defeats");
                            lvi.SubItems.Add(tempRow["att_player"].ToString());
                            lvi.SubItems.Add("in");
                            lvi.SubItems.Add(tempRow["def_region"].ToString());
                            lvi.SubItems.Add("");
                            lvi.SubItems.Add("");
                            lvi.SubItems.Add(tempRow["inserton"].ToString());
                            //lvi.SubItems.Add("and maintain their ground");

                            lvi.UseItemStyleForSubItems = false;

                            lvi.ForeColor = Color.ForestGreen;
                            lvi.SubItems[1].ForeColor = Color.DarkBlue;
                            lvi.SubItems[2].ForeColor = Color.ForestGreen;
                            lvi.SubItems[3].ForeColor = Color.DarkBlue;
                            lvi.SubItems[4].ForeColor = Color.DarkRed;
                            lvi.SubItems[5].ForeColor = Color.DarkBlue;
                            lvi.SubItems[6].ForeColor = Color.DarkRed;
                            lvi.SubItems[7].ForeColor = Color.DarkOrange;

                            listView4.Items.Add(lvi);
                        }
                        else
                        {
                            ListViewItem lvi = new ListViewItem(tempRow["def_region"].ToString());
                            lvi.SubItems.Add("defeats");
                            lvi.SubItems.Add(tempRow["att_player"].ToString());
                            lvi.SubItems.Add("");
                            lvi.SubItems.Add("");
                            lvi.SubItems.Add("");
                            lvi.SubItems.Add("");
                            lvi.SubItems.Add(tempRow["inserton"].ToString());
                            //lvi.SubItems.Add("and maintain their ground");

                            lvi.UseItemStyleForSubItems = false;

                            lvi.ForeColor = Color.DarkRed;
                            lvi.SubItems[1].ForeColor = Color.DarkBlue;
                            lvi.SubItems[2].ForeColor = Color.ForestGreen;
                            lvi.SubItems[3].ForeColor = Color.DarkBlue;
                            lvi.SubItems[4].ForeColor = Color.DarkRed;
                            lvi.SubItems[5].ForeColor = Color.DarkBlue;
                            lvi.SubItems[6].ForeColor = Color.DarkRed;
                            lvi.SubItems[7].ForeColor = Color.DarkOrange;

                            listView4.Items.Add(lvi);
                        }
                    }else
                    {
                        if (tempRow["def_player"].ToString() != "")
                        {
                            ListViewItem lvi = new ListViewItem(tempRow["att_player"].ToString());
                            lvi.SubItems.Add("conquers");
                            lvi.SubItems.Add(tempRow["def_region"].ToString());
                            lvi.SubItems.Add("from");
                            lvi.SubItems.Add(tempRow["def_player"].ToString());
                            lvi.SubItems.Add("");
                            lvi.SubItems.Add("");
                            lvi.SubItems.Add(tempRow["inserton"].ToString());

                            lvi.UseItemStyleForSubItems = false;

                            lvi.ForeColor = Color.ForestGreen;
                            lvi.SubItems[1].ForeColor = Color.DarkBlue;
                            lvi.SubItems[2].ForeColor = Color.DarkRed;
                            lvi.SubItems[3].ForeColor = Color.DarkBlue;
                            lvi.SubItems[4].ForeColor = Color.ForestGreen;
                            lvi.SubItems[5].ForeColor = Color.DarkBlue;
                            lvi.SubItems[6].ForeColor = Color.DarkRed;
                            lvi.SubItems[7].ForeColor = Color.DarkOrange;

                            listView4.Items.Add(lvi);
                        }
                        else
                        {
                            ListViewItem lvi = new ListViewItem(tempRow["att_player"].ToString());
                            lvi.SubItems.Add("conquers");
                            lvi.SubItems.Add(tempRow["def_region"].ToString());
                            lvi.SubItems.Add("");
                            lvi.SubItems.Add("");
                            lvi.SubItems.Add("");
                            lvi.SubItems.Add("");
                            lvi.SubItems.Add(tempRow["inserton"].ToString());

                            lvi.UseItemStyleForSubItems = false;

                            lvi.ForeColor = Color.ForestGreen;
                            lvi.SubItems[1].ForeColor = Color.DarkBlue;
                            lvi.SubItems[2].ForeColor = Color.DarkRed;
                            lvi.SubItems[3].ForeColor = Color.DarkBlue;
                            lvi.SubItems[4].ForeColor = Color.DarkRed;
                            lvi.SubItems[5].ForeColor = Color.DarkBlue;
                            lvi.SubItems[6].ForeColor = Color.DarkRed;
                            lvi.SubItems[7].ForeColor = Color.DarkOrange;

                            listView4.Items.Add(lvi);
                        }
                    }
                }
            }
            connection.Close();
        }

        public void FillAttack()
        {

            string str = sqlcon;
            MySqlConnection con = new MySqlConnection(str);
            MySqlDataReader reader;

            string com = "Select * from attacks where att_player='" + textBox23.Text + "' and kind=1";
            MySqlDataAdapter adpt = new MySqlDataAdapter(com, con);
            DataSet myDataSet = new DataSet();
            adpt.Fill(myDataSet, "attacks");
            DataTable myDataTable = myDataSet.Tables[0];
            DataRow tempRow = null;

            listView1.Items.Clear();

            con.Open();
            foreach (DataRow tempRow_Variable in myDataTable.Rows)
            {
                int sum_help_army = 0;
                tempRow = tempRow_Variable;
                //listBox5.Items.Add((tempRow["id"]));
                

                MySqlCommand command3 = new MySqlCommand("SELECT SUM(army) FROM attacks where def_region='" + tempRow["def_region"] + "' && turn='" + tempRow["turn"] + "' && kind=2", con);
                reader = command3.ExecuteReader();
                if (reader.Read())
                {
                    if (reader[0] != System.DBNull.Value)
                    {
                        sum_help_army = Convert.ToInt32(reader[0]);
                    }
                }
                reader.Close();

                MySqlCommand command = con.CreateCommand();
                command.CommandText = "SELECT * FROM regions where name=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", tempRow["def_region"]);
                reader = command.ExecuteReader();
                reader.Read();
                int defence = Convert.ToInt32(reader["defence"]);

                reader.Close();

                command.CommandText = "SELECT * FROM def_othomanoi where name='" + tempRow["def_region"] + "'";
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
                        defence = defence * factor1;
                    }
                }

                reader.Close();

                command.CommandText = "SELECT * FROM player where username=@p2";
                command.Prepare();
                command.Parameters.AddWithValue("@p2", tempRow["att_player"]);
                reader = command.ExecuteReader();
                reader.Read();
                decimal military = Convert.ToDecimal(reader["military"]);
                reader.Close();

                //MySqlCommand command1 = con.CreateCommand();
                //command1.CommandText = "SELECT * FROM player where username=@p1";
                //command1.Prepare();
                //command1.Parameters.AddWithValue("@p1", tempRow["att_player"]);
                //reader = command1.ExecuteReader();
                //reader.Read();
                //int military = Convert.ToInt32(reader["military"]);
                //int army = Convert.ToInt32(tempRow["att_player"]);
                //int defence1 = army * military;
                int army = Convert.ToInt32(tempRow["army"]);
                int army_sunasp = Convert.ToInt32(tempRow["army_sunasp"]);
                int army_enwsi = Convert.ToInt32(tempRow["army_enwsi"]);
                string army_coplayer = tempRow["army_coplayer"].ToString();
                string region_coplayer = tempRow["region_coplayer"].ToString();
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
                //reader.Close();

                // Define the list items
                ListViewItem lvi = new ListViewItem(tempRow["att_region"].ToString());
                lvi.SubItems.Add(tempRow["def_region"].ToString());
                lvi.SubItems.Add(tempRow["turn"].ToString());
                lvi.SubItems.Add(Convert.ToString(defence + sum_help_army));
                lvi.SubItems.Add(Convert.ToString(Math.Round((army + army_sunasp + army_enwsi + sum_co_army) * (military * Convert.ToDecimal(vars["off_fact"]) + 1))));
                lvi.SubItems.Add(Convert.ToString(army + army_sunasp + army_enwsi + sum_co_army));

                lvi.UseItemStyleForSubItems = false;
                
                lvi.ForeColor = Color.DarkRed;

                // You also have access to the list view's SubItems collection

                lvi.SubItems[1].ForeColor = Color.DarkRed;
                lvi.SubItems[2].ForeColor = Color.Blue;
                lvi.SubItems[3].ForeColor = Color.Blue;
                lvi.SubItems[4].ForeColor = Color.Blue;
                lvi.SubItems[5].ForeColor = Color.Blue;
                // Add the list items to the ListView
                listView1.Items.Add(lvi);

                //listView1.Items.AddRange(new ListViewItem[] {item1});
                //listView1.Items[item1].SubItems.AddRange(row1);
                //listView1.Items.Add((tempRow["att_region"]) + "      " + (tempRow["def_region"]) + "         " + tempRow["army"] + " " + tempRow["turn"] + " " + Convert.ToString(army + sum_help_army));
            }

            //string com1 = "Select * from attacks where att_player='"+ textBox23.Text+"'";
            //MySqlDataAdapter adpt1 = new MySqlDataAdapter(com1, con);
            //DataSet myDataSet1 = new DataSet();
            //adpt1.Fill(myDataSet1, "attacks");
            //DataTable myDataTable1 = myDataSet1.Tables[0];
            //DataRow tempRow1 = null;

            //foreach (DataRow tempRow1_Variable in myDataTable1.Rows)
            //{
            //    tempRow1 = tempRow1_Variable;
            //    listBox4.Items.Add((tempRow1["id"] + "  " + tempRow1["att_region"] + " to " + tempRow1["def_region"] + "   Απόσταση: " + tempRow1["distance"] + " Γύροι: " + tempRow1["turn"]));
            //}
            listView2.Items.Clear();

            string com2 = "Select * from attacks where def_player='" + textBox23.Text + "' and kind=1";
            MySqlDataAdapter adpt2 = new MySqlDataAdapter(com2, con);
            DataSet myDataSet2 = new DataSet();
            adpt2.Fill(myDataSet2, "attacks");
            DataTable myDataTable2 = myDataSet2.Tables[0];
            DataRow tempRow2 = null;

            foreach (DataRow tempRow2_Variable in myDataTable2.Rows)
            {
                tempRow2 = tempRow2_Variable;
                int sum_help_army = 0;
                
                //listBox5.Items.Add((tempRow["id"]));

                MySqlCommand command3 = new MySqlCommand("SELECT SUM(army) FROM attacks where def_region='" + tempRow2["def_region"] + "' && turn='" + tempRow2["turn"] + "' && kind=2", con);
                reader = command3.ExecuteReader();
                if (reader.Read())
                {
                    if (reader[0] != System.DBNull.Value)
                    {
                        sum_help_army = Convert.ToInt32(reader[0]);
                    }
                }
                reader.Close();

                MySqlCommand command = con.CreateCommand();
                command.CommandText = "SELECT * FROM regions where name=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", tempRow2["def_region"]);
                reader = command.ExecuteReader();
                reader.Read();
                int defence = Convert.ToInt32(reader["defence"]);

                reader.Close();

                command.CommandText = "SELECT * FROM def_othomanoi where name='" + tempRow2["def_region"] + "'";
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
                        defence = defence * factor1;
                    }
                }

                reader.Close();

                command.CommandText = "SELECT * FROM player where username=@p2";
                command.Prepare();
                command.Parameters.AddWithValue("@p2", tempRow2["att_player"]);
                reader = command.ExecuteReader();
                reader.Read();
                decimal military = Convert.ToDecimal(reader["military"]);
                reader.Close();

                //MySqlCommand command1 = con.CreateCommand();
                //command1.CommandText = "SELECT * FROM player where username=@p1";
                //command1.Prepare();
                //command1.Parameters.AddWithValue("@p1", tempRow2["att_player"]);
                //reader = command1.ExecuteReader();
                //reader.Read();
                //int military = Convert.ToInt32(reader["military"]);
                //int army = Convert.ToInt32(tempRow2["att_player"]);
                //int defence1 = army * military;
                int army = Convert.ToInt32(tempRow2["army"]);
                //reader.Close();

                // Define the list items
                ListViewItem lvi = new ListViewItem(tempRow2["att_region"].ToString());
                lvi.SubItems.Add(tempRow2["def_region"].ToString());
                lvi.SubItems.Add(tempRow2["turn"].ToString());
                lvi.SubItems.Add(Convert.ToString(defence + sum_help_army));
                lvi.SubItems.Add(Convert.ToString(Math.Round(army * (military * Convert.ToDecimal(vars["off_fact"]) + 1))));
                lvi.SubItems.Add(tempRow2["army"].ToString());

                lvi.UseItemStyleForSubItems = false;
                
                lvi.ForeColor = Color.DarkRed;

                // You also have access to the list view's SubItems collection
                lvi.SubItems[1].ForeColor = Color.DarkRed;
                lvi.SubItems[2].ForeColor = Color.Blue;
                lvi.SubItems[3].ForeColor = Color.Blue;
                lvi.SubItems[4].ForeColor = Color.Blue;
                lvi.SubItems[5].ForeColor = Color.Blue;

                // Add the list items to the ListView
                listView2.Items.Add(lvi);
                //listBox7.Items.Add((tempRow2["att_region"]) + "  VS  " + (tempRow2["def_region"]) + " με " + tempRow2["army"] + " στρατιώτες σε " + tempRow2["turn"] + " γύρους");
            }

            con.Close();
        }

        private void listBox3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string text = listBox3.GetItemText(listBox3.SelectedItem);

            Form8 frm = new Form8(text);
            frm.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;
            
            Immune();

            string com2 = "Select id from attacks where kind='3'";
            MySqlDataAdapter adpt2 = new MySqlDataAdapter(com2, connection);
            DataSet myDataSet2 = new DataSet();
            adpt2.Fill(myDataSet2, "attacks");
            DataTable myDataTable2 = myDataSet2.Tables[0];
            DataRow tempRow2 = null;

            foreach (DataRow tempRow2_Variable in myDataTable2.Rows)
            {
                tempRow2 = tempRow2_Variable;
                NewTurn2(Convert.ToInt32(tempRow2["id"]));
            }

            string com = "Select id from attacks where kind='2'";
            MySqlDataAdapter adpt = new MySqlDataAdapter(com, connection);
            DataSet myDataSet = new DataSet();
            adpt.Fill(myDataSet, "attacks");
            DataTable myDataTable = myDataSet.Tables[0];
            DataRow tempRow = null;

            foreach (DataRow tempRow_Variable in myDataTable.Rows)
            {
                tempRow = tempRow_Variable;
                NewTurn1(Convert.ToInt32(tempRow["id"]));
            }

            string com1 = "Select id from attacks where kind='1'";
            MySqlDataAdapter adpt1 = new MySqlDataAdapter(com1, connection);
            DataSet myDataSet1 = new DataSet();
            adpt1.Fill(myDataSet1, "attacks");
            DataTable myDataTable1 = myDataSet1.Tables[0];
            DataRow tempRow1 = null;

            foreach (DataRow tempRow1_Variable in myDataTable1.Rows)
            {
                tempRow1 = tempRow1_Variable;
                NewTurn(Convert.ToInt32(tempRow1["id"]));
            }

            string com3 = "Select * from attacks where turn=0 && kind='2'";
            MySqlDataAdapter adpt3 = new MySqlDataAdapter(com3, connection);
            DataSet myDataSet3 = new DataSet();
            adpt3.Fill(myDataSet3, "attacks");
            DataTable myDataTable3 = myDataSet3.Tables[0];
            DataRow tempRow3 = null;

            foreach (DataRow tempRow3_Variable in myDataTable3.Rows)
            {
                tempRow3 = tempRow3_Variable;

                command.CommandText = "SELECT * FROM regions where name=@p1";
                command.Prepare();
                command.Parameters.AddWithValue("@p1", tempRow3["att_region"]);
                reader = command.ExecuteReader();
                reader.Read();

                string att_name = reader["owner"].ToString();
                int att_army = Convert.ToInt32(reader["army"]);
                int att_farm = Convert.ToInt32(reader["farm"]);
                int att_craft = Convert.ToInt32(reader["craft"]);
                int att_dealer = Convert.ToInt32(reader["dealer"]);
                decimal att_def_fact = Convert.ToDecimal(reader["def_fact"]);

                reader.Close();

                command.CommandText = "SELECT * FROM player where username=@p2";
                command.Prepare();
                command.Parameters.AddWithValue("@p2", att_name);
                reader = command.ExecuteReader();
                reader.Read();

                decimal att_military = Convert.ToDecimal(reader["military"]);

                reader.Close();

                int offense = Convert.ToInt32(Math.Round((att_army + Convert.ToInt32(tempRow3["army"])) * (att_military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                decimal defence = Math.Round(att_def_fact * (att_farm + att_craft + att_dealer) + offense);
                command.CommandText = "UPDATE regions SET army=@p3, defence=@p4, cost=@p5, offense=@p6 Where name=@p7 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p3", att_army + Convert.ToInt32(tempRow3["army"]));
                command.Parameters.AddWithValue("@p4", Convert.ToInt32(Math.Round(defence)));
                command.Parameters.AddWithValue("@p5", att_army + Convert.ToInt32(tempRow3["army"]));
                command.Parameters.AddWithValue("@p6", offense);
                command.Parameters.AddWithValue("@p7", tempRow3["att_region"]);
                command.ExecuteNonQuery();


                command.CommandText = "SELECT * FROM regions where name=@p8";
                command.Prepare();
                command.Parameters.AddWithValue("@p8", tempRow3["def_region"]);
                reader = command.ExecuteReader();
                reader.Read();

                string def_name = reader["owner"].ToString();
                int def_army = Convert.ToInt32(reader["army"]);
                int def_farm = Convert.ToInt32(reader["farm"]);
                int def_craft = Convert.ToInt32(reader["craft"]);
                int def_dealer = Convert.ToInt32(reader["dealer"]);
                decimal def_def_fact = Convert.ToDecimal(reader["def_fact"]);

                reader.Close();

                command.CommandText = "SELECT * FROM player where username=@p9";
                command.Prepare();
                command.Parameters.AddWithValue("@p9", def_name);
                reader = command.ExecuteReader();
                reader.Read();

                decimal def_military = Convert.ToDecimal(reader["military"]);

                reader.Close();

                int offense1 = Convert.ToInt32(Math.Round((def_army - Convert.ToInt32(tempRow3["army"])) * (def_military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                decimal defence1 = Math.Round(def_def_fact * (def_farm + def_craft + def_dealer) + offense1);
                command.CommandText = "UPDATE regions SET army=@p10, defence=@p11, cost=@p12, offense=@p13 Where name=@p14 ";
                command.Prepare();
                command.Parameters.AddWithValue("@p10", def_army - Convert.ToInt32(tempRow3["army"]));
                command.Parameters.AddWithValue("@p11", Convert.ToInt32(Math.Round(defence1)));
                command.Parameters.AddWithValue("@p12", def_army - Convert.ToInt32(tempRow3["army"]));
                command.Parameters.AddWithValue("@p13", offense1);
                command.Parameters.AddWithValue("@p14", tempRow3["def_region"]);
                command.ExecuteNonQuery();
            }

            command.CommandText = "DELETE FROM attacks Where turn=0 && kind=2";
            command.Prepare();
            command.ExecuteNonQuery();

            string com4 = "Select * from sunaspismos where 1";
            MySqlDataAdapter adpt4 = new MySqlDataAdapter(com4, connection);
            DataSet myDataSet4 = new DataSet();
            adpt4.Fill(myDataSet4, "attacks");
            DataTable myDataTable4 = myDataSet4.Tables[0];
            DataRow tempRow4 = null;

            foreach (DataRow tempRow4_Variable in myDataTable4.Rows)
            {
                tempRow4 = tempRow4_Variable;

                string players = tempRow4["players"].ToString();
                string name = tempRow4["name"].ToString();
                int status = Convert.ToInt32(tempRow4["status"]);
                int army_live = Convert.ToInt32(tempRow4["army_live"]);
                string[] players_arr = players.Split(',');
                int total_sunasp_army = 0;

                if (players_arr.Length >= Convert.ToInt32(vars["sunasp_player_limit"]))
                {
                    foreach (string player in players_arr)
                    {
                        command.Parameters.Clear();

                        if (status == 0)
                        {
                            command.CommandText = "UPDATE regions SET def_fact= def_fact + 1 Where owner=@p15 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p15", player);
                            command.ExecuteNonQuery();
                        }

                        int total_population = 0;
                        int free_army = 0;
                        int army_debt_sunasp = 0;
                        int army_given_sunasp = 0;

                        command.CommandText = "SELECT SUM(farmcon),SUM(craftcon),SUM(dealercon) FROM regions where owner=@p19";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p19", player);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            if (reader[0] != System.DBNull.Value)
                            {
                                total_population = Convert.ToInt32(reader[0]) + Convert.ToInt32(reader[1]) + Convert.ToInt32(reader[2]);
                            }
                        }
                        reader.Close();

                        command.CommandText = "SELECT * FROM player where username=@p24";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p24", player);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            free_army = Convert.ToInt32(reader["free_army"]);
                            army_given_sunasp = Convert.ToInt32(reader["army_given_sunasp"]);
                        }
                        reader.Close();

                        total_sunasp_army += Convert.ToInt32(total_population * Convert.ToDecimal(vars["sunasp_army_contribution"]));

                        army_debt_sunasp = Convert.ToInt32(total_population * Convert.ToDecimal(vars["sunasp_army_contribution"])) - army_given_sunasp;
                        if (army_debt_sunasp > 0)
                        {
                            if ((army_debt_sunasp - free_army) >= 0)
                            {
                                army_given_sunasp = army_given_sunasp + free_army;
                                army_live += free_army;
                                free_army = 0;
                            }
                            else
                            {
                                free_army = Math.Abs(army_debt_sunasp - free_army);
                                army_given_sunasp = (Convert.ToInt32(total_population * Convert.ToDecimal(vars["sunasp_army_contribution"])));
                                army_live += army_debt_sunasp;
                                army_debt_sunasp = 0;
                            }
                        }
                        else if(army_debt_sunasp < 0)
                        {
                            free_army = free_army + Math.Abs(army_debt_sunasp);
                            int missing_population = Math.Abs(army_debt_sunasp);
                            army_given_sunasp = (Convert.ToInt32(total_population * Convert.ToDecimal(vars["sunasp_army_contribution"])));
                            missing_population = missing_population - army_live;

                            if (missing_population <= 0)
                            {
                                army_live = Math.Abs(missing_population);
                                missing_population = 0;

                                command.CommandText = "UPDATE sunaspismos SET army_live=@p34 Where name=@p35 ";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p34", army_live);
                                command.Parameters.AddWithValue("@p35", name);
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                army_live = 0;

                                command.CommandText = "UPDATE sunaspismos SET army_live=@p34 Where name=@p35 ";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p34", army_live);
                                command.Parameters.AddWithValue("@p35", name);
                                command.ExecuteNonQuery();

                                string com6 = "Select * from attacks where sunaspismos='" + name + "'";
                                MySqlDataAdapter adpt6 = new MySqlDataAdapter(com6, connection);
                                DataSet myDataSet6 = new DataSet();
                                adpt6.Fill(myDataSet6, "attacks");
                                DataTable myDataTable6 = myDataSet6.Tables[0];
                                DataRow tempRow6 = null;

                                foreach (DataRow tempRow6_Variable in myDataTable6.Rows)
                                {
                                    command.Parameters.Clear();

                                    tempRow6 = tempRow6_Variable;

                                    int army_sunasp1 = Convert.ToInt32(tempRow6["army_sunasp"]);
                                    int id2 = Convert.ToInt32(tempRow6["id"]);

                                    missing_population = missing_population - army_sunasp1;

                                    if (missing_population <= 0)
                                    {
                                        army_sunasp1 = Math.Abs(missing_population);
                                        missing_population = 0;

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

                        }

                        command.CommandText = "UPDATE player SET free_army=@p27, army_debt_sunasp=@p20, army_given_sunasp=@p25 Where username=@p21 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p20", Convert.ToInt32(total_population * Convert.ToDecimal(vars["sunasp_army_contribution"])));
                        command.Parameters.AddWithValue("@p21", player);
                        command.Parameters.AddWithValue("@p25", army_given_sunasp);
                        command.Parameters.AddWithValue("@p27", free_army);
                        command.ExecuteNonQuery();
                    }
                    command.CommandText = "UPDATE sunaspismos SET army=@p22, army_live=@p30 Where name=@p23 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p22", total_sunasp_army);
                    command.Parameters.AddWithValue("@p23", name);
                    command.Parameters.AddWithValue("@p30", army_live);
                    command.ExecuteNonQuery();

                    if (status == 0)
                    {
                        command.CommandText = "UPDATE sunaspismos SET status=@p28 Where name=@p29 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p28", 1);
                        command.Parameters.AddWithValue("@p29", name);
                        command.ExecuteNonQuery(); 
                    }
                }
                else if (players_arr.Length < Convert.ToInt32(vars["sunasp_player_limit"]) && status == 1)
                {
                    foreach (string player in players_arr)
                    {
                        command.Parameters.Clear();

                        command.CommandText = "UPDATE regions SET def_fact= def_fact - 1 Where owner=@p16 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p16", player);
                        command.ExecuteNonQuery();

                        command.CommandText = "UPDATE attacks SET army_sunasp=@p19, sunaspismos=@p20 Where att_player=@p21 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p19", 0);
                        command.Parameters.AddWithValue("@p20", "");
                        command.Parameters.AddWithValue("@p21", player);
                        command.ExecuteNonQuery();

                        int free_army = 0;
                        int army_given_sunasp = 0;

                        command.CommandText = "SELECT * FROM player where username=@p24";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p24", player);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            free_army = Convert.ToInt32(reader["free_army"]);
                            army_given_sunasp = Convert.ToInt32(reader["army_given_sunasp"]);
                        }
                        reader.Close();

                        command.CommandText = "UPDATE player SET free_army=@p25, army_debt_sunasp=@p26, army_given_sunasp=@p27 Where username=@p24 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p25", free_army + army_given_sunasp);
                        command.Parameters.AddWithValue("@p26", 0);
                        command.Parameters.AddWithValue("@p27", 0);
                        command.ExecuteNonQuery();
                    }
                    command.CommandText = "UPDATE sunaspismos SET army=@p28, army_live=@p29, status=@p17 Where name=@p18 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p17", 0);
                    command.Parameters.AddWithValue("@p18", name);
                    command.Parameters.AddWithValue("@p28", 0);
                    command.Parameters.AddWithValue("@p29", 0);
                    command.ExecuteNonQuery();
                }
                else if (players == "")
                {
                    command.Parameters.Clear();

                    command.CommandText = "DELETE FROM sunaspismos Where name=@p18 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p18", name);
                    command.ExecuteNonQuery();

                    command.CommandText = "DELETE FROM agreement Where sunaspismos=@p19 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p19", name);
                    command.ExecuteNonQuery();
                }
                foreach (string player5 in players_arr)
                {
                    Variables.fix_defence(player5);
                }
            }

            string com5 = "Select * from enwsi where 1";
            MySqlDataAdapter adpt5 = new MySqlDataAdapter(com5, connection);
            DataSet myDataSet5 = new DataSet();
            adpt5.Fill(myDataSet5, "attacks");
            DataTable myDataTable5 = myDataSet5.Tables[0];
            DataRow tempRow5 = null;

            foreach (DataRow tempRow5_Variable in myDataTable5.Rows)
            {
                tempRow5 = tempRow5_Variable;

                string players = tempRow5["players"].ToString();
                string leader = tempRow5["leader"].ToString();
                string name = tempRow5["name"].ToString();
                int leader_rem_rounds = Convert.ToInt32(tempRow5["leader_rem_rounds"]);
                int vote_rem_rounds = Convert.ToInt32(tempRow5["vote_rem_rounds"]);
                int break_rem_rounds = Convert.ToInt32(tempRow5["break_rem_rounds"]);
                int status = Convert.ToInt32(tempRow5["status"]);
                int army_live = Convert.ToInt32(tempRow5["army_live"]);
                int gold_live = Convert.ToInt32(tempRow5["gold_live"]);
                decimal gold_percent = Convert.ToDecimal(tempRow5["gold_percent"]);
                decimal farm_percent = Convert.ToDecimal(tempRow5["farm_percent"]);
                decimal craft_percent = Convert.ToDecimal(tempRow5["craft_percent"]);
                decimal dealer_percent = Convert.ToDecimal(tempRow5["dealer_percent"]);
                decimal army_percent = Convert.ToDecimal(tempRow5["army_percent"]);
                string[] players_arr = players.Split(',');
                int total_enwsi_army = 0;

                if (players_arr.Length >= Convert.ToInt32(vars["enwsi_player_limit"]))
                {
                    int gold_change = 0;
                    int total_enwsi_farm = 0;
                    int total_enwsi_craft = 0;
                    int total_enwsi_dealer = 0;
                    int total_enwsi_army1 = 0;
                    int total_edres = 0;

                    if (status == 1)
                    {
                        command.Parameters.Clear();
                        gold_change = gold_live - Convert.ToInt32(gold_live * gold_percent);
                        gold_live = Convert.ToInt32(gold_live * gold_percent);

                        command.CommandText = "UPDATE enwsi SET gold_live=@p1 Where name=@p2 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p1", gold_live);
                        command.Parameters.AddWithValue("@p2", name);
                        command.ExecuteNonQuery();
                    }

                    Dictionary<string, int> elections = new Dictionary<string, int>();

                    command.Parameters.Clear();
                    if (leader_rem_rounds != 0)
                    {
                        command.CommandText = "UPDATE enwsi SET leader_rem_rounds= leader_rem_rounds - 1 Where name=@p1 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p1", name);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        if (vote_rem_rounds == Convert.ToInt32(vars["enwsi_vote_rounds"]) && break_rem_rounds == Convert.ToInt32(vars["enwsi_break_rounds"]))
                        {
                            leader_rem_rounds = 0;
                            status = 0;

                            foreach (string player7 in players_arr)
                            {
                                command.Parameters.Clear();

                                command.CommandText = "UPDATE regions SET def_fact= def_fact - 2 Where owner=@p16 ";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p16", player7);
                                command.ExecuteNonQuery();

                                command.CommandText = "UPDATE attacks SET army_enwsi=@p19, enwsi=@p20 Where att_player=@p21 ";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p19", 0);
                                command.Parameters.AddWithValue("@p20", "");
                                command.Parameters.AddWithValue("@p21", player7);
                                command.ExecuteNonQuery();

                                int free_army = 0;
                                int army_given_enwsi = 0;

                                command.CommandText = "SELECT * FROM player where username=@p24";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p24", player7);
                                reader = command.ExecuteReader();
                                if (reader.Read())
                                {
                                    free_army = Convert.ToInt32(reader["free_army"]);
                                    army_given_enwsi = Convert.ToInt32(reader["army_given_enwsi"]);
                                }
                                reader.Close();

                                command.CommandText = "UPDATE player SET free_army=@p25, army_debt_enwsi=@p26, army_given_enwsi=@p27, enwsi_vote=@p30 Where username=@p24 ";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p25", free_army + army_given_enwsi);
                                command.Parameters.AddWithValue("@p26", 0);
                                command.Parameters.AddWithValue("@p27", 0);
                                command.Parameters.AddWithValue("@p30", "");
                                command.ExecuteNonQuery();
                            }
                            command.Parameters.Clear();

                            command.CommandText = "UPDATE enwsi SET leader_rem_rounds=@p30, army=@p28, army_live=@p29, status=@p17 Where name=@p18 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p17", 0);
                            command.Parameters.AddWithValue("@p18", name);
                            command.Parameters.AddWithValue("@p28", 0);
                            command.Parameters.AddWithValue("@p29", 0);
                            command.Parameters.AddWithValue("@p30", 0);
                            command.ExecuteNonQuery(); 
                        }

                        command.CommandText = "UPDATE enwsi SET vote_rem_rounds= vote_rem_rounds - 1 Where name=@p2 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p2", name);
                        command.ExecuteNonQuery();
                    }

                    foreach (string player in players_arr)
                    {
                        command.Parameters.Clear();

                        if (status == 0 && leader_rem_rounds != 0)
                        {
                            command.CommandText = "UPDATE regions SET def_fact= def_fact + 2 Where owner=@p15 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p15", player);
                            command.ExecuteNonQuery();
                        }

                        int total_population = 0;
                        int free_army = 0;
                        int army_debt_enwsi = 0;
                        int army_given_enwsi = 0;
                        string enwsi_vote = "";

                        command.CommandText = "SELECT SUM(farmcon),SUM(craftcon),SUM(dealercon),SUM(farm),SUM(craft),SUM(dealer),SUM(army) FROM regions where owner=@p19";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p19", player);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            if (reader[0] != System.DBNull.Value)
                            {
                                total_population = Convert.ToInt32(reader[0]) + Convert.ToInt32(reader[1]) + Convert.ToInt32(reader[2]);
                                total_enwsi_farm += Convert.ToInt32(reader[3]);
                                total_enwsi_craft += Convert.ToInt32(reader[4]);
                                total_enwsi_dealer += Convert.ToInt32(reader[5]);
                                total_enwsi_army1 += Convert.ToInt32(reader[6]);
                            }
                        }
                        reader.Close();
                        
                        command.CommandText = "SELECT * FROM player where username=@p24";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p24", player);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            free_army = Convert.ToInt32(reader["free_army"]);
                            army_given_enwsi = Convert.ToInt32(reader["army_given_enwsi"]);
                            enwsi_vote = reader["enwsi_vote"].ToString();
                        }
                        reader.Close();
                        ////////////////////////////////////////////////////////////////////////////////////////
                        MySqlCommand command1 = new MySqlCommand("SELECT COUNT(*) FROM regions where owner='" + player + "'", connection);
                        string temp3 = command1.ExecuteScalar().ToString();
                        int num_cit = Convert.ToInt32(temp3);
                        int player_edres = 0;

                        if (num_cit != 0)
                        {
                            player_edres = total_population / num_cit;
                            total_edres += player_edres;

                            if (!elections.ContainsKey(enwsi_vote) && enwsi_vote != "")
                            {
                                elections.Add(enwsi_vote, player_edres);
                            }
                            else if (elections.ContainsKey(enwsi_vote) && enwsi_vote != "")
                            {
                                elections[enwsi_vote] = elections[enwsi_vote] + player_edres;
                            }
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////
                        if (status == 1 && leader_rem_rounds != 0)
                        {
                            total_enwsi_army += (Convert.ToInt32(total_population * Convert.ToDecimal(vars["enwsi_army_contribution"])));

                            army_debt_enwsi = (Convert.ToInt32(total_population * Convert.ToDecimal(vars["enwsi_army_contribution"]))) - army_given_enwsi;
                            if (army_debt_enwsi > 0)
                            {
                                if ((army_debt_enwsi - free_army) >= 0)
                                {
                                    army_given_enwsi = army_given_enwsi + free_army;
                                    army_live += free_army;
                                    free_army = 0;
                                }
                                else
                                {
                                    free_army = Math.Abs(army_debt_enwsi - free_army);
                                    army_given_enwsi = (Convert.ToInt32(total_population * Convert.ToDecimal(vars["enwsi_army_contribution"])));
                                    army_live += army_debt_enwsi;
                                    army_debt_enwsi = 0;
                                }
                            }
                            else if (army_debt_enwsi < 0)
                            {
                                free_army = free_army + Math.Abs(army_debt_enwsi);
                                int missing_population = Math.Abs(army_debt_enwsi);
                                army_given_enwsi = (Convert.ToInt32(total_population * Convert.ToDecimal(vars["enwsi_army_contribution"])));
                                missing_population = missing_population - army_live;

                                if (missing_population <= 0)
                                {
                                    army_live = Math.Abs(missing_population);
                                    missing_population = 0;

                                    command.CommandText = "UPDATE enwsi SET army_live=@p34 Where name=@p35 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p34", army_live);
                                    command.Parameters.AddWithValue("@p35", name);
                                    command.ExecuteNonQuery();
                                }
                                else
                                {
                                    army_live = 0;

                                    command.CommandText = "UPDATE enwsi SET army_live=@p34 Where name=@p35 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p34", army_live);
                                    command.Parameters.AddWithValue("@p35", name);
                                    command.ExecuteNonQuery();

                                    string com6 = "Select * from attacks where enwsi='" + name + "'";
                                    MySqlDataAdapter adpt6 = new MySqlDataAdapter(com6, connection);
                                    DataSet myDataSet6 = new DataSet();
                                    adpt6.Fill(myDataSet6, "attacks");
                                    DataTable myDataTable6 = myDataSet6.Tables[0];
                                    DataRow tempRow6 = null;

                                    foreach (DataRow tempRow6_Variable in myDataTable6.Rows)
                                    {
                                        command.Parameters.Clear();

                                        tempRow6 = tempRow6_Variable;

                                        int army_enwsi = Convert.ToInt32(tempRow6["army_enwsi"]);
                                        int id2 = Convert.ToInt32(tempRow6["id"]);

                                        missing_population = missing_population - army_enwsi;

                                        if (missing_population <= 0)
                                        {
                                            army_enwsi = Math.Abs(missing_population);
                                            missing_population = 0;

                                            command.CommandText = "UPDATE attacks SET army_enwsi=@p36 Where id=@p37 ";
                                            command.Prepare();
                                            command.Parameters.AddWithValue("@p36", army_enwsi);
                                            command.Parameters.AddWithValue("@p37", id2);
                                            command.ExecuteNonQuery();

                                            break;
                                        }
                                        else
                                        {
                                            army_enwsi = 0;

                                            command.CommandText = "UPDATE attacks SET army_enwsi=@p36, enwsi=@p38 Where id=@p37 ";
                                            command.Prepare();
                                            command.Parameters.AddWithValue("@p36", army_enwsi);
                                            command.Parameters.AddWithValue("@p37", id2);
                                            command.Parameters.AddWithValue("@p38", "");
                                            command.ExecuteNonQuery();
                                        }

                                    }
                                }

                            }
                            command.CommandText = "UPDATE player SET free_army=@p27, army_debt_enwsi=@p20, army_given_enwsi=@p25 Where username=@p21 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p20", Convert.ToInt32(total_population * Convert.ToDecimal(vars["enwsi_army_contribution"])));
                            command.Parameters.AddWithValue("@p21", player);
                            command.Parameters.AddWithValue("@p25", army_given_enwsi);
                            command.Parameters.AddWithValue("@p27", free_army);
                            command.ExecuteNonQuery(); 
                        }
                    }

                    command.Parameters.Clear();

                    if (leader_rem_rounds != 0 && status == 1)
                    {
                        int most_votes = elections.Values.Max();
                        string top_candidate = elections.MaxBy(kvp => kvp.Value).Key;

                        if (most_votes <= (total_edres * Convert.ToDecimal(vars["edres_demand_percent"])))
                        {
                            foreach (string player7 in players_arr)
                            {
                                command.Parameters.Clear();

                                //command.CommandText = "UPDATE regions SET def_fact= def_fact - 2 Where owner=@p16 ";
                                //command.Prepare();
                                //command.Parameters.AddWithValue("@p16", player7);
                                //command.ExecuteNonQuery();

                                command.CommandText = "UPDATE attacks SET army_enwsi=@p19, enwsi=@p20 Where att_player=@p21 ";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p19", 0);
                                command.Parameters.AddWithValue("@p20", "");
                                command.Parameters.AddWithValue("@p21", player7);
                                command.ExecuteNonQuery();

                                int free_army = 0;
                                int army_given_enwsi = 0;

                                command.CommandText = "SELECT * FROM player where username=@p24";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p24", player7);
                                reader = command.ExecuteReader();
                                if (reader.Read())
                                {
                                    free_army = Convert.ToInt32(reader["free_army"]);
                                    army_given_enwsi = Convert.ToInt32(reader["army_given_enwsi"]);
                                }
                                reader.Close();

                                command.CommandText = "UPDATE player SET free_army=@p25, army_debt_enwsi=@p26, army_given_enwsi=@p27, enwsi_vote=@p30 Where username=@p24 ";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p25", free_army + army_given_enwsi);
                                command.Parameters.AddWithValue("@p26", 0);
                                command.Parameters.AddWithValue("@p27", 0);
                                command.Parameters.AddWithValue("@p30", "");
                                command.ExecuteNonQuery();
                            }
                            command.Parameters.Clear();

                            command.CommandText = "UPDATE enwsi SET leader_rem_rounds=@p30, army=@p28, army_live=@p29, status=@p17 Where name=@p18 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p17", 0);
                            command.Parameters.AddWithValue("@p18", name);
                            command.Parameters.AddWithValue("@p28", 0);
                            command.Parameters.AddWithValue("@p29", 0);
                            command.Parameters.AddWithValue("@p30", 0);
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            command.CommandText = "UPDATE enwsi SET army=@p22, army_live=@p30 Where name=@p23 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p22", total_enwsi_army);
                            command.Parameters.AddWithValue("@p23", name);
                            command.Parameters.AddWithValue("@p30", army_live);
                            command.ExecuteNonQuery();
                        }
                    }
                    else if(leader_rem_rounds == 0 && vote_rem_rounds == 0)
                    {
                        int most_votes = 0;
                        string top_candidate = "";

                        if (elections.Any())
                        {
                            most_votes = elections.Values.Max();
                            top_candidate = elections.MaxBy(kvp => kvp.Value).Key; 
                        }

                        if (most_votes > (total_edres * Convert.ToDecimal(vars["edres_demand_percent"])) && most_votes > 0)
                        {
                            foreach (string player8 in players_arr)
                            {
                                command.Parameters.Clear();

                                command.CommandText = "UPDATE player SET enwsi_vote=@p30 Where username=@p24 and enwsi_vote=''";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p24", player8);
                                command.Parameters.AddWithValue("@p30", top_candidate);
                                command.ExecuteNonQuery();
                            }

                            command.CommandText = "UPDATE enwsi SET leader=@p34, leader_rem_rounds=@p33, vote_rem_rounds=@p31, break_rem_rounds=@p35 Where name=@p25 ";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p25", name);
                            command.Parameters.AddWithValue("@p31", Convert.ToInt32(vars["enwsi_vote_rounds"]));
                            command.Parameters.AddWithValue("@p33", Convert.ToInt32(vars["enwsi_leader_rounds"]));
                            command.Parameters.AddWithValue("@p34", top_candidate);
                            command.Parameters.AddWithValue("@p35", Convert.ToInt32(vars["enwsi_break_rounds"]));
                            command.ExecuteNonQuery(); 
                        }
                        else
                        {
                            foreach (string player8 in players_arr)
                            {
                                command.Parameters.Clear();

                                command.CommandText = "UPDATE player SET enwsi_vote=@p30 Where username=@p24 ";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p24", player8);
                                command.Parameters.AddWithValue("@p30", "");
                                command.ExecuteNonQuery();
                            }

                            if (break_rem_rounds > 0)
                            {
                                command.CommandText = "UPDATE enwsi SET vote_rem_rounds=@p31, break_rem_rounds= break_rem_rounds - 1 Where name=@p25 ";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p25", name);
                                command.Parameters.AddWithValue("@p31", Convert.ToInt32(vars["enwsi_vote_rounds"]));
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                command.Parameters.Clear();

                                command.CommandText = "DELETE FROM enwsi Where name=@p18 ";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p18", name);
                                command.ExecuteNonQuery();

                                command.CommandText = "DELETE FROM agreement Where enwsi=@p19 ";
                                command.Prepare();
                                command.Parameters.AddWithValue("@p19", name);
                                command.ExecuteNonQuery();

                                foreach (string player9 in players_arr)
                                {
                                    command.Parameters.Clear();

                                    command.CommandText = "UPDATE player SET enwsi=@p31, enwsi_vote=@p30 Where username=@p24 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p24", player9);
                                    command.Parameters.AddWithValue("@p30", "");
                                    command.Parameters.AddWithValue("@p31", "");
                                    command.ExecuteNonQuery();

                                    MySqlCommand command4 = new MySqlCommand("SELECT COUNT(*) FROM regions where owner='" + player9 + "'", connection);
                                    string temp4 = command4.ExecuteScalar().ToString();
                                    decimal num_cit2 = Convert.ToDecimal(temp4);

                                    int number_lost_regions = Convert.ToInt32(Math.Round(num_cit2 - (num_cit2 * Convert.ToDecimal(vars["enwsi_lost_regions_percent"]))));

                                    string com7 = "SELECT * FROM regions WHERE owner='" + player9 + "' ORDER BY RAND() LIMIT " + number_lost_regions;
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

                                        command.CommandText = "UPDATE regions SET owner=@p10 Where name=@p24 ";
                                        command.Prepare();
                                        command.Parameters.AddWithValue("@p10", "");
                                        command.Parameters.AddWithValue("@p24", reg_name);
                                        command.ExecuteNonQuery();

                                        command.CommandText = "UPDATE attacks SET def_player=@p11 Where def_region=@p25 ";
                                        command.Prepare();
                                        command.Parameters.AddWithValue("@p11", "");
                                        command.Parameters.AddWithValue("@p25", reg_name);
                                        command.ExecuteNonQuery();

                                        Variables.fix_defence(player9);
                                    }
                                }
                            }
                        }
                    }

                    if (status == 0 && leader_rem_rounds != 0)
                    {
                        command.CommandText = "UPDATE enwsi SET status=@p28 Where name=@p29 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p28", 1);
                        command.Parameters.AddWithValue("@p29", name);
                        command.ExecuteNonQuery();
                    }

                    if (status == 1)
                    {
                        int gold_return = 0;
                        int gold_farm = Convert.ToInt32( gold_change * farm_percent);
                        int gold_craft = Convert.ToInt32( gold_change * craft_percent);
                        int gold_dealer = Convert.ToInt32( gold_change * dealer_percent);
                        int gold_army = Convert.ToInt32( gold_change * army_percent);

                        foreach (string player1 in players_arr)
                        {
                            command.Parameters.Clear();

                            command.CommandText = "SELECT SUM(farm),SUM(craft),SUM(dealer),SUM(army) FROM regions where owner=@p19";
                            command.Prepare();
                            command.Parameters.AddWithValue("@p19", player1);
                            reader = command.ExecuteReader();
                            if (reader.Read())
                            {
                                if (reader[0] != System.DBNull.Value)
                                {
                                    if (total_enwsi_army1 == 0)
                                    {
                                        gold_return = (Convert.ToInt32((Convert.ToDecimal(reader[0]) / Convert.ToDecimal(total_enwsi_farm)) * gold_farm)) + (Convert.ToInt32((Convert.ToDecimal(reader[1]) / Convert.ToDecimal(total_enwsi_craft)) * gold_craft)) + (Convert.ToInt32((Convert.ToDecimal(reader[2]) / Convert.ToDecimal(total_enwsi_dealer)) * gold_dealer)); 
                                    }else
                                    {
                                        gold_return = (Convert.ToInt32((Convert.ToDecimal(reader[0]) / Convert.ToDecimal(total_enwsi_farm)) * gold_farm)) + (Convert.ToInt32((Convert.ToDecimal(reader[1]) / Convert.ToDecimal(total_enwsi_craft)) * gold_craft)) + (Convert.ToInt32((Convert.ToDecimal(reader[2]) / Convert.ToDecimal(total_enwsi_dealer)) * gold_dealer)) + (Convert.ToInt32((Convert.ToDecimal(reader[3]) / Convert.ToDecimal(total_enwsi_army1)) * gold_army)); 
                                    }

                                    reader.Close();

                                    command.CommandText = "UPDATE player SET gold= gold + @p1 Where username=@p2 ";
                                    command.Prepare();
                                    command.Parameters.AddWithValue("@p1", gold_return);
                                    command.Parameters.AddWithValue("@p2", player1);
                                    command.ExecuteNonQuery();
                                }
                                else
                                {
                                    reader.Close();
                                }
                            }
                            
                        }
                    }
                }
                else if (players_arr.Length < Convert.ToInt32(vars["enwsi_player_limit"]) && status == 1)
                {
                    foreach (string player in players_arr)
                    {
                        command.Parameters.Clear();

                        command.CommandText = "UPDATE regions SET def_fact= def_fact - 2 Where owner=@p16 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p16", player);
                        command.ExecuteNonQuery();

                        command.CommandText = "UPDATE attacks SET army_enwsi=@p19, enwsi=@p20 Where att_player=@p21 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p19", 0);
                        command.Parameters.AddWithValue("@p20", "");
                        command.Parameters.AddWithValue("@p21", player);
                        command.ExecuteNonQuery();

                        int free_army = 0;
                        int army_given_enwsi = 0;

                        command.CommandText = "SELECT * FROM player where username=@p24";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p24", player);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            free_army = Convert.ToInt32(reader["free_army"]);
                            army_given_enwsi = Convert.ToInt32(reader["army_given_enwsi"]);
                        }
                        reader.Close();

                        command.CommandText = "UPDATE player SET free_army=@p25, army_debt_enwsi=@p26, army_given_enwsi=@p27, enwsi_vote=@p30 Where username=@p24 ";
                        command.Prepare();
                        command.Parameters.AddWithValue("@p25", free_army + army_given_enwsi);
                        command.Parameters.AddWithValue("@p26", 0);
                        command.Parameters.AddWithValue("@p27", 0);
                        command.Parameters.AddWithValue("@p30", "");
                        command.ExecuteNonQuery();
                    }
                    command.Parameters.Clear();

                    command.CommandText = "UPDATE enwsi SET leader_rem_rounds=@p30, vote_rem_rounds=@p31, break_rem_rounds=@p32, army=@p28, army_live=@p29, status=@p17 Where name=@p18 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p17", 0);
                    command.Parameters.AddWithValue("@p18", name);
                    command.Parameters.AddWithValue("@p28", 0);
                    command.Parameters.AddWithValue("@p29", 0);
                    command.Parameters.AddWithValue("@p30", Convert.ToInt32(vars["enwsi_leader_rounds"]));
                    command.Parameters.AddWithValue("@p31", Convert.ToInt32(vars["enwsi_vote_rounds"]));
                    command.Parameters.AddWithValue("@p32", Convert.ToInt32(vars["enwsi_break_rounds"]));
                    command.ExecuteNonQuery();
                }
                else if (players == "")
                {
                    command.Parameters.Clear();

                    command.CommandText = "DELETE FROM enwsi Where name=@p18 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p18", name);
                    command.ExecuteNonQuery();

                    command.CommandText = "DELETE FROM agreement Where enwsi=@p19 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p19", name);
                    command.ExecuteNonQuery();
                }
                foreach (string player5 in players_arr)
                {
                    Variables.fix_defence(player5);
                }
            }

            listBox1.Items.Clear();
            listView1.Items.Clear();
            listView2.Items.Clear();
            listBox3.Items.Clear();
            listBox6.Items.Clear();

            reduce_turn();
            check_sunaspismos();
            PlayerGold();
            FillList();
            FillAttack();
            fillevents();
        }

        public void reduce_turn()
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();

            command.CommandText = "UPDATE support SET turn= turn - 1 Where 1";
            command.Prepare();
            command.ExecuteNonQuery();

            connection.Close();
        }

        public void check_sunaspismos()
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            int free_army = 0;
            int army_given_sunasp = 0;
            int army_debt_sunasp = 0;
            int army_given_enwsi = 0;
            int army_debt_enwsi = 0;
            string sunasp = "";
            string enwsi = "";

            command.CommandText = "SELECT * FROM player where username=@p24";
            command.Prepare();
            command.Parameters.AddWithValue("@p24", textBox23.Text);
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                free_army = Convert.ToInt32(reader["free_army"]);
                army_given_sunasp = Convert.ToInt32(reader["army_given_sunasp"]);
                army_debt_sunasp = Convert.ToInt32(reader["army_debt_sunasp"]);
                army_given_enwsi = Convert.ToInt32(reader["army_given_enwsi"]);
                army_debt_enwsi = Convert.ToInt32(reader["army_debt_enwsi"]);
                sunasp = reader["sunasp"].ToString();
                enwsi = reader["enwsi"].ToString();
            }
            reader.Close();

            if (sunasp != "" && enwsi != "")
            {
                label5.Visible = true;
                textBox44.Visible = true;
                string link = textBox23.Text.ToUpper() + " (" + sunasp + "," + enwsi + ")";
                label33.Text = link;

                textBox44.Text = Convert.ToString((army_debt_sunasp - army_given_sunasp) + (army_debt_enwsi - army_given_enwsi));
            }
            else
            {
                if (sunasp != "")
                {
                    label5.Visible = true;
                    textBox44.Visible = true;
                    string link = textBox23.Text.ToUpper() + " (" + sunasp + ")";
                    label33.Text = link;

                    textBox44.Text = Convert.ToString(army_debt_sunasp - army_given_sunasp);
                }
                else
                {
                    label33.Text = textBox23.Text.ToUpper();
                    label5.Visible = false;
                    textBox44.Visible = false;
                }
                if (enwsi != "")
                {
                    label5.Visible = true;
                    textBox44.Visible = true;
                    string link = textBox23.Text.ToUpper() + " (" + enwsi + ")";
                    label33.Text = link;

                    textBox44.Text = Convert.ToString(army_debt_enwsi - army_given_enwsi);
                }
                else
                {
                    label33.Text = textBox23.Text.ToUpper();
                    label5.Visible = false;
                    textBox44.Visible = false;
                }
            }
            
            connection.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button11_Click_1(sender, e);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (Convert.ToInt32(textBox39.Text) > 0) timer1.Interval = Convert.ToInt32(textBox39.Text);
                timer1.Enabled = true;
            }
            else
            {
                timer1.Enabled = false;
            }
        }

        void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            display2.Focus();
        }


        void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                if (e.Delta <= 0)
                {
                    if (zoom <= 0)
                    {
                        zoom = 0;
                        display2.Width = 800;
                        display2.Height = 800;
                        XOff1 = display2.Width / 2;
                        Yoff1 = display2.Height / 2;
                        display2.Location = new Point(300, 0);
                    }
                    else
                    {
                        zoom -= 1;
                        display2.Width -= 200;
                        display2.Height -= 200;
                        XOff1 = display2.Width / 2;
                        Yoff1 = display2.Height / 2;
                        display2.Location = new Point(300, 0);
                    }
                    
                    
                    //if (zoom < 0)
                    //    return;
                    //zoom -= 2;
                    ////set minimum size to zoom
                    //if (display2.Width < 800)
                    //    return;
                    //display2.Width += Convert.ToInt32(display2.Width * e.Delta / 1000);
                    //display2.Height += Convert.ToInt32(display2.Height * e.Delta / 1000);
                    //XOff1 = display2.Width / 2;
                    //Yoff1 = display2.Height / 2;
                    //display2.Location = new Point(300, 0);
                    
                }
                else
                {
                    if (zoom >= 10)
                    {
                        zoom = 10;
                        display2.Width = 2800;
                        display2.Height = 2800;
                        XOff1 = display2.Width / 2;
                        Yoff1 = display2.Height / 2;
                        display2.Location = new Point(300, 0);
                    }
                    else
                    {
                        zoom += 1;
                        display2.Width += 200;
                        display2.Height += 200;
                        XOff1 = display2.Width / 2;
                        Yoff1 = display2.Height / 2;
                        display2.Location = new Point(300, 0);
                    }
                    //zoom += 2;
                    ////set maximum size to zoom
                    //if (display2.Width > 2000)
                    //    return;
                    //display2.Width += Convert.ToInt32(display2.Width * e.Delta / 1000);
                    //display2.Height += Convert.ToInt32(display2.Height * e.Delta / 1000);
                    //XOff1 = display2.Width / 2;
                    //Yoff1 = display2.Height / 2;
                    //display2.Location = new Point(300, 0);
                    
                }
                //display2.Width += Convert.ToInt32(display2.Width * e.Delta / 1000);
                //display2.Height += Convert.ToInt32(display2.Height * e.Delta / 1000);
                //display2.Width += 500;
                //display2.Height += 500;
                //XOff1 = display2.Width / 2;
                //Yoff1 = display2.Height / 2;
                //display2.Location = new Point(300, 0);
                DrawMap();
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Form4 frm = new Form4(textBox23.Text);
            frm.Show();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Form5 frm = new Form5(textBox23.Text);
            frm.Show();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            Form6 frm = new Form6(textBox23.Text);
            frm.Show();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            Form7 frm = new Form7(textBox23.Text);
            frm.Show();
        }

        void display2_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        protected void LB_DoubleClick(object sender, EventArgs e)
        {
            //attempt to cast the sender as a label
            Label lbl = sender as Label;
            if (listBox3.Items.Contains(lbl.Name))
            {
                Form8 frm = new Form8(lbl.Name);
                frm.Show();
            }
        }

        protected void LB_Click(object sender, EventArgs e)
        {
            //attempt to cast the sender as a label
            Label lbl = sender as Label;

            //if the cast was successful
            if (lbl != null)
            {
                listView3.Items.Clear();
                MySqlConnection connection = new MySqlConnection(sqlcon);
                connection.Open();
                string com2 = "Select * from regions where name='" + lbl.Name + "'";
                MySqlDataAdapter adpt2 = new MySqlDataAdapter(com2, connection);
                DataSet myDataSet2 = new DataSet();
                adpt2.Fill(myDataSet2, "regions");
                DataTable myDataTable2 = myDataSet2.Tables[0];
                DataRow tempRow2 = null;

                foreach (DataRow tempRow2_Variable in myDataTable2.Rows)
                {
                    tempRow2 = tempRow2_Variable;

                    MySqlCommand command = connection.CreateCommand();
                    MySqlDataReader reader;

                    command.CommandText = "SELECT * FROM player where username=@p1";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p1", tempRow2["owner"]);
                    reader = command.ExecuteReader();

                    int military = 1;
                    if (reader.Read())
                    {
                        military = Convert.ToInt32(reader["military"]);
                    }
                    reader.Close();

                    int army = Convert.ToInt32(tempRow2["army"]);

                    int defence = Convert.ToInt32(tempRow2["defence"]);
                    command.CommandText = "SELECT * FROM def_othomanoi where name='" + lbl.Name + "'";
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

                    // Define the list items
                    ListViewItem lvi = new ListViewItem(tempRow2["name"].ToString());
                    //lvi.SubItems.Add(tempRow2["name"].ToString());
                    lvi.SubItems.Add(tempRow2["owner"].ToString());
                    lvi.SubItems.Add(Convert.ToString(defence));
                    lvi.SubItems.Add(Convert.ToString(Math.Round(army * (military * Convert.ToDecimal(vars["off_fact"]) + 1))));
                    lvi.SubItems.Add(tempRow2["army"].ToString());
                    lvi.SubItems.Add(tempRow2["gold"].ToString());
                    lvi.SubItems.Add(tempRow2["farm"].ToString());
                    lvi.SubItems.Add(tempRow2["craft"].ToString());
                    lvi.SubItems.Add(tempRow2["dealer"].ToString());

                    lvi.UseItemStyleForSubItems = false;

                    lvi.ForeColor = Color.DarkRed;

                    // You also have access to the list view's SubItems collection

                    //lvi.SubItems[1].ForeColor = Color.DarkRed;
                    lvi.SubItems[1].ForeColor = Color.ForestGreen;
                    lvi.SubItems[2].ForeColor = Color.DarkBlue;
                    lvi.SubItems[3].ForeColor = Color.DarkBlue;
                    lvi.SubItems[4].ForeColor = Color.DarkBlue;
                    lvi.SubItems[5].ForeColor = Color.DarkOrange;
                    lvi.SubItems[6].ForeColor = Color.DarkBlue;
                    lvi.SubItems[7].ForeColor = Color.DarkBlue;
                    lvi.SubItems[8].ForeColor = Color.DarkBlue;

                    // Add the list items to the ListView
                    listView3.Items.Add(lvi);
                    //listBox7.Items.Add((tempRow2["att_region"]) + "  VS  " + (tempRow2["def_region"]) + " με " + tempRow2["army"] + " στρατιώτες σε " + tempRow2["turn"] + " γύρους");
                }
                connection.Close();
            }
        }

        private void listBox3_MouseClick(object sender, MouseEventArgs e)
        {
            string text = listBox3.GetItemText(listBox3.SelectedItem);
            if (display2.Controls[text] != null)
            {
                //display2.Controls[text].BackColor = Color.Green;

                Label lbl = display2.Controls.Find(text, true).FirstOrDefault() as Label;
                lbl.Image =  new Bitmap(WindowsFormsApplication2.Properties.Resources.tower2);
                //display2.Controls[text].Width = 40;
                //display2.Controls[text].Height = 63;
            }
        }

        private void display2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _mousePt = e.Location;
                _tracking = true;
            }
        }

        private void display2_MouseMove(object sender, MouseEventArgs e)
        {

            //label56.Text = Convert.ToString(-(XOff1 - _mousePt.X));
            //label57.Text = Convert.ToString(Yoff1 - _mousePt.Y);
            if (_tracking &&
            (display2.Width > flowLayoutPanel1.Width ||
            display2.Height > flowLayoutPanel1.Height))
            {
                flowLayoutPanel1.AutoScrollPosition = new Point(-flowLayoutPanel1.AutoScrollPosition.X + (_mousePt.X - e.X),
                 -flowLayoutPanel1.AutoScrollPosition.Y + (_mousePt.Y - e.Y));
            }
        }

        private void display2_MouseUp(object sender, MouseEventArgs e)
        {
            _tracking = false;
        }

        protected void LB_MouseHover(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            if (listBox3.Items.Contains(lbl.Name))
            {
                lbl.Image = new Bitmap(WindowsFormsApplication2.Properties.Resources.tower4);
            }
            else
            {
                lbl.Image = new Bitmap(WindowsFormsApplication2.Properties.Resources.tower1);
            } 
            ToolTip tip = new ToolTip();
            tip.SetToolTip(lbl, lbl.Name);
            
        }

        protected void LB_MouseLeave(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            if(listBox3.Items.Contains(lbl.Name))
            {
                lbl.Image = new Bitmap(WindowsFormsApplication2.Properties.Resources.tower2);
            }
            else
            {
                lbl.Image = new Bitmap(WindowsFormsApplication2.Properties.Resources.tower);
            } 
        }

        private void button17_MouseHover(object sender, EventArgs e)
        {
            button17.BackgroundImage = new Bitmap(WindowsFormsApplication2.Properties.Resources.region6);
        }

        private void button17_MouseLeave(object sender, EventArgs e)
        {
            button17.BackgroundImage = new Bitmap(WindowsFormsApplication2.Properties.Resources.region51);
        }

        private void button18_MouseHover(object sender, EventArgs e)
        {
            button18.Image = new Bitmap(WindowsFormsApplication2.Properties.Resources.Attack_Notification1);
        }

        private void button18_MouseLeave(object sender, EventArgs e)
        {
            button18.Image = new Bitmap(WindowsFormsApplication2.Properties.Resources.Attack_Notification);
        }

        private void button19_MouseHover(object sender, EventArgs e)
        {
            button19.BackgroundImage = new Bitmap(WindowsFormsApplication2.Properties.Resources.defence1);
        }

        private void button19_MouseLeave(object sender, EventArgs e)
        {
            button19.BackgroundImage = new Bitmap(WindowsFormsApplication2.Properties.Resources.defence);
        }

        private void button20_MouseHover(object sender, EventArgs e)
        {
            button20.BackgroundImage = new Bitmap(WindowsFormsApplication2.Properties.Resources.shake_225x225_black1);
        }

        private void button20_MouseLeave(object sender, EventArgs e)
        {
            button20.BackgroundImage = new Bitmap(WindowsFormsApplication2.Properties.Resources.shake_225x225_black);
        }

        private void listView4_MouseHover(object sender, EventArgs e)
        {
            listView4.Focus();
        }

        private void getMessage()
        {
            while (true)
            {
                try
                {
                    serverStream = clientSocket.GetStream();
                    int buffSize = 0;
                    byte[] inStream = new byte[70000];
                    buffSize = clientSocket.ReceiveBufferSize;
                    serverStream.Read(inStream, 0, buffSize);
                    string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                    readData = "" + returndata;
                    msg();
                }
                catch (System.IO.IOException e)
                {

                }
            }
        }

        private void msg()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(msg));
            }
            else
            {
                textBox40.Text = textBox40.Text + Environment.NewLine + " >> " + readData;
                textBox40.SelectionStart = textBox40.Text.Length;
                textBox40.ScrollToCaret();
            }
        } 

        private void textBox41_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 13)
            {
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(textBox41.Text + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                textBox41.Text = "";
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM player where username=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", textBox23.Text);
            reader = command.ExecuteReader();
            reader.Read();

            decimal military = Convert.ToDecimal(reader["military"]);
            int free_army = Convert.ToInt32(reader["free_army"]);

            reader.Close();

            int selected_army = Convert.ToInt32(numericUpDown7.Value);
            
            string text = listBox3.GetItemText(listBox3.SelectedItem);
            if (listBox3.SelectedItem != null)
            {
                if (selected_army <= free_army)
                {
                    command.CommandText = "SELECT * FROM regions where name='" + text + "'";
                    command.Prepare();
                    reader = command.ExecuteReader();

                    reader.Read();

                    int id = Convert.ToInt32(reader["id"]);
                    string owner = reader["owner"].ToString();
                    int farm = Convert.ToInt32(reader["farm"]);
                    int craft = Convert.ToInt32(reader["craft"]);
                    int dealer = Convert.ToInt32(reader["dealer"]);
                    int army = Convert.ToInt32(reader["army"]);
                    decimal def_fact = Convert.ToDecimal(reader["def_fact"]);

                    reader.Close();

                    int offense = Convert.ToInt32(Math.Round((army + selected_army) * (military * Convert.ToDecimal(vars["off_fact"]) + 1)));
                    decimal defence = Math.Round(def_fact * (farm + craft + dealer) + offense);

                    command.CommandText = "UPDATE regions SET army=@p11, defence=@p12, cost=@p13, offense=@p14 Where name=@p16 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p11", army + selected_army);
                    command.Parameters.AddWithValue("@p12", Convert.ToInt32(Math.Round(defence)));
                    command.Parameters.AddWithValue("@p13", army + selected_army);
                    command.Parameters.AddWithValue("@p14", offense);
                    command.Parameters.AddWithValue("@p16", text);
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE player SET free_army=@p5 Where username=@p6 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p5", free_army - selected_army);
                    command.Parameters.AddWithValue("@p6", owner);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Η περιοχή " + text + " ενημερώθηκε με στρατιώτες");

                    PlayerGold();

                    Variables.calculate_cost(id);
                }
                else
                {
                    MessageBox.Show("Ο αριθμός των στρατιωτών που επιλέξατε είναι μεγαλύτερος από τον διαθέσιμο");
                }
            }
            else
            {
                MessageBox.Show("Επιλέξτε περιοχή");
            }

            connection.Close();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button16_Click_1(object sender, EventArgs e)
        {
            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT * FROM player where username=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", textBox23.Text);
            reader = command.ExecuteReader();
            reader.Read();

            int gold = Convert.ToInt32(reader["gold"]);

            reader.Close();

            int selected_gold = Convert.ToInt32(numericUpDown8.Value);

            string text = listBox3.GetItemText(listBox3.SelectedItem);
            if (listBox3.SelectedItem != null)
            {
                if (selected_gold <= gold)
                {
                    command.CommandText = "SELECT * FROM regions where name='" + text + "'";
                    command.Prepare();
                    reader = command.ExecuteReader();

                    reader.Read();

                    int city_gold = Convert.ToInt32(reader["gold"]);

                    reader.Close();

                    command.CommandText = "UPDATE regions SET gold=@p11 Where name=@p16 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p11", city_gold + selected_gold);
                    command.Parameters.AddWithValue("@p16", text);
                    command.ExecuteNonQuery();

                    command.CommandText = "UPDATE player SET gold=@p5 Where username=@p6 ";
                    command.Prepare();
                    command.Parameters.AddWithValue("@p5", gold - selected_gold);
                    command.Parameters.AddWithValue("@p6", textBox23.Text);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Η περιοχή " + text + " ενημερώθηκε με χρήματα");

                    PlayerGold();
                }
                else
                {
                    MessageBox.Show("Ο αριθμός των χρημάτων που επιλέξατε είναι μεγαλύτερος από τον διαθέσιμο");
                }
            }
            else
            {
                MessageBox.Show("Επιλέξτε περιοχή");
            }

            connection.Close();
        }

        private void button22_Click(object sender, EventArgs e)
        {
            PlayerGold();
        }

        public void login_player(string username)
        {
            label33.Text = username;
            label33.Text = label33.Text.ToUpper();

            MySqlConnection connection = new MySqlConnection(sqlcon);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader reader;

            command.CommandText = "SELECT picture FROM player where username=@p1";
            command.Prepare();
            command.Parameters.AddWithValue("@p1", username);
            reader = command.ExecuteReader();
            if(reader.Read())
            {
                byte[] img = (byte[])reader["picture"];
                MemoryStream ms = new MemoryStream(img);
                pictureBox2.BackgroundImage = null;
                if (img.Length != 0)
                {
                    pictureBox2.BackgroundImage = Image.FromStream(ms);
                }
            }

            reader.Close();

            connection.Close();
            
            textBox23.Text = username;
            textBox23.Visible = false;

            listView1.Items.Clear();
            listView2.Items.Clear();
            listView3.Items.Clear();
            listBox1.Items.Clear();
            listBox3.Items.Clear();

            DrawMap();
            FillList();
            FillAttack();
            fillevents();
            PlayerGold();
            check_sunaspismos();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            login_player(Convert.ToString(comboBox1.SelectedValue));
        }
    }
}


