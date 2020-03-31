# DependencyInjection
An extremely lightweight and simple dependency injection system.

## Getting Started
These instructions will get you a copy of the package installed and provide examples on how to use it.

### Installation
The package can be installed via the nuget package manager in visual studio or nuget cli tool.
```
nuget install CodeCompendium.DependencyInjection
```

### Usage
Using the package is very straightforward. The first step is to setup and register your classes, typically done on application launch.

Creating a new instance:
```
SimpleInjector simpleInjector = new SimpleInjector();
```
You can also pass in an existing instance to create a full copy containing all internal registrations.

Adding a singleton:
```
TestClass singleton = new TestClass();
simpleInjector.RegisterSingleInstance(singleton);
```

Adding a singleton to be created on first request:
```
simpleInjector.RegisterSingleInstance<TestClass>();
```

Mapping an interface to implementation:
```
simpleInjector.Register<ITestClass, TestClass>();
```

Dependencies are typically defined by a class's constructor, but setter injection is also supported with the [SimpleDependency] attribute.
```
class SomClass
{
   [SimpleDependency]
   public TestClass TestClass { get; set; }
}
```

Resolving an instance:
```
ITestClass test = simpleInjector.Resolve<ITestClass>()
```

More detailed usage information can be found by looking through the project's unit tests.

## License
This project is licensed under the MIT License. See LICENSE for details.
