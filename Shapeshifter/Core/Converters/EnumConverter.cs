using System;

namespace Shapeshifter.Core.Converters
{
    internal class EnumConverter
    {
        [Serializer(typeof(Enum), ForAllDescendants=true)]
        public static void Serialize(IShapeshifterWriter writer, Enum enumValue)
        {
            writer.Write(Constants.EnumValueKey, enumValue.ToString("G"));
        }

        [Deserializer(typeof(Enum), ForAllDescendants = true)]
        public static object Deserialize(IShapeshifterReader reader, Type targetType)
        {
            var valueAsString = reader.Read<string>(Constants.EnumValueKey);
            return Enum.Parse(targetType, valueAsString, false);
        }
    }
}