
/*
 * SMI, SPPI and ASN.1 lexer/parser 
 * 
 * Portions copyright (C) 2005 Nigel Sheridan-Smith
 *
 * Modified 11 January 2005 through to 13 January 2005 and beyond 
 *
 * Derived from ASN.1 lexer/parser by Vivek Gupta (11 November 2003)
 *
 */
grammar smi; 

options
{
	language=CSharp2;
	output=AST;
}

@parser::header
{
#pragma warning disable 3001, 3003, 3005, 3009, 1591 
}

@lexer::header
{
#pragma warning disable 3001, 3003, 3005, 3009, 1591 
}

@parser::namespace { Lextm.SharpSnmpLib.Mib.Ast.ANTLR }
@lexer::namespace { Lextm.SharpSnmpLib.Mib.Ast.ANTLR }

@parser::footer
{
#pragma warning restore 3001, 3003, 3005, 3009, 1591 
}

@lexer::footer
{
#pragma warning restore 3001, 3003, 3005, 3009, 1591 
}
//	Creation of ASN.1 grammar for ANTLR	V2.7.1
// ===================================================
//		  TOKENS FOR ASN.1 LEXER DEFINITIONS
// ===================================================

//	ASN1 Tokens 
/*
tokens {	
	ABSENT_KW;
	ABSTRACT_SYNTAX_KW;
	ALL_KW ;
	ANY_KW ;
	ARGUMENT_KW ;
	APPLICATION_KW ;
	AUTOMATIC_KW ;
	BASED_NUM_KW ;
	BEGIN_KW ;
	BIT_KW ;
	BMP_STR_KW ;
	BOOLEAN_KW ;
	BY_KW ;
	CHARACTER_KW ;
	CHOICE_KW ;
	CLASS_KW ;
	COMPONENTS_KW ;
	COMPONENT_KW ;
	CONSTRAINED_KW ;
	DEFAULT_KW ;
	DEFINED_KW ;
	DEFINITIONS_KW ;
	EMBEDDED_KW ;
	END_KW ;
	ENUMERATED_KW ;
	ERROR_KW ;
	ERRORS_KW ;
	EXCEPT_KW ;
	EXPLICIT_KW ;
	EXPORTS_KW ;
	EXTENSIBILITY_KW ;
	EXTERNAL_KW ;
	FALSE_KW ;
	FROM_KW ;
	GENERALIZED_TIME_KW ;
	GENERAL_STR_KW ;
	GRAPHIC_STR_KW ;
	IA5_STRING_KW ;
	IDENTIFIER_KW ;
	IMPLICIT_KW ;
	IMPLIED_KW ;
	IMPORTS_KW ;
	INCLUDES_KW ;
	INSTANCE_KW ;
	INTEGER_KW ;
	 INTERSECTION_KW ;
	 ISO646_STR_KW ;
	 LINKED_KW ;
	 MAX_KW ;
	 MINUS_INFINITY_KW ;
	 MIN_KW ;
	 NULL_KW ;
	 NUMERIC_STR_KW ;
	 OBJECT_DESCRIPTOR_KW ;
	 OBJECT_KW ;
	 OCTET_KW ;
	 OPERATION_KW ;
	 OF_KW ;
	 OID_KW ;
	 OPTIONAL_KW ;
	 PARAMETER_KW ;
	 PATTERN_KW;
	 PDV_KW ;
	 PLUS_INFINITY_KW ;
	 PRESENT_KW ;
	 PRINTABLE_STR_KW ;
	 PRIVATE_KW ;
	 REAL_KW ;
	 RELATIVE_KW ;
	 RESULT_KW ;
	 SEQUENCE_KW ;
	 SET_KW ;
	 SIZE_KW ;
	 STRING_KW ;
	 TAGS_KW ;
	 TELETEX_STR_KW ;
	 T61_STR_KW;
	 TRUE_KW ;
	 TYPE_IDENTIFIER_KW ;
	 UNION_KW ;
	 UNIQUE_KW ;
	 UNIVERSAL_KW ;
	 UNIVERSAL_STR_KW ;
	 UTC_TIME_KW ;
	 UTF8_STR_KW ;
	 VIDEOTEX_STR_KW ;
	 VISIBLE_STR_KW ;
	 WITH_KW ;
}
*/
ABSENT_KW
	:	'ABSENT'
	;

ABSTRACT_SYNTAX_KW
	:	'ABSTRACT-SYNTAX'
	;

ALL_KW
	:	'ALL'
	;

ANY_KW
	:	'ANY'
	;

ARGUMENT_KW
	:	'ARGUMENT'
	;

APPLICATION_KW
	:	'APPLICATION'
	;

AUTOMATIC_KW
	:	'AUTOMATIC'
	;

BASED_NUM_KW
	:	'BASEDNUM'
	;

BEGIN_KW
	:	'BEGIN'
	;

BIT_KW
	:	'BIT'
	;

BMP_STR_KW
	:	'BMPString'
	;

BOOLEAN_KW
	:	'BOOLEAN'
	;

