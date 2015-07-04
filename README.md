# NullCheckCompetition
Multiple variants of NullCheck for arguments validation (raw check, single lambda, double lambda, anonymous class,etc)


As most of you know, there are at least 4 approaches of testing method's arguments for NULL-value. Usually, if argument is null, an exception is thrown (ArgumentNullException).

Raw check approach
---------
First of all, let's all agree, that there cannot be faster(performance cost) and cheaper(memory cost) method than raw check, but it has it has other cons, like maintainability and readability
   
    public void MethodToValidate(object _arg)
    {
		if (_arg == null)
		{
			throw new ArgumentNullException("_arg");
		}
	}
	
Everything in that approach works fine right until you will rename you argument "_arg" to something else, e.g. "_newArg". By default, Visual Studio WILL NOT rename string literal "_arg" in ArgumentNullException constructor.  Of course, there are tools like ReSharper, that will do that for you, but usually not all developers in team have them installed. So, we need a better method to validate arguments.

I will provide code and test results for 4 methods.

Single-labmda expression approach
---------------------------------
    public void MethodToValidate(object _arg)
    {
		ThrowIfNull(_arg, () => _arg);
	}

This method uses lambda's Member/Unary Expression properties to extract argument name from expression body. 

Double-labmda expression approach
---------------------------------
    public void MethodToValidate(object _arg)
    {
		ThrowIfNull(_arg, () => () => _arg);
	}
There is one bad thing with single-lambda approach - expression will be compilated event if argument is not null => "success" checks will cost almost as high as "fail" checks. Double-lambda method does not have price - internal expression will be compiled only if argument IS null.

Anonymous class approach
------------------------
    public void MethodToValidate(object _arg)
    {
		ThrowIfNull(_arg, new { _arg });
	}

This approach uses anonymous class, that will be created during compile-time by compiler. Compiled class will have only one property and property name will be the same as the arg's name => it can be extracted via Reflection. As you will see from test results, cost of that operation is fairly low

Test results
------------
I got that results on my machine. You can download and build sources to get your results. I used [BenchmarkDotNet](https://github.com/PerfDotNet/BenchmarkDotNet) to avoid writing test code by myself. It's a great lib, you should try it by yourself :)
![enter image description here](http://goo.gl/UdhkBR)
