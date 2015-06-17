RosMockLyn [![Build status](https://ci.appveyor.com/api/projects/status/github/AlexEndris/RosMockLyn?branch=master&svg=true)](https://ci.appveyor.com/project/AlexEndris/rosmocklyn?Branch=master) [![NuGet downloads](https://img.shields.io/nuget/dt/RosMockLyn.svg)](https://www.nuget.org/packages/RosMockLyn) [![Version](https://img.shields.io/nuget/v/RosMockLyn.svg)](https://www.nuget.org/packages/RosMockLyn)
========
RosMockLyn is the most user friendly compile-time mocking framework out there. It is - as the name implies - powered by Project Roslyn compilers which means, as long as your project compiles itself, RosMockLyn will create the mocks for you.

##How are the mocks created?
The NuGet package installs a MSBuild target into the test project that automatically scans the project references (not assembly references), collects all interfaces (even internal ones, so no need to put a special 'InternalsVisibleTo' attribute for RosMockLyn) and creates the mocks while making sure that the fully build test-assembly has those mocks as a reference (which is realized with Mono.Cecil). This all means that you don't need to worry about the mocks as all of this is completely automated. Apart from adding RosMockLyn through NuGet, nothing has to be done.

##How do I use the created mocks in my tests?
The mocking interfaces (get a mock, set up return values, assert method calls etc.) are heavily influenced by the big frameworks out there and you might find many familiar things.

```csharp
[TestMethod]
public void SomeTest()
{
	// Creates the mock for the specified interface
	var mock = Mock.For<IMyInterface>();

	// Makes 'SomeMethod' return "I love it" 
	// on any argument
	mock.Setup(x => x.SomeMethod(Arg.Any<int>()))
	    .Returns("I love it!");

	// This is probably in the class you want to test
	var result = mock.SomeMethod(123);

	// Assertion is okay
	Assert.AreEqual("I love it!", result);

	// Assertion is also okay
	mock.Received(x => x.SomeMethod(123)).One();
	
	// This assertion will fail
	mock.Received(x => x.SomeMethod(123)).None();

	// While this one will succeed
	mock.Received(x => x.SomeMethod(23)).None();
}
```
