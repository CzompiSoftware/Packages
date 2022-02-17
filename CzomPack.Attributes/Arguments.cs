using CzomPack.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CzomPack.Attributes;

public class Arguments : IReadOnlyList<Argument>
{
    private readonly List<Argument> _arguments = new();

    public Arguments()
    {

    }

    internal Arguments(List<Argument> arguments) : base() => _arguments = arguments;

    public string[] GetArgumentList()
    {
        var args = new List<string>();

        _arguments.ForEach(arg => args.AddRange(new string[] { arg.Name, arg.Value ?? "" }));

        return args.ToArray();
    }

    public string? WithName(string name) => _arguments.First(x => x.Name.EqualsIgnoreCase(name))?.Value;
    public bool ContainsName(string name) => _arguments.Any(x => x.Name.EqualsIgnoreCase(name));

    //public static Arguments Parse(List<Argument> arguments) => new(arguments);

    public static Arguments Parse(string[] args, string delimiter)
    {
        if (args == null || args.Length == 0) return new();

        var argList = new List<Argument>();
        var _argStr = string.Join(" ", args);
        var _argList = _argStr.Split(separator: delimiter);
        for (int i = 0; i < _argList.Length; i++)
        {
            if (string.IsNullOrEmpty(_argList[i]) || string.IsNullOrWhiteSpace(_argList[i])) continue;
            List<string> _args = _argList[i].Contains(' ') ? _argList[i].Split(' ').ToList() : new() { _argList[i] };
            argList.Add(new()
            {
                Name = _args[0],
                Value = _args.Count >= 2 ? string.Join(' ', _args.Skip(1)) : null
            });
        }
        return new(argList);
    }

    public Argument this[int index] => _arguments[index];

    public int Count => _arguments.Count;


    public IEnumerator<Argument> GetEnumerator() => _arguments.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _arguments.GetEnumerator();

    public override string ToString()
    {
        return $"{GetType().Name}{{{string.Join(", ", _arguments.Select(x=>x.ToString()))}}}";
    }
}