BY_KW
	:	'BY'
	;

CHARACTER_KW
	:	'CHARACTER'
	;

CHOICE_KW
	:	'CHOICE'
	;

CLASS_KW
	:	'CLASS'
	;

COMPONENTS_KW
	:	'COMPONENTS'
	;

COMPONENT_KW
	:	'COMPONENT'
	;

CONSTRAINED_KW
	:	'CONSTRAINED'
	;

DEFAULT_KW
	:	'DEFAULT'
	;

DEFINED_KW
	:	'DEFINED'
	;

DEFINITIONS_KW
	:	'DEFINITIONS'
	;

EMBEDDED_KW
	:	'EMBEDDED'
	;

END_KW
	:	'END'
	;

ENUMERATED_KW
	:	'ENUMERATED'
	;

ERROR_KW
	:	'ERROR'
	;

ERRORS_KW
	:	'ERRORS'
	;

EXCEPT_KW
	:	'EXCEPT'
	;

EXPLICIT_KW
	:	'EXPLICIT'
	;

EXPORTS_KW
	:	'EXPORTS'
	;

EXTENSIBILITY_KW
	:	'EXTENSIBILITY'
	;

EXTERNAL_KW
	:	'EXTERNAL'
	;

FALSE_KW
	:	'FALSE'
	;

FROM_KW
	:	'FROM'
	;

GENERALIZED_TIME_KW
	:	'GeneralizedTime'
	;

GENERAL_STR_KW
	:	'GeneralString'
	;

GRAPHIC_STR_KW
	:	'GraphicString'
	;

IA5_STRING_KW
	:	'IA5String'
	;

IDENTIFIER_KW
	:	'IDENTIFIER'
	;

IMPLICIT_KW
	:	'IMPLICIT'
	;

IMPLIED_KW
	:	'IMPLIED'
	;

IMPORTS_KW
	:	'IMPORTS'
	;

INCLUDES_KW
	:	'INCLUDES'
	;

INSTANCE_KW
	:	'INSTANCE'
	;

INTEGER_KW
	:	'INTEGER'
	;

INTERSECTION_KW
	:	'INTERSECTION'
	;

ISO646_STR_KW
	:	'ISO646String'
	;

LINKED_KW
	:	'LINKED'
	;

MAX_KW
	:	'MAX'
	;

MINUS_INFINITY_KW
	:	'MINUSINFINITY'
	;

MIN_KW
	:	'MIN'
	;

NULL_KW
	:	'NULL'
	;

NUMERIC_STR_KW
	:	'NumericString'
	;

OBJECT_DESCRIPTOR_KW
	:	'ObjectDescriptor'
	;

OBJECT_KW
	:	'OBJECT'
	;

OCTET_KW
	:	'OCTET'
	;

OPERATION_KW
	:	'OPERATION'
	;

OF_KW
	:	'OF'
	;

OID_KW
	:	'OID'
	;

OPTIONAL_KW
	:	'OPTIONAL'
	;

PARAMETER_KW
	:	'PARAMETER'
	;

PDV_KW
	:	'PDV'
	;

PLUS_INFINITY_KW
	:	'PLUSINFINITY'
	;

PRESENT_KW
	:	'PRESENT'
	;

PRINTABLE_STR_KW
	:	'PrintableString'
	;

PRIVATE_KW
	:	'PRIVATE'
	;

REAL_KW
	:	'REAL'
	;

RELATIVE_KW
	:	'RELATIVE'
	;

RESULT_KW
	:	'RESULT'
	;

SEQUENCE_KW
	:	'SEQUENCE'
	;

SET_KW
	:	'SET'
	;

SIZE_KW
	:	'SIZE'
	;

STRING_KW
	:	'STRING'
	;

TAGS_KW
	:	'TAGS'
	;

TELETEX_STR_KW
	:	'TeletexString'
	;
	
T61_STR_KW
	:	'T61String'
	;

TRUE_KW
	:	'TRUE'
	;

TYPE_IDENTIFIER_KW
	:	'TYPE-IDENTIFIER'
	;

UNION_KW
	:	'UNION'
	;

UNIQUE_KW
	:	'UNIQUE'
	;

UNIVERSAL_KW
	:	'UNIVERSAL'
	;

UNIVERSAL_STR_KW
	:	'UniversalString'
	;

UTC_TIME_KW
	:	'UTCTime'
	;

UTF8_STR_KW
	:	'UTF8String'
	;

VIDEOTEX_STR_KW
	:	'VideotexString'
	;

VISIBLE_STR_KW
	:	'VisibleString'
	;

WITH_KW
	:	'WITH'
	;
	
PATTERN_KW
	: 'PATTERN'
	;	

// Operators

ASSIGN_OP:	'::=';
BAR:		'|';
COLON:		':';
COMMA:		',';
COMMENT:	'--';
DOT:		'.';
DOTDOT:		'..';
DOTDOTDOT
	:	'...';
