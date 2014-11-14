using System;

namespace Hjerpbakk.FermiContainer
{
    /// <summary>
    ///  Defines a set of methods used to register services into the service container.
    /// </summary>
    public interface IFermiContainer
    {
        /// <summary>
        /// Registers a class with an default constructor in the container.
        /// </summary>
        /// <typeparam name="TInterface">The interface which the class satisfies.</typeparam>
        /// <typeparam name="TClass">The implementation of the interface.</typeparam>
        void Register<TInterface, TClass>() where TClass : TInterface, new();

		/// <summary>
		/// Registers a class in the container using a factory method.
		/// Useful for classes without a default constructor.
		/// </summary>
		/// <param name="factory">The factory method to use.</param>
		/// <typeparam name="TInterface">The interface which the class satisfies.</typeparam>
		/// <typeparam name="TClass">The implementation of the interface.</typeparam>
		void Register<TInterface, TClass>(Func<object> factory) where TClass : TInterface;

        /// <summary>
        /// Returns an implementation of the given interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface from which to get an implementation.</typeparam>
        TInterface Resolve<TInterface>() where TInterface : class;

        /// <summary>
        /// Returns an implementation of the given interface as a singleton.
        /// This method will always return the same instance for a given interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface from which to get a singleton.</typeparam>
        TInterface Singleton<TInterface>() where TInterface : class;
    }
}

