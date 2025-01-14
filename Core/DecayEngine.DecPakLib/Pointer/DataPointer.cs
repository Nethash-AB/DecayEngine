using System;
using System.IO;
using System.IO.Compression;
using DecayEngine.DecPakLib.Package;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Pointer
{
    [ProtoContract(AsReferenceDefault = true)]
    public class DataPointer
    {
        [ProtoMember(1)]
        public string SourcePath { get; set; }
        [ProtoIgnore]
        public string FullSourcePath { get; set; }
        [ProtoMember(2, AsReference = true)]
        public DataPackage Package { get; set; }
        [ProtoMember(3)]
        public long Offset { get; set; }
        [ProtoMember(4)]
        public long Size { get; set; }
        [ProtoIgnore]
        public bool Valid => Size > 0;

        public MemoryStream GetData()
        {
            if (Package == null)
            {
                throw new Exception("Invalid Resource Bundle. Did you Load/Save the bundle before trying to load data?");
            }

            using (MemoryStream bufferStream = new MemoryStream())
            {
                Stream pkgStream = Package.Read();

                #region findInPackage

                if (!Package.PackageStreamer.CanSeek)
                {
                    MemoryStream seekWorkaroundStream = new MemoryStream();
                    pkgStream.CopyTo(seekWorkaroundStream);
                    pkgStream.Close();
                    pkgStream = seekWorkaroundStream;
                }
                pkgStream.Seek(Offset, SeekOrigin.Begin);

                long remainingBytes = Size;

                byte[] buffer = new byte[4096];
                do
                {
                    int amountOfBytesToRead = buffer.Length;
                    if (amountOfBytesToRead > remainingBytes) amountOfBytesToRead = (int) remainingBytes;

                    int count = pkgStream.Read(buffer, 0, amountOfBytesToRead);
                    if (count > 0)
                    {
                        bufferStream.Write(buffer, 0, count);
                        remainingBytes -= count;
                    }
                    else
                    {
                        break;
                    }
                } while (remainingBytes > 0);
                pkgStream.Close();

                #endregion

                bufferStream.Position = 0;
                using (GZipStream decompressorStream = new GZipStream(bufferStream, CompressionMode.Decompress))
                {
                    MemoryStream decompressedDataStream = new MemoryStream();
                    decompressorStream.CopyTo(decompressedDataStream);
                    decompressedDataStream.Position = 0;

                    return decompressedDataStream;
                }
            }
        }

        public byte[] GetDataAsByteArray()
        {
            byte[] data;
            using (MemoryStream ms = GetData())
            {
                data = ms.ToArray();
            }

            return data;
        }
    }
}