using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AssemblyBrowser
{
	internal class ClrMetadata : IFolder, IProperties
	{
		public uint Address { get; set; }
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

					var signature = reader.ReadUInt32();
					var majorVersion = reader.ReadUInt16();
					var minorVersion = reader.ReadUInt16();
					var reserved = reader.ReadUInt32();
					var versionLength = reader.ReadUInt32();
					var version = reader.ReadChars((int)versionLength);
					var flags = reader.ReadUInt16();
					var streamCount = reader.ReadUInt16();

					for (var stream2 = 0; stream2 < streamCount; stream2++)
					{
						var offset = reader.ReadUInt32();
						var size = reader.ReadUInt32();

						var characters = ReadString(reader, stream);

						var name = new string(characters);

						switch (name)
						{
							case "#~\0\0":
								yield return new ClrMetadataTable { Offset = offset, Address = Address + offset, Size = size, Position = Position + offset, Path = Path };
								break;

							case "#Strings\0\0\0\0":
								yield return new ClrAsciiTable { Offset = offset, Size = size, Position = Position + offset, Path = Path };
								break;

							case "#US\0":
								yield return new ClrUnicodeTable { Offset = offset, Size = size, Position = Position + offset, Path = Path };
								break;

							case "#Blob\0\0\0":
								yield return new ClrBlobTable { Offset = offset, Size = size, Position = Position + offset, Path = Path };
								break;

							case "#GUID\0\0\0":
								yield return new ClrGuidTable { Offset = offset, Size = size, Position = Position + offset, Path = Path };
								break;

							default:
								yield return name;
								break;
						}
					}
				}
			}
		}

		private char[] ReadString(BinaryReader reader, FileStream stream)
		{
			var characters = new List<char>();

			while (true)
			{
				var character = reader.ReadChar();

				characters.Add(character);

				if (character == 0 && (characters.Count & 0x03) == 0)
					return characters.ToArray();
			}
		}

		public object Properties => new { Address, Size };

		public override string ToString() => "CLR Metadata";
	}
}