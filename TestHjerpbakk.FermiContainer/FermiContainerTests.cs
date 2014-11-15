using System;
using System.Collections.Generic;
using Hjerpbakk.FermiContainer;
using NUnit.Framework;

namespace TestHjerpbakk.FermiContainer {
	[TestFixture]
	public class FermiContainerTests {
		private IFermiContainer m_fermiContainer;

		[SetUp]
		public void Init() {
			m_fermiContainer = new Hjerpbakk.FermiContainer.FermiContainer();
		}

		[Test]
		public void Resolve_ClassRegistered_ReturnsNewObject() {
			m_fermiContainer.Register<ICalculator, Calculator>();

			var calculator = m_fermiContainer.Resolve<ICalculator>();

			Assert.IsInstanceOf<Calculator>(calculator);
		}

		[Test]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void Resolve_ClassNotRegistered_ThrowsException() {
			m_fermiContainer.Resolve<ICalculator>();
		}

		[Test]
		public void ResolveTwoTimes_ClassRegistered_TwoObjectsCreated() {
			m_fermiContainer.Register<ICalculator, Calculator>();
			var calculator = m_fermiContainer.Resolve<ICalculator>();

			var calculator2 = m_fermiContainer.Resolve<ICalculator>();

			Assert.AreNotSame(calculator, calculator2);
		}

		[Test]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void Singleton_ClassNotRegistered_ThrowsException() {
			m_fermiContainer.Singleton<ICalculator>();
		}

		[Test]
		public void SingletonTwoTimes_ClassRegistered_OneObjectCreated() {
			m_fermiContainer.Register<ICalculator, Calculator>();
			var calculator = m_fermiContainer.Singleton<ICalculator>();

			var calculator2 = m_fermiContainer.Singleton<ICalculator>();

			Assert.AreSame(calculator, calculator2);
		}

		[Test]
		public void Resolve_ClassRegisteredWithCustomCtor_ReturnsNewObject() {
			m_fermiContainer.Register<ICalculator, Calculator>(() => new Calculator());

			var calculator = m_fermiContainer.Resolve<ICalculator>();

			Assert.IsInstanceOf<Calculator>(calculator);
		}

		[Test]
		public void Resolve_RegisteredPremadeObject_ReturnsTheObject() {
			var calculator = new Calculator();
			m_fermiContainer.Register<ICalculator, Calculator>(() => calculator);

			var calculator2 = m_fermiContainer.Resolve<ICalculator>();

			Assert.AreSame(calculator, calculator2);
		}

		[Test]
		public void Register_ComplexClassWithFactory_CanBeResolved() {
			m_fermiContainer.Register<ICalculator, Calculator>();
			m_fermiContainer.Register<IComplex, ComplexClass>(() => new ComplexClass(m_fermiContainer.Resolve<ICalculator>()));

			var complexInstance = m_fermiContainer.Resolve<IComplex>();

			Assert.IsInstanceOf<IComplex>(complexInstance);
		}

		[Test]
		public void Register_ComplexClass_CanBeResolved() {
			m_fermiContainer.Register<ICalculator, Calculator>();
			m_fermiContainer.Register<IComplex, ComplexClass>();

			var complexInstance = m_fermiContainer.Resolve<IComplex>();

			Assert.IsInstanceOf<IComplex>(complexInstance);
		}

		[Test]
		public void Register_EvenMoreComplexClass_CanBeResolved() {
			m_fermiContainer.Register<IEvenMoreComplex, EvenMoreComplex>();
			m_fermiContainer.Register<ICalculator, Calculator>();
			m_fermiContainer.Register<IComplex, ComplexClass>();
			m_fermiContainer.Register<ClassWithoutInterface>();

			var complexInstance = m_fermiContainer.Resolve<IEvenMoreComplex>();

			Assert.IsInstanceOf<IEvenMoreComplex>(complexInstance);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Register_InterfaceAlreadyRegistered_ThrowsException() {
			m_fermiContainer.Register<ICalculator, Calculator>();

			m_fermiContainer.Register<ICalculator, Calculator>();
		}

		[Test]
		public void Register_OnlyImplementation_CanBeResolved() {
			m_fermiContainer.Register<ClassWithoutInterface>();

			var classWithoutInterface = m_fermiContainer.Resolve<ClassWithoutInterface>();

			Assert.IsInstanceOf<ClassWithoutInterface>(classWithoutInterface);
		}
	}

	public interface ICalculator {
	}

	public class Calculator : ICalculator {
	}

	public interface IComplex {
	}

	public class ComplexClass : IComplex {
		public ComplexClass(ICalculator calculator) {
		}
	}

	public interface IEvenMoreComplex {
	}

	public class EvenMoreComplex : IEvenMoreComplex {
		public EvenMoreComplex(IComplex complex, ICalculator calculator, ClassWithoutInterface classWithoutInterface) {
		}
	}

	public class ClassWithoutInterface {}
}

