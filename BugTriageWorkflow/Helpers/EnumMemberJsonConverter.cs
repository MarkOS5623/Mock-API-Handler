using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BugTriageWorkflow.Helpers;

/// <summary>
/// JSON converter that respects [EnumMember(Value = "...")] attributes
/// for System.Text.Json serialization/deserialization.
/// Falls back to enum name if EnumMember attribute is not present.
/// </summary>
public class EnumMemberJsonConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum {
    private readonly Dictionary<TEnum, string> _enumToString = new();
    private readonly Dictionary<string, TEnum> _stringToEnum = new();

    public EnumMemberJsonConverter() {
        var type = typeof(TEnum);

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static)) {
            var enumValue = (TEnum)field.GetValue(null)!;
            var enumMemberAttr = field.GetCustomAttribute<EnumMemberAttribute>();
            var stringValue = enumMemberAttr?.Value ?? field.Name;

            _enumToString[enumValue] = stringValue;
            _stringToEnum[stringValue] = enumValue;

            // Also support the enum field name itself (PascalCase)
            _stringToEnum[field.Name] = enumValue;

            // Also support case-insensitive lookup
            _stringToEnum[stringValue.ToLowerInvariant()] = enumValue;
            _stringToEnum[stringValue.ToUpperInvariant()] = enumValue;
            _stringToEnum[field.Name.ToLowerInvariant()] = enumValue;
        }
    }

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var stringValue = reader.GetString();

        if (string.IsNullOrWhiteSpace(stringValue)) {
            throw new JsonException($"Cannot convert empty string to {typeof(TEnum).Name}");
        }

        if (_stringToEnum.TryGetValue(stringValue, out var enumValue)) {
            return enumValue;
        }

        // Try case-insensitive
        if (_stringToEnum.TryGetValue(stringValue.ToLowerInvariant(), out enumValue)) {
            return enumValue;
        }

        throw new JsonException($"Unable to convert \"{stringValue}\" to enum {typeof(TEnum).Name}");
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options) {
        if (_enumToString.TryGetValue(value, out var stringValue)) {
            writer.WriteStringValue(stringValue);
        } else {
            writer.WriteStringValue(value.ToString());
        }
    }
}
