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
*-------------------------------------
* Load all Smalltalk libraries
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Object subclass: #Widgets
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Widgets subclass: #Desktop
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Widgets subclass: #Layouts
instanceVariableNames: 'pane window'
classVariableNames: ''
!
*-------------------------------------
* Class Methods
*-------------------------------------
Layouts class
!
defaultTitle
  ^ self printString
!
new
  | w |
  w := super new init.
  ^ w
!
open
  ^ self new open
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Layouts
!
color: aColor
  window color: aColor
!
init
  window := self topWindowClass new.
  window title: self class defaultTitle
!
open
  window open
!
pane
  ^ pane
!
pane: aPane
  pane := aPane.
  window add: pane
!
title
  ^ window title
!
title: aString
  window title: aString
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Layouts subclass: #FourPane
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
FourPane subclass: #FourPaneTreeTabTreeText
instanceVariableNames: 'nrTextboxRight treeviewLeft treeviewRight tabcontrol textbox'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
FourPaneTreeTabTreeText
!
init
  | bottom bottomRight topLeft topRight |
  super init.
  treeviewLeft := TreeView new.
  treeviewRight := TreeView new.
  tabcontrol := TabControl new.
  textbox := TextEditor new.
  nrTextboxRight := TextBox new.
  topLeft := Grid hsplit: 0.5 child1: treeviewLeft child2: tabcontrol.
  topRight := Grid hsplit: 0.66 child1: topLeft child2: treeviewRight.
  bottom := Grid hsplit: 0.97 child1: textbox child2: nrTextboxRight.
  self pane: (Grid vsplit: 0.335 child1: topRight child2: bottom)
!
nrTextboxRight
  ^ nrTextboxRight
!
tabcontrol
  ^ tabcontrol
!
textbox
  ^ textbox
!
treeviewLeft
  ^ treeviewLeft
!
treeviewRight
  ^ treeviewRight
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Layouts subclass: #OnePane
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
OnePane subclass: #OnePaneDesigner
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
init
  super init.
  self pane: Designer new
!
*-------------------------------------
* Class Declaration
*-------------------------------------
OnePane subclass: #OnePaneText
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
init
  super init.
  self pane: TextBox new
!
text
  ^ pane text
!
text: aString
  pane text: aString
!
*-------------------------------------
* Class Declaration
*-------------------------------------
OnePane subclass: #OnePaneTextEditor
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
init
  super init.
  self pane: TextEditor new
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Layouts subclass: #ThreePane
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
ThreePane subclass: #ThreePaneTreeTabText
instanceVariableNames: 'treeview tabcontrol textbox'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
init
  | top |
  super init.
  treeview := TreeView new.
  tabcontrol := TabControl new.
  textbox := TextBox new.
  top := Grid hsplit: 0.45 child1: treeview child2: tabcontrol.
  self pane: (Grid vsplit: 0.5 child1: top child2: textbox)
!
tabcontrol
  ^ tabcontrol
!
textbox
  ^ textbox
!
treeview
  ^ treeview
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ThreePane subclass: #ThreePaneTreeTabEditor
instanceVariableNames: 'treeview tabcontrol textbox'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
init
  | top |
  super init.
  treeview := TreeView new.
  tabcontrol := TabControl new.
  textbox := TextEditor new.
  top := Grid hsplit: 0.45 child1: treeview child2: tabcontrol.
  self pane: (Grid vsplit: 0.5 child1: top child2: textbox)
!
tabcontrol
  ^ tabcontrol
!
textbox
  ^ textbox
!
treeview
  ^ treeview
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Layouts subclass: #TwoPane
instanceVariableNames: ''
classVariableNames: ''

!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
TwoPane subclass: #TwoPaneListText
instanceVariableNames: 'listbox textbox'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
init
  super init.
  listbox := ListBox new.
  textbox := TextBox new.
  self pane: (Grid hsplit: 0.45 child1: listbox child2: textbox)
!
listbox
  ^ listbox
!
textbox
  ^ textbox
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Object subclass: #WinFx
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
WinFx subclass: #Char
instanceVariableNames: ''
classVariableNames: ''
maps: 'char'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Char class
!
isUpper: aChar
  <primitive: 'char isupper char'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Char
!
isUpper
  ^ self class isUpper: self
!
*-------------------------------------
* Class Declaration
*-------------------------------------
WinFx subclass: #Clipboard
instanceVariableNames: ''
classVariableNames: ''
maps: 'clipboard'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Clipboard class
!
clear
  <primitive: 'clipboard clear'>
!
getText
  <primitive: 'clipboard gettext'>
!
setText: aString
  <primitive: 'clipboard settext string'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
WinFx subclass: #Convert
instanceVariableNames: ''
classVariableNames: ''
maps: 'convert'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Convert class
!
toSingle: anObject
  <primitive: 'convert tosingle object'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
WinFx subclass: #DateTime
instanceVariableNames: ''
classVariableNames: ''
maps: 'datetime'
!
*-------------------------------------
* Class Methods
*-------------------------------------
DateTime class
!
now
  <primitive: 'datetime get_now'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
WinFx subclass: #Encoding
instanceVariableNames: ''
classVariableNames: ''
maps: 'encoding'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Encoding class
!
ascii
  <primitive: 'encoding get_ascii'>
!
getBytes: aString
  <primitive: 'encoding getbytes string'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
WinFx subclass: #Environment
instanceVariableNames: ''
classVariableNames: ''
maps: 'environment'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Environment class
!
newline
  <primitive: 'environment get_newline'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
WinFx subclass: #GC
instanceVariableNames: ''
classVariableNames: ''
maps: 'gc'
!
*-------------------------------------
* Class Methods
*-------------------------------------
GC class
!
getTotalMemory: aBoolean
  <primitive: 'gc gettotalmemory bool'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
WinFx subclass: #Math
instanceVariableNames: ''
classVariableNames: ''
maps: 'math'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Math class
!
cos: aDouble
  <primitive: 'math cos double'>
!
e
  <primitive: 'math e'>
!
pi
  <primitive: 'math pi'>
!
round: aDouble
  <primitive: 'math round double'>
!
sin: aDouble
  <primitive: 'math sin double'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
WinFx subclass: #Random
instanceVariableNames: ''
classVariableNames: ''
maps: 'random'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Random class
!
new
  <primitive: 'random .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Random
!
next
  <primitive: 'random next'>
!
nextMax: anInt
  <primitive: 'random next int'>
!
nextMin: aInt1 max: anInt2
  <primitive: 'random next int int'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
WinFx subclass: #String
instanceVariableNames: ''
classVariableNames: ''
maps: 'string'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
+ aString
   ^ self concatPrim: self with: aString
!
= anObject
   ^ self equals: anObject
!
asString
   ^ self
!
asSymbol
   ^ Symbol intern: self
!
at: anIndex
   <primitive: 'string get_chars int'>
!
clone
   <primitive: 'string clone'>
!
compareTo: aString
   <primitive: 'string compareto string'>
!
concat: aString
   ^ self concatPrim: self with: aString
!
concat: aString1 with: aString2
   ^ self concatPrim: self with: aString1 with: aString2
!
concatPrim: aString1 with: aString2
   <primitive: 'string concat string string'>
!
concatPrim: aString1 with: aString2 with: aString3
   <primitive: 'string concat string string string'>
!
countOf: aString
   | count index start |
   count := 0.
   index := 0.
   start := 0.
   [index >= 0] whileTrue: [
      index := self indexOf: aString start: start.
      index >= 0 ifTrue: [count := count + 1].
      start := index + 1].
   ^ count
!
endsWith: aString
   <primitive: 'string endswith string'>
!
eval
   ^ Vm eval: self
!
evalInline
   ^ Vm evalInline: self
!
getEnumerator
   <primitive: 'string getenumerator'>
!
getHashCode
   <primitive: 'string gethashCode'>
!
getTypeCode
   <primitive: 'string gettypecode'>
!
indexOf: aString
   <primitive: 'string indexof string'>
!
indexOf: aString start: anInteger
   <primitive: 'string indexof string int'>
!
indexOf: aString start: aStart len: aLength
   <primitive: 'string indexof string int int'>
!
indexOfAny: chars start: aStart len: aLength
   <primitive: 'string indexof char[] int int'>
!
insertAt: anIndex string: aString
   <primitive: 'string insert int string'>
!
intern
  ^ Symbol intern: self
!
isString
   ^ true
!
lastIndexOf: aString
   <primitive: 'string lastindexof string'>
!
lastIndexOfAny: chars start: anIndex len: aLength
   <primitive: 'string lastindexofany char[] int int'>
!
length
   <primitive: 'string get_length'>
!
padLeft: aCount withChar: aChar
   <primitive: 'string padleft int char'>
!
padRight: aCount withChar: aChar
   <primitive: 'string padright int char'>
!
printString
   ^ StringUtility asSmalltalkString: self
!
remove: startIndex count: count
   <primitive: 'string remove int int'>
!
replace: aString1 with: aString2
   <primitive: 'string replace string string'>
!
size
   ^ self length
!
split: aCharArray
   <primitive: 'string split char[]'>
!
split: aCharArray start: anIndex
   <primitive: 'string split char[] int'>
!
splitStr: aString
   ^ self split: aString toCharArray
!
startsWith: aString
   <primitive: 'string startswith string'>
!
stringEquals: aString
   <primitive: 'string equals string'>
!
substring: anIndex
   <primitive: 'string substring int'>
!
substring: anIndex len: aLength
   <primitive: 'string substring int int'>
!
toCharArray
   <primitive: 'string tochararray'>
!
toCharArray: anIndex len: aLength
   <primitive: 'string tochararray int int'>
!
toLower
   <primitive: 'string tolower'>
!
toString
   ^ self
!
toUpper
   <primitive: 'string toupper'>
!
trim
   <primitive: 'string trim'>
!
trimEnd: aCharArray
   <primitive: 'string trimend char[]'>
!
trimStart: aCharArray
   <primitive: 'string trimstart char[]'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
WinFx subclass: #WfControl
instanceVariableNames: ''
classVariableNames: ''
maps: 'wfcontrol'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
dock
  <primitive: 'wfcontrol get_dock'>
!
dock: aDockStyle
  <primitive: 'wfcontrol set_dock wfdockstyle'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
WfControl subclass: #WebBrowserBase
instanceVariableNames: ''
classVariableNames: ''
maps: 'webbrowserbase'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
WebBrowserBase subclass: #WebBrowser
instanceVariableNames: ''
classVariableNames: ''
maps: 'webbrowser'
!
*-------------------------------------
* Class Methods
*-------------------------------------
WebBrowser class
!
new
  <primitive: 'webbrowser .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
WebBrowser
!
navigate: aString
  <primitive: 'webbrowser navigate string'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Object subclass: #Wpf
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #Brushes
instanceVariableNames: ''
classVariableNames: ''
maps: 'brushes'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Brushes class
!
aliceBlue
  <primitive: 'brushes get_aliceblue'>
!
black
  <primitive: 'brushes get_black'>
!
blue
  <primitive: 'brushes get_blue'>
!
darkBlue
  <primitive: 'brushes get_darkblue'>
!
darkGreen
  <primitive: 'brushes get_darkgreen'>
!
fireBrick
  <primitive: 'brushes get_firebrick'>
!
gray
  <primitive: 'brushes get_gray'>
!
green
  <primitive: 'brushes get_green'>
!
ivory
  <primitive: 'brushes get_ivory'>
!
lightGray
  <primitive: 'brushes get_lightgray'>
!
navy
  <primitive: 'brushes get_navy'>
!
red
  <primitive: 'brushes get_red'>
!
yellow
  <primitive: 'brushes get_yellow'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #Color
instanceVariableNames: ''
classVariableNames: ''
maps: 'color'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Color class
!
customBlue
  ^self fromSc: 1 r: 0.89 g: 0.89 b: 1
!
customDarkGray
  ^self fromSc: 1 r: 0.06 g: 0.06 b: 0.06
!
customGray
  ^self fromSc: 1 r: 0.8 g: 0.8 b: 0.8
!
customGreen
  ^self fromSc: 1 r: 0.89 g: 1 b: 0.89
!
customOrange
  ^self fromSc: 1 r: 0.76 g: 0.28 b: 0
!
fromSc: sc r: red g: green b: blue
  ^ Color fromScPrim: sc asFloat r: red asFloat g: green asFloat b: blue asFloat
!
fromScPrim: scFloat r: redFloat g: greenFloat b: blueFloat
  <primitive: 'color fromscrgb float float float float'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #Colors
instanceVariableNames: ''
classVariableNames: ''
maps: 'colors'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Colors class
!
aliceBlue
  <primitive: 'colors get_aliceblue'>
!
antiqueWhite
  <primitive: 'colors get_antiquewhite'>
!
black
  <primitive: 'colors get_black'>
!
blue
  <primitive: 'colors get_blue'>
!
bisque
  <primitive: 'colors get_bisque'>
!
blueViolet
  <primitive: 'colors get_blueviolet'>
!
burlyWood
  <primitive: 'colors get_burlywood'>
!
chocolate
  <primitive: 'colors get_chocolate'>
!
darkBlue
  <primitive: 'colors get_darkblue'>
!
darkCyan
  <primitive: 'colors get_darkcyan'>
!
darkGoldenrod
  <primitive: 'colors get_darkgoldenrod'>
!
darkGray
  <primitive: 'colors get_darkgray'>
!
darkGreen
  <primitive: 'colors get_darkgreen'>
!
darkKhaki
  <primitive: 'colors get_darkkhaki'>
!
darkMagenta
  <primitive: 'colors get_darkmagenta'>
!
darkOliveGreen
  <primitive: 'colors get_darkolivegreen'>
!
darkOrange
  <primitive: 'colors get_darkorange'>
!
darkOrchid
  <primitive: 'colors get_darkorchid'>
!
darkRed
  <primitive: 'colors get_darkred'>
!
darkSalmon
  <primitive: 'colors get_darksalmon'>
!
darkSeaGreen
  <primitive: 'colors get_darkseagreen'>
!
darkSlateBlue
  <primitive: 'colors get_darkslateblue'>
!
darkSlateGray
  <primitive: 'colors get_darkslategray'>
!
darkTurquoise
  <primitive: 'colors get_darkturquoise'>
!
darkViolet
  <primitive: 'colors get_darkviolet'>
!
deepPink
  <primitive: 'colors get_deeppink'>
!
deepSkyBlue
  <primitive: 'colors get_deepskyblue'>
!
firebrick
  <primitive: 'colors get_firebrick'>
!
gold
  <primitive: 'colors get_gold'>
!
gray
  <primitive: 'colors get_gray'>
!
green
  <primitive: 'colors get_green'>
!
indigo
  <primitive: 'colors get_indigo'>
!
ivory
  <primitive: 'colors get_ivory'>
!
lime
  <primitive: 'colors get_lime'>
!
maroon
  <primitive: 'colors get_maroon'>
!
navy
  <primitive: 'colors get_navy'>
!
oliveDrab
  <primitive: 'colors get_olivedrab'>
!
orange
  <primitive: 'colors get_orange'>
!
paleGreen
  <primitive: 'colors get_palegreen'>
!
purple
  <primitive: 'colors get_purple'>
!
red
  <primitive: 'colors get_red'>
!
silver
  <primitive: 'colors get_silver'>
!
slateBlue
  <primitive: 'colors get_slateBlue'>
!
slateGray
  <primitive: 'colors get_slategray'>
!
tan
  <primitive: 'colors get_tan'>
!
tomato
  <primitive: 'colors get_tomato'>
!
violet
  <primitive: 'colors get_violet'>
!
white
  <primitive: 'colors get_white'>
!
whiteSmoke
  <primitive: 'colors get_whitesmoke'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #ColumnDefinitionCollection
instanceVariableNames: ''
classVariableNames: ''
maps: 'columndefinitioncollection'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
add: aColumnDefinition
  <primitive: 'columndefinitioncollection add columndefinition'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #DispatcherObject
instanceVariableNames: ''
classVariableNames: ''
maps: 'dispatcherobject'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
DispatcherObject subclass: #Clock
instanceVariableNames: ''
classVariableNames: ''
maps: 'clock'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Clock subclass: #AnimationClock
instanceVariableNames: ''
classVariableNames: ''
maps: 'animationclock'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
DispatcherObject subclass: #DependencyObject
instanceVariableNames: ''
classVariableNames: ''
maps: 'dependencyobject'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
DependencyObject subclass: #ContentElement
instanceVariableNames: ''
classVariableNames: ''
maps: 'contentelement'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
ContentElement subclass: #FrameworkContentElement
instanceVariableNames: ''
classVariableNames: ''
maps: 'frameworkcontentelement'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
FrameworkContentElement subclass: #DefinitionBase
instanceVariableNames: ''
classVariableNames: ''
maps: 'definitionbase'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
DefinitionBase subclass: #ColumnDefinition
instanceVariableNames: ''
classVariableNames: ''
maps: 'columndefinition'
!
*-------------------------------------
* Class Methods
*-------------------------------------
ColumnDefinition class
!
new
  <primitive: 'columndefinition .ctor'>
!
star: aDouble
  | def |
  def := self new.
  def width: (GridLength star: aDouble).
  ^ def
!
*-------------------------------------
* Instance Methods
*-------------------------------------
ColumnDefinition
!
width
  <primitive: 'columndefinition get_width'>
!
width: aGridLength
  <primitive: 'columndefinition set_width gridlength'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
DefinitionBase subclass: #RowDefinition
instanceVariableNames: ''
classVariableNames: ''
maps: 'rowdefinition'
!
*-------------------------------------
* Class Methods
*-------------------------------------
RowDefinition class
!
new
  <primitive: 'rowdefinition .ctor'>
!
star: aDouble
  | def |
  def := self new.
  def height: (GridLength star: aDouble).
  ^ def
!
*-------------------------------------
* Instance Methods
*-------------------------------------
RowDefinition
!
height
  <primitive: 'rowdefinition get_height'>
!
height: aGridLength
  <primitive: 'rowdefinition set_height gridlength'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
DependencyObject subclass: #Freezable
instanceVariableNames: ''
classVariableNames: ''
maps: 'freezable'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Freezable subclass: #Animatible
instanceVariableNames: ''
classVariableNames: ''
maps: 'animatible'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Animatible subclass: #Brush
instanceVariableNames: ''
classVariableNames: ''
maps: 'brush'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Brush subclass: #LinearGradientBrush
instanceVariableNames: ''
classVariableNames: ''
maps: 'lineargradientbrush'
!
*-------------------------------------
* Class Methods
*-------------------------------------
LinearGradientBrush class
!
color1: color1 color2: color2 angle: aDouble
  <primitive: 'lineargradientbrush .ctor color color double'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Brush subclass: #SolidColorBrush
