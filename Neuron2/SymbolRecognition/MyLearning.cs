using Neuron2.Core;
using Neuron2.SymbolsGenerator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Neuron2.SymbolRecognition
{
    public class MyLearning
    {
        static LearningArguments arguments;
        static BaseMaker baseMaker;
        static double[,] percentage;
        static double totalErrors;
        static BackPropagationLearning teacher;
        static Network network;
        static char[] symbols = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        static int x = 0;
        /// <summary>
        /// Форма
        /// </summary>
        static Form form;
        static Chart success;

        static TextBox textBox;
        static Random rnd = new Random();

        static void ForEachWeight(Network network, Func<double, double> modifier)
        {
            foreach (var l in network.Layers)
                foreach (var n in l.Neurons)
                    for (int i = 0; i < n.Weights.Length; i++)
                        n.Weights[i] = modifier(n.Weights[i]);

        }

        static void Learn()
        {

            //создание нейронной сети с одним скрытым слоем
            network = new Network(
                baseMaker.InputSize,
                arguments.NeuronsCount,
                baseMaker.OutputSize
                );
            //рандомизируем нейронную сеть
            network.Randomize();
            foreach (var l in network.Layers)
                foreach (var n in l.Neurons)
                    for (int i = 0; i < n.Weights.Length; i++)
                        n.Weights[i] = rnd.NextDouble() * 2 - 1;

            teacher = new BackPropagationLearning(network);
            while (true)
            {
                //секция обучения
                var watch = new Stopwatch();
                watch.Start();
                while (watch.ElapsedMilliseconds < 500)
                {
                    //проводит итерацию обучения в цикле
                    teacher.RunEpoch(baseMaker.Inputs, baseMaker.Answers);

                }
                watch.Stop();
                //тестовый запуск
                var count = 0;
                percentage = new double[baseMaker.OutputSize, baseMaker.OutputSize];
                for (int i = 0; i < baseMaker.OutputSize; i++)
                    for (int j = 0; j < baseMaker.OutputSize * 5; j++)
                    {
                        //генерация случайного символа
                        double[] task;
                        lock (baseMaker)
                        {
                            task = baseMaker.GenerateRandom(i);
                        }
                        //анализ с помощью нейронной сети
                        var output = network.Compute(task);
                        //вычисление максимума выхода нейронной сети
                        var max = output.Max();
                        //вычисление номера нейрона, который выдал этот максимум
                        var maxIndex = Enumerable.Range(0, output.Length).Where(z => output[z] == max).First();
                        //заполнение квадратной матрицы, которая показ, сколько
                        //раз i символ был принят за символ с номером maxIndex
                        percentage[i, maxIndex]++;
                        if (i != maxIndex) totalErrors++;
                        count++;
                    }
                //общее к-во ошибок \ к-во экспериментов 
                //получаем вероятность того, что нейронная сеть совершит ошибку
                totalErrors /= count;
                //перерисовка
                form.BeginInvoke(new Action(Update));
                //success.Series["Error"].Points.AddXY(x, totalErrors);

            }
        }
        static void Update()
        {
            textBox.Text = totalErrors.ToString();
            success.Series["Error"].Points.AddXY(x, totalErrors);
            x++;
        }



        public static void Run(LearningArguments _arguments)
        {
            arguments = _arguments;
            baseMaker = _arguments.BaseMaker;
            baseMaker.Generate();

            textBox = new TextBox()
            {
                Dock = DockStyle.Fill,
                AllowDrop = false
            };

            success = new Chart
            {
                Dock = DockStyle.Fill,
                Size = new Size(400, 400),
                AllowDrop = false
            };
            ChartArea chartArrea1 = new ChartArea
            {
                Name = "ChartArre1",
                AxisY = new Axis
                {
                    Maximum = 1,
                }
            };
            success.ChartAreas.Add("ChartArrea1");
            success.ChartAreas["ChartArrea1"].AxisY.MajorGrid.Enabled = false;
            success.ChartAreas["ChartArrea1"].AxisX.MajorGrid.Enabled = false;
            success.Series.Add("Error");
            success.Series["Error"].ChartType = SeriesChartType.Spline;
            var buttonTeach = new Button
            {
                Text = "Teach",
                Dock = DockStyle.Fill,
                AllowDrop = false
            };

            var buttonTest = new Button
            {
                Text = "Test",
                Dock = DockStyle.Fill,
                AllowDrop = false
            };
            var textAnswer = new TextBox
            {
                Text = "",
                Dock = DockStyle.Fill,
                AllowDrop = false
            };
            var textSymbol = new TextBox
            {
                Text = "",
                Dock = DockStyle.Fill,
                AllowDrop = false
            };

            var labelTestError = new Label
            {
                Text = "Test Error",
                AutoSize = false,
                AllowDrop = true

            };

            var textBoxTestError = new TextBox
            {
                Text = "",
                Dock = DockStyle.Fill,
                AllowDrop = false
            };

            //секция тестирования

            buttonTest.Click += (sender, args) =>
            {
                var testCount = 0;
                var testErrors = 0.0;
                textSymbol.Text = "";
                textAnswer.Text = "";
                for (int k=0; k<10; k++)
                {
                    var i = rnd.Next(7);
                    //генерация случайного символа
                    double[] task;
                    lock (baseMaker)
                    {
                        task = baseMaker.GenerateRandom(i);
                    }
                    for (int p = 0; p<30; p++)
                    {
                        var noise = rnd.Next(task.Length-1);
                        task[noise] = 1;
                    }

                    textSymbol.Text += symbols[i].ToString() + " ";
                    //анализ с помощью нейронной сети
                    var output = network.Compute(task);
                    //вычисление максимума выхода нейронной сети
                    var max = output.Max();
                    //вычисление номера нейрона, который выдал этот максимум
                    var maxIndex = Enumerable.Range(0, output.Length).Where(z => output[z] == max).First();
                    textAnswer.Text += symbols[maxIndex].ToString() + " ";
                    if (symbols[i] != symbols[maxIndex])
                        testErrors++;
                    testCount++;
                }
                double testError =  testErrors / testCount;
                textBoxTestError.Text = testError.ToString();

            };
            var buttonUse = new Button
            {
                Text = "Use",
                Dock = DockStyle.Fill,
                AllowDrop = false
            };
           


            //главная панель
            var tableMain = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                Size = new Size(800, 400),
                AllowDrop = false
            };
            tableMain.RowStyles.Clear();
            tableMain.RowStyles.Add(new RowStyle(SizeType.Percent, 95));
            tableMain.RowStyles.Add(new RowStyle(SizeType.Percent, 5));
            tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            tableMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            //павнель с кнопками
            var table = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                Size = new Size(400, 400),
                AllowDrop = false
            };
            table.RowStyles.Clear();
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));

            table.Controls.Add(buttonTeach, 0, 0);
            table.Controls.Add(buttonTest, 0, 1);
            table.Controls.Add(labelTestError, 0, 2);
            table.Controls.Add(textBoxTestError, 1, 2);
            table.Controls.Add(buttonUse, 0, 3);
            table.Controls.Add(textSymbol, 1, 0);
            table.Controls.Add(textAnswer, 1, 1);

            //панель для спользования
            var tableUse = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                AllowDrop = false
            };
            tableUse.RowStyles.Clear();
            tableUse.RowStyles.Add(new RowStyle(SizeType.Percent, 90));
            tableUse.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            tableUse.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            var buttonInput = new Button()
            {
                Text = "Input",
                AllowDrop = false
            };

            var grid = new DataGridView()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 16,
                RowCount = 16,
                AutoSize = true,
                ColumnHeadersVisible = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                AllowDrop = false
            };
            grid.CellClick += (sender, args) =>
            {
                DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)
                                grid.Rows[args.RowIndex].Cells[args.ColumnIndex];
                if (cell.Style.BackColor == System.Drawing.Color.Black)
                {
                    cell.Value = 0.0;
                    cell.Style.BackColor = System.Drawing.Color.White;
                }

                else
                {
                    cell.Value = 1.0;
                    cell.Style.BackColor = System.Drawing.Color.Black;
                }

            };
            tableUse.Controls.Add(grid, 0, 0);
            tableUse.Controls.Add(buttonInput, 0, 1);


            table.Controls.Add(tableUse, 1, 3);
            //заполнение главной панели
            tableMain.Controls.Add(table, 0, 0);
            tableMain.Controls.Add(success, 1, 0);
            tableMain.Controls.Add(textBox, 1, 1);
            //создание окошка
            form = new Form()
            {
                Text = "Symbols recognition",
                ClientSize = new Size(800, 400),
                Controls =
                {
                    tableMain
                }
            };

            buttonUse.Click += (sender, args) =>
            {
                var i = rnd.Next(7);
                //генерация случайного символа
                double[] task;
                lock (baseMaker)
                {
                    task = baseMaker.GenerateRandom(i);
                }
                textSymbol.Text = symbols[i].ToString();
                textAnswer.Text = "";

                for (int k=0; k<16; k++)
                {
                    for (int j=0; j<16; j++)
                    {
                        grid[k, j].Value = 0;
                        grid[k, j].Style.BackColor = System.Drawing.Color.White;
                        grid[k, j].Value = task[k * 16 + j];
                        if (task[k*16+j]>0)
                            grid[k,j].Style.BackColor = System.Drawing.Color.Black;

                    }
                }
            };

            buttonInput.Click += (sender, args) =>
            {

                double[] task = new double[16 * 16];
                for (int k = 0; k < 16; k++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        task[k * 16 + j] = (double)grid[k, j].Value;

                    }
                }

                //анализ с помощью нейронной сети
                var output = network.Compute(task);
                //вычисление максимума выхода нейронной сети
                var max = output.Max();
                //вычисление номера нейрона, который выдал этот максимум
                var maxIndex = Enumerable.Range(0, output.Length).Where(z => output[z] == max).First();
                textAnswer.Text = symbols[maxIndex].ToString();
            };

            //запуск в отдельном потоке метода learn
            new Action(Learn).BeginInvoke(null, null);
            


            Application.Run(form);

        }
    }
}
