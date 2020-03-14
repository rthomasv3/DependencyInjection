using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeCompendium.DependencyInjection
{
   /// <summary>
   /// A class used to provide simple dependency injection.
   /// </summary>
   public sealed class SimpleInjector
   {
      #region Fields

      private static readonly string _implementationError = "A concrete implementation for the requested interface was not registered.";
      private static readonly string _constructionError = "Unable to find an appropriate public constructor.";
      private static readonly string _attributeError = "A public setter is required for a property with a dependency attribute.";

      private readonly Dictionary<Type, object> _singleInstanceMap;
      private readonly Dictionary<Type, Type> _implementationMap;
      private readonly HashSet<Type> _controlledLifetimeTypes;

      #endregion

      #region Constructor

      /// <summary>
      /// Creates a new instance of the <see cref="SimpleInjector"/> class.
      /// </summary>
      public SimpleInjector()
      {
         _singleInstanceMap = new Dictionary<Type, object>();
         _implementationMap = new Dictionary<Type, Type>();
         _controlledLifetimeTypes = new HashSet<Type>();
      }

      /// <summary>
      /// Creates a new instance of the <see cref="SimpleInjector"/> class based on the provided instance.
      /// </summary>
      public SimpleInjector(SimpleInjector simpleInjector) : this()
      {
         foreach (KeyValuePair<Type, object> pair in simpleInjector._singleInstanceMap)
         {
            _singleInstanceMap[pair.Key] = pair.Value;
         }

         foreach (KeyValuePair<Type, Type> pair in simpleInjector._implementationMap)
         {
            _implementationMap[pair.Key] = pair.Value;
         }
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Registers an object as a single instance.
      /// Future calls to <see cref="Resolve{T}"/> for its type will return the provided instance.
      /// </summary>
      public void RegisterSingleInstance<T>(T obj)
         where T : class
      {
         _singleInstanceMap[typeof(T)] = obj;
      }

      /// <summary>
      /// Registers a type as a single instance.
      /// The first time <see cref="Resolve{T}"/> is called it will create an instance of this type.
      /// Future calls to <see cref="Resolve{T}"/> for its type will return the previously created instance.
      /// </summary>
      public void RegisterSingleInstance(Type type)
      {
         _controlledLifetimeTypes.Add(type);
      }

      /// <summary>
		/// Registers an object type's implementation.
		/// </summary>
		public void Register<T1, T2>() 
         where T1 : class 
         where T2 : class
      {
         Register(typeof(T1), typeof(T2));
      }

      /// <summary>
		/// Registers an object type's implementation.
		/// </summary>
		public void Register(Type type, Type implementation)
      {
         _implementationMap[type] = implementation;
      }

      /// <summary>
      /// Resolves an instance of an object of the provided type.
      /// </summary>
      public T Resolve<T>()
      {
         return (T)Resolve(typeof(T));
      }

      #endregion

      #region Private Methods

      private object Resolve(Type type)
      {
         if (type == null)
         {
            throw new ArgumentNullException(nameof(type));
         }

         object obj = null;

         if (type == this.GetType())
         {
            obj = this;
         }
         else if (type.IsInterface)
         {
            if (_implementationMap.ContainsKey(type))
            {
               obj = Resolve(_implementationMap[type]);
            }
            else
            {
               throw new ArgumentException(_implementationError);
            }
         }
         else if (_singleInstanceMap.ContainsKey(type))
         {
            obj = _singleInstanceMap[type];
         }
         else if (type.IsPrimitive)
         {
            obj = Activator.CreateInstance(type);
         }
         else
         {
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);

            if (constructor != null)
            {
               obj = Activator.CreateInstance(type);
            }
            else
            {
               constructor = type.GetConstructors().FirstOrDefault(x => x.IsPublic && x.GetParameters().Any());

               if (constructor != null)
               {
                  List<object> parameters = new List<object>();

                  foreach (ParameterInfo parameter in constructor.GetParameters())
                  {
                     parameters.Add(Resolve(parameter.ParameterType));
                  }

                  obj = Activator.CreateInstance(type, parameters.ToArray());
               }
               else
               {
                  throw new EntryPointNotFoundException(_constructionError);
               }
            }
         }

         if (obj != null)
         {
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
               if (Attribute.IsDefined(propertyInfo, typeof(SimpleDependencyAttribute)))
               {
                  if (propertyInfo.SetMethod != null && propertyInfo.SetMethod.IsPublic)
                  {
                     object instance = Resolve(propertyInfo.PropertyType);
                     if (instance != null)
                     {
                        propertyInfo.SetValue(obj, instance);
                     }
                  }
                  else
                  {
                     throw new MissingMethodException(_attributeError);
                  }
               }
            }
         }

         if (_controlledLifetimeTypes.Contains(type) && !_singleInstanceMap.ContainsKey(type))
         {
            _singleInstanceMap[type] = obj;
         }

         return obj;
      }

      #endregion
   }
}
