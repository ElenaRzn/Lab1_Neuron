using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Neuron2.Core
{
    [Serializable]
    public class Network
    {
        /// <summary>
        /// Количество входов
        /// </summary>
        protected int _inputsCount;

        /// <summary>
        /// Количество слоев
        /// </summary>
        protected int _layersCount;

        /// <summary>
        /// Слои
        /// </summary>
        protected Layer[] _layers;

        /// <summary>
        /// Выходной вектор сети
        /// </summary>
        protected double[] _output;
        /// <summary>
        /// Количество входов
        /// </summary>
        public int InputsCount
        {
            get { return _inputsCount; }
        }

        /// <summary>
        /// Слои
        /// </summary>
        public Layer[] Layers
        {
            get { return _layers; }
        }

        /// <summary>
        ///Выходной вектор
        ///Инициализируется при вызове метода "Compute"
        /// </summary>
        public double[] Output
        {
            get { return _output; }
        }



        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parInputsCount">количество входов</param>
        /// /// <param name="neuronsCount">количество нейронов в каждом слое.</param>
        public Network(int parInputsCount, params int[] parNeuronsCount)
        {
            this._inputsCount = Math.Max(1, parInputsCount);
            this._layersCount = Math.Max(1, parNeuronsCount.Length);
            // создание слоев
            this._layers = new Layer[this._layersCount];

            // содание каждого слоя
            for (int i = 0; i < parNeuronsCount.Length; i++)
            {
                _layers[i] = new Layer(
                    // neurons count in the layer
                    parNeuronsCount[i],
                    // inputs count of the layer
                    (i == 0) ? parInputsCount : parNeuronsCount[i - 1]);
            }
        }

        /// <summary>
        /// Вычисляет выходной вектор Нейронной сети
        /// </summary>
        /// <param name="parInput">входной вектор</param>
        /// <returns>Выходной вектор.</returns>

        public virtual double[] Compute(double[] input)
        {
            // копирование входного массива
            double[] output = input;

            // вычисление каждого слоя
            for (int i = 0; i < _layers.Length; i++)
            {
                output = _layers[i].Compute(output);
            }

            // запись в свойство
            this._output = output;

            return output;
        }

        /// <summary>
        /// Рандомизация всех слоев сети
        /// </summary>
        public virtual void Randomize()
        {
            foreach (Layer layer in _layers)
            {
                layer.Randomize();
            }
        }

        /// <summary>
        /// Сохранение сети в указанный файл
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        public void Save(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Save(stream);
            stream.Close();
        }

        /// <summary>
        /// Сохранение сети
        /// </summary>
        /// <param name="stream">Поток для сохранения</param>
        public void Save(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
        }

        /// <summary>
        /// Загрузка сети из файла
        /// </summary>
        /// <param name="fileName">Имя файла для загрузки</param>
        /// <returns>Объект сети, со всеми проинициализированными из файла полями</returns>
        public static Network Load(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Network network = Load(stream);
            stream.Close();

            return network;
        }

        /// <summary>
        /// Загрузка сети из файла
        /// </summary>
        /// <param name="stream">Поток для загрузки</param>
        /// <returns>Объект сети, со всеми проинициализированными из файла полями</returns>
        public static Network Load(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            Network network = (Network)formatter.Deserialize(stream);
            return network;
        }
    }
}