instanceVariableNames: ''
classVariableNames: ''
maps: 'solidcolorbrush'
!
*-------------------------------------
* Class Methods
*-------------------------------------
SolidColorBrush class
!
new: aColor
  <primitive: 'solidcolorbrush .ctor color'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Animatible subclass: #Drawing
instanceVariableNames: ''
classVariableNames: ''
maps: 'drawing'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Drawing subclass: #DrawingGroup
instanceVariableNames: ''
classVariableNames: ''
maps: 'drawinggroup'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Animatible subclass: #GeneralTransform
instanceVariableNames: ''
classVariableNames: ''
maps: 'generaltransform'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
GeneralTransform subclass: #Transform
instanceVariableNames: ''
classVariableNames: ''
maps: 'transform'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Transform class
!
identity
  <primitive: 'transform get_identity'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Transform subclass: #MatrixTransform
instanceVariableNames: ''
classVariableNames: ''
maps: 'matrixtransform'
!
*-------------------------------------
* Class Methods
*-------------------------------------
MatrixTransform class
!
new
  <primitive: 'matrixtransform .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
MatrixTransform
!
matrix
  <primitive: 'matrixtransform get_matrix'>
!
matrix: aMatrix
  <primitive: 'matrixtransform set_matrix matrix'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Transform subclass: #RotateTransform
instanceVariableNames: ''
classVariableNames: ''
maps: 'rotatetransform'
!
*-------------------------------------
* Class Methods
*-------------------------------------
RotateTransform class
!
angleProperty
  <primitive: 'rotatetransform angleproperty'>
!
new
  <primitive: 'rotatetransform .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
RotateTransform
!
angle
  <primitive: 'rotatetransform get_angle'>
!
angle: aDouble
  <primitive: 'rotatetransform set_angle double'>
!
centerX
  <primitive: 'rotatetransform get_centerx'>
!
centerX: aDouble
  <primitive: 'rotatetransform set_centerx double'>
!
centerY
  <primitive: 'rotatetransform get_centery'>
!
centerY: aDouble
  <primitive: 'rotatetransform set_centery double'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Transform subclass: #ScaleTransform
instanceVariableNames: ''
classVariableNames: ''
maps: 'scaletransform'
!
*-------------------------------------
* Class Methods
*-------------------------------------
ScaleTransform class
!
new
  <primitive: 'scaletransform .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
ScaleTransform
!
scaleX
  <primitive: 'scaletransform get_scalex'>
!
scaleX: aDouble
  <primitive: 'scaletransform set_scalex double'>
!
scaleY
  <primitive: 'scaletransform get_scaley'>
!
scaleY: aDouble
  <primitive: 'scaletransform set_scaley double'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Transform subclass: #SkewTransform
instanceVariableNames: ''
classVariableNames: ''
maps: 'skewtransform'
!
*-------------------------------------
* Class Methods
*-------------------------------------
SkewTransform class
!
new
  <primitive: 'skewtransform .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
SkewTransform
!
angleX
  <primitive: 'skewtransform get_anglex'>
!
angleX: aDouble
  <primitive: 'skewtransform set_anglex double'>
!
angleY
  <primitive: 'skewtransform get_angley'>
!
angleY: aDouble
  <primitive: 'skewtransform set_angley double'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Animatible subclass: #Geometry
instanceVariableNames: ''
classVariableNames: ''
maps: 'geometry'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Geometry subclass: #CombinedGeometry
instanceVariableNames: ''
classVariableNames: ''
maps: 'combinedgeometry'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Geometry subclass: #EllipseGeometry
instanceVariableNames: ''
classVariableNames: ''
maps: 'ellipsegeometry'
!
*-------------------------------------
* Class Methods
*-------------------------------------
EllipseGeometry class
!
center: aPoint radius: aDouble
  | g |
  g := self new.
  g radiusX: aDouble; radiusY: aDouble; center: aPoint.
  ^ g
!
new
  <primitive: 'ellipsegeometry .ctor'>
!
x: aDouble1 y: aDouble2
  | g |
  g := self new.
  g radiusX: aDouble1; radiusY: aDouble2.
  ^ g
!
*-------------------------------------
* Instance Methods
*-------------------------------------
EllipseGeometry
!
center
  <primitive: 'ellipsegeometry get_center'>
!
center: aPoint
  <primitive: 'ellipsegeometry set_center point'>
!
radiusX
  <primitive: 'ellipsegeometry get_radiusx'>
!
radiusX: aDouble
  <primitive: 'ellipsegeometry set_radiusx double'>
!
radiusY
  <primitive: 'ellipsegeometry get_radiusy'>
!
radiusY: aDouble
  <primitive: 'ellipsegeometry set_radiusy double'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Geometry subclass: #GeometryGroup
instanceVariableNames: ''
classVariableNames: ''
maps: 'geometrygroup'
!
*-------------------------------------
* Class Methods
*-------------------------------------
GeometryGroup class
!
new
  <primitive: 'geometrygroup .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
GeometryGroup
!
add: aGeometry
  self children add: aGeometry
!
children
  <primitive: 'geometrygroup get_children'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Geometry subclass: #LineGeometry
instanceVariableNames: ''
classVariableNames: ''
maps: 'linegeometry'
!
*-------------------------------------
* Class Methods
*-------------------------------------
LineGeometry class
!
start: aPoint1 end: aPoint2
  <primitive: 'linegeometry .ctor point point'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
LineGeometry
!
endPoint
  <primitive: 'linegeometry get_endpoint'>
!
endPoint: aPoint
  <primitive: 'linegeometry set_endpoint point'>
!
startPoint
  <primitive: 'linegeometry get_startpoint'>
!
startPoint: aPoint
  <primitive: 'linegeometry set_startpoint point'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Geometry subclass: #PathGeometry
instanceVariableNames: ''
classVariableNames: ''
maps: 'pathgeometry'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Geometry subclass: #RectangleGeometry
instanceVariableNames: ''
classVariableNames: ''
maps: 'rectanglegeometry'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Geometry subclass: #StreamGeometry
instanceVariableNames: ''
classVariableNames: ''
maps: 'streamgeometry'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Animatible subclass: #GeometryCollection
instanceVariableNames: ''
classVariableNames: ''
maps: 'geometrycollection'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
GeometryCollection
!
add: aGeometry
  <primitive: 'geometrycollection add geometry'>
!
at: anInteger
  <primitive: 'geometrycollection get_item int'>
!
at: anInteger put: aGeometry
  <primitive: 'geometrycollection set_item int geometry'>
!
clear
  <primitive: 'geometrycollection clear'>
!
count
  <primitive: 'geometrycollection get_count'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Animatible subclass: #Timeline
instanceVariableNames: ''
classVariableNames: ''
maps: 'timeline'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Timeline subclass: #AnimationTimeline
instanceVariableNames: ''
classVariableNames: ''
maps: 'animationtimeline'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
AnimationTimeline subclass: #DoubleAnimationBase
instanceVariableNames: ''
classVariableNames: ''
maps: 'doubleanimationbase'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
DoubleAnimationBase subclass: #DoubleAnimation
instanceVariableNames: ''
classVariableNames: ''
maps: 'doubleanimation'
!
*-------------------------------------
* Class Methods
*-------------------------------------
DoubleAnimation class
!
from: aDouble1 to: aDouble2 duration: aDuration
  <primitive: 'doubleanimation .ctor double double duration'>
!
new
  <primitive: 'doubleanimation .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
DoubleAnimation
!
duration
  <primitive: 'doubleanimation get_duration'>
!
duration: aDuration
  <primitive: 'doubleanimation set_duration duration'>
!
from
  <primitive: 'doubleanimation get_from'>
!
from: aDouble
  <primitive: 'doubleanimation set_from double'>
!
getValue: aDependencyProperty
  <primitive: 'doubleanimation getvalue dependencyproperty'>
!
setValue: aDependencyProperty to: anObject
  <primitive: 'doubleanimation setvalue dependencyproperty object'>
!
to
  <primitive: 'doubleanimation get_to'>
!
to: aDouble
  <primitive: 'doubleanimation set_to double'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
AnimationTimeline subclass: #PointAnimationBase
instanceVariableNames: ''
classVariableNames: ''
maps: 'pointanimationbase'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
PointAnimationBase subclass: #PointAnimation
instanceVariableNames: ''
classVariableNames: ''
maps: 'pointanimation'
!
*-------------------------------------
* Class Methods
*-------------------------------------
PointAnimation class
!
from: aPoint1 to: aPoint2 duration: aDuration
  <primitive: 'pointanimation .ctor point point duration'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Timeline subclass: #TimelineGroup
instanceVariableNames: ''
classVariableNames: ''
maps: 'timelinegroup'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
TimelineGroup subclass: #ParallelTimeline
instanceVariableNames: ''
classVariableNames: ''
maps: 'paralleltimeline'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
ParallelTimeline subclass: #StoryBoard
instanceVariableNames: ''
classVariableNames: ''
maps: 'storyboard'
!
*-------------------------------------
* Class Methods
*-------------------------------------
StoryBoard class
!
getTargetName: aDependencyObject 
  <primitive: 'storyboard gettargetname dependencyobject'>
!
getTargetProperty: aDependencyObject 
  <primitive: 'storyboard gettargetproperty dependencyobject'>
!
new
  <primitive: 'storyboard .ctor'>
!
setTargetName: aDependencyObject to: aString
  <primitive: 'storyboard settargetname dependencyobject string'>
!
setTargetProperty: aDependencyObject to: aPropertyPath
  <primitive: 'storyboard settargetproperty dependencyobject propertypath'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
StoryBoard
!
begin: aFrameworkElement
  <primitive: 'storyboard begin frameworkelement'>
!
children
  <primitive: 'storyboard get_children'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Animatible subclass: #TimelineCollection
instanceVariableNames: ''
classVariableNames: ''
maps: 'timelinecollection'
!
*-------------------------------------
* Class Methods
*-------------------------------------
TimelineCollection class
!
new
  <primitive: 'timelinecollection .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
TimelineCollection
!
add: aTimeLine
  <primitive: 'timelinecollection add timeline'>
!
clear
  <primitive: 'timelinecollection clear'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
DependencyObject subclass: #TriggerBase
instanceVariableNames: ''
classVariableNames: ''
maps: 'triggerbase'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
TriggerBase subclass: #EventTrigger
instanceVariableNames: ''
classVariableNames: ''
maps: 'eventtrigger'
!
*-------------------------------------
* Class Methods
*-------------------------------------
EventTrigger class
!
new
  <primitive: 'eventtrigger .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
EventTrigger
!
actions
  <primitive: 'eventtrigger get_actions'>
!
routedEvent
  <primitive: 'eventtrigger get_routedevent'>
!
routedEvent: aRoutedEvent
  <primitive: 'eventtrigger set_routedevent routedevent'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
DependencyObject subclass: #Visual
instanceVariableNames: ''
classVariableNames: ''
maps: 'visual'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Visual subclass: #UIElement
instanceVariableNames: ''
classVariableNames: ''
maps: 'uielement'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
mouseEnterEvent
  <primitive: 'uielement mouseenterevent'>
!
renderTransform
  <primitive: 'uielement get_rendertransform'>
!
renderTransform: aTransform
  <primitive: 'uielement set_rendertransform transform'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
UIElement subclass: #FrameworkElement
instanceVariableNames: ''
classVariableNames: ''
maps: 'frameworkelement'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
FrameworkElement
!
actualHeight
  <primitive: 'frameworkelement get_actualheight'>
!
actualWidth
  <primitive: 'frameworkelement get_actualwidth'>
!
contextMenu
  <primitive: 'frameworkelement get_contextmenu'>
!
contextMenu: aMenu
  <primitive: 'frameworkelement set_contextmenu contextmenu'>
!
expandToFill
  self horizontalAlignment: 'Stretch'.
  self verticalAlignment: 'Stretch'.
  self width: Double nan; height: Double nan.
  self margin: (Thickness new: 0)
!
findName: aString
  <primitive: 'frameworkelement findname string'>
!
height
  <primitive: 'frameworkelement get_height'>
!
height: aDouble
  <primitive: 'frameworkelement set_height double'>
!
horizontalAlignment
  <primitive: 'frameworkelement get_horizontalalignment'>
!
horizontalAlignment: anAlignment
  <primitive: 'frameworkelement set_horizontalalignment horizontalalignment'>
!
isEnabled
  <primitive: 'frameworkelement get_isenabled'>
!
isEnabled: aBoolean
  <primitive: 'frameworkelement set_isenabled bool'>
!
location: x y: y
  self margin: (Thickness newLeft: x top: y right: 0 bottom: 0)
!
margin
  <primitive: 'frameworkelement get_margin'>
!
margin: aThickness
  <primitive: 'frameworkelement set_margin thickness'>
!
margins: aDouble
  self margin: (Thickness new: aDouble)
!
name
  <primitive: 'frameworkelement get_name'>
!
name: aString
  <primitive: 'frameworkelement set_name string'>
!
parent
  <primitive: 'frameworkelement get_parent'>
!
registerName: aString for: anObject
  <primitive: 'frameworkelement registername string object'>
!
tag
  <primitive: 'frameworkelement get_tag'>
!
tag: anObject
  <primitive: 'frameworkelement set_tag object'>
!
tofront
  self parent children tofront: self
!
triggers
  <primitive: 'frameworkelement get_triggers'>
!
verticalAlignment
  <primitive: 'frameworkelement get_verticalalignment'>
!
verticalAlignment: anAlignment
  <primitive: 'frameworkelement set_verticalalignment verticalalignment'>
!
width
  <primitive: 'frameworkelement get_width'>
!
width: aDouble
  <primitive: 'frameworkelement set_width double'>
!
widthProperty
  <primitive: 'frameworkelement widthproperty'>
!
widthProperty: aDependencyObject
  <primitive: 'frameworkelement widthproperty dependencyobject'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
FrameworkElement subclass: #ContentPresenter
instanceVariableNames: ''
classVariableNames: ''
maps: 'contentpresenter'
!
*-------------------------------------
* Class Methods
*-------------------------------------
ContentPresenter class
!
new
  <primitive: 'contentpresenter .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
ContentPresenter
!
content
  <primitive: 'contentpresenter get_content'>
!
content: anObject
  <primitive: 'contentpresenter set_content object'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
FrameworkElement subclass: #Control
instanceVariableNames: ''
classVariableNames: ''
maps: 'control'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
background
  <primitive: 'control get_background'>
!
background: aBrush
  <primitive: 'control set_background brush'>
!
borderbrush
  <primitive: 'control get_borderbrush'>
!
borderbrush: aBrush
  <primitive: 'control set_borderbrush brush'>
!
borderthickness
  <primitive: 'control get_borderthickness'>
!
borderthickness: aThickness
  <primitive: 'control set_borderthickness thickness'>
!
fontsize
  <primitive: 'control get_fontsize'>
!
fontsize: aDouble
  <primitive: 'control set_fontsize double'>
!
fontweight
  <primitive: 'control get_fontweight'>
!
fontweight: aFontWeight
  <primitive: 'control set_fontweight fontweight'>
!
foreground
  <primitive: 'control get_foreground'>
!
foreground: aBrush
  <primitive: 'control set_foreground brush'>
!
horizontalContentAlignment
  <primitive: 'control get_horizontalcontentalignment'>
!
horizontalContentAlignment: aHorizontalAlignment
  <primitive: 'control set_horizontalcontentalignment horizontalalignment'>
!
isenabled
  <primitive: 'control get_isenabled'>
!
isenabled: aBoolean
  <primitive: 'control set_isenabled bool'>
!
padding
  <primitive: 'control get_padding'>
!
padding: aThickness
  <primitive: 'control set_padding thickness'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Control subclass: #ContentControl
instanceVariableNames: ''
classVariableNames: ''
maps: 'contentcontrol'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
content
  <primitive: 'contentcontrol get_content'>
!
content: anObject
  <primitive: 'contentcontrol set_content object'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
ContentControl subclass: #ButtonBase
instanceVariableNames: ''
classVariableNames: ''
maps: 'buttonbase'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
ButtonBase subclass: #Button
instanceVariableNames: ''
classVariableNames: ''
maps: 'button'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Button class
!
content: aContent callback: aBlock
  | btn |
  btn := self new.
  btn content: aContent.
  Handler on: btn event: 'Click' block: aBlock.
  ^ btn
!
new
  <primitive: 'button .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Button
!
isButton
  ^ true
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ButtonBase subclass: #ToggleButton
instanceVariableNames: ''
classVariableNames: ''
maps: 'togglebutton'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
ToggleButton subclass: #RadioButton
instanceVariableNames: ''
classVariableNames: ''
maps: 'radiobutton'
!
*-------------------------------------
* Class Methods
*-------------------------------------
RadioButton class
!
new
  <primitive: 'radiobutton .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
RadioButton
!
ischecked
  <primitive: 'radiobutton get_ischecked'>
!
ischecked: aBoolean
  <primitive: 'radiobutton set_ischecked bool'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ContentControl subclass: #HeaderedContentControl
instanceVariableNames: ''
classVariableNames: ''
maps: 'headeredcontentcontrol'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
header
  <primitive: 'headeredcontentcontrol get_header'>
!
header: anObject
  <primitive: 'headeredcontentcontrol set_header object'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
HeaderedContentControl subclass: #TabItem
instanceVariableNames: ''
classVariableNames: ''
maps: 'tabitem'
!
*-------------------------------------
* Class Methods
*-------------------------------------
TabItem class
!
new
  <primitive: 'tabitem .ctor'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ContentControl subclass: #Label
instanceVariableNames: ''
classVariableNames: ''
maps: 'label'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Label class
!
new
  <primitive: 'label .ctor'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ContentControl subclass: #ListBoxItem
instanceVariableNames: ''
classVariableNames: ''
maps: 'listboxitem'
!
*-------------------------------------
* Class Methods
*-------------------------------------
ListBoxItem class
!
new
  <primitive: 'listboxitem .ctor'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ContentControl subclass: #UserControl
instanceVariableNames: ''
classVariableNames: ''
maps: 'usercontrol'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
UserControl subclass: #Designer
instanceVariableNames: ''
classVariableNames: ''
maps: 'designer'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Designer class
!
new
  <primitive: 'designer .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Designer
!
view
  <primitive: 'designer get_view'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
UserControl subclass: #TextEditor
instanceVariableNames: ''
classVariableNames: ''
maps: 'texteditor'
!
*-------------------------------------
* Class Methods
*-------------------------------------
TextEditor class
!
new
  <primitive: 'texteditor .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
TextEditor
!
appendText: aString
  <primitive: 'texteditor appendtext string'>
!
background
  <primitive: 'texteditor editorbackground'>
!
background: aBrush
  <primitive: 'texteditor editorbackground brush'>
!
clearBreakpoints
  <primitive: 'texteditor clearbreakpoints'>
!
contextMenu
  <primitive: 'texteditor editorcontextmenu'>
!
contextMenu: aMenu
  <primitive: 'texteditor editorcontextmenu contextmenu'>
