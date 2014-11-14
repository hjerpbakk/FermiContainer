using System;
using NUnit.Framework;
using Hjerpbakk.FermiContainer;
using System.Diagnostics;

namespace TestHjerpbakk.FermiContainer {
	[TestFixture]
	public class PerformanceTests {
		private const int Iterations = 1000000;

		private IFermiContainer m_fermiContainer;

		[SetUp]
		public void Init()
		{
			m_fermiContainer = new Hjerpbakk.FermiContainer.FermiContainer();
		}

		[Test]
		public void NewOperator()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			for (int i = 0; i < Iterations; i++) {
				new Calculator();
			}

			stopwatch.Stop();
			Console.WriteLine("Baseline: " + stopwatch.ElapsedMilliseconds);
		}

		[Test]
		public void NoConstructorArguments()
		{
			m_fermiContainer.Register<ICalculator, Calculator>();

			var stopwatch = new Stopwatch();
			stopwatch.Start();

			for (int i = 0; i < Iterations; i++) {
				m_fermiContainer.Resolve<ICalculator>();
			}

			stopwatch.Stop();
			Console.WriteLine("No constructor args: " + stopwatch.ElapsedMilliseconds);
		}

		[Test]
		public void Singleton()
		{
			m_fermiContainer.Register<ICalculator, Calculator>();

			var stopwatch = new Stopwatch();
			stopwatch.Start();

			for (int i = 0; i < Iterations; i++) {
				m_fermiContainer.Singleton<ICalculator>();
			}

			stopwatch.Stop();
			Console.WriteLine("Singleton: " + stopwatch.ElapsedMilliseconds);
		}

		[Test]
		public void PreConstructed()
		{
			var calculator = new Calculator();
			m_fermiContainer.Register<ICalculator, Calculator>(() => calculator);

			var stopwatch = new Stopwatch();
			stopwatch.Start();

			for (int i = 0; i < Iterations; i++) {
				m_fermiContainer.Singleton<ICalculator>();
			}

			stopwatch.Stop();
			Console.WriteLine("Pre constructed: " + stopwatch.ElapsedMilliseconds);
		}
	}
}

