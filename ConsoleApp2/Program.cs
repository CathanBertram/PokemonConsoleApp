using ConsoleApp2;
using System.Reflection;
using System.Text;

string clearBuffer = null; // Clear this if window size changes
int maxBarLength = 90;
int kills = 0;
int longestStreak = 0;
string longestStreakPoke = "";
Queue<string> messages = new Queue<string>();
int sleepTime = 0;
int loopsPerFrame = 1000;
double iter = 0;
int damagelessIterations = 0;
int maxDamagelessIterations = 25;
Queue<Pokemon> streakers = new Queue<Pokemon>();
bool closing = false;
StreamWriter writer = null;

Main();

void Main()
{
    AppDomain.CurrentDomain.ProcessExit += ProcessExitHandler;
    Time.Start();
    PokemonDatabase.Start();
    Events.addMessage += AddMessage;
    Events.streakUpdated += StreakUpdated;
    Events.addStreaker += AddStreaker;
    var p1 = PokemonDatabase.GetRandomPokemon(true);
    var p2 = PokemonDatabase.GetRandomPokemon(true);
    bool newSpawn = true;

    Thread t = new Thread(CSVWriter);
    t.Start();
    while (true)
    {
        Events.Update();

        if (p1.curHealth <= 0)
        {
            p1 = PokemonDatabase.GetRandomPokemon(true);
            p2.Heal();
            newSpawn = true;
            kills++;
        }
        if (p2.curHealth <= 0)
        {
            p2 = PokemonDatabase.GetRandomPokemon(true);
            p1.Heal();
            newSpawn = true;
            kills++;
        }
        ClearConsole();
        ;
        var p1Start = p1.curHealth;
        var p2Start = p2.curHealth;
        if (!newSpawn)
        {
            if (p2.speed > p1.speed)
            {
                p1.TakeDamage(p2, p2.PickMove(p1));
                if (p1.curHealth > 0)
                    p2.TakeDamage(p1, p1.PickMove(p2));
            }
            else
            {
                p2.TakeDamage(p1, p1.PickMove(p2));
                if (p2.curHealth > 0)
                    p1.TakeDamage(p2, p2.PickMove(p1));
            }
        }

        if (p1Start == p1.curHealth && p2Start == p2.curHealth)
            damagelessIterations++;
        else
            damagelessIterations = 0;

        if (damagelessIterations >= maxDamagelessIterations)
        {
            p1 = PokemonDatabase.GetRandomPokemon(true);
            p2 = PokemonDatabase.GetRandomPokemon(true);
            newSpawn = true;
        }

        Display(p1, p2);
        newSpawn = false;
        iter++;
        
        Thread.Sleep(sleepTime);
    }
    
}

