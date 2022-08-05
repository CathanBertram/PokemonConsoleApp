using Newtonsoft.Json.Linq;
using System.Net;

namespace ConsoleApp2
{
    public static class PokemonDatabase
    {
        private static int maxPokedexIndex = 898;
        private static string[] types = new string[18] { "normal", "fighting", "flying", "poison", "ground", "rock", "bug", "ghost", "steel", "fire", "water", "grass", "electric", "psychic", "ice", "dragon", "dark", "fairy" };
        private static float[,] typeMatrix = new float[18, 18]
        {
            {1,1,1,1,1,0.5f,1,0,0.5f,1,1,1,1,1,1,1,1,1},
            {2,1,0.5f,0.5f,1,2,0.5f,0,2,1,1,1,1,0.5f,2,1,2,0.5f},
            {1,2,1,1,1,0.5f,2,1,0.5f,1,1,2,0.5f,1,1,1,1,1},
            {1,1,1,0.5f,0.5f,0.5f,1,0.5f,0,1,1,2,1,1,1,1,1,2},
            {1,1,0,2,1,2,0.5f,1,2,2,1,0.5f,2,1,1,1,1,1},
            {1,0.5f,2,1,0.5f,1,2,1,0.5f,2,1,1,1,1,2,1,1,1},
            {1,0.5f,0.5f,0.5f,1,1,1,0.5f,0.5f,0.5f,1,2,1,2,1,1,2,0.5f},
            {0,1,1,1,1,1,1,2,1,1,1,1,1,2,1,1,0.5f,1},
            {1,1,1,1,1,2,1,1,0.5f,0.5f,0.5f,1,0.5f,1,2,1,1,2},
            {1,1,1,1,1,0.5f,2,1,2,0.5f,0.5f,2,1,1,2,0.5f,1,1},
            {1,1,1,1,2,2,1,1,1,2,0.5f,0.5f,1,1,1,0.5f,1,1},
            {1,1,0.5f,0.5f,2,2,0.5f,1,0.5f,0.5f,2,0.5f,1,1,1,0.5f,1,1},
            {1,1,2,1,0,1,1,1,1,1,2,0.5f,0.5f,1,1,0.5f,1,1},
            {1,2,1,2,1,1,1,1,0.5f,1,1,1,1,0.5f,1,1,0,1},
            {1,1,2,1,2,1,1,1,0.5f,0.5f,0.5f,2,1,1,0.5f,2,1,1,},
            {1,1,1,1,1,1,1,1,0.5f,1,1,1,1,1,1,2,1,0},
            {1,0.5f,1,1,1,1,1,2,1,1,1,1,1,2,1,1,0.5f,0.5f},
            {1,2,1,0.5f,1,1,1,1,0.5f,0.5f,1,1,1,1,1,2,2,1}
        };

        private static List<Pokemon> pokemon = new List<Pokemon>();
        private static Dictionary<string, Move> moves = new Dictionary<string, Move>();
        public static void Start()
        {
            var filePaths = Directory.GetFiles("Moves");
            
            foreach (var path in filePaths)
            {
                var data = File.ReadAllText(path);
                var m = new Move(data);
                moves.Add(m.name, m);
            }
        }
        public static Pokemon GetRandomPokemon(bool maxStats = false)
        {
            var rand = new System.Random();
            var id = rand.Next(1, maxPokedexIndex);
            return GetPokemon(id, maxStats);
        }
        //public static Pokemon GetPokemon(string name)
        //{
        //    var p = pokemon.Where(x => x.name == name).ToList();
        //    if (p != null)        
        //        return p[0];

        //    return null;
        //}
        //public static bool TryGetPokemon(string name, out Pokemon p)
        //{
        //    p = null;
        //    var pList = pokemon.Where(x => x.name == name).ToList();
        //    if (pList != null)
        //    {
        //        p = pList[0];
        //        return true;
        //    }

        //    return false;
        //}

        //public static Pokemon GetPokemon(int id)
        //{
        //    var p = pokemon.Where(x => x.id == id).ToList();
        //    if (p != null)
        //        return p[0];

        //    return null;
        //}

