using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace Hjerpbakk.FermiContainer {
	public class FermiContainer : IFermiContainer {
		private static readonly Lazy<IFermiContainer> defaultInstance;
		protected readonly Dictionary<Type, Service> Services;

		static FermiContainer() {
			defaultInstance = new Lazy<IFermiContainer>(() => new FermiContainer());
		}

		public FermiContainer() {
			Services = new Dictionary<Type, Service>();
		}

		public static IFermiContainer DefaultInstance { get { return defaultInstance.Value; } }

		public void Register<TInterface, TClass>() where TClass : class, TInterface {
			var type = typeof(TClass);
			var ctor = type.GetConstructors()[0];
			var neededParameters = ctor.GetParameters();
			var n = neededParameters.Length;
			var arguments = new Expression[n];
			for (int i = 0; i < n; i++) {
				var argumentType = neededParameters[i].ParameterType;
				Expression<Func<object>> getService = () => Services[argumentType].Factory();
				arguments[i] = Expression.Convert(getService.Body, argumentType);
			}

			var newExpression = Expression.New(ctor, arguments);
			var newAsLambda = Expression.Lambda<Func<object>>(newExpression);
			Services.Add(typeof(TInterface), new Service(newAsLambda.Compile()));
		}

		public void Register<TClass>() where TClass : class {
			Register<TClass, TClass>();
		}

		public void Register<TInterface, TClass>(Func<object> factory) where TClass : class, TInterface {
			Services.Add(typeof(TInterface), new Service(factory));
		}

		public TInterface Resolve<TInterface>() where TInterface : class {
			return (TInterface)Services[typeof(TInterface)].Factory();
		}

		public TInterface Singleton<TInterface>() where TInterface : class {
			var service = Services[typeof(TInterface)];
			if (Interlocked.Exchange(ref service.SingletonInitialized, 1) == 0) {
				var value = (TInterface)service.Factory();
				service.Factory = () => value;
				return value;
			}

			return (TInterface)service.Factory();
		}

		protected class Service {
			public int SingletonInitialized;
			public Func<object> Factory;

			public Service(Func<object> factory) {
				Factory = factory;
			}
		}
	}

	/// <summary>
	///  Defines a set of methods used to register services into the service container.
	/// </summary>
	public interface IFermiContainer {
		/// <summary>
		/// Registers an interface with a given implementing class in the container.
		/// The first constructor of the implementing class is used, and constructor
		/// arguments are automatically resolved using the container.
		/// </summary>
		/// <typeparam name="TInterface">The interface which the class satisfies.</typeparam>
		/// <typeparam name="TClass">The implementing class of the interface.</typeparam>
		void Register<TInterface, TClass>() where TClass : class, TInterface;

		/// <summary>
		/// Registers a class without an interface in the container.
		/// </summary>
		/// <typeparam name="TClass">The class to be registered.</typeparam>
		void Register<TClass>() where TClass : class;

		/// <summary>
		/// Registers an implementing class in the container using a factory method.
		/// Use this if your implementation has dependencies not present
		/// in the container.
		/// </summary>
		/// <param name="factory">The factory method to use.</param>
		/// <typeparam name="TInterface">The interface which the class satisfies.</typeparam>
		/// <typeparam name="TClass">The implementing class of the interface.</typeparam>
		void Register<TInterface, TClass>(Func<object> factory) where TClass : class, TInterface;

		/// <summary>
		/// Returns the registered implementing class of the given interface.
		/// </summary>
		/// <typeparam name="TInterface">The interface from which to get the implementation.</typeparam>
		TInterface Resolve<TInterface>() where TInterface : class;

		/// <summary>
		/// Returns the registered implementing class of the given interface as a singleton.
		/// This method will always return the same instance for a given interface.
		/// </summary>
		/// <typeparam name="TInterface">The interface from which to get the implementation.</typeparam>
		TInterface Singleton<TInterface>() where TInterface : class;
	}
}

