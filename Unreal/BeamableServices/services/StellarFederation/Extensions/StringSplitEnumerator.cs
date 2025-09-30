using System;

namespace Beamable.StellarFederation.Extensions;

public ref struct StringSplitEnumerator
{
    private ReadOnlySpan<char> _span;
    private readonly char _separator;
    private int _start;
    private bool _finished;

    public StringSplitEnumerator(ReadOnlySpan<char> span, char separator)
    {
        _span = span;
        _separator = separator;
        _start = 0;
        _finished = span.IsEmpty;
    }

    public ReadOnlySpan<char> Current { get; private set; } = default;

    public bool MoveNext()
    {
        if (_finished) return false;

        var remaining = _span[_start..];
        var separatorIndex = remaining.IndexOf(_separator);

        if (separatorIndex == -1)
        {
            Current = remaining;
            _finished = true;
            return !Current.IsEmpty;
        }

        Current = remaining[..separatorIndex];
        _start += separatorIndex + 1;
        return true;
    }

    public StringSplitEnumerator GetEnumerator() => new(_span, _separator);
}