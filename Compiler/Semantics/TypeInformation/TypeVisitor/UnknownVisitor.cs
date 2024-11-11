using Compiler.Semantics.TypeInformation.Types;

namespace Compiler.Semantics.TypeInformation.TypeVisitor;

public class UnknownVisitor : BaseTypeInfoVisitor
{
    public bool HasUnknownTypes { get; private set; }

    public override UnknownTypeInfo VisitUnknownTypeInfo(UnknownTypeInfo unknownTypeInfo)
    {
        HasUnknownTypes = true;

        return base.VisitUnknownTypeInfo(unknownTypeInfo);
    }

    public static bool HasUnknownType(TypeInfo typeInfo)
    {
        var visitor = new UnknownVisitor();
        visitor.VisitTypeInfo(typeInfo);
        return visitor.HasUnknownTypes;
    }
}