!
foreground
  <primitive: 'texteditor editorforeground'>
!
foreground: aBrush
  <primitive: 'texteditor editorforeground brush'>
!
insertAtEndOfSelection: aString
  <primitive: 'texteditor insertatendofselection string'>
!
println: aString
  self appendText: aString + Environment newline
!
selectedText
  <primitive: 'texteditor get_selectedtext'>
!
selectedText: aString
  <primitive: 'texteditor set_selectedtext string'>
!
selectionLength
  <primitive: 'texteditor get_selectionlength'>
!
selectionLength: anInteger
  <primitive: 'texteditor set_selectionlength string'>
!
selectionStart
  <primitive: 'texteditor get_selectionstart'>
!
selectionStart: anInteger
  <primitive: 'texteditor set_selectionstart string'>
!
setMethod: aMethod
  <primitive: 'texteditor setmethod ibreakpoints'>
!
showBreakpoints
  <primitive: 'texteditor get_showbreakpoints'>
!
showBreakpoints: aBoolean
  <primitive: 'texteditor set_showbreakpoints bool'>
!
showLines
  <primitive: 'texteditor get_showlines'>
!
showLines: aBoolean
  <primitive: 'texteditor set_showlines bool'>
!
text
  <primitive: 'texteditor get_text'>
!
text: aString
  <primitive: 'texteditor set_text string'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ContentControl subclass: #Window
instanceVariableNames: ''
classVariableNames: ''
maps: 'window'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Window class
!
new
  | w |
  w := self newPrim.
  w width: 300; height: 250.
  ^ w
!
newPrim
  <primitive: 'window .ctor'>
!
open
  | w |
  w := self new.
  w open.
  ^ w
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Window
!
add: anElement
  anElement width: Double nan; height: Double nan.
  self content: anElement
!
clear
  self add: Canvas new
!
color: aColor
  ^ nil
!
content
  <primitive: 'window get_content'>
!
content: anObject
  <primitive: 'window set_content object'>
!
height
  <primitive: 'window get_height'>
!
height: aDouble
  <primitive: 'window set_height double'>
!
left
  <primitive: 'window get_left'>
!
left: aDouble
  <primitive: 'window set_left double'>
!
open
  self show
!
show
  <primitive: 'window show'>
!
showDialog
  <primitive: 'window showdialog'>
!
title
  <primitive: 'window get_title'>
!
title: aString
  <primitive: 'window set_title string'>
!
top
  <primitive: 'window get_top'>
!
top: aDouble
  <primitive: 'window set_top double'>
!
width
  <primitive: 'window get_width'>
!
width: aDouble
  <primitive: 'window set_width double'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Control subclass: #ItemsControl
instanceVariableNames: ''
classVariableNames: ''
maps: 'itemscontrol'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
add: anObject
  self items add: anObject
!
clear
  self items clear
!
items
  <primitive: 'itemscontrol get_items'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
ItemsControl subclass: #HeaderedItemsControl
instanceVariableNames: ''
classVariableNames: ''
maps: 'headereditemscontrol'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
header
  <primitive: 'headereditemscontrol get_header'>
!
header: anObject
  <primitive: 'headereditemscontrol set_header object'>
!
items
  <primitive: 'headereditemscontrol get_items'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
HeaderedItemsControl subclass: #MenuItem
instanceVariableNames: ''
classVariableNames: ''
maps: 'menuitem'
!
*-------------------------------------
* Class Methods
*-------------------------------------
MenuItem class
!
name: aName action: aBlock
  | mi |
  mi := self new.
  mi header: aName.
  mi addEvent: 'Click' action: aBlock.
  ^ mi
!
new
  <primitive: 'menuitem .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
MenuItem
!
name: aName action: aBlock
  | mi |
  mi := self new.
  mi header: aName.
  mi addEvent: 'Click' action: aBlock.
  ^ mi
!
*-------------------------------------
* Class Declaration
*-------------------------------------
HeaderedItemsControl subclass: #TreeViewItem
instanceVariableNames: ''
classVariableNames: ''
maps: 'treeviewitem'
!
*-------------------------------------
* Class Methods
*-------------------------------------
TreeViewItem class
!
new
  <primitive: 'treeviewitem .ctor'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ItemsControl subclass: #MenuBase
instanceVariableNames: ''
classVariableNames: ''
maps: 'menubase'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
add: anItem
  self items add: anItem
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
MenuBase subclass: #ContextMenu
instanceVariableNames: ''
classVariableNames: ''
maps: 'contextmenu'
!
*-------------------------------------
* Class Methods
*-------------------------------------
ContextMenu class
!
new
  <primitive: 'contextmenu .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
ContextMenu
!
items
  <primitive: 'contextmenu get_items'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
MenuBase subclass: #Menu
instanceVariableNames: ''
classVariableNames: ''
maps: 'menu'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Menu class
!
new
  <primitive: 'menu .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Menu
!
items
  <primitive: 'menu get_items'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ItemsControl subclass: #Selector
instanceVariableNames: ''
classVariableNames: ''
maps: 'selector'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
selectedIndex
  <primitive: 'selector get_selectedindex'>
!
selectedIndex: anIndex
  <primitive: 'selector set_selectedindex int'>
!
selectedItem
  <primitive: 'selector get_selecteditem'>
!
selectedItem: anObject
  <primitive: 'selector set_selecteditem object'>
!
selectedValue
  <primitive: 'selector get_selectedvalue'>
!
selectedValue: aValue
  <primitive: 'selector set_selectedvalue object'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Selector subclass: #ComboBox
instanceVariableNames: ''
classVariableNames: ''
maps: 'combobox'
!
*-------------------------------------
* Class Methods
*-------------------------------------
ComboBox class
!
new
  <primitive: 'combobox .ctor'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Selector subclass: #ListBox
instanceVariableNames: ''
classVariableNames: ''
maps: 'listbox'
!
*-------------------------------------
* Class Methods
*-------------------------------------
ListBox class
!
new
  <primitive: 'listbox .ctor'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Selector subclass: #TabControl
instanceVariableNames: ''
classVariableNames: ''
maps: 'tabcontrol'
!
*-------------------------------------
* Class Methods
*-------------------------------------
TabControl class
!
new
  <primitive: 'tabcontrol .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
TabControl
!
selectedIndex
  <primitive: 'tabcontrol get_selectedindex'>
!
tabStripPlacement
  <primitive: 'tabcontrol get_tabstripplacement'>
!
tabStripPlacement: aDock
  <primitive: 'tabcontrol set_tabstripplacement dock'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ItemsControl subclass: #TreeView
instanceVariableNames: ''
classVariableNames: ''
maps: 'treeview'
!
*-------------------------------------
* Class Methods
*-------------------------------------
TreeView class
!
new
  <primitive: 'treeview .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
TreeView
!
selectedItem
  <primitive: 'treeview get_selecteditem'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Control subclass: #RangeBase
instanceVariableNames: ''
classVariableNames: ''
maps: 'rangebase'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
maximum
  <primitive: 'rangebase get_maximum'>
!
maximum: aDouble
  <primitive: 'rangebase set_maximum double'>
!
minimum
  <primitive: 'rangebase get_minimum'>
!
minimum: aDouble
  <primitive: 'rangebase set_minimum double'>
!
value
  <primitive: 'rangebase get_value'>
!
value: aDouble
  <primitive: 'rangebase set_value double'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
RangeBase subclass: #Slider
instanceVariableNames: ''
classVariableNames: ''
maps: 'slider'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Slider class
!
new
  <primitive: 'slider .ctor'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Control subclass: #Separator 
instanceVariableNames: ''
classVariableNames: ''
maps: 'separator'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Separator class
!
new
  <primitive: 'separator .ctor'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Control subclass: #TextBoxBase
instanceVariableNames: ''
classVariableNames: ''
maps: 'textboxbase'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
acceptsReturn
  <primitive: 'textboxbase get_acceptsreturn'>
!
acceptsReturn: aBoolean
  <primitive: 'textboxbase set_acceptsreturn bool'>
!
appendText: aString
  <primitive: 'textboxbase appendtext string'>
!
hideSelection
  <primitive: 'textboxbase get_hideselection'>
!
hideSelection: aBoolean
  <primitive: 'textboxbase set_hideselection bool'>
!
horizontalScrollbarVisibility
  <primitive: 'textboxbase get_horizontalscrollbarvisibility'>
!
horizontalScrollbarVisibility: aVisibility
  <primitive: 'textboxbase set_horizontalscrollbarvisibility scrollbarvisibility'>
!
verticalScrollbarVisibility
  <primitive: 'textboxbase get_verticalscrollbarvisibility'>
!
verticalScrollbarVisibility: aVisibility
  <primitive: 'textboxbase set_verticalscrollbarvisibility scrollbarvisibility'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
TextBoxBase subclass: #TextBox
instanceVariableNames: ''
classVariableNames: ''
maps: 'textbox'
!
*-------------------------------------
* Class Methods
*-------------------------------------
TextBox class
!
new
  ^ self newprim init
!
newprim
  <primitive: 'textbox .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
TextBox
!
clear
  self text: ''
!
init
  self acceptsReturn: true;
       width: Double nan;
       height: Double nan;
       background: Brushes ivory;
       foreground: Brushes navy;
       horizontalScrollbarVisibility: 'Auto';
       verticalScrollbarVisibility: 'Auto'
!
print: aString
  self appendText: aString
!
println: aString
  self appendText: aString + Environment newline
!
selectedText
  <primitive: 'textbox get_selectedtext'>
!
selectedText: aString
  <primitive: 'textbox set_selectedtext string'>
!
selectionLength
  <primitive: 'textbox get_selectionlength'>
!
selectionLength: anInt
  <primitive: 'textbox set_selectionlength int'>
!
selectionStart
  <primitive: 'textbox get_selectionStart'>
!
selectionStart: anInt
  <primitive: 'textbox set_selectionstart int'>
!
text
  <primitive: 'textbox get_text'>
!
text: aString
  <primitive: 'textbox set_text string'>
!
textalignment
  <primitive: 'textbox get_textalignment'>
!
textalignment: aTextAlignment
  <primitive: 'textbox set_textalignment textalignment'>
!
textwrapping
  <primitive: 'textbox get_textwrapping'>
!
textwrapping: aWrapping
  <primitive: 'textbox set_textwrapping textwrapping'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Control subclass: #Thumb
instanceVariableNames: ''
classVariableNames: ''
maps: 'thumb'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Thumb subclass: #GridSplitter
instanceVariableNames: ''
classVariableNames: ''
maps: 'gridsplitter'
!
*-------------------------------------
* Class Methods
*-------------------------------------
GridSplitter class
!
new
  <primitive: 'gridsplitter .ctor'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
FrameworkElement subclass: #Decorator
instanceVariableNames: ''
classVariableNames: ''
maps: 'decorator'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
child
  <primitive: 'decorator get_child'>
!
child: aUiElement
  <primitive: 'decorator set_child uielement'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Decorator subclass: #Border
instanceVariableNames: ''
classVariableNames: ''
maps: 'border'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Border class
!
new
  <primitive: 'border .ctor'>
!
*-------------------------------------
* Class Methods
*-------------------------------------
Border
!
background
  <primitive: 'border get_background'>
!
background: aBrush
  <primitive: 'border set_background brush'>
!
borderBrush
  <primitive: 'border get_borderbrush'>
!
borderBrush: aBrush
  <primitive: 'border set_borderbrush brush'>
!
borderThickness
  <primitive: 'border get_borderthickness'>
!
borderThickness: aThickness
  <primitive: 'border set_borderthickness thickness'>
!
child
  <primitive: 'border get_child'>
!
child: aUiElement
  <primitive: 'border set_child uielement'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Border subclass: #MdiChildBase
instanceVariableNames: ''
classVariableNames: ''
maps: 'mdichildbase'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
close
  <primitive: 'mdichildbase close'>
!
color: aColor
  <primitive: 'mdichildbase setbordercolor color'>
!
location: x y: y
  <primitive: 'mdichildbase location double double'>
!
open
  <primitive: 'mdichildbase open'>
!
size: w height: h
  <primitive: 'mdichildbase size double double'>
!
tofront
  <primitive: 'mdichildbase tofront'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
MdiChildBase subclass: #DesktopWindow
instanceVariableNames: ''
classVariableNames: ''
maps: 'vdesktopwindow'
!
*-------------------------------------
* Class Methods
*-------------------------------------
DesktopWindow class
!
new
  <primitive: 'vdesktopwindow .ctor'>
!
new: aColor
  <primitive: 'vdesktopwindow .ctor color'>
!
open
  | w |
  w := self new.
  w open.
  ^ w
!
*-------------------------------------
* Instance Methods
*-------------------------------------
DesktopWindow
!
add: anElement
  anElement width: Double nan; height: Double nan.
  self content: anElement
!
clear
  self add: Canvas new
!
content
  <primitive: 'vdesktopwindow get_content'>
!
content: aUiElement
  <primitive: 'vdesktopwindow set_content uielement'>
!
title
  <primitive: 'vdesktopwindow get_title'>
!
title: aString
  <primitive: 'vdesktopwindow set_title string'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Decorator subclass: #ViewBox
instanceVariableNames: ''
classVariableNames: ''
maps: 'viewbox'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
FrameworkElement subclass: #HwndHost
instanceVariableNames: ''
classVariableNames: ''
maps: 'hwndhost'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
HwndHost subclass: #WindowsFormsHost
instanceVariableNames: ''
classVariableNames: ''
maps: 'windowsformshost'
!
*-------------------------------------
* Class Methods
*-------------------------------------
WindowsFormsHost class
!
new
  <primitive: 'windowsformshost .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
WindowsFormsHost
!
child
  <primitive: 'windowsformshost get_child'>
!
child: aWfControl
  <primitive: 'windowsformshost set_child wfcontrol'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
FrameworkElement subclass: #InkCanvas
instanceVariableNames: ''
classVariableNames: ''
maps: 'inkcanvas'
!
*-------------------------------------
* Class Methods
*-------------------------------------
InkCanvas class
!
new
  <primitive: 'inkcanvas .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
InkCanvas
!
background
  <primitive: 'inkcanvas get_background'>
!
background: aBrush
  <primitive: 'inkcanvas set_background brush'>
!
editingMode
  <primitive: 'inkcanvas get_editingmode'>
!
editingMode: anInkCanvasEditingMode
  <primitive: 'inkcanvas set_editingmode inkcanvaseditingmode'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
FrameworkElement subclass: #Panel
instanceVariableNames: ''
classVariableNames: ''
maps: 'panel'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
add: anElement
  self children add: anElement
!
background
  <primitive: 'panel get_background'>
!
background: aBrush
  <primitive: 'panel set_background brush'>
!
children
  <primitive: 'panel get_children'>
!
clear
  self children clear
!
clipToBounds
  <primitive: 'panel get_cliptobounds'>
!
clipToBounds: aBoolean
  <primitive: 'panel set_cliptobounds bool'>
!
getZindex: anElement
  <primitive: 'panel getzindex uielement'>
!
setZindex: anElement to: anInt
  <primitive: 'panel setzindex uielement int'>
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Panel subclass: #Canvas
instanceVariableNames: ''
classVariableNames: ''
maps: 'canvas'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Canvas class
!
getBottom: anElement
  <primitive: 'canvas getbottom uielement'>
!
getLeft: anElement
  <primitive: 'canvas getleft uielement'>
!
getRight: anElement
  <primitive: 'canvas getright uielement'>
!
getTop: anElement
  <primitive: 'canvas gettop uielement'>
!
leftProperty
  <primitive: 'canvas leftproperty'>
!
new
  <primitive: 'canvas .ctor'>
!
setBottom: anElement to: aDouble
  <primitive: 'canvas setbottom uielement double'>
!
setLeft: anElement to: aDouble
  <primitive: 'canvas setleft uielement double'>
!
setRight: anElement to: aDouble
  <primitive: 'canvas setright uielement double'>
!
setTop: anElement to: aDouble
  <primitive: 'canvas settop uielement double'>
!
topProperty
  <primitive: 'canvas topproperty'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Panel subclass: #DockPanel
instanceVariableNames: ''
classVariableNames: ''
maps: 'dockpanel'
!
*-------------------------------------
* Class Methods
*-------------------------------------
DockPanel class
!
new
  <primitive: 'dockpanel .ctor'>
!
setDock: aUiElement to: aDock
  <primitive: 'dockpanel setdock uielement dock'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
DockPanel
!
lastChildFill
  <primitive: 'dockpanel get_lastchildfill'>
!
lastChildFill: aBoolean
  <primitive: 'dockpanel set_lastchildfill bool'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Panel subclass: #Grid
instanceVariableNames: ''
classVariableNames: ''
maps: 'grid'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Grid class
!
hsplit: percent child1: c1 child2: c2
  ^ self new hsplit: percent child1: c1 child2: c2
!
hsplit: aBrush child1: c1 child2: c2 child3: c3 child4: c4
  ^ self new hsplit: aBrush child1: c1 child2: c2 child3: c3 child4: c4
!
new
  <primitive: 'grid .ctor'>
!
setColumn: anElement to: anInt
  <primitive: 'grid setcolumn uielement int'>
!
setRow: anElement to: anInt
  <primitive: 'grid setrow uielement int'>
!
vsplit: percent child1: c1 child2: c2
  ^ self new vsplit: percent child1: c1 child2: c2
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Grid
!
columnDefinitions
  <primitive: 'grid get_columnDefinitions'>
!
hsplit: percent child1: c1 child2: c2
  | splitter |
  self columnDefinitions add: (ColumnDefinition star: percent).
  self columnDefinitions add: (ColumnDefinition star: 1 - percent).
  splitter := GridSplitter new.
  splitter horizontalAlignment: HorizontalAlignment right.
  splitter width: 3.
  splitter background: (SolidColorBrush new: Color customOrange).
  Grid setColumn: splitter to: 0.
  Grid setColumn: c1 to: 0.
  Grid setColumn: c2 to: 1.
  self children add: c1.
  self children add: splitter.
  self children add: c2.
  c1 expandToFill.
  c2 expandToFill.
  c1 margin: (Thickness newLeft: 0 top: 0 right: 3 bottom: 0)
