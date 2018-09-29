using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neuron2.Core
{
    public interface IActivationFunction
    {
        /// <summary>
        /// Функция
        /// </summary>
        /// <param name="x">Аргумент</param>
        /// <returns>Значение</returns>
        double Function(double x);

        /// <summary>
        /// Первая производная.
        /// </summary>
        /// <param name="x">Аргумент</param>
        /// <returns>f'(x)</returns>
        double Derivative(double x);

        /// <summary>
        /// Производная по y
        /// </summary>
        /// <param name="y">значение функции</param>
        /// <returns>f'(y)</returns>
        double Derivative2(double y);
    }
}
