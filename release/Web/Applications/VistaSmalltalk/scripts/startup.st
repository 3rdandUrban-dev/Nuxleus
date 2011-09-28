nil subclass: #Object
instanceVariableNames: ''
classVariableNames: ''
!
Object class
!
new
  ^ self basicNew
!
Object
!
asObject
   ^ self
!
asString
   ^ self printString
!
debug
  Debugger open
!
defaultInspector
  ^ DefaultInspector
!
doesNotUnderstand: aMessage
  Transcript showCr: self printString + ' does not understand ' + aMessage printString
!
inspect
  self defaultInspector openOn: self
!
isArray
   ^ false
!
isBehavior
   ^ false
!
isBigInteger
   ^ false
!
isBoolean
   ^ false
!
isButton
   ^ false
!
isDictionary
   ^ false
!
isDouble
   ^ false
!
isFloat
   ^ false
!
isInteger
   ^ false
!
isList
   ^ false
!
isMemberOf: aClass 
   ^ self class == aClass
!
isNil
   ^ false
!
isNumber
   ^ false
!
isString
   ^ false
!
isSystemDictionary
   ^ false
!
notNil
   ^ true
!
printString
  <primitive: 'object tostring'>
!
readSite: aFile
  self readSite: aFile callback: [:response | self onReadResponse: response]
!
readSite: aFile callback: aBlock
  | php |
  php := PhpConnect new.
  php url: 'http://vistascript.net/vistascript/docuwiki/' + aFile.
  php callback: aBlock.
  php serialized: false.
  php queryServer: ''
!
readWiki: aFile
  self readWiki: aFile replace: ''
!
readWiki: aFile callback: aBlock
  self readWiki: aFile replace: '' callback: aBlock
!
readWiki: aFile replace: aString
  self readWiki: aFile replace: aString callback: [:response | self onReadResponse: response]
!
readWiki: aFile replace: aString callback: aBlock
  | php |
  php := PhpConnect new.
  php url: 'http://vistascript.net/vistascript/docuwiki/load.php'.
  php callback: aBlock.
  php serialized: false.
  php queryServer: 'file=' + aFile + '&replace=' + aString
!
sleep: milliseconds
  Processor currentProcess sleep: milliseconds
!
topWindowClass
  Vm virtualDesktop
    ifTrue: [^ DesktopWindow]
    ifFalse: [^ Window]
!
value
  ^ self
!
yield
  Processor currentProcess yield
!
yourself
   ^ self
!
Object subclass: #Behavior
instanceVariableNames: 'instanceVariables methodDict superclass'
classVariableNames: ''
maps: 'vbehavior'
!
Behavior
!
addSubclass: aString
   <primitive: 'vbehavior addsubclass string'>
!
hasSelector: aSymbol
   <primitive: 'vbehavior hasselector vsymbol'>
!
inheritsFrom: aClass 
   | aSuperclass |
   aSuperclass := self superclass.
   [aSuperclass == nil] whileFalse: [
      aSuperclass == aClass ifTrue: [^ true].
      aSuperclass := aSuperclass superclass].
   ^false
!
instanceVariables 
   <primitive: 'vbehavior getinstancevariables'>
!
isBehavior
   ^ true
!
lookupSelector: aSymbol
   <primitive: 'vbehavior lookupselector vsymbol'>
!
method: aString
   <primitive: 'vbehavior lookupselector string'>
!
methodDictionary
   <primitive: 'vbehavior getmethoddictionary'>
!
methodSelectors
   <primitive: 'vbehavior getmethodselectors'>
!
methodSelectorsSorted
   | methods |
   methods := self methodSelectors toArrayList.
   methods sort.
   ^ methods toArray
!
removeMethod: aSymbol
   <primitive: 'vbehavior removemethod vsymbol'>
!
superclass
   <primitive: 'vbehavior getsuperclass'>
!
Behavior subclass: #Class
instanceVariableNames: 'name subclasses'
classVariableNames: ''
maps: 'vclass'
!
Class class
!
allClassesAndMetaClasses
  | classes metaclasses |
  classes := Object withAllSubclasses.
  metaclasses := classes collect:[:cls | cls class].
  classes addList: metaclasses.
  ^ classes
