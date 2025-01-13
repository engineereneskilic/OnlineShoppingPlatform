using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace OnlineShoppingPlatform.Business.Validations
{
    public class CustomDateTimeConverter : Newtonsoft.Json.JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            // Eğer değer null ise, JSON'a yazma
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // Eğer değer DateTime ise, sadece tarihi "yyyy-MM-dd" formatında yaz
            if (value is DateTime dateTime)
            {
                writer.WriteValue(dateTime.ToLocalTime().ToString("yyyy-MM-dd"));
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            // Tarih kısmını okuyup DateTime'a dönüştür
            if (reader.Value != null)
            {
                return DateTime.Parse(reader.Value.ToString());
            }

            return null; // Null değeri işle
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }
    }
}
