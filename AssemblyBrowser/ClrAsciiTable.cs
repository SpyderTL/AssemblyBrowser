using System.Collections;
using System.Collections.Generic;

namespace AssemblyBrowser
{
	internal class ClrAsciiTable : IFolder, IProperties
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
					var position = stream.Position;

					var characters = new List<char>();

					while (stream.Position < Position + Size)
					{
						var character = reader.ReadChar();

						if (character == 0)
						{
							if (characters.Count != 0)
							{
								yield return new string(characters.ToArray()) + ": " + (position - Position);

								characters.Clear();
							}

							position = stream.Position;
						}
						else
							characters.Add(character);
					}
				}
			}
		}

		public object Properties => new { Offset, Size };

		public override string ToString() => "ASCII Table";
	}
}