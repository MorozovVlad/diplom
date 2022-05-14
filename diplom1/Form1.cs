using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;

namespace diplom1
{
    public partial class Form1 : Form
    {
        private SqlConnection SqlConnection = null;

        public Form1()
        {
            InitializeComponent();
        }
        public class distAndPoints
        {
            public double dist;
            public int point1;
            public int point2;

            public distAndPoints(double dist, int i, int j)
            {
                this.dist = dist;
                this.point1 = i;
                this.point2 = j;
            }
        }

        int totalCost = 0;
        int cost1 = 0;
        int lampsCount = 0;
        bool check = false;
        int radius = 0;
        int bright1 = 0;
        List<Point> koordinat = new List<Point>();
        List<int> brightList = new List<int>();



        private void Form1_Load_1(object sender, EventArgs e)
        {
            SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["LampsDB"].ConnectionString);
            SqlConnection.Open();

            if (SqlConnection.State == ConnectionState.Open)
            {
                MessageBox.Show("Подключение установлено!");
            }

            Graphics graphics = pictureBox1.CreateGraphics();
            graphics.Clear(Color.White);
            pictureBox1.Image = Properties.Resources.fon;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            Image image = (Image)Properties.Resources.fon;
            Rectangle rectangle = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
            graphics.DrawImage(image, rectangle);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string nametable = "Lamp1";
            data_output(sender, e, nametable);
        }


        private void data_output(object sender, EventArgs e, string nametable)
        {
            listView1.Items.Clear();

            SqlDataReader dataReader = null;

            try
            {
                string sqlCommand = "SELECT Name, Bright, Power, Cost FROM Lamps WHERE Name=@nametable";

                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["LampsDB"].ConnectionString);
                
                SqlCommand command = new SqlCommand(sqlCommand, connection);
                    
                connection.Open();
                SqlParameter sqlParameter = new SqlParameter("@nametable", nametable);

                command.Parameters.Add(sqlParameter);
                SqlDataReader reader = command.ExecuteReader();             
                                                   
                ListViewItem item = null;

                while (reader.Read())
                {
                    item = new ListViewItem(new string[] { Convert.ToString(reader["Name"]), Convert.ToString(reader["Bright"]), Convert.ToString(reader["Power"]), Convert.ToString(reader["Cost"]) });

                    listView1.Items.Add(item);

                    String bright = Convert.ToString(reader["Bright"]);

                    bright1 = int.Parse(bright);

                    String cost = Convert.ToString(reader["Cost"]);

                    cost1 = int.Parse(cost);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (dataReader != null && !dataReader.IsClosed)
                {
                    dataReader.Close();
                }
            }
            check = true;
            brightList.Add(bright1);
            lampsCount++;
        }


        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {           
            if (check)
            {
                Point click;
                click = e.Location;

                koordinat.Add(new Point(click.X, click.Y));
                render();
            }
        }

        private void render()
        {
            
            Graphics graphics = pictureBox1.CreateGraphics();
            graphics.Clear(Color.White);

            Image image = (Image)Properties.Resources.fon;
            Rectangle rectangle = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
            graphics.DrawImage(image, rectangle);
            for (int i = 0; i < koordinat.Count(); i++)
            {                   
                radius = brightList[i] / 10;
                graphics.FillEllipse(Brushes.Orange, koordinat[i].X - radius, koordinat[i].Y - radius, radius * 2, radius * 2);
            }
            for (int i = 0; i < koordinat.Count(); i++)
            {
                radius = brightList[i] / 10;
                graphics.FillEllipse(Brushes.Tomato, koordinat[i].X - radius * 3 / 4, koordinat[i].Y - radius * 3 / 4, radius * 3 / 2, radius * 3 / 2);
            }
            for (int i = 0; i < koordinat.Count(); i++)
            {
                radius = brightList[i] / 10;
                graphics.FillEllipse(Brushes.Red, koordinat[i].X - radius / 2, koordinat[i].Y - radius / 2, radius, radius);
            }
            for (int i = 0; i < koordinat.Count(); i++)
            {                    
                graphics.FillEllipse(Brushes.Black, koordinat[i].X - 4, koordinat[i].Y - 4, 8, 8);
                Pen blackPen1 = new Pen(Color.Black, 1);


            }


            check = false;
            totalCost = totalCost + cost1;
            textBox2.Text = "Использовано фонарей: " + lampsCount;
            textBox1.Text = "Общая стоимость: " + totalCost;
            link();
        }

        private void link()
        {
            Graphics graphics = pictureBox1.CreateGraphics();


            int[] components = new int[koordinat.Count()];
            for (int i = 0; i < components.Length; i++)
            {
                components[i] = i + 1;
            }


            List<distAndPoints> dist_and_numbers = new List<distAndPoints>();

            for (int i = 0; i < koordinat.Count(); i++)
            {
                for (int j = i + 1; j < koordinat.Count(); j++)
                {
                    int x1 = koordinat[i].X;
                    int y1 = koordinat[i].Y;

                    int x2 = koordinat[j].X;
                    int y2 = koordinat[j].Y;

                    double dist = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
                    dist_and_numbers.Add(new distAndPoints(dist, i, j));
                }
            }
            
            List<distAndPoints> sorted_dist_and_numbers = dist_and_numbers.OrderBy(item => item.dist).ToList();
            int xx = 0;
            for (int i = 0; i < sorted_dist_and_numbers.Count(); i++)
            {
                distAndPoints x = sorted_dist_and_numbers[i];


                if (components[x.point1] == components[x.point2]) continue;
                
                Pen blackPen1 = new Pen(Color.Black, 1);
                graphics.DrawLine(blackPen1, koordinat[x.point1].X, koordinat[x.point1].Y, koordinat[x.point2].X, koordinat[x.point2].Y);
                xx++;
                String w = "";
                for (int j = 0; j < components.Count(); j++)
                {
                    w = w + components[j].ToString();

                }
                int color1 = components[x.point1];
                int color2 = components[x.point2];
                for (int j = 0; j < components.Count(); j++)
                {
                    if (components[j] == color1)
                    {
                        components[j] = color2;

                    }
                }
                String s ="";
                for (int j = 0; j < components.Count(); j++)
                {
                    s = s + components[j].ToString();
                    
                }
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string nametable = "Lamp2";
            data_output(sender, e, nametable);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string nametable = "Lamp3";
            data_output(sender, e, nametable);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string nametable = "Lamp4";
            data_output(sender, e, nametable);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string nametable = "Lamp5";
            data_output(sender, e, nametable);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string nametable = "Lamp6";
            data_output(sender, e, nametable);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string nametable = "Lamp7";
            data_output(sender, e, nametable);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string nametable = "Lamp8";
            data_output(sender, e, nametable);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string nametable = "Lamp9";
            data_output(sender, e, nametable);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string nametable = "Lamp10";
            data_output(sender, e, nametable);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {

        }

        private void pictureBox1_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Graphics graphics = pictureBox1.CreateGraphics();
            graphics.Clear(Color.White);
            //pictureBox1.Image = Properties.Resources.fon;
            //pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            Image image = (Image)Properties.Resources.fon;
            Rectangle rectangle = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
            graphics.DrawImage(image, rectangle);
        }
    }
}
