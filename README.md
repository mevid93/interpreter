# interpreter
Mini-PL interpreter


## Contect-free grammer for Mini-PL
---
'''
  <prog>   ::=  <stmts> 
  <stmts>  ::=  <stmt> ";" ( <stmt> ";" )* 
  <stmt>   ::=  "var" <var_ident> ":" <type> [ ":=" <expr> ]       
            |   <var_ident> ":=" <expr>         	
            |   "for" <var_ident> "in" <expr> ".." <expr> "do"               	              <stmts> "end" "for"         	
            |   "read" <var_ident>         	
            |   "print" <expr>         	
            |   "assert" "(" <expr> ")" 

  <expr>   ::=  <opnd> <op> <opnd>       	
            |   [ <unary_op> ] <opnd> 
  
  <opnd>   ::=  <int>       	
            |   <string>       	
            |   <var_ident>       	
            |   "(" expr ")" 

  <type>   ::=  "int" | "string" | "bool" 
  <var_ident> ::= <ident> 

  <reserved keyword> ::=  
             "var" | "for" | "end" | "in" | "do" | "read" |           	        "print" | "int" | "string" | "bool" | "assert"
'''


## Example program
---
'''
var nTimes : int := 0; 	
print "How many times? ";  	
read nTimes;  	
var x : int; 	
for x in 0..nTimes do      	
    print x;     	
    print " : Hello, World!\n"; 	
end for; 	
assert (x = nTimes); 	
'''