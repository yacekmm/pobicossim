package pl.edu.pw.pobicos.ng;

/**
 * Supports the necessary conversions based on hex and byte representation of data
 * @author Micha³ Krzysztof Szczerbak
 */
public class Conversions {

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
