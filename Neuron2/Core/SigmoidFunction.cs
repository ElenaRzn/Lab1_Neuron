using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neuron2.Core
{
    /// <summary>
    /// Сигмоидальная функция
	///                1
    /// f(x) = ------------------
    ///        1 + exp(-alpha * x)
    ///
    ///           alpha * exp(-alpha * x )
    /// f'(x) = ---------------------------- = alpha * f(x) * (1 - f(x))
    ///           (1 + exp(-alpha * x))^2
    /// </summary>
    [Serializable]
    public class SigmoidFunction : IActivationFunction
    {
        // коэффициент
        private double alpha = 2;

        /// <summary>
        /// коэффициент
		/// чем больше, тем круче функция
        /// </summary>
        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        public SigmoidFunction() { }

        public SigmoidFunction(double alpha)
        {
            this.alpha = alpha;
        }


        /// <summary>
        /// Вычисление функции
        /// </summary>
        /// <param name="x">Аргумент</param>
        /// <returns>f(x)/returns>
        public double Function(double x)
        {
            return (1 / (1 + Math.Exp(-alpha * x)));
        }

        /// <summary>
        /// Производная
        /// </summary>
        /// <param name="x">Аргумент
        /// <returns>f'(x)</returns>
        public double Derivative(double x)
        {
            double y = Function(x);

            return (alpha * y * (1 - y));
        }

        /// <summary>
        /// Производная по `y
        /// </summary>
        /// <param name="y">Значение функции.</param>
        /// <returns>f'(y).</returns>
        public double Derivative2(double y)
        {
            return (alpha * y * (1 - y));
        }
    }
}
