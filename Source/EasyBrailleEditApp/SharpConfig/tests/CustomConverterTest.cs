// Copyright (c) 2013-2018 Cemalettin Dervis, MIT License.
// https://github.com/cemdervis/SharpConfig

using System;
using SharpConfig;
using NUnit.Framework;

namespace Tests
{
  class Person
  {
    public string Name { get; set; }
    public int Age { get; set; }
  }

  class PersonStringConverter : TypeStringConverter<Person>
  {
    // This method is responsible for converting a Person object to a string.
    public override string ConvertToString(object value)
    {
      var person = (Person)value;
      return string.Format("[{0};{1}]", person.Name, person.Age);
    }

    // This method attempts to convert the value to a Person object.
    // It is used instead when Setting.GetOrDefault<T> is called.
    public override object TryConvertFromString(string value, Type hint)
    {
      try
      {
        var split = value.Trim('[', ']').Split(';');

        var person = new Person();
        person.Name = split[0];
        person.Age = int.Parse(split[1]);

        return person;
      }
      catch
      {
        return null;
      }
    }
  }

  [TestFixture]
  public sealed class CustomConverterTest
  {
    [Test]
    public void CustomConverter()
    {
      Configuration.RegisterTypeStringConverter(new PersonStringConverter());

      var p = new Person()
      {
        Name = "TestPerson",
        Age = 123
      };

      var cfg = new Configuration();
      cfg["TestSection"]["Person"].SetValue(p);

      var pp = cfg["TestSection"]["Person"].GetValue<Person>();

      Assert.AreEqual(p.Name, pp.Name);
      Assert.AreEqual(p.Age, pp.Age);
    }
  }
}
