using System.Collections.Generic;
using System.Security.Cryptography;

public static class ListExtensions
{
	private static RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

	private static byte[] randomNumber = new byte[1];

	public static void Shuffle<T>(this IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			do
			{
				provider.GetBytes(randomNumber);
			}
			while (randomNumber[0] >= num * (255 / num));
			int index = randomNumber[0] % num;
			num--;
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}
}
