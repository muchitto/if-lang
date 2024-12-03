using Compiler.Semantics.ScopeHandling;

namespace Compiler.Semantics.TypeInformation.Types;

public abstract class BaseEnumTypeInfo(Scope scope, List<AbstractStructuralFieldTypeInfo> fields)
    : AbstractStructuralTypeInfo(scope, fields)
{
}