// ELLIPSIS:	'...';
EXCLAMATION:	'!';
INTERSECTION:	'^';
LESS:		'<';
L_BRACE:	'{';
L_BRACKET:	'[';
L_PAREN:	'(';
MINUS:		'-';
PLUS:		'+';
R_BRACE:	'}';
R_BRACKET:	']';
R_PAREN:	')';
SEMI:		';';
SINGLE_QUOTE:	'\'';
CHARB:		'\'B';
CHARH:		'\'H';

// Whitespace -- ignored

WS			
    	:	(	' ' | '\t' | '\f'	|	(	
	:	'\r\n'		// DOS
	|	'\r'   		// Macintosh
	|	'\n'		// Unix 
	))+
	{ Skip(); }
	;

// Single-line comments
SL_COMMENT
	: (
	: COMMENT (  { input.LA(2)!='-' }? '-' 	|	~('-'|'\n'|'\r'))*	( (('\r')? '\n') | COMMENT) )
		{Skip();}
	;

protected
BDIG		: ('0'|'1') ;
protected
HDIG		:	( :('0'..'9') )
			|	('A'..'F')
			|	('a'..'f')
			;

NUMBER	:	('0'..'9')+ ;

/* NSS 13/1/05: Added '_' as acceptable character - required for some PIBs */

UPPER	
	:   ('A'..'Z') 
		(
	:	( 'a'..'z' | 'A'..'Z' |'-' | '0'..'9' ))* 	;   // '_' | 

LOWER
	:	('a'..'z') 
		(
	:	( 'a'..'z' | 'A'..'Z' |'-' | '0'..'9' ))* 	;   // '_' | 




// Unable to resolve a string like 010101 followed by 'H
//B_STRING 	: 	SINGLE_QUOTE ({LA(3)!='B'}? BDIG)+  BDIG SINGLE_QUOTE 'B';
//H_STRING 	: 	SINGLE_QUOTE ({LA(3)!='H'}? HDIG)+  HDIG SINGLE_QUOTE 'H';

B_OR_H_STRING
	:	(
		:(B_STRING)=>B_STRING
		| H_STRING)
	;
	
/* Changed by NSS 13/1/05 - upper case *or* lower case 'B' and 'H'; zero or more digits */
fragment
B_STRING 	: 	SINGLE_QUOTE (BDIG)* SINGLE_QUOTE ('B' | 'b') ;
fragment
H_STRING 	: 	SINGLE_QUOTE (HDIG)* SINGLE_QUOTE ('H' | 'h') ;	
	
C_STRING 	: 	'"' (options {greedy=false;}
                             : '\r\n'		// DOS
                             | '\r'   		// Macintosh
                             | '\n'		// Unix 
                             | ~('\r' | '\n')
                            )* 
                        '"' ;


//*************************************************************************
//**********		PARSER DEFINITIONS
//*************************************************************************


// Grammar Definitions

/* NSS 13/1/05: Added 'PIB-DEFINITIONS' for SPPI */
module_definition: module_identifier ('PIB-DEFINITIONS' | DEFINITIONS_KW) 
		( (EXPLICIT_KW | IMPLICIT_KW | AUTOMATIC_KW) TAGS_KW )? 
		(EXTENSIBILITY_KW IMPLIED_KW)?
		ASSIGN_OP BEGIN_KW module_body END_KW;

module_identifier: UPPER (obj_id_comp_lst)? ;

module_body: (exports)? (imports)? (assignment)* ;

/* NSS 15/1/05: Added syntactic predicate */
obj_id_comp_lst: L_BRACE ((LOWER (LOWER|NUMBER)) => defined_value)? (obj_id_component)+ R_BRACE;
//obj_id_comp_lst: L_BRACE (defined_value)? (obj_id_component)+ R_BRACE;

protected defined_value: (UPPER DOT)? LOWER ;

/* NSS 14/1/05: Checked against X.680 */
obj_id_component: NUMBER 
                | LOWER (L_PAREN NUMBER R_PAREN)?;

//obj_id_component: NUMBER 
//                | ( LOWER (L_PAREN NUMBER R_PAREN)? ) => LOWER (L_PAREN NUMBER R_PAREN)? 
//                | (defined_value) => defined_value;

tag_default: EXPLICIT_KW | IMPLICIT_KW | AUTOMATIC_KW;

exports: EXPORTS_KW ( (symbol_list)? | ALL_KW ) SEMI;

imports: IMPORTS_KW (symbols_from_module)* SEMI ;

/* NSS 14/1/05: Shouldn't need syntactic predicate */
assignment: UPPER ASSIGN_OP type 
          | LOWER type ASSIGN_OP value 
          | (UPPER | macroName) 'MACRO' ASSIGN_OP BEGIN_KW (~(END_KW))* END_KW ;

//assignment: UPPER ASSIGN_OP type 
//          | LOWER type ASSIGN_OP value 
//          | ((UPPER | macroName)
//                "MACRO" ASSIGN_OP BEGIN_KW (~(END_KW) )* END_KW) => 
//                (UPPER | macroName) "MACRO" ASSIGN_OP BEGIN_KW (~(END_KW))* END_KW ;

