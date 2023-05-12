using Newtonsoft.Json;
using STC.Models;

namespace STC.Helpers
{
    public class HelperModelPartidoYEventosConverter : JsonConverter<ModelPartidoCompleto>
    {
        public override void WriteJson(JsonWriter writer, ModelPartidoCompleto value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            // Serializar la propiedad "partido"
            writer.WritePropertyName("partido");
            serializer.Serialize(writer, value.partido);

            // Serializar la propiedad "eventos"
            writer.WritePropertyName("eventos");
            serializer.Serialize(writer, value.eventos);

            writer.WriteEndObject();
        }

        public override ModelPartidoCompleto ReadJson(JsonReader reader, Type objectType, ModelPartidoCompleto existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            ModelPartidoCompleto partidoYEventos = new ModelPartidoCompleto();

            // Avanzar el lector de JSON al inicio del objeto
            reader.Read();

            // Leer las propiedades del objeto
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    string propertyName = reader.Value.ToString();

                    // Asignar la propiedad "partido"
                    if (propertyName == "partido")
                    {
                        partidoYEventos.partido = serializer.Deserialize<Partido>(reader);
                    }

                    // Asignar la propiedad "eventos"
                    else if (propertyName == "eventos")
                    {
                        partidoYEventos.eventos = serializer.Deserialize<List<Evento>>(reader);
                    }
                }
                else if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }
            }

            return partidoYEventos;
        }
    }
}
