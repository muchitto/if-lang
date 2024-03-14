lexer grammar IFLanguageLexer;

// Lexer rules

// Keywords
NEW: 'new';
DEF: 'def';
VAR: 'var';
IS: 'is';
IF: 'if';
ELSE: 'else';
WHILE: 'while';
FOR: 'for';
IN: 'in';
BREAK: 'break';
CONTINUE: 'continue';
RETURN: 'return';
TYPE: 'type';
WITH: 'with';
IMPORT: 'import';
FROM: 'from';
STATIC: 'static';
EXTEND: 'extend';
ON: 'on';
OFF: 'off';
HAS: 'has';

BOOLEAN: TRUE | FALSE; // Boolean
TRUE: 'true';
FALSE: 'false';
// Type is used when defining a new object type, instead of instantiating the object straight away

// Symbols
LCURLY: '{';
RCURLY: '}';
LPAREN: '(';
RPAREN: ')';
LBRAKET: '[';
RBRAKET: ']';
COLON: ':';
SEMICOLON: ';';
COMMA: ',';
AT: '@';
ARROW: '->';
DOT: '.';

PLUS: '+';
MINUS: '-';
TIMES: '*';
DIVIDE: '/';
MOD: '%';
AND: '&&' | 'and';
OR: '||' | 'or';
NOT: '!' | 'not';
ASSIGN: '=';
EQ: '==';
NEQ: '!=';
LT: '<';
LE: '<=';
GT: '>';
GE: '>=';

// Skipping and utility tokens
NEWLINE: ('\n' WS?)+; // Matches one or more newline sequences
WS: [ \t\r\u00A0]+ -> skip; // Skips whitespaces
COMMENT: '//' (~[\r\n])* NEWLINE -> skip; // Single line comment
MULTILINE_COMMENT_START: '/*' .*? '*/' -> skip;
// Use nested multiline comment mode in the future

fragment QUOTE: '"';
fragment SINGLE_QUOTE: '\'';
fragment BACKTICK: '`';

// Literals and identifiers
NUMBER: DIGIT ('.' DIGIT)?; // Number, supports integer and float
STRING: (QUOTE ( '\\\\' | '\\"' | ~["\\])* QUOTE)
	| (SINGLE_QUOTE ( '\\\\' | '\\\'' | ~['\\])* SINGLE_QUOTE)
	| (BACKTICK ( '\\\\' | '\\`' | ~['\\])* BACKTICK); // String
IDENTIFIER: LETTER (LETTER | DIGIT)*;

// These are fine where they were; no need to change
fragment DIGIT: [0-9]+;
fragment LETTER: [a-zA-Z_];

mode MULTILINE_COMMENT_MODE;
MULTILINE_COMMENT_END: '*/' -> popMode;
MULTILINE_COMMENT_CONTENT: . -> skip;