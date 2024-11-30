using Compiler.Semantics.ScopeHandling;

namespace Compiler.Semantics.TypeInformation.Types;

public abstract class AbstractStructuralTypeInfo(Scope scope, List<AbstractStructuralFieldTypeInfo> fields)
    : ScopedTypeInfo(scope)
{
    public IReadOnlyList<AbstractStructuralFieldTypeInfo> Fields { get; set; } = fields;

    public void SetFieldType(AbstractStructuralFieldTypeInfo fieldTypeInfo)
    {
        var newList = new List<AbstractStructuralFieldTypeInfo>();

        var added = false;

        foreach (var field in Fields)
        {
            if (field.Name == fieldTypeInfo.Name)
            {
                newList.Add(fieldTypeInfo);
                added = true;
                continue;
            }

            newList.Add(field);
        }

        if (!added)
        {
            newList.Add(fieldTypeInfo);
        }

        Fields = newList;
    }

    public void SetFieldType(string name, TypeRef typeRef)
    {
        SetFieldType(new AbstractStructuralFieldTypeInfo(name, typeRef));
    }

    public AbstractStructuralFieldTypeInfo? GetField(string name)
    {
        foreach (var field in Fields)
        {
            if (field.Name == name)
            {
                return field;
            }
        }

        return null;
    }
}