using System;
using System.IO;
using LicenseKey;

namespace NUnit.LicenseNunit
{
	using System.Collections;
	using NUnit.Framework;

	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture] 
	public class BuildTest
	{
		Randomm	rnd;		// the random number 

		public BuildTest()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		[SetUp] public void Init() 
		{
			rnd = new Randomm();
		}


		/// <summary>
		/// 
		/// </summary>
		[TearDown] public void Dispose()
		{
		}


		/// <summary>
		/// Test the random number generation
		/// </summary>
		[Test] public void Test01_Random() 
		{
			try 
			{
				int		rannum; 
				string	strrndnum; 
				int		lopcnt;
				int		lopcnt1;
				bool	usebase10;

				usebase10 = false;
				for ( lopcnt1 = 0; lopcnt1 < 10; lopcnt1++ ) 
				{
					for ( lopcnt = 1; lopcnt < 7; lopcnt++) 
					{
						rnd.SetMaxLength(lopcnt);

						rannum = rnd.GetRandomNumber();
						strrndnum = NumberDisplay.CreateNumberString((uint)rannum, lopcnt, usebase10);
						if ( strrndnum.Length != lopcnt ) 
						{
							throw new Exception("Strings not the correct length");
						}
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with no parameters 8-4-8-4
		/// </summary>
		[Test] public void Test02_GenNoParB10Byt8484()  
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "xxxxxxxx-xxxx-xxxxxxxx-xxxx";
					gkey.MaxTokens = 0;
					gkey.UseBase10 = true;
					gkey.UseBytes = true;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( finalkey.Length != gkey.LicenseTemplate.Length ) 
					{
						throw new Exception("Keys are not the same length");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with no parameters 8-4-8-4
		/// </summary>
		[Test] public void Test03_GenNoParB10Byt4884() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "xxxx-xxxxxxxx-xxxxxxxx-xxxx";
					gkey.MaxTokens = 0;
					gkey.UseBase10 = true;
					gkey.UseBytes = true;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( finalkey.Length != gkey.LicenseTemplate.Length ) 
					{
						throw new Exception("Keys are not the same length");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}

		
		/// <summary>
		/// Test a generic key generation with 2 parameters 8-4-8-4
		/// </summary>
		[Test] public void Test04_Gen2ParB10Byt4884() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "xxvv-xxxxxxxx-xxxxxxxx-ppxx";
					gkey.MaxTokens = 2;
					gkey.AddToken(0, "v", LicenseKey.GenerateKey.TokenTypes.NUMBER, "12");
					gkey.AddToken(1, "p", LicenseKey.GenerateKey.TokenTypes.NUMBER, "23");
					gkey.UseBase10 = true;
					gkey.UseBytes = true;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( finalkey.Length != gkey.LicenseTemplate.Length ) 
					{
						throw new Exception("Keys are not the same length");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with no parameters 5-5-5-5-5
		/// </summary>
		[Test] public void Test05_GenNoParB16Byt55555() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;

				for ( lopcnt = 1; lopcnt < 20; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "xxxxx-xxxxx-xxxxx-xxxxx-xxxxx";
					gkey.MaxTokens = 0;
					gkey.UseBase10 = false;
					gkey.UseBytes = true;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( finalkey.Length != gkey.LicenseTemplate.Length ) 
					{
						throw new Exception("Keys are not the same length");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with no parameters 5-5-5-5-5
		/// </summary>
		[Test] public void Test06_Gen2ParB16Byt55555() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "vvxxx-xxxxx-ppxxx-xxxxx-xxxxx";
					gkey.MaxTokens = 2;
					gkey.AddToken(0, "v", LicenseKey.GenerateKey.TokenTypes.NUMBER, "12");
					gkey.AddToken(1, "p", LicenseKey.GenerateKey.TokenTypes.NUMBER, "23");
					gkey.UseBase10 = false;
					gkey.UseBytes = true;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( finalkey.Length != gkey.LicenseTemplate.Length ) 
					{
						throw new Exception("Keys are not the same length");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with no parameters 5-5-5-5-5
		/// </summary>
		[Test] public void Test07_Gen2ParB16Bit55555() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "vvvvppppxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxx";
					gkey.MaxTokens = 2;
					gkey.AddToken(0, "v", LicenseKey.GenerateKey.TokenTypes.NUMBER, "1");
					gkey.AddToken(1, "p", LicenseKey.GenerateKey.TokenTypes.NUMBER, "2");
					gkey.UseBase10 = false;
					gkey.UseBytes = false;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( (finalkey.Length-4) != ((gkey.LicenseTemplate.Length-4)/4) ) 
					{
						throw new Exception("Keys are not the same length");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with 3 parameters 5-5-5-5-5
		/// </summary>
		[Test] public void Test08_Gen3ParB16Bit55555() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "vvvvppppxxxxxxxxxxxx-wwwwxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxx";
					gkey.MaxTokens = 3;
					gkey.AddToken(0, "v", LicenseKey.GenerateKey.TokenTypes.NUMBER, "1");
					gkey.AddToken(1, "p", LicenseKey.GenerateKey.TokenTypes.NUMBER, "2");
					gkey.AddToken(2, "w", LicenseKey.GenerateKey.TokenTypes.NUMBER, "8");
					gkey.UseBase10 = false;
					gkey.UseBytes = false;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( (finalkey.Length-4) != ((gkey.LicenseTemplate.Length-4)/4) ) 
					{
						throw new Exception("Keys are not the same length");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with 2 parameters 5-5-5-5-5
		/// </summary>
		[Test] public void Test09_Gen3ParB16Byt55555() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "vvxxx-xxxxx-ppxxx-ssxxx-xxxxx";
					gkey.MaxTokens = 3;
					gkey.AddToken(0, "v", LicenseKey.GenerateKey.TokenTypes.NUMBER, "12");
					gkey.AddToken(1, "p", LicenseKey.GenerateKey.TokenTypes.NUMBER, "23");
					gkey.AddToken(2, "s", LicenseKey.GenerateKey.TokenTypes.CHARACTER, "QW");
					gkey.UseBase10 = false;
					gkey.UseBytes = true;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( finalkey.Length != gkey.LicenseTemplate.Length ) 
					{
						throw new Exception("Keys are not the same length");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with 2 parameters 5-5-5-5-5
		/// </summary>
		[Test] public void Test10_Gen3ParB16Bit55555() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "vvvvppppxxxxxxxxxxxx-wwwwwwwwxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxx";
					gkey.MaxTokens = 3;
					gkey.AddToken(0, "v", LicenseKey.GenerateKey.TokenTypes.NUMBER, "1");
					gkey.AddToken(1, "p", LicenseKey.GenerateKey.TokenTypes.NUMBER, "2");
					gkey.AddToken(2, "w", LicenseKey.GenerateKey.TokenTypes.CHARACTER, "QR");
					gkey.UseBase10 = false;
					gkey.UseBytes = false;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( (finalkey.Length-4) != ((gkey.LicenseTemplate.Length-4)/4) ) 
					{
						throw new Exception("Keys are not the same length");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}

	
		/// <summary>
		/// Test a generic key generation/Disassemble with 2 parameters 8-4-8-4
		/// </summary>
		[Test] public void Test11_GenDis2ParB10Byt4884() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;
				string	result;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "xxvv-xxxxxxxx-xxxxxxxx-ppxx";
					gkey.MaxTokens = 2;
					gkey.AddToken(0, "v", LicenseKey.GenerateKey.TokenTypes.NUMBER, "12");
					gkey.AddToken(1, "p", LicenseKey.GenerateKey.TokenTypes.NUMBER, "23");
					gkey.UseBase10 = true;
					gkey.UseBytes = true;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( finalkey.Length != gkey.LicenseTemplate.Length ) 
					{
						throw new Exception("Keys are not the same length");
					}
					result = gkey.DisassembleKey("v");
					if ( result != "12" ) 
					{
						throw new Exception("The first tokens are not equal");
					}
					result = gkey.DisassembleKey("p");
					if ( result != "23" ) 
					{
						throw new Exception("The second tokens are not equal");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with no parameters 5-5-5-5-5
		/// </summary>
		[Test] public void Test12_GenDis2ParB16Byt55555() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;
				string	result;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "vvxxx-xxxxx-ppxxx-xxxxx-xxxxx";
					gkey.MaxTokens = 2;
					gkey.AddToken(0, "v", LicenseKey.GenerateKey.TokenTypes.NUMBER, "12");
					gkey.AddToken(1, "p", LicenseKey.GenerateKey.TokenTypes.NUMBER, "23");
					gkey.UseBase10 = false;
					gkey.UseBytes = true;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( finalkey.Length != gkey.LicenseTemplate.Length ) 
					{
						throw new Exception("Keys are not the same length");
					}
					result = gkey.DisassembleKey("v");
					if ( result != "12" ) 
					{
						throw new Exception("The first tokens are not equal");
					}
					result = gkey.DisassembleKey("p");
					if ( result != "23" ) 
					{
						throw new Exception("The first tokens are not equal");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with no parameters 5-5-5-5-5
		/// </summary>
		[Test] public void Test13_GenDis3ParB16Byt55555() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;
				string	result;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "vvxxx-xxxxx-ppxxx-ssxxx-xxxxx";
					gkey.MaxTokens = 3;
					gkey.AddToken(0, "v", LicenseKey.GenerateKey.TokenTypes.NUMBER, "12");
					gkey.AddToken(1, "p", LicenseKey.GenerateKey.TokenTypes.NUMBER, "23");
					gkey.AddToken(2, "s", LicenseKey.GenerateKey.TokenTypes.CHARACTER, "GH");
					gkey.UseBase10 = false;
					gkey.UseBytes = true;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( finalkey.Length != gkey.LicenseTemplate.Length ) 
					{
						throw new Exception("Keys are not the same length");
					}
					result = gkey.DisassembleKey("v");
					if ( result != "12" ) 
					{
						throw new Exception("The first tokens are not equal");
					}
					result = gkey.DisassembleKey("p");
					if ( result != "23" ) 
					{
						throw new Exception("The second tokens are not equal");
					}
					result = gkey.DisassembleKey("s");
					if ( result != "GH" ) 
					{
						throw new Exception("The third tokens are not equal");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with 3 parameters 5-5-5-5-5
		/// </summary>
		[Test] public void Test14_Gen3DisParB16Bit55555() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;
				string	result;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "vvvvppppxxxxxxxxxxxx-wwwwxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxx";
					gkey.MaxTokens = 3;
					gkey.AddToken(0, "v", LicenseKey.GenerateKey.TokenTypes.NUMBER, "1");
					gkey.AddToken(1, "p", LicenseKey.GenerateKey.TokenTypes.NUMBER, "2");
					gkey.AddToken(2, "w", LicenseKey.GenerateKey.TokenTypes.NUMBER, "6");
					gkey.UseBase10 = false;
					gkey.UseBytes = false;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( (finalkey.Length-4) != ((gkey.LicenseTemplate.Length-4)/4) ) 
					{
						throw new Exception("Keys are not the same length");
					}
					result = gkey.DisassembleKey("v");
					if ( result != "1" ) 
					{
						throw new Exception("The first tokens are not equal");
					}
					result = gkey.DisassembleKey("p");
					if ( result != "2" ) 
					{
						throw new Exception("The second tokens are not equal");
					}
					result = gkey.DisassembleKey("w");
					if ( result != "6" ) 
					{
						throw new Exception("The third tokens are not equal");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with 5 parameters 5-5-5-5-5
		/// </summary>
		[Test] public void Test15_Gen5DisParB16Bit55555() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;
				string	result;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "vvvvvvvvppppxxxxxxxx-wwwwxxxxxxxxxxxxxxxx-ssssssssxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxxrrrr";
					gkey.MaxTokens = 5;
					gkey.AddToken(0, "v", LicenseKey.GenerateKey.TokenTypes.NUMBER, "14");
					gkey.AddToken(1, "p", LicenseKey.GenerateKey.TokenTypes.NUMBER, "2");
					gkey.AddToken(2, "w", LicenseKey.GenerateKey.TokenTypes.NUMBER, "6");
					gkey.AddToken(3, "s", LicenseKey.GenerateKey.TokenTypes.NUMBER, "BC");
					gkey.AddToken(4, "r", LicenseKey.GenerateKey.TokenTypes.NUMBER, "5");
					gkey.UseBase10 = false;
					gkey.UseBytes = false;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( (finalkey.Length-4) != ((gkey.LicenseTemplate.Length-4)/4) ) 
					{
						throw new Exception("Keys are not the same length");
					}
					result = gkey.DisassembleKey("v");
					if ( result != "14" ) 
					{
						throw new Exception("The first tokens are not equal");
					}
					result = gkey.DisassembleKey("p");
					if ( result != "2" ) 
					{
						throw new Exception("The second tokens are not equal");
					}
					result = gkey.DisassembleKey("w");
					if ( result != "6" ) 
					{
						throw new Exception("The third tokens are not equal");
					}
					result = gkey.DisassembleKey("s");
					if ( result != "BC" ) 
					{
						throw new Exception("The third tokens are not equal");
					}
					result = gkey.DisassembleKey("r");
					if ( result != "5" ) 
					{
						throw new Exception("The third tokens are not equal");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with 5 parameters 5-5-5-5-5
		/// </summary>
		[Test] public void Test16_Gen5DisParB16BitChk155555() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;
				string	result;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "vvvvvvvvppppxxxxxxxx-wwwwxxxxxxxxxxxxxxxx-ssssssssxxxxxxxxxxxx-xxxxxxxxxxxxcccccccc-xxxxxxxxxxxxxxxxrrrr";
					gkey.MaxTokens = 5;
					gkey.ChecksumAlgorithm = Checksum.ChecksumType.Type1;
					gkey.AddToken(0, "v", LicenseKey.GenerateKey.TokenTypes.NUMBER, "14");
					gkey.AddToken(1, "p", LicenseKey.GenerateKey.TokenTypes.NUMBER, "2");
					gkey.AddToken(2, "w", LicenseKey.GenerateKey.TokenTypes.NUMBER, "6");
					gkey.AddToken(3, "s", LicenseKey.GenerateKey.TokenTypes.NUMBER, "BC");
					gkey.AddToken(4, "r", LicenseKey.GenerateKey.TokenTypes.NUMBER, "5");
					gkey.UseBase10 = false;
					gkey.UseBytes = false;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( (finalkey.Length-4) != ((gkey.LicenseTemplate.Length-4)/4) ) 
					{
						throw new Exception("Keys are not the same length");
					}
					result = gkey.DisassembleKey("v");
					if ( result != "14" ) 
					{
						throw new Exception("The first tokens are not equal");
					}
					result = gkey.DisassembleKey("p");
					if ( result != "2" ) 
					{
						throw new Exception("The second tokens are not equal");
					}
					result = gkey.DisassembleKey("w");
					if ( result != "6" ) 
					{
						throw new Exception("The third tokens are not equal");
					}
					result = gkey.DisassembleKey("s");
					if ( result != "BC" ) 
					{
						throw new Exception("The third tokens are not equal");
					}
					result = gkey.DisassembleKey("r");
					if ( result != "5" ) 
					{
						throw new Exception("The third tokens are not equal");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with 5 parameters 5-5-5-5-5
		/// </summary>
		[Test] public void Test17_Gen5DisParB16BitChk255555() 
		{
			int	lopcnt;

			try 
			{
				GenerateKey	gkey;
				string	finalkey;
				string	result;

				for ( lopcnt = 1; lopcnt < 30; lopcnt++) 
				{
					gkey = new GenerateKey();
					gkey.LicenseTemplate = "vvvvvvvvppppxxxxxxxx-wwwwxxxxxxxxxxxxxxxx-ssssssssxxxxxxxxxxxx-xxxxxxxxxxxxcccccccc-xxxxxxxxxxxxxxxxrrrr";
					gkey.MaxTokens = 5;
					gkey.ChecksumAlgorithm = Checksum.ChecksumType.Type2;
					gkey.AddToken(0, "v", LicenseKey.GenerateKey.TokenTypes.NUMBER, "34");
					gkey.AddToken(1, "p", LicenseKey.GenerateKey.TokenTypes.NUMBER, "6");
					gkey.AddToken(2, "w", LicenseKey.GenerateKey.TokenTypes.NUMBER, "8");
					gkey.AddToken(3, "s", LicenseKey.GenerateKey.TokenTypes.NUMBER, "AB");
					gkey.AddToken(4, "r", LicenseKey.GenerateKey.TokenTypes.NUMBER, "3");
					gkey.UseBase10 = false;
					gkey.UseBytes = false;
					gkey.CreateKey();
					finalkey = gkey.GetLicenseKey();
					if ( (finalkey.Length-4) != ((gkey.LicenseTemplate.Length-4)/4) ) 
					{
						throw new Exception("Keys are not the same length");
					}
					result = gkey.DisassembleKey("v");
					if ( result != "34" ) 
					{
						throw new Exception("The first tokens are not equal");
					}
					result = gkey.DisassembleKey("p");
					if ( result != "6" ) 
					{
						throw new Exception("The second tokens are not equal");
					}
					result = gkey.DisassembleKey("w");
					if ( result != "8" ) 
					{
						throw new Exception("The third tokens are not equal");
					}
					result = gkey.DisassembleKey("s");
					if ( result != "AB" ) 
					{
						throw new Exception("The third tokens are not equal");
					}
					result = gkey.DisassembleKey("r");
					if ( result != "3" ) 
					{
						throw new Exception("The third tokens are not equal");
					}
				}
			}
			catch (Exception e) 
			{
				Assert.AreEqual(66, 1, "UnExpected Failure. " + e.Message);
			}
		}


		/// <summary>
		/// Test a generic key generation with 5 parameters 5-5-5-5-5
		/// </summary>
		[Test] public void Test99_DoFile() 
		{
			// The StreamWriter must be defined outside of the try-catch block
			//   in order to reference it in the Finally block.
			StreamWriter myStreamWriter = null;
			StreamReader myStreamReader = null;
			LicenseKey.GenerateKey	gkey;
			string		myInputstring;
			string		delimStr = " ";
			char []		delimiter = delimStr.ToCharArray();
			string []	split = null;
			int			x=8; 
			int			nNumTokens;
			int			lop;
			int			lnum;
			int			ncnt;
			string		arg1;
			string		arg2;
			string		arg3; 
			string		arg4; 
			string		arg5; 
			string		arg6; 

			ncnt = 0;

			// Ensure that the creation of the new StreamWriter is wrapped in a 
			//   try-catch block, since an invalid filename could have been used.
			try 
			{
				// create the license key object
				gkey = new LicenseKey.GenerateKey();

				// Create a StreamReader using a static File class.
				myStreamReader = File.OpenText("LicTestDofile.TXT");

				// Create a StreamWriter using a static File class.
				myStreamWriter = File.CreateText("LicTestDofileout.txt");

				// Begin by reading a single line
				// Continue reading while there are still lines to be read
				while ((myInputstring = myStreamReader.ReadLine()) != null)
				{
					split = myInputstring.Split(delimiter, x);
					if ( split.Length != x ) 
					{
						break;
					}
					arg1 = split[0];	// base 1= 10 0 =16
					arg2 = split[1];	// bytes 1 = bytes 0 = bits
					arg3 = split[2]; 	// number of tokens
					myStreamWriter.Write(arg1 + " " + arg2 + " " + arg3 + " ");

					// see what base we are to use
					if ( arg1 == "1" ) 
					{
						// use base 10
						gkey.UseBase10 = true;
					}
					else 
					{
						// use base 16
						gkey.UseBase10 = false;
					}

					// see what format we use
					if ( arg2 == "1" ) 
					{
						// use bytes 
						gkey.UseBytes = true;
					}
					else 
					{
						// use bits
						gkey.UseBytes = false;
					}
					// set the number of tokens
					nNumTokens = Convert.ToInt32(arg3);
					gkey.MaxTokens= nNumTokens;

					lnum = 3;
					for ( lop = 0; lop < nNumTokens; lop++) 
					{
						arg4 = split[lnum]; 
						lnum++;
						arg5 = split[lnum]; 
						lnum++;
						myStreamWriter.Write(arg4 + " " + arg5 + " ");
						try 
						{
							gkey.AddToken(lop, arg4, LicenseKey.GenerateKey.TokenTypes.NUMBER, arg5);
						}
						catch (Exception ex) 
						{
							Assert.AreEqual(66, 1, "Error from AddToken. " + ex.Message);
							return;
						}
					}
					arg6 = split[lnum];
					gkey.LicenseTemplate = arg6;

					try 
					{
						gkey.CreateKey();
					}
					catch (Exception ex) 
					{
						Assert.AreEqual(66, 1, "Error from CreateKey. " + ex.Message);
						return;
					}
				
					myStreamWriter.WriteLine(arg6 + " " + gkey.GetLicenseKey());
					ncnt++;
					//					myStreamWriter.WriteLine(arg1 + " " + arg2 + " " + arg3 + "    " + arg4 + "    " + arg5 + "    " + gkey.getLicenseKey());
					//					myStreamWriter.WriteLine(arg1 + " " + arg2 + " " + arg3 + "    " + arg6 + "    " + gkey.getLicenseKey());
				}
				myStreamWriter.Flush();
			}
			catch(Exception exc)
			{
				string	errstr;

				errstr = "File could not be opened or read." + Environment.NewLine + "Please verify that the filename is correct, and that you have read permissions for the desired directory." + Environment.NewLine + Environment.NewLine + "Exception: " + exc.Message;
				// Show the error to the user.
				Assert.AreEqual(66, 1, "UnExpected Failure. " + errstr);
			}
			finally
			{
				// Close the object if it has been created.
				if (myStreamReader != null)
				{
					myStreamReader.Close();
				}
				// Close the object if it has been created.
				if (myStreamWriter != null)
				{
					myStreamWriter.Close();
				}
			}
		}
	}
}
