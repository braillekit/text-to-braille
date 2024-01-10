// Copyright (c) 2013-2018 Cemalettin Dervis, MIT License.
// https://github.com/cemdervis/SharpConfig
// https://github.com/huanlin/SharpConfig (Since February 2022)

using System;
using SharpConfig;
using NUnit.Framework;
using System.IO;

namespace Tests
{
  [TestFixture]
  public sealed class SimpleConfigTest
  {
    [Test]
    public void SingleValues()
    {
      var cfg = new Configuration();

      cfg["TestSection"]["IntSetting1"].IntValue = 100;
      cfg["TestSection"]["IntSetting2"].IntValue = 200;
      cfg["TestSection"]["StringSetting1"].StringValue = "Test";

      Assert.AreEqual(cfg["TestSection"]["IntSetting1"].IntValue, 100);
      Assert.AreEqual(cfg["TestSection"]["IntSetting2"].IntValue, 200);
      Assert.AreEqual(cfg["TestSection"]["StringSetting1"].StringValue, "Test");
    }

    [Test]
    public void ArrayValues()
    {
      var cfg = new Configuration();

      var ints = new int[] { -3, -2, -1, 0, 1, 2, 3 };
      var strings = new string[] { "Hello", "World", "!" };
      var floats = new float[] { 0.5f, 1.0f, 1.5f };

      cfg["TestSection"]["IntArray"].IntValueArray = ints;
      cfg["TestSection"]["StringArray"].StringValueArray = strings;
      cfg["TestSection"]["FloatArray"].FloatValueArray = floats;

      // ints
      {
        var arr = cfg["TestSection"]["IntArray"].IntValueArray;
        Assert.AreEqual(ints.Length, arr.Length);
        for (int i = 0; i < ints.Length; i++)
          Assert.AreEqual(ints[i], arr[i]);
      }
      // strings
      {
        var arr = cfg["TestSection"]["StringArray"].StringValueArray;
        Assert.AreEqual(strings.Length, arr.Length);
        for (int i = 0; i < strings.Length; i++)
          Assert.AreEqual(strings[i], arr[i]);
      }
      // floats
      {
        var arr = cfg["TestSection"]["FloatArray"].FloatValueArray;
        Assert.AreEqual(floats.Length, arr.Length);
        for (int i = 0; i < floats.Length; i++)
          Assert.AreEqual(floats[i], arr[i]);
      }
    }

    [Test]
    public void SetGetValue()
    {
      var cfg = new Configuration();

      var ints = new int[] { 1, 2, 3 };

      cfg["TestSection"]["IntSetting1"].SetValue(100);
      cfg["TestSection"]["IntSetting2"].SetValue(200);
      cfg["TestSection"]["StringSetting1"].SetValue("Test");
      cfg["TestSection"]["IntArray"].SetValue(ints);

      Assert.AreEqual(cfg["TestSection"]["IntSetting1"].GetValue(typeof(int)), 100);
      Assert.AreEqual(cfg["TestSection"]["IntSetting2"].GetValue(typeof(int)), 200);
      Assert.AreEqual(cfg["TestSection"]["StringSetting1"].GetValue(typeof(string)), "Test");

      var intsNonGeneric = cfg["TestSection"]["IntArray"].GetValueArray(typeof(int));
      var intsGeneric = cfg["TestSection"]["IntArray"].GetValueArray<int>();

      Assert.AreEqual(intsNonGeneric.Length, intsGeneric.Length);
      Assert.AreEqual(intsGeneric.Length, ints.Length);

      for (int i = 0; i < intsNonGeneric.Length; i++)
      {
        Assert.AreEqual(intsNonGeneric[i], intsGeneric[i]);
        Assert.AreEqual(intsGeneric[i], ints[i]);
      }

      // Verify that wrong usage of GetValue throws.
      Assert.Throws<InvalidOperationException>(() =>
      {
        cfg["TestSection"]["IntArray"].GetValue(typeof(int[]));
      });
      Assert.Throws<InvalidOperationException>(() =>
      {
        cfg["TestSection"]["IntArray"].GetValue(typeof(int[][]));
      });
      Assert.Throws<InvalidOperationException>(() =>
      {
        cfg["TestSection"]["IntArray"].GetValue<int[]>();
      });
      Assert.Throws<InvalidOperationException>(() =>
      {
        cfg["TestSection"]["IntArray"].GetValue<int[][]>();
      });

      // Verify that wrong usage of GetValueArray throws.
      Assert.Throws<ArgumentException>(() =>
      {
        cfg["TestSection"]["IntArray"].GetValueArray(typeof(int[]));
      });
      Assert.Throws<ArgumentException>(() =>
      {
        cfg["TestSection"]["IntArray"].GetValueArray<int[]>();
      });
      Assert.Throws<ArgumentException>(() =>
      {
        cfg["TestSection"]["IntArray"].GetValueArray(typeof(int[][]));
      });
      Assert.Throws<ArgumentException>(() =>
      {
        cfg["TestSection"]["IntArray"].GetValueArray<int[][]>();
      });
    }

