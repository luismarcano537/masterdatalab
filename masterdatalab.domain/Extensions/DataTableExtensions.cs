using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace masterdatalab.domain.Extensions
{
    public static class DataTableExtensions
    {
        public static List<T>? ToModelList<T>(this DataTable dataTable, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            var serialized = JsonConvert.SerializeObject(dataTable, jsonSerializerSettings);

            return JsonConvert.DeserializeObject<List<T>>(serialized, jsonSerializerSettings);
        }
        public static T? ToModel<T>(this DataTable dataTable, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            if (dataTable.Rows.Count == 0) return default;

            var serialized = JsonConvert.SerializeObject(dataTable);
            var list = JsonConvert.DeserializeObject<List<T>>(serialized, jsonSerializerSettings);

            return list.FirstOrDefault();
        }
    }
}
