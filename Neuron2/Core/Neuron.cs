using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neuron2.Core
{
    /// <summary>
    /// Нейрон
    /// </summary>
    [Serializable]
    public class Neuron
    {
        /// <summary>
        ///Входное количество
        /// </summary>
        protected int _inputsCount = 0;

        /// <summary>
        /// Веса
        /// </summary>
        protected double[] _weights = null;

        /// <summary>
        /// Выходное значение
        /// </summary>
        protected double _output = 0;

        /// <summary>
        /// Генератор для рандомизации весов
        /// *******************************************
        /// </summary>
        protected static Random _rand = new Random();

        /// <summary>
        /// Входное количество нейронов
        /// </summary>
        public int InputsCount
        {
            get { return _inputsCount; }
        }

        /// <summary>
        /// Выходное значение нейронов
        /// Способ вычисления выходных значений нейронов определяется в классе наследнике
        /// </summary>
        public double Output
        {
            get { return _output; }
        }


        /// <summary>
        /// Веса нейрона
        /// </summary>
        public double[] Weights
        {
            get { return _weights; }
        }

        /// <summary>
        /// Пороговое значение
        /// Добавляется к сумме весов перед активацией
        /// </summary>
        protected double _threshold = 0.0;

        /// <summary>
        /// Активационная функция. Применяется к сумме веслв + пороговому значению
        /// </summary>

        //SigmoidFunction
        protected IActivationFunction _function = new SigmoidFunction();

        public IActivationFunction Function
        {
            get { return _function; }
        }

        /// <summary>
        /// Пороговое значение
        /// Добавляется к сумме весов перед активацией
        /// </summary>
        public double Threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public Neuron(int parInputs)
        {
            // назначение весов
            _inputsCount = Math.Max(1, parInputs);
            _weights = new double[_inputsCount];
            // Рандомизация
            Randomize();
        }

        /// <summary>
        /// Рандомизация
        /// Инициализация весов со случайными дначениями в заданном диапазоне
        /// </summary>
        public virtual void Randomize()
        {
            // рандомизация
            for (int i = 0; i < _inputsCount; i++)
                _weights[i] = _rand.NextDouble() * 2 - 1;

            // рандомизация порогового значения
            _threshold = _rand.NextDouble() * 2 - 1;
        }

        /// <summary>
        /// Подсчет выходного значения
        /// </summary> 
        /// <param name="parInput">Входной вектор</param>
        /// <returns>Выходное значение нейрона</returns>
        /// <remarks><para>Выходное значение нейрона - это значение активационной функции,
        /// от суммы весов + пороговое значение. Также сохраняется в свойстве Output
        /// <exception cref="ArgumentException">Длина входного вектора не совпадает с ожидаемым 
        /// значением в InputsCount</see>.</exception>
        public double Compute(double[] parInput)
        {
            // проверка корректности входного вектора
            if (parInput.Length != _inputsCount)
                throw new ArgumentException("Wrong length of the input vector.");

            // инициализация суммы
            double sum = 0.0;

            // расчет суммы вес*входное значение
            for (int i = 0; i < _weights.Length; i++)
            {
                sum += _weights[i] * parInput[i];
            }
            sum += _threshold;

            double output = _function.Function(sum);
            this._output = output;

            return output;
        }
    }
}
