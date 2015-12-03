using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TowerDefense
{
    class GameObject {
        public int posX;
        public int posY;
    }
    class Monster:GameObject {
        public double health;
        public int nextNodeId;
        public bool finished = false;
        public int value;
        public Monster(int health, List<PathNode> list)
        {
            this.health = health;
            nextNodeId = 0;
            value = health;
            posX = list[0].posX;
            posY = list[0].posY;
            Program.DrawTo(2+posX*2,1+posY,'M');
        }

        public void Move(List<PathNode> list, ref int strikes) {
            if (nextNodeId < list.Count)
            {

                Program.DrawTo(2 + posX * 2, 1 + posY, ' ');
                posX = list[nextNodeId].posX;
                posY = list[nextNodeId].posY;
                Program.DrawTo(2 + posX * 2, 1 + posY, 'M');
            }
            else {
                if (!finished) {
                    strikes++;
                    Program.DrawStrikes();
                    finished = true;
                    Program.DrawTo(2 + posX * 2, 1 + posY, ' ');
                }
            }
            nextNodeId++;
        }
    }
    class Tower:GameObject {
        public double range;
        public double damage;
        public int age = 0;
        public int damageLevel = 1;
        public int rangeLevel = 1;
        public Tower(double range, double damage, int posX, int posY) {
            this.damage = damage;
            this.range = range;
            this.posX = posX;
            this.posY = posY;
        }
        public bool dealDamege(ref List<Monster> monster, ref int gold) {
            for (int i = 0; i < monster.Count; i++)
            {
                if (Math.Pow((monster[i].posX - posX), 2) + Math.Pow((monster[i].posY - posY), 2)<=Math.Pow(range,2)) {
                    monster[i].health = monster[i].health - damage;
                    if (monster[i].health <= 0) {
                        if (monster[i].value > 10)
                        {
                            gold += 25;
                        }
                        else {
                            gold += 10;
                        }
                        Program.DrawMoney();
                        Program.DrawTo(2 + monster[i].posX * 2, 1 + monster[i].posY, ' ');
                        monster.RemoveAt(i);
                    }
                    return true;
                }
            }
            return false;
        }
    }
    class PathNode:GameObject {
        public PathNode(int posX, int posY) {
            this.posX = posX;
            this.posY = posY;
        }
    }
    class Program
    {

        static Random r = new Random();

        static List<Tower> towerList = new List<Tower>();
        static List<Monster> MonsterList = new List<Monster>();
        static List<PathNode> pathList = new List<PathNode>();

        static int strikes = 0;
        static int clevel = 0;
        static int remainMonsters = 0;
        static int gold = 100;

        static int sizeX = 15;
        static int sizeY = 15;
        static int cursorX = 0;
        static int cursorY = 0;

        static int monsterHealth = 0;
        static bool menu = false;

        static int select = 0;
        static int turn = 6;

        static int ind = 0;
        public static int menuRow = 6;
        static ConsoleKey key = ConsoleKey.A;

        static void Main(string[] args)
        {

            GeneratePath();

            Console.SetWindowSize(Console.LargestWindowWidth,Console.LargestWindowHeight);
            
            Console.Title ="Tower Defense";
            Console.CursorVisible = false;

            for (int i = 0; i <= sizeX; i++)
            {
                Console.Write("  ");
            } 
            Console.Write('\n');

            for (int i = 0; i < sizeX; i++)
            {
                Console.Write("  ");
                for (int j = 0; j < sizeY; j++)
                {
                    Console.Write("■ ");
                }
                Console.Write('\n');
            }
            for (int i = 0; i < pathList.Count; i++)
            {
                DrawTo(2+pathList[i].posX * 2, pathList[i].posY + 1, ' ');
            }
            DrawTo(cursorX * 2 + 1, cursorY + 1, "[");
            DrawTo(cursorX * 2 + 3, cursorY + 1, "]");
            DrawTo(sizeX * 2 + 3, 3, "STRIKES [   |   |   ]");
            DrawMoney();
            while (key!=ConsoleKey.Q) {
                remainMonsters = 5 + (clevel * 2);
                monsterHealth = 5 + (clevel * 5);
                DrawMonCount();
                for (int i = 0; i < towerList.Count; i++)
                {
                    towerList[i].age++;
                }
                ind = 0;
                while (ind < towerList.Count)
                {
                    if (towerList[ind].age > 4)
                    {
                        RemoveTower(towerList[ind].posX, towerList[ind].posY);
                    }
                    else
                    {
                        ind++;
                    }
                }
                for (int i = 0; i < pathList.Count; i++)
                {
                    DrawTo(2 + pathList[i].posX * 2, pathList[i].posY + 1, ' ');
                }
                strikes = 0;
                DrawTo(sizeX * 2 + 3, 3, "STRIKES [   |   |   ]");
                DrawTo(sizeX * 2 + 21, 1, "WAVE "+(clevel+1));
                DrawTowerDetails();
                MonsterList.Clear();
                Input();
                Game();
            }
            
        }
        static void clearMenu() {
            DrawTo(3 + sizeX * 2, menuRow, new string(' ', Console.LargestWindowWidth - (3 + sizeX * 2)));
            DrawTo(3 + sizeX * 2, menuRow+1, new string(' ', Console.LargestWindowWidth - (3 + sizeX * 2)));
            DrawTo(3 + sizeX * 2, menuRow+2, new string(' ', Console.LargestWindowWidth - (3 + sizeX * 2)));
            DrawTo(3 + sizeX * 2, menuRow+3, new string(' ', Console.LargestWindowWidth - (3 + sizeX * 2)));
        }

        static void Input() {
            
            bool prepared = false;

            while (!prepared)
            {
                //Draw();
                Console.SetCursorPosition(0, Console.CursorTop);
                ConsoleKey c = Console.ReadKey().Key;
                if (c == ConsoleKey.UpArrow)
                {
                    if (!menu)
                    {
                        if (cursorY > 0)
                        {
                            DrawTo(cursorX*2+1,cursorY+1," ");
                            DrawTo(cursorX*2 + 3, cursorY + 1, " ");
                            cursorY--;
                            DrawTo(cursorX*2 + 1, cursorY + 1, "[");
                            DrawTo(cursorX*2 + 3, cursorY + 1, "]");
                        }
                    }
                    else
                    {
                        if (select > 0)
                        {
                            DrawTo(3 + sizeX * 2, select + menuRow, ' ');
                            select--;
                            DrawTo(3 + sizeX * 2, select + menuRow, '>');
                        }
                    }
                }
                else if (c == ConsoleKey.DownArrow)
                {
                    if (!menu)
                    {
                        if (cursorY < sizeY - 1)
                        {
                            DrawTo(cursorX*2 + 1, cursorY + 1, " ");
                            DrawTo(cursorX*2 + 3, cursorY + 1, " ");
                            cursorY++;
                            DrawTo(cursorX*2 + 1, cursorY + 1, "[");
                            DrawTo(cursorX*2 + 3, cursorY + 1, "]");
                        }
                    }
                    else
                    {
                        if (select < 3)
                        {
                            DrawTo(3 + sizeX * 2, select + menuRow, ' ');
                            select++;
                            DrawTo(3 + sizeX * 2, select + menuRow, '>');
                        }
                    }

                }
                else if (c == ConsoleKey.RightArrow)
                {
                    if (!menu)
                    {
                        if (cursorX < sizeX - 1)
                        {
                            DrawTo(cursorX*2 + 1, cursorY + 1, " ");
                            DrawTo(cursorX*2 + 3, cursorY + 1, " ");
                            cursorX++;
                            DrawTo(cursorX*2 + 1, cursorY + 1, "[");
                            DrawTo(cursorX*2 + 3, cursorY + 1, "]");
                        }
                    }

                }
                else if (c == ConsoleKey.LeftArrow)
                {
                    if (!menu)
                    {
                        if (cursorX > 0)
                        {
                            DrawTo(cursorX*2 + 1, cursorY + 1, " ");
                            DrawTo(cursorX*2 + 3, cursorY + 1, " ");
                            cursorX--;
                            DrawTo(cursorX*2 + 1, cursorY + 1, "[");
                            DrawTo(cursorX*2 + 3, cursorY + 1, "]");
                        }
                    }

                }
                else if (c == ConsoleKey.Spacebar)
                {
                    prepared = true;
                    menu = false;
                    clearMenu();
                    
                }
                else if (c == ConsoleKey.Enter)
                {
                    if (menu)
                    {
                        if (!hasTower(cursorX, cursorY))
                        {
                            AddTower(cursorX, cursorY);

                        }
                        else
                        {
                            switch (select)
                            {
                                case 0:
                                    RepairTower(cursorX, cursorY);
                                    break;
                                case 1:
                                    UpgradeDamage(cursorX, cursorY);
                                    break;
                                case 2:
                                    UpgradeRange(cursorX, cursorY);
                                    break;
                                case 3:
                                    RemoveTower(cursorX, cursorY);
                                    break;
                                default:
                                    break;
                            }
                        }
                        menu = false;
                        clearMenu();
                    }
                    else
                    {
                        menu = true;
                        if (!hasTower(cursorX, cursorY))
                        {
                            DrawTo(3 + sizeX * 2, menuRow, ">Add Tower");
                        }
                        else {
                            Tower temp = SelectTower(cursorX, cursorY);
                            DrawTo(4 + sizeX * 2, menuRow, "Repair -" + ((temp.rangeLevel + temp.damageLevel) * 25) + "G");
                            DrawTo(4 + sizeX * 2, menuRow+1, "Upgrade Damage -" + (temp.damageLevel * 25) + "G");
                            DrawTo(4 + sizeX * 2, menuRow+2, "Upgrade Range -" + (temp.rangeLevel * 25) + "G");
                            DrawTo(4 + sizeX * 2, menuRow+3, "Remove");
                            DrawTo(3 + sizeX * 2, select +menuRow, ">");
                        }
                    }
                }
                else if (c == ConsoleKey.Backspace)
                {
                    menu = false;
                    clearMenu();
                }
                DrawTowerDetails();
                DrawTo(0, 1 + sizeY, new string(' ',sizeX*2+2));
                DrawTo(0, 2 + sizeY, new string(' ', sizeX * 2 + 2));
                DrawTo(0, 3 + sizeY, new string(' ', sizeX * 2 + 2));
            }
        }
        static void Game() {
            while (!WaveEnd())
            {
                ind = 0;
                while (ind < MonsterList.Count)
                {
                    if (MonsterList[ind].finished)
                    {
                        MonsterList.RemoveAt(ind);
                    }
                    else
                    {
                        ind++;
                    }
                }
                if (turn % 2 == 0 && remainMonsters != 0)
                {
                    MonsterList.Add(new Monster(r.Next(1 + (clevel * 5), 6 + (clevel * 5)), pathList));
                    remainMonsters--;
                }
                for (int i = 0; i < towerList.Count; i++)
                {
                    towerList[i].dealDamege(ref MonsterList, ref gold);
                }
                for (int i = 0; i < MonsterList.Count; i++)
                {
                    MonsterList[i].Move(pathList, ref strikes);
                }

                //Draw();
                System.Threading.Thread.Sleep(300);
                turn++;
            }
            if (strikes == 3)
            {
                Console.WriteLine("Level FAILED");
            }
            else
            {
                Console.WriteLine("Level Successfully Cleared");
            }
            key = Console.ReadKey().Key;
            Console.SetCursorPosition(0,Console.CursorTop-1);
            Console.Write(new string(' ',Console.LargestWindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);

        }

        static bool WaveEnd()
        {
            if (strikes == 3 || (MonsterList.Count == 0 && remainMonsters == 0))
            {
                if (MonsterList.Count == 0 && remainMonsters == 0)
                {
                    clevel++;
                }
                return true;
            }
            return false;
        }
        static void GeneratePath()
        {
            pathList.Clear();
            pathList.Add(new PathNode(r.Next(2, sizeX - 2), 0));
            while (pathList[pathList.Count - 1].posY != sizeY - 1)
            {
                int x = pathList[pathList.Count - 1].posX;
                int y = pathList[pathList.Count - 1].posY;
                if (r.Next(5) == 1)
                {
                    y = r.Next(pathList[pathList.Count - 1].posY, pathList[pathList.Count - 1].posY + 2);
                }
                else
                {
                    x = r.Next(pathList[pathList.Count - 1].posX - 1, pathList[pathList.Count - 1].posX + 2);
                }

                if (!hasNode(x, y))
                {
                    if (y > -1 && y < sizeY && x > 0 && x < sizeX - 1)
                    {
                        if (y == pathList[pathList.Count - 1].posY)
                        {
                            if (!hasNode(x, y - 1))
                            {
                                pathList.Add(new PathNode(x, y));
                            }
                        }
                        else
                        {
                            pathList.Add(new PathNode(x, y));
                        }
                    }
                } 
            }

        }

        /*static void Draw()
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    map[i, j] = '■';
                }
            }
            for (int i = 0; i < pathList.Count; i++)
            {
                map[pathList[i].posX, pathList[i].posY] = ' ';
            }
            for (int i = 0; i < MonsterList.Count; i++)
            {
                map[MonsterList[i].posX, MonsterList[i].posY] = 'M';
            }
            for (int i = 0; i < towerList.Count; i++)
            {
                map[towerList[i].posX, towerList[i].posY] = 'T';
            }
            Console.Clear();
            TimeSpan a = DateTime.Now.TimeOfDay;
            Console.Write("  ");
            for (int i = 0; i < sizeY; i++)
            {
                Console.Write("  ");
            }
            Console.Write("\tSTRIKES: ");
            for (int k = 0; k < strikes; k++)
            {
                Console.Write('X');
            }
            Console.Write("\tWave " + (clevel + 1));
            Console.Write("\tMoney: " + gold + "G");
            Console.Write('\n');
            for (int i = 0; i < sizeX; i++)
            {
                Console.Write(" ");
                if (cursorX == 0 && cursorY == i)
                {
                    Console.Write("[");
                }
                else
                {
                    Console.Write(" ");
                }
                for (int j = 0; j < sizeY; j++)
                {
                    Console.Write(map[j, i]);
                    if (cursorX == j && cursorY == i)
                    {
                        Console.Write("]");
                    }
                    else if (cursorX - 1 == j && cursorY == i)
                    {
                        Console.Write("[");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                    if (sizeY - 1 == j)
                    {

                        if (i == 1)
                        {
                            Console.Write("\tRemain Monsters: ");
                            Console.Write(remainMonsters + MonsterList.Count);
                            Console.Write("\tMax strength: ");
                            Console.Write(monsterHealth);
                        }
                        else if (i == 2)
                        {
                            Console.Write("\t");
                            for (int k = 0; k < MonsterList.Count; k++)
                            {
                                Console.Write("M" + MonsterList[k].health + " ");
                            }
                        }
                        else if (i == 4)
                        {
                            if (hasTower(cursorX, cursorY))
                            {
                                Tower temp = SelectTower(cursorX, cursorY);
                                Console.Write("\tTower " + ((5.0f - temp.age) / 5.0f) * 100 + "%\tDamage: ");
                                Console.Write(temp.damage);
                                Console.Write("\t Range: ");
                                Console.Write(temp.range);
                            }
                        }
                        if (menu)
                        {
                            if (hasTower(cursorX, cursorY))
                            {
                                Tower temp = SelectTower(cursorX, cursorY);
                                switch (i)
                                {
                                    case 5:
                                        Console.Write("\t");
                                        if (select == 0)
                                        {
                                            Console.Write("[");
                                        }
                                        else
                                        {
                                            Console.Write(" ");
                                        }
                                        Console.Write("Repair Tower");
                                        if (select == 0)
                                        {
                                            Console.Write("]");
                                        }
                                        else
                                        {
                                            Console.Write(" ");
                                        }
                                        Console.Write(" -" + ((temp.rangeLevel + temp.damageLevel) * 25) + "G");
                                        break;
                                    case 6:
                                        Console.Write('\t');
                                        if (select == 1)
                                        {
                                            Console.Write("[");
                                        }
                                        else
                                        {
                                            Console.Write(" ");
                                        }
                                        Console.Write("Upgrade Damage");
                                        if (select == 1)
                                        {
                                            Console.Write("]");
                                        }
                                        else
                                        {
                                            Console.Write(" ");
                                        }
                                        Console.Write(" -" + (temp.damageLevel * 25) + "G");
                                        break;
                                    case 7:

                                        Console.Write("\t");
                                        if (select == 2)
                                        {
                                            Console.Write("[");
                                        }
                                        else
                                        {
                                            Console.Write(" ");
                                        }
                                        Console.Write("Upgrade Range");
                                        if (select == 2)
                                        {
                                            Console.Write("]");
                                        }
                                        else
                                        {
                                            Console.Write(" ");
                                        }
                                        Console.Write(" -" + (temp.rangeLevel * 25) + "G");
                                        break;

                                    case 8:
                                        Console.Write("\t");
                                        if (select == 3)
                                        {
                                            Console.Write("[");
                                        }
                                        else
                                        {
                                            Console.Write(" ");
                                        }
                                        Console.Write("Remove");
                                        if (select == 3)
                                        {
                                            Console.Write("]");
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                if (i == 5)
                                {
                                    Console.Write("\t[Add Tower]");
                                }
                            }
                        }
                    }
                }
                Console.Write("\n");

            }
            TimeSpan b = DateTime.Now.TimeOfDay - a;
            Console.WriteLine(b.ToString());
            DrawTo(2 + sizeX * 2, 2, "12345678");
        }*/
        public static void DrawTo(int x, int y, string str)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(str);
            Console.SetCursorPosition(0, sizeY+1);
        }
        public static void DrawTo(int x, int y, char str)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(str);
            Console.SetCursorPosition(0, sizeY+1);
        }
        public static void DrawStrikes() {
            DrawTo(strikes*4 + 9 + sizeX * 2, 3, 'X');
        }
        public static void DrawMoney() {
            DrawTo(sizeX * 2 + 31, 3, new string(' ',Console.LargestWindowWidth-(sizeX*2+31)));
            DrawTo(sizeX * 2 + 31, 3, "Money: " + gold + "G");
        }
        public static void DrawTowerDetails() {
            DrawTo(sizeX * 2 + 3, 5, new string(' ', Console.LargestWindowWidth - (sizeX * 2 + 3)));
            if (hasTower(cursorX, cursorY))
            {
                Tower temp = SelectTower(cursorX, cursorY);
                DrawTo(sizeX * 2 + 3, 5, "Tower " + ((5.0f - temp.age) / 5.0f) * 100 + " % Damage: " +temp.damage+" Range: "+temp.range);
            }
        }
        public static void DrawMonCount() {
            DrawTo(sizeX * 2 + 3, 1, new string(' ', 18));
            DrawTo(sizeX * 2 + 3, 1, "M " + remainMonsters+"/"+ (5 + (clevel * 5)));
        }
        static bool hasNode(int x, int y) {
            for (int i = 0; i < pathList.Count; i++)
            {
                if (pathList[i].posX == x && pathList[i].posY == y)
                {
                    return true;
                }
            }
            return false;
        }
        static bool hasTower(int x, int y)
        {
            for (int i = 0; i < towerList.Count; i++)
            {
                if (towerList[i].posX == x && towerList[i].posY == y)
                {
                    return true;
                }
            }
            return false;
        }

        static bool AddTower(int x, int y)
        {
            if (gold >= 50)
            {
                for (int i = 0; i < pathList.Count; i++)
                {
                    if (pathList[i].posX == x && pathList[i].posY == y)
                    {
                        Console.WriteLine("You can\'t place tower here");
                        Console.ReadKey();
                        return false;
                    }
                }
                for (int i = 0; i < towerList.Count; i++)
                {
                    if (towerList[i].posX == x && towerList[i].posY == y)
                    {
                        Console.WriteLine("You have Tower on these place");
                        Console.ReadKey();
                        return false;
                    }
                }
                towerList.Add(new Tower(2, 1, x, y));
                gold -= 50;
                DrawMoney();
                DrawTo(2+x*2,y+1,'T');
            }
            else
            {
                Console.WriteLine("You don\'t have enough money");
                Console.ReadKey();
                return false;
            }
            return true;
        }
        static void RemoveTower(int x, int y)
        {
            for (int i = 0; i < towerList.Count; i++)
            {
                if (towerList[i].posX == x && towerList[i].posY == y)
                {
                    towerList.RemoveAt(i);
                    gold += 50;
                    DrawMoney();
                    DrawTo(2+x*2,y+1,'■');
                }
            }
        }
        static Tower SelectTower(int x, int y) {
            for (int i = 0; i < towerList.Count; i++)
            {
                if (towerList[i].posX == x && towerList[i].posY == y)
                {
                    return towerList[i];
                }
            }
            return null;
        }

        static void RepairTower(int x, int y)
        {

            for (int i = 0; i < towerList.Count; i++)
            {
                if (towerList[i].posX == x && towerList[i].posY == y)
                {
                    if (gold >= (towerList[i].damageLevel + towerList[i].rangeLevel) * 25)
                    {
                        gold -= (towerList[i].damageLevel + towerList[i].rangeLevel) * 25;
                        DrawMoney();
                        towerList[i].age = 0;
                    }
                    else
                    {
                        Console.WriteLine("You don\'t have enough Gold");
                        Console.ReadKey();
                    }
                }
            }
        }
        static void UpgradeDamage(int x, int y)
        {

            for (int i = 0; i < towerList.Count; i++)
            {
                if (towerList[i].posX == x && towerList[i].posY == y)
                {
                    if (gold >= towerList[i].damageLevel * 25)
                    {
                        gold -= towerList[i].damageLevel * 25;
                        DrawMoney();
                        towerList[i].damage +=(double)towerList[i].damageLevel/2;
                        towerList[i].damageLevel += 1;
                    }
                    else
                    {
                        Console.WriteLine("You don\'t have enough Gold");
                        Console.ReadKey();
                    }
                }
            }
        }
        static void UpgradeRange(int x, int y)
        {

            for (int i = 0; i < towerList.Count; i++)
            {
                if (towerList[i].posX == x && towerList[i].posY == y)
                {
                    if (gold >= towerList[i].rangeLevel * 25)
                    {
                        gold -= towerList[i].rangeLevel * 25;
                        towerList[i].range += (double)towerList[i].rangeLevel/10;
                        towerList[i].rangeLevel += 1;
                        DrawMoney();
                    }
                    else
                    {
                        Console.WriteLine("You don\'t have enough Gold");
                        Console.ReadKey();
                    }
                }
            }
        }
    }
}