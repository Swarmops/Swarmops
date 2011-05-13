using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Activizr
{
	public class DotNetChartingCrack
	{
		static public string DecodeFile (string licenseFile)
		{
			string text = null;
			FileStream stream = null;
			try
			{
				string strPassword = null;
				stream = new FileStream(licenseFile, FileMode.Open, FileAccess.Read);
				MemoryStream stream2 = new MemoryStream();
				byte[] buffer = new byte[0x100];
				long num = 0;
				long length = stream.Length;
				int count = 0;
				for (int i = 0x80; i < 0x88; i++)
				{
					strPassword = strPassword + Convert.ToChar(i).ToString();
				}
				DES des = new DESCryptoServiceProvider();
				byte[] bytes = new PasswordDeriveBytes(strPassword, null).GetBytes(8);
				des.Key = bytes;
				des.IV = new byte[8];
				CryptoStream stream3 = new CryptoStream(stream2, des.CreateDecryptor(), CryptoStreamMode.Write);
				while (num < length)
				{
					count = stream.Read(buffer, 0, 0x100);
					stream3.Write(buffer, 0, count);
					num += count;
				}
				stream3.Close();
				string text3 = Encoding.ASCII.GetString(stream2.GetBuffer(), 0, count);
				int index = text3.IndexOf('\0');
				text = text3.Substring(0, index);
				stream2.Close();
			}
			catch
			{
			}
			finally
			{
				stream.Close();
			}
			return text;
		}


		static public void EncodeFile3x(string licenseFile, string domainToLicense)
		{
			// The format of the encrypted license file is "m[contractnumber]domain". However, the
			// verifier only checks for the presence of the first lowercase "m", then finds the
			// end square bracket and reads the domain from there onwards. So this works too.
			WriteLicenseFile(licenseFile, "methinks you should Vote Pirate for a DRM free world.]" + domainToLicense);
		}

		static public void EncodeFile4x(string licenseFile, string domainToLicense)
		{
			// The format of the encrypted license file is "o[contractnumber]domain". However, the
			// verifier only checks for the presence of the first lowercase "o", then finds the
			// end square bracket and reads the domain from there onwards. So this works too.
			WriteLicenseFile(licenseFile, "oh, was this the best you could do?]" + domainToLicense);
		}

        static public void WriteManyFiles (string path, string domainToLicense)
        {
            for (char ch = 'p'; ch <= 'z'; ch++)
            {
                WriteLicenseFile(Path.Combine(path, ch + "-" + domainToLicense + ".lic"), ch + "]" + domainToLicense);
            }
        }

		static private void WriteLicenseFile(string licenseFile, string contents)
		{
			FileStream stream = null;
			try
			{
				string strPassword = null;
				stream = new FileStream(licenseFile, FileMode.Create, FileAccess.Write);
				byte[] buffer = new byte[0x100];
				for (int i = 0x80; i < 0x88; i++)
				{
					strPassword = strPassword + Convert.ToChar(i).ToString();
				}
				DES des = new DESCryptoServiceProvider();
				byte[] bytes = new PasswordDeriveBytes(strPassword, null).GetBytes(8);
				byte[] domainBytes = Encoding.Default.GetBytes(contents);
				des.Key = bytes;
				des.IV = new byte[8];

				CryptoStream stream3 = new CryptoStream(stream, des.CreateEncryptor(), CryptoStreamMode.Write);
				stream3.Write(domainBytes, 0, domainBytes.Length);
				stream3.Close();
			}
			catch
			{
			}
			finally
			{
				stream.Close();
			}
		}

	}
}
