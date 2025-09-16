
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
namespace SchoolProject
{
    public partial class Form5 : Form
    {
        string seriesName = "myData";
        List<double> x = new List<double>();
        List<double> y = new List<double>();

        DataTable charactersTable = new DataTable();
        string characterSeriesName = "Attractiveness";

        public Form5()
        {
            InitializeComponent();
            // Fill chart types
            comboBox1.Items.AddRange(Enum.GetNames(typeof(SeriesChartType)));

            // Create table only once
            charactersTable.Columns.Add("CharacterID", typeof(int));
            charactersTable.Columns.Add("Name", typeof(string));
            charactersTable.Columns.Add("House", typeof(string));
            charactersTable.Columns.Add("AttractivenessScore", typeof(int));

            charactersTable.Rows.Add(1, "Jon Snow", "Stark", 2);
            charactersTable.Rows.Add(2, "Daenerys", "Targaryen", 10);
            charactersTable.Rows.Add(3, "Tyrion", "Lannister", 7);
            charactersTable.Rows.Add(4, "Arya Stark", "Stark", 2);
            charactersTable.Rows.Add(5, "Jaime", "Lannister", 6);
            charactersTable.Rows.Add(6, "Ise", "Lannister", 5);
            charactersTable.Rows.Add(7, "Khan", "Mongolia", 4);
            charactersTable.Rows.Add(8, "SuperMan", "Space", 3);
            charactersTable.Rows.Add(9, "Mel", "USA", 8);
            charactersTable.Rows.Add(10, "Alia", "Pakistan", 6);
            charactersTable.Rows.Add(11, "Jay", "Florida", 9);
            LoadCharacterChart(SeriesChartType.Pyramid);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
                return;

            string selectedType = comboBox1.SelectedItem.ToString();

            if (Enum.TryParse(selectedType, out SeriesChartType chartType))
            {
                LoadCharacterChart(chartType); // Just reload with the new chart type
            }
        }

        private void PlotMathFunction()
        {
            //chart1.Series.Clear();
            //chart1.ChartAreas.Clear();

            //ChartArea area = new ChartArea("MainArea");
            //area.AxisX.Minimum = x.Min();
            //area.AxisX.Maximum = x.Max();
            //area.AxisX.Interval = 1;
            //area.AxisX.TitleForeColor = Color.Gold;
            //chart1.ChartAreas.Add(area);

            //Series series = new Series(seriesName)
            //{
            //    ChartType = SeriesChartType.Line,
            //    Color = Color.DarkSlateGray,
            //    ChartArea = "MainArea"
            //};

            //series.Points.DataBindXY(x, y);
            //chart1.Series.Add(series);

            //chart1.BackColor = Color.LightBlue;
            //comboBox1.Enabled = true;
        }


        private void plot_Click(object sender, EventArgs e)
        {
            PlotMathFunction();
        }
        private void LoadCharacterChart(SeriesChartType chartType)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            ChartArea area = new ChartArea("MainArea");
            chart1.ChartAreas.Add(area);

            Series series = new Series(characterSeriesName)
            {
                ChartType = chartType,
                Color = Color.MediumPurple,
                XValueMember = "Name",
                YValueMembers = "AttractivenessScore",
                IsValueShownAsLabel = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            chart1.Series.Add(series);
            chart1.DataSource = charactersTable;
            chart1.DataBind();

            chart1.Titles.Clear();
            chart1.Titles.Add("Game of Thrones Character Attractiveness");
        }



        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += PrintDoc_PrintPage;

            PrintPreviewDialog previewDialog = new PrintPreviewDialog
            {
                Document = printDoc
            };
            previewDialog.ShowDialog();
        }
       
          private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Capture the chart as a bitmap
            using (Bitmap chartBitmap = new Bitmap(chart1.Width, chart1.Height))
            {
                chart1.DrawToBitmap(chartBitmap, new Rectangle(0, 0, chart1.Width, chart1.Height));

                // Get printable area of the page (with margins)
                RectangleF marginBounds = e.MarginBounds;

                // Optional: maintain aspect ratio
                float scale = Math.Min(marginBounds.Width / chartBitmap.Width, marginBounds.Height / chartBitmap.Height);
                int scaledWidth = (int)(chartBitmap.Width * scale);
                int scaledHeight = (int)(chartBitmap.Height * scale);

                // Center the chart on the page
                int x = (int)(marginBounds.Left + (marginBounds.Width - scaledWidth) / 2);
                int y = (int)(marginBounds.Top + (marginBounds.Height - scaledHeight) / 2);

                // Draw scaled chart image
                e.Graphics.DrawImage(chartBitmap, new Rectangle(x, y, scaledWidth, scaledHeight));
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            //LoadCharactersChart();
        }
    }








}