!
template
   ^ ' subclass: #NameOfSubclass
      instanceVariableNames: ''''
      classVariableNames: '''''
!
Class
!
mapType
   <primitive: 'vclass getmaptype'>
!
name
   <primitive: 'vclass name'>
!
new
   <primitive: 'vclass createinstance'>
!
remove
   <primitive: 'vclass remove'>
!
storeString
   <primitive: 'vclass storestring'>
!
subclasses
   <primitive: 'vclass getsubclasses'>
!
withAllSubclasses
  | list |
  list := List new.
  self withAllSubclasses: list.
  list sort.
  ^ list
!
withAllSubclasses: aList
  aList add: self.
  self subclasses do:[:cls | cls withAllSubclasses: aList]
!
Behavior subclass: #MetaClass
instanceVariableNames: 'thisClass'
classVariableNames: ''
maps: 'vmetaclass'
!
MetaClass
!
thisClass
   <primitive: 'vmetaclass getthisclass'>
!
Object subclass: #Boolean
instanceVariableNames: ''
classVariableNames: ''
!
Boolean
!
isBoolean
 ^ true
!
Boolean subclass: #False
instanceVariableNames: ''
classVariableNames: ''
!
False
!
and: aBoolean
  ^ self
!
not
  ^ true
!
or: anObject
  ^ anObject value
!
Boolean subclass: #True
instanceVariableNames: ''
classVariableNames: ''
!
True
!
and: anObject
  ^ anObject value
!
not
  ^ false
!
or: anObject
  ^ self
!
Object subclass: #Collection
instanceVariableNames: ''
classVariableNames: ''
!
Collection
!
do: aBlock
  self reset.
  [self move] whileTrue: [aBlock value: self current]
!
Collection subclass: #Dictionary
instanceVariableNames: ''
classVariableNames: ''
maps: 'vdictionary'
!
Dictionary class
!
new
  <primitive: 'vdictionary .ctor'>
!
Dictionary
!
at: aKey
   <primitive: 'vdictionary get_item object'>
!
at: aKey put: aValue
   <primitive: 'vdictionary set_item object object'>
!
containsKey: aKey
   <primitive: 'vdictionary containskey object'>
!
current
  <primitive: 'vdictionary current'>
!
defaultInspector
  ^ DictionaryInspector
!
isDictionary
  ^ true
!
keys
  | list |
  list := List new.
  self keysDo:[:k | list add: k].
  ^ list
!
keysDo: aBlock
  self do:[:kvp | aBlock value: kvp key]
!
move
  <primitive: 'vdictionary move'>
!
reset
  <primitive: 'vdictionary reset'>
!
size
  <primitive: 'vdictionary count'>
!
values
  | list |
  list := List new.
  self valuesDo:[:v | list add: v].
  ^ list
!
valuesDo: aBlock
  self do:[:kvp | aBlock value: kvp value]
!
Dictionary subclass: #SystemDictionary
instanceVariableNames: ''
classVariableNames: ''
maps: 'vsystemdictionary'
!
SystemDictionary class
!
new
  <primitive: 'vsystemdictionary getinstance'>
!
SystemDictionary
!
at: aSymbol
   <primitive: 'vsystemdictionary get_item object'>
!
at: aSymbol put: aValue
   <primitive: 'vsystemdictionary set_item object voop'>
!
current
  <primitive: 'vsystemdictionary current'>
!
isSystemDictionary
  ^ true
!
move
  <primitive: 'vsystemdictionary move'>
!
reset
  <primitive: 'vsystemdictionary reset'>
!
size
  <primitive: 'vsystemdictionary count'>
!
Collection subclass: #OrderedCollection
instanceVariableNames: ''
classVariableNames: ''
!
OrderedCollection
!
collect: aBlock 
   | newCollection |
   newCollection := List new.
   self do: [:each | newCollection add: (aBlock value: each)].
   ^ newCollection
!
defaultInspector
  ^ ArrayInspector
