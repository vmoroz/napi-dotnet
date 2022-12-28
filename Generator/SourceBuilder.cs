using System;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace NodeApi.Generator;

internal class SourceBuilder : SourceText
{
    private readonly StringBuilder _text;

    public SourceBuilder(string indent = "\t")
    {
        _text = new StringBuilder();
        Indent = indent;
    }

    public override Encoding? Encoding => Encoding.UTF8;

    public override int Length => _text.Length;

    public override char this[int position] => _text[position];

    public override void CopyTo(
        int sourceIndex, char[] destination, int destinationIndex, int count)
    {
        _text.CopyTo(sourceIndex, destination, destinationIndex, count);
    }

    public override string ToString() => _text.ToString();

    public string Indent { get; }

    public int CurrentIndent { get; private set; } = 0;

    public SourceBuilder IncreaseIndent()
    {
        CurrentIndent++;
        return this;
    }

    public SourceBuilder DecreaseIndent()
    {
        if (CurrentIndent == 0)
        {
            throw new InvalidOperationException("Imbalanced unindent.");
        }

        CurrentIndent--;
        return this;
    }

    public SourceBuilder Append(string text)
    {
        _text.Append(text);
        return this;
    }

    public SourceBuilder AppendLine(string line)
    {
        if (line.StartsWith("}"))
        {
            DecreaseIndent();
        }

        if (line.Length > 0)
        {
            AppendIndent();
        }

        _text.AppendLine(line);

        if (line.EndsWith("{"))
        {
            IncreaseIndent();
        }

        return this;
    }

    public SourceBuilder AppendIndent()
    {
        for (int i = 0; i < CurrentIndent; ++i)
        {
            _text.Append(Indent);
        }

        return this;
    }

    public static SourceBuilder operator +(SourceBuilder s, string line)
    {
        return s.AppendLine(line);
    }

    public static SourceBuilder operator ++(SourceBuilder s)
    {
        return s.AppendLine(string.Empty);
    }
}
