using Compiler.Semantics.ScopeHandling;
using Compiler.Semantics.TypeInformation.Types;
using Compiler.Syntax.Nodes;

namespace Compiler.Semantics;

public class SemanticHandler(SemanticContext semanticContext)
{
    public readonly SemanticContext SemanticContext = semanticContext;

    public Scope CurrentScope => SemanticContext.CurrentScope;

    public Scope NewScope(ScopeType scopeType, BaseNode node)
    {
        var scope = new Scope(CurrentScope, scopeType, node);
        SemanticContext.ScopeStack.Add(scope);
        SemanticContext.AllScopes.Add(scope);
        node.Scope = scope;

        return scope;
    }

    public Scope RecallScope(Scope scope)
    {
        SemanticContext.ScopeStack.Add(scope);

        return scope;
    }

    public Scope RecallNodeScope(BaseNode baseNode)
    {
        var scope = baseNode.Scope;

        if (scope == null)
        {
            throw new Exception("could not get scope from node");
        }

        SemanticContext.ScopeStack.Add(scope);

        return scope;
    }

    public Scope PopScope()
    {
        var scope = SemanticContext.ScopeStack.Last();
        SemanticContext.ScopeStack.Remove(scope);
        return scope;
    }

    public virtual bool InScopeType(ScopeType scopeType)
    {
        return TraverseScopes(scope => scope.Type == scopeType);
    }

    public virtual bool TraverseScopes(Func<Scope?, bool> callback)
    {
        var scope = CurrentScope;

        while (scope != null)
        {
            if (callback(scope))
            {
                return true;
            }

            scope = scope.Parent;
        }

        return false;
    }


    public virtual void SetSymbol(Scope scope, Symbol symbol, bool needToBeUnique)
    {
        var foundIndex = scope.Symbols.FindIndex(s => s.Name == symbol.Name && s.Type == symbol.Type);

        if (foundIndex != -1)
        {
            if (needToBeUnique)
            {
                throw new Exception($"symbol {symbol.Name} already exists in scope");
            }

            scope.Symbols[foundIndex] = symbol;
        }
        else
        {
            scope.Symbols.Add(symbol);
        }
    }

    public virtual void SetSymbol(Symbol symbol, bool needToBeUnique)
    {
        SetSymbol(CurrentScope, symbol, needToBeUnique);
    }


    public virtual void SetSymbolParent(Symbol symbol, bool needToBeUnique)
    {
        SetSymbol(CurrentScope.Parent ?? CurrentScope, symbol, needToBeUnique);
    }

    public bool TryLookupIdentifier(string name, out Symbol symbol)
    {
        return TryLookupSymbolOfType(name, SymbolType.Identifier, out symbol);
    }

    public bool TryLookupType(string name, out Symbol symbol)
    {
        return TryLookupSymbolOfType(name, SymbolType.Type, out symbol);
    }

    public bool TryLookupSymbolOfType(string name, SymbolType type, out Symbol symbol)
    {
        var scope = CurrentScope;

        while (scope != null)
        {
            symbol = scope.Symbols.Find(s => s.Name == name && s.Type == type);

            if (symbol != null)
            {
                return true;
            }

            if (type == SymbolType.Identifier && scope.Type == ScopeType.Object)
            {
                var objectScope = ((ObjectTypeInfo)scope.AttachedNode.TypeRef.TypeInfo).Scope;

                while (objectScope != null)
                {
                    if (objectScope.AttachedNode is not ObjectDeclarationNode objectDeclarationNode)
                    {
                        break;
                    }

                    if (objectDeclarationNode.Scope != null)
                    {
                        symbol = objectDeclarationNode.Scope.Symbols.Find(s => s.Name == name && s.Type == type);

                        if (symbol != null)
                        {
                            return true;
                        }
                    }

                    if (objectDeclarationNode.BaseName?.TypeRef.TypeInfo is not ObjectTypeInfo objectTypeInfo)
                    {
                        break;
                    }

                    objectScope = objectTypeInfo.Scope;
                }
            }

            scope = scope.Parent;
        }

        symbol = null;
        return false;
    }

    public virtual bool InNodeScope(BaseNode node)
    {
        var scope = CurrentScope;

        while (scope != null)
        {
            if (scope.AttachedNode == node)
            {
                return true;
            }

            scope = scope.Parent;
        }

        return false;
    }

    public virtual bool InNodeScope(string name)
    {
        var scope = CurrentScope;

        while (scope != null)
        {
            if (scope.AttachedNode is DeclarationNode declarationNode)
            {
                if (declarationNode.Named.Name == name)
                {
                    return true;
                }
            }

            scope = scope.Parent;
        }

        return false;
    }

    public virtual bool TryGetInNodeScope(string name, SymbolType symbolType, out Symbol symbol)
    {
        var scope = CurrentScope;

        while (scope != null)
        {
            if (scope.AttachedNode is DeclarationNode declarationNode)
            {
                if (declarationNode.Named.Name == name)
                {
                    if (scope.Parent == null)
                    {
                        symbol = null;

                        return false;
                    }

                    if (scope.Parent.Symbols.Find(s => s.Name == name && s.Type == symbolType) is { } foundSymbol)
                    {
                        symbol = foundSymbol;
                        return true;
                    }
                }
            }

            scope = scope.Parent;
        }

        symbol = null;

        return false;
    }

    public virtual bool TryGetScopeOfType(ScopeType scopeType, out Scope scope)
    {
        var currentScope = CurrentScope;

        while (currentScope != null)
        {
            if (currentScope.Type == scopeType)
            {
                scope = currentScope;
                return true;
            }

            currentScope = currentScope.Parent;
        }

        scope = null;
        return false;
    }
}