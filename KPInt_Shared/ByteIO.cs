using KPInt_Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

public class ByteArrayReader
{
    public int Length => _array.Length - _index;

    private readonly byte[] _array;
    private int _index;

    public ByteArrayReader(byte[] value)
    {
        _array = value;
        _index = 0;
    }

    public int ReadInt32()
    {
        var res = BitConverter.ToInt32(_array, _index);
        _index += 4;
        return res;
    }

    public string ReadString()
    {
        var len = ReadInt32();
        var res = Encoding.UTF8.GetString(_array, _index, len);
        _index += len;
        return res;
    }

    public byte ReadByte()
    {
        var res = _array[_index];
        _index++;
        return res;
    }

    public byte[] ReadBytes(int length)
    {
        var res = new byte[length];
        Array.Copy(res, 0, _array, _index, length);
        _index += length;
        return res;
    }

    public ColorLine ReadColorLine()
    {
        return new ColorLine(
            new Point(ReadInt32(), ReadInt32()),
            new Point(ReadInt32(), ReadInt32()),
            Color.FromRgb(ReadByte(), ReadByte(), ReadByte()),
            ReadByte());
    }
}

public class ByteArrayWriter
{
    private readonly List<byte> _array = new List<byte>();

    public byte[] Array => _array.ToArray();

    public ByteArrayWriter Append(int value)
    {
        _array.AddRange(BitConverter.GetBytes(value));
        return this;
    }

    public ByteArrayWriter Append(string value)
    {
        var msg = Encoding.UTF8.GetBytes(value);
        Append(msg.Length);
        Append(msg);
        return this;
    }

    public ByteArrayWriter Append(byte value)
    {
        _array.Add(value);
        return this;
    }

    public ByteArrayWriter Append(byte[] value)
    {
        _array.AddRange(value);
        return this;
    }

    public ByteArrayWriter Append(ColorLine colorLine)
    {
        Append((int)colorLine.Start.X);
        Append((int)colorLine.Start.Y);
        Append((int)colorLine.End.X);
        Append((int)colorLine.End.Y);
        Append(colorLine.Color.R);
        Append(colorLine.Color.G);
        Append(colorLine.Color.B);
        Append(colorLine.Thickness);
        return this;
    }
}
