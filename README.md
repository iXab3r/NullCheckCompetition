# NullCheckCompetition
Multiple variants of NullCheck for arguments validation (raw check, single lambda, double lambda, anonymous class,etc)


As most of you know, there are at least 4 approaches of testing method's arguments for NULL-value. Usually, if argument is null, an exception is thrown (ArgumentNullException).

Raw check approach
---------
First of all, let's all agree, that there cannot be faster(performance cost) and cheaper(memory cost) method than raw check, but it has it has other cons, like maintainability and readability
```
public void MethodToValidate(object _arg)
{
	if (_arg == null)
	{
		throw new ArgumentNullException("_arg");
	}
}
```
Everything in that approach works fine right until you will rename you argument "_arg" to something else, e.g. "_newArg". By default, Visual Studio WILL NOT rename string literal "_arg" in ArgumentNullException's constructor.  Of course, there are tools like ReSharper, that will do that for you, but usually not all developers in team have them installed. So, we need a better method to validate arguments.

Single-labmda expression approach
---------------------------------
```
public void MethodToValidate(object _arg)
{
	ThrowIfNull(_arg, () => _arg);
}
```
This method uses lambda's Member/Unary Expression properties to extract argument name from expression body. 

Double-labmda expression approach
---------------------------------
```
public void MethodToValidate(object _arg)
{
	ThrowIfNull(_arg, () => () => _arg);
}
```
There is one bad thing with single-lambda approach - expression will be compilated event if argument is not null => "success" checks will cost almost as high as "fail" checks. Double-lambda method does not have price - internal expression will be compiled only if argument IS null.

Anonymous class approach
------------------------
```
public void MethodToValidate(object _arg)
{
	ThrowIfNull(_arg, new { _arg });
}
```
This approach uses anonymous class, that will be created during compile-time by compiler. Compiled class will have only one property and property name will be the same as the arg's name => it can be extracted via Reflection. As you will see from test results, cost of that operation is fairly low

