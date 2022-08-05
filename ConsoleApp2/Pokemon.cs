using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Pokemon
    {
        public string name { get; private set; }
        public int id { get; private set; }
        public int health { get; private set; }
        public int physAtk { get; private set; }
        public int physDef { get; private set; }
        public int specAtk { get; private set; }
        public int specDef { get; private set; }
        public int speed { get; private set; }

        public int baseHealth{get; private set;}
        public int basePhysAtk{get; private set;}
        public int basePhysDef{get; private set;}
        public int baseSpecAtk{get; private set;}
        public int baseSpecDef{get; private set;}
        public int baseSpeed{get; private set;}
        public int healthIV{get; private set;}
        public int physAtkIV{get; private set;}
        public int physDefIV{get; private set;}
        public int specAtkIV{get; private set;}
        public int specDefIV{get; private set;}
        public int speedIV{get; private set;}
        public int healthEV{get; private set;}
        public int physAtkEV{get; private set;}
        public int physDefEV{get; private set;}
        public int specAtkEV{get; private set;}
        public int specDefEV{get; private set;}
        public int speedEV{get; private set;}

        public string[] types { get; private set; }
        public Move[] moves;

        public double maxHealth { get; private set; }
        public double curHealth { get; private set; }
        public int curStreak;
        public int level { get; private set; } = 1;

        public List<Pokemon> defeatedPokemon = new List<Pokemon>();
        public void Heal()
        {
            curHealth += maxHealth * Static.healPercent;
            if (curHealth > maxHealth)
                curHealth = maxHealth;
        }
        public Pokemon(string fileData, bool maxStats = false)
        {
            dynamic pokemonJson = JObject.Parse(fileData);
            id = (int)pokemonJson["id"];
            name = (string)pokemonJson["species"]["name"];

            var stats = pokemonJson["stats"];

            foreach (var item in stats)
            {
                string n = item["stat"]["name"];
                n = n.Replace("{", "");
                n = n.Replace("}", "");
                switch (n)
                {
                    case "hp":
                        baseHealth = item["base_stat"];
                        break;
                    case "attack":
                        basePhysAtk = item["base_stat"];
                        break;
                    case "defense":
                        basePhysDef = item["base_stat"];
                        break;
                    case "special-attack":
                        baseSpecAtk = item["base_stat"];
                        break;
                    case "special-defense":
                        baseSpecDef = item["base_stat"];
                        break;
                    case "speed":
                        baseSpeed = item["base_stat"];
                        break;
                    default:
                        break;
                }
            }

            var jTypes = pokemonJson["types"];
            types = new string[jTypes.Count];
            types[0] = jTypes[0]["type"]["name"];
            if (types.Count() == 2)
                types[1] = jTypes[1]["type"]["name"];
            level = Static.level;
            if (maxStats)
            {
                healthIV = 31;
                physAtkIV = 31;
                physDefIV = 31;
                specAtkIV = 31;
                specDefIV = 31;
                speedIV = 31;
            }
            else
            {
                healthIV = Random.Next(0, 31);
                physAtkIV = Random.Next(0, 31);
                physDefIV = Random.Next(0, 31);
                specAtkIV = Random.Next(0, 31);
                specDefIV = Random.Next(0, 31);
                speedIV = Random.Next(0, 31);
            }
            GetRandomEVS(maxStats);
            CalculateStats();
            GetMoves(pokemonJson, false);
            Start();
            Events.AddMessage($"Pokemon Spawned - {name}");
            for (int i = 0; i < moves.Count(); i++)
            {
                Events.AddMessage($"Move {i + 1} is {moves[i].name}");
            }
        }
        public Move PickMove(Pokemon defendingPokemon)
        {
            Move selectedMove = null;
            double curDamage = 0;
            foreach (var move in moves)
            {
                var estDamage = EstimateDamage(move, defendingPokemon);

                if (estDamage > curDamage)
                {
                    curDamage = estDamage;
                    selectedMove = move;
                }
            }

            if (selectedMove == null)
                selectedMove = moves[Random.Next(0, moves.Count())];
            else
                Events.AddMessage($"Best Move Is {selectedMove.name}");
            return selectedMove;
        }
        private double EstimateDamage(Move move, Pokemon defendingPokemon = null)
        {
            float typeMulti;
            if (defendingPokemon == null)
            {
                typeMulti = 1;
            }
            else
            {
                float type1 = PokemonDatabase.GetTypeDamage(move.moveType, defendingPokemon.types[0]);
                float type2 = 1;
                if (defendingPokemon.types.Count() > 1)
                    type2 = PokemonDatabase.GetTypeDamage(move.moveType, defendingPokemon.types[1]);
                typeMulti = type1 * type2;
            }
            
            var power = move.power;
            float stab = 1;
            foreach (var type in types)
            {
                if (move.moveType == type)
                    stab = 1.5f;
            }

            var damageType = physAtk;
            if (move.damageType == "special")
                damageType = specAtk;

            return ((((2 * level) / 5 + 2) * power * damageType) / 50 + 2) * stab * typeMulti;
        }
        public Move GetRandomMove()
        {
            return moves[Random.Next(0, moves.Length - 1)];
        }
        private void GetRandomEVS(bool maxStats)
        {
            int[] values = new int[6];
            if (!maxStats)
            {
                int evsToGive = Random.Next(0, 510);

                for (int i = 0; i < 6; i++)
                {
                    if (evsToGive > 255)
                        values[i] = Random.Next(0, 255);
                    else
                        values[i] = Random.Next(0, evsToGive);

                    evsToGive -= values[i];
                }

                Random.Shuffle(values);
            }
            else
            {
                if (basePhysAtk > baseSpecAtk)
                {
                    values[0] = 0;
                    values[1] = 252;
                    values[2] = 0;
                    values[3] = 6;
                    values[4] = 0;
                    values[5] = 252;
                }
                else
                {
                    values[0] = 0;
                    values[1] = 6;
                    values[2] = 0;
                    values[3] = 252;
                    values[4] = 0;
                    values[5] = 252;
                }
                
            }

            healthEV = values[0];
            physAtkEV = values[1];
            physDefEV = values[2];
            specAtkEV = values[3];
            specDefEV = values[4];
            speedEV = values[5];
        }
        private void CalculateStats()
        {
            health = (((2 * baseHealth + healthIV + (healthEV / 4)) * level) / 100) + level + 10;
            physAtk = (((2 * basePhysAtk + physAtkIV + (physAtkEV / 4)) * level) / 100) + 5;
            physDef = (((2 * basePhysDef + physDefIV + (physDefEV / 4)) * level) / 100) + 5;
            specAtk = (((2 * baseSpecAtk + specAtkIV + (specAtkEV / 4)) * level) / 100) + 5;
            specDef = (((2 * baseSpecDef + specDefIV + (specDefEV / 4)) * level) / 100) + 5;
            speed = (((2 * baseSpeed + speedIV + (speedEV / 4)) * level) / 100) + 5;
        }
        private void GetMoves(dynamic pokemonJson, bool random = true)
        {      
            dynamic moveList = pokemonJson["moves"];
            List<int> moveIndexes = new List<int>();
            for (int i = 0; i < moveList.Count; i++)
            {
                if (moveList[i]["version_group_details"][0]["level_learned_at"] <= level)
                {
                    moveIndexes.Add(i);
                }
            }

            if (random)
            {
                int moveCount2 = 4;
                if (moveIndexes.Count() < moveCount2)
                    moveCount2 = moveIndexes.Count();

                moves = new Move[moveCount2];

                for (int i = 0; i < moves.Count(); i++)
                {
                    var index = Random.Next(0, moveIndexes.Count());
                    var moveIndex = moveIndexes[index];
                    string name = moveList[moveIndex]["move"]["name"];
                    string url = moveList[moveIndex]["move"]["url"];

                    name = name.Replace("{", "");
                    name = name.Replace("}", "");
                    url = url.Replace("{", "");
                    url = url.Replace("}", "");
                    moves[i] = PokemonDatabase.GetMove(name, url);
                    moveIndexes.Remove(index);
                }
                return;
            }
            var tempMoves = new List<Move>();
            foreach (var item in moveIndexes)
            {
                string name = moveList[item]["move"]["name"];
                string url = moveList[item]["move"]["url"];

                name = name.Replace("{", "");
                name = name.Replace("}", "");
                url = url.Replace("{", "");
                url = url.Replace("}", "");
                tempMoves.Add(PokemonDatabase.GetMove(name, url));
            }
            var sortedMoves = tempMoves.OrderBy(x => EstimateDamage(x)).ToList();
            sortedMoves.Reverse();
            int moveCount = 4;
            if (sortedMoves.Count() < moveCount)
                moveCount = sortedMoves.Count();

            moves = new Move[moveCount];

            for (int i = 0; i < moves.Count(); i++)
            {
                moves[i] = sortedMoves[i];
            }
           

        }
    
        public void Start()
        {
            maxHealth = health;
            curHealth = health;
        }
        public void TakeDamage(Pokemon damager, Move move)
        {
            if (curHealth == 0) return;
            Events.AddMessage($"{damager.name} used {move.name}");

            if (move.power == 0)
            {
                Events.AddMessage($"{move.name} had no effect");
                return;
            }

            var hit = Random.Next(0, 100);
            if (hit > move.accuracy)
            {
                Events.AddMessage($"{move.name} missed");
                return;
            }

            var critical = Random.Next(1, 2);
            var atk = damager.physAtk;
            var def = physDef;
            if (move.damageType == "special")
            {
                atk = damager.specAtk;
                def = specDef;
            }
            var power = move.power;           
            float stab = 1;
            foreach (var item in damager.types)
            {
                if (move.moveType == item)
                    stab = 1.5f;
            }
            float type1 = PokemonDatabase.GetTypeDamage(move.moveType, types[0]);
            float type2 = 1;
            if (types.Count() > 1)
                type2 = PokemonDatabase.GetTypeDamage(move.moveType, types[1]);
            var random = Random.Next(0.8, 1.2);
            var typeBonus = type1 * type2;
            if (typeBonus == 0)
            {
                Events.AddMessage($"{move.name} does not affect {name}");
                return;
            }
            //var damage = ((((2 * level * critical) * power * (atk / def)) / 50) + 2) * stab * type1 * type2 * random;
            var damage = ((((2 * level) / 5 + 2) * power * atk / def) / 50 + 2) * critical * random * stab * typeBonus;

            Events.AddMessage($"{move.name} did {(int)damage} damage to {name}");

            curHealth -= damage;
            //curHealth -= damage * 0.01f;

            if (curHealth < 0)
            {
                curHealth = 0;
                damager.curStreak++;
                damager.defeatedPokemon.Add(this);
                Events.StreakUpdated(damager);
                Events.AddMessage($"{name} Fainted");
                Events.AddMessage($"{damager.name} is on a {damager.curStreak} streak");

                if (curStreak >= Static.minStreak)
                {
                    Events.AddStreaker(this);
                }
            }
        }
    }
}
