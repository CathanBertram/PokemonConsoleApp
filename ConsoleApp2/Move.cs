using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Move
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public int accuracy { get; private set; }
        public int pp { get; private set; }
        public int power { get; private set; }
        public string damageType { get; private set;}
        public string moveType { get; private set; }

        public Move(string fileData)
        {
            dynamic moveJson = JObject.Parse(fileData);
            id = (int)moveJson["id"];
            name = (string)moveJson["name"];
            if (moveJson["accuracy"] == null)
                accuracy = 100;
            else
                accuracy = (int)moveJson["accuracy"];
            pp = (int)moveJson["pp"];
            if (moveJson["power"] == null)
                power = 0;
            else
                power = (int)moveJson["power"];
            damageType = (string)moveJson["damage_class"]["name"];
            moveType = (string)moveJson["type"]["name"];
        }
    }
}
