using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neuron2.Core
{
    /// <summary>
    /// Алгоритм обратного распространения ошибки
    /// </summary>
    public class BackPropagationLearning
    {
        // сеть для обучения
        private Network network;
        // скорость обучения
        private double learningRate = 1;
        // коэффициент для изменения весов
        private double momentum = 0.0;

        /// <summary>
        /// Ошибка нейрона
        /// </summary>

        private double[][] neuronErrors = null;
        /// <summary>
        ///Изменения весов
        /// </summary>
        private double[][][] weightsUpdates = null;
        /// <summary>
        /// Изменения порогов
        /// </summary>
        private double[][] thresholdsUpdates = null;


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="parNetwork">Сеть для обучения</param>
        public BackPropagationLearning(Network parNetwork)
        {
            this.network = parNetwork;

            neuronErrors = new double[network.Layers.Length][];
            weightsUpdates = new double[network.Layers.Length][][];
            thresholdsUpdates = new double[network.Layers.Length][];

            // инициализация массивов для каждого слоя
            for (int i = 0; i < network.Layers.Length; i++)
            {
                Layer layer = network.Layers[i];

                neuronErrors[i] = new double[layer.Neurons.Length];
                weightsUpdates[i] = new double[layer.Neurons.Length][];
                thresholdsUpdates[i] = new double[layer.Neurons.Length];

                // цикл по нейронам
                for (int j = 0; j < weightsUpdates[i].Length; j++)
                {
                    weightsUpdates[i][j] = new double[layer.InputsCount];
                }
            }
        }

        /// <summary>
        /// Запуск одной итерации обучения (изменения весов)
        /// </summary>
        /// <param name="parInput">Входной вектор</param>
        /// <param name="parOutput">Ожидаемый выходной вектор</param>
        /// <returns>СКО.</returns>

        public double Run(double[] parInput, double[] parOutput)
        {
            // Вычисление выходного вектора сети
            network.Compute(parInput);

            // ошибка сети
            double error = CalculateError(parOutput);

            // изменение весов
            CalculateUpdates(parInput);

            // обновление сети
            UpdateNetwork();

            return error;
        }

        /// <summary>
        /// Запуск обучение для эпохи, вызывает метод Run для каждого входного вектора
        /// </summary>
        /// <param name="parInput">Массив входных векторов</param>
        /// <param name="parOutput">Массив выходных векторов</param>
        /// <returns>ссумарная ошибка обучения для эпохи</returns>
        public double RunEpoch(double[][] parInput, double[][] parOutput)
        {
            double error = 0.0;

            for (int i = 0; i < parInput.Length; i++)
            {
                error += Run(parInput[i], parOutput[i]);
            }
            return error;
        }


        /// <summary>
        /// Вычисление ошибки для всех нейронов сети
        /// </summary>
        /// <param name="parDesiredOutput">Ожидаемый выходной вектор</param>
        /// <returns>СКО/2.</returns>
        private double CalculateError(double[] parDesiredOutput)
        {
            // текущий и следующий слои
            Layer layer, layerNext;
            // текущий и следующий массивы ошибки
            double[] errors, errorsNext;
            // значения ошибки
            double error = 0, e, sum;
            // выходное значение нейрона
            double output;
            // число слоев
            int layersCount = network.Layers.Length;

            // cошибка последнего слоя
            layer = network.Layers[layersCount - 1];
            errors = neuronErrors[layersCount - 1];

            for (int i = 0; i < layer.Neurons.Length; i++)
            {
                output = layer.Neurons[i].Output;
                // ошибка нейрона
                e = parDesiredOutput[i] - output;
                //ошибка, умноженная на производную функции активации
                errors[i] = e * layer.Neurons[i].Function.Derivative2(output);
                // ссумируем квадраты ошибок
                error += (e * e);
            }

            // ошибка остальных слоев
            for (int j = layersCount - 2; j >= 0; j--)
            {
                layer = network.Layers[j];
                layerNext = network.Layers[j + 1];
                errors = neuronErrors[j];
                errorsNext = neuronErrors[j + 1];

                // для всех нейронов сети
                for (int i = 0; i < layer.Neurons.Length; i++)
                {
                    sum = 0.0;
                    // для всех нейронов следующего уровня
                    for (int k = 0; k < layerNext.Neurons.Length; k++)
                    {
                        sum += errorsNext[k] * layerNext.Neurons[k].Weights[i];
                    }
                    errors[i] = sum * layer.Neurons[i].Function.Derivative2(layer.Neurons[i].Output);
                }
            }

            // СКО / 2
            return error / 2.0;
        }

        /// <summary>
        /// Вычисления изменения весов
        /// </summary>
        /// 
        /// <param name="parInput">Входные вектора</param>
        /// 
        private void CalculateUpdates(double[] parInput)
        {
            // текущий нейрон
            Neuron neuron;
            // текущий и пердыдущий слои
            Layer layer, layerPrev;
            // изменения для весов слоя
            double[][] layerWeightsUpdates;
            // изменения для порогов слоя
            double[] layerThresholdUpdates;
            // ошибки слоя
            double[] errors;
            // изменения весов нейрона
            double[] neuronWeightUpdates;
            // знячения ошибки

            //**************************************
            // Вычисления изменения для первого слоя
            layer = network.Layers[0];
            errors = neuronErrors[0];
            layerWeightsUpdates = weightsUpdates[0];
            layerThresholdUpdates = thresholdsUpdates[0];

            // сохранение часто используемых значений
            double cachedMomentum = learningRate * momentum;
            double cached1mMomentum = learningRate * (1 - momentum);
            double cachedError;

            // Для каждого слоя
            for (int i = 0; i < layer.Neurons.Length; i++)
            {
                neuron = layer.Neurons[i];
                cachedError = errors[i] * cached1mMomentum;
                neuronWeightUpdates = layerWeightsUpdates[i];

                // для каждого веса нейрона
                for (int j = 0; j < neuronWeightUpdates.Length; j++)
                {
                    // вычисление изменения весв
                    neuronWeightUpdates[j] = cachedMomentum * neuronWeightUpdates[j] + cachedError * parInput[j];
                }

                // вычисление изменения порога
                layerThresholdUpdates[i] = cachedMomentum * layerThresholdUpdates[i] + cachedError;
            }

            //*************************
            // Для всех остальных слоев
            for (int k = 1; k < network.Layers.Length; k++)
            {
                layerPrev = network.Layers[k - 1];
                layer = network.Layers[k];
                errors = neuronErrors[k];
                layerWeightsUpdates = weightsUpdates[k];
                layerThresholdUpdates = thresholdsUpdates[k];

                // для каждого нейрона
                for (int i = 0; i < layer.Neurons.Length; i++)
                {
                    neuron = layer.Neurons[i];
                    cachedError = errors[i] * cached1mMomentum;
                    neuronWeightUpdates = layerWeightsUpdates[i];

                    // для каждого синапса нейрона
                    for (int j = 0; j < neuronWeightUpdates.Length; j++)
                    {
                        // вычисление изменения весв
                        neuronWeightUpdates[j] = cachedMomentum * neuronWeightUpdates[j] + cachedError * layerPrev.Neurons[j].Output;
                    }

                    // вычисление изменения порого
                    layerThresholdUpdates[i] = cachedMomentum * layerThresholdUpdates[i] + cachedError;
                }
            }
        }

        /// <summary>
        /// Изменение весов сети
        /// </summary>
        /// 
        private void UpdateNetwork()
        {
            // текущий нейрон
            Neuron neuron;
            // текущий слой
            Layer layer;
            // изменения весов слоя
            double[][] layerWeightsUpdates;
            // изменения порогов слоя
            double[] layerThresholdUpdates;
            // изменения весов нейрона
            double[] neuronWeightUpdates;

            // для каждого слоя сети
            for (int i = 0; i < network.Layers.Length; i++)
            {
                layer = network.Layers[i];
                layerWeightsUpdates = weightsUpdates[i];
                layerThresholdUpdates = thresholdsUpdates[i];

                // для каждого нейрона слоя
                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    neuron = layer.Neurons[j] as Neuron;
                    neuronWeightUpdates = layerWeightsUpdates[j];

                    // для каждого веса нейрона
                    for (int k = 0; k < neuron.Weights.Length; k++)
                    {
                        // изменение весов
                        neuron.Weights[k] += neuronWeightUpdates[k];
                    }
                    // изменение порога
                    neuron.Threshold += layerThresholdUpdates[j];
                }
            }
        }
    }
}