    [Test]
    public void SectionAdditionAndRemoval()
    {
      var cfg = new Configuration();

      var section1 = new Section("Section1");
      var section2 = new Section("Section2");

      cfg.Add(section1);
      cfg.Add(section2);
      cfg["Section3"]["Setting1"].SetValue(0);

      Assert.IsTrue(cfg.Contains(section1));
      Assert.IsTrue(cfg.Contains(section2));
      Assert.IsTrue(cfg.Contains("Section1"));
      Assert.IsTrue(cfg.Contains("Section2"));
      Assert.IsTrue(cfg.Contains("Section3"));
      Assert.IsTrue(cfg["Section3"].Contains("Setting1"));

      cfg.Add(new Section("Section1"));
      cfg.Add(new Section("Section1"));

      {
        var sections = cfg.GetSectionsNamed("Section1");
        int actualCount = 0;
        foreach (var sec in sections)
        {
          Assert.AreEqual(sec.Name, "Section1");
          actualCount++;
        }

        Assert.AreEqual(actualCount, 3);
      }

      Assert.IsTrue(cfg.Remove("Section1"));
      cfg.RemoveAllNamed("Section1");

      Assert.IsTrue(!cfg.Contains("Section1"));
    }

    [Test]
    public void SetValueOverload()
    {
      var cfg = new Configuration();

      object[] obj = new object[] { 1, 2, 3 };

      var setting = cfg["TestSection"]["TestSetting"];
      setting.SetValue(obj);

      // GetValue() should throw, because the setting is an array now.
      // It should notify us to use GetValueArray() instead.
      Assert.Throws<InvalidOperationException>(() =>
      {
        setting.GetValue(typeof(int));
      });

      // Now get the array object and check.
      object[] intsNonGeneric = setting.GetValueArray(typeof(int));
      int[] intsGeneric = setting.GetValueArray<int>();

      Assert.AreEqual(obj.Length, intsGeneric.Length);
      Assert.AreEqual(intsGeneric.Length, intsNonGeneric.Length);

      for (int i = 0; i < obj.Length; i++)
      {
        Assert.AreEqual(obj[i], intsGeneric[i]);
        Assert.AreEqual(intsGeneric[i], intsNonGeneric[i]);
      }
    }

    [Test]
    public void SaveAndLoadComments()
    {
      var cfgStr =
          "# Line1" + Environment.NewLine +
          "; Line2" + Environment.NewLine +
          "#" + Environment.NewLine +
          "# Line4" + Environment.NewLine +
          "[Section] # InlineComment1" + Environment.NewLine +
          "Setting = Value ; InlineComment2" + Environment.NewLine +
          Environment.NewLine +
          "# Line1   " + Environment.NewLine +
          "#Line2 " + Environment.NewLine +
          "## ###" + Environment.NewLine +
          ";Line4" + Environment.NewLine +
          "[Section2]" + Environment.NewLine +
          "Setting=\"Val;#ue\"# InlineComment3" + Environment.NewLine +
          "ValidUglySetting1 = \"this is # not a comment\" # this is a comment \"with a quote\" inside" + Environment.NewLine +
          "ValidUglySetting2 = this is \\# not a comment # this is a comment" + Environment.NewLine +
          "ValidUglySetting3 = { first, \"second # still, second\" } # comment \"with a quote\" and a closing brace }";

      var cfg = Configuration.LoadFromString(cfgStr);

      SaveAndLoadComments_Check(cfg);

      TestWithFile(cfg, filename =>
      {
        // Textual first
        cfg.SaveToFile(filename);
        cfg = Configuration.LoadFromFile(filename);
        SaveAndLoadComments_Check(cfg);

        // Now binary
        cfg.SaveToBinaryFile(filename);
        cfg = Configuration.LoadFromBinaryFile(filename);
        SaveAndLoadComments_Check(cfg);
      });
    }

