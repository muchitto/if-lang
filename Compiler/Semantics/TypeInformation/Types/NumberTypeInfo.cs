namespace Compiler.Semantics.TypeInformation.Types;

public enum NumberType
{
    Int8,
    Int16,
    Int32,
    Int64,
    UInt16,
    UInt32,
    UInt64,
    Float32,
    Float64
}

public class NumberTypeInfo(NumberType type) : FoundationalTypeInfo
{
    private static readonly Dictionary<NumberType, NumberType[]> Conversions = new()
    {
        {
            NumberType.Int8,
            [NumberType.Int16, NumberType.Int32, NumberType.Int64, NumberType.Float32, NumberType.Float64]
        },
        { NumberType.Int16, [NumberType.Int32, NumberType.Int64, NumberType.Float32, NumberType.Float64] },
        { NumberType.Int32, [NumberType.Int64, NumberType.Float32, NumberType.Float64] },
        { NumberType.Int64, [NumberType.Float64] },
        { NumberType.UInt16, [NumberType.UInt32, NumberType.UInt64, NumberType.Float32, NumberType.Float64] },
        { NumberType.UInt32, [NumberType.UInt64, NumberType.Float32, NumberType.Float64] },
        { NumberType.UInt64, [NumberType.Float64] },
        { NumberType.Float32, [NumberType.Float64] },
        { NumberType.Float64, [] } // No implicit conversions from Float64
    };

    public NumberType NumberType { get; } = type;

    public bool CanImplicitlyConvert(NumberType to)
    {
        return CanImplicitlyConvert(NumberType, to);
    }

    public static bool CanImplicitlyConvert(NumberType from, NumberType to)
    {
        if (from == to)
        {
            return true;
        }

        return Conversions.TryGetValue(from, out var convertableTypes) && convertableTypes.Contains(to);
    }

    public override string ToString()
    {
        return NumberType.ToString().ToLower();
    }

    public override void Accept(ITypeInfoVisitor visitor)
    {
        visitor.VisitNumberTypeInfo(this);
    }

    public override bool Compare(ITypeComparer comparer, TypeInfo other)
    {
        return comparer.CompareNumberTypeInfo(this, other);
    }
}