symbol_list: symbol (COMMA symbol)* ;

symbols_from_module: symbol_list FROM_KW UPPER 
                        ( obj_id_comp_lst 
                          | (defined_value) => defined_value 
                        )? ;

symbol: UPPER | LOWER | macroName;

macroName: OPERATION_KW | ERROR_KW  | 'BIND' | 'UNBIND' 
         | 'APPLICATION-SERVICE-ELEMENT' | 'APPLICATION-CONTEXT' | 'EXTENSION' 
         | 'EXTENSIONS' | 'EXTENSION-ATTRIBUTE' | 'TOKEN' | 'TOKEN-DATA' 
	 | 'SECURITY-CATEGORY' | 'OBJECT' | 'PORT' | 'REFINE' | 'ABSTRACT-BIND' 
	 | 'ABSTRACT-UNBIND' | 'ABSTRACT-OPERATION' | 'ABSTRACT-ERROR' 
	 | 'ALGORITHM' | 'ENCRYPTED' | 'SIGNED' | 'SIGNATURE' | 'PROTECTED' 
	 | smi_macros;

type: built_in_type | defined_type | selection_type | macros_type | smi_type;

value: (TRUE_KW) => TRUE_KW 
     | (FALSE_KW) => FALSE_KW 
     | (NULL_KW) => NULL_KW  
     | (C_STRING) => C_STRING 
     | (defined_value) => defined_value 
     | (signed_number) => signed_number
     | (choice_value) => choice_value 
     | (sequence_value) => sequence_value 
     | (sequenceof_value) => sequenceof_value 
     | (cstr_value) => cstr_value 
     | (obj_id_comp_lst) => obj_id_comp_lst 
     | (PLUS_INFINITY_KW) => PLUS_INFINITY_KW 
     | (MINUS_INFINITY_KW) => MINUS_INFINITY_KW;

built_in_type: any_type 
             | bit_string_type 
             | boolean_type 
             | character_str_type 
             | choice_type
             | embedded_type EMBEDDED_KW PDV_KW 
             | enum_type
             | external_type
	     | integer_type
	     | null_type
	     | object_identifier_type
	     | octetString_type
	     | real_type
	     | relativeOid_type
	     | sequence_type
	     | sequenceof_type
	     | set_type
	     | setof_type
	     | tagged_type;

defined_type: (UPPER DOT)? UPPER (constraint)? ;

selection_type: LOWER LESS type;

any_type: ANY_KW (DEFINED_KW BY_KW LOWER)? ;

/* NSS 15/1/2005: Added syntactic predicate */
bit_string_type: BIT_KW STRING_KW ((L_BRACE namedNumber) => namedNumber_list)? (constraint)? ;
//bit_string_type: BIT_KW STRING_KW (namedNumber_list)? (constraint)? ;

boolean_type: BOOLEAN_KW;

character_str_type: CHARACTER_KW STRING_KW | character_set (constraint)? ;

choice_type: CHOICE_KW L_BRACE elementType_list R_BRACE;

embedded_type: EMBEDDED_KW PDV_KW;

enum_type: ENUMERATED_KW namedNumber_list;

external_type: EXTERNAL_KW;

/* NSS 15/1/05: Added syntactic predicate */
integer_type: INTEGER_KW ((L_BRACE namedNumber) => namedNumber_list | constraint)? ;
//integer_type: INTEGER_KW (namedNumber_list | constraint)? ;

null_type: NULL_KW;

object_identifier_type: OBJECT_KW IDENTIFIER_KW;

octetString_type: OCTET_KW STRING_KW (constraint)? ;

real_type: REAL_KW;

/* NSS 14/1/05: Will this work? I think not! This token detected is an UPPER */
relativeOid_type: 'RELATIVE-OID';
//relativeOid_type: RELATIVE_KW MINUS OID_KW;

sequence_type: SEQUENCE_KW L_BRACE (elementType_list)? R_BRACE ;

sequenceof_type: SEQUENCE_KW (SIZE_KW constraint)? OF_KW type;

set_type: SET_KW L_BRACE (elementType_list)? R_BRACE;

setof_type: SET_KW (SIZE_KW constraint)? OF_KW type;

tagged_type: tag (tag_default)? type;

namedNumber_list: L_BRACE namedNumber (COMMA namedNumber)* R_BRACE;

constraint: L_PAREN (element_set_specs)? (exception_spec)? R_PAREN;

character_set: BMP_STR_KW | GENERALIZED_TIME_KW | GENERAL_STR_KW | GRAPHIC_STR_KW
             | IA5_STRING_KW | ISO646_STR_KW | NUMERIC_STR_KW | PRINTABLE_STR_KW
             | TELETEX_STR_KW | T61_STR_KW | UNIVERSAL_STR_KW | UTF8_STR_KW
             | UTC_TIME_KW | VIDEOTEX_STR_KW | VISIBLE_STR_KW;

elementType_list: elementType (COMMA elementType)* ;

