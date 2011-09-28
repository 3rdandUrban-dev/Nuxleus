// Guids.h
//

// do not use #pragma once - used by ctc compiler
#ifndef __GUIDS_H_
#define __GUIDS_H_

#ifndef _CTC_GUIDS_


// guidPersistanceSlot ID for our Tool Window
// {9278d37d-d304-4967-952f-34a31adfcac5}
DEFINE_GUID(GUID_guidPersistanceSlot, 
0x9278D37D, 0xD304, 0x4967, 0x95, 0x2F, 0x34, 0xA3, 0x1A, 0xDF, 0xCA, 0xC5);

#define guidXPathmaniaPkg   CLSID_XPathmaniaPackage

// Command set guid for our commands (used with IOleCommandTarget)
// {ea087ddc-5244-4d7f-b1ca-e1211a815a04}
DEFINE_GUID(guidXPathmaniaCmdSet, 
0xEA087DDC, 0x5244, 0x4D7F, 0xB1, 0xCA, 0xE1, 0x21, 0x1A, 0x81, 0x5A, 0x4);

#else  // _CTC_GUIDS

#define guidXPathmaniaPkg      { 0x2E2C89A9, 0x57E5, 0x449F, { 0x9A, 0xD4, 0x90, 0x8, 0x3, 0x23, 0xD6, 0x2C } }
#define guidXPathmaniaCmdSet	  { 0xEA087DDC, 0x5244, 0x4D7F, { 0xB1, 0xCA, 0xE1, 0x21, 0x1A, 0x81, 0x5A, 0x4 } }


#endif // _CTC_GUIDS_

#endif // __GUIDS_H_
