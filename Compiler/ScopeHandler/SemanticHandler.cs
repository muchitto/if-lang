using Compiler.Syntax.Nodes;

namespace Compiler.ScopeHandler;

public class SemanticHandler(SemanticContext semanticContext)
{
    protected SemanticContext SemanticContext = semanticContext;

    public Scope CurrentScope => SemanticContext.ScopeStack.Last();

    public bool InGlobalScope => CurrentScope.Parent == null;

    public bool CanSetAlreadyDeclaredSymbol => InGlobalScope || InScopeType(ScopeType.Object);

    public virtual Scope? NewScope(ScopeType scopeType, BaseNode node)
    {
        var parentScope = SemanticContext.ScopeStack.Count > 0 ? SemanticContext.ScopeStack.Last() : null;
        var scope = new Scope(
            parentScope,
            scopeType,
            node
        );
        SemanticContext.ScopeStack.Add(scope);
        SemanticContext.AllScopes.Add(scope);
        node.Scope = scope;
        return scope;
    }

    public virtual Scope? RecallNodeScope(BaseNode node)
    {
        var scope = node.Scope;

        if (scope == null)
        {
            throw new Exception("Node has no scope");
        }

        SemanticContext.ScopeStack.Add(scope);

        return scope;
    }


    public virtual void SetSymbol(Symbol symbol, bool needToBeUnique)
    {
        var foundIndex = CurrentScope.Symbols.FindIndex(s => s.Name == symbol.Name && s.Type == symbol.Type);

        if (foundIndex != -1)
        {
            if (needToBeUnique)
            {
                throw new Exception($"symbol {symbol.Name} already exists in scope");
            }

            CurrentScope.Symbols[foundIndex] = symbol;
        }
        else
        {
            CurrentScope.Symbols.Add(symbol);
        }
    }

    public virtual Scope? PopScope()
    {
        var scope = SemanticContext.ScopeStack.Last();
        SemanticContext.ScopeStack.RemoveAt(SemanticContext.ScopeStack.Count - 1);
        return scope;
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
            var foundSymbol = scope.Symbols.Find(s => s.Name == name && s.Type == type);

            if (foundSymbol != null)
            {
                symbol = foundSymbol;
                return true;
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
                if (declarationNode.Name.Name == name)
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
                if (declarationNode.Name.Name == name)
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

    public virtual bool InScopeType(ScopeType scopeType)
    {
        return TraverseScopes(scope => scope.Type == scopeType);
    }
}