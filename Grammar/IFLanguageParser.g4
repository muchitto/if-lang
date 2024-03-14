parser grammar IFLanguageParser;

options {
	tokenVocab = IFLanguageLexer;
} // Use the lexer grammar's tokens

// Parser rules

// First there is the import statement, which is used to import other modules After that there are
// the main level statements
program:
	NEWLINE? (importStatement NEWLINE)* (
		mainLevelStatement NEWLINE?
	)*;

importStatement: (IMPORT IDENTIFIER FROM STRING)
	| (IMPORT STRING);

statementEnd: SEMICOLON? NEWLINE; // Statement end

mainLevelStatement:
	extendStatement
	| varDeclaration
	| objectDeclaration
	| functionDeclaration; // Main level statements include object definitions for now

extendStatement: (EXTEND IDENTIFIER objectDeclarationBody);

typeInfo: IDENTIFIER;

paramName: IDENTIFIER;
param: paramName (COLON typeInfo);

paramList: param (COMMA param)*?;

objectDeclaration:
	(annotation* NEWLINE)? (NEW | TYPE) declarationName declarationName withExpression?
		objectDeclarationBody;

objectDeclarationBody:
	NEWLINE? LCURLY NEWLINE? objectBodyStatement*? NEWLINE? RCURLY NEWLINE?;

flagExpression: (
		(IDENTIFIER IS? (ON | OFF))
		| (IS NOT? IDENTIFIER)
	);

propertySetExpression:
	IDENTIFIER (ASSIGN | IS)? expression; // Property set

annotationParameters: (atom (atom+)?) | (atom ((COMMA atom)+)?);

arrayAccessor: LBRAKET expression RBRAKET (arrayAccessor+)?;
arrayAccess: IDENTIFIER arrayAccessor;

fieldAccess: IDENTIFIER DOT (IDENTIFIER | fieldAccess)+?;

annotation:
	AT IDENTIFIER (
		(LPAREN annotationParameters RPAREN)
		| annotationParameters
	); // Defines an attribute

objectBodyStatement:
	STATIC? (
		functionDeclaration
		| (varDeclaration statementEnd)
		| (flagExpression statementEnd)
		| (propertySetExpression statementEnd)
	) NEWLINE?;

bodyStatement:
	(
		ifStatement
		| whileStatement
		| returnStatement
		| forStatement
		| (functionCall statementEnd)
		| (varDeclaration statementEnd)
		| functionDeclaration
	) NEWLINE?;

bodyBlock: LCURLY NEWLINE? (bodyStatement)* NEWLINE? RCURLY;

returnTypeInfo: ARROW typeInfo;

functionDeclaration:
	((annotation NEWLINE)+)? (
		(DEF declarationName LPAREN paramList? RPAREN)
		| (DEF declarationName)
	) returnTypeInfo? bodyBlock;

declarationName: IDENTIFIER;

// defExpression: DEF LPAREN paramList? RPAREN bodyBlock;

varDeclaration:
	VAR varDeclarationName (
		((COLON typeInfo)? (ASSIGN expression))
		| (COLON typeInfo)
	);

varDeclarationName: IDENTIFIER;

controlFlowExpression: (LPAREN expression RPAREN) | expression;

ifStatement:
	IF controlFlowExpression bodyBlock (
		(ELSE bodyBlock)
		| (ELSE ifStatement)
	)?;

whileStatement: WHILE controlFlowExpression bodyBlock;

forStatement: FOR IDENTIFIER IN expression bodyBlock;

arrayLiteral:
	LBRAKET NEWLINE? (expression (COMMA NEWLINE? expression)*)? NEWLINE? RBRAKET;

dictionaryRow: expression COLON expression;
multiKeyDictionaryRow: expression atom* COLON expression;

dictionaryLiteral:
	LBRAKET NEWLINE? (
		(dictionaryRow | multiKeyDictionaryRow) (
			COMMA? NEWLINE? (
				dictionaryRow
				| multiKeyDictionaryRow
			)
		)*
	)? NEWLINE? RBRAKET;

returnStatement: RETURN expression;

operator:
	TIMES		# multOp
	| DIVIDE	# divOp
	| MOD		# modOp
	| PLUS		# plusOp
	| MINUS		# minusOp
	| AND		# andOp
	| OR		# orOp
	| EQ		# eqOp
	| NEQ		# neqOp
	| LT		# ltOp
	| LE		# leOp
	| GT		# gtOp
	| GE		# geOp;

expression:
	primary TIMES expression
	| primary DIVIDE expression
	| primary MOD expression
	| primary PLUS expression
	| primary MINUS expression
	| LPAREN expression RPAREN
	| primary AND expression
	| primary OR expression
	| primary EQ expression
	| primary NEQ expression
	| primary LT expression
	| primary LE expression
	| primary GT expression
	| primary GE expression
	| NOT expression
	| flagExpression
	| primary
	| arrayAccess
	| fieldAccess
	| functionCall;

functionCall: (
		(
			IDENTIFIER LPAREN (expression (COMMA expression)*?)? RPAREN
		)
		| (IDENTIFIER (expression (COMMA expression)*?)?)
	);

primary: atom | arrayLiteral | dictionaryLiteral;
//| defExpression;

identifier: IDENTIFIER;
number: NUMBER;
string: STRING;
boolean: BOOLEAN;

atom: identifier | number | string | boolean;

withExpression:
	WITH (propertySetExpression | flagExpression) (
		(COMMA (propertySetExpression | flagExpression))+
	)?;