!
hsplit: aBrush child1: c1 child2: c2 child3: c3 child4: c4
  | splitter1 splitter2 splitter3 |
  self columnDefinitions add: (ColumnDefinition star: 0.25).
  self columnDefinitions add: (ColumnDefinition star: 0.25).
  self columnDefinitions add: (ColumnDefinition star: 0.25).
  self columnDefinitions add: (ColumnDefinition star: 0.25).
  splitter1 := GridSplitter new.
  splitter1 horizontalAlignment: HorizontalAlignment right.
  splitter1 width: 3.
  splitter1 background: aBrush.
  splitter2 := GridSplitter new.
  splitter2 horizontalAlignment: HorizontalAlignment right.
  splitter2 width: 3.
  splitter2 background: aBrush.
  splitter3 := GridSplitter new.
  splitter3 horizontalAlignment: HorizontalAlignment right.
  splitter3 width: 3.
  splitter3 background: aBrush.
  Grid setColumn: splitter1 to: 0.
  Grid setColumn: splitter2 to: 1.
  Grid setColumn: splitter3 to: 2.
  Grid setColumn: c1 to: 0.
  Grid setColumn: c2 to: 1.
  Grid setColumn: c3 to: 2.
  Grid setColumn: c4 to: 3.
  self children add: c1.
  self children add: splitter1.
  self children add: c2.
  self children add: splitter2.
  self children add: c3.
  self children add: splitter3.
  self children add: c4.
  c1 expandToFill.
  c2 expandToFill.
  c3 expandToFill.
  c4 expandToFill.
  c1 margin: (Thickness newLeft: 0 top: 0 right: 3 bottom: 0).
  c2 margin: (Thickness newLeft: 0 top: 0 right: 3 bottom: 0).
  c3 margin: (Thickness newLeft: 0 top: 0 right: 3 bottom: 0)
!
rowDefinitions
  <primitive: 'grid get_rowDefinitions'>
!
vsplit: percent child1: c1 child2: c2
  | splitter |
  self rowDefinitions add: (RowDefinition star: percent).
  self rowDefinitions add: (RowDefinition star: 1 - percent).
  splitter := GridSplitter new.
  splitter horizontalAlignment: HorizontalAlignment stretch.
  splitter verticalAlignment: VerticalAlignment bottom.
  splitter height: 3.
  splitter width: Double nan.
  splitter background: (SolidColorBrush new: Color customOrange).
  Grid setRow: splitter to: 0.
  Grid setRow: c1 to: 0.
  Grid setRow: c2 to: 1.
  self children add: c1.
  self children add: splitter.
  self children add: c2.
  c1 expandToFill.
  c2 expandToFill.
  c1 margin: (Thickness newLeft: 0 top: 0 right: 0 bottom: 3)
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Grid subclass: #View
instanceVariableNames: ''
classVariableNames: ''
maps: 'view'
!
*-------------------------------------
* Class Methods
*-------------------------------------
View class
!
new
  <primitive: 'view .ctor'>
!
views
  <primitive: 'view get_views'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
View
!
designMode
  <primitive: 'view get_designmode'>
!
designMode: aBoolean
  <primitive: 'view set_designmode bool'>
!
find: aString
  <primitive: 'view find string'>
!
generate
  <primitive: 'view generate'>
!
generate: aClassName parent: aSuperClassName
  <primitive: 'view generate string string'>
!
id
  <primitive: 'view get_id'>
!
isDraggable: aUiElement
  <primitive: 'view getdraggable uielement'>
!
keepInBounds
  <primitive: 'view get_keepinbounds'>
!
keepInBounds: aBoolean
  <primitive: 'view set_keepinbounds bool'>
!
packager
  <primitive: 'view get_packager'>
!
setDraggable: aUiElement to: aBoolean
  <primitive: 'view setdraggable uielement bool'>
!
setLocation: aName left: aDouble1 top: aDouble2
  <primitive: 'view setlocation string double double'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Panel subclass: #StackPanel
instanceVariableNames: ''
classVariableNames: ''
maps: 'stackpanel'
!
*-------------------------------------
* Class Methods
*-------------------------------------
StackPanel class
!
horizontal
  | sp |
  sp := self new.
  sp orientation: Orientation horizontal.
  ^ sp
!
new
  <primitive: 'stackpanel .ctor'>
!
vertical
  | sp |
  sp := self new.
  sp orientation: Orientation vertical.
  ^ sp
!
*-------------------------------------
* Instance Methods
*-------------------------------------
StackPanel
!
orientation
  <primitive: 'stackpanel get_orientation'>
!
orientation: anOrientation
  <primitive: 'stackpanel set_orientation orientation'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
FrameworkElement subclass: #Popup
instanceVariableNames: ''
classVariableNames: ''
maps: 'popup'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
FrameworkElement subclass: #Shape
instanceVariableNames: ''
classVariableNames: ''
maps: 'shape'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Shape class
!
new
  ^ self newprim init
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Shape
!
black
  self fill: Brushes black
!
blue
  self fill: Brushes blue.
  ^ self
!
fill
  <primitive: 'shape get_fill'>
!
fill: aBrush
  <primitive: 'shape set_fill brush'>
!
green
  self fill: Brushes green
!
init
  self size: 50; blue
!
opacity
  <primitive: 'control get_opacity'>
!
opacity: aDouble
  <primitive: 'shape set_opacity double'>
!
red
  self fill: Brushes red
!
size: anInteger
  self height: anInteger; width: anInteger
!
stroke
  <primitive: 'control get_stroke'>
!
stroke: aBrush
  <primitive: 'shape set_stroke brush'>
!
strokethickness
  <primitive: 'shape get_strokethickness'>
!
strokethickness: aDouble
  <primitive: 'shape set_strokethickness double'>
!
yellow
  self fill: Brushes yellow
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Shape subclass: #Ellipse
instanceVariableNames: ''
classVariableNames: ''
maps: 'ellipse'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Ellipse class
!
newprim
  <primitive: 'ellipse .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Ellipse
!
radius: aDouble
  self width: aDouble; height: aDouble
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Shape subclass: #Line
instanceVariableNames: ''
classVariableNames: ''
maps: 'line'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Shape subclass: #Path
instanceVariableNames: ''
classVariableNames: ''
maps: 'path'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Path class
!
new
  <primitive: 'path .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Path
!
data
  <primitive: 'path get_data'>
!
data: aGeometry
  <primitive: 'path set_data geometry'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Shape subclass: #Polygon
instanceVariableNames: ''
classVariableNames: ''
maps: 'polygon'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Shape subclass: #Rectangle
instanceVariableNames: ''
classVariableNames: ''
maps: 'rectangle'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Rectangle class
!
newprim
  <primitive: 'rectangle .ctor'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #Duration
instanceVariableNames: ''
classVariableNames: ''
maps: 'duration'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Duration class
!
new: aDuration
  <primitive: 'duration .ctor timespan'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #FontWeights
instanceVariableNames: ''
classVariableNames: ''
maps: 'fontweights'
!
*-------------------------------------
* Class Methods
*-------------------------------------
FontWeights class
!
black
 <primitive: 'fontweights get_black'>
!
bold
 <primitive: 'fontweights get_bold'>
!
demibold
 <primitive: 'fontweights get_demibold'>
!
extrablack
 <primitive: 'fontweights get_extrablack'>
!
extrabold
 <primitive: 'fontweights get_extrabold'>
!
extralight
 <primitive: 'fontweights get_extralight'>
!
heavy
 <primitive: 'fontweights get_heavy'>
!
light
 <primitive: 'fontweights get_light'>
!
medium
 <primitive: 'fontweights get_medium'>
!
normal
 <primitive: 'fontweights get_normal'>
!
regular
 <primitive: 'fontweights get_regular'>
!
semibold
 <primitive: 'fontweights get_semibold'>
!
thin
 <primitive: 'fontweights get_thin'>
!
ultrablack
 <primitive: 'fontweights get_ultrablack'>
!
ultrabold
 <primitive: 'fontweights get_ultrabold'>
!
ultrathin
 <primitive: 'fontweights get_ultrathin'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #INameScope
instanceVariableNames: ''
classVariableNames: ''
maps: 'inamescope'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #ItemCollection
instanceVariableNames: ''
classVariableNames: ''
maps: 'itemcollection'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
add: anObject
  <primitive: 'itemcollection add object'>
!
at: anInteger
  <primitive: 'itemcollection get_item int'>
!
clear
  <primitive: 'itemcollection clear'>
!
do: aBlock
  1 to: self size do:[:i | aBlock value: (self at: (i - 1))]
!
size
  <primitive: 'itemcollection get_count'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #NameScope
instanceVariableNames: ''
classVariableNames: ''
maps: 'namescope'
!
*-------------------------------------
* Class Methods
*-------------------------------------
NameScope class
!
new
  <primitive: 'namescope .ctor'>
!
setNameScope: aDependencyObject to: aNameScope
  <primitive: 'namescope setnamescope dependencyobject inamescope'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #PropertyPath
instanceVariableNames: ''
classVariableNames: ''
maps: 'propertypath'
!
*-------------------------------------
* Class Methods
*-------------------------------------
PropertyPath class
!
new: aDependencyProperty
  <primitive: 'propertypath .ctor object'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #ResourceDictionary
instanceVariableNames: ''
classVariableNames: ''
maps: 'resourcedictionary'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #RowDefinitionCollection
instanceVariableNames: ''
classVariableNames: ''
maps: 'rowdefinitioncollection'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
RowDefinitionCollection
!
add: aUiElement
  <primitive: 'rowdefinitioncollection add rowdefinition'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #TriggerActionCollection
instanceVariableNames: ''
classVariableNames: ''
maps: 'triggeractioncollection'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
TriggerActionCollection
!
add: aTriggerAction
  <primitive: 'triggeractioncollection add triggeraction'>
!
clear
  <primitive: 'triggeractioncollection clear'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #TriggerCollection
instanceVariableNames: ''
classVariableNames: ''
maps: 'triggercollection'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
TriggerCollection
!
add: aTriggerBase
  <primitive: 'triggercollection add triggerbase'>
!
clear
  <primitive: 'triggercollection clear'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #UIElementCollection
instanceVariableNames: ''
classVariableNames: ''
maps: 'uielementcollection'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
add: aTriggerAction
  <primitive: 'uielementcollection add uielement'>
!
at: index
  <primitive: 'uielementcollection get_item int'>
!
at: index put: anElement
  self removeAt: index.
  self insert: index element: anElement
!
clear
  <primitive: 'uielementcollection clear'>
!
do: aBlock
  1 to: self size do:[:i | aBlock value: (self at: (i - 1))]
!
insert: index element: anElement
  <primitive: 'uielementcollection insert int uielement'>
!
last
  ^ self at: self size - 1
!
remove: aUielement
  <primitive: 'uielementcollection remove uielement'>
!
removeAt: index
  <primitive: 'uielementcollection removeat int'>
!
size
  <primitive: 'uielementcollection count'>
!
tofront: aUielement
  self remove: aUielement; add: aUielement
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Wpf subclass: #ValueType
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
ValueType subclass: #Enum
instanceVariableNames: ''
classVariableNames: ''
maps: 'enum'
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Enum subclass: #Dock
instanceVariableNames: ''
classVariableNames: ''
maps: 'dock'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Dock class
!
bottom
  ^ Vm findEnum: 'dock' for: 'bottom'
!
left
  ^ Vm findEnum: 'dock' for: 'left'
!
right
  ^ Vm findEnum: 'dock' for: 'right'
!
top
  ^ Vm findEnum: 'dock' for: 'top'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Enum subclass: #GridUnitType
instanceVariableNames: ''
classVariableNames: ''
maps: 'gridunittype'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Enum subclass: #HorizontalAlignment
instanceVariableNames: ''
classVariableNames: ''
maps: 'horizontalalignment'
!
*-------------------------------------
* Class Methods
*-------------------------------------
HorizontalAlignment class
!
center
  ^ Vm findEnum: 'horizontalalignment' for: 'center'
!
left
  ^ Vm findEnum: 'horizontalalignment' for: 'left'
!
right
  ^ Vm findEnum: 'horizontalalignment' for: 'right'
!
stretch
  ^ Vm findEnum: 'horizontalalignment' for: 'stretch'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Enum subclass: #InkCanvasEditingMode
instanceVariableNames: ''
classVariableNames: ''
maps: 'inkcanvaseditingmode'
!
*-------------------------------------
* Class Methods
*-------------------------------------
InkCanvasEditingMode class
!
eraseByPoint
  ^ Vm findEnum: 'inkcanvaseditingmode' for: 'erasebypoint'
!
eraseByStroke
  ^ Vm findEnum: 'inkcanvaseditingmode' for: 'erasebystroke'
!
gestureOnly
  ^ Vm findEnum: 'inkcanvaseditingmode' for: 'gestureonly'
!
ink
  ^ Vm findEnum: 'inkcanvaseditingmode' for: 'ink'
!
inkAndGesture
  ^ Vm findEnum: 'inkcanvaseditingmode' for: 'inkandgesture'
!
none
  ^ Vm findEnum: 'inkcanvaseditingmode' for: 'none'
!
select
  ^ Vm findEnum: 'inkcanvaseditingmode' for: 'select'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Enum subclass: #Orientation
instanceVariableNames: ''
classVariableNames: ''
maps: 'orientation'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Orientation class
!
horizontal
  ^ Vm findEnum: 'orientation' for: 'horizontal'
!
vertical
  ^ Vm findEnum: 'orientation' for: 'vertical'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Enum subclass: #TextAlignment
instanceVariableNames: ''
classVariableNames: ''
maps: 'textalignment'
!
*-------------------------------------
* Class Methods
*-------------------------------------
TextAlignment class
!
center
  ^ Vm findEnum: 'textalignment' for: 'center'
!
justify
  ^ Vm findEnum: 'textalignment' for: 'justify'
!
left
  ^ Vm findEnum: 'textalignment' for: 'left'
!
right
  ^ Vm findEnum: 'textalignment' for: 'right'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Enum subclass: #TextWrapping
instanceVariableNames: ''
classVariableNames: ''
maps: 'textwrapping'
!
*-------------------------------------
* Class Methods
*-------------------------------------
TextWrapping class
!
nowrap
  ^ Vm findEnum: 'textwrapping' for: 'nowrap'
!
wrap
  ^ Vm findEnum: 'textwrapping' for: 'wrap'
!
wrapWithOverflow
  ^ Vm findEnum: 'textwrapping' for: 'wrapwithoverflow'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Enum subclass: #VerticalAlignment
instanceVariableNames: ''
classVariableNames: ''
maps: 'verticalalignment'
!
*-------------------------------------
* Class Methods
*-------------------------------------
VerticalAlignment class
!
bottom
  ^ Vm findEnum: 'verticalalignment' for: 'bottom'
!
center
  ^ Vm findEnum: 'verticalalignment' for: 'center'
!
stretch
  ^ Vm findEnum: 'verticalalignment' for: 'stretch'
!
top
  ^ Vm findEnum: 'verticalalignment' for: 'top'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Enum subclass: #WfDockStyle
instanceVariableNames: ''
classVariableNames: ''
maps: 'wfdockstyle'
!
*-------------------------------------
* Class Methods
*-------------------------------------
WfDockStyle class
!
bottom
  ^ Vm findEnum: 'wfdockstyle' for: 'bottom'
!
fill
  ^ Vm findEnum: 'wfdockstyle' for: 'fill'
!
left
  ^ Vm findEnum: 'wfdockstyle' for: 'left'
!
right
  ^ Vm findEnum: 'wfdockstyle' for: 'right'
!
top
  ^ Vm findEnum: 'wfdockstyle' for: 'top'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ValueType subclass: #FontWeight
instanceVariableNames: ''
classVariableNames: ''
maps: 'fontweight'
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ValueType subclass: #GridLength
instanceVariableNames: ''
classVariableNames: ''
maps: 'gridlength'
!
*-------------------------------------
* Class Methods
*-------------------------------------
GridLength class
!
new: aDouble type: aGridUnitType
  <primitive: 'gridlength .ctor double gridunittype'>
!
star: aDouble
  ^ self new: aDouble type: (Vm findEnum: 'gridunittype' for: 'star')
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ValueType subclass: #Matrix
instanceVariableNames: ''
classVariableNames: ''
maps: 'matrix'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Matrix class
!
m11: d1 m12: d2 m21: d3 m22: d4 x: d5 y: d6
  <primitive: 'matrix .ctor double double double double double double'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Matrix
!
scaleX: aDouble1 y: aDouble2
  <primitive: 'matrix scale double double'>
!
skewX: aDouble1 y: aDouble2
  <primitive: 'matrix skew double double'>
!
translateX: aDouble1 y: aDouble2
  <primitive: 'matrix translate double double'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ValueType subclass: #Point
instanceVariableNames: ''
classVariableNames: ''
maps: 'point'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Point class
!
x: d1 y: d2
  <primitive: 'point .ctor double double'>
!
*-------------------------------------
* Class Methods
*-------------------------------------
Point
!
+ aPoint
  | x1 y1 x2 y2 |
  x1 := self x.
  y1 := self y.
  x2 := aPoint x.
  y2 := aPoint y.
  ^ Point x: x1+x2 y: y1+y2
!
x
  <primitive: 'point get_x'>
!
x: anInt
  <primitive: 'point set_x int'>
!
y
  <primitive: 'point get_y'>
!
y: anInt
  <primitive: 'point set_y int'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ValueType subclass: #Thickness
instanceVariableNames: ''
classVariableNames: ''
maps: 'thickness'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Thickness class
!
left: n1 top: n2 right: n3 bottom: n4
  <primitive: 'thickness .ctor double double double double'>
!
new: aDouble
  <primitive: 'thickness .ctor double'>
!
newLeft: n1 top: n2 right: n3 bottom: n4
  <primitive: 'thickness .ctor double double double double'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Thickness
!
bottom
  <primitive: 'thickness get_bottom'>
!
left
  <primitive: 'thickness get_left'>
!
right
  <primitive: 'thickness get_right'>
!
top
  <primitive: 'thickness get_top'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
ValueType subclass: #TimeSpan
instanceVariableNames: ''
classVariableNames: ''
maps: 'timespan'
!
*-------------------------------------
* Class Methods
*-------------------------------------
TimeSpan class
!
fromSeconds: aDouble
  <primitive: 'timespan fromseconds double'>
!
*-------------------------------------
* Load classes
*-------------------------------------
*-------------------------------------
* Instance Methods
*-------------------------------------
Object
!
addEvent: aString action: aBlock
  Handler on: self event: aString block: aBlock
!
asConsOrAtom
  ^ self
!
performArray: anArray
  self perform: anArray first asSymbol withArguments: anArray withoutFirst
!
performArrayIndirect: anArray
  (Smalltalk at: anArray first asSymbol) perform: anArray second asSymbol
                                   withArguments: anArray withoutFirst2
!
*-------------------------------------
* Load classes
*-------------------------------------
*-------------------------------------
* Instance Methods
*-------------------------------------
Array
!
asArray
  ^ self
!
asCons
  ^ Cons fromArray: self
!
first
  ^ self at: 1
!
second
  ^ self at: 2
!
sorted
  <primitive: 'varray sorted'>
!
withoutFirst
  <primitive: 'varray withoutfirst'>
!
withoutFirst2
  ^ self withoutFirst withoutFirst
!
*-------------------------------------
* Class Declaration
*-------------------------------------
OrderedCollection subclass: #Cons
instanceVariableNames: ''
classVariableNames: ''
maps: 'vcons'
!
*-------------------------------------
* Class Methods
*-------------------------------------
Cons class
!
car: anObject
  <primitive: 'vcons .ctor object'>
