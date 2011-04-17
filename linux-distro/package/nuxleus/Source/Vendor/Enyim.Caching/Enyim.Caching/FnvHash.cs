using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Threading;

namespace Enyim
{
	/// <summary>
	/// Implements a 64 bit long Fowler-Noll-Vo hash
	/// </summary>
	/// <remarks>
	/// Calculation found at http://lists.danga.com/pipermail/memcached/2007-April/003846.html, but 
	/// it is pretty much available everywhere
	/// </remarks>
	public sealed class FnvHash64 : System.Security.Cryptography.HashAlgorithm
	{
		private const ulong FNV_64_INIT = 0xcbf29ce484222325L;
		private const ulong FNV_64_PRIME = 0x100000001b3L;

		private ulong currentHashValue;

		public FnvHash64()
		{
			base.HashSizeValue = 64;

			this.Initialize();
		}

		public override void Initialize()
		{
			this.currentHashValue = FNV_64_INIT;
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			int end = ibStart + cbSize;

			for (int i = ibStart; i < end; i++)
			{
				this.currentHashValue = (this.currentHashValue * FNV_64_PRIME) ^ array[i];
			}
		}

		protected override byte[] HashFinal()
		{
			return BitConverter.GetBytes(this.currentHashValue);
		}

		public ulong CurrentHashValue
		{
			get { return this.currentHashValue; }
		}
	}

	public class FNV1a : HashAlgorithm
	{
		private const uint Prime = 16777619;
		private const uint Offset = 2166136261;

		protected uint CurrentHashValue;

		public FNV1a()
		{
			this.HashSizeValue = 32;
			this.Initialize();
		}

		public override void Initialize()
		{
			this.CurrentHashValue = Offset;
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			int end = ibStart + cbSize;

			for (int i = ibStart; i < end; i++)
			{
				this.CurrentHashValue = (this.CurrentHashValue ^ array[i]) * FNV1a.Prime;
			}
		}

		protected override byte[] HashFinal()
		{
			return BitConverter.GetBytes(this.CurrentHashValue);
		}
	}

	// algorithm found at http://bretm.home.comcast.net/hash/6.html
	// provides better distribution but it's only 32 bit long
	public class ModifiedFNV : FNV1a
	{
		protected override byte[] HashFinal()
		{
			this.CurrentHashValue += this.CurrentHashValue << 13;
			this.CurrentHashValue ^= this.CurrentHashValue >> 7;
			this.CurrentHashValue += this.CurrentHashValue << 3;
			this.CurrentHashValue ^= this.CurrentHashValue >> 17;
			this.CurrentHashValue += this.CurrentHashValue << 5;

			return base.HashFinal();
		}
	}
}