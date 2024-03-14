using GeneratorHelpersLibrary;
using Microsoft.CodeAnalysis;

// Assume solutionPath is the path to your .sln file
namespace HelperGenerators;

public static class BaseVisitorGenerator
{
    public static void Generate(string location)
    {
        var solution = SyntaxTreeExtensions.GetSolutionFromPath(location);

        if (solution == null)
        {
            throw new Exception("HEAHAE");
        }

        var project = solution.GetProject("Compiler");

        if (project == null)
        {
            throw new Exception("HEAHAE");
        }

        var baseNodeClassPath = "Compiler.Syntax.Nodes.BaseNode";

        var baseNodeClass = project.FindTypeByString(baseNodeClassPath)?.IsOfType<INamedTypeSymbol>();

        if (baseNodeClass == null)
        {
            throw new Exception("HEAHAE");
        }

        var classDeclarations = project.GetClassDeclarationsWithAncestor(baseNodeClassPath);

        foreach (var classDeclaration in classDeclarations)
        {
            var members = classDeclaration.GetProperties();

            foreach (var member in members)
            {
                if (member.IsPropertyTypeDerivedFromClass(project, baseNodeClassPath))
                {
                    Console.WriteLine(member);
                }
            }

            Console.WriteLine("HWEAHEYH");
        }
    }
}