!
car: anObject1 cdr: anObject2
  <primitive: 'vcons .ctor object object'>
!
fromArray: anArray
  <primitive: 'vcons fromarray varray'>
!
new
  <primitive: 'vcons .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Cons
!
asArray
  <primitive: 'vcons toarray'>
!
asCons
  ^ self
!
car
  <primitive: 'vcons get_car'>
!
cdr
  <primitive: 'vcons get_cdr'>
!
length
  <primitive: 'vcons get_length'>
!
push: anObject
  <primitive: 'vcons push object'>
!
sorted
  ^ self asArray sorted asCons
!
*-------------------------------------
* Instance Methods
*-------------------------------------
List
!
asCons
  ^ self asArray asCons
!
*-------------------------------------
* Instance Methods
*-------------------------------------
OrderedCollection
!
asConsOrAtom
  ^ self asCons
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Kernel subclass: #EventArgs
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
EventArgs subclass: #ElementMovedArgs
instanceVariableNames: ''
classVariableNames: ''
maps: 'elementmovedargs'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
ElementMovedArgs
!
name
  <primitive: 'elementmovedargs get_name'>
!
x
  <primitive: 'elementmovedargs get_x'>
!
y
  <primitive: 'elementmovedargs get_y'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Kernel subclass: #Lisp
instanceVariableNames: ''
classVariableNames: ''
maps: 'lispvm'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Lisp class
!
eval: anObject
  ^ self evalPrim: anObject asConsOrAtom
!
evalPrim: aConsOrAtom
  <primitive: 'lispvm eval object'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Object subclass: #Models
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
String
!
encodeBase64
  ^ StringUtility base64Encode: self
!
decodeBase64
  ^ StringUtility base64Decode: self
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Object subclass: #Utility
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Utility subclass: #JsonConvert
instanceVariableNames: ''
classVariableNames: ''
maps: 'jsonconvert'
!
*-------------------------------------
* Class Methods
*-------------------------------------
JsonConvert class
!
fromJson: aString
  <primitive: 'jsonconvert fromjson string'>
!
toJson: aDictionary
  <primitive: 'jsonconvert tojson idictionary'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Utility subclass: #StringUtility
instanceVariableNames: ''
classVariableNames: ''
maps: 'stringutility'
!
*-------------------------------------
* Class Methods
*-------------------------------------
StringUtility class
!
asSmalltalkString: aString
  <primitive: 'stringutility assmalltalkstring string'>
!
base64Decode: aString
  <primitive: 'stringutility base64decode string'>
!
base64Encode: aString
  <primitive: 'stringutility base64encode string'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Utility subclass: #ViewPackager
instanceVariableNames: ''
classVariableNames: ''
maps: 'viewpackager'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
package
  <primitive: 'viewpackager get_package'>
!
package: anObject
  <primitive: 'viewpackager set_package object'>
!
xaml
  <primitive: 'viewpackager get_xaml'>
!
xaml: aString
  <primitive: 'viewpackager set_xaml string'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Utility subclass: #XamlUtility
instanceVariableNames: ''
classVariableNames: ''
maps: 'xamlutility'
!
*-------------------------------------
* Class Methods
*-------------------------------------
XamlUtility class
!
xamlFromString: aString
  <primitive: 'xamlutility xamlfromstring string'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Utility subclass: #XmlUtility
instanceVariableNames: ''
classVariableNames: ''
maps: 'xmlutility'
!
*-------------------------------------
* Class Methods
*-------------------------------------
XmlUtility class
!
childNodes: anObject
  <primitive: 'xmlutility childnodes object'>
!
childNodes: anXmlDocument
  <primitive: 'xmlutility childnodes xmldocument'>
!
childNodes: anXmlElement
  <primitive: 'xmlutility childnodes xmlelement'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Object subclass: #Views
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Object subclass: #GraphicLibraries
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
GraphicLibraries subclass: #Wheel
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Class Methods
*-------------------------------------
Wheel class
!
createWheel: numberOfSpokes radius: aRadius center: aPoint
  | angle group rim wheel |
  angle := Math pi / numberOfSpokes.
  group := GeometryGroup new.
  rim := EllipseGeometry center: aPoint radius: aRadius.
  group add: rim.
  0 to: (numberOfSpokes - 1) do: [:i |
    group add: (self spoke: i * angle center: aPoint radius: aRadius)].
  wheel := Path new.
  wheel data: group; stroke: Brushes black.
  ^ wheel
!
spoke: anAngle center: aPoint radius: aRadius
  | xoffset yoffset |
  xoffset := aRadius * (Math sin: anAngle).
  yoffset := aRadius * (Math cos: anAngle).
  ^ LineGeometry start: aPoint + (Point x: xoffset y: yoffset)
                   end: aPoint+ (Point x: xoffset minus y: yoffset minus)
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Object subclass: #Internet
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Class Methods
*-------------------------------------
Internet class
!
readTextFromUrl: aUrl callback: aBlock
  | php |
  php := PhpConnect new.
  php url: aUrl.
  php callback: aBlock.
  php queryServer: ''
!
*-------------------------------------
* Load subclasses
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Internet subclass: #GmailConnect
instanceVariableNames: ''
classVariableNames: ''
maps: 'gmailconnect'
!
*-------------------------------------
* Class Methods
*-------------------------------------
GmailConnect class
!
send: fromString to: toString cc: ccString bcc: bccString subject: subjectString body: bodyString
  <primitive: 'gmailconnect sendmail string string string string string string'>
!
send: fromString to: toString subject: subjectString body: bodyString
  self send: fromString to: toString cc: '' bcc: '' subject: subjectString body: bodyString
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Internet subclass: #HttpUtility
instanceVariableNames: ''
classVariableNames: ''
maps: 'httputility'
!
*-------------------------------------
* Class Methods
*-------------------------------------
HttpUtility class
!
urlDecode: aString
  <primitive: 'httputility urldecode string'>
!
urlEncode: aString
  <primitive: 'httputility urlencode string'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Internet subclass: #JsonConnect
instanceVariableNames: ''
classVariableNames: ''
maps: 'jsonconnect'
!
*-------------------------------------
* Class Methods
*-------------------------------------
JsonConnect class
!
new
  <primitive: 'jsonconnect .ctor'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
JsonConnect
!
callback
  <primitive: 'jsonconnect get_callback'>
!
callback: aBlock
  <primitive: 'jsonconnect set_callback object'>
!
queryServer: aQueryString
  <primitive: 'jsonconnect queryserverasync string'>
!
rawResponse
  <primitive: 'jsonconnect get_rawresponse'>
!
rawResponse: aBoolean
  <primitive: 'jsonconnect set_rawresponse bool'>
!
sendData: anObject
  <primitive: 'jsonconnect senddata object'>
!
url
  <primitive: 'jsonconnect get_url'>
!
url: aString
  <primitive: 'jsonconnect set_url string'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Internet subclass: #MessagingBase
instanceVariableNames: ''
classVariableNames: ''
maps: 'messagingbase'
!
*-------------------------------------
* Instance Methods
*-------------------------------------
MessagingBase
!
callback
  <primitive: 'messagingbase get_callback'>
!
callback: aBlock
  <primitive: 'messagingbase set_callback object'>
!
channel
  <primitive: 'messagingbase get_channel'>
!
channel: aString
  <primitive: 'messagingbase set_channel string'>
!
contact
  <primitive: 'messagingbase get_contact'>
!
contact: aString
  <primitive: 'messagingbase set_contact string'>
!
*-------------------------------------
* Load subclasses
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
MessagingBase subclass: #JabberConnect
instanceVariableNames: ''
classVariableNames: ''
maps: 'jabberconnect'
!
*-------------------------------------
* Class Methods
*-------------------------------------
JabberConnect class
!
new: aBoolean
  <primitive: 'jabberconnect .ctor bool'>
!
*-------------------------------------
* Instance Methods
*-------------------------------------
JabberConnect
!
close
  <primitive: 'jabberconnect close'>
!
connect
  <primitive: 'jabberconnect connect'>
!
passwd: aString
  <primitive: 'jabberconnect set_passwd string'>
!
send: anObject
  self send: self contact object: anObject
!
send: aName object: anObject
  <primitive: 'jabberconnect sendto string object'>
!
user: aString
  <primitive: 'jabberconnect set_user string'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
MessagingBase subclass: #TmsConnect
instanceVariableNames: ''
classVariableNames: ''
maps: 'tmsconnect'
!
*-------------------------------------
* Class Methods
*-------------------------------------
TmsConnect class
!
new
  <primitive: 'tmsconnect .ctor'>
!
channel: aChannel name: aName
  | tms |
  tms := self new.
  tms url: 'http://vistascript.net/vistascript/webserv/tms.php'.
  tms channel: aChannel.
  tms name: aName.
  tms clear.
  ^ tms
!
channel: aChannel name: aName callback: aBlock
  | tms |
  tms := self channel: aChannel name: aName.
  tms callback: aBlock.
  ^ tms
!
*-------------------------------------
* Instance Methods
*-------------------------------------
TmsConnect
!
clear
  <primitive: 'tmsconnect clear'>
!
listen
  <primitive: 'tmsconnect listen'>
!
name
  <primitive: 'tmsconnect get_name'>
!
name: aBlock
  <primitive: 'tmsconnect set_name string'>
!
send: anObject
  <primitive: 'tmsconnect send object'>
!
send64: anObject
  self send: anObject asString encodeBase64
!
startPolling: anInteger
  <primitive: 'tmsconnect startpolling int'>
!
stopPolling
  <primitive: 'tmsconnect stoppolling'>
!
url
  <primitive: 'tmsconnect get_url'>
!
url: aString
  <primitive: 'tmsconnect set_url string'>
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Object subclass: #Tools
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Class Methods
*-------------------------------------
Tools class
!
open
  | w |
  w := super new.
  w open.
  ^ w
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Tools
!
open
  ^ nil
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Tools subclass: #ClassBrowser
instanceVariableNames: 'classesTv classDict classLb instDict instLb selectedClass tabctrl texteditor root window'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
classMethodSelection
  | selector method |
  classLb selectedItem notNil ifTrue: [
    selector := classLb selectedItem content.
    method := selectedClass class lookupSelector: selector.
    self showMethodText: method]
!
classSelectionChanged
  | cls |
  classesTv selectedItem isNil ifTrue:[^nil].
  cls := classesTv selectedItem tag.
  selectedClass := cls.
  self refreshMethodsLists.
  self showClassText
!
implementorsClassMethod
  | selector |
  selector := classLb selectedItem content.
  (Vm implementorsOf: selector) inspect
!
implementorsInstMethod
   | selector |
   selector := instLb selectedItem content.
  (Vm implementorsOf: selector) inspect
!
instMethodSelection
  | selector method |
  instLb selectedItem notNil ifTrue: [
    selector := instLb selectedItem content.
    method := selectedClass lookupSelector: selector.
    self showMethodText: method]
!
open
  "   ClassBrowser open   " 
  | tab |
  window := ThreePaneTreeTabEditor new.
  window color: Colors darkCyan.
  window title: 'Class Browser'.
  classesTv := window treeview.
  classesTv background: (SolidColorBrush new: Colors darkSlateGray).
  tabctrl := window tabcontrol.
  texteditor := window textbox.
  texteditor foreground: Brushes navy.
  texteditor background: Brushes ivory.
  texteditor showLines: true.
  instLb := ListBox new.
  instLb background: (SolidColorBrush new: Colors darkSlateGray).
  instLb borderbrush: (SolidColorBrush new: Color customDarkGray).
  instLb foreground: (SolidColorBrush new: Colors gold).
  classLb := ListBox new.
  classLb background: (SolidColorBrush new: Colors darkSlateGray).
  classLb borderbrush: (SolidColorBrush new: Color customDarkGray).
  classLb foreground: (SolidColorBrush new: Colors gold).
  tab := TabItem new.
  tab header: 'Inst'.
  tab content: instLb.
  window tabcontrol tabStripPlacement: Dock bottom.
  window tabcontrol items add: tab.
  tab := TabItem new.
  tab header: 'Class'.
  tab content: classLb.
  window tabcontrol items add: tab.
  classesTv addEvent: 'SelectedItemChanged' action: [self classSelectionChanged].
  instLb addEvent: 'SelectionChanged' action: [self instMethodSelection].
  classLb addEvent: 'SelectionChanged' action: [self classMethodSelection].
  self setInstLbContextMenu; setClassLbContextMenu; setTextBoxContextMenu.
  self setFontSize.
  window open.
  self refreshClassTree
!
refreshClassTree
  | rootClass rootNode |
  rootClass := Vm rootClass.
  classesTv items clear.
  rootNode := TreeViewItem new.
  rootNode foreground: (SolidColorBrush new: Colors gold).
  rootNode header: rootClass name.
  rootNode tag: rootClass.
  classesTv items add: rootNode.
  self refreshClassTree: rootClass node: rootNode
!
refreshClassTree: aClass node: aNode
  | childNode |
  aClass subclasses do:[:cls |
    childNode := TreeViewItem new.
    childNode foreground: (SolidColorBrush new: Colors gold).
    childNode header: cls name.
    childNode tag: cls.
    aNode items add: childNode.
    self refreshClassTree: cls node: childNode]
!
refreshMethodsLists
  | li |
  instDict := selectedClass methodDictionary.
  classDict := selectedClass class methodDictionary.
  instLb items clear.
  classLb items clear.
  texteditor text: ''.
  instDict keysDo: [:k |
    li := ListBoxItem new.
    li content: k.
    instLb items add: li].
  classDict keysDo: [:k |
    li := ListBoxItem new.
    li content: k.
    classLb items add: li]
!
removeClassMethod
  | selector |
  selector := classLb selectedItem content.
  selectedClass class removeMethod: selector.
  self refreshMethodsLists
!
removeInstMethod
  | selector |
  selector := instLb selectedItem content.
  selectedClass removeMethod: selector.
  self refreshMethodsLists
!
selectedClass
  | index |
  index := tabctrl selectedIndex.
  index == 0
    ifTrue: [^ selectedClass]
    ifFalse: [^ selectedClass class]
!
sendersClassMethod
  | selector |
  selector := classLb selectedItem content.
  (Vm sendersOf: selector) inspect
!
sendersInstMethod
   | selector |
   selector := instLb selectedItem content.
  (Vm sendersOf: selector) inspect
!
setClassLbContextMenu
  | menu |
  menu := ContextMenu new.
  menu add: (MenuItem name: 'Implementors' action: [self implementorsClassMethod]).
  menu add: (MenuItem name: 'Senders' action: [self sendersClassMethod]).
  menu add: (MenuItem name: 'Remove' action: [self removeClassMethod]).
  classLb contextMenu: menu.
!
setFontSize
  | fntsize |
  fntsize := 12.
  classesTv fontsize: fntsize.
  classLb fontsize: fntsize.
  instLb fontsize: fntsize.
!
setInstLbContextMenu
  | menu |
  menu := ContextMenu new.
  menu add: (MenuItem name: 'Implementors' action: [self implementorsInstMethod]).
  menu add: (MenuItem name: 'Senders' action: [self sendersInstMethod]).
  menu add: (MenuItem name: 'Remove' action: [self removeInstMethod]).
  instLb contextMenu: menu.
!
setTextBoxContextMenu
  | menu |
  menu := ContextMenu new.
  menu add: (MenuItem name: 'Save' action: [self textSave]).
  menu add: Separator new.
  menu add: (MenuItem name: 'Breakpoints' action: [self toggleBreakpoints]).
  menu add: (MenuItem name: 'Linenumbers' action: [self toggleLines]).
  texteditor contextMenu: menu.
!
showClassText
  texteditor clearBreakpoints.
  texteditor text: selectedClass storeString
!
showMethodText: aMethod
  texteditor setMethod: aMethod.
  texteditor text: aMethod text
!
textSave
  | txt |
  txt := texteditor text trim.
  txt length == 0 ifTrue: [^nil].
  ((txt at: 0) isUpper and: (txt indexOf: ' subclass:') > 0)
  ifTrue: [
    SmalltalkReader addSubclass: self selectedClass string: txt.
    self refreshClassTree]
  ifFalse: [
    SmalltalkReader addMethod: self selectedClass string: txt.
    self refreshMethodsLists]
!
toggleBreakpoints
  texteditor showBreakpoints: texteditor showBreakpoints not
!
toggleLines
  texteditor showLines: texteditor showLines not
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Tools subclass: #Inspectors
instanceVariableNames: 'view'
classVariableNames: ''
!
*-------------------------------------
* Class Methods
*-------------------------------------
Inspectors class
!
openOn: anObject
  | w |
  w := self new.
  w openOn: anObject.
  ^ w
!
*-------------------------------------
* Instance Methods
*-------------------------------------
Inspectors
!
view
  ^ view
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Inspectors subclass: #ArrayInspector
instanceVariableNames: 'array'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
currentValue
  | index |
  index := view listbox selectedIndex + 1.
  ^ array at: index
!
inspectValue
  self currentValue inspect
!
openOn: anOrderedCollection
  | menu menuitem |
  array := anOrderedCollection asArray.
  view := TwoPaneListText new.
  view title: 'ArrayInspector on ' + anOrderedCollection class printString.
  view listbox addEvent: 'SelectionChanged' action: [self showValue].
  menu := ContextMenu new.
  menuitem := MenuItem new.
  menuitem header: 'Inspect'.
  menuitem addEvent: 'Click' action: [self inspectValue].
  menu items add: menuitem.
  view listbox contextMenu: menu.
  view open.
  self showIndices
!
showIndices
  | li n |
  n := array size.
  1 to: n do:[:i |
    li := ListBoxItem new.
    li content: i.
    view listbox items add: li]
!
showValue
  view textbox text: self currentValue printString
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Inspectors subclass: #DefaultInspector
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
openOn: anObject
  view := OnePaneText new.
  view title: 'DefaultInspector on ' + anObject class printString.
  view text: anObject printString.
  view open
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Inspectors subclass: #DictionaryInspector
instanceVariableNames: 'dictionary'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
currentValue
  | key |
  key := view listbox selectedItem content.
  ^ dictionary at: key
!
inspectValue
  self currentValue inspect
!
openOn: aDictionary
  | menu menuitem |
  dictionary := aDictionary.
  view := TwoPaneListText new.
  view title: 'DictionaryInspector on ' + aDictionary class printString.
  view listbox addEvent: 'SelectionChanged' action: [self showValue].
  menu := ContextMenu new.
  menuitem := MenuItem new.
  menuitem header: 'Inspect'.
  menuitem addEvent: 'Click' action: [self inspectValue].
  menu items add: menuitem.
  view listbox contextMenu: menu.
  view open.
  self showKeys
