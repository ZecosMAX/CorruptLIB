﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZecosMAX.Corrupt
{
    [Serializable]
    public class YouAreNotASmartKidException : Exception
    {
        public YouAreNotASmartKidException() { }
        public YouAreNotASmartKidException(string message) : base(message) { }
        public YouAreNotASmartKidException(string message, Exception inner) : base(message, inner) { }
        protected YouAreNotASmartKidException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    struct CorruptState
    {
        private string message;
        private int errorCode;
        private int bytesChanged;

        public string Message { get => message; }
        public int ErrorCode { get => errorCode; }
        public int BytesChanged { get => bytesChanged; }

        public CorruptState(string message, int ec, int bc)
        {
            this.message = message;
            this.errorCode = ec;
            this.bytesChanged = bc;
        }
    }
    abstract class Corruptor
    {

    }
    class PNGCorruptor : Corruptor
    {
        private PNGChunk[] chunks;
        private PNGCorruptType corruptType;

        public PNGChunk[] Chunks { get => chunks; set => chunks = value; }
        public PNGCorruptType CorruptType { get => corruptType; set => corruptType = value; }

        static void Swap<T>(int indexA, int indexB, T[] arr)
        {
            var temp = arr[indexA];
            arr[indexA] = arr[indexB];
            arr[indexB] = temp;
        }

        public PNGCorruptor()
        {
            Corrupt(PNGCorruptType.Slide);
        }
        /// <summary>
        /// An main method to corrupt PNG
        /// </summary>
        /// <param name="corruptType">Required parameter to specify type of corrupt</param>
        /// <param name="additional"></param>
        public CorruptState Corrupt(PNGCorruptType corruptType, params int[] additional)
        {
            CorruptState res;
            switch (corruptType)
            {
                case PNGCorruptType.ForwardSwap:
                    break;
                case PNGCorruptType.BackwardSwap:
                    break;
                case PNGCorruptType.ManualSwap:
                    break;
                case PNGCorruptType.AutoSwap:
                    break;
                case PNGCorruptType.Slide:
                    break;
                case PNGCorruptType.SingleChange:
                    foreach (var item in chunks)
                    {
                        if (additional != null)
                        {
                            //somehow parse params in additional
                        }
                        else
                        {
                            if (new string(item.Type).ToLower() == "idat") {
                                item.Change();
                                item.RecalcCrc();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            res = new CorruptState("", 0, 0);
            return res;
        }
        private void SlideCorrupt(int offset, int numOfIDAT = 1)
        {

        }
        private void ChangeCorrupt()
        {

        }
        private void SwapCorrupt(int first, int second)
        {

        }
    }
    /// <summary>
    /// A class which represents a PNG chunk
    /// <para/>Guide to PNG <seealso cref="https://habrahabr.ru/post/130472/"/>
    /// </summary> 
    public class PNGChunk
    {
        static uint[] crcTable;
        private int length;  //Chunk length
        private char[] type; //4 ASCII Symbols
        private byte[] data; //a chunk data with presented length
        private byte[] cRC;  //a crc32-checksum

        public int Length { get => length; set => length = value; }
        public char[] Type { get => type; set => type = value; }
        public byte[] Data { get => data; set => data = value; }
        public byte[] CRC { get => cRC; set => cRC = value; }

        /// <summary>
        /// Constructor without parameters
        /// </summary>
        public PNGChunk()
        {
            //that was added for lulz, you really thought you can make a chunk without data and fuck up entire system?
            throw new YouAreNotASmartKidException("You're not able to create a chunk without parameters");
        }
        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="length"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="CRC"></param>
        public PNGChunk(int length, char[] type, byte[] data, byte[] CRC = null)
        {
            this.length = length;
            this.type = type;
            this.data = data;
            this.CRC = CRC;
        }
        private decimal extendmod(decimal left, decimal right)
        {
            //for neg (right - Math.Abs(left % right)) % right
            //for pos (left % right)
            if (left < 0)
                return ((right - Math.Abs(left % right)) % right);
            else
                return (left % right);
        }
        /// <summary>
        /// A method, that returns array of strings, containing it's(chunk's) information
        /// </summary>
        /// <param name="isData">a parameter to decide whether the data should be showed or not</param>
        /// <returns></returns>
        public String[] Show(bool isData = false)
        {

            List<String> res = new List<string>();//res.Add(string.Fromat(
            res.Add(string.Format("Length: {0}", length));
            res.Add(string.Format("type: {0}", new String(type)));
            if(isData)
                res.Add(string.Format("Data: {0}", data.Aggregate("", (s, b) => s += $"{b:X2} ")));
            res.Add(string.Format("CRC: {0}", CRC.Aggregate("", (s, b) => s += $"{b:X2}")));

            return res.ToArray();
        }
        UInt32 crc(byte[] source)
        {
            UInt32[] crc_table = new UInt32[256];
            UInt32 crc;

            for (UInt32 i = 0; i < 256; i++)
            {
                crc = i;
                for (UInt32 j = 0; j < 8; j++)
                    crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xEDB88320 : crc >> 1;

                crc_table[i] = crc;
            };

            crc = 0xFFFFFFFF;

            foreach (byte s in source)
            {
                crc = crc_table[(crc ^ s) & 0xFF] ^ (crc >> 8);
            }

            crc ^= 0xFFFFFFFF;

            return crc;
        }
        /// <summary>
        /// A method to slide bytes in chunk data array.
        /// So the next array, with parameter of offset of 1 becomes:
        /// <para/>0-1-2-3-4 --> 4-0-1-2-3
        /// </summary>
        /// <param name="off">an offset to slide, if offset is equal to array size or is a multiple of its size by an integer, then literally nothing happens</param>
        public void Slide(int off)
        {
            for (int x = 0; x < off; x++)
            {
                byte mem = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    if (i == 0)
                    {
                        mem = data[i];
                        data[i] = data[(int)extendmod(i - 1, data.Length)];
                    }
                    else
                    {
                        byte tmp = mem;
                        mem = data[i];
                        data[i] = tmp;
                    }
                }
            }
        }
        /// <summary>
        /// A method to change one particular byte to random another
        /// </summary>
        /// <param name="off">offset. really should be called a "position", but i tak soidyot<para/>if offset is set, then it changes byte at that position</param>
        public void Change(int off = -1)
        {
            Random random = new Random();
            byte newb = 0;
            if (off == -1)
                data[random.Next(0, data.Length)] = (byte)random.Next(0, 255);
            else
                data[off] = (byte)random.Next(0, 255);
        }
        /// <summary>
        /// Recalculates CRC32-Checksum of a chunk, really, you don't have to use that, corruptor class make that care of it for you, but if you want, you can, but this'll be a waste of perfomance
        /// </summary>
        public void RecalcCrc()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Encoding.ASCII.GetBytes(type));
            bytes.AddRange(data);
            var res = crc(bytes.ToArray());
            //Console.WriteLine("{0:X}", res);
            CRC = BitConverter.GetBytes(res).Reverse().ToArray();
            //Console.WriteLine(CRC.Aggregate("", (s, b) => s += $"{b:X2}"));
        }
    }
    /// <summary>
    /// A class to parse structure of differrent types of image
    /// </summary>
    public class Parser
    {
        private Corruptor corruptorInstance;
        private string path;

        internal Corruptor CorruptorInstance { get => corruptorInstance; set => corruptorInstance = value; }

        /// <summary>
        /// A constructor of that magnificent class
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        public Parser(String path, ImageType imageType)
        {
            this.path = path;
            switch (imageType)
            {
                case ImageType.JPG:
                    break;
                case ImageType.PNG:
                    this.corruptorInstance = new PNGCorruptor();
                    break;
                case ImageType.BMP:
                    break;
                case ImageType.TIFF:
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// A method to parse a PNG file.
        /// </summary>
        public List<PNGChunk> ParsePNG()
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            int hexIn;

            List<Chunk> Chunks = new List<Chunk>();

            byte[] hex = new byte[fs.Length];

            for (int i = 0; (hexIn = fs.ReadByte()) != -1; i++)
            {
                //hex += byte.Format("{0:X2}", hexIn);
                hex[i] = (byte)hexIn;
                //Console.Write(Convert.ToString((int)hex[i],16) + " ");
            }
            long len = fs.Length;
            fs.Close();

            int offset = 8; // PNG Signature
            for (int i = offset; i < hex.Length; i = offset) //needs to handle 1 chunk
            {
                Console.Write(i);
                byte[] lengthByte = new byte[4];
                byte[] name = new byte[4];

                lengthByte[3] = hex[i];
                lengthByte[2] = hex[i + 1];
                lengthByte[1] = hex[i + 2];
                lengthByte[0] = hex[i + 3];
                offset += 4;
                int dataLength = BitConverter.ToInt32(lengthByte, 0); //parsing lengh of data
                Console.Write(" : " + dataLength);

                name[0] = hex[i + 4];
                name[1] = hex[i + 5];
                name[2] = hex[i + 6];
                name[3] = hex[i + 7];
                offset += 4;
                string type = Encoding.ASCII.GetString(name);//parsing type
                Console.Write(" : " + type);

                byte[] data = new byte[dataLength];
                Array.Copy(hex, i + 8, data, 0, dataLength);//copy data to byte array
                offset += dataLength;

                byte[] crc = new byte[4];
                crc[0] = hex[offset];
                crc[1] = hex[offset + 1];
                crc[2] = hex[offset + 2];
                crc[3] = hex[offset + 3];//parsing crc checksum
                offset += 4;
                Console.WriteLine(" : {0}:{1} - {2}:{3} - {4}:{5} - {6}:{7}",
                    offset, crc[0],
                    offset + 1, crc[1],
                    offset + 2, crc[2],
                    offset + 3, crc[3]
                    );

                Chunk chunk = new Chunk(dataLength, type.ToCharArray(), data, crc);

                //chunk.length = dataLength;
                //chunk.type = type.ToCharArray();
                //chunk.data = data;
                //chunk.CRC = crc;

                Chunks.Add(chunk);
            }
            return Chunks;
        }
        /// <summary>
        /// A method to Build PNG file from it's chunks
        /// </summary>
        /// <param name="chunks">A array of chunks, basically containing all of it's information (except PNG signature) and they are must be in right order</param>
        /// <param name="path">A path, where file will be created, if it's null then new file will be created in the same directory where executable is with name "builded.png"</param>
        public void BuildPNG(PNGChunk[] chunks, string path = null)
        {
            
            //PNGSIG: 89 50 4E 47 0D 0A 1A 0A or 137, 80, 78, 71, 13, 10, 26, 10
            List<byte> bytes = new List<byte>();
            bytes.AddRange(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });
            foreach(var item in chunks)
            {
                bytes.AddRange(BitConverter.GetBytes(item.Length).Reverse());
                bytes.AddRange(Encoding.ASCII.GetBytes(item.Type));
                bytes.AddRange(item.Data);
                bytes.AddRange(item.CRC);
            }
            if (path != null)
            {
                FileStream fs = new FileStream(path, FileMode.Create);
                fs.Write(bytes.ToArray(), 0, bytes.Count);
                fs.Close();
            }
            else
            {
                File.WriteAllBytes("builded.png", bytes.ToArray());
            }
        }
    }
    /// <summary>
    /// A Enumerable object, containig both yet done types and types that is planned to be added.
    /// </summary>
    public enum ImageType
    {
        JPG,
        PNG,
        BMP,
        TIFF,
    }
    /// <summary>
    /// A Enumerable object, containig different known ways to corrupt PNG
    /// </summary>
    public enum PNGCorruptType
    {
        ForwardSwap,
        BackwardSwap,
        ManualSwap,
        AutoSwap,
        Slide,
        SingleChange,
    }
}