!
select: aBlock 
   | newCollection |
   newCollection := List new.
   self do: [:each | (aBlock value: each) ifTrue: [newCollection add: each]].
   ^ newCollection
!
OrderedCollection subclass: #Array
instanceVariableNames: ''
classVariableNames: ''
maps: 'varray'
!
Array class
!
new: anInteger
  <primitive: 'varray .ctor int'>
!
with: anObject
   | array |
   array := self new: 1.
   array at: 1 put: anObject.
   ^ array
!
with: anObject1 with: anObject2
   | array |
   array := self new: 2.
   array at: 1 put: anObject1.
   array at: 2 put: anObject2.
   ^ array
!
with: anObject1 with: anObject2 with: anObject3
   | array |
   array := self new: 3.
   array at: 1 put: anObject1.
   array at: 2 put: anObject2.
   array at: 3 put: anObject3.
   ^ array
!
with: anObject1 with: anObject2 with: anObject3 with: anObject4
   | array |
   array := self new: 4.
   array at: 1 put: anObject1.
   array at: 2 put: anObject2.
   array at: 3 put: anObject3.
   array at: 4 put: anObject4.
   ^ array
!
Array
!
asArray
   ^ self
!
asList
   | list |
   list := List new.
   1 to: self size do:[:i | list add: (self at: i)].
   ^ list
!
at: anInteger
   <primitive: 'varray get_item int'>
!
at: anInteger put: anObject
   <primitive: 'varray set_item int object'>
!
current
  <primitive: 'varray get_current'>
!
isArray
   ^ true
!
move
  <primitive: 'varray move'>
!
reset
  <primitive: 'varray reset'>
!
size
   <primitive: 'varray get_length'>
!
OrderedCollection subclass: #List
instanceVariableNames: ''
classVariableNames: ''
maps: 'vlist'
!
List class
!
new
   <primitive: 'vlist .ctor'>
!
List
!
add: anObject
   <primitive: 'vlist add object'>
!
addList: aList
   aList do: [:x | self add: x]
!
asArray
   <primitive: 'vlist toarray'>
!
asList
   ^ self
!
at: anInteger
   <primitive: 'vlist get_item int'>
!
at: anInteger put: anObject
   <primitive: 'vlist set_item int object'>
!
current
  <primitive: 'vlist get_current'>
!
isList
   ^ true
!
move
  <primitive: 'vlist move'>
!
reset
  <primitive: 'vlist reset'>
!
reverse
   <primitive: 'vlist reverse'>
!
size
   <primitive: 'vlist get_count'>
!
sort
   <primitive: 'vlist sort'>
!
Object subclass: #Context
instanceVariableNames: ''
classVariableNames: ''
maps: 'vcontext'
!
Context subclass: #Block
instanceVariableNames: ''
classVariableNames: ''
maps: 'vblock'
!
Block
!
asBlock
   ^ self
!
homeContext
   <primitive: 'vblock homecontext'>
!
isBlock
   ^ true
!
nargs
   <primitive: 'vblock nargs'>
!
printString
   ^ 'a BlockContext'
!
reset
   <primitive: 'vblock  reset'>
!
run
  | process |
  process := Process fromBlock: self.
  Processor scheduleProcess: process
!
value
   <primitive: 'vblock value'>
!
value: arg1
   ^ self valueWithArguments: (Array with: arg1)
!
value: arg1 value: arg2
   ^ self valueWithArguments: (Array with: arg1 with: arg2)
!
value: arg1 value: arg2 value: arg3
   ^ self valueWithArguments: (Array with: arg1 with: arg2 with: arg3)
!
value: arg1 value: arg2 value: arg3 value: arg4
   ^ self valueWithArguments: (Array with: arg1 with: arg2 with: arg3 with: arg4)
!
valueWithArguments: anArray
   <primitive: 'vblock valuewitharguments object[]'>
!
Context subclass: #MethodContext
instanceVariableNames: ''
classVariableNames: ''
maps: 'vmethodcontext'
!
MethodContext class
!
template
   ^'selector
   "comments"

   | locals |
   statements'
!
MethodContext
!
homeContext
   ^ self
!
method
  <primitive: 'vmethodcontext get_method'>
