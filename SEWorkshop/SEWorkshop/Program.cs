using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SEWorkshop
{
    public class Program
    {
        [Obsolete]
        public static void Main(string[] args) {
            string schemaJson = @"{
                'description': 'Register',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'username': {'type':'string', 'required': 'true'},
                    'password': {'type':'string', 'required': 'true'}
                },
                'additionalProperties': false
            }";

            JsonSchema schema = JsonSchema.Parse(schemaJson);

            string jsonFile = File.ReadAllText("ActionsFile.json");
            List<JObject> actions = JsonConvert.DeserializeObject<List<JObject>>(jsonFile);
            if (actions == null)
                return;

            foreach (JObject action in actions)
            {
                string command = action.First.Last.ToString();
                bool valid = action.IsValid(schema);
            }
        }
    }
}