tag: L_BRACKET (clazz)? class_NUMBER R_BRACKET;

clazz: UNIVERSAL_KW | APPLICATION_KW | PRIVATE_KW;

/* NSS 14/1/05: 'defined_value' not 'LOWER' */
class_NUMBER: NUMBER | defined_value;

/* NSS 15/1/05: Added syntactic predicates; removed 'SEMI' */
operation_macro: 'OPERATION' (ARGUMENT_KW ((LOWER) => LOWER)? type )? 
                    ( (RESULT_KW) => RESULT_KW 
                        ((LOWER) => ((LOWER) => LOWER)? type )? 
					)?
                    ( (ERRORS_KW) => ERRORS_KW L_BRACE (operation_errorlist)? R_BRACE )? 
                    ( (LINKED_KW) => LINKED_KW L_BRACE (linkedOp_list)? R_BRACE )? ;
//operation_macro: "OPERATION" (ARGUMENT_KW (LOWER)? type )? 
//                    ( RESULT_KW 
//                        ( (SEMI) => SEMI 
//                          | ((LOWER)? type) => (LOWER)? type )? 
//                        )? 
//                    ( ERRORS_KW L_BRACE (operation_errorlist)? R_BRACE )? 
//                    ( LINKED_KW L_BRACE (linkedOp_list)? R_BRACE )? ;

/* NSS 15/1/05: Added syntactic predicate */
error_macro: ERROR_KW ( PARAMETER_KW ((LOWER) => LOWER)? type )? ;
//error_macro: ERROR_KW ( PARAMETER_KW (LOWER)? type )? ;


/* SMI processing - a bunch of macros defined in RFC 1155 and RFC 2578 for SMI v1 and v2 respectively */
/* Adapted/added by Nigel Sheridan-Smith 12 January 2005 */

macros_type: operation_macro | error_macro | objecttype_macro | moduleidentity_macro 
           | objectidentity_macro | notificationtype_macro | textualconvention_macro 
           | objectgroup_macro | notificationgroup_macro | modulecompliance_macro 
           | agentcapabilities_macro | traptype_macro;

smi_macros: 'OBJECT-TYPE' | 'MODULE-IDENTITY' | 'OBJECT-IDENTITY' | 'NOTIFICATION-TYPE' 
         | 'TEXTUAL-CONVENTION' | 'OBJECT-GROUP' | 'NOTIFICATION-GROUP' | 'MODULE-COMPLIANCE' 
         | 'AGENT-CAPABILITIES' | 'TRAP-TYPE';

/* NSS 12-13/1/05: SMI types - some of these are standard 'textual conventions' which can replace BITS */
/* NSS 13/1/05: Added 'LOWER' since some PIBs can't handle it */
smi_type: 'BITS' | INTEGER_KW | OCTET_KW STRING_KW | OBJECT_KW IDENTIFIER_KW | UPPER; // | LOWER;

/* Possible SMI types??? - IpAddress Counter32 TimeTicks Opaque Counter64 Unsigned32 */


/* SMI v2: Sub-typing - defined in RFC 1902 section 7.1 and appendix C and RFC 1904 */
smi_subtyping: L_PAREN subtype_range (BAR subtype_range)* R_PAREN
             | L_PAREN 'SIZE' L_PAREN subtype_range (BAR subtype_range)* R_PAREN R_PAREN;
subtype_range: subtype_value (DOTDOT subtype_value)? ;
subtype_value: (MINUS)? NUMBER | B_STRING | H_STRING;

/* SMI v1/2 and SPPI: Object-type macro */
objecttype_macro: 'OBJECT-TYPE' 'SYNTAX' 
                    ( (smi_type L_BRACE) => smi_type objecttype_macro_namedbits 
                     | (smi_type) => smi_type (smi_subtyping)?
                     | type
                    ) 
                  ('UNITS' C_STRING)? 
                  ( ('MAX-ACCESS' | 'ACCESS') objecttype_macro_accesstypes 
                   | 'PIB-ACCESS' objecttype_macro_pibaccess)?              /* Only in SPPI; Optional */
                  ('PIB-REFERENCES' L_BRACE value R_BRACE)?                 /* Only in SPPI */
                  ('PIB-TAG' L_BRACE value R_BRACE)?                        /* Only in SPPI */
                  'STATUS' objecttype_macro_statustypes 
                  ( ('DESCRIPTION') => 'DESCRIPTION' C_STRING )?                               /* Optional only for SMIv1 */
                  ('INSTALL-ERRORS' L_BRACE objecttype_macro_error (COMMA objecttype_macro_error)* R_BRACE)?    /* Only in SPPI */
		  ( 'REFERENCE' C_STRING )? 
		  ( (~('PIB-INDEX')) => 'INDEX' objecttype_macro_index 
                    | 'AUGMENTS' objecttype_macro_augments 
                    | 'PIB-INDEX' L_BRACE value R_BRACE                     /* Only in SPPI */
                    | 'EXTENDS' L_BRACE value R_BRACE                       /* Only in SPPI */
                  )? 
                  ( 'INDEX' objecttype_macro_index )?                       /* Only in SPPI - replicated from above */
                  ( 'UNIQUENESS' L_BRACE (value (COMMA value)* )? R_BRACE)?                      /* Only in SPPI */
		  ( ('DEFVAL') => 'DEFVAL' L_BRACE 
                    ( (L_BRACE LOWER (COMMA | R_BRACE)) => objecttype_macro_bitsvalue
                       | value) 
                    R_BRACE )? ;
