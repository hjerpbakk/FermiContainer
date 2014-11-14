using System;
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

        public void Register<TInterface, TClass>() where TClass : TInterface, new()
        {
			Services.Add(typeof(TInterface), () => new TClass ());
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

