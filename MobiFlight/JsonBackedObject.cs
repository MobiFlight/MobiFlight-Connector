using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;

namespace MobiFlight
{
    public interface IMigrateable
    {
        void Migrate();
    }

    public class JsonBackedObject
    {
        public static List<T> LoadDefinitions<T>(string[] definitionFiles, string schemaFile, Action<T> onSuccess) where T : IMigrateable
        {
            var result = new List<T>();
            JSchema schema;

            try
            {
                schema = JSchema.Parse(File.ReadAllText(schemaFile));
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Unable to load schema file {schemaFile}: {ex.Message}", LogSeverity.Error);
                return result;
            }
                
            foreach (var definitionFile in definitionFiles)
            {

                try
                {
                    var definition = JsonBackedObject.LoadFromFile<T>(definitionFile, schema);

                    // Boards that fail to load already logged the necessary errors in LoadFromFile()
                    // so just continue to the next file.
                    if (definition == null)
                    {
                        continue;
                    }

                    result.Add(definition);
                    onSuccess?.Invoke(definition);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Unable to load {definitionFile}: {ex.Message}", LogSeverity.Error);
                }
            }

            return result;
        }

        private static T LoadFromFile<T>(string path, JSchema schema) where T : IMigrateable
        {
            // Load the inital, un-migrated, file, then migrate it. Migration has to
            // happen before schema validation to ensure old files that have an upgrade
            // flow won't fail validation.
            var definition = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            definition.Migrate();

            // Now that T is migrated it can be converted back to a JObject for schema validation.
            // The NullValueHandling must be set to ignore otherwise the converted JObject will have a bunch of
            // null values that cause schema validation to fail as invalid values.
            var rawJson = JObject.FromObject(definition, new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });

            // Actually validate against the schema. If the schema validation fails then continue
            // to the next file, otherwise add the board to the list of known board definitions.
            var jsonIsValid = rawJson.IsValid(schema, out IList<string> validationMessages);

            if (!jsonIsValid)
            {
                Log.Instance.log($"{path} isn't valid:", LogSeverity.Error);
                foreach (var message in validationMessages)
                {
                    Log.Instance.log(message, LogSeverity.Error);
                }

                return default;
            }

            return definition;
        }
    }
}