void Display(Pokemon p1, Pokemon p2)
{
    Console.WriteLine($"Total Kills - {kills} Longest Streak - {longestStreak} Pokemon - {longestStreakPoke} Iteration - {iter} DeltaTime - {Time.deltaTime}");

    Console.Write($"{p1.name} lvl {p1.level} {(int)p1.curHealth}/{(int)p1.maxHealth}");
    var h1 = (p1.curHealth / p1.maxHealth) * maxBarLength;

    if (p1.curHealth < p1.maxHealth / 4)
        Console.ForegroundColor = ConsoleColor.Red;
    else if (p1.curHealth < p1.maxHealth / 2)
        Console.ForegroundColor = ConsoleColor.Yellow;
    else
        Console.ForegroundColor = ConsoleColor.Green;

    for (int i = 0; i < h1; i++)
            Console.Write(Static.block);
    for (int i = 0; i <= maxBarLength - h1; i++)
        Console.Write(" ");
    Console.WriteLine();

    if (p2.curHealth < p2.maxHealth / 4)
        Console.ForegroundColor = ConsoleColor.Red;
    else if (p2.curHealth < p2.maxHealth / 4)
        Console.ForegroundColor = ConsoleColor.Yellow;
    else
        Console.ForegroundColor = ConsoleColor.Green;
    var h2 = (p2.curHealth / p2.maxHealth) * maxBarLength;
    for (int i = 0; i < maxBarLength - h2; i++)
        Console.Write(" ");

    for (int i = 0; i < h2; i++)
        Console.Write(Static.block);

    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($" {p2.name} lvl {p2.level} {(int)p2.curHealth}/{(int)p2.maxHealth}");
    Console.WriteLine();

    //Console.WriteLine($"Total, {p1.name}, {p2.name}, EV, {p1.name}, {p2.name}, IV, {p1.name}, {p2.name}");
    //Console.WriteLine();
    //Console.WriteLine($"HP, {p1.health}, {p2.health}, HP, {p1.healthEV}, {p2.healthEV}, HP, {p1.healthIV}, {p2.healthIV}");
    //Console.WriteLine($"Atk, {p1.physAtk}, {p2.physAtk}, Atk, {p1.physAtkEV}, {p2.physAtkEV}, Atk, {p1.physAtkIV}, {p2.physAtkIV}");
    //Console.WriteLine($"Def, {p1.physDef}, {p2.physDef}, Def, {p1.physDefEV}, {p2.physDefEV}, Def, {p1.physDefIV}, {p2.physDefIV}");
    //Console.WriteLine($"SpAtk, {p1.specAtk}, {p2.specAtk}, SpAtk, {p1.specAtkEV}, {p2.specAtkEV}, SpAtk, {p1.specAtkIV}, {p2.specAtkIV}");
    //Console.WriteLine($"SpDef, {p1.specDef}, {p2.specDef}, SpDef, {p1.specDefEV}, {p2.specDefEV}, SpDef, {p1.specDefIV}, {p2.specDefIV}");
    //Console.WriteLine($"Speed, {p1.speed}, {p2.speed}, Speed, {p1.speedEV}, {p2.speedEV}, Speed, {p1.speedIV}, {p2.speedIV}");
    //Console.WriteLine();
    //Console.WriteLine($"Total, {p1.health + p1.physAtk + p1.physDef + p1.specAtk + p1.specDef + p1.speed}, " +
    //    $"{p2.health + p2.physAtk + p2.physDef + p2.specAtk + p2.specDef + p2.speed}, Total, {p1.healthEV + p1.physAtkEV + p1.physDefEV + p1.specAtkEV + p1.specDefEV + p1.speedEV}, " +
    //    $"{p2.healthEV + p2.physAtkEV + p2.physDefEV + p2.specAtkEV + p2.specDefEV + p2.speedEV}, Total, {p1.healthIV + p1.physAtkIV + p1.physDefIV + p1.specAtkIV + p1.specDefIV + p1.speedIV}, " +
    //    $"{p2.healthIV + p2.physAtkIV + p2.physDefIV + p2.specAtkIV + p2.specDefIV + p2.speedIV}");

    Static.PrintRow("Total", p1.name, p2.name, "EV", p1.name, p2.name, "IV", p1.name, p2.name);
    Static.PrintLine();
    Static.PrintRow("HP", p1.health.ToString(), p2.health.ToString(), "HP", p1.healthEV.ToString(), p2.healthEV.ToString(), "HP", p1.healthIV.ToString(), p2.healthIV.ToString());
    Static.PrintRow("Atk", p1.physAtk.ToString(), p2.physAtk.ToString(), "Atk", p1.physAtkEV.ToString(), p2.physAtkEV.ToString(), "Atk", p1.physAtkIV.ToString(), p2.physAtkIV.ToString());
    Static.PrintRow("Def", p1.physDef.ToString(), p2.physDef.ToString(), "Def", p1.physDefEV.ToString(), p2.physDefEV.ToString(), "Def", p1.physDefIV.ToString(), p2.physDefIV.ToString());
    Static.PrintRow("SpAtk", p1.specAtk.ToString(), p2.specAtk.ToString(), "SpAtk", p1.specAtkEV.ToString(), p2.specAtkEV.ToString(), "SpAtk", p1.specAtkIV.ToString(), p2.specAtkIV.ToString());
    Static.PrintRow("SpDef", p1.specDef.ToString(), p2.specDef.ToString(), "SpDef", p1.specDefEV.ToString(), p2.specDefEV.ToString(), "SpDef", p1.specDefIV.ToString(), p2.specDefIV.ToString());
    Static.PrintRow("Spd", p1.speed.ToString(), p2.speed.ToString(), "Spd", p1.speedEV.ToString(), p2.speedEV.ToString(), "Spd", p1.speedIV.ToString(), p2.speedIV.ToString());
    Static.PrintLine();
    var t1 = p1.health + p1.physAtk + p1.physDef + p1.specAtk + p1.specDef + p1.speed;
    var t2 = p2.health + p2.physAtk + p2.physDef + p2.specAtk + p2.specDef + p2.speed;
    var t3 = p1.healthEV + p1.physAtkEV + p1.physDefEV + p1.specAtkEV + p1.specDefEV + p1.speedEV;
    var t4 = p2.healthEV + p2.physAtkEV + p2.physDefEV + p2.specAtkEV + p2.specDefEV + p2.speedEV;
    var t5 = p1.healthIV + p1.physAtkIV + p1.physDefIV + p1.specAtkIV + p1.specDefIV + p1.speedIV;
    var t6 = p2.healthIV + p2.physAtkIV + p2.physDefIV + p2.specAtkIV + p2.specDefIV + p2.speedIV;
    Static.PrintRow("Total", t1.ToString(), t2.ToString(), "Total", t3.ToString(), t4.ToString(), "Total", t5.ToString(), t6.ToString());
    while (messages.Count > 0)
    {
        //Thread.Sleep((int)(sleepTime * 0.1f));
        Console.WriteLine(messages.Dequeue());
    }
}
void AddStreaker(Pokemon poke)
{
    streakers.Enqueue(poke);
}

