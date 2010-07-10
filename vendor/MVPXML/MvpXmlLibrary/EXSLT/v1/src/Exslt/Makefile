all:            
	@echo off
        @echo ########### Building Exslt library...        
        csc /out:Mvp.Xml.Exslt.dll /target:library *.cs MultiOutput\*.cs /debug- /optimize+         
        nmake rename-methods
        
rename-methods:
        @echo ########### Disassembing...
        ildasm Mvp.Xml.Exslt.dll  /out=Mvp.Xml.Exslt.il /nobar
        @echo ########### Renaming methods...	
        MethodRenamer.exe Mvp.Xml.Exslt.il Mvp.Xml.Exslt.Fixed.il
        @echo ########### Assembling library back...
        ilasm Mvp.Xml.Exslt.Fixed.il /RESOURCE=Mvp.Xml.Exslt.res  /DLL /OUTPUT=Mvp.Xml.Exslt.dll /KEY=../../../../Global/mvp-xml.snk
        nmake clean

clean:
        @echo ########### Cleaning...
        del Mvp.Xml.Exslt.res
        del Mvp.Xml.Exslt.il
        del Mvp.Xml.Exslt.Fixed.il
