using Neuron2.SymbolRecognition;
using Neuron2.SymbolsGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neuron2
{
    //static class Program
    //{
    //    /// <summary>
    //    /// Главная точка входа для приложения.
    //    /// </summary>
    //    [STAThread]
    //    static void Main()
    //    {
    //        Application.EnableVisualStyles();
    //        Application.SetCompatibleTextRenderingDefault(false);
    //        Application.Run(new Form1());
    //    }
    //}

    /// <summary>
    /// Класс аргументов обучения
    /// </summary>
    public class LearningArguments
    {
        /// <summary>
        /// Генератор обучающей базы (символов)
        /// </summary>
        public BaseMaker BaseMaker;
        /// <summary>
        /// Количество нейронов
        /// </summary>
        public int NeuronsCount;
    }


    class Program
    {
        static LearningArguments task1 = new LearningArguments
        {
            BaseMaker = new BaseMaker
            {
                //символы
                Symbols = "abcdefgh",
                //углы поворота от и до
                MinAngle = 0,
                MaxAngle = 60,
                //шаг угла поворота
                DeltaAngle = 10,
                //показывает всю выборку
                ShowWhenCreated = true,
            },
            NeuronsCount = 30
        };

        static LearningArguments task2 = new LearningArguments
        {
            BaseMaker = new BaseMaker
            {
                Symbols = "abcdefgh",
                MinAngle = 0,
                MaxAngle = 60,
                DeltaAngle = 5,
                ShowWhenCreated = true,
            },
            NeuronsCount = 30
        };

        static LearningArguments task3 = new LearningArguments
        {
            BaseMaker = new BaseMaker
            {
                Symbols = "abcdefgh",
                MinAngle = 0,
                MaxAngle = 60,
                DeltaAngle = 5,
                NoiseLevel = 0.05,
                ShowWhenCreated = true,
            },
            NeuronsCount = 30
        };

        static LearningArguments task4 = new LearningArguments
        {
            BaseMaker = new BaseMaker
            {
                Symbols = "abcdefgh",
                MinAngle = 0,
                MaxAngle = 60,
                DeltaAngle = 5,
                NoiseLevel = 0.05,
                ShowWhenCreated = true,
            },
            NeuronsCount = 60
        };

        static LearningArguments task5 = new LearningArguments
        {
            BaseMaker = new BaseMaker
            {
                Symbols = "abcdefgh",
                MinAngle = 0,
                MaxAngle = 40,
                DeltaAngle = 3,
                NoiseLevel = 0,
                ShowWhenCreated = true,
            },
            NeuronsCount = 60
        };


        [STAThreadAttribute]
        public static void Main()
        {
            MyLearning.Run(task5);

        }
    }
}
