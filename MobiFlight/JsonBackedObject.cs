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
        /// <summary>
        /// Loads objects from a list of JSON files on disk, migrating each one and validating against the provided schema.
        /// </summary>
        /// <typeparam name="T">The type of object to create from the JSON files. Must implement the IMigrateable interface.</typeparam>
        /// <param name="definitionFiles">A list of JSON files to load.</param>
        /// <param name="schemaFile">The path to the JSON schema to use for validation.</param>
        /// <param name="onSuccess">Method called on each successful object creation.</param>
        /// <param name="onError">Method called on each failure.</param>
        /// <returns>A list of the valid objects.</returns>
        public static List<T> LoadDefinitions<T>(string[] definitionFiles, string schemaFile, Action<T, string> onSuccess, Action onError) where T : IMigrateable
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
                onError?.Invoke();
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
                        onError?.Invoke();
                        continue;
                    }

                    result.Add(definition);
                    onSuccess?.Invoke(definition, definitionFile);
                }
                catch (Exception ex)
                {
                    onError?.Invoke();
                    Log.Instance.log($"Unable to load {definitionFile}: {ex.Message}", LogSeverity.Error);
                }
            }

            return result;
        }

        /// <summary>
        /// Loads an object from a JSON file, migrating it and validating it against the provided schema.
        /// </summary>
        /// <typeparam name="T">The type of object to create. Must implement IMigrateable.</typeparam>
        /// <param name="path">Path to the JSON file to load.</param>
        /// <param name="schema">Schema to validate the object against.</param>
        /// <returns>The created object, or null if the object creation failed (due to schema validation failure or other error).</returns>
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
