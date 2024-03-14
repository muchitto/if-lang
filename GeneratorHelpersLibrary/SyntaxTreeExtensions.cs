using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GeneratorHelpersLibrary;

public static partial class SyntaxTreeExtensions
{
    public static SyntaxTree GetSyntaxTree(this Document document)
    {
        var syntaxTree = document.GetSyntaxTreeAsync();
        syntaxTree.Wait();
        return syntaxTree.Result;
    }

    public static SemanticModel GetSemanticModel(this Document document)
    {
        var semanticModel = document.GetSemanticModelAsync();
        semanticModel.Wait();
        return semanticModel.Result;
    }

    public static IEnumerable<SyntaxTree> GetSyntaxTrees(this Project project)
    {
        var documents = project.Documents;
        return documents.Select(d => d.GetSyntaxTree());
    }

    public static IEnumerable<ClassDeclarationSyntax> GetClassDeclarations(this SyntaxTree syntaxTree)
    {
        var root = syntaxTree.GetRoot();
        return root.DescendantNodes().OfType<ClassDeclarationSyntax>();
    }

    public static IEnumerable<ClassDeclarationSyntax> GetClassDeclarations(this Document document)
    {
        return document.GetSyntaxTree().GetClassDeclarations();
    }

    public static IEnumerable<ClassDeclarationSyntax> GetClassDeclarations(this Project project)
    {
        var documents = project.Documents;
        var classes = new List<ClassDeclarationSyntax>();
        foreach (var document in documents)
        {
            classes.AddRange(document.GetClassDeclarations());
        }

        return classes;
    }

    public static IEnumerable<ClassDeclarationSyntax> GetClassDeclarationsWithAncestor(this Project project,
        ClassDeclarationSyntax ancestor)
    {
        return project.GetClassDeclarationsWithAncestor(ancestor.Identifier.Text);
    }

    public static bool HasAncestorClass(this ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel,
        string ancestorFullName)
    {
        var baseTypeSyntax = classDeclaration.BaseList?.Types.FirstOrDefault();
        if (baseTypeSyntax == null)
        {
            return false;
        }

        var baseTypeSymbol = semanticModel.GetTypeInfo(baseTypeSyntax.Type).Type as INamedTypeSymbol;
        while (baseTypeSymbol != null)
        {
            // Get the fully qualified name including the namespace
            var baseTypeFullName = baseTypeSymbol.ContainingNamespace + "." + baseTypeSymbol.Name;

            // Normalize namespace prefix in case of global:: prefix
            baseTypeFullName = baseTypeFullName.Replace("global::", "");

            if (baseTypeFullName == ancestorFullName)
            {
                return true;
            }

            baseTypeSymbol = baseTypeSymbol.BaseType;
        }

        return false;
    }

    public static IEnumerable<ClassDeclarationSyntax> GetClassDeclarationsWithAncestor(
        this Project project,
        string ancestorFullName
    )
    {
        var classesWithAncestor = new List<ClassDeclarationSyntax>();
        foreach (var document in project.Documents)
        {
            var model = document.GetSemanticModel();
            var classDeclarations = document.GetSyntaxTree().GetClassDeclarations();

            foreach (var classDeclaration in classDeclarations)
            {
                if (classDeclaration.HasAncestorClass(model, ancestorFullName))
                {
                    classesWithAncestor.Add(classDeclaration);
                }
            }
        }

        return classesWithAncestor;
    }

    public static IEnumerable<PropertyDeclarationSyntax> GetProperties(
        this ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Members.OfType<PropertyDeclarationSyntax>();
    }

    public static IEnumerable<MethodDeclarationSyntax> GetMethods(
        this ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Members.OfType<MethodDeclarationSyntax>();
    }

    public static IEnumerable<FieldDeclarationSyntax> GetFields(
        this ClassDeclarationSyntax classDeclarationSyntax)
    {
        return classDeclarationSyntax.Members.OfType<FieldDeclarationSyntax>();
    }

    public static bool IsPropertyTypeDerivedFromClass(this PropertyDeclarationSyntax property,
        SemanticModel semanticModel, string classFullName)
    {
        // Get the property type symbol
        var typeInfo = semanticModel.GetTypeInfo(property.Type);
        var typeSymbol = typeInfo.Type;

        // Check if the type is a collection
        if (typeSymbol.IsCollectionType(out var elementTypeSymbol))
        {
            // For collections, we check the element type
            return IsTypeOrDerivedFromClass(elementTypeSymbol, classFullName, semanticModel);
        }

        // For non-collection types, we check the type itself
        return IsTypeOrDerivedFromClass(typeSymbol, classFullName, semanticModel);
    }

    public static bool IsPropertyTypeDerivedFromClass(this PropertyDeclarationSyntax property, Document document,
        string classFullName)
    {
        var semanticModel = document.GetSemanticModel();
        return property.IsPropertyTypeDerivedFromClass(semanticModel, classFullName);
    }

    public static bool IsPropertyTypeDerivedFromClass(
        this PropertyDeclarationSyntax property,
        Project project,
        string classFullName
    )
    {
        var documents = project.Documents;

        return documents.Any(document => property.IsPropertyTypeDerivedFromClass(document, classFullName));
    }

    private static bool IsCollectionType(this ITypeSymbol typeSymbol, out ITypeSymbol elementTypeSymbol)
    {
        elementTypeSymbol = null;
        if (typeSymbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol)
        {
            // Assuming System.Collections.Generic.IEnumerable<T> is a good proxy for collection types
            var iEnumerableInterface =
                namedTypeSymbol.AllInterfaces.FirstOrDefault(i => i.MetadataName == "IEnumerable`1");
            if (iEnumerableInterface != null)
            {
                elementTypeSymbol = iEnumerableInterface.TypeArguments[0];
                return true;
            }
        }

        return false;
    }

    private static bool IsTypeOrDerivedFromClass(ITypeSymbol typeSymbol, string classFullName,
        SemanticModel semanticModel)
    {
        while (typeSymbol != null)
        {
            if (typeSymbol.GetFullTypeName() == classFullName)
            {
                return true;
            }

            typeSymbol = typeSymbol.BaseType;
        }

        return false;
    }

    private static string GetFullTypeName(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.ContainingNamespace + "." + typeSymbol.Name;
    }
}