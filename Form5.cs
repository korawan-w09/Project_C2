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
    public partial class Form5 : Form
    {

        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=kubota;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        public Form5()
        {
            InitializeComponent();
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


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form5_Load(object sender, EventArgs e)
        {
            //comboBox1.Items.Add("รุ่นDC70");
            //comboBox1.Items.Add("รุ่นDC95");
        }

        private void databasket_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
                cmd.CommandText = "SELECT รหัสสินค้า, name, รุ่น_DC,ราคา,จำนวน,x FROM stock_70 ";

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                adapter.Fill(ds);
                conn.Close();
                datastock.DataSource = ds.Tables[0].DefaultView;




            }
            else if (comboBox1.Text == "รุ่นDC95")
            {
                MySqlConnection conn = databaseConnection();
                DataSet ds = new DataSet();
                conn.Open();
                MySqlCommand cmd;

                cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT รหัสสินค้า,name,รุ่น_DC,ราคา,จำนวน,x FROM stock_95 ";

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(ds);
                conn.Close();
                datastock.DataSource = ds.Tables[0].DefaultView;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int code2 = int.Parse(code.Text);
            MySqlConnection conn = databaseConnection();

            String sql = "INSERT INTO basket_stock(x,name,รุ่น_DC,ราคา,จำนวน,รวม) VALUES('" + code.Text + "','" + name.Text + "' ,'" + Program.DC + "',' " + price.Text + "','" + textBox1.Text + "','" + total.Text + "')";
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

        private void datastock_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            datastock.CurrentRow.Selected = true;
            code.Text = datastock.Rows[e.RowIndex].Cells["x"].FormattedValue.ToString();
            name.Text = datastock.Rows[e.RowIndex].Cells["name"].FormattedValue.ToString();
            price.Text = datastock.Rows[e.RowIndex].Cells["ราคา"].FormattedValue.ToString();
            Program.DC = datastock.Rows[e.RowIndex].Cells["รุ่น_DC"].FormattedValue.ToString();
            //x_txt.Text = datastock.Rows[e.RowIndex].Cells["x"].FormattedValue.ToString();
            //total.Text = datastock.Rows[e.RowIndex].Cells["ราคา"].FormattedValue.ToString();
        }

        private void total_Click(object sender, EventArgs e)
        {
            int price2 = int.Parse(price.Text);
            int amount = int.Parse(textBox1.Text);
            int sum = price2 * amount;
            total.Text = sum.ToString();
        }

        private void databasket_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            databasket.CurrentRow.Selected = true;
            x_txt.Text = databasket.Rows[e.RowIndex].Cells["x"].FormattedValue.ToString();
            textBox5.Text = databasket.Rows[e.RowIndex].Cells["name"].FormattedValue.ToString();
            amount_edit.Text = databasket.Rows[e.RowIndex].Cells["จำนวน"].FormattedValue.ToString();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int a = int.Parse(textBox3.Text);
            int b = int.Parse(totaltext.Text);
            textBox4.Text = (a - b).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("คุณต้องการลบข้อมูลใช่หรือไม่??", "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation); //เงื่อนไขปุ่มลบ
            if (dialogResult == DialogResult.Yes)
            {
                int selectRow = databasket.CurrentCell.RowIndex;
                int deleteId = Convert.ToInt32(databasket.Rows[selectRow].Cells["ลำดับ"].Value);

                MySqlConnection conn = databaseConnection();

                string sql = "DELETE FROM basket_stock WHERE รหัสสินค้า='" + deleteId + "'";
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

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void btn_edit_Click(object sender, EventArgs e)
        {

        }
    }
}