Test results
------------
I got that results on my machine. You can download and build sources to get your results. I used [BenchmarkDotNet](https://github.com/PerfDotNet/BenchmarkDotNet) to avoid writing test code by myself. It's a great lib, you should try it by yourself :)


```
// BenchmarkDotNet=v0.7.6.0
// OS=Microsoft Windows NT 6.2.9200.0
// Processor=Intel(R) Core(TM) i7-3770K CPU @ 3.50GHz, ProcessorCount=8
// CLR=MS.NET 4.0.30319.0, Arch=64-bit  [RyuJIT]
Common:  Type=NullCheckCompetition  Mode=Throughput  Jit=CurrentJit
```

                    Method | Platform | .NET |   AvrTime |     StdDev |         op/s |
-------------------------- |--------- |----- |---------- |----------- |------------- |
        FailAnonymousClass |      X64 |  V40 |  67.87 ns |   0.264 ns |  14734720.29 |
          FailDoubleLambda |      X64 |  V40 | 643.98 ns |   18.85 ns |   1552846.92 |
    FailLazyAnonymousClass |      X64 |  V40 |  69.36 ns |   0.236 ns |  14417370.53 |
              FailRawCheck |      X64 |  V40 |   1.08 ns | 0.00447 ns | 928621853.49 |
          FailSingleLambda |      X64 |  V40 | 643.27 ns |   19.00 ns |   1554564.63 |
     SuccessAnonymousClass |      X64 |  V40 |   6.33 ns |  0.0140 ns | 157929048.78 |
       SuccessDoubleLambda |      X64 |  V40 |   8.48 ns |  0.0352 ns | 117948534.94 |
 SuccessLazyAnonymousClass |      X64 |  V40 |   8.78 ns |  0.0296 ns | 113835487.67 |
           SuccessRawCheck |      X64 |  V40 |   1.08 ns | 0.00681 ns | 926345022.24 |
       SuccessSingleLambda |      X64 |  V40 | 628.28 ns |    3.14 ns |   1591649.56 |
      
                    Method | Platform | .NET |   AvrTime |     StdDev |         op/s |
-------------------------- |--------- |----- |---------- |----------- |------------- |
        FailAnonymousClass |      X64 |  V45 |  67.12 ns |    2.76 ns |  14898130.43 |
          FailDoubleLambda |      X64 |  V45 | 675.43 ns |   52.53 ns |   1480531.17 |
    FailLazyAnonymousClass |      X64 |  V45 |  69.63 ns |    2.50 ns |  14361232.75 |
              FailRawCheck |      X64 |  V45 |   1.06 ns |  0.0240 ns | 940812150.49 |
          FailSingleLambda |      X64 |  V45 | 625.65 ns |   23.85 ns |    1598332.6 |
     SuccessAnonymousClass |      X64 |  V45 |   6.81 ns |   0.187 ns | 146843131.42 |
       SuccessDoubleLambda |      X64 |  V45 |   8.91 ns |   0.158 ns | 112290052.31 |
 SuccessLazyAnonymousClass |      X64 |  V45 |   8.76 ns |   0.443 ns | 114092370.45 |
           SuccessRawCheck |      X64 |  V45 |   1.09 ns |  0.0149 ns | 920809453.85 |
       SuccessSingleLambda |      X64 |  V45 | 631.60 ns |   29.57 ns |   1583458.93 |
       
                    Method | Platform | .NET |   AvrTime |     StdDev |         op/s |
-------------------------- |--------- |----- |---------- |----------- |------------- |
        FailAnonymousClass |      X86 |  V40 |  63.15 ns |    1.69 ns |  15834371.72 |
          FailDoubleLambda |      X86 |  V40 | 629.63 ns |   17.85 ns |   1588245.11 |
    FailLazyAnonymousClass |      X86 |  V40 |  66.76 ns |   0.987 ns |  14979842.49 |
              FailRawCheck |      X86 |  V40 |   1.62 ns |  0.0157 ns | 615978640.25 |
          FailSingleLambda |      X86 |  V40 | 640.02 ns |    9.80 ns |   1562457.01 |
     SuccessAnonymousClass |      X86 |  V40 |   5.16 ns |  0.0729 ns | 193893980.55 |
       SuccessDoubleLambda |      X86 |  V40 |   6.86 ns |  0.0827 ns | 145843539.13 |
 SuccessLazyAnonymousClass |      X86 |  V40 |   7.09 ns |  0.0586 ns | 141005871.21 |
           SuccessRawCheck |      X86 |  V40 |   1.61 ns | 0.00803 ns | 620122392.11 |
       SuccessSingleLambda |      X86 |  V40 | 593.16 ns |   19.43 ns |   1685878.14 |
       
                    Method | Platform | .NET |   AvrTime |     StdDev |         op/s |
-------------------------- |--------- |----- |---------- |----------- |------------- |
        FailAnonymousClass |      X86 |  V45 |  62.33 ns |    4.85 ns |  16044173.38 |
          FailDoubleLambda |      X86 |  V45 | 610.09 ns |   18.33 ns |   1639089.27 |
    FailLazyAnonymousClass |      X86 |  V45 |  65.15 ns |   0.226 ns |  15349795.19 |
              FailRawCheck |      X86 |  V45 |   1.63 ns |  0.0175 ns | 613334880.45 |
          FailSingleLambda |      X86 |  V45 | 605.77 ns |   19.77 ns |   1650782.51 |
     SuccessAnonymousClass |      X86 |  V45 |   5.17 ns |  0.0552 ns | 193296459.61 |
       SuccessDoubleLambda |      X86 |  V45 |   6.93 ns |  0.0698 ns | 144266734.12 |
 SuccessLazyAnonymousClass |      X86 |  V45 |   7.29 ns |  0.0474 ns |  137203230.6 |
           SuccessRawCheck |      X86 |  V45 |   1.63 ns |  0.0223 ns | 612291573.06 |
       SuccessSingleLambda |      X86 |  V45 | 619.56 ns |   24.27 ns |   1614055.14 |
       
Link to screenshot - http://goo.gl/Cty9wm
