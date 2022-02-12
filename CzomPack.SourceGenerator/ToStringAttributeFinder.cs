using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CzomPack.SourceGenerator;

public class ToStringAttributeFinder : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> Classes { get; }
        = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax @class)
        {
            //{string.Join("; ", @class.AttributeLists.Select(x=> string.Join(", ", x.Attributes.Select())))}
            if (@class.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString().Equals("ToStringAttribute") || a.Name.ToString().Equals("ToString"))))
            {
                Classes.Add(@class);
            }
        }
    }
}