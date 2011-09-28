copy /y ..\..\IronPython.dll
copy /y ..\..\IronMath.dll
csc /debug+ /r:IronPython.dll MonthAtAGlance.cs App.cs MonthAtAGlance.Designer.cs
