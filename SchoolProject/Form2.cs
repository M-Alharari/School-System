using SchoolProjectBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolProject
{
    public partial class Form2 : Form
    {
        private static DataTable _dtAllPeople = clsPerson.GetAllPeople();
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //kryptonDataGridView1.DataSource = _dtAllPeople;
        }
    }
}
