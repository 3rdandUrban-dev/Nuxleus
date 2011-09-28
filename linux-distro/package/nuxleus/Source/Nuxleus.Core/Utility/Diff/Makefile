CSC=gmcs
all: Diff.dll diff.exe diff.tgz Tests.exe xmldiff.exe

SOURCES=Diff.cs Patch.cs Merge.cs UnifiedDiff.cs StructuredDiff.cs TextDiff.cs Assembly.cs

Diff.dll: $(SOURCES)
	$(CSC) -debug -t:library $(SOURCES)
	
diff.exe: DiffApp.cs
	$(CSC) -out:diff.exe DiffApp.cs -r:Diff.dll -r:Mono.GetOptions
	
DiffGtkWidget.dll: DiffWidget.cs Diff.dll
	$(CSC) -debug -t:library -out:DiffGtkWidget.dll DiffWidget.cs -r:Diff.dll -pkg:gtk-sharp-2.0

xmldiff.exe: XmlDiff.cs
	$(CSC) -out:xmldiff.exe XmlDiff.cs -r:Diff.dll -r:Mono.GetOptions
	
csdiff.exe: CSDiff.cs
	$(CSC) -out:csdiff.exe CSDiff.cs -r:Diff.dll -r:Mono.GetOptions -r:antlr.runtime.dll -r:CSharp.dll

docs:
	mono /usr/lib/monodoc/monodocer.exe --assembly Diff.dll --path doc
	mono /usr/lib/monodoc/monodocs2html.exe --source doc --dest html --template docstemplate.xsl

Tests.exe: Tests.cs Diff.dll
	$(CSC) -g Tests.cs -r:Diff.dll
	
diff.tgz:
	tar -zcf diff.tgz -C .. \
	--exclude .svn \
	diff/Makefile \
	diff/Diff.cs diff/Patch.cs diff/Merge.cs diff/DiffApp.cs diff/Assembly.cs diff/StructuredDiff.cs diff/XmlDiff.cs diff/UnifiedDiff.cs diff/TextDiff.cs \
	diff/doc \
	diff/html \
	diff/docstemplate.xsl \
	diff/DiffWidget.cs
	
NDiff.exe: NDiff.cs
	$(CSC) NDiff.cs -r:Diff.dll -pkg:gtk-sharp -r:Mono.GetOptions
	
diff.zip: Diff.dll DiffGtkWidget.dll Makefile ChangeLog README.xhtml
	zip diff.zip $(SOURCES) Makefile README.xhtml ChangeLog \
		Diff.dll DiffGtkWidget.dll \
		doc/*.xml doc/*/*.xml
