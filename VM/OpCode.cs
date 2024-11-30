namespace VM;

public enum OpCode
{
    // Basic Arithmetic and Logic Operations
    Add, // Add two values
    Sub, // Subtract one value from another
    Mul, // Multiply two values
    Div, // Divide one value by another
    Mod, // Modulus operation
    And, // Logical AND
    Or, // Logical OR
    Not, // Logical NOT
    Eq, // Equality comparison
    Neq, // Not equal comparison
    Gt, // Greater than
    Lt, // Less than
    Gte, // Greater than or equal to
    Lte, // Less than or equal to

    // Variable and Data Management
    Load, // Load a variable or constant value
    Store, // Store a value in a variable
    Const, // Push a constant value (number, string, etc.)

    // Control Flow
    Jmp, // Unconditional jump to another instruction
    JmpIfTrue, // Jump if the top of the stack evaluates to true
    JmpIfFalse, // Jump if the top of the stack evaluates to false
    Call, // Call a function/method
    Ret, // Return from a function/method

    // Object-Oriented Programming
    NewObj, // Instantiate an object
    NewType, // Define a new object type (does not instantiate)
    SetAttr, // Set an attribute on an object
    GetAttr, // Get an attribute from an object
    AttachMethod, // Attach a function which is on the stack
    InvokeMethod, // Call a method on an object
    ExtendType, // Extend an existing type

    // Function

    // Attribute and Decorator Handling
    Annotate // Apply an annotation/attribute to a method or class. This is used for recording annotations that can be queried at runtime.
}