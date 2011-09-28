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
if not exist "atom" md "atom"

@rem turn output on
@echo on

@echo *** Generating Unique ID's ***
@rem generate unique ID's from the master RSS file
cscript /nologo fsatomconvert.js %1% > atom\v1.xml

@echo.
@echo *** test 1: create new nodes, no conflict ***

@rem endpoint 1, create nodes
cscript /nologo fsatomcreate.js atom\v1.xml ep1.100 "Buy groceries" "Get milk and eggs" EP1 > atom\ep1_v1a.xml
cscript /nologo fsatomcreate.js atom\ep1_v1a.xml ep1.101 "Get car serviced" "Needs oil change" EP1 > atom\ep1_v1b.xml

@rem endpoint 2: create nodes
cscript /nologo fsatomcreate.js atom\v1.xml ep2.100 "Return books to library" "2 books in total" EP2 > atom\ep2_v1a.xml
cscript /nologo fsatomcreate.js atom\ep2_v1a.xml ep2.101 "Get birthday gift for mother" "She likes blue flowers" EP2 > atom\ep2_v1b.xml

@rem merge, creating V2
cscript /nologo fsatommerge.js atom\ep1_v1b.xml atom\ep2_v1b.xml > atom\v2.xml


@echo.
@echo *** test 2: update with no conflict ***

@rem update endpoint 1 node
cscript /nologo fsatomupdate.js atom\v2.xml ep1.100 "Buy Groceries" "Get milk, eggs and butter" EP1 > atom\ep1_v2a.xml

@rem update endpoint 2 node
cscript /nologo fsatomupdate.js atom\ep1_v2a.xml ep1.100 "Buy Groceries" "Get milk, eggs, butter and bread" EP2 > atom\v3.xml


@echo.
@echo *** test 3: update with conflict ***

@rem endpoint 1 modification
cscript /nologo fsatomupdate.js atom\v3.xml ep1.100 "Buy Groceries" "Get milk, eggs, butter and rolls" EP1 > atom\ep1_v3a.xml

@rem insert pause due to time granularity, to ensure known outcome of endpoint 2 winning.
@rem otherwise with a gmt format granularity of only seconds, the update will be on the 
@rem identical time tick.
@echo Inserting time delay for modification 
@pause 
cscript /nologo fsatomupdate.js atom\v3.xml ep1.100 "Buy Groceries - DONE" "Get milk, eggs, butter and bread" EP2 > atom\ep2_v3a.xml

@rem merge
cscript /nologo fsatommerge.js atom\ep1_v3a.xml atom\ep2_v3a.xml > atom\v4.xml


@echo.
@echo *** test 4: resolve conflict

@rem endpoint 2 resolve
cscript /nologo fsatomupdate.js atom\v4.xml ep1.100 "Buy Groceries - DONE" "Get milk, eggs, butter and bread" EP2 true > atom\v5.xml


@echo.
@echo *** test 5: delete without conflict

@rem do independent deletes
cscript /nologo fsatomdelete.js atom\v5.xml ep1.101 EP1 > atom\ep1_v5a.xml
cscript /nologo fsatomdelete.js atom\v5.xml ep2.101 EP2 > atom\ep2_v5a.xml

@rem merge
cscript /nologo fsatommerge.js atom\ep1_v5a.xml atom\ep2_v5a.xml > atom\v6.xml


@echo.
@echo *** test 6: delete with conflict - both delete same item.  Semantically not a conflict, but by replication rules, this is a conflict.
cscript /nologo fsatomdelete.js atom\v6.xml ep2.100 EP1 > atom\ep1_v6a.xml

@echo Inserting time delay for modification 
@pause 
cscript /nologo fsatomdelete.js atom\v6.xml ep2.100 EP2 > atom\ep2_v6a.xml

@rem merge
cscript /nologo fsatommerge.js atom\ep1_v6a.xml atom\ep2_v6a.xml > atom\v7.xml

goto END

:DISPLAY_USAGE
@echo.
@echo Usage: testscript [source file]
@echo.

:END