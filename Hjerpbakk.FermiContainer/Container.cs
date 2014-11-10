namespace Hjerpbakk.FermiContainer
{
    /// <summary>
    /// A static helper class for resolving implementations for a given <see cref="IFermiContainer"/>.
    /// </summary>
    public static class Container
    {
        /// <summary>
        /// The <see cref="IFermiContainter"/> to use.
        /// </summary>
        /// <value>Sets the <see cref="IFermiContainter"/> to use.</value>
        public static IFermiContainer Instance { private get; set; }

        /// <summary>
        /// Returns an implementation of the given interface using the set <see cref="IFermiContainter"/>.
        /// </summary>
        /// <typeparam name="TInterface">The interface from which to get an implementation.</typeparam>
        public static TInterface Resolve<TInterface>() where TInterface : class
        {
            return Instance.Resolve<TInterface>();
        }

        /// <summary>
        /// Returns an implementation of the given interface as a singleton using the set <see cref="IFermiContainter"/>.
        /// This method will always return the same instance for a given interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface from which to get an implementation.</typeparam>
        public static TInterface Singleton<TInterface>() where TInterface : class
        {
            return Instance.Singleton<TInterface>();
        }
    }
}

