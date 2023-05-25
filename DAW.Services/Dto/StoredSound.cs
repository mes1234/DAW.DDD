using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DAW.Services.Dto;

public record StoredSound(byte[] Content, string Name, Guid SourceId, int Pitch);

public static class StoredSoundExtensions
{
    public static Guid GetStoredId(Guid sourceId, int pitch)
    {
        var first = sourceId.ToByteArray();
        var second = BitConverter.GetBytes(pitch);

        byte[] ret = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);


        return new Guid(MD5.HashData(ret));
    }
}