    private static void SaveAndLoadComments_Check(Configuration cfg)
    {
      Assert.AreEqual(2, cfg.SectionCount);
      Assert.IsTrue(cfg.Contains("Section"));
      Assert.IsTrue(cfg.Contains("Section2"));
      Assert.IsTrue(cfg.Contains("Section", "Setting"));
      Assert.IsTrue(cfg.Contains("Section2", "Setting"));
      Assert.IsTrue(cfg.Contains("Section2", "ValidUglySetting1"));
      Assert.IsTrue(cfg.Contains("Section2", "ValidUglySetting2"));
      Assert.IsTrue(cfg.Contains("Section2", "ValidUglySetting3"));

      var section = cfg["Section"];
      var section2 = cfg["Section2"];

      Assert.IsNotNull(section.PreComment);
      Assert.IsNotNull(section2.PreComment);

      Assert.AreEqual(
          "Line1" + Environment.NewLine +
          "Line2" + Environment.NewLine +
          Environment.NewLine +
          "Line4",
          section.PreComment
          );

      Assert.AreEqual(
          "Line1" + Environment.NewLine +
          "Line2" + Environment.NewLine +
          "# ###" + Environment.NewLine +
          "Line4",
          section2.PreComment
          );

      Assert.AreEqual("InlineComment1", section.Comment);
      Assert.AreEqual("InlineComment2", section["Setting"].Comment);

      Assert.IsNull(section2.Comment);
      Assert.AreEqual("InlineComment3", section2["Setting"].Comment);
      Assert.AreEqual("this is a comment \"with a quote\" inside", section2["ValidUglySetting1"].Comment);
      Assert.AreEqual("this is a comment", section2["ValidUglySetting2"].Comment);
      Assert.AreEqual("comment \"with a quote\" and a closing brace }", section2["ValidUglySetting3"].Comment);

      Assert.AreEqual("Value", section["Setting"].StringValue);
      Assert.AreEqual("Val;#ue", section2["Setting"].StringValue);
      Assert.AreEqual("this is # not a comment", section2["ValidUglySetting1"].StringValue);
      Assert.AreEqual("this is \\# not a comment", section2["ValidUglySetting2"].StringValue);
      Assert.IsTrue(section2["ValidUglySetting3"].IsArray);
      Assert.AreEqual(2, section2["ValidUglySetting3"].ArraySize);
      Assert.AreEqual("first", section2["ValidUglySetting3"].StringValueArray[0]);
      Assert.AreEqual("second # still, second", section2["ValidUglySetting3"].StringValueArray[1]);
      
    }

    [Test]
    public void ArrayParsing()
    {
      var cfg = new Configuration();
      var section = cfg["Section"];

      section["Setting1"].StringValue = "{1,2,3}";
      section["Setting2"].StringValue = "   {1,2,3}   ";
      section["Setting3"].StringValue = " d {1,2,3} d ";
      section["Setting4"].StringValue = "{ 1,2,   3  }";
      section["Setting5"].StringValue = "{ 123, 456, 789 }";
      section["Setting6"].StringValue = "{}";
      section["Setting7"].StringValue = "{,}";
      section["Setting8"].StringValue = "{13,}";
      section["Setting9"].StringValue = "{{1},{2},{3}}";
      section["Setting10"].StringValue = "{ {123}, 456, {{789}} }";
      section["Setting11"].StringValue = "{\"12,34\", 5678}";
      section["Setting12"].StringValue = "{\"{123}\", 456}";
      section["Setting13"].StringValue = "{ \"first\"\"second\", \"\"\"third fourth\"\"\", fifth }";

      AssertArraysAreEqual(new[] { "1", "2", "3" }, section["Setting1"].StringValueArray);
      AssertArraysAreEqual(new[] { "1", "2", "3" }, section["Setting2"].StringValueArray);

      Assert.IsFalse(section["Setting3"].IsArray);
      Assert.AreEqual(" d {1,2,3} d ", section["Setting3"].StringValue);

      AssertArraysAreEqual(new[] { "1", "2", "3" }, section["Setting4"].StringValueArray);
      AssertArraysAreEqual(new[] { "123", "456", "789" }, section["Setting5"].StringValueArray);

      Assert.IsTrue(section["Setting6"].IsArray);
      Assert.AreEqual(0, section["Setting6"].ArraySize);

      Assert.IsFalse(section["Setting7"].IsArray);
      Assert.IsFalse(section["Setting8"].IsArray);

      AssertArraysAreEqual(new[] { "{1}", "{2}", "{3}" }, section["Setting9"].StringValueArray);
      AssertArraysAreEqual(new[] { "{123}", "456", "{{789}}" }, section["Setting10"].StringValueArray);
      AssertArraysAreEqual(new[] { "12,34", "5678" }, section["Setting11"].StringValueArray);
      AssertArraysAreEqual(new[] { "{123}", "456" }, section["Setting12"].StringValueArray);

      Assert.IsTrue(section["Setting13"].IsArray);
      AssertArraysAreEqual(new[] { "first\"\"second", "third fourth", "fifth" }, section["Setting13"].StringValueArray);
    }

