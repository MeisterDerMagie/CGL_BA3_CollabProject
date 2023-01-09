using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

/// <summary>
/// Data structures that hold our recordings
/// A bar is a list of 8 eighths and every eighth holds information whether it contains a note or not, and which instrumentID
/// </summary>

[Serializable]
public struct Eighth : INetworkSerializable, IEquatable<Eighth>
{
    public bool contains;
    public int instrumentID;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref contains);
        serializer.SerializeValue(ref instrumentID);
    }

    public bool Equals(Eighth other) => contains == other.contains && instrumentID == other.instrumentID;
    public override bool Equals(object obj) => obj is Eighth other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(contains, instrumentID);
}