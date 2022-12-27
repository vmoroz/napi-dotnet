using System;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace NodeApi.Generator;

internal class SourceBuilder : SourceText
{
    private readonly StringBuilder _text;
    private string _currentIndent = string.Empty;

    public SourceBuilder(string indent = "\t", bool autoIndent = true)
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

    public bool AutoIndent { get; }

    public void IncreaseIndent()
    {
        _currentIndent += Indent;
    }

    public void DecreaseIndent()
    {
        if (_currentIndent.Length == 0)
        {
            throw new InvalidOperationException("Imbalanced unindent.");
        }

        _currentIndent = _currentIndent.Substring(0, _currentIndent.Length - Indent.Length);
    }

    private void AppendLine(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            _text.AppendLine("");
            return;
        }

        string[] lines = line.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        foreach (string text in lines)
        {
            if (line.StartsWith("}") && AutoIndent)
            {
                DecreaseIndent();
            }

            if (text.Length > 0)
            {
                _text.Append(_currentIndent);
            }

            _text.AppendLine(text);

            if (line.EndsWith("{") && AutoIndent)
            {
                IncreaseIndent();
            }
        }
    }

    public static SourceBuilder operator +(SourceBuilder s, string line)
    {
        s.AppendLine(line);
        return s;
    }

    public static SourceBuilder operator ++(SourceBuilder s)
    {
        s.AppendLine(string.Empty);
        return s;
    }
}
