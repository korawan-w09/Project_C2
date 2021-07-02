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

namespace project
{
    public partial class Form4 : Form
    {
        
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=kubota;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;


        }
       
        public Form4()
        {
            InitializeComponent();

         
        }
        private void showraka() //โชว์ราคาในTexbox3อัน
        {

            MySqlConnection conn = databaseConnection();
            conn.Open();
            MySqlCommand cmd;
            cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COALESCE(sum(รวม),0)  FROM basket_stock ";
            MySqlDataReader rowss = cmd.ExecuteReader();
            rowss.Read();
            Program.check = rowss.GetString("COALESCE(sum(รวม),0)");
            totaltext.Text = Program.check;
            
                    
         
            

        }


        private void Form4_Load(object sender, EventArgs e)
        {
            //comboBox1.Items.Add("รุ่นDC70");
            //comboBox1.Items.Add("รุ่นDC95");
        }
        private void basket_stock() //เป็นคำสั่งที่ไว้เรียกใช้เวลาแสดงรายการสินค้าที่สั่งซื้อ
        {
            MySqlConnection conn = databaseConnection();
            DataSet ds = new DataSet();
            conn.Open();
            MySqlCommand cmd;

            cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT รหัสสินค้า AS ลำดับ,name,รุ่น_DC,ราคา,จำนวน,รวม,time,x FROM basket_stock ";

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(ds);
            conn.Close();
            databasket.DataSource = ds.Tables[0].DefaultView;
            
            

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) //คำสั่งที่เลือกรุ่นในcombobox
        {
            if (comboBox1.Text == "รุ่นDC70")
            {
                MySqlConnection conn = databaseConnection();
                DataSet ds = new DataSet();
                conn.Open();
                MySqlCommand cmd;

                cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT รหัสสินค้า, name, รุ่น_DC,ราคา,x FROM stock_70 "; 

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                
                adapter.Fill(ds);
                conn.Close();
                datastock.DataSource = ds.Tables[0].DefaultView;
                
                


            }
            else if(comboBox1.Text == "รุ่นDC95")
            {
                MySqlConnection conn = databaseConnection();
                DataSet ds = new DataSet();
                conn.Open();
                MySqlCommand cmd;

                cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT รหัสสินค้า,name,รุ่น_DC,ราคา,x FROM stock_95 ";

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(ds);
                conn.Close();
                datastock.DataSource = ds.Tables[0].DefaultView;
                
            }
        }

        private void datastock_CellClick(object sender, DataGridViewCellEventArgs e) //นำข้อมูลไปใส่Textbox
        {
            datastock.CurrentRow.Selected = true;
            code.Text = datastock.Rows[e.RowIndex].Cells["x"].FormattedValue.ToString();
            name.Text = datastock.Rows[e.RowIndex].Cells["name"].FormattedValue.ToString();
            price.Text = datastock.Rows[e.RowIndex].Cells["ราคา"].FormattedValue.ToString();
            Program.DC = datastock.Rows[e.RowIndex].Cells["รุ่น_DC"].FormattedValue.ToString();
            //x_txt.Text = datastock.Rows[e.RowIndex].Cells["x"].FormattedValue.ToString();
            //total.Text = datastock.Rows[e.RowIndex].Cells["ราคา"].FormattedValue.ToString();
            
            
        }

        private void total_Click(object sender, EventArgs e) //คำนวณราคาสินค้า
        {
            int price2 = int.Parse(price.Text);
            int amount = int.Parse(textBox1.Text);
            int sum = price2 * amount;
            total.Text = sum.ToString();

            

        }

        private void button1_Click(object sender, EventArgs e)//ปุ่มเพิ่มสินค้า
        {
            int code2 = int.Parse(code.Text);
            MySqlConnection conn = databaseConnection();

            String sql = "INSERT INTO basket_stock(x,name,รุ่น_DC,ราคา,จำนวน,รวม) VALUES('"+ code.Text+"','" + name.Text + "' ,'" + Program.DC + "',' "+price.Text+"','" + textBox1.Text + "','" + total.Text + "')";
            MySqlCommand cmd = new MySqlCommand(sql, conn);

            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            conn.Close();
            if (rows > 0)
            {
                

                basket_stock();//เรียกแสดงข้อมูลใหม่
                showraka();   
            }
        }

