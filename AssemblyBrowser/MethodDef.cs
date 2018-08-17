using System;
using System.Collections;

namespace AssemblyBrowser
{
	internal class MethodDef : IFolder, IProperties
	{
		public uint Address { get; set; }
		public ushort Name { get; set; }
		public string Path { get; set; }
		public uint Position { get; set; }

		public IEnumerable Items
		{
			get
			{
				using (var stream = System.IO.File.OpenRead(Path))
				using (var reader = new System.IO.BinaryReader(stream))
				{
					stream.Position = Position;

					var headerSize = reader.ReadByte();

					uint bodySize;

					if ((headerSize & 0x03) == 0x02)
					{
						// 1-Byte Header
						bodySize = (uint)headerSize >> 2;

						yield return "Tiny Header";
					}
					else
					{
						// 12-Byte Header
						var headerSize2 = reader.ReadByte();
						var maxStack = reader.ReadUInt16();
						bodySize = reader.ReadUInt32();
						var localVariableLayoutTableIndex = reader.ReadUInt32();

						yield return "Fat Header";
					}

					var bodyStart = stream.Position;

					while (stream.Position < bodyStart + bodySize)
					{
						yield return ReadInstruction(reader);
					}

					yield break;
				}
			}
		}

		private object ReadInstruction(System.IO.BinaryReader reader)
		{
			var value = reader.ReadByte();

			switch (value)
			{
				case 0x00:
					return "nop";

				case 0x01:
					return "break";

				case 0x02:
					return "ldarg.0";

				case 0x03:
					return "ldarg.1";

				case 0x04:
					return "ldarg.2";

				case 0x05:
					return "ldarg.3";

				case 0x06:
					return "ldloc.0";

				case 0x07:
					return "ldloc.1";

				case 0x08:
					return "ldloc.2";

				case 0x09:
					return "ldloc.3";

				case 0x0a:
					return "stloc.0";

				case 0x0b:
					return "stloc.1";

				case 0x0c:
					return "stloc.2";

				case 0x0d:
					return "stloc.3";

				case 0x0e:
					return "ldarg.s: " + reader.ReadByte();

				case 0x15:
					return "ldc.i4.m1";

				case 0x16:
					return "ldc.i4.0";

				case 0x17:
					return "ldc.i4.1";

				case 0x18:
					return "ldc.i4.2";

				case 0x19:
					return "ldc.i4.3";

				case 0x1a:
					return "ldc.i4.4";

				case 0x1b:
					return "ldc.i4.5";

				case 0x1c:
					return "ldc.i4.6";

				case 0x1d:
					return "ldc.i4.7";

				case 0x1e:
					return "ldc.i4.8";

				case 0x1f:
					return "ldc.i4.s: " + reader.ReadByte();

				case 0x25:
					return "dup";

				case 0x26:
					return "pop";

				case 0x28:
					return "call: " + reader.ReadUInt16();

				case 0x29:
					return "calli: " + reader.ReadUInt16();

				case 0x2a:
					return "ret";

				case 0x2c:
					return "brfalse.s " + reader.ReadByte();

				case 0x2d:
					return "brtrue.s " + reader.ReadByte();

				case 0x34:
					return "bge.un.s " + reader.ReadByte();

				case 0x46:
					return "ldind.i1";

				case 0x6f:
					return "callvirt: " + reader.ReadUInt16();

				case 0x70:
					return "cpobj: " + reader.ReadUInt16();

				case 0x72:
					return "ldstr: " + reader.ReadUInt16();

				case 0x8d:
					return "newarr: " + reader.ReadUInt16();

				case 0xa2:
					return "stelem.ref";

				case 0xa4:
					return "stelem: " + reader.ReadUInt16();

				default:
					return value.ToString("X2");
			}
		}

		public object Properties => new { Address, Name };

		public override string ToString() => "MethodDef:" + Name;
	}
}