!
showKeys
  | li |
  dictionary keysDo:[:k |
    li := ListBoxItem new.
    li content: k.
    view listbox items add: li]
!
showValue
  view textbox text: self currentValue asString
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Tools subclass: #Launcher
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
open
  "   Launcher open   "
  | panel w |
  panel := StackPanel new.
  panel add: (Button content: 'Demos' callback: [DemoLauncher open]).
  panel add: (Button content: 'Games' callback: [GamesLauncher open]).
  panel add: (Button content: 'Graphics' callback: [GraphicsLauncher open]).
  panel add: (Button content: 'Remote' callback: [RemoteLauncher open]).
  panel add: (Button content: 'Xaml' callback: [XamlLauncher open]).
  panel add: (Button content: 'Designer' callback: [OnePaneDesigner open]).
  w := self topWindowClass new.
  w width: 150; height: 170; title: 'Main Launcher'; add: panel; open
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Tools subclass: #Workspace
instanceVariableNames: 'texteditor window'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
addContextMenu
  | menu menuitem |
  menu := ContextMenu new.
  menuitem := MenuItem new.
  menuitem header: 'Do It'.
  menuitem addEvent: 'Click' action: [self doIt].
  menu items add: menuitem.
  menuitem := MenuItem new.
  menuitem header: 'Show It'.
  menuitem addEvent: 'Click' action: [self showIt].
  menu items add: menuitem.
  menuitem := MenuItem new.
  menuitem header: 'Inspect It'.
  menuitem addEvent: 'Click' action: [self inspectIt].
  menu items add: menuitem.
  menu add: Separator new.
  menuitem := MenuItem new.
  menuitem header: 'Clear'.
  menuitem addEvent: 'Click' action: [texteditor text: ''].
  menu items add: menuitem.
  menuitem := MenuItem new.
  menuitem header: 'Breakpoints'.
  menuitem addEvent: 'Click' action: [self toggleBreakpoints].
  menu items add: menuitem.
  menuitem := MenuItem new.
  menuitem header: 'Linenumbers'.
  menuitem addEvent: 'Click' action: [self toggleLines].
  menu items add: menuitem.
  texteditor contextMenu: menu.
!
doIt
  | method str |
  str := texteditor selectedText trim.
  str length == 0 ifTrue: [^nil].
  method := Compiler compileSnippet: str.
  method isNil ifTrue:[^nil].
  Processor scheduleProcess: (Process fromMethod: method)
!
inspectIt
  | method process str |
  str := texteditor selectedText trim.
  str length == 0 ifTrue: [^nil].
  method := Compiler compileSnippet: str.
  method isNil ifTrue:[^nil].
  process := Process fromMethod: method.
  process callback: [:val | self inspectResult: val].
  Processor scheduleProcess: process
!
inspectResult: anObject
  anObject inspect
!
methodTemplate
  texteditor println: '| myVar |'.
  texteditor println: ''.
  texteditor println: 'Transcript showCr:'.
  texteditor selectionStart: 2.
!
open
  "   Workspace open   "
  window := OnePaneTextEditor new.
  window color: Colors darkGoldenrod.
  texteditor := window pane.
  texteditor foreground: Brushes navy.
  window title: 'Workspace'.
  self addContextMenu.
  window open
!
showIt
  | method process str |
  str := texteditor selectedText trim.
  str length == 0 ifTrue: [^nil].
  method := Compiler compileSnippet: str.
  method isNil ifTrue:[^nil].
  process := Process fromMethod: method.
  process callback: [:val | self showResult: val].
  Processor scheduleProcess: process
!
showResult: anObject
  texteditor insertAtEndOfSelection: anObject printString
!
toggleBreakpoints
  texteditor showBreakpoints: texteditor showBreakpoints not
!
toggleLines
  texteditor showLines: texteditor showLines not
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Object subclass: #UserApps
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Class Methods
*-------------------------------------
UserApps class
!
open
  ^ super new open
!
*-------------------------------------
* Instance Methods
*-------------------------------------
UserApps
!
open
  ^ nil
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
UserApps subclass: #Demos
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Demos subclass: #DemoLauncher
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
open
  " DemoLauncher open "
  | panel w |
  panel := StackPanel new.
  panel add: (Button content: 'RssDemo' callback: [RssDemo open]).
  panel add: (Button content: 'Video Store' callback: [VideoStore open]).
  panel add: (Button content: 'Yahoo Local Search' callback: [YahooLocalSearch open]).
  panel add: (Button content: 'Yahoo News Search' callback: [YahooNewsSearch open]).
  w := self topWindowClass new.
  w width: 150; height: 130; title: 'Demos'; add: panel; open
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Demos subclass: #RssDemo
instanceVariableNames: 'channel data document rss titlesLb descTb urlTb window'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
buildBottomPanel
  | button  textbox |
  button := Button new.
  button content: 'Load'.
  button addEvent: 'Click' action: [self loadFeed].
  urlTb := TextBox new.
  urlTb acceptsReturn: false.
  urlTb text: 'http://rss.cnn.com/rss/edition.rss'.
  dockpanel := DockPanel new.
  dockpanel add: button.
  DockPanel setDock: button to: Dock right.
  dockpanel add: urlTb .
  ^ dockpanel
!
buildTopPanel
  titlesLb := ListBox new.
  titlesLb background: (SolidColorBrush new: Color customBlue).
  titlesLb addEvent: 'SelectionChanged' action: [self selectedTitle].
  descTb := TextBox new.
  descTb background: (SolidColorBrush new: Color customGreen).
  descTb textwrapping: TextWrapping wrapWithOverflow.
  descTb horizontalScrollbarVisibility: 'Disabled'.
  ^ Grid hsplit: 0.45 child1: titlesLb child2: descTb
!
findChannel
  | children |
  children := XmlUtility childNodes: rss.
  children do:[:x | x name == 'channel' ifTrue: [channel := x]]
!
findRss
  | children |
  children := XmlUtility childNodes: document.
  children do:[:x | rss := x]
!
loadFeed
  | url |
  data := List new.
  titlesLb clear.
  descTb clear.
  url := urlTb text trim.
  url length == 0 ifTrue: [^nil].
  Internet readTextFromUrl: url callback: [:t | self loadFeed2: t]
!
loadFeed2: aString
  | children reader stringreader |
  stringreader := StringReader onString: aString.
  reader := XmlTextReader fromTextReader: stringreader.
  document := XmlDocument new.
  document load: reader.
  self findRss.
  rss isNil ifTrue: [^nil].
  self findChannel.
  channel isNil ifTrue:[^nil].
  self refreshHeading; refreshItems
!
open
  | bottom dockpanel top |
  window := self topWindowClass new.
  window title: 'Rss Demo'.
  dockpanel := DockPanel new.
  bottom := self buildBottomPanel.
  dockpanel add: bottom.
  DockPanel setDock: bottom to: Dock bottom.
  top := self buildTopPanel.
  dockpanel add: top.
  window add: dockpanel.
  window open
!
refreshHeading
  | title language link description |
  title := (channel at: 'title') innerText.
  language := (channel at: 'language') innerText.
  link := (channel at: 'link') innerText.
  description := (channel at: 'description') innerText.
  window title: title
!
refreshItem: anItem
  | title description li link |
  title := (anItem at: 'title') innerText.
  description := (anItem at: 'description') innerText.
  link := (anItem at: 'link') innerText.
  data add: (Array with: title with: description with: link).
  li := ListBoxItem new.
  li content: title.
  titlesLb items add: li
!
refreshItems
  | children |
  children := XmlUtility childNodes: channel.
  children do: [:x | x name == 'item' ifTrue: [self refreshItem: x]]
!
selectedTitle
  | desc index |
  index := titlesLb selectedIndex.
  desc := (data at: index + 1) at: 2.
  descTb text: desc
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Demos subclass: #VideoStore
instanceVariableNames: 'clientsLb rentalsLb textbox url'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
buildBottomPanel
  textbox := TextBox new.
  ^ textbox
!
buildTopPanel
  clientsLb := ListBox new.
  clientsLb background: (SolidColorBrush new: Color customGreen).
  clientsLb foreground: Brushes darkGreen.
  clientsLb addEvent: 'SelectionChanged' action: [self clientSelection].
  rentalsLb := ListBox new.
  rentalsLb background: (SolidColorBrush new: Color customBlue).
  rentalsLb foreground: Brushes darkBlue.
  rentalsLb addEvent: 'SelectionChanged' action: [self rentalSelection].
  ^ Grid hsplit: 0.45
         child1: clientsLb
         child2: rentalsLb
!
clientDetail: aString
  | lines |
  lines := aString splitStr: ':'.
  textbox clear.
  lines do: [:line | textbox println: line]
!
clientRentals: aList
  | li |
  rentalsLb clear.
  aList do: [:item |
    li := ListBoxItem new.
    li content: (item at: 1).
    li tag: item.
    rentalsLb items add: li]
!
clientSelection
  | id php |
  clientsLb selectedItem isNil ifTrue:[^nil].
  id := clientsLb selectedItem tag.
  php := PhpConnect new.
  php url: url.
  php callback: [:detail | self clientDetail: detail].
  php serialized: true.
  php queryServer: 'cmd=clientdetail&id=' + id.
  php := PhpConnect new.
  php url: url.
  php callback: [:rentals | self clientRentals: rentals].
  php serialized: true.
  php queryServer: 'cmd=clientrentals&id=' + id
!
open
  | w |
  url := 'http://vistascript.net/vistascript/webserv/videostore.php'.
  w := self topWindowClass new.
  w title: 'Video Store Demo'.
  w add: (Grid vsplit: 0.60
               child1: self buildTopPanel
               child2: self buildBottomPanel).
  w open.
  self refreshClientList
!
refreshClientList
  | php |
  php := PhpConnect new.
  php url: url.
  php callback: [:list | self refreshClientList: list].
  php serialized: true.
  php queryServer: 'cmd=clientlist'
!
refreshClientList: aList
  | li |
  clientsLb clear.
  aList do: [:item |
    li := ListBoxItem new.
    li content: (item at: 1).
    li tag: (item at: 2).
    clientsLb items add: li]
!
rentalSelection
  | rental |
  rentalsLb selectedItem isNil ifTrue:[^nil].
  rental := rentalsLb selectedItem tag.
  textbox clear.
  rental do:[:item | textbox println: item]
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Demos subclass: #YahooLocalSearch
instanceVariableNames: 'resultsLb resultTb searchTb url zipCb'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
buildBottomPanel
  | button combobox dockpanel1 dockpanel2 |
  button := Button new.
  button content: 'Search'.
  button addEvent: 'Click' action: [self submitQuery].
  searchTb := TextBox new.
  searchTb acceptsReturn: false.
  zipCb := ComboBox new.
  zipCb width: 100.
  zipCb add: '10001'; add: '60001'; add: '90001'.
  zipCb selectedIndex: 0.
  dockpanel1 := DockPanel new.
  dockpanel1 add: zipCb.
  DockPanel setDock: zipCb to: Dock left.
  dockpanel1 add: searchTb.
  dockpanel2 := DockPanel new.
  dockpanel2 add: button.
  DockPanel setDock: button to: Dock right.
  dockpanel2 add: dockpanel1 .
  ^ dockpanel2
!
buildTopPanel
  resultsLb := ListBox new.
  resultsLb background: (SolidColorBrush new: Color customBlue).
  resultTb := TextBox new.
  resultsLb addEvent: 'SelectionChanged' action: [self selectedResult].
  resultTb background: (SolidColorBrush new: Color customGreen).
  resultTb textwrapping: TextWrapping wrapWithOverflow.
  resultTb horizontalScrollbarVisibility: 'Disabled'.
  ^ Grid hsplit: 0.45 child1: resultsLb child2: resultTb
!
noResult
  resultTb text: 'No results'
!
open
  | bottom dockpanel top w |
  url := 'http://api.local.yahoo.com/LocalSearchService/V3/localSearch'.
  w := self topWindowClass new.
  w title: 'Yahoo Local Search'.
  dockpanel := DockPanel new.
  bottom := self buildBottomPanel.
  dockpanel add: bottom.
  DockPanel setDock: bottom to: Dock bottom.
  top := self buildTopPanel.
  dockpanel add: top.
  w add: dockpanel.
  w open
!
selectedResult
  | item |
  item := resultsLb selectedItem.
  item isNil ifTrue: [^nil].
  resultTb text: item tag printString
!
showResults: aResultDictionary
  | li result resultset goodlist |
  aResultDictionary isDictionary ifFalse:[^self noResult].
  resultset := aResultDictionary at: 'ResultSet'.
  resultset isDictionary ifFalse: [^self noResult].
  result := resultset at: 'Result'.
  result isArray ifFalse:[^self noResult].
  goodlist := result select:[:v | (v isDictionary and:
                                  [v containsKey: 'Title']) and:
                                  [v containsKey: 'Address']].
  goodlist size == 0 ifTrue: [^self noResult].
  goodlist do:[:item |
    li := ListBoxItem new.
    li content: (item at: 'Title').
    li tag: (item at: 'Address').
    resultsLb items add: li].
!
submitQuery
  | query |
  resultsLb clear.
  resultTb clear.
  query := searchTb text trim.
  query length == 0 ifTrue:[^nil].
  self submitQuery: query
!
submitQuery: aQuery
  | json |
  json := JsonConnect new.
  json url: url.
  json callback: [:r | self showResults: r].
  json queryServer: 'appid=vstsearchnews&output=json&results=20&language=en&zip=' +
                     zipCb selectedValue + '&query=' + aQuery
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Demos subclass: #YahooNewsSearch
instanceVariableNames: 'resultsLb resultTb searchTb url'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
buildBottomPanel
  | button  textbox |
  button := Button new.
  button content: 'Search'.
  button addEvent: 'Click' action: [self submitQuery].
  searchTb := TextBox new.
  searchTb acceptsReturn: false.
  dockpanel := DockPanel new.
  dockpanel add: button.
  DockPanel setDock: button to: Dock right.
  dockpanel add: searchTb .
  ^ dockpanel
!
buildTopPanel
  resultsLb := ListBox new.
  resultsLb background: (SolidColorBrush new: Color customBlue).
  resultTb := TextBox new.
  resultsLb addEvent: 'SelectionChanged' action: [self selectedResult].
  resultTb background: (SolidColorBrush new: Color customGreen).
  resultTb textwrapping: TextWrapping wrapWithOverflow.
  resultTb horizontalScrollbarVisibility: 'Disabled'.
  ^ Grid hsplit: 0.45 child1: resultsLb child2: resultTb
!
noResult
  resultTb text: 'No results'
!
open
  | bottom dockpanel top w |
  url := 'http://api.search.yahoo.com/NewsSearchService/V1/newsSearch'.
  w := self topWindowClass new.
  w title: 'Yahoo News Search'.
  dockpanel := DockPanel new.
  bottom := self buildBottomPanel.
  dockpanel add: bottom.
  DockPanel setDock: bottom to: Dock bottom.
  top := self buildTopPanel.
  dockpanel add: top.
  w add: dockpanel.
  w open
!
selectedResult
  | item |
  item := resultsLb selectedItem.
  item isNil ifTrue: [^nil].
  resultTb text: item tag printString
!
showResults: aResultDictionary
  | li result resultset goodlist |
  aResultDictionary isDictionary ifFalse:[^self noResult].
  resultset := aResultDictionary at: 'ResultSet'.
  resultset isDictionary ifFalse: [^self noResult].
  result := resultset at: 'Result'.
  result isDictionary ifFalse:[^self noResult].
  goodlist := result values select:[:v | (v isDictionary and:
                                         [v containsKey: 'Title']) and:
                                         [v containsKey: 'Summary']].
  goodlist size == 0 ifTrue: [^self noResult].
  goodlist do:[:item |
    li := ListBoxItem new.
    li content: (item at: 'Title').
    li tag: (item at: 'Summary').
    resultsLb items add: li].
!
submitQuery
  | query |
  resultsLb clear.
  resultTb clear.
  query := searchTb text trim.
  query length == 0 ifTrue:[^nil].
  self submitQuery: query
!
submitQuery: aQuery
  | php |
  php := PhpConnect new.
  php url: url.
  php callback: [:r | self showResults: r].
  php serialized: true.
  php queryServer: 'appid=vstsearchnews&output=php&results=50&language=en&query=' + aQuery
!
*-------------------------------------
* Class Declaration
*-------------------------------------
UserApps subclass: #Games
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Games subclass: #GamesLauncher
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
open
  " GamesLauncher open "
  | panel w |
  panel := StackPanel new.
  panel add: (Button content: 'CalmoPuzzle' callback: [CalmoPuzzle open]).
  w := self topWindowClass new.
  w width: 150; height: 62; title: 'Games'; add: panel; open
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Games subclass: #CalmoPuzzle
instanceVariableNames: 'canvas empty size size2 tileArray'
classVariableNames: ''
!
*-------------------------------------
* Class Methods
*-------------------------------------
CalmoPuzzle class
!
copyright
   "   ~  written by: Gal Zsolt & Cathlyn Cosca ( CalmoSoft ) ~ 2006 ~ calmosoft@gmail.com ~   "
!
*-------------------------------------
* Instance Methods
*-------------------------------------
CalmoPuzzle
!
open
  "   Written by Gal Zsolt (CalmoSoft) ~ 2006   "
  "   CalmoPuzzle open   "
  | button dockpanel top window |
  self initVariables.
  window := self topWindowClass new.
  window title: '15 Puzzle'.
  dockpanel := DockPanel new.
  button := (Button content: 'Scramble' callback: [self scramble]).
  dockpanel add: button.
  DockPanel setDock: button to: Dock bottom.
  top := self buildTopPanel.
  dockpanel add: top.
  DockPanel setDock: top to: Dock top.
  canvas := Canvas new.
  canvas background: Brushes gray.
  dockpanel add: canvas.
  window add: dockpanel.
  window open.
  self addTiles.
!
addTiles
  1 to: size2 do:[:nr |
    self createTile: nr.
    self placeTile: nr].
  empty := size2.
  (tileArray at: empty) content: ''.
!
buildTopPanel
  | sp |
  sp := StackPanel horizontal.
  sp add: (Button content: '2x2' callback: [self changeSize: 2]).
  sp add: (Button content: '3x3' callback: [self changeSize: 3]).
  sp add: (Button content: '4x4' callback: [self changeSize: 4]).
  sp add: (Button content: '5x5' callback: [self changeSize: 5]).
  sp add: (Button content: '6x6' callback: [self changeSize: 6]).
  sp add: (Button content: '7x7' callback: [self changeSize: 7]).
  sp add: (Button content: '8x8' callback: [self changeSize: 8]).
  sp add: (Button content: '9x9' callback: [self changeSize: 9]).
  ^ sp
!
canMove: nr
  up := (empty = (nr - size)).
  down := (empty = (nr + size)).
  left := ((empty = (nr - 1)) and: ((nr\\size)~=1)).
  right := ((empty = (nr + 1)) and: ((nr\\size)~=0)).
  move1 := up or: down.
  move2 := move1 or: left.
  move3 := move2 or: right.
  ^move3