protected objecttype_macro_accesstypes: l=LOWER 
                                        {if (l.Text == ("read-only") || l.Text == ("read-write") 
                                            || l.Text == ("write-only") || l.Text == ("read-create") 
                                            || l.Text == ("not-accessible") || l.Text == ("accessible-for-notify"))
                                            {/*DOSOMETHING*/} else {throw new SemanticException("(invalid)");}};
protected objecttype_macro_pibaccess: l=LOWER 
                                        {if (l.Text == ("install") 
                                            || l.Text == ("notify") 
                                            || l.Text == ("install-notify") 
                                            || l.Text == ("report-only"))
                                            {/*DOSOMETHING*/} else {throw new SemanticException("(invalid)");}};
protected objecttype_macro_statustypes: l=LOWER
                                        {if (l.Text == ("mandatory") 
                                            || l.Text == ("optional") 
                                            || l.Text == ("obsolete") 
                                            || l.Text == ("current") 
                                            || l.Text == ("deprecated"))
                                            {/*DOSOMETHING*/} else {throw new SemanticException("(invalid)");}};

// 'typeorvaluelist' in original ASN.1 grammar between braces
objecttype_macro_index: L_BRACE objecttype_macro_indextype (COMMA objecttype_macro_indextype)* R_BRACE;       
objecttype_macro_indextype: ('IMPLIED')? value;
objecttype_macro_augments: L_BRACE value R_BRACE;  
/* NSS 13/1/05: Added LOWER *and* UPPER for a PIB */
objecttype_macro_namedbits: L_BRACE (LOWER) L_PAREN NUMBER R_PAREN (COMMA (LOWER) L_PAREN NUMBER R_PAREN)* R_BRACE;     //|UPPER
objecttype_macro_bitsvalue: L_BRACE LOWER (COMMA LOWER)* R_BRACE;     
objecttype_macro_error: LOWER L_PAREN NUMBER R_PAREN;

/* SMI v2 and SPPI: Module-identity macro */
moduleidentity_macro: 'MODULE-IDENTITY' 
                        ('SUBJECT-CATEGORIES' L_BRACE moduleidentity_macro_categories R_BRACE)? /* Only in SPPI */
                        'LAST-UPDATED' C_STRING 'ORGANIZATION' C_STRING 'CONTACT-INFO' C_STRING 
                        'DESCRIPTION' C_STRING (moduleidentity_macro_revision)* ;
moduleidentity_macro_revision: 'REVISION' C_STRING 'DESCRIPTION' C_STRING; 
moduleidentity_macro_categories: l=LOWER {if (l.Text !=  ("all")) {throw new SemanticException ("(invalid)");}} 
                               | moduleidentity_macro_categoryid (COMMA moduleidentity_macro_categoryid)*;
moduleidentity_macro_categoryid: LOWER L_PAREN NUMBER R_PAREN;
 
/* SMI v2 and SPPI: Object-identity macro */
objectidentity_macro: 'OBJECT-IDENTITY' 'STATUS' objectidentity_macro_statustypes 'DESCRIPTION' C_STRING ('REFERENCE' C_STRING)? ;
protected objectidentity_macro_statustypes: l=LOWER 
                                        {if (l.Text == ("current") 
                                                || l.Text == ("deprecated") 
                                                || l.Text == ("obsolete"))
                                            {/*DOSOMETHING*/} else {throw new SemanticException("(invalid)");}};


/* SMI v2: Notification-type macro */
notificationtype_macro: 'NOTIFICATION-TYPE' ('OBJECTS' L_BRACE value (COMMA value)* R_BRACE)? 
                                            'STATUS' notificationtype_macro_status 'DESCRIPTION' C_STRING ('REFERENCE' C_STRING)? ;
protected notificationtype_macro_status: l=LOWER 
                                        {if (l.Text == ("current") 
                                                || l.Text == ("deprecated") 
                                                || l.Text == ("obsolete"))
                                            {/*DOSOMETHING*/} else {throw new SemanticException("(invalid)");}};

/* SMI v2 and SPPI: Textual convention */
textualconvention_macro: 'TEXTUAL-CONVENTION' ('DISPLAY-HINT' C_STRING)? 
                            'STATUS' textualconvention_macro_status 
                            'DESCRIPTION' C_STRING 
                            ('REFERENCE' C_STRING)? 
                            'SYNTAX' ( (smi_type L_BRACE) => smi_type L_BRACE textualconvention_macro_namedbit 
                                    (COMMA textualconvention_macro_namedbit)* R_BRACE | type);