!
receiver
   <primitive: 'vmethodContext receiver'>
!
Object subclass: #InputOutput
instanceVariableNames: ''
classVariableNames: ''
!
InputOutput subclass: #File
instanceVariableNames: ''
classVariableNames: ''
maps: 'file'
!
File class
!
exists: aPath
  <primitive: 'file exists string'>
!
openText: aPath
  <primitive: 'file opentext string'>
!
readAllText: aPath
  <primitive: 'file readalltext string'>
!
writeAllText: aPath from: aString
  <primitive: 'file writealltext string string'>
!
InputOutput subclass: #MemoryStream
instanceVariableNames: ''
classVariableNames: ''
!
MemoryStream class
!
new: aByteArray
  <primitive: 'memorystream .ctor byte[]'>
!
InputOutput subclass: #SmalltalkReader
instanceVariableNames: ''
classVariableNames: ''
maps: 'vsmalltalkreader'
!
SmalltalkReader class
!
addMethod: aBehavior string: aString
  <primitive: 'vsmalltalkreader addmethod vbehavior string'>
!
addSubclass: aBehavior string: aString
  <primitive: 'vsmalltalkreader addsubclass vbehavior string'>
!
InputOutput subclass: #TextReader
instanceVariableNames: ''
classVariableNames: ''
maps: 'textreader'
!
TextReader subclass: #StreamReader
instanceVariableNames: ''
classVariableNames: ''
maps: 'streamreader'
!
TextReader subclass: #StringReader
instanceVariableNames: ''
classVariableNames: ''
maps: 'stringreader'
!
StringReader class
!
onString: aString
  <primitive: 'stringreader .ctor string'>
!
InputOutput subclass: #TextWriter
instanceVariableNames: ''
classVariableNames: ''
maps: 'textwriter'
!
TextWriter subclass: #StreamWriter
instanceVariableNames: ''
classVariableNames: ''
maps: 'streamwriter'
!
InputOutput subclass: #Transcript
instanceVariableNames: ''
classVariableNames: ''
!
Transcript class
!
cr
  <primitive: 'vm println'>
!
show: aString
  <primitive: 'vm print object'>
!
showCr: aString
  <primitive: 'vm println object'>
!
InputOutput subclass: #XamlReader
instanceVariableNames: ''
classVariableNames: ''
maps: 'xamlreader'
!
InputOutput subclass: #Xml
instanceVariableNames: ''
classVariableNames: ''
!
Xml subclass: #XmlDocument
instanceVariableNames: ''
classVariableNames: ''
maps: 'xmldocument'
!
XmlDocument class
!
new
  <primitive: 'xmldocument .ctor'>
!
XmlDocument
!
childNodes
  <primitive: 'xmldocument get_childnodes'>
!
load: anXmlReader
  <primitive: 'xmldocument load xmlreader'>
!
Xml subclass: #XmlNode
instanceVariableNames: ''
classVariableNames: ''
maps: 'xmlnode'
!
XmlNode
!
at: aString
  <primitive: 'xmlnode get_item string'>
!
childNodes
  <primitive: 'xmlnode get_childnodes'>
!
XmlNode subclass: #XmlLinkedNode
instanceVariableNames: ''
classVariableNames: ''
maps: 'xmllinkednode'
!
XmlLinkedNode subclass: #XmlDeclaration
instanceVariableNames: ''
classVariableNames: ''
maps: 'xmldeclaration'
!
XmlDeclaration
!
name
  <primitive: 'xmldeclaration get_name'>
!
XmlLinkedNode subclass: #XmlElement
instanceVariableNames: ''
classVariableNames: ''
maps: 'xmlelement'
!
XmlElement
!
innerText
  <primitive: 'xmlelement get_innertext'>
!
name
  <primitive: 'xmlelement get_name'>
!
Xml subclass: #XmlNodeList
instanceVariableNames: ''
classVariableNames: ''
maps: 'xmlnodelist'
!
XmlNodeList
!
count
  <primitive: 'xmlnodelist get_count'>
