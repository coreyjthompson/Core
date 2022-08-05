namespace MEI.Security.Cryptography
{
	using System.Collections.Generic;

	public interface IKeyManager
	{
		int CurrentAuthKeyVersionNumber { get; }

		int CurrentCryptKeyVersionNumber { get; }

		IReadOnlyDictionary<int, byte[]> AuthKeys { get; }

		IReadOnlyDictionary<int, byte[]> CryptKeys { get; }
	}
}
