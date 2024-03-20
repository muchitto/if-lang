using Compiler.Semantics.TypeInformation.Types;

namespace Compiler.Semantics.TypeInformation;

public abstract class TypeInfo
{
    public static readonly TypeInfo Void = new VoidTypeInfo();
    public static readonly TypeInfo String = new StringTypeInfo();
    public static readonly TypeInfo Int = new NumberTypeInfo(NumberType.Int32);
    public static readonly TypeInfo Int16 = new NumberTypeInfo(NumberType.Int16);
    public static readonly TypeInfo Int32 = new NumberTypeInfo(NumberType.Int32);
    public static readonly TypeInfo Int64 = new NumberTypeInfo(NumberType.Int64);
    public static readonly TypeInfo UInt16 = new NumberTypeInfo(NumberType.UInt16);
    public static readonly TypeInfo UInt32 = new NumberTypeInfo(NumberType.UInt32);
    public static readonly TypeInfo UInt64 = new NumberTypeInfo(NumberType.UInt64);
    public static readonly TypeInfo Float = new NumberTypeInfo(NumberType.Float64);
    public static readonly TypeInfo Float32 = new NumberTypeInfo(NumberType.Float32);
    public static readonly TypeInfo Float64 = new NumberTypeInfo(NumberType.Float64);


    public static readonly TypeInfo Boolean = new BooleanTypeInfo();
    public static readonly TypeInfo Unknown = new UnknownTypeInfo();
    public static readonly TypeInfo Deferred = new DeferredTypeInfo();
    public static readonly TypeInfo Object = new ObjectTypeInfo(null, "object", [], null);
    public static readonly TypeRef ObjectRef = new(Object);

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
            case "int":
                typeInfo = Int;
                return true;
            case "int16":
                typeInfo = Int16;
                return true;
            case "int32":
                typeInfo = Int32;
                return true;
            case "int64":
                typeInfo = Int64;
                return true;
            case "uint16":
                typeInfo = UInt16;
                return true;
            case "uint32":
                typeInfo = UInt32;
                return true;
            case "uint64":
                typeInfo = UInt64;
                return true;
            case "float":
                typeInfo = Float;
                return true;
            case "float32":
                typeInfo = Float32;
                return true;
            case "float64":
                typeInfo = Float64;
                return true;
            case "bool":
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