!
Xml subclass: #XmlNodeType
instanceVariableNames: ''
classVariableNames: ''
maps: 'xmlnodetype'
!
Xml subclass: #XmlTextReader
instanceVariableNames: ''
classVariableNames: ''
maps: 'xmltextreader'
!
XmlTextReader class
!
fromTextReader: aTextReader
  <primitive: 'xmltextreader .ctor textreader'>
!
fromUrl: aUrl
  <primitive: 'xmltextreader .ctor string'>
!
Xml subclass: #XmlUtility
instanceVariableNames: ''
classVariableNames: ''
maps: 'xmlutility'
!
XmlUtility class
!
childNodes: anObject
  <primitive: 'xmlutility childnodes object'>
!
Object subclass: #Internet
instanceVariableNames: ''
classVariableNames: ''
!
Internet subclass: #Loader
instanceVariableNames: 'reference'
classVariableNames: ''
!
Loader class
!
loadSite: aFile
  self new loadSite: aFile
!
loadWiki: wikiReference
  self new loadWiki: wikiReference
!
Loader
!
loadSite: aFile
  reference := aFile.
  Transcript showCr: 'loading ' + reference + '...'.
  self readSite: reference
!
loadWiki: wikiReference
  reference := 'libraries:smalltalk:loader:' + wikiReference.
  Transcript showCr: 'loading ' + reference + '...'.
  self readWiki: reference
!
onReadResponse: aString
  | success |
  success :=  Vm fileInFromString: aString.
  success ifFalse: [Transcript showCr: 'Failed: ' + reference]
!
Internet subclass: #PhpConnect
instanceVariableNames: ''
classVariableNames: ''
maps: 'phpconnect'
!
PhpConnect class
!
new
  <primitive: 'phpconnect .ctor'>
!
PhpConnect
!
callback
  <primitive: 'phpconnect get_callback'>
!
callback: aBlock
  <primitive: 'phpconnect set_callback object'>
!
queryServer: aQueryString
  <primitive: 'phpconnect queryserverasync string'>
!
serialized
  <primitive: 'phpconnect get_serialized'>
!
serialized: aBlock
  <primitive: 'phpconnect set_serialized bool'>
!
url
  <primitive: 'phpconnect get_url'>
!
url: aString
  <primitive: 'phpconnect set_url string'>
!
Object subclass: #Kernel
instanceVariableNames: ''
classVariableNames: ''
!
Kernel subclass: #CompiledMethod
instanceVariableNames: ''
classVariableNames: ''
maps: 'vcompiledmethod'
!
CompiledMethod
!
text
  <primitive: 'vcompiledmethod text'>
!
Kernel subclass: #Compiler
instanceVariableNames: ''
classVariableNames: ''
maps: 'vcompiler'
!
Compiler class
!
compileMethod: aString
  <primitive: 'vcompiler compilemethod string'>
!
compileSnippet: aString
  <primitive: 'vcompiler compilesnippet string'>
!
Kernel subclass: #Handler
instanceVariableNames: ''
classVariableNames: ''
maps: 'vhandler'
!
Handler class
!
on: anObject event: aString block: aBlock
  <primitive: 'vhandler addhandler object string vblock'>
!
Kernel subclass: #Message
instanceVariableNames: ''
classVariableNames: ''
maps: 'vmessage'
!
Message class
!
receiver: aReceiver selector: aSelector argumemts: anArray
  <primitive: 'vmessage .ctor voop voop voop[]'>
!
Message
!
arguments
  <primitive: 'vmessage get_arguments'>
!
receiver
  <primitive: 'vmessage get_receiver'>
!
selector
  <primitive: 'vmessage get_selector'>
!
Kernel subclass: #Process
instanceVariableNames: ''
classVariableNames: ''
maps: 'vprocess'
!
Process class
!
fromBlock: aBlock
  <primitive: 'vprocess fromblock vblock'>
!
fromMethod: aCompiledMethod
  <primitive: 'vprocess frommethod vcompiledmethod'>
!
Process
!
callback
  <primitive: 'vprocess get_callback'>
!
callback: aBlock
  <primitive: 'vprocess set_callback vblock'>
