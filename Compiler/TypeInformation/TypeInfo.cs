using Compiler.TypeInformation.Types;

namespace Compiler.TypeInformation;

public abstract class TypeInfo
{
    public static readonly TypeInfo Void = new VoidTypeInfo();
    public static readonly TypeInfo String = new StringTypeInfo();
    public static readonly TypeInfo Number = new NumberTypeInfo();
    public static readonly TypeInfo Boolean = new BooleanTypeInfo();
    public static readonly TypeInfo Unknown = new UnknownTypeInfo();
    public static readonly TypeInfo Deferred = new DeferredTypeInfo();
    public static readonly TypeInfo Object = new ObjectTypeInfo("object", []);

    public bool IsIncomplete
    {
        get
        {
            return this switch
            {
                DeferredTypeInfo or UnknownTypeInfo => true,
                GenericTypeInfo genericTypeInfo => genericTypeInfo.GenericParams.Any(x => x.TypeInfo.IsIncomplete),
                _ => false
            };
        }
    }

    public abstract bool Compare(TypeInfo other);

    public abstract bool HasDeferredTypes();

    public static bool TryGetBuiltInType(string name, out TypeInfo typeInfo)
    {
        switch (name)
        {
            case "void":
                typeInfo = Void;
                return true;
            case "string":
                typeInfo = String;
                return true;
            case "number":
                typeInfo = Number;
                return true;
            case "boolean":
                typeInfo = Boolean;
                return true;
            case "object":
                typeInfo = Object;
                return true;
            default:
                typeInfo = null;
                return false;
        }
    }

    public virtual void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitTypeInfo(this);
    }
}