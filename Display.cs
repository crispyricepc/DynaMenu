using System;
using System.Collections.Generic;

namespace DynaMenu {
    internal static class Display {
        private static int consoleX;
        private static int consoleY;

        private static void ClearLine() {
            while (Console.CursorLeft < Console.WindowWidth - 1)
                Console.Write(" ");
            Console.WriteLine();
        }

        private static void PrintMenu(List<Menu.MenuItem> menuItems, int selectedIndex, int pageSize) {
            Console.CursorVisible = false;

            int itemCount = 0;
            int index = 0;
            if (selectedIndex >= pageSize)
                index = (selectedIndex - pageSize) + 1;
            for (; index < menuItems.Count && itemCount < pageSize; itemCount++, index++) {
                Menu.MenuItem item = menuItems[index];
                if (index == selectedIndex)
                    Console.Write("\x1b[1m");
                Console.Write(item.displayName);
                ClearLine();
                consoleY++;
                if (item.description != null) {
                    Console.Write($"    {item.description}");
                    ClearLine();
                    consoleY++;
                }
                Console.Write("\x1b[0m");
            }

            while (Console.CursorTop < Console.WindowHeight - 1) {
                ClearLine();
                consoleY++;
            }

            Console.SetCursorPosition(consoleX, Console.CursorTop - consoleY);
            Console.CursorVisible = true;
        }

        private static List<Menu.MenuItem> Filter(List<Menu.MenuItem> menuItems, string filterString) {
            List<Menu.MenuItem> returnItems = new();
            foreach (var item in menuItems)
                if (item.key.Contains(filterString))
                    returnItems.Add(item);
            return returnItems;
        }

        public static Menu.MenuItem? ShowMenu(Menu menu, string initialQuery) {
            Console.Write($"{menu.Prompt} (type to filter, arrow keys to move, enter to select) : ");

            // Main loop
            string query = initialQuery;
            List<Menu.MenuItem> filteredItems = Filter(menu.MenuItems, query);
            int selectedIndex = 0;
            ConsoleKeyInfo? keyInfo = null;
            do {
                if (keyInfo.HasValue) {
                    switch (keyInfo.Value.Key) {
                    case ConsoleKey.Backspace:
                        if (query.Length != 0) {
                            query = query.Remove(query.Length - 1, 1);
                            Console.Write("\b \b");
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (selectedIndex > 0)
                            selectedIndex--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (selectedIndex + 1 < filteredItems.Count)
                            selectedIndex++;
                        break;
                    default:
                        if (Char.IsLetterOrDigit(keyInfo.Value.KeyChar)
                            || Char.IsWhiteSpace(keyInfo.Value.KeyChar)) {
                            query += keyInfo.Value.KeyChar;
                            Console.Write(keyInfo.Value.KeyChar);
                        }
                        break;
                    }
                }

                consoleX = Console.CursorLeft;
                consoleY = 1;
                Console.WriteLine();
                filteredItems = Filter(menu.MenuItems, query);
                if (selectedIndex > filteredItems.Count)
                    selectedIndex = filteredItems.Count - 1;
                PrintMenu(filteredItems, selectedIndex, menu.PagerItemCount);

                keyInfo = Console.ReadKey(true);
            } while (keyInfo.Value.Key != ConsoleKey.Enter &&
                !(keyInfo.Value.Modifiers == ConsoleModifiers.Control && keyInfo.Value.Key == ConsoleKey.D));
            return null;
        }

        public static Menu.MenuItem? ShowMenu(Menu menu) {
            return ShowMenu(menu, "");
        }
    }
}