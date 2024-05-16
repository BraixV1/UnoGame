using System.Text;
using GameEngine;
using MenuItem;
namespace menu
{
    public class Menu
    {
        private int selectedIndex = 0;
        private List<Menuitem> menuItems = new List<Menuitem>();
        private bool drawTurn = false;
        public bool isRunning = true; // Add a flag to control menu activity
        private UnoGameState _gameState;
        private string _menuTitle = "";
        public Stack<Menu> menus = new Stack<Menu>();

        public Menu(params Menuitem[] items)
        {
            menuItems.AddRange(items);
        }
        
        public Menu(UnoGameState gameState,bool drawturn, params Menuitem[] items) // used for displaying the current game turn
        {
            menuItems.AddRange(items);
            drawTurn = drawturn;
            _gameState = gameState;
        }
        
        public Menu(string menuTitle, params Menuitem[] items) // used for displaying the current game turn
        {
            menuItems.AddRange(items);
            _menuTitle = menuTitle;
            if (menus.Count == 0)
            {
                menus = new Stack<Menu>();
            }
            menus.Push(this);
        }

        private void Render()
        {
            Console.Clear();
            if (drawTurn)
            {
                Console.OutputEncoding = Encoding.UTF8;
                for (var i = 0; i < _gameState.Players.Count; i++)
                {
                    if (i == _gameState.TurnIndex)
                    {
                        Console.WriteLine($"{_gameState.Players[i].Name}(ME) has {_gameState.Players[i].Hand.Count} cards {_gameState.Players[i].Shout}");
                        continue;
                    }
                    Console.WriteLine($"{_gameState.Players[i].Name} has {_gameState.Players[i].Hand.Count} cards {_gameState.Players[i].Shout}");
                }
                Console.WriteLine("---------------------------------------");
                if (_gameState.OnTable == null) Console.WriteLine("EMPTY");
                if (_gameState.OnTable != null) Console.WriteLine(_gameState.OnTable.ToString());
    
                Console.WriteLine("--------------------------------------");
                Console.WriteLine($"Your hand: {_gameState.Players[_gameState.TurnIndex].GetHandAsString()}");
            }

            if (!_menuTitle.Equals(""))
            {
                Console.WriteLine(_menuTitle);
            }
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("> " + menuItems[i].Label + " <");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(menuItems[i].Label);
                }
            }
        }

        public void HandleInput()
        {
            var keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                selectedIndex--;
                if (selectedIndex < 0)
                {
                    selectedIndex = menuItems.Count - 1;
                }
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                selectedIndex++;
                if (selectedIndex >= menuItems.Count)
                {
                    selectedIndex = 0;
                }
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                var selectedItem = menuItems[selectedIndex];
                selectedItem.Action.Invoke();

                if (selectedItem.NextMenu != null)
                {
                    menus.Push(selectedItem.NextMenu);
                    selectedItem.NextMenu.menus = menus;
                    selectedItem.NextMenu.Display();
                }
                else
                {
                    isRunning = false;
                }
            }
        }

        public void Display()
        {
            isRunning = true;
            while (isRunning)
            {
                Render();
                HandleInput();
            }
            
            while (menus.Count != 0)
            {
                menus.Pop().isRunning = false;
            }
        }
    }
}
