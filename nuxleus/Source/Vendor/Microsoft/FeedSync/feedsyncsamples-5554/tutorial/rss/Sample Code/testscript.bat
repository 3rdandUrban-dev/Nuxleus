@rem This script runs a series of tests simulating two endpoints modifying and
@rem merging RSS files.  The tests cover the Create/Update/Move/Delete operations,
@rem in conflict and non-conflict updates.

@rem Copyright (c) 2006 Microsoft Corporation.  All Rights Reserved.

@rem Inputs:  
@rem	any RSS file which is the "base" starting point for the modifications.
@rem Output:
@rem 	A series of output files are produced, showing each endpoint's operation, and the 
@rem 	resulting merge.   Also, a fullmerge.log is creating showing the operations completed
@rem 	by the merge processor.

@rem turn output off
@echo off

@rem make sure that a file exists, if not exit
if "%1%"=="" goto DISPLAY_USAGE

@rem create output directory if it does not exist
if not exist "rss" md "rss"

@rem turn output on
@echo on

@echo *** Generating Unique ID's ***
@rem generate unique ID's from the master RSS file
cscript /nologo fsrssconvert.js %1% > rss\v1.xml


@echo.
@echo *** test 1: create new nodes, no conflict ***

@rem endpoint 1, create nodes
cscript /nologo fsrsscreate.js rss\v1.xml ep1.100 "Buy groceries" "Get milk and eggs" EP1 > rss\ep1_v1a.xml
cscript /nologo fsrsscreate.js rss\ep1_v1a.xml ep1.101 "Get car serviced" "Needs oil change" EP1 > rss\ep1_v1b.xml

@rem endpoint 2: create nodes
cscript /nologo fsrsscreate.js rss\v1.xml ep2.100 "Return books to library" "2 books in total" EP2 > rss\ep2_v1a.xml
cscript /nologo fsrsscreate.js rss\ep2_v1a.xml ep2.101 "Get birthday gift for mother" "She likes blue flowers" EP2 > rss\ep2_v1b.xml

@rem merge, creating V2
cscript /nologo fsrssmerge.js rss\ep1_v1b.xml rss\ep2_v1b.xml > rss\v2.xml


@echo.
@echo *** test 2: update with no conflict ***

@rem update endpoint 1 node
cscript /nologo fsrssupdate.js rss\v2.xml ep1.100 "Buy Groceries" "Get milk, eggs and butter" EP1 > rss\ep1_v2a.xml

@rem update endpoint 2 node
cscript /nologo fsrssupdate.js rss\ep1_v2a.xml ep1.100 "Buy Groceries" "Get milk, eggs, butter and bread" EP2 > rss\v3.xml


@echo.
@echo *** test 3: update with conflict ***

@rem endpoint 1 modification
cscript /nologo fsrssupdate.js rss\v3.xml ep1.100 "Buy Groceries" "Get milk, eggs, butter and rolls" EP1 > rss\ep1_v3a.xml

@rem insert pause due to time granularity, to ensure known outcome of endpoint 2 winning.
@rem otherwise with a gmt format granularity of only seconds, the update will be on the 
@rem identical time tick.
@echo Inserting time delay for modification 
@pause 
cscript /nologo fsrssupdate.js rss\v3.xml ep1.100 "Buy Groceries - DONE" "Get milk, eggs, butter and bread" EP2 > rss\ep2_v3a.xml

@rem merge
cscript /nologo fsrssmerge.js rss\ep1_v3a.xml rss\ep2_v3a.xml > rss\v4.xml


@echo.
@echo *** test 4: resolve conflict

@rem endpoint 2 resolve
cscript /nologo fsrssupdate.js rss\v4.xml ep1.100 "Buy Groceries - DONE" "Get milk, eggs, butter and bread" EP2 true > rss\v5.xml


@echo.
@echo *** test 5: delete without conflict

@rem do independent deletes
cscript /nologo fsrssdelete.js rss\v5.xml ep1.101 EP1 > rss\ep1_v5a.xml
cscript /nologo fsrssdelete.js rss\v5.xml ep2.101 EP2 > rss\ep2_v5a.xml

@rem merge
cscript /nologo fsrssmerge.js rss\ep1_v5a.xml rss\ep2_v5a.xml > rss\v6.xml


@echo.
@echo *** test 6: delete with conflict - both delete same item.  Semantically not a conflict, but by replication rules, this is a conflict.
cscript /nologo fsrssdelete.js rss\v6.xml ep2.100 EP1 > rss\ep1_v6a.xml

@echo Inserting time delay for modification 
@pause 
cscript /nologo fsrssdelete.js rss\v6.xml ep2.100 EP2 > rss\ep2_v6a.xml

@rem merge
cscript /nologo fsrssmerge.js rss\ep1_v6a.xml rss\ep2_v6a.xml > rss\v7.xml

goto END

:DISPLAY_USAGE
@echo.
@echo Usage: testscript [source file]
@echo.

:END