    sealed class SectionTestObject
    {
      public string[] SomeArrayProp { get; set; }
      public string[] SomeArrayField = null;
    }

    [Test]
    public void SectionObjectMapping()
    {
      var cfg = new Configuration();

      var section = cfg["Section"];
      section["SomeArrayProp"].StringValue = "{1,2,3}";
      section["SomeArrayField"].StringValue = "{4,5,6}";

      var obj = section.ToObject<SectionTestObject>();

      AssertArraysAreEqual(new[] { "1", "2", "3" }, obj.SomeArrayProp);
      AssertArraysAreEqual(new[] { "4", "5", "6" }, obj.SomeArrayField);

      section = cfg["Section2"];
      section.Add("SomeArrayProp");
      section.Add("SomeArrayField");
      section.GetValuesFrom(obj);

      AssertArraysAreEqual(new[] { "1", "2", "3" }, section["SomeArrayProp"].StringValueArray);
      AssertArraysAreEqual(new[] { "4", "5", "6" }, section["SomeArrayField"].StringValueArray);
    }

    [Test]
    public void Floats()
    {
      var cfg = new Configuration();
      var setting = cfg["Section"]["Setting"];

      setting.FloatValue = 100.0f;
      Assert.AreEqual(setting.FloatValue, 100.0f);

      setting.FloatValue = -100.0f;
      Assert.AreEqual(setting.FloatValue, -100.0f);

      var floats = new float[] { 0.0f, 100.0f, -100.0f, 40000.0f, 20.4028f };
      setting.FloatValueArray = floats;
      AssertArraysAreEqual(setting.FloatValueArray, floats);

      TestWithFile(cfg, filename =>
      {
        cfg.SaveToFile(filename);
        cfg = Configuration.LoadFromFile(filename);
        setting = cfg["Section"]["Setting"];

        AssertArraysAreEqual(floats, setting.FloatValueArray);

        cfg.SaveToBinaryFile(filename);
        cfg = Configuration.LoadFromBinaryFile(filename);
        setting = cfg["Section"]["Setting"];

        AssertArraysAreEqual(floats, setting.FloatValueArray);
      });
    }

    [Test]
    public void Doubles()
    {
      var cfg = new Configuration();
      var setting = cfg["Section"]["Setting"];

      setting.DoubleValue = 100.0;
      Assert.AreEqual(setting.DoubleValue, 100.0);

      setting.DoubleValue = -100.0;
      Assert.AreEqual(setting.DoubleValue, -100.0);

      var doubles = new double[] { 0.0, 100.0, -100.0, 40000.0, 2004.40493028 };
      setting.DoubleValueArray = doubles;
      AssertArraysAreEqual(setting.DoubleValueArray, doubles);

      TestWithFile(cfg, filename =>
      {
        cfg.SaveToFile(filename);
        cfg = Configuration.LoadFromFile(filename);
        setting = cfg["Section"]["Setting"];

        AssertArraysAreEqual(doubles, setting.DoubleValueArray);

        cfg.SaveToBinaryFile(filename);
        cfg = Configuration.LoadFromBinaryFile(filename);
        setting = cfg["Section"]["Setting"];

        AssertArraysAreEqual(doubles, setting.DoubleValueArray);
      });
    }

