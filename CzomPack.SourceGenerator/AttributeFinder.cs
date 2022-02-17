using CzomPack.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CzomPack.SourceGenerator;

public class AttributeFinder : ISyntaxReceiver
{
    public AttributeFinder(string name)
    {
        Name = name;
    }
    public List<ClassDeclarationSyntax> Classes { get; }
        = new();
    public string Name { get; }

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax @class)
        {
            if (@class.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString().EqualsIgnoreCase($"{Name}Attribute") || a.Name.ToString().EqualsIgnoreCase($"{Name}"))))
            {
                Classes.Add(@class);
            }
        }
    }
}