namespace LTNet
{
	using System;
	using System.IO;
	using UnityEngine;

	public class DataStream
	{
		private BinaryReader mBinReader;
		private BinaryWriter mBinWriter;
		private MemoryStream mMemStream;
		private bool mBEMode;//big endian mode

		public DataStream(bool isBigEndian)
		{
			mMemStream = new MemoryStream();
			InitWithMemoryStream(mMemStream, isBigEndian);
		}

		public DataStream(byte[] buffer, bool isBigEndian)
        {
            mMemStream = new MemoryStream(buffer);
			InitWithMemoryStream(mMemStream, isBigEndian);
        }

		public DataStream(byte[] buffer, int index, int count, bool isBigEndian)
		{
			mMemStream = new MemoryStream(buffer, index, count);
			InitWithMemoryStream(mMemStream, isBigEndian);
		}

		private void InitWithMemoryStream(MemoryStream ms, bool isBigEndian)
		{
			mBinReader = new BinaryReader(mMemStream);
			mBinWriter = new BinaryWriter(mMemStream);
			mBEMode = isBigEndian;
		}

		public void Close()
		{
			mMemStream.Close();
			mBinReader.Close();
			mBinWriter.Close();
		}

		public void SetBigEndian(bool isBigEndian)
		{
			mBEMode = isBigEndian;
		}

		public bool IsBigEndian()
		{
			return mBEMode;
		}

        public long Position
        {
            get 
            {
                return mMemStream.Position;
            }
            set 
            {
                mMemStream.Position = value;
            }
        }

		public long Length
		{
			get
			{
				return mMemStream.Length;
			}
		}

		public byte[] ToByteArray()
		{
				//return mMemStream.GetBuffer();
				return mMemStream.ToArray();
		}

        public long Seek(long offset, SeekOrigin loc)
        {
            return mMemStream.Seek(offset, loc);
        }

		public void WriteRaw(byte[] bytes)
		{
			mBinWriter.Write(bytes);
		}

        public void WriteRaw(byte[] bytes, int offset, int count)
        {
            mBinWriter.Write(bytes, offset, count);
        }

		public void WriteByte(byte value)
		{
			mBinWriter.Write(value);
		}

        public byte ReadByte()
        {
            return mBinReader.ReadByte();
        }

		public void WriteInt16(UInt16 value)
		{
			WriteInteger(BitConverter.GetBytes(value));
		}

        public UInt16 ReadInt16()
        {
            UInt16 val =  mBinReader.ReadUInt16();
            if (mBEMode)
               return BitConverter.ToUInt16(FlipBytes(BitConverter.GetBytes(val)), 0);
            return val;
        }

		public void WriteInt32(UInt32 value)
		{
			WriteInteger(BitConverter.GetBytes(value));
		}

        public UInt32 ReadInt32()
        {
            UInt32 val = mBinReader.ReadUInt32();
            if (mBEMode)
                return BitConverter.ToUInt32(FlipBytes(BitConverter.GetBytes(val)), 0);
            return val;
        }

		public void WriteInt64(UInt64 value)
		{
			WriteInteger(BitConverter.GetBytes(value));
		}

        public UInt64 ReadInt64()
        {
            UInt64 val = mBinReader.ReadUInt64();
            if (mBEMode)
                return BitConverter.ToUInt64(FlipBytes(BitConverter.GetBytes(val)), 0);
            return val;
        }

        public void WriteString8(string value)
        {
            WriteInteger(BitConverter.GetBytes((byte)value.Length));
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            mBinWriter.Write(encoding.GetBytes(value));
        }

        public string ReadString8()
        {
            int len = ReadByte();
            byte[] bytes = mBinReader.ReadBytes(len);
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetString(bytes);
        }

		public void WriteString16(string value)
		{
			WriteInteger(BitConverter.GetBytes((Int16)value.Length));
			System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
			mBinWriter.Write(encoding.GetBytes(value));
		}

        public string ReadString16()
        {
            int len = ReadInt16();
            byte[] bytes = mBinReader.ReadBytes(len);
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetString(bytes);
        }

		private void WriteInteger(byte[] bytes)
		{
			if (mBEMode)
				FlipBytes(bytes);
			mBinWriter.Write(bytes);
		}

        //ע��float�ľ��ȣ������λ��Ч����
        public Single ReadFloat()
        {
            Single val = mBinReader.ReadSingle();
            if (mBEMode)
                return BitConverter.ToSingle(FlipBytes(BitConverter.GetBytes(val)), 0);
            return val;
        }

        //ע��float�ľ��ȣ������λ��Ч����
        public void WriteFloat(Single value)
        {
            WriteInteger(BitConverter.GetBytes(value));
        }

		private byte[] FlipBytes(byte[] bytes)
		{
			//Array.Reverse(bytes); 
			for (int i = 0, j = bytes.Length - 1; i < j; ++i, --j)
			{
				byte temp = bytes[i];
				bytes[i] = bytes[j];
				bytes[j] = temp;
			}
			return bytes;
		}
	}
}