    [Test]
    public void Decimals()
    {
      var cfg = new Configuration();
      var setting = cfg["Section"]["Setting"];

      setting.DecimalValue = 100.0m;
      Assert.AreEqual(setting.DoubleValue, 100.0);

      setting.DecimalValue = -100.0m;
      Assert.AreEqual(setting.DoubleValue, -100.0);

      var decimals = new decimal[] { 0.0m, 100.0m, -100.0m, 40000.0m, 2004.40493028m, decimal.MinValue, decimal.MaxValue };
      setting.DecimalValueArray = decimals;
      AssertArraysAreEqual(setting.DecimalValueArray, decimals);

      TestWithFile(cfg, filename =>
      {
        cfg.SaveToFile(filename);
        cfg = Configuration.LoadFromFile(filename);
        setting = cfg["Section"]["Setting"];

        AssertArraysAreEqual(decimals, setting.DecimalValueArray);

        cfg.SaveToBinaryFile(filename);
        cfg = Configuration.LoadFromBinaryFile(filename);
        setting = cfg["Section"]["Setting"];

        AssertArraysAreEqual(decimals, setting.DecimalValueArray);
      });
    }

    [Test]
    public void Bytes()
    {
      var cfg = new Configuration();
      var setting = cfg["Section"]["Setting"];

      setting.ByteValue = 100;
      Assert.AreEqual(setting.ByteValue, (byte)100);

      setting.ByteValue = 255;
      Assert.AreEqual(setting.ByteValue, (byte)255);

      var bytes = new byte[] { 0, 100, 255 };
      setting.ByteValueArray = bytes;
      AssertArraysAreEqual(setting.ByteValueArray, bytes);

      TestWithFile(cfg, filename =>
      {
        // Textual first
        cfg.SaveToFile(filename);
        cfg = Configuration.LoadFromFile(filename);
        setting = cfg["Section"]["Setting"];

        AssertArraysAreEqual(bytes, setting.ByteValueArray);

        // Now binary
        cfg.SaveToBinaryFile(filename);
        cfg = Configuration.LoadFromBinaryFile(filename);
        setting = cfg["Section"]["Setting"];

        AssertArraysAreEqual(bytes, setting.ByteValueArray);
      });
    }

    [Test]
    public void SBytes()
    {
      var cfg = new Configuration();
      var setting = cfg["Section"]["Setting"];

      setting.SByteValue = 100;
      Assert.AreEqual(setting.SByteValue, (sbyte)100);

      int value = 255;
      setting.SByteValue = (sbyte)value;
      Assert.AreEqual(setting.SByteValue, (sbyte)value);

      setting.IntValue = 500;
      Assert.Throws<SettingValueCastException>(() =>
      {
        sbyte value2 = setting.SByteValue;
      });

      var bytes = new sbyte[] { 0, 100, 120 };
      setting.SByteValueArray = bytes;
      AssertArraysAreEqual(setting.SByteValueArray, bytes);

      TestWithFile(cfg, filename =>
      {
        // Textual first
        cfg.SaveToFile(filename);
        cfg = Configuration.LoadFromFile(filename);
        setting = cfg["Section"]["Setting"];

        AssertArraysAreEqual(bytes, setting.SByteValueArray);

        // Now binary
        cfg.SaveToBinaryFile(filename);
        cfg = Configuration.LoadFromBinaryFile(filename);
        setting = cfg["Section"]["Setting"];

        AssertArraysAreEqual(bytes, setting.SByteValueArray);
      });
    }

