using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace gol
{
    public partial class Form1 : Form
    {
        int ld=0, md=0; //статистика смертей от одиночества и от перенаселения
        bool[,] plane;
        int pix = 10;
        public Form1()
        {
            InitializeComponent();
            StartPlane();
            button2.Enabled = false;
            label1.Text = "";
            label2.Text = "";
            groupBox2.Visible=false;
        }        

        //изначальное поле для клеток
        private void StartPlane()
        {
            plane = new bool[50, 50];
        }

        //получение количества соседей у каждой клетки
        private int GetNeigh(bool[,] pl, int x, int y)
        {
            int nghb = 0;
            int my = plane.GetLength(0);
            int mx = plane.GetLength(1);

            for (int i=-1; i<=1; i++)
            {
                for (int j=-1; j<=1; j++)
                {
                    int nx = x + i;
                    int ny = y + j;
                    if (nx == x && ny == y)
                        continue;
                    if (nx < 0 || nx >= mx)
                        continue;
                    if (ny < 0 || ny >= my)
                        continue;

                    if (pl[nx, ny])
                        nghb++;
                }
            }
            return nghb;
        }

        //заполнение клетки 
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush life = Brushes.White;
            Brush death = Brushes.Black;
            for (int x = 0; x<plane.GetLength(0); x++)
            {
                for (int y = 0; y<plane.GetLength(1); y++)
                {
                    if (plane[x,y])
                    {
                        g.FillRectangle(life, x * pix, y * pix, pix, pix);
                    }
                }
            }
        }

        //клетка заполняется при нажатии на нее в режиме заполнения вручную
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if ((radioButton2.Checked==true || radioButton4.Checked==true) && button2.Enabled==false)
            {
                int x = e.X / pix;
                int y = e.Y / pix;
                plane[x, y] = !plane[x, y];
                pictureBox1.Invalidate();
            }
        }

        //начало жизненного цикла колонии клеток
        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            button2.Enabled = true;
            button4.Enabled = false;
            button3.Enabled = false;
            timer1.Interval = (510 - trackBar1.Value);
            groupBox1.Enabled = false;
            timer1.Start();
        }

        //вывод нового поколения происходит по таймеру с интервалом, задаваемым пользователем
        private void timer1_Tick(object sender, EventArgs e)
        {
            int over = 0;
            plane = playfunc(plane);
            label2.Text = "Смертей от одиночества:"+ld+"\nСмертей от перенаселения: "+md;
            pictureBox1.Invalidate();
            for (int x = 0; x < plane.GetLength(0); x++)
            {
                for (int y = 0; y < plane.GetLength(1); y++)
                {
                    if (plane[x, y])
                    {
                        over = 1;
                        break;
                    }
                }
            }
            if (over == 0)
            {
                label1.Text = "GAME OVER";
                button2.Enabled = false;
                groupBox1.Enabled = true;
                button4.Enabled = true;
                button3.Enabled = true;
                timer1.Stop();
            }
        }

        //изменение интервала таймера, т.е. скорости заполнения
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = (510-trackBar1.Value);
        }

        //приостановка игры
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            groupBox1.Enabled = true;
            button2.Enabled = false;
            button4.Enabled = true;
            button3.Enabled = true;
            label1.Text = "Для продолжения \nнажмите \"Старт\"";
        }

        //получение нового поколения
        private bool[,] playfunc(bool[,] plan)
        {
            bool[,] plane2 = new bool[plan.GetLength(0), plan.GetLength(1)];

            for (int x=0; x<plan.GetLength(0); x++)
            {
                for (int y=0; y<plan.GetLength(1); y++)
                {
                    int ngbg = GetNeigh(plan, x, y);
                    if (ngbg < 2)
                    {
                        if (plane[x, y])
                            ld++;
                        plane2[x, y] = false;
                    }
                    else if (ngbg == 2 && plan[x, y] == true)
                    {
                        plane2[x, y] = true;
                    }
                    else if (ngbg == 3)
                    {
                        plane2[x, y] = true;
                    }
                    else if (ngbg > 3)
                    {
                        if (plane[x, y])
                            md++;
                        plane2[x, y] = false;
                    }
                }
            }
            return plane2;
        }

        //очистка поля для задания нового
        private void newplane()
        {
            if (button2.Enabled == false)
            {
                for (int x = 0; x < plane.GetLength(0); x++)
                {
                    for (int y = 0; y < plane.GetLength(1); y++)
                    {
                        plane[x, y] = false;
                    }
                }
                pictureBox1.Invalidate();
            }
        }

        //при выборе случайного метода заполнения поля появляется кнопка для генерации поля и очищается текущее
        private void radioButton1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            if (radioButton1.Checked == true)
            {
                groupBox2.Visible = false;
                ld = 0;
                md = 0;
                label2.Text = "";
                button3.Visible = true;
                newplane();
            }
        }

        //при выборе метода заполнения вручную поле очищается и пользователю предоставляется возможность заполнить поле
        private void radioButton2_Click(object sender, EventArgs e)
        {            
            label1.Text = "";
            if (radioButton2.Checked==true)
            {
                groupBox2.Visible = false;
                ld = 0;
                md = 0;
                label2.Text = "";
                button3.Visible = false;
                newplane();
            }
        }

        //при нажатии на кнопку "Сгенерировать поле", поле колонии заполняется случайным образом
        private void button3_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            if (radioButton1.Checked)
            {
                label1.Text = "";
                ld = 0;
                md = 0;
                label2.Text = "";
                for (int x = 0; x < plane.GetLength(0); x++)
                {
                    for (int y = 0; y < plane.GetLength(1); y++)
                    {
                        plane[x, y] = rnd.Next(100) % 2 == 0;
                    }
                }
                pictureBox1.Invalidate();
            }
        }

        //очистка поля
        private void button4_Click(object sender, EventArgs e)
        {
            if (button2.Enabled == false)
            {
                newplane();
                label1.Text = "";
                ld = 0;
                md = 0;
                label2.Text = "";
            }
        }

        //выбор паттерна из стандартных, имеющихся в системе
        private void radioButton3_Click(object sender, EventArgs e)
        {
            if (radioButton3.Checked==true)
            {
                groupBox2.Visible = true;
                label1.Text = "";
                label2.Text = "";
                newplane();
                button5.Visible = false;
                gliderbutton.Text = "Glider";
                tumbutton.Text="Tumbler";
                spaceshipbutton.Text="Spaceship";
                rowbutton.Text="10 Cell Row";
                exploderbutton.Text="Exploder";
                gunbutton.Text="Gospel Glider Gun";
            }
        }

        //выбор из сохраненных
        private void radioButton4_Click(object sender, EventArgs e)
        {
            if (radioButton4.Checked == true)
            {
                groupBox2.Visible = true;
                button5.Visible = false;
                label1.Text = "";
                label2.Text = "";
                newplane();
                gliderbutton.Text = "Сохранение 1";
                tumbutton.Text = "Сохранение 2";
                spaceshipbutton.Text = "Сохранение 3";
                rowbutton.Text = "Сохранение 4";
                exploderbutton.Text = "Сохранение 5";
                gunbutton.Text = "Сохранение 6";
            }
        }

        //загрузка выбранного паттерна
        private void loadpattern(string name)
        {
            try
            {
                FileStream file = File.OpenRead(name);
                for (int x = 0; x < plane.GetLength(0); x++)
                {
                    for (int y = 0; y < plane.GetLength(1); y++)
                    {
                        file.ReadByte();
                        int a = file.ReadByte();
                        if (a == 49)
                            plane[x, y] = true;
                    }
                }
                pictureBox1.Invalidate();
            }

            catch
            {
                label1.Text = "Ошибка!";
            }
            
        }     

        private string savename()
        {
            string sname="";
            if (gliderbutton.Checked==true)
            {
                sname = Environment.CurrentDirectory+ "/savings/pat1.bin";
            }
            else if (tumbutton.Checked == true)
            {
                sname = Environment.CurrentDirectory + "/savings/pat2.bin";
            }
            else if (spaceshipbutton.Checked == true)
            {
                sname = Environment.CurrentDirectory + "/savings/pat3.bin";
            }
            else if (rowbutton.Checked == true)
            {
                sname = Environment.CurrentDirectory + "/savings/pat4.bin";
            }
            else if (exploderbutton.Checked == true)
            {
                sname = Environment.CurrentDirectory + "/savings/pat5.bin";
            }
            else if (gunbutton.Checked == true)
            {
                sname = Environment.CurrentDirectory + "/savings/pat6.bin";
            }
            return sname;
        }

        //сохранение текущего паттерна
        private void button5_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            string sname = savename();
            BinaryWriter dataOut;
            dataOut = new BinaryWriter(new FileStream(sname, FileMode.Create));
            //loadpattern(sname);            
            string a;
            for (int x = 0; x < plane.GetLength(0); x++)
            {
                for (int y = 0; y < plane.GetLength(1); y++)
                {
                    if (plane[x, y])
                        a="1";
                    else a="0";
                    dataOut.Write(a);
                }
            }
            dataOut.Close();
        }

        string name; //имя файла
        //загрузка одного из паттернов
        private void gliderbutton_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            
            if (gliderbutton.Checked == true)
            {
                if ((radioButton3.Checked == true || radioButton4.Checked == true))
                {
                    newplane();
                    if (radioButton3.Checked == true)
                     {
                         name = Environment.CurrentDirectory + "/patterns/glider.txt";                        
                     }
                    else if (radioButton4.Checked == true)
                    {
                        name = Environment.CurrentDirectory + "/savings/pat1.bin";
                    }
                    loadpattern(name);
                }
                if (radioButton5.Checked == true)
                {
                    label1.Text = "Внимание!\nСохранение будет \nперезаписано.";
                }
            }
        }

        private void exploderbutton_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            
            if (exploderbutton.Checked == true)
            {
                if ((radioButton3.Checked == true || radioButton4.Checked == true))
            {
                newplane();
                if (radioButton3.Checked == true)
                {
                    name = Environment.CurrentDirectory + "/patterns/exploder.txt";
                }
                else if (radioButton4.Checked == true)
                {
                    name = Environment.CurrentDirectory + "/savings/pat5.bin";
                }
                loadpattern(name);
            }
                if(radioButton5.Checked==true)
                    label1.Text = "Внимание!\nСохранение будет \nперезаписано.";
            }
        }

        private void rowbutton_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";

            if (rowbutton.Checked == true)
            {
            if ((radioButton3.Checked == true || radioButton4.Checked == true))
            {
                newplane();
                if (radioButton3.Checked == true)
                {
                    name = Environment.CurrentDirectory + "/patterns/row.txt";
                }
                else if (radioButton4.Checked == true)
                {
                    name = Environment.CurrentDirectory + "/savings/pat4.bin";
                }
                loadpattern(name);
            }
            if (radioButton5.Checked == true)
                label1.Text = "Внимание!\nСохранение будет \nперезаписано.";
            }
        }

        private void spaceshipbutton_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            if (spaceshipbutton.Checked == true)
            {
                if ((radioButton3.Checked == true || radioButton4.Checked == true))
            {
                newplane();
                if (radioButton3.Checked == true)
                {
                    name = Environment.CurrentDirectory + "/patterns/spaceship.txt";
                }
                else if (radioButton4.Checked == true)
                {
                    name = Environment.CurrentDirectory + "/savings/pat3.bin";
                }
                loadpattern(name);
            }
            if (radioButton5.Checked == true)
                label1.Text = "Внимание!\nСохранение будет \nперезаписано.";
            }

        }

        private void tumbutton_Enter(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";

            if (tumbutton.Checked == true)
            {
                if ((radioButton3.Checked == true || radioButton4.Checked == true))
                {
                newplane();
                if (radioButton3.Checked == true)
                {
                    name = Environment.CurrentDirectory + "/patterns/tmb.txt";
                }
                else if (radioButton4.Checked == true)
                {
                    name = Environment.CurrentDirectory + "/savings/pat2.bin";
                }
                loadpattern(name);
                }
            if (radioButton5.Checked == true)
                label1.Text = "Внимание!\nСохранение будет \nперезаписано.";
            }
        }

        private void gunbutton_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";

            if (gunbutton.Checked == true)
            {
                if ((radioButton3.Checked == true || radioButton4.Checked == true))
            {
                newplane();
                if (radioButton3.Checked == true)
                {
                    name = Environment.CurrentDirectory + "/patterns/gun.txt";
                }
                else if (radioButton4.Checked == true)
                {
                    name = Environment.CurrentDirectory + "/savings/pat6.bin";
                }
                loadpattern(name);
            }
            if (radioButton5.Checked == true)
                label1.Text = "Внимание!\nСохранение будет \nперезаписано.";
            }
        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            groupBox2.Visible = true;
            button5.Visible = true;
            label1.Text = "";
            label2.Text = "";
            gliderbutton.Text = "Сохранение 1";
            tumbutton.Text = "Сохранение 2";
            spaceshipbutton.Text = "Сохранение 3";
            rowbutton.Text = "Сохранение 4";
            exploderbutton.Text = "Сохранение 5";
            gunbutton.Text = "Сохранение 6";
        }
    }
}