protected textualconvention_macro_status: l=LOWER 
                                        {if (l.Text == ("current") 
                                                || l.Text == ("deprecated") 
                                                || l.Text == ("obsolete"))
                                            {/*DOSOMETHING*/} else {throw new SemanticException("(invalid)");}};
textualconvention_macro_namedbit: LOWER L_PAREN (MINUS)? NUMBER R_PAREN;

/* SMI v2 and SPPI: Object group */
objectgroup_macro: 'OBJECT-GROUP' 'OBJECTS' L_BRACE value (COMMA value)* R_BRACE 
                        'STATUS' objectgroup_macro_status 'DESCRIPTION' C_STRING ('REFERENCE' C_STRING)? ;
objectgroup_macro_status: l=LOWER
                                        {if (l.Text == ("current") 
                                                || l.Text == ("deprecated") 
                                                || l.Text == ("obsolete"))
                                            {/*DOSOMETHING*/} else {throw new SemanticException("(invalid)");}};
 
/* SMI v2: Notification group*/
notificationgroup_macro: 'NOTIFICATION-GROUP' 'NOTIFICATIONS' L_BRACE value (COMMA value)* R_BRACE 
                        'STATUS' notificationgroup_macro_status 'DESCRIPTION' C_STRING ('REFERENCE' C_STRING)? ;
notificationgroup_macro_status: l=LOWER
                                        {if (l.Text == ("current") 
                                                || l.Text == ("deprecated") 
                                                || l.Text == ("obsolete"))
                                            {/*DOSOMETHING*/} else {throw new SemanticException("(invalid)");}};
 
/* SMI v2 and SPPI: Module compliance */
modulecompliance_macro: 'MODULE-COMPLIANCE' 'STATUS' modulecompliance_macro_status
                        'DESCRIPTION' C_STRING ('REFERENCE' C_STRING)? (modulecompliance_macro_module)+ ;
modulecompliance_macro_status: l=LOWER
                                        {if (l.Text == ("current") 
                                                || l.Text == ("deprecated") 
                                                || l.Text == ("obsolete"))
                                            {/*DOSOMETHING*/} else {throw new SemanticException("(invalid)");}};
modulecompliance_macro_module: 'MODULE' ((UPPER) => UPPER ((value) => value)? )? 
                                ('MANDATORY-GROUPS' L_BRACE value (COMMA value)* R_BRACE)?
                                (modulecompliance_macro_compliance)* ;
modulecompliance_macro_compliance: 'GROUP' value 'DESCRIPTION' C_STRING
                                 | 'OBJECT' value 
                                    ('SYNTAX' modulecompliance_macro_syntax)? 
                                    ('WRITE-SYNTAX' modulecompliance_macro_syntax)?     /* Only in SMI v2 */
                                    ('MIN-ACCESS' modulecompliance_macro_access)? 
                                    ('PIB-MIN-ACCESS' modulecompliance_macro_pibaccess)?    /* Only in SPPI */
                                    'DESCRIPTION' C_STRING;
modulecompliance_macro_syntax: (smi_type L_BRACE) => smi_type L_BRACE modulecompliance_macro_namedbit (COMMA modulecompliance_macro_namedbit)* R_BRACE
                             | (smi_type) => smi_type (smi_subtyping)?
                             | type;
modulecompliance_macro_namedbit: LOWER L_PAREN NUMBER R_PAREN;
modulecompliance_macro_access: l=LOWER
                                        {if (l.Text == ("not-accessible") 
                                                || l.Text == ("accessible-for-notify") 
                                                || l.Text == ("read-only")
                                                || l.Text == ("read-write")
                                                || l.Text == ("read-create"))
                                            {/*DOSOMETHING*/} else {throw new SemanticException("(invalid)");}};
modulecompliance_macro_pibaccess: l=LOWER
                                        {if (l.Text == ("not-accessible") 
                                                || l.Text == ("install") 
                                                || l.Text == ("notify")
                                                || l.Text == ("install-notify")
                                                || l.Text == ("report-only"))
                                            {/*DOSOMETHING*/} else {throw new SemanticException("(invalid)");}};

/* SMI v2: Agent capabilities */
agentcapabilities_macro: 'AGENT-CAPABILITIES' 'PRODUCT-RELEASE' C_STRING 'STATUS' agentcapabilities_macro_status
                         'DESCRIPTION' C_STRING ('REFERENCE' C_STRING)? (agentcapabilities_macro_module)*;
agentcapabilities_macro_status: l=LOWER
                                        {if (l.Text == ("current") 
                                                || l.Text == ("obsolete"))
                                            {/*DOSOMETHING*/} else {throw new SemanticException("(invalid)");}};
agentcapabilities_macro_module: 'SUPPORTS' LOWER (value)? 
                                'INCLUDES' L_BRACE value (COMMA value)* R_BRACE 
                                (agentcapabilities_macro_variation)*;
