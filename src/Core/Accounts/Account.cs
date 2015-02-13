using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Trezorix.Sparql.Api.Core.Accounts
{
  using MongoRepository;

  public class Account
	{
		public string FullName { get; set; }
		public string UserName { get; set; }
		public IEnumerable<string> Roles { get; set; }
		public string Password { get; set; }
		public Guid ApiKey { get; set; }

    public string Id {
      get { return ApiKey.ToString(); }
    }

		public virtual string ComputeSaltedHash(string password)
		{
			return SHA512(password, ReverseString(UserName.ToLower()));
		}

		/// <summary>
		/// Calculates a hash string for value and salt using the SHA512 algoritm.
		/// </summary>
		private static string SHA512(string value, string salt)
		{
			var encoding = new ASCIIEncoding();

			byte[] saltedValue = encoding.GetBytes(value).Concat(encoding.GetBytes(salt)).ToArray();

			var sha512 = new HMACSHA512(encoding.GetBytes("1Xh6M01GX4pxs99932L17843rIvnAL3sB2RmeaDu9c4GA3z8w6Dwf9DE8rx57DO7Wy23St89FNl0pqbp5YJaj0r63z18CVqCM3vrw01HJgO60Hu6t4Bt894m416RA89C"));
			string result = ByteToString(sha512.ComputeHash(saltedValue));

			return result;
		}

		// Ti: Joppe: Could compare and store byte arrays convert incoming hashes from string to byte array
		private static string ByteToString(byte[] buff)
		{
			string sbinary = string.Empty;

			for (int i = 0; i < buff.Length; i++)
			{
				sbinary += buff[i].ToString("X2"); // hex format
			}

			return sbinary;
		}
	
		/// <summary>
		/// Receives string and returns the string with its letters reversed.
		/// </summary>
		internal static string ReverseString(string s)
		{
			char[] arr = s.ToCharArray();
			Array.Reverse(arr);
			return new string(arr);
		}

	}
}
