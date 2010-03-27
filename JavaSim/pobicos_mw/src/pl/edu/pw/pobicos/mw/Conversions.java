package pl.edu.pw.pobicos.mw;

/**
 * Supports the necessary conversions based on hex and byte representation of data.
 * @author Michal Krzysztof Szczerbak
 */
public class Conversions {
	
	/**
	 * Converts several bytes into a long integer value.
	 * @param index index of a start byte in the array
	 * @param length number of bytes
	 * @param bytes byte array
	 * @param invert indicates the big(if true)- or small-endian
	 * @return integer value
	 */
	public static long byteToInt(int index, int length, byte[] bytes, boolean invert)
	{
		long result = 0;
		if(invert)
			for(int i = index, j = 0; i < index + length; i++, j++)
				result += ((int)bytes[i] & 0x00FF) * Math.pow(256, j);
		else
			for(int i = index + length - 1, j = 0; i >= index; i--, j++)
				result += ((int)bytes[i] & 0x00FF) * Math.pow(256, j);
		return result;
	}
	
	/**
	 * Retrieves a subarray of an array of bytes.
	 * @param index index of a start byte in the array
	 * @param length number of bytes
	 * @param bytes byte array
	 * @return byte subarray
	 */
	public static byte[] subByte(int index, int length, byte[] bytes)
	{
		byte[] result = new byte[length];
		for(int i = 0; i < length; i++)
			result[i] = bytes[index + i];
		return result;
	}
	
	/**
	 * Searches an index of a requested byte.
	 * @param searched a byte to search
	 * @param bytes byte array
	 * @param index index to start searching from
	 * @return index of the first occurrence or '-1' if nothing is found
	 */
	public static int indexOfByte(byte searched, byte[] bytes, int index)
	{
		for(int i = index; i < bytes.length; i++)
			if(bytes[i] == searched)
				return i;
		return -1;
	}

	/**
	 * Converts string of hex digits into a long integer value.
	 * @param hex string
	 * @return integer
	 */
	public static long hexStringToLong(String hex)
	{
		long result = 0;
		for(int i = hex.length() - 1, j = 0; i >= 0; i--, j++)
		{
			result += hexCharToInt(hex.charAt(i)) * Math.pow(16, j);
		}
		return result;
	}

	/**
	 * Converts long integer value into a string of hex digits of a specified length.
	 * @param in integer
	 * @param bytes number of bytes
	 * @return hex string
	 */
	public static String longToHexString(long in, int bytes)
	{
		String result = "";
		for(int i = 0; i < 2 * bytes; i++)
			result = intToHexChar((int)((in / Math.pow(16, i)) % 16)) + result;
		return result;
	}

	private static int hexCharToInt(char ch)
	{
		switch(ch)
		{
		case '1':
			return 1;
		case '2':
			return 2;
		case '3':
			return 3;
		case '4':
			return 4;
		case '5':
			return 5;
		case '6':
			return 6;
		case '7':
			return 7;
		case '8':
			return 8;
		case '9':
			return 9;
		case 'A':
		case 'a':
			return 10;
		case 'B':
		case 'b':
			return 11;
		case 'C':
		case 'c':
			return 12;
		case 'D':
		case 'd':
			return 13;
		case 'E':
		case 'e':
			return 14;
		case 'F':
		case 'f':
			return 15;
		default:
			return 0;
		}
	}
	
	private static char intToHexChar(int ch)
	{
		switch(ch)
		{
		case 1:
			return '1';
		case 2:
			return '2';
		case 3:
			return '3';
		case 4:
			return '4';
		case 5:
			return '5';
		case 6:
			return '6';
		case 7:
			return '7';
		case 8:
			return '8';
		case 9:
			return '9';
		case 10:
			return 'a';
		case 11:
			return 'b';
		case 12:
			return 'c';
		case 13:
			return 'd';
		case 14:
			return 'e';
		case 15:
			return 'f';
		default:
			return '0';
		}
	}
}