        private void databasket_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            databasket.CurrentRow.Selected = true;
            x_txt.Text = databasket.Rows[e.RowIndex].Cells["x"].FormattedValue.ToString();
            textBox5.Text = databasket.Rows[e.RowIndex].Cells["name"].FormattedValue.ToString();


        }
        
        private void button2_Click(object sender, EventArgs e) //ปุ่มลบรายการ
        {
            DialogResult dialogResult = MessageBox.Show("คุณต้องการลบข้อมูลใช่หรือไม่??", "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation); //เงื่อนไขปุ่มลบ
            if (dialogResult == DialogResult.Yes)
            {
                int selectRow = databasket.CurrentCell.RowIndex;
                int deleteId = Convert.ToInt32(databasket.Rows[selectRow].Cells["ลำดับ"].Value);

                MySqlConnection conn = databaseConnection();

                string sql = "DELETE FROM basket_stock WHERE รหัสสินค้า='"+ deleteId +"'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                conn.Open();

                int rows = cmd.ExecuteNonQuery();
                conn.Close();

                if (rows > 0)
                {
                    MessageBox.Show("ลบข้อมูลสำเร็จ", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    basket_stock();
                    showraka();
                   
                }
            }
            else
            { }

        }

        private void button3_Click(object sender, EventArgs e) //คำนวณราคารวม เงินทอน 
        {
            int a = int.Parse(textBox3.Text);
            int b = int.Parse(totaltext.Text);
            textBox4.Text = (a-b).ToString();
        }

        private List<Class1> allbook = new List<Class1>();

        private void button4_Click(object sender, EventArgs e)
        {
            allbook.Clear();
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) //โค้ดสลิป
        {
            //Image logo = Image.FromFile(@"C:\Users\qqx99\Documents\Angkun_C#\project\image\name2.png");
            //e.Graphics.DrawImage(logo, new PointF(50, 50));
            e.Graphics.DrawString("ใบเสร็จ", new Font("TH SarabunPSK", 26, FontStyle.Bold), Brushes.Black, new PointF(350, 80));
            e.Graphics.DrawString("ร้าน Wonderland ขายอะไหล่รถเกี่ยว", new Font("TH SarabunPSK", 20, FontStyle.Bold), Brushes.Black, new PointF(240, 120));
            //e.Graphics.DrawString("-------------------------------------------------------------------------------------------------------------------------------------", new Font("TH SarabunPSK", 18, FontStyle.Bold), Brushes.Black, new PointF(50, 160));
            e.Graphics.DrawString("ข้อมูลร้าน :  84/17 บ้านหนองขุ่น ตำบลมิตรภาพ อำเภอแกดำ จังหวัดมหาสารคาม 44190 ", new Font("TH SarabunPSK", 18, FontStyle.Bold), Brushes.Black, new PointF(50, 180));
            //e.Graphics.DrawString("หน่วยงาน/คณะ " + Program.facutyadmin, new Font("TH SarabunPSK", 18, FontStyle.Bold), Brushes.Black, new PointF(400, 192));
            e.Graphics.DrawString("วันที่และเวลา :  " + System.DateTime.Now.ToString("dd / MM / yyyy   HH : mm : ss น."), new Font("TH SarabunPSK", 18, FontStyle.Bold), Brushes.Black, new PointF(50, 215));
            //e.Graphics.DrawString("พิมพ์โดย " + Program.admin, new Font("TH SarabunPSK", 18, FontStyle.Bold), Brushes.Black, new PointF(400, 215));
            e.Graphics.DrawString("-------------------------------------------------------------------------------------------------------------------------------", new Font("TH SarabunPSK", 18, FontStyle.Bold), Brushes.Black, new PointF(50, 240));
            e.Graphics.DrawString("ลำดับ", new Font("TH SarabunPSK", 18, FontStyle.Bold), Brushes.Black, new PointF(60, 255));
            e.Graphics.DrawString("ชื่อสินค้า", new Font("TH SarabunPSK", 18, FontStyle.Bold), Brushes.Black, new PointF(150, 255));
            e.Graphics.DrawString("ราคา(บาท)", new Font("TH SarabunPSK", 18, FontStyle.Bold), Brushes.Black, new PointF(330, 255));
            e.Graphics.DrawString("รุ่น_DC", new Font("TH SarabunPSK", 18, FontStyle.Bold), Brushes.Black, new PointF(470, 255));
            e.Graphics.DrawString("จำนวน", new Font("TH SarabunPSK", 18, FontStyle.Bold), Brushes.Black, new PointF(650, 255));
            e.Graphics.DrawString("-------------------------------------------------------------------------------------------------------------------------------", new Font("TH SarabunPSK", 18, FontStyle.Bold), Brushes.Black, new PointF(50, 265));
            int y = 290;
            allbook.Clear();

            loaddata();
            foreach (var i in allbook)
            {
                e.Graphics.DrawString(i.id, new Font("TH SarabunPSK", 16, FontStyle.Regular), Brushes.Black, new PointF(80, y));
                e.Graphics.DrawString(i.name, new Font("TH SarabunPSK", 16, FontStyle.Regular), Brushes.Black, new PointF(140, y));
                e.Graphics.DrawString(i.price, new Font("TH SarabunPSK", 16, FontStyle.Regular), Brushes.Black, new PointF(360, y));
                e.Graphics.DrawString(i.generation, new Font("TH SarabunPSK", 16, FontStyle.Regular), Brushes.Black, new PointF(495, y));
                e.Graphics.DrawString(i.amount, new Font("TH SarabunPSK", 16, FontStyle.Regular), Brushes.Black, new PointF(680, y));
                y = y + 20;


            }
            e.Graphics.DrawString("----------------------------------------------------------------------------------------------------------------------------------------", new Font("TH SarabunPSK", 18, FontStyle.Regular), Brushes.Black, new Point(50, y + 0));
            e.Graphics.DrawString("รวมทั้งสิ้น    " + totaltext.Text + "          บาท", new Font("TH SarabunPSK", 18, FontStyle.Regular), Brushes.Black, new Point(570, (y + 50) + 45));
            //e.Graphics.DrawString("ชื่อผู้ให้บริการ    " + Program.userid.ToString(), new Font("DB Helvethaica X v3.2", 16, FontStyle.Bold), Brushes.Black, new Point(80, (y + 30) + 45));
            e.Graphics.DrawString("รับเงิน        " + textBox3.Text + "           บาท", new Font("TH SarabunPSK", 18, FontStyle.Regular), Brushes.Black, new Point(570, ((y + 30) + 45) + 60));
            e.Graphics.DrawString("เงินทอน      " + textBox4.Text + "              บาท", new Font("TH SarabunPSK", 18, FontStyle.Regular), Brushes.Black, new Point(570, (((y + 30) + 45) + 45) + 60));
            e.Graphics.DrawString("ร้านWonderland ขายอะไหล่รถเกี่ยว", new Font("TH SarabunPSK", 18, FontStyle.Regular), Brushes.Black, new Point(290, ((((y + 30) + 115) + 45) + 45) + 60));
            e.Graphics.DrawString("       ขอบคุณที่ใช้บริการ      ", new Font("TH SarabunPSK", 18, FontStyle.Regular), Brushes.Black, new Point(290, (((((y + 30) + 115) + 45) + 45) + 45) + 60));

        }
        private void loaddata()
        {
            MySqlConnection conn = databaseConnection();

            conn.Open();

            
            
            

                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM basket_stock  ", conn);
                MySqlDataReader adapter = cmd.ExecuteReader();

                while (adapter.Read())
                {
                    Program.id = adapter.GetString("รหัสสินค้า");
                    Program.name = adapter.GetString("name");
                    Program.price = adapter.GetString("ราคา");
                    Program.generation = adapter.GetString("รุ่น_DC");
                    Program.amount = adapter.GetString("จำนวน");
                    Class1 item = new Class1()
                    {
                        id = Program.id,
                        name = Program.name,
                        price = Program.price,
                        generation = Program.generation,
                        amount = Program.amount

                    };
                    allbook.Add(item);
                }
            conn.Close();
            
        }

        private void totaltext_TextChanged(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {
          
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void datastock_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }
    }
}