!
callstack
  <primitive: 'vprocess getcallstack'>
!
errorblock
  <primitive: 'vprocess get_errorblock'>
!
errorblock: aBlock
  <primitive: 'vprocess set_errorblock vblock'>
!
halt
  <primitive: 'vprocess halt'>
!
sleep: milliseconds
  <primitive: 'vprocess sleep int'>
!
yield
  <primitive: 'vprocess yield'>
!
Kernel subclass: #Processor
instanceVariableNames: ''
classVariableNames: ''
maps: 'vprocessor'
!
Processor class
!
activeProcessCount
  <primitive: 'vprocessor get_activeprocesscount'>
!
activeProcesses
  <primitive: 'vprocessor get_activeprocesses'>
!
currentProcess
  <primitive: 'vprocessor get_currentprocess'>
!
scheduleProcess: aProcess
  <primitive: 'vprocessor scheduleprocess vprocess'>
!
Kernel subclass: #Symbol
instanceVariableNames: ''
classVariableNames: ''
maps: 'vsymbol'
!
Symbol class
!
intern: aString
  <primitive: 'vsymbol intern string'>
!
Symbol
!
asSymbol
  ^ self
!
printString
  ^ '#' + super printString
!
Kernel subclass: #Vm
instanceVariableNames: ''
classVariableNames: ''
!
Vm class
!
errorLog
  <primitive: 'vm geterrorlog'>
!
eval: aString
  <primitive: 'vm eval string'>
!
evalInline: aString
  <primitive: 'vm evalinline string'>
!
fileInFromString: aString
  <primitive: 'vm fileinfromstring string'>
!
fileOutAll
  <primitive: 'vm fileoutall'>
!
findEnum: clsname for: valuename
  <primitive: 'vm findenum string string'>
!
gateway
  <primitive: 'vm gateway'>
!
implementorsOf: aSymbol
  <primitive: 'vm implementorsof vsymbol'>
!
lastQuery
  <primitive: 'vm lastquery'>
!
rootClass
  <primitive: 'vm rootclass'>
!
sendersOf: aSymbol
  <primitive: 'vm sendersof vsymbol'>
!
start
  <primitive: 'vm start'>
!
typeList
  <primitive: 'vm get_typelist'>
!
virtualDesktop
  <primitive: 'vm get_virtualdesktop'>
!
Object subclass: #Magnitude
instanceVariableNames: ''
classVariableNames: ''
!
Magnitude subclass: #KeyedVariable
instanceVariableNames: ''
classVariableNames: ''
!
Magnitude subclass: #KeyValuePair
instanceVariableNames: ''
classVariableNames: ''
!
KeyValuePair
!
key
  <primitive: 'vkeyvaluepair get_key'>
!
value
  <primitive: 'vkeyvaluepair get_value'>
!
Magnitude subclass: #Number
instanceVariableNames: ''
classVariableNames: ''
!
Number
!
abs
  self < 0 ifTrue: [^ -1 * self].
  ^ self
!
asFloat
  ^ Convert toSingle: self
!
asInteger
   ^ self // 1
!
even
  ^ self \\ 2 == 0
!
isNumber
   ^ true
!
minus
  ^ 0 - self
!
odd
  ^ self even not
!
Number subclass: #Double
instanceVariableNames: ''
classVariableNames: ''
!
Double class
!
nan
  <primitive: 'vdouble get_nan'>
!
Double
!
isDouble
   ^ true
!
Number subclass: #Float
instanceVariableNames: ''
classVariableNames: ''
!
Float
!
isFloat
   ^ true
!
Number subclass: #Integer
instanceVariableNames: ''
classVariableNames: ''
!
Integer
!
isInteger
   ^ true
!
timesRepeat: aBlock
  1 to: self do:[:i | aBlock value]
!
Number subclass: #Long
instanceVariableNames: ''
classVariableNames: ''
!
Object subclass: #UndefinedObject
instanceVariableNames: ''
classVariableNames: ''
!
UndefinedObject
!
isNil
   ^ true
!
notNil
   ^ false
!
*--------------------------------------------------------
* End of file
*--------------------------------------------------------