void StreakUpdated(Pokemon poke)
{
    if (poke.curStreak > longestStreak)
    {
        longestStreak = poke.curStreak;
        longestStreakPoke = poke.name;
    }
}
void AddMessage(string msg)
{
    if (messages == null)
        messages = new Queue<string>();
    messages.Enqueue(msg);
}
void ClearConsole()
{
    if (clearBuffer == null)
    {
        var line = "".PadLeft(Console.WindowWidth, ' ');
        var lines = new StringBuilder();

        for (var i = 0; i < Console.WindowHeight; i++)
        {
            lines.AppendLine(line);
        }

        clearBuffer = lines.ToString();
    }

    Console.SetCursorPosition(0, 0);
    Console.Write(clearBuffer);
    Console.SetCursorPosition(0, 0);
}
void CSVWriter()
{
    int index = 0;
    while (!closing)
    {
        if (!File.Exists($"Streakers{index}.csv"))
        {
            break;
        }
        index++;
    }
    FileStream fs = new FileStream($"Streakers{index}.csv", FileMode.Append, FileAccess.Write, FileShare.Read);
    writer = new StreamWriter(fs);
    writer.AutoFlush = true;
    writer.WriteLine("Name,Lvl,Move,Move,Move,MoveStreak Length,HP,Atk,Def,SpAtk,SpDef,Spd,HPEV,AtkEV,DefEV,SpAtkEV,SpDefEV,SpdEV,HPIV,AtkIV,DefIV,SpAtkIV,SpDefIV,SpdIV,");
    while (!closing)
    {
        while (streakers.Count() > 0)
        {
            var p = streakers.Dequeue();
            var writeString = $"{p.name},{p.level},{p.curStreak},";
            int x = 4;
            for (int i = 0; i < p.moves.Count(); i++)
            {
                writeString += $"{p.moves[i].name},";
            }
            x -= p.moves.Count();
            for (int i = 0; i < x; i++)
            {
                writeString += ",";
            }

            writeString += $"{p.health},{p.physAtk},{p.physDef},{p.specAtk},{p.specDef},{p.speed}," +
                $"{p.healthEV},{p.physAtkEV},{p.physDefEV},{p.specAtkEV},{p.specDefEV},{p.speedEV}," +
                $"{p.healthIV},{p.physAtkIV},{p.physDefIV},{p.specAtkIV},{p.specDefIV},{p.speedIV},";

            foreach (var item in p.defeatedPokemon)
            {
                writeString += item.name + ",";
            }
            writer.WriteLine(writeString);
        }
    }
    writer.Flush();
}

void ProcessExitHandler(object sender, EventArgs e)
{
    writer.Flush();

    closing = true;
}