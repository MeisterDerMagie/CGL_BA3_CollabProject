using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

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

[Serializable]
public struct Bar
{
    public List<Eighth> eighth;

    public Bar(List<Eighth> _n)
    {
        eighth = new List<Eighth>();
        eighth = _n;
    }

}
