using menu;
namespace MenuItem
{
    public class Menuitem
    {
        public string Label { get; }
        public Action Action { get; }
        public Menu NextMenu { get; set; }

        public Menuitem(string label, Action action, Menu nextMenu = null)
        {
            Label = label;
            Action = action;
            NextMenu = nextMenu;
        }
    }
}