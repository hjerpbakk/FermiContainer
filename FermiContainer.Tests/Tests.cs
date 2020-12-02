using System;
using System.Collections.Generic;
using FluentAssertions;
using Hjerpbakk.FermiContainer;
using Xunit;

namespace FermiContainer.Tests
{
    public class Tests
    {
        readonly IFermiContainer fermiContainer;

        public Tests()
        {
            fermiContainer = new Hjerpbakk.FermiContainer.FermiContainer();
        }

        [Fact]
        public void Resolve_ClassRegistered_ReturnsNewObject()
        {
            fermiContainer.Register<ICalculator, Calculator>();

            var calculator = fermiContainer.Resolve<ICalculator>();

            calculator.Should().BeAssignableTo<Calculator>();
        }

        [Fact]
        public void Resolve_ClassNotRegistered_ThrowsException()
        {
            Action retriveNonRegisteredInstance = () => fermiContainer.Resolve<ICalculator>();

            retriveNonRegisteredInstance.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void ResolveTwoTimes_ClassRegistered_TwoObjectsCreated()
        {
            fermiContainer.Register<ICalculator, Calculator>();
            var calculator = fermiContainer.Resolve<ICalculator>();

            var calculator2 = fermiContainer.Resolve<ICalculator>();

            calculator.Should().NotBeSameAs(calculator2);
        }

        [Fact]
        public void Singleton_ClassNotRegistered_ThrowsException()
        {
            Action nonRegisteredClassAsSingleton = () => fermiContainer.Singleton<ICalculator>();

            nonRegisteredClassAsSingleton.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void SingletonTwoTimes_ClassRegistered_OneObjectCreated()
        {
            fermiContainer.Register<ICalculator, Calculator>();
            var calculator = fermiContainer.Singleton<ICalculator>();

            var calculator2 = fermiContainer.Singleton<ICalculator>();

            calculator.Should().BeSameAs(calculator2);
        }

        [Fact]
        public void Resolve_ClassRegisteredWithCustomCtor_ReturnsNewObject()
        {
            fermiContainer.Register<ICalculator>(() => new Calculator());

            var calculator = fermiContainer.Resolve<ICalculator>();

            calculator.Should().BeAssignableTo<Calculator>();
        }

        [Fact]
        public void Resolve_RegisteredPremadeObject_ReturnsTheObject()
        {
            var calculator = new Calculator();
            fermiContainer.Register<ICalculator>(() => calculator);

            var calculator2 = fermiContainer.Resolve<ICalculator>();

            calculator.Should().BeSameAs(calculator2);
        }

        [Fact]
        public void Register_ComplexClassWithFactory_CanBeResolved()
        {
            fermiContainer.Register<ICalculator, Calculator>();
            fermiContainer.Register<IComplex>(() => new ComplexClass(fermiContainer.Resolve<ICalculator>()));

            var complexInstance = fermiContainer.Resolve<IComplex>();

            complexInstance.Should().BeAssignableTo<IComplex>();
        }

        [Fact]
        public void Register_ComplexClass_CanBeResolved()
        {
            fermiContainer.Register<ICalculator, Calculator>();
            fermiContainer.Register<IComplex, ComplexClass>();

            var complexInstance = fermiContainer.Resolve<IComplex>();

            complexInstance.Should().BeAssignableTo<IComplex>();
        }

        [Fact]
        public void Register_EvenMoreComplexClass_CanBeResolved()
        {
            fermiContainer.Register<IEvenMoreComplex, EvenMoreComplex>();
            fermiContainer.Register<ICalculator, Calculator>();
            fermiContainer.Register<IComplex, ComplexClass>();
            fermiContainer.Register<ClassWithoutInterface>();

            var complexInstance = fermiContainer.Resolve<IEvenMoreComplex>();

            complexInstance.Should().BeAssignableTo<IEvenMoreComplex>();
        }

        [Fact]
        public void Register_InterfaceAlreadyRegistered_ThrowsException()
        {
            fermiContainer.Register<ICalculator, Calculator>();

            Action secondRegistration = () => fermiContainer.Register<ICalculator, Calculator>();

            secondRegistration.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Register_OnlyImplementation_CanBeResolved()
        {
            fermiContainer.Register<ClassWithoutInterface>();

            var classWithoutInterface = fermiContainer.Resolve<ClassWithoutInterface>();

            classWithoutInterface.Should().BeAssignableTo<ClassWithoutInterface>();
        }

        [Fact]
        public void DefaultInstance_Used_IsOnlyOneInstance()
        {
            var instance = Hjerpbakk.FermiContainer.FermiContainer.DefaultInstance;

            instance.Should().BeAssignableTo<IFermiContainer>();
            instance.Should().BeSameAs(Hjerpbakk.FermiContainer.FermiContainer.DefaultInstance);
        }
    }

    public interface ICalculator { }

    public class Calculator : ICalculator { }

    public interface IComplex { }

    public class ComplexClass : IComplex
    {
        public ComplexClass(ICalculator calculator) { }
    }

    public interface IEvenMoreComplex { }

    public class EvenMoreComplex : IEvenMoreComplex
    {
        public EvenMoreComplex(IComplex complex, ICalculator calculator, ClassWithoutInterface classWithoutInterface) { }
    }

    public class ClassWithoutInterface { }
}
