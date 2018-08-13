using System.Collections;
using System.Collections.Generic;

namespace AssemblyBrowser
{
	internal class ClrUnicodeTable : IFolder, IProperties
	{
		public uint Offset { get; set; }
		public uint Size { get; set; }
		public uint Position { get; set; }
		public string Path { get; set; }

		public IEnumerable Items
		{
			get
			{
				using (var stream = System.IO.File.OpenRead(Path))
				using (var reader = new System.IO.BinaryReader(stream))
				{
					stream.Position = Position;

					while (stream.Position < Position + Size)
					{
						var length = (int)reader.ReadByte();

						if (length == 0)
							continue;

						if ((length & 0x80) == 0x80)
						{
							if ((length & 0x40) == 0x00)
								length = ((length & 0x3f) << 8) | reader.ReadByte();
							else
								length = ((length & 0x3f) << 24) | (reader.ReadByte() << 16) | (reader.ReadByte() << 8) | reader.ReadByte();
						}

						var data = reader.ReadBytes(length - 1);

						var extendedUnicode = reader.ReadByte();

						yield return System.Text.Encoding.Unicode.GetString(data);
					}
				}
			}
		}

		public object Properties => new { Offset, Size };

		public override string ToString() => "Unicode Table";
	}
}