    [Test]
    public void Chars()
    {
      var cfg = new Configuration();
      var setting = cfg["Section"]["Setting"];

      setting.CharValue = 'x';
      Assert.AreEqual(setting.CharValue, 'x');

      setting.CharValue = (char)190;
      Assert.AreEqual(setting.CharValue, (char)190);

      setting.CharValue = '\0';
      Assert.AreEqual(setting.CharValue, '\0');

      var chars = new char[] { 'a', 'b', '\0', '-', (char)160, (char)194, (char)240 };
      setting.CharValueArray = chars;

      AssertArraysAreEqual(chars, setting.CharValueArray);

      TestWithFile(cfg, filename =>
      {
        // Textual first
        cfg.SaveToFile(filename);
        cfg = Configuration.LoadFromFile(filename);
        setting = cfg["Section"]["Setting"];

        AssertArraysAreEqual(chars, setting.CharValueArray);

        // Now binary
        cfg.SaveToBinaryFile(filename);
        cfg = Configuration.LoadFromBinaryFile(filename);
        setting = cfg["Section"]["Setting"];

        AssertArraysAreEqual(chars, setting.CharValueArray);
      });
    }

    [Test]
    public void SupressArrayParsing() 
    {
      var section = "General";
      var name = "AutoReplacedTexts";
      var value = "{1=one, 2=two, 3=three}";
      Configuration.SupressArrayParsing = true;
      var cfg = Configuration.LoadFromString($"[{section}]\r\n{name} = {value}");
      Assert.AreEqual(value, cfg[section][name].StringValue);
    }

    [Test]
    public void GetValueOrDefault()
    {
      var cfg = new Configuration();
      var setting = cfg["Section"]["Setting"];

      /* Test all the converters with valid and invalid values:
       * bool, byte, char, datetime, decimal, double, enum, int16, int32,
       * int64, sbyte, single, uint16, uint32, uint64
       * Use explicit type argument specification in all cases even though
       * it is not always necessary. */

      #region Bool

      setting.BoolValue = true; // valid value
      Assert.AreEqual(setting.GetValueOrDefault<bool>(false), true);
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<bool>(false), false);
      setting.GetValueOrDefault<bool>(false, true); // test setDef
      Assert.AreEqual(setting.BoolValue, false);

      #endregion
      #region Byte

      setting.ByteValue = 100; // valid value
      Assert.AreEqual(setting.GetValueOrDefault<byte>(200), 100);
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<byte>(200), 200);
      setting.GetValueOrDefault<byte>(200, true); // test setDef
      Assert.AreEqual(setting.ByteValue, 200);

      #endregion
      #region Char

