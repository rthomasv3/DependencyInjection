using System;
using CodeCompendium.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DependencyInjectionUnitTests
{
   [TestClass]
   public class SimpleInjectorTests
   {
      [TestMethod]
      [ExpectedException(typeof(ArgumentNullException))]
      public void Constructor_ArgumentNull_ThrowsArgumentNullException()
      {
         SimpleInjector simpleInjector = new SimpleInjector(null);
      }

      [TestMethod]
      public void Constructor_InjectorWithInstanceMapProvided_InstanceMapCopied()
      {
         TestClass singleton = new TestClass(16);
         SimpleInjector simpleInjector = new SimpleInjector();
         simpleInjector.RegisterSingleInstance(singleton);

         SimpleInjector simpleInjectorCopy = new SimpleInjector(simpleInjector);

         Assert.AreEqual(singleton, simpleInjectorCopy.Resolve<TestClass>());
      }

      [TestMethod]
      public void Constructor_InjectorWithImplementationMapProvided_ImplementationMapCopied()
      {
         SimpleInjector simpleInjector = new SimpleInjector();
         simpleInjector.Register<ITestClass, TestClass>();

         SimpleInjector simpleInjectorCopy = new SimpleInjector(simpleInjector);

         Assert.IsNotNull(simpleInjectorCopy.Resolve<ITestClass>());
      }

      [TestMethod]
      public void Constructor_InjectorWithControlledLifetimeTypesProvided_ControlledLifetimeTypesCopied()
      {
         SimpleInjector simpleInjector = new SimpleInjector();
         simpleInjector.RegisterSingleInstance<TestClass>();

         SimpleInjector simpleInjectorCopy = new SimpleInjector(simpleInjector);

         Assert.AreEqual(simpleInjectorCopy.Resolve<TestClass>(), simpleInjectorCopy.Resolve<TestClass>());
      }

      [TestMethod]
      public void RegisterSingleInstance_InstanceProvided_SingleInstanceRegistered()
      {
         TestClass singleton = new TestClass(16);
         SimpleInjector simpleInjector = new SimpleInjector();
         simpleInjector.RegisterSingleInstance(singleton);

         Assert.AreEqual(singleton, simpleInjector.Resolve<TestClass>());
      }

      [TestMethod]
      public void RegisterSingleInstance_TypeProvided_SingleInstanceRegistered()
      {
         SimpleInjector simpleInjector = new SimpleInjector();
         simpleInjector.RegisterSingleInstance<TestClass>();

         Assert.AreEqual(simpleInjector.Resolve<TestClass>(), simpleInjector.Resolve<TestClass>());
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentException))]
      public void Register_ImplementationTypeNotProvided_ThrowsArgumentException()
      {
         SimpleInjector simpleInjector = new SimpleInjector();

         simpleInjector.Resolve<ITestClass>();
      }

      [TestMethod]
      public void Register_ImplementationTypeProvided_InterfaceResolved()
      {
         SimpleInjector simpleInjector = new SimpleInjector();
         simpleInjector.Register<ITestClass, TestClass>();

         Assert.IsNotNull(simpleInjector.Resolve<ITestClass>());
      }

      [TestMethod]
      public void Resolve_SimpleInjectorTypeProvided_ExistingInstanceReturned()
      {
         SimpleInjector simpleInjector = new SimpleInjector();

         Assert.AreEqual(simpleInjector, simpleInjector.Resolve<SimpleInjector>());
      }

      [TestMethod]
      public void Resolve_PrimitiveTypeProvided_DefaultValueReturned()
      {
         SimpleInjector simpleInjector = new SimpleInjector();

         Assert.AreEqual(default(int), simpleInjector.Resolve<int>());
      }

      [TestMethod]
      public void Resolve_ClassHasRegisteredConstructorDependency_DependencyResolved()
      {
         SimpleInjector simpleInjector = new SimpleInjector();
         simpleInjector.Register<ITestClass, TestClass>();

         TestClassConstructor result = simpleInjector.Resolve<TestClassConstructor>();

         Assert.IsNotNull(result);
      }

      [TestMethod]
      public void Resolve_ClassHasRegisteredSetterDependency_DependencyResolved()
      {
         SimpleInjector simpleInjector = new SimpleInjector();
         simpleInjector.Register<ITestClass, TestClass>();

         TestClassSetter result = simpleInjector.Resolve<TestClassSetter>();

         Assert.IsNotNull(result);
      }

      [TestMethod]
      [ExpectedException(typeof(MissingMethodException))]
      public void Resolve_DependencyPropertyHasNoSetter_ThrowsMissingMethodException()
      {
         SimpleInjector simpleInjector = new SimpleInjector();

         TestClassNoSetter result = simpleInjector.Resolve<TestClassNoSetter>();
      }

      [TestMethod]
      public void Resolve_ConstructorHasValueDependency_DependencyResolved()
      {
         SimpleInjector simpleInjector = new SimpleInjector();

         TestClassValueParameter result = simpleInjector.Resolve<TestClassValueParameter>();

         Assert.IsNotNull(result);
      }

      [TestMethod]
      [ExpectedException(typeof(EntryPointNotFoundException))]
      public void Resolve_NoPublicConstructor_ThrowsEntryPointNotFoundException()
      {
         SimpleInjector simpleInjector = new SimpleInjector();

         TestClassNoPublicConstructor result = simpleInjector.Resolve<TestClassNoPublicConstructor>();
      }

      [TestMethod]
      public void Resolve_ConstructorHasDefaultValueDependency_DependencyResolved()
      {
         SimpleInjector simpleInjector = new SimpleInjector();

         TestClassDefaultValueParameter result = simpleInjector.Resolve<TestClassDefaultValueParameter>();

         Assert.IsNotNull(result);
         Assert.AreEqual(32f, result.Value);
      }
   }

   sealed class TestClass : ITestClass
   {
      private int _intValue = 1;

      public TestClass() { }

      public TestClass(int intValue)
      {
         _intValue = intValue;
      }

      public int GetIntValue()
      {
         return _intValue;
      }
   }

   interface ITestClass
   {
      int GetIntValue();
   }

   sealed class TestClassConstructor
   {
      public TestClassConstructor(ITestClass testClass)
      {
      }
   }

   sealed class TestClassSetter
   {
      [SimpleDependency]
      public TestClass TestClass { get; set; }
   }

   sealed class TestClassNoSetter
   {
      [SimpleDependency]
      public TestClass TestClass { get; }
   }

   sealed class TestClassValueParameter
   {
      public TestClassValueParameter(float value)
      {
      }
   }

   sealed class TestClassDefaultValueParameter
   {
      public TestClassDefaultValueParameter(float value = 32f)
      {
         Value = value;
      }

      public float Value { get; }
   }

   sealed class TestClassNoPublicConstructor
   {
      private TestClassNoPublicConstructor()
      {
      }
   }
}
