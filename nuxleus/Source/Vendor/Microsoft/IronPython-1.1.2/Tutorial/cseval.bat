copy /y ..\*.dll
csc /debug+ /r:IronPython.dll eval.cs