      setting.CharValue = 'c'; // valid value
      Assert.AreEqual(setting.GetValueOrDefault<char>('f'), 'c');
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<char>('f'), 'f');
      setting.GetValueOrDefault<char>('f', true); // test setDef
      Assert.AreEqual(setting.CharValue, 'f');

      #endregion
      #region DateTime

      // Some problems with DateTime.ToString omitting milliseconds when DateTime.Now was used as test value.
      setting.DateTimeValue = DateTime.Today; // valid value
      Assert.AreEqual(setting.GetValueOrDefault<DateTime>(DateTime.MinValue), DateTime.Today);
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<DateTime>(DateTime.MinValue), DateTime.MinValue);
      setting.GetValueOrDefault<DateTime>(DateTime.MinValue, true); // test setDef
      Assert.AreEqual(setting.DateTimeValue, DateTime.MinValue);

      #endregion
      #region Decimal

      setting.DecimalValue = 2004.40493028m; // valid value
      Assert.AreEqual(setting.GetValueOrDefault<decimal>(1000.2028m), 2004.40493028m);
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<decimal>(1000.2028m), 1000.2028m);
      setting.GetValueOrDefault<decimal>(1000.2028m, true); // test setDef
      Assert.AreEqual(setting.DecimalValue, 1000.2028m);

      #endregion
      #region Double

      setting.DoubleValue = 404.404; // valid value
      Assert.AreEqual(setting.GetValueOrDefault<double>(123.456), 404.404);
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<double>(123.456), 123.456);
      setting.GetValueOrDefault<double>(123.456, true); // test setDef
      Assert.AreEqual(setting.DoubleValue, 123.456);

      #endregion
      #region Enum

      // Chose a random enum
      setting.SetValue(GCNotificationStatus.NotApplicable); // valid value
      Assert.AreEqual(setting.GetValueOrDefault<GCNotificationStatus>(GCNotificationStatus.Succeeded), GCNotificationStatus.NotApplicable);
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<GCNotificationStatus>(GCNotificationStatus.Succeeded), GCNotificationStatus.Succeeded);
      setting.GetValueOrDefault<GCNotificationStatus>(GCNotificationStatus.Succeeded, true); // test setDef
      Assert.AreEqual(setting.GetValue(typeof(GCNotificationStatus)), GCNotificationStatus.Succeeded);

      #endregion
      #region Int16

      setting.SetValue((short)123); // valid value
      Assert.AreEqual(setting.GetValueOrDefault<short>(456), 123);
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<short>(456), 456);
      setting.GetValueOrDefault<short>(456, true); // test setDef
      Assert.AreEqual(setting.GetValue(typeof(short)), 456);

      #endregion
      #region Int32

      setting.IntValue = 4567; // valid value
      Assert.AreEqual(setting.GetValueOrDefault<int>(1010), 4567);
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<int>(1010), 1010);
      setting.GetValueOrDefault<int>(1010, true); // test setDef
      Assert.AreEqual(setting.IntValue, 1010);

      #endregion
      #region Int64

      setting.SetValue((long)75467456); // valid value
      Assert.AreEqual(setting.GetValueOrDefault<long>(14623146), 75467456);
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<long>(14623146), 14623146);
      setting.GetValueOrDefault<long>(14623146, true); // test setDef
      Assert.AreEqual(setting.GetValue(typeof(long)), 14623146);

      #endregion
      #region SByte

      setting.SByteValue = 123; // valid value
      Assert.AreEqual(setting.GetValueOrDefault<sbyte>(-123), 123);
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<sbyte>(-123), -123);
      setting.GetValueOrDefault<sbyte>(-123, true); // test setDef
      Assert.AreEqual(setting.SByteValue, -123);

      #endregion
      #region Single

      setting.FloatValue = 123.456f; // valid value
      Assert.AreEqual(setting.GetValueOrDefault<float>(-456.123f), 123.456f);
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<float>(-456.123f), -456.123f);
      setting.GetValueOrDefault<float>(-456.123f, true); // test setDef
      Assert.AreEqual(setting.FloatValue, -456.123f);

      #endregion
      #region String

      // Test that double quotation marks are trimmed properly
      setting.StringValue = "\"string\"";
      Assert.AreEqual(setting.GetValueOrDefault<string>("default"), "string");
      setting.StringValue = "\"\"\"Triple quotes\"\"\"";
      Assert.AreEqual(setting.GetValueOrDefault<string>("default"), "Triple quotes");
      setting.GetValueOrDefault<string>("\"\"\"Triple quotes\"\"\"", true); // test setDef
      Assert.AreEqual(setting.StringValue, "Triple quotes");

      #endregion
      #region UInt16

      setting.SetValue((ushort)1000); // valid value
      Assert.AreEqual(setting.GetValueOrDefault<ushort>(2000), 1000);
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<ushort>(2000), 2000);
      setting.GetValueOrDefault<ushort>(2000, true); // test setDef
      Assert.AreEqual(setting.GetValue(typeof(ushort)), 2000);

      #endregion
      #region UInt32

      setting.SetValue((uint)12345); // valid value
      Assert.AreEqual(setting.GetValueOrDefault<uint>(54321), 12345);
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<uint>(54321), 54321);
      setting.GetValueOrDefault<uint>(54321, true); // test setDef
      Assert.AreEqual(setting.GetValue(typeof(uint)), 54321);

      #endregion
      #region UInt64

      setting.SetValue((ulong)1234567); // valid value
      Assert.AreEqual(setting.GetValueOrDefault<ulong>(7654321), 1234567);
      setting.SetValue("invalid value"); // invalid value
      Assert.AreEqual(setting.GetValueOrDefault<ulong>(7654321), 7654321);
      setting.GetValueOrDefault<ulong>(7654321, true); // test setDef
      Assert.AreEqual(setting.GetValue(typeof(ulong)), 7654321);

      #endregion
    }

    private static void TestWithFile(Configuration cfg, Action<string> action)
    {
      var filename = Path.GetTempFileName();
      try
      {
        action(filename);
      }
      finally
      {
        if (File.Exists(filename))
          File.Delete(filename);
      }
    }

    private static void AssertArraysAreEqual<T>(T[] expected, T[] actual)
    {
      Assert.AreEqual(expected.Length, actual.Length);
      for (int i = 0; i < expected.Length; ++i)
        Assert.AreEqual(expected[i], actual[i]);
    }
  }
}
