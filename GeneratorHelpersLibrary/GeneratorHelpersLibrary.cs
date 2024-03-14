using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;

namespace GeneratorHelpersLibrary;

public static partial class SyntaxTreeExtensions
{
    public static Solution? GetSolutionFromPath(string solutionPath)
    {
        var workspace = MSBuildWorkspace.Create();
        var solution = workspace.OpenSolutionAsync(solutionPath);
        solution.Wait();
        return solution.Result;
    }

    public static Project? GetProject(this Solution solution, string projectName)
    {
        return solution.Projects.FirstOrDefault(p => p.Name == projectName);
    }

    public static Compilation? GetCompilation(this Project project)
    {
        var compilation = project.GetCompilationAsync();
        compilation.Wait();
        return compilation.Result;
    }

    public static IEnumerable<INamedTypeSymbol> FindDerivedClasses(this INamedTypeSymbol typeSymbol,
        Solution solution)
    {
        var result = SymbolFinder.FindDerivedClassesAsync(typeSymbol, solution);
        result.Wait();
        return result.Result;
    }

    public static IEnumerable<INamedTypeSymbol> FindDerivedClasses(this INamedTypeSymbol typeSymbol,
        Project project)
    {
        return FindDerivedClasses(typeSymbol, project.Solution);
    }

    public static IEnumerable<INamedTypeSymbol> FindDerivedClassesByString(this Project project, string typeSymbol)
    {
        var compilation = project.GetCompilation();
        var baseNodeSymbol = compilation.GetTypeByMetadataName(typeSymbol);
        return FindDerivedClasses(baseNodeSymbol, project);
    }

    public static IEnumerable<INamedTypeSymbol> FindDerivedClasses(this Project project, INamedTypeSymbol typeSymbol)
    {
        return FindDerivedClasses(typeSymbol, project);
    }

    // Get derived classes that have the needed ancestor
    public static IEnumerable<INamedTypeSymbol> FindDerivedClassesWithAncestors(this Project project, string typeSymbol)
    {
        var compilation = project.GetCompilation();
        if (compilation == null)
        {
            throw new Exception("Could not get compilation");
        }

        var baseNodeSymbol = compilation.GetTypeByMetadataName(typeSymbol);

        if (baseNodeSymbol == null)
        {
            throw new Exception("Could not get base node symbol");
        }

        return project.FindDerivedClassesWithAncestors(baseNodeSymbol);
    }

    // Get derived classes that have the needed ancestor
    public static IEnumerable<INamedTypeSymbol> FindDerivedClassesWithAncestors(this Project project,
        INamedTypeSymbol typeSymbol)
    {
        var compilation = project.GetCompilation();

        if (compilation == null)
        {
            throw new Exception("Could not get compilation");
        }

        // Get ALL classes first
        var allClasses = compilation.GlobalNamespace.GetNamespaceMembers()
            .SelectMany(n => n.GetMembers().OfType<INamedTypeSymbol>());

        foreach (var namedTypeSymbol in allClasses)
        {
            Console.WriteLine(namedTypeSymbol.Name);
        }

        // Then filter out the ones that have the needed ancestor
        return allClasses.Where(c => c.AllInterfaces.Contains(typeSymbol));
    }

    // Get properties of a class
    public static IEnumerable<IPropertySymbol> GetProperties(this INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers().Where(m => m.Kind is SymbolKind.Property).Select(m => (IPropertySymbol)m);
    }

    // Get fields of a class
    public static IEnumerable<IFieldSymbol> GetFields(this INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers().Where(m => m.Kind is SymbolKind.Field).Select(m => (IFieldSymbol)m);
    }

    // Get the clean name of a property
    public static string GetCleanName(this IPropertySymbol propertySymbol)
    {
        return propertySymbol.Name;
    }

    public static TResult IsOfType<TResult>(this ITypeSymbol obj)
    {
        if (obj is not TResult result)
        {
            throw new Exception($"Object is not of type {typeof(TResult).Name}");
        }

        return result;
    }

    public static ITypeSymbol? FindTypeByString(this Project project, string typeName)
    {
        return project.GetCompilation()?.GetTypeByMetadataName(typeName);
    }
}