using Compiler.Semantics.TypeInformation.TypeComparer;
using Compiler.Semantics.TypeInformation.Types;

namespace Compiler.Semantics.TypeInformation.TypeVisitor;

public class IncompleteVisitor : UnknownVisitor
{
    private bool _isIncomplete;

    public bool IsIncomplete => _isIncomplete || HasUnknownTypes;

    public override DeferredTypeInfo VisitDeferredTypeInfo(DeferredTypeInfo deferredTypeInfo)
    {
        _isIncomplete = true;

        return base.VisitDeferredTypeInfo(deferredTypeInfo);
    }

    public static bool IsIncompleteType(TypeInfo typeInfo)
    {
        var visitor = new IncompleteVisitor();
        visitor.VisitTypeInfo(typeInfo);
        return visitor.IsIncomplete;
    }
}