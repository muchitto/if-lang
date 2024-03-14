namespace VM;

public enum OpCode
{
    // Basic Arithmetic and Logic Operations
    ADD, // Add two values
    SUB, // Subtract one value from another
    MUL, // Multiply two values
    DIV, // Divide one value by another
    MOD, // Modulus operation
    AND, // Logical AND
    OR, // Logical OR
    NOT, // Logical NOT
    EQ, // Equality comparison
    NEQ, // Not equal comparison
    GT, // Greater than
    LT, // Less than
    GTE, // Greater than or equal to
    LTE, // Less than or equal to

    // Variable and Data Management
    LOAD, // Load a variable or constant value
    STORE, // Store a value in a variable
    CONST, // Push a constant value (number, string, etc.)

    // Control Flow
    JMP, // Unconditional jump to another instruction
    JMP_IF_TRUE, // Jump if the top of the stack evaluates to true
    JMP_IF_FALSE, // Jump if the top of the stack evaluates to false
    CALL, // Call a function/method
    RET, // Return from a function/method

    // Object-Oriented Programming
    NEW_OBJ, // Instantiate an object
    NEW_TYPE, // Define a new object type (does not instantiate)
    SET_ATTR, // Set an attribute on an object
    GET_ATTR, // Get an attribute from an object
    ATTACH_METHOD, // Attach a function which is on the stack
    INVOKE_METHOD, // Call a method on an object
    EXTEND_TYPE, // Extend an existing type

    // Function

    // Attribute and Decorator Handling
    ANNOTATE // Apply an annotation/attribute to a method or class. This is used for recording annotations that can be queried at runtime.
}