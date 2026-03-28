using System.Text.Json;
using Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Converters;

public class LocalizedStringConverter() : ValueConverter<LocalizedString, string>(
    x => JsonSerializer.Serialize(x, (JsonSerializerOptions?)null),
    x => JsonSerializer.Deserialize<LocalizedString>(x, (JsonSerializerOptions?)null)!);