!
changeSize: nr
  self deleteTiles.
  size := nr.
  size2 := size*size.
  tileArray := Array new: size2.
  self addTiles.
!
createTile: nr
  tile := (Button content: nr callback: [self tryMove: nr]).
  tile width: canvas actualWidth / size.
  tile height: canvas actualHeight / size.
  tileArray at: nr put: tile.
  canvas add: (tileArray at: nr).
  ^ tile
!
deleteTiles
  canvas clear
!
gameOver
  1 to: (size2 - 1) do: [:nr |
    tileContent := (tileArray at: nr) content.
    (tileContent ~= nr) ifTrue: [^false]].
  ^true
!
initVariables  
  size := 4.
  size2 := size*size.
  tileArray := Array new: size2.
!
placeTile: nr
  | row col x y |
  tile := tileArray at: nr.
  row := ((nr - 1) // size) + 1.
  col := ((nr - 1) \\ size) + 1.
  x := canvas actualWidth * (col - 1) / size.
  y := canvas actualHeight * (row - 1) / size.
  Canvas setLeft: tile to: x.
  Canvas setTop: tile to: y
!
scramble
  | random |
  random := Random new.
  1000 timesRepeat: [
    rand := (random nextMin: 1 max: (size2 + 1)).
    self tryMove: rand]
!
tryMove: nr
  (self canMove: nr) ifTrue: [
  (tileArray at: empty) content: (tileArray at: nr) content.
  (tileArray at: nr) content: ''.
  empty := nr.
  self gameOver ifTrue: [(tileArray at: size2) content: 'Hooray!']]
!
*-------------------------------------
* Class Declaration
*-------------------------------------
UserApps subclass: #Graphics
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Graphics subclass: #GraphicsLauncher
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
open
  " GraphicsLauncher open "
  | panel w |
  panel := StackPanel new.
  panel add: (Button content: 'RotateBlocks' callback: [RotateBlocks open]).
  panel add: (Button content: 'RotateText' callback: [RotateText open]).
  panel add: (Button content: 'ScaleDemo' callback: [ScaleDemo open]).
  panel add: (Button content: 'SketchPad' callback: [SketchPad open]).
  panel add: (Button content: 'WheelDemo' callback: [WheelDemo open]).
  w := self topWindowClass new.
  w width: 150; height: 150; title: 'Graphics'; add: panel; open
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Graphics subclass: #RotateBlocks
instanceVariableNames: 'canvas storyboard'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
open
  "   RotateBlocks open   "
  | btn dp w |
  storyboard := StoryBoard new.
  w := self topWindowClass new.
  w title: 'Rotate Blocks'.
  dp := DockPanel new.
  btn := Button new.
  btn content: 'Start'.
  btn addEvent: 'Click' action: [self startStop].
  dp add: btn.
  DockPanel setDock: btn to: Dock bottom.
  canvas := Canvas new.
  canvas clipToBounds: true.
  NameScope setNameScope: canvas to: NameScope new.
  dp add: canvas.
  w add: dp.
  w width: 250; height: 300; open.
  self addRectangle: 'CenteredRotation' x: 25 y: 25 top: 30.
  self addRectangle: 'CornerRotation' x: 0 y: 0 top: 120
!
addRectangle: aName x: x y: y top: top
  | animation rect1 rect2 transform tname |
  rect1 := Rectangle new.
  rect1 width: 50; height: 50.
  rect1 stroke: Brushes black.
  rect1 strokethickness: 5.
  rect1 opacity: 0.25.
  canvas add: rect1.
  Canvas setTop: rect1 to: top; setLeft: rect1 to: 85.
  rect2 := Rectangle new.
  rect2 width: 50; height: 50.
  rect2 stroke: Brushes black.
  rect2 strokethickness: 5.
  canvas add: rect2.
  Canvas setTop: rect2 to: top; setLeft: rect2 to: 85.
  tname := aName.
  transform := RotateTransform new.
  canvas registerName: tname for: transform.
  transform angle: 0; centerX: x; centerY: y.
  rect2 renderTransform: transform.
  animation := DoubleAnimation from: 0.0 to: 360.0 duration: (Duration new: (TimeSpan fromSeconds: 2.0)).
  StoryBoard setTargetName: animation to: tname.
  StoryBoard setTargetProperty: animation to: (PropertyPath new: RotateTransform angleProperty).
  storyboard children add: animation
!
startStop
   storyboard begin: canvas
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Graphics subclass: #RotateText
instanceVariableNames: 'angle1 angle2 canvas storyboard'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
open
  "   RotateText open   "
  | btn dp w |
  angle1 := 0.0.
  angle2 := 45.0.
  storyboard := StoryBoard new.
  w := self topWindowClass new.
  w title: 'Rotate Text'.
  dp := DockPanel new.
  btn := Button new.
  btn content: 'Start'.
  btn addEvent: 'Click' action: [self startStop].
  dp add: btn.
  DockPanel setDock: btn to: Dock bottom.
  canvas := Canvas new.
  canvas background: Brushes aliceBlue.
  canvas clipToBounds: true.
  NameScope setNameScope: canvas to: NameScope new.
  dp add: canvas.
  w add: dp.
  w width: 450; height: 300; open.
  self addRectangle: 'CenteredRotation' x: 125 y: 75 top: 60.
!
addRectangle: aName x: x y: y top: top
  | animation rect1 rect2 transform tname |
  rect1 := Rectangle new.
  rect1 width: 250; height: 150.
  rect1 stroke: Brushes black.
  rect1 strokethickness: 5.
  rect1 opacity: 0.25.
  canvas add: rect1.
  Canvas setTop: rect1 to: top; setLeft: rect1 to: 75.
  rect2 := TextEditor new.
  rect2 showLines: true.
  rect2 text: 'Hello world'.
  rect2 width: 250; height: 150.
  rect2 background: Brushes ivory.
  canvas add: rect2.
  Canvas setTop: rect2 to: top; setLeft: rect2 to: 75.
  tname := aName.
  transform := RotateTransform new.
  canvas registerName: tname for: transform.
  transform angle: angle1 ; centerX: x; centerY: y.
  rect2 renderTransform: transform.
  animation := DoubleAnimation from: angle1 to: angle2 duration: (Duration new: (TimeSpan fromSeconds: 1.0)).
  StoryBoard setTargetName: animation to: tname.
  StoryBoard setTargetProperty: animation to: (PropertyPath new: RotateTransform angleProperty).
  storyboard children add: animation
!
startStop
   storyboard begin: canvas
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Graphics subclass: #ScaleDemo
instanceVariableNames: 'text'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
buildBottomPanel
  | sp |
  sp := StackPanel horizontal.
  sp add: (Button content: 'Reset' callback: [self reset]).
  sp add: (Button content: 'Scale 1' callback: [self scale1]).
  sp add: (Button content: 'Scale 2' callback: [self scale2]).
  sp add: (Button content: 'Skew 1' callback: [self skew1]).
  sp add: (Button content: 'Skew 2' callback: [self skew2]).
  ^ sp
!
buildTopPanel
  | canvas |
  canvas := Canvas new.
  text := TextBox new.
  text fontsize: 50; textalignment: TextAlignment center; fontweight: FontWeights ultrabold.
  text isenabled: false; foreground: Brushes blue.
  text text: 'SAMPLE'.
  canvas add: text.
  Canvas setLeft: text to: 0.
  Canvas setTop: text to: 0.
  canvas clipToBounds: true.
  ^ canvas
!
open
  | bottom dockpanel window |
  window := self topWindowClass new.
  window title: 'Scale Demo'.
  dockpanel := DockPanel new.
  bottom := self buildBottomPanel.
  dockpanel add: bottom.
  DockPanel setDock: bottom to: Dock bottom.
  dockpanel add: self buildTopPanel.
  window add: dockpanel.
  window open
!
reset
   text renderTransform: Transform identity
!
scale1
   | transform |
   transform := ScaleTransform new.
   transform scaleX: 0.5; scaleY: 2.
   text renderTransform: transform
!
scale2
   | transform |
   transform := ScaleTransform new.
   transform scaleX: 2; scaleY: 0.5.
   text renderTransform: transform
!
skew1
   | transform |
   transform := SkewTransform new.
   transform angleX: 45.
   text renderTransform: transform
!
skew2
   | transform |
   transform := SkewTransform new.
   transform angleY: 30.
   text renderTransform: transform
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Graphics subclass: #SketchPad
instanceVariableNames: 'inkcanvas'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
open
  | w |
  inkcanvas := InkCanvas new.
  w := self topWindowClass new.
  w title: 'SketchPad'.
  w add: inkcanvas.
  inkcanvas background: Brushes yellow.
  " inkcanvas editingMode: InkCanvasEditingMode inkAndGesture."
  w open
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Graphics subclass: #WheelDemo
instanceVariableNames: 'canvas storyboard'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
addAnimation: aWheel center: aPoint radius: aRadius
  | animation transform tname1 tname2 xstart xend |
  tname1 := 'RotateWheel'.
  transform := RotateTransform new.
  canvas registerName: tname1 for: transform.
  transform angle: 0; centerX: aPoint x; centerY: aPoint y.
  aWheel renderTransform: transform.
  animation := DoubleAnimation from: 0.0 to: 360.0 duration: (Duration new: (TimeSpan fromSeconds: 2.0)).
  StoryBoard setTargetName: animation to: tname1.
  StoryBoard setTargetProperty: animation to: (PropertyPath new: RotateTransform angleProperty).
  storyboard children add: animation.
  xstart := aPoint x - aRadius.
  xend := xstart + (Math pi * 2 * aRadius).
  animation := DoubleAnimation from:  xstart to: xend duration: (Duration new: (TimeSpan fromSeconds: 2.0)).
  tname2 := 'MoveWheel'.
  canvas registerName: tname2 for: aWheel.
  StoryBoard setTargetName: animation to: tname2.
  StoryBoard setTargetProperty: animation to: (PropertyPath new: Canvas leftProperty).
  storyboard children add: animation
!
open
  | btn center radius dp w wheel |
  storyboard := StoryBoard new.
  w := self topWindowClass new.
  w title: 'Wheel Demo'.
  dp := DockPanel new.
  btn := Button new.
  btn content: 'Start'.
  btn addEvent: 'Click' action: [self start].
  dp add: btn.
  DockPanel setDock: btn to: Dock bottom.
  canvas := Canvas new.
  canvas clipToBounds: true.
  NameScope setNameScope: canvas to: NameScope new.
  dp add: canvas.
  w add: dp.
  w height: 250; width: 350.
  w open.
  center := Point x: 100 y: 100.
  radius := 50.
  wheel := Wheel createWheel: 6 radius: radius center: center.
  canvas add: wheel.
  self addAnimation: wheel center: center radius: radius
!
start
    storyboard begin: canvas
!
*-------------------------------------
* Class Declaration
*-------------------------------------
UserApps subclass: #Remote
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Remote subclass: #RemoteLauncher
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
open
  " RemoteLauncher open "
  | panel w |
  panel := StackPanel new.
  panel add: (Button content: 'DrawAndChat-A' callback: [DrawAndChat tms: 'A' listenTo: #('B' 'C')]).
  panel add: (Button content: 'DrawAndChat-B' callback: [DrawAndChat tms: 'B' listenTo: #('A' 'C')]).
  panel add: (Button content: 'DrawAndChat-C' callback: [DrawAndChat tms: 'C' listenTo: #('A' 'B')]).
  panel add: (Button content: 'Robot TicTacToe' callback: [RobotTicTacToe open]).
  panel add: (Button content: 'TicTacToe-X' callback: [TmsTicTacToe token1: 'X' token2: 'O']).
  panel add: (Button content: 'TicTacToe-O' callback: [TmsTicTacToe token1: 'O' token2: 'X']).
  panel add: (Button content: 'TmsInspect-1' callback: [TmsInspect me: 1 you: 2]).
  panel add: (Button content: 'TmsInspect-2' callback: [TmsInspect me: 2 you: 1]).
  panel add: (Button content: 'TmsXaml-1' callback: [TmsXaml me: 1 you: 2]).
  panel add: (Button content: 'TmsXaml-2' callback: [TmsXaml me: 2 you: 1]).
  w := self topWindowClass new.
  w width: 150; height: 255; title: 'Remote'; add: panel; open
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Remote subclass: #DrawAndChat
instanceVariableNames: 'connection designer textin textout'
classVariableNames: ''
!
*-------------------------------------
* Class Methods
*-------------------------------------
DrawAndChat class
!
jabber: anId passwd: passwd contact: anId2
  ^ super new jabber: anId passwd: passwd contact: anId2
!
tms: anId listenTo: anArray
  ^ super new tms: anId listenTo: anArray
!
*-------------------------------------
* Instance Methods
*-------------------------------------
DrawAndChat
!
buildChatPanel
  | button dockpanel grid |
  textin := TextBox new.
  textout := TextBox new.
  grid := Grid vsplit: 0.45 child1: textin child2: textout.
  grid background: Brushes gray.
  dockpanel := DockPanel new.
  button := Button new.
  button content: 'Send'.
  button addEvent: 'Click' action: [self sendText].
  dockpanel add: button.
  DockPanel setDock: button to: Dock bottom.
  dockpanel add: grid.
  ^ dockpanel
!
buildDesignerPanel
  | button dockpanel |
  designer := Designer new.
  dockpanel := DockPanel new.
  button := Button new.
  button content: 'Send'.
  button addEvent: 'Click' action: [self sendDrawing].
  dockpanel add: button.
  DockPanel setDock: button to: Dock bottom.
  dockpanel add: designer.
  ^ dockpanel
!
jabber: anId passwd: passwd contact: anId2
  | w |
  connection := JabberConnect new: true.
  connection user: anId; passwd: passwd; contact: anId2.
  connection callback: [:array | self performArray: array].
  connection connect.
  w := self topWindowClass new.
  w title: 'Draw ''N Chat (' + anId + ')'.
  w add: (Grid hsplit: 0.45
               child1: self buildDesignerPanel
               child2: self buildChatPanel).
  w open
!
printText: aMessage
  textin println: aMessage
!
sendMessage: anArray
  connection send: anArray
!
sendText
  self sendMessage: (Array with: 'printText:' with: textout text trim).
  textout text: ''
!
sendDrawing
  self sendMessage: (Array with: 'setLocalPackage:' with: designer view packager package)
!
setLocalPackage: anObject
  designer view packager package: anObject
!
tms: myId listenTo: anArray
  | listener w |
  connection := TmsConnect channel: 'test' name: 'drawandchat' + myId.
  anArray do: [:theirId |
    listener := TmsConnect channel: 'test' name: 'drawandchat' + theirId
                        callback: [:array | self performArray: array].
    listener startPolling: 1].
  w := self topWindowClass new.
  w title: 'Draw ''N Chat (' + myId + ')'.
  w add: (Grid hsplit: 0.45
               child1: self buildDesignerPanel
               child2: self buildChatPanel).
  w open
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Remote subclass: #RobotTicTacToe
instanceVariableNames: 'canvas textin json tileArray token'
classVariableNames: ''
!
*-------------------------------------
* Class Methods
*-------------------------------------
RobotTicTacToe class
!
open
  ^ super new open
!
*-------------------------------------
* Instance Methods
*-------------------------------------
RobotTicTacToe
!
buildBoardPanel
  | dockpanel tile block |
  tileArray := Array new: 9.
  canvas := Canvas new.
  1 to: 9 do: [:i |
    tile := self createButton: i with: token.
    tileArray at: i put: tile.
    canvas add: tile].
  canvas clipToBounds: true.
  canvas addEvent: 'SizeChanged' action: [self placeTiles].
  dockpanel := DockPanel new.
  dockpanel add: canvas.
  ^ dockpanel
!
buildChatPanel
  | button dockpanel |
  textin := TextBox new.
  dockpanel := DockPanel new.
  button := Button new.
  button content: 'Restart'.
  button addEvent: 'Click' action: [self restart].
  dockpanel add: button.
  DockPanel setDock: button to: Dock bottom.
  dockpanel add: textin.
  ^ dockpanel
!
createButton: anIndex with: aString
   | tile |
   tile := Button new.
   tile content: ''.
   tile addEvent: 'Click' action: [self markTiles: anIndex with: aString].
   ^ tile
!
markLocalTile: index with: aString
  | tile |
  tile := tileArray at: index.
  tile content: aString
!
markTiles: index with: aString
  self markLocalTile: index with: aString.
  self sendMessageToRobot
!
performRobotMessages: messages
  messages do:[:msg | self performArray: msg]
!
placeTiles
  | tile row col x y size |
  1 to: 9 do:[:nr | 
    size := 3.
    tile := tileArray at: nr.
    tile width: canvas actualWidth / size .
    tile height: canvas actualHeight / size .
    row := ((nr - 1) // size) + 1.
    col := ((nr - 1) \\ size) + 1.
    x := canvas actualWidth * (col - 1) / size .
    y := canvas actualHeight * (row - 1) / size .
    Canvas setLeft: tile to: x.
    Canvas setTop: tile to: y]
!
printMessage: aMessage
  textin println: aMessage
!
sendMessageToRobot
  | board |
  board := (tileArray collect: [:x | x content]) asArray.
  json sendData: (Array with: 'O' with: board)
!
open
  | w |
  token := 'X'.
  json := JsonConnect new.
  json url: 'http://vistascript.net/vistascript/webserv/t3bot.php'.
  json callback: [:response | self performRobotMessages: (response at: 'data')].
  w := self topWindowClass new.
  w title: 'Robot Tic-Tac-Toe'.
  w add: (Grid hsplit: 0.45
               child1: self buildBoardPanel
               child2: self buildChatPanel).
  w open
!
restart
  tileArray do:[:tile | tile content: ''].
  textin text: ''
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Remote subclass: #TicTacToe
instanceVariableNames: 'canvas textin textout jabber user contact tileArray token'
classVariableNames: ''
!
*-------------------------------------
* Class Methods
*-------------------------------------
TicTacToe class
!
gtalk: aUser passwd: aPasswd contact: aContact token: anXorO
  ^ super new gtalk: true user: aUser passwd: aPasswd contact: aContact token: anXorO
!
vst: aUser passwd: aPasswd contact: aContact token: anXorO
  ^ super new gtalk: false user: aUser passwd: aPasswd contact: aContact token: anXorO
!
*-------------------------------------
* Instance Methods
*-------------------------------------
TicTacToe
!
buildBoardPanel
  | dockpanel tile block |
  tileArray := Array new: 9.
  canvas := Canvas new.
  1 to: 9 do: [:i |
    tile := self createButton: i with: token.
    tileArray at: i put: tile.
    canvas add: tile].
  canvas clipToBounds: true.
  canvas addEvent: 'SizeChanged' action: [self placeTiles].
  dockpanel := DockPanel new.
  dockpanel add: canvas.
  ^ dockpanel
!
buildChatPanel
  | button dockpanel grid |
  textin := TextBox new.
  textout := TextBox new.
  grid := Grid vsplit: 0.45 child1: textin child2: textout.
  grid background: Brushes gray.
  dockpanel := DockPanel new.
  button := Button new.
  button content: 'Send'.
  button addEvent: 'Click' action: [self sendText].
  dockpanel add: button.
  DockPanel setDock: button to: Dock bottom.
  dockpanel add: grid.
  ^ dockpanel
!
createButton: anIndex with: aString
   | tile |
   tile := Button new.
   tile addEvent: 'Click' action: [self markTiles: anIndex with: aString].
   ^ tile
!
login: isGtalk with: aPasswd
  jabber := JabberConnect new: isGtalk.
  jabber user: user; passwd: aPasswd.
  jabber callback: [:array | self performArray: array].
  jabber connect
!
markLocalTile: index with: aString
  | tile |
  tile := tileArray at: index.
  tile content: aString
!
markRemoteTile: index with: aString
  self sendMessage: (Array with: 'markLocalTile:with:' with: index with: aString)
!
markTiles: index with: aString
  self markLocalTile: index with: aString.
  self markRemoteTile: index with: aString
!
placeTiles
  | tile row col x y size |
  1 to: 9 do:[:nr | 
    size := 3.
    tile := tileArray at: nr.
    tile width: canvas actualWidth / size .
    tile height: canvas actualHeight / size .
    row := ((nr - 1) // size) + 1.
    col := ((nr - 1) \\ size) + 1.
    x := canvas actualWidth * (col - 1) / size .
    y := canvas actualHeight * (row - 1) / size .
    Canvas setLeft: tile to: x.
    Canvas setTop: tile to: y]
!
printMessage: aMessage
  textin println: aMessage
!
sendText
  self sendMessage: (Array with: 'printMessage:' with: textout text trim).
  textout text: ''
!
sendMessage: anArray
  jabber send: contact object: anArray
!
gtalk: isGtalk user: aUser passwd: aPasswd contact: aContact token: anXorO
  | w |
  Proxy := self.
  user := aUser.
  contact := aContact.
  token := anXorO.
  self login: isGtalk with: aPasswd.
  w := self topWindowClass new.
  w title: 'Tic-Tac-Toe (' + user + ')'.
  w add: (Grid hsplit: 0.45
               child1: self buildBoardPanel
               child2: self buildChatPanel).
  w open
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Remote subclass: #TmsTicTacToe
instanceVariableNames: 'canvas textin textout tmsin tmsout tileArray token'
classVariableNames: ''
!
*-------------------------------------
* Class Methods
*-------------------------------------
TmsTicTacToe class
!
token1: mytoken token2: theirtoken
  ^ super new token1: mytoken token2: theirtoken
!
*-------------------------------------
* Instance Methods
*-------------------------------------
TmsTicTacToe
!
buildBoardPanel
  | dockpanel tile block |
  tileArray := Array new: 9.
  canvas := Canvas new.
  1 to: 9 do: [:i |
    tile := self createButton: i with: token.
    tileArray at: i put: tile.
    canvas add: tile].
  canvas clipToBounds: true.
  canvas addEvent: 'SizeChanged' action: [self placeTiles].
  dockpanel := DockPanel new.
  dockpanel add: canvas.
  ^ dockpanel
!
buildChatPanel
  | button dockpanel grid |
  textin := TextBox new.
  textout := TextBox new.
  grid := Grid vsplit: 0.45 child1: textin child2: textout.
  grid background: Brushes gray.
  dockpanel := DockPanel new.
  button := Button new.
  button content: 'Send'.
  button addEvent: 'Click' action: [self sendText].
  dockpanel add: button.
  DockPanel setDock: button to: Dock bottom.
  dockpanel add: grid.
  ^ dockpanel
!
createButton: anIndex with: aString
   | tile |
   tile := Button new.
   tile addEvent: 'Click' action: [self markTiles: anIndex with: aString].
   ^ tile
!
markLocalTile: index with: aString
  | tile |
  tile := tileArray at: index.
  tile content: aString
!
markRemoteTile: index with: aString
  self sendMessage: (Array with: 'markLocalTile:with:' with: index with: aString)
!
markTiles: index with: aString
  self markLocalTile: index with: aString.
  self markRemoteTile: index with: aString
!
placeTiles
  | tile row col x y size |
  1 to: 9 do:[:nr | 
    size := 3.
    tile := tileArray at: nr.
    tile width: canvas actualWidth / size .
    tile height: canvas actualHeight / size .
    row := ((nr - 1) // size) + 1.
    col := ((nr - 1) \\ size) + 1.
    x := canvas actualWidth * (col - 1) / size .
    y := canvas actualHeight * (row - 1) / size .
    Canvas setLeft: tile to: x.
    Canvas setTop: tile to: y]
!
printMessage: aMessage
  textin println: aMessage
!
sendMessage: anArray
  tmsout send: anArray
!
sendText
  self sendMessage: (Array with: 'printMessage:' with: textout text trim).
  textout text: ''
!
token1: mytoken token2: theirtoken
  | w |
  token := mytoken.
  tmsout := TmsConnect channel: 'test' name: 'tictactoe' + mytoken.
  tmsin := TmsConnect channel: 'test' name: 'tictactoe' + theirtoken
                      callback: [:array | self performArray: array].
  tmsin startPolling: 1.
  w := self topWindowClass new.
  w title: 'Tic-Tac-Toe (' + token + ')'.
  w add: (Grid hsplit: 0.45
               child1: self buildBoardPanel
               child2: self buildChatPanel).
  w open
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Remote subclass: #TmsInspect
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Class Methods
*-------------------------------------
TmsInspect class
!
me: tag1 you: tag2
  TmsOut := TmsConnect channel: 'test' name: 'tmsinspect-' + tag2.
  TmsIn := TmsConnect channel: 'test' name: 'tmsinspect-' + tag1 callback:[:x | x inspect].
  TmsIn startPolling: 1.
  Transcript showCr: 'TmsInspect for: ' + tag1
!
stop
  TmsIn stopPolling
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Remote subclass: #TmsXaml
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Class Methods
*-------------------------------------
TmsXaml class
!
me: tag1 you: tag2
  TmsOut := TmsConnect channel: 'test' name: 'tmsxaml-' + tag2.
  TmsIn := TmsConnect channel: 'test' name: 'tmsxaml-' + tag1 callback:[:string | self window: string].
  TmsIn startPolling: 1.
  Transcript showCr: 'TmsXaml for: ' + tag1
!
stop
  TmsIn stopPolling
!
window: aString
  | w x |
  w := self topWindowClass new.
  x := XamlUtility xamlFromString: aString decodeBase64.
  x isNil ifFalse: [w add: x].
  w width: 150; height: 150; title: 'Xaml Object'.
  w open
!
*-------------------------------------
* Class Declaration
*-------------------------------------
UserApps subclass: #Xaml
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Xaml subclass: #XamlLauncher
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
open
  " XamlLauncher open "
  | panel w |
  panel := StackPanel new.
  panel add: (Button content: 'ColorButtons' callback: [ColorButtons open]).
  panel add: (Button content: 'CombinedXaml' callback: [CombinedXaml open]).
  panel add: (Button content: 'Greetings' callback: [Greetings open]).
  panel add: (Button content: 'List2Text' callback: [List2Text open]).
  panel add: (Button content: 'VideoStore2' callback: [VideoStore2 open]).
  panel add: (Button content: 'XamlDemo' callback: [XamlDemo open]).
  w := self topWindowClass new.
  w width: 150; height: 170; title: 'Xaml'; add: panel; open
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Xaml subclass: #ColorButtons
instanceVariableNames: 'ellipse'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
open
  self readWiki: 'libraries:xaml:tests:colorbuttons.txt'
        replace: '$$Label$$@Color Buttons:$$Btn01$$@Red:$$Btn02$$@Green:$$Btn03$$@Yellow'
!
greenClick
  ellipse fill: Brushes green
!
onWikiResponse: aString
  | btn w x |
  w := self topWindowClass new.
  x := XamlUtility xamlFromString: aString.
  x isNil ifFalse: [w add: x].
  ellipse := x findName: 'Ellipse'.
  btn := x findName: 'Btn01'.
  btn addEvent: 'Click' action: [self redClick].
  btn := x findName: 'Btn02'.
  btn addEvent: 'Click' action: [self greenClick].
  btn := x findName: 'Btn03'.
  btn addEvent: 'Click' action: [self yellowClick].
  w width: 450; height: 250; title: 'Color Buttons'.
  w open
!
redClick
  ellipse fill: Brushes red
!
yellowClick
  ellipse fill: Brushes yellow
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Xaml subclass: #CombinedXaml
instanceVariableNames: 'border canvas grid'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
addWheel: aRadius x: x
  | center wheel |
  center := Point x: x y: 225 - aRadius.
  wheel := Wheel createWheel: 14 radius: aRadius center: center.
  canvas add: wheel
!
open
  self readWiki: 'libraries:xaml:containers:redcontainer.txt'
        replace: '$$Label$$@Combined Xaml'
       callback: [:s | self onContainerResponse: s]
!
onCanvasResponse: aString
  | x |
  x := XamlUtility xamlFromString: aString.
  canvas := x findName: 'Canvas'.
  x children clear.
  border child: canvas.
  self addWheel: 100 x: 150;
       addWheel:  75 x: 325;
       addWheel:  50 x: 450;
       show
!
onContainerResponse: aString
  grid := XamlUtility xamlFromString: aString.
  border := grid findName: 'Border'.
  self readWiki: 'libraries:xaml:canvases:colorfill.txt'
       callback: [:s | self onCanvasResponse: s]
!
show
  | w |
  w := self topWindowClass new.
  grid isNil ifFalse: [w add: grid].
  w width: 550; height: 350; title: 'Combined Xaml'.
  w open
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Xaml subclass: #Greetings
instanceVariableNames: 'window'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
open
  | panel |
  panel := StackPanel new.
  panel add: (Button content: 'English' callback: [self readWiki: 'libraries:xaml:tests:greetings:english.txt']).
  panel add: (Button content: 'French' callback: [self readWiki: 'libraries:xaml:tests:greetings:french.txt']).
  panel add: (Button content: 'German' callback: [self readWiki: 'libraries:xaml:tests:greetings:german.txt']).
  panel add: (Button content: 'Hungarian' callback: [self readWiki: 'libraries:xaml:tests:greetings:hungarian.txt']).
  panel add: (Button content: 'Italian' callback: [self readWiki: 'libraries:xaml:tests:greetings:italian.txt']).
  panel add: (Button content: 'Spanish' callback: [self readWiki: 'libraries:xaml:tests:greetings:spanish.txt']).
  window := self topWindowClass new.
  window width: 150; height: 170; title: 'Languages Demo'; add: panel; open
!
onWikiResponse: aString
  | x |
  x := XamlUtility xamlFromString: aString.
  x isNil ifFalse: [window add: x].
  window width: 650; height: 260
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Xaml subclass: #List2Text
instanceVariableNames: 'border grid inner'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
open
  self readWiki: 'libraries:xaml:containers:redcontainer.txt'
        replace: '$$Label$$@List2Text'
       callback: [:s | self onContainerResponse: s]
!
onGridResponse: aString
  | x |
  x := XamlUtility xamlFromString: aString.
  inner := x findName: 'InnerGrid'.
  x children clear.
  border child: inner.
  self show
!
onContainerResponse: aString
  grid := XamlUtility xamlFromString: aString.
  border := grid findName: 'Border'.
  self readWiki: 'libraries:xaml:combinations:list2text.txt'
       callback: [:s | self onGridResponse: s]
!
show
  | w |
  w := self topWindowClass new.
  grid isNil ifFalse: [w add: grid].
  w width: 550; height: 350; title: 'List2Text Xaml'.
  w open
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Xaml subclass: #VideoStore2
instanceVariableNames: 'border clientsLb grid inner rentalsLb textbox url'
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
open
  url := 'http://vistascript.net/vistascript/webserv/videostore.php'.
  self readWiki: 'libraries:xaml:containers:redcontainer.txt'
        replace: '$$Label$$@Video Store'
       callback: [:s | self onContainerResponse: s]
!
clientDetail: aString
  | lines |
  lines := aString splitStr: ':'.
  textbox clear.
  lines do: [:line | textbox println: line]
!
clientRentals: aList
  | li |
  rentalsLb clear.
  aList do: [:item |
    li := ListBoxItem new.
    li content: (item at: 1).
    li tag: item.
    rentalsLb items add: li]
!
clientSelection
  | id php |
  clientsLb selectedItem isNil ifTrue:[^nil].
  id := clientsLb selectedItem tag.
  php := PhpConnect new.
  php url: url.
  php callback: [:detail | self clientDetail: detail].
  php serialized: true.
  php queryServer: 'cmd=clientdetail&id=' + id.
  php := PhpConnect new.
  php url: url.
  php callback: [:rentals | self clientRentals: rentals].
  php serialized: true.
  php queryServer: 'cmd=clientrentals&id=' + id
!
onContainerResponse: aString
  grid := XamlUtility xamlFromString: aString.
  border := grid findName: 'Border'.
  self readWiki: 'libraries:xaml:combinations:list2text.txt'
       callback: [:s | self onInnerResponse: s]
!
onInnerResponse: aString
  | top |
  top := XamlUtility xamlFromString: aString.
  inner := top findName: 'InnerGrid'.
  clientsLb := inner findName: 'ListBox01'.
  clientsLb background: (SolidColorBrush new: Color customGreen); foreground: Brushes darkGreen.
  clientsLb addEvent: 'SelectionChanged' action: [self clientSelection].
  rentalsLb := inner findName: 'ListBox02'.
  rentalsLb background: (SolidColorBrush new: Color customBlue); foreground: Brushes darkBlue.
  rentalsLb addEvent: 'SelectionChanged' action: [self rentalSelection].
  textbox := inner findName: 'TextBox01'.
  textbox background: Brushes ivory; foreground: Brushes navy.
  top children clear.
  border child: inner.
  self show
!
refreshClientList
  | php |
  php := PhpConnect new.
  php url: url.
  php callback: [:list | self refreshClientList: list].
  php serialized: true.
  php queryServer: 'cmd=clientlist'
!
refreshClientList: aList
  | li |
  clientsLb clear.
  aList do: [:item |
    li := ListBoxItem new.
    li content: (item at: 1).
    li tag: (item at: 2).
    clientsLb items add: li]
!
rentalSelection
  | rental |
  rentalsLb selectedItem isNil ifTrue:[^nil].
  rental := rentalsLb selectedItem tag.
  textbox clear.
  rental do:[:item | textbox println: item]
!
show
  | w |
  w := self topWindowClass new.
  grid isNil ifFalse: [w add: grid].
  w width: 550; height: 350; title: 'Video Store'.
  w open.
  self refreshClientList
!
*-------------------------------------
* Class Declaration
*-------------------------------------
Xaml subclass: #XamlDemo
instanceVariableNames: ''
classVariableNames: ''
!
*-------------------------------------
* Instance Methods
*-------------------------------------
open
  self readWiki: 'libraries:xaml:tests:test1.txt'.
!
onWikiResponse: aString
  | w x |
  w := self topWindowClass new.
  x := XamlUtility xamlFromString: aString.
  x isNil ifFalse: [w add: x].
  w width: 550; height: 500; title: 'Xaml Demo'.
  w open
!
*-------------------------------------
* Load sub classes
*-------------------------------------
*-------------------------------------
* Class Declaration
*-------------------------------------
Tools subclass: #Debugger
instanceVariableNames: 'process sourceTb stackLb'
classVariableNames: ''
!
*-------------------------------------
* Class Methods
*-------------------------------------
Debugger class
!
openOnCurrentProcess
  | process |
  process := Processor currentProcess.
  [self openOn: process] run.
  process halt
!
openOn: aProcess
  | w |
  w := super new.
  w openOn: aProcess.
  ^ w
!
Debugger
!
*-------------------------------------
* Instance Methods
*-------------------------------------
buildBottomPanel
  | buttonpanel dockpanel listpanel |
  buttonpanel := self buildButtonPanel.
  listpanel := self buildListPanel.
  sourceTb := TextEditor new.
  dockpanel := DockPanel new.
  dockpanel add: buttonpanel.
  DockPanel setDock: buttonpanel to: Dock top.
  dockpanel add: (Grid vsplit: 0.65
               child1: sourceTb 
               child2: listpanel).
  ^ dockpanel
!
buildButton: aName
  | btn |
  btn := Button new.
  btn content: aName.
  ^ btn
!
buildButtonPanel
  | border stackpanel |
  border := Border new.
  border background: Brushes lightGray.
  border borderBrush: Brushes black.
  border borderThickness: (Thickness left: 0 top: 0 right: 0 bottom: 1).
  stackpanel := StackPanel new.
  stackpanel orientation: Orientation horizontal.
  stackpanel
    add: (self buildButton: 'Proceed');
    add: (self buildButton: 'Restart');
    add: (self buildButton: 'Into');
    add: (self buildButton: 'Over');
    add: (self buildButton: 'Through').
  border child: stackpanel.
  ^ border
!
buildListPanel
  | brush list1 list2 list3 list4 |
  brush := SolidColorBrush new: Color customGreen.
  list1 := ListBox new.
  list1 background: brush.
  list2 := ListBox new.
  list2 background: brush.
  list3 := ListBox new.
  list3 background: brush.
  list4 := ListBox new.
  list4 background: brush.
  ^ Grid hsplit: Brushes gray child1: list1 child2: list2 child3: list3 child4: list4
!
buildTopPanel
  stackLb := ListBox new.
  stackLb background: (SolidColorBrush new: Color customGreen).
  stackLb addEvent: 'SelectionChanged' action: [self stackSelection].
  ^ stackLb
!
open
  | w |
  w := self topWindowClass new.
  w title: 'Debugger'.
  w add: (Grid vsplit: 0.25
               child1: self buildTopPanel
               child2: self buildBottomPanel).
  w width: 500; height: 450; open
!
openOn: aProcess
  | w |
  process := aProcess.
  w := self topWindowClass new.
  w title: 'Debugger'.
  w add: (Grid vsplit: 0.25
               child1: self buildTopPanel
               child2: self buildBottomPanel).
  w open.
  self showStack
!
showStack
  | li stack |
  stackLb clear.
  stack := process callstack.
  stack do:[:context |
    li := ListBoxItem new.
    li content: context.
    li tag: context.
    stackLb items add: li]
!
stackSelection
  | item |
  item := stackLb selectedItem.
  item isNil ifTrue: [^nil].
  sourceTb text: item tag method text
!
$eval Transcript showCr: 'loadall.st finished'
$eval Vm start
