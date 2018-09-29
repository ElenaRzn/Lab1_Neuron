using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neuron2.Core
{
    /// <summary>
    /// Слой нейронной сети
    /// </summary>
    [Serializable]
    public class Layer
    {
        /// <summary>
        /// Количество слоев
        /// </summary>
        protected int _inputsCount = 0;

        /// <summary>
        ///Количество нейронов в слое
        /// </summary>
        protected int _neuronsCount = 0;

        /// <summary>
        /// Нейроны слоя
        /// </summary>
        protected Neuron[] _neurons;

        /// <summary>
        /// Выходной вектор слоя
        /// </summary>
        protected double[] _output;

        /// <summary>
        /// Количество слоев
        /// </summary>
        public int InputsCount
        {
            get { return _inputsCount; }
        }

        /// <summary>
        ///Количество нейронов в слое
        /// </summary>
        /// 
        public Neuron[] Neurons
        {
            get { return _neurons; }
        }

        /// <summary>
        /// Выходной вектор слоя
        /// Вычисление определяется нейронами, составляющими слой
        /// Свойство инициализируется после вызова метода Compute
        /// </summary>
        public double[] Output
        {
            get { return _output; }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parNeuronsCount">Количество нейронов</param>
        /// <param name="parInputsCount">Количество слоев</param>
        /// <param name="parFunction">Активационная функция нейронов слоя</param>
        public Layer(int parNeuronsCount, int parInputsCount)
        {
            this._inputsCount = Math.Max(1, parInputsCount);
            this._neuronsCount = Math.Max(1, parNeuronsCount);
            // создание массива нейронов
            _neurons = new Neuron[this._neuronsCount];
            // создание каждого нейрона
            for (int i = 0; i < _neurons.Length; i++)
                _neurons[i] = new Neuron(parInputsCount);
        }

        /// <summary>
        /// Вычисление выходного вектора слоя
        /// Определется нейронами, составляющими слой.
        /// </summary>
        /// <param name="parInput">Входной вектор.</param>
        /// <returns>Выходной вектор слоя.</returns>

        public virtual double[] Compute(double[] input)
        {
            // Создание переменной для хранения выходного вектора
            double[] output = new double[_neuronsCount];

            // вычисление для каждого нейрона
            for (int i = 0; i < _neurons.Length; i++)
                output[i] = _neurons[i].Compute(input);

            // Сохранение в свойтве
            this._output = output;

            return output;
        }

        /// <summary>
        /// Рандомизация слоя
        /// </summary>
        public virtual void Randomize()
        {
            foreach (Neuron neuron in _neurons)
                neuron.Randomize();
        }
    }
}
