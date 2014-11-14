using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hjerpbakk.FermiContainer
{
    public class FermiContainer : IFermiContainer
    {
		protected readonly Dictionary<Type, Func<object>> Services;

        public FermiContainer()
        {
            Services = new Dictionary<Type, Func<object>>();
        }

		public void Register<TInterface, TClass>(Func<object> factory) where TClass : TInterface
		{
			Services.Add(typeof(TInterface), factory);
		}

		public void Register<TInterface, TClass>() where TClass : TInterface
		{
			var ctor = typeof(TClass).GetConstructors()[0];
			var neededParameters = ctor.GetParameters();
			var n = neededParameters.Length;

			if (n == 0) {
				var newExp = Expression.New(typeof(TClass));
				var lambda = Expression.Lambda<Func<object>>(newExp);
				Services.Add(typeof(TInterface), lambda.Compile());
				return;
			}

			var parameters = new Func<object>[n];
			for (int i = 0; i < n; i++) {
				var type = neededParameters[i].ParameterType;
				parameters[i] = () => Services[type]();
			}

			Services.Add(typeof(TInterface), () => ctor.Invoke(parameters.Select(p => p()).ToArray()));
        }

        public TInterface Resolve<TInterface>() where TInterface : class
        {
            return (TInterface)Services[typeof(TInterface)]();
        }

        public TInterface Singleton<TInterface>() where TInterface : class
        {
			var value = (TInterface)Services[typeof(TInterface)]();
			Services[typeof(TInterface)] = () => value;
			return value;
        }
    }
}

