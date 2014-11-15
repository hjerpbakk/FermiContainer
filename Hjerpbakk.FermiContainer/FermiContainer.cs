using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Hjerpbakk.FermiContainer {
	public class FermiContainer : IFermiContainer {
		protected readonly Dictionary<Type, Service> Services;

		public FermiContainer() {
			Services = new Dictionary<Type, Service>();
		}

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
			var value = (TInterface)service.Factory();
			; 
			if (service.IsSingleton) {
				return value;
			}

			service.IsSingleton = true;
			service.Factory = () => value;
			return value;
		}

		protected class Service {
			public bool IsSingleton;
			public Func<object> Factory;

			public Service(Func<object> factory) {
				Factory = factory;
			}
		}
	}
}

