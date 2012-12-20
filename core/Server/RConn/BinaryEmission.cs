using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Server.RConn
{
    /// <summary>
    /// This class stores all the logic for converting to and from byte arrays for RConn primitives.
    /// E.G. Word, Int, and Packet.
    /// </summary>
    public static class BinaryEmission
    {
        public static byte[] Emit(this UInt32 value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);

            if (!BitConverter.IsLittleEndian)
            {
                valueBytes.Reverse();
            }

            return valueBytes;
        }
        public static UInt32 BytesToUInt(this byte[] bytes, int start = 0)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToUInt32(bytes, start);
            }
            else
            {
                //
                // BitConverter will assume byte stream is same endianess as itself.
                //

                byte[] integer = new byte[4];
                System.Array.Copy(bytes, start, integer, 0, 4);
                integer.Reverse();
                return BitConverter.ToUInt32(integer, 0);
            }
        }

        public static byte[] Emit(this Word word)
        {
            byte[] sizeBytes = word.Size.Emit();
            byte[] wordBytes = word.Content.Select(c => (byte)c).ToArray();
            byte[] terminator = new byte[] { 0 };

            byte[] resultBytes = new byte[sizeBytes.Length + wordBytes.Length + 1];

            sizeBytes.CopyTo(resultBytes, 0);
            wordBytes.CopyTo(resultBytes, sizeBytes.Length);
            terminator.CopyTo(resultBytes, sizeBytes.Length + wordBytes.Length);

            return resultBytes;
        }
        public static Word BytesToWord(this byte[] bytes, int start = 0)
        {
            Word result = new Word();
            UInt32 size = bytes.BytesToUInt(start);
            result.Content = new char[size];
            System.Array.Copy(bytes, start+4, result.Content, 0, size);

            return result;
        }

        public static byte[] Emit(this Packet packet)
        {
            UInt32 isFromClientFlag = (UInt32)(packet.OrigininatesFromClient ? 1 : 0);
            UInt32 isResponseFlag = (UInt32)(packet.IsResponse ? 1 : 0);

            UInt32 SequenceValue = isFromClientFlag << 31 | isResponseFlag << 30 | packet.SequenceNumber;

            byte[] sequenceBytes = SequenceValue.Emit();
            byte[] sizeBytes = packet.Size.Emit();
            byte[] wordCountBytes = packet.WordCount.Emit();

            List<byte[]> wordBytes = new List<byte[]>();

            int wordByteCount = 0;
            foreach(Word word in packet.Words)
            {
                byte[] bytes = word.Emit();
                wordBytes.Add(bytes);
                wordByteCount += bytes.Length;
            }

            byte[] resultBytes = new byte[sequenceBytes.Length + sizeBytes.Length + wordCountBytes.Length + wordByteCount];

            sequenceBytes.CopyTo(resultBytes, 0);
            sizeBytes.CopyTo(resultBytes, sequenceBytes.Length);
            wordCountBytes.CopyTo(resultBytes, sequenceBytes.Length + sizeBytes.Length);

            wordByteCount = 0;

            foreach(byte[] bytes in wordBytes)
            {
                bytes.CopyTo(resultBytes, sequenceBytes.Length + sizeBytes.Length + wordCountBytes.Length + wordByteCount);
                wordByteCount += bytes.Length;
            }

            return resultBytes;
        }
        public static Packet BytesToPacket(this byte[] bytes, int start = 0)
        {
            UInt32 sequenceNumber = bytes.BytesToUInt(0);
            UInt32 size = bytes.BytesToUInt(4);
            UInt32 numWords = bytes.BytesToUInt(8);

            Packet result = new Packet();
            result.OrigininatesFromClient = (sequenceNumber & 0x80000000) > 0 ? true : false;
            result.IsResponse = (sequenceNumber & 0x40000000) > 0 ? true : false;

            int offset = 12;
            for (int i = 0; i < numWords; i++)
            {
                Word word = bytes.BytesToWord(offset);
                offset += (int)word.Size + 5; //Include the null terminator and the size int
                result.Words.Add(word);
            }

            return result;
        }
    }
}