agentcapabilities_macro_variation: 'VARIATION' value ('SYNTAX' agentcapabilities_macro_syntax)? ('WRITE-SYNTAX' agentcapabilities_macro_syntax)? ('ACCESS' agentcapabilities_macro_access)? 
                                    ('CREATION-REQUIRES' L_BRACE value (COMMA value)* R_BRACE)? 
                                    ('DEFVAL' L_BRACE ((L_BRACE (LOWER | COMMA | R_BRACE)) => L_BRACE (LOWER)? (COMMA LOWER)* R_BRACE | value) )? 
                                    'DESCRIPTION' C_STRING;
agentcapabilities_macro_syntax: (smi_type L_BRACE) => 
                                    smi_type L_BRACE agentcapabilities_macro_namedbit (COMMA agentcapabilities_macro_namedbit)* R_BRACE
                              | (smi_type) => smi_type (smi_subtyping)?
                              | type ;
agentcapabilities_macro_access: l=LOWER
                                        {if (l.Text == ("not-implemented") 
                                                || l.Text == ("accessible-for-notify") 
                                                || l.Text == ("read-only")
                                                || l.Text == ("read-write")
                                                || l.Text == ("read-create")
                                                || l.Text == ("write-only"))
                                            {/*DOSOMETHING*/} else {throw new SemanticException("(invalid)");}};
agentcapabilities_macro_namedbit: LOWER L_PAREN NUMBER R_PAREN;


/* SMI v1: Trap types */
traptype_macro: 'TRAP-TYPE' 'ENTERPRISE' value ('VARIABLES' L_BRACE value (COMMA value)* R_BRACE)? 
                    (('DESCRIPTION') => 'DESCRIPTION' value)? ('REFERENCE' value)? ;

operation_errorlist: typeorvalue (COMMA typeorvalue)* ;

linkedOp_list: typeorvalue (COMMA typeorvalue)* ;

typeorvalue: (type) => type | value;

// ERROR HERE in ASN.1 grammar? '*' was only applied to 'typeorvalue'
typeorvaluelist: typeorvalue (COMMA typeorvalue)* ;

/* NSS 15/1/05: Added syntactic predicate */
elementType: LOWER  ((L_BRACKET (NUMBER|UPPER|LOWER)) => tag)? 
                    (tag_default)? type (OPTIONAL_KW | DEFAULT_KW value)? 
           | COMPONENTS_KW OF_KW type;
//elementType: LOWER  (tag)? (tag_default)? type (OPTIONAL_KW | DEFAULT_KW value)? 
//           | COMPONENTS_KW OF_KW type;

namedNumber: LOWER L_PAREN (signed_number | defined_value) R_PAREN;

signed_number: (MINUS)? NUMBER;

element_set_specs: element_set_spec (COMMA DOTDOTDOT)? (COMMA element_set_spec)? ;

exception_spec: EXCLAMATION 
                ( (signed_number) => signed_number 
                  | (defined_value) => defined_value 
                  | type COLON value
                );

element_set_spec: intersections ( (BAR | UNION_KW) intersections )* 
                | ALL_KW EXCEPT_KW constraint_elements;

intersections: constraint_elements (EXCEPT_KW constraint_elements)? 
                ( (INTERSECTION | INTERSECTION_KW) constraint_elements (EXCEPT_KW constraint_elements)? )* ;

constraint_elements: (value) => value 
                   | (value_range) => value_range
                   | SIZE_KW constraint 
                   | FROM_KW constraint 
                   | L_PAREN element_set_spec R_PAREN 
                   | (INCLUDES_KW)? type 
                   | PATTERN_KW value 
                   | WITH_KW (COMPONENT_KW constraint 
                              | COMPONENTS_KW L_BRACE (DOTDOTDOT COMMA)? 
                                type_constraint_list R_BRACE);

value_range: (value | MIN_KW) (LESS)? DOTDOT (LESS)? (value | MAX_KW) ;

type_constraint_list: named_constraint (COMMA named_constraint)* ;

named_constraint: LOWER (constraint)? (PRESENT_KW | ABSENT_KW | OPTIONAL_KW)? ;

choice_value: LOWER (COLON)? value;

sequence_value: L_BRACE (named_value)? (COMMA named_value)* R_BRACE;

sequenceof_value: L_BRACE (value)? (COMMA value)* R_BRACE;

cstr_value: (H_STRING) => H_STRING 
          | (B_STRING) => B_STRING 
          | L_BRACE 
            ( (id_list) => id_list 
              | (char_defs_list) => char_defs_list 
              | tuple_or_quad
            ) R_BRACE;

id_list: LOWER (COMMA LOWER)* ;

char_defs_list: char_defs (COMMA char_defs)* ;

//ERROR: no R_BRACE required here
tuple_or_quad: signed_number COMMA signed_number (COMMA signed_number COMMA signed_number)? ;


char_defs: C_STRING 
         | L_BRACE tuple_or_quad R_BRACE
         | defined_value;
//char_defs: C_STRING 
//         | L_BRACE signed_number COMMA signed_number ( R_BRACE | COMMA signed_number COMMA signed_number R_BRACE ) 
//         | defined_value;

named_value: LOWER value;