        //public static bool TryGetPokemon(int id, out Pokemon p)
        //{
        //    p = null;
        //    var pList = pokemon.Where(x => x.id == id).ToList();
        //    if (pList != null)
        //    {
        //        p = pList[0];
        //        return true;
        //    }

        //    return false;
        //}

        public static float GetTypeDamage(string attackingType, string defendingType)
        {
            return typeMatrix[Array.IndexOf(types, attackingType), Array.IndexOf(types, defendingType)];
        }
        #region API
        public static Move GetMove(string moveName, string moveURL)
        {
            if (moves.ContainsKey(moveName))
                return moves[moveName];

            if (!Directory.Exists("Moves"))
                Directory.CreateDirectory("Moves");

            if (!File.Exists($"Moves/{moveName}.json"))
                SaveJson($"Moves/{moveName}.json", GetMoveFromAPI(moveURL));

            var data = File.ReadAllText($"Moves/{moveName}.json");

            var m = new Move(data);

            return m;
        }
        private static string GetMoveFromAPI(string url)
        {
            //Create WebRequest
            WebRequest request = WebRequest.Create(url);

            //Get WebResponse from WebRequest
            WebResponse response = request.GetResponse();

            //Read data from response
            Stream data = response.GetResponseStream();

            //Create StreamReader from response data
            StreamReader reader = new StreamReader(data);

            //Read data from StreamReader
            string responseFromServer = reader.ReadToEnd();

            response.Close();
            return responseFromServer;
        }
        private static Pokemon GetPokemon(int id, bool maxStats)
        {
            if (!Directory.Exists("Pokemon"))
                Directory.CreateDirectory("Pokemon");
            int newID = id;
            if (!File.Exists($"Pokemon/{id}.json"))
            {
                var fileData = GetPokemonFromAPI(id, out newID);
                SaveJson($"Pokemon/{newID}.json", fileData);
            }

            var data = File.ReadAllText($"Pokemon/{newID}.json");

            var p = new Pokemon(data, maxStats);

            return p;
        }
        private static Pokemon GetPokemon(string name)
        {
            if (!Directory.Exists("Pokemon"))
                Directory.CreateDirectory("Pokemon");

            if (!File.Exists($"Pokemon/{name}.json"))
                SaveJson($"Pokemon/{name}.json", GetPokemonFromAPI(name));

            var data = File.ReadAllText($"Pokemon/{name}.json");

            var p = new Pokemon(data);

            return p;
        }
        private static void SaveJson(string filePath, string fileData)
        {
            var data = JObject.Parse(fileData);
            File.WriteAllText(filePath, data.ToString());
        }
        private static string GetPokemonFromAPI(int id, out int newID)
        {
            newID = id;
            try
            {
                //URL for HTTP API Request
                string url = $"https://pokeapi.co/api/v2/pokemon/{id}/";

                //Create WebRequest
                WebRequest request = WebRequest.Create(url);

                //Get WebResponse from WebRequest
                WebResponse response = request.GetResponse();

                //Read data from response
                Stream data = response.GetResponseStream();

                //Create StreamReader from response data
                StreamReader reader = new StreamReader(data);

                //Read data from StreamReader
                string responseFromServer = reader.ReadToEnd();

                response.Close();
                return responseFromServer;
            }
            catch (Exception)
            {
                Events.AddMessage($"Pokemon with {id} returned null");
                newID = Random.Next(1, maxPokedexIndex);
                return GetPokemonFromAPI(newID, out newID);        
            }
            
        }

        private static string GetPokemonFromAPI(string name)
        {
            //URL for HTTP API Request
            string url = $"https://pokeapi.co/api/v2/pokemon/{name}/";

            //Create WebRequest
            WebRequest request = WebRequest.Create(url);

            //Get WebResponse from WebRequest
            WebResponse response = request.GetResponse();

            //Read data from response
            Stream data = response.GetResponseStream();

            //Create StreamReader from response data
            StreamReader reader = new StreamReader(data);

            //Read data from StreamReader
            string responseFromServer = reader.ReadToEnd();

            response.Close();
            return responseFromServer;
        }
        #endregion
    }
}
