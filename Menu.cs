using System;
using System.Collections.Generic;

namespace DynaMenu {
    public class Menu {
        public class MenuItem {
            public string key;
            public string displayName;
            public string? description;

            public MenuItem(string key, string displayName, string? description) {
                this.key = key;
                this.displayName = displayName;
                this.description = description;
            }

            public MenuItem(string key, string displayName) : this(key, displayName, null) { }
        }

        internal List<MenuItem> MenuItems { get; }
        internal string Prompt { get; }
        internal int PagerItemCount { get; }

        public Menu(string prompt, int pagerItemCount) {
            MenuItems = new List<MenuItem>();
            Prompt = prompt;
            PagerItemCount = pagerItemCount;
        }

        public void AddItem(MenuItem item) {
            MenuItems.Add(item);
        }

        public MenuItem? Show() {
            return Display.ShowMenu(this);
        }
    }
}
