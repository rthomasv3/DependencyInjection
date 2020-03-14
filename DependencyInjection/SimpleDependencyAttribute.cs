using System;

namespace CodeCompendium.DependencyInjection
{
   /// <summary>
   /// Attribute used to mark a property for dependency injection.
   /// </summary>
   [AttributeUsage(AttributeTargets.Property, Inherited = false)]
   public sealed class SimpleDependencyAttribute : Attribute
   {
   }
}
