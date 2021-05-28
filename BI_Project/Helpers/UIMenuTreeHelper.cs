
using BI_Project.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace BI_Project.Helpers
{
    public class UIMenuTreeHelper
    {
        private IEnumerable<EntityMenuModel> _dataMenu;
        private Dictionary<int, List<EntityMenuModel>> MenuTree = new Dictionary<int, List<EntityMenuModel>>();
        public int? RootId;
        private List<int> childrenList = new List<int>();

        private string htmlRaw = "";

        public UIMenuTreeHelper(IEnumerable<EntityMenuModel> dataMenu)
        {
            _dataMenu = dataMenu;

            foreach (var menuItem in dataMenu)
            {
                AddMenuItem(menuItem);
            }
        }

        private void AddMenuItem(EntityMenuModel item)
        {
            if (!MenuTree.ContainsKey(item.ParentId)) MenuTree.Add(item.ParentId, new List<EntityMenuModel>());

            if (!MenuTree.ContainsKey(item.MenuId)) MenuTree.Add(item.MenuId, new List<EntityMenuModel>());

            childrenList.Add(item.MenuId);

            RootId = childrenList.Contains(item.ParentId) ? RootId : item.ParentId;

            var list = MenuTree[item.ParentId];

            list.Add(item);
        }

        //TreeList Menu
        public string BuildInItem(int? key = null)
        {

            if (key == null) return "";

            var menuItem = _dataMenu.Where(x => x.MenuId == key).FirstOrDefault();

            var menuName = menuItem == null ? "" : menuItem.Name;

            var menuItemJsonData = new JavaScriptSerializer().Serialize(menuItem);

            var action = menuItem == null ? "" : $" [<a class=\"menu_detail_btn\" data-item-data='{menuItemJsonData}'>chi tiết</a> | <a href=\"/Menus/Delete?menuid={key}\" onclick=\"return confirm(\'Bạn có chắc chắn muốn xóa không?');\"><span style=\"color:red;\">xóa</span></a>]";
            htmlRaw += $"<li data-jstree='{{\"opened\": true}}'>{menuName} " + action;

            var child = MenuTree[key.Value].OrderBy(x => x.Priority).ThenBy(x => x.LevelTree).ToList();

            if (child.Count != 0)
            {
                htmlRaw += "<ul>";

                foreach (var i in child)
                {
                    BuildInItem(i.MenuId);
                }

                htmlRaw += "</ul>";
            }
            htmlRaw += "</li>";
            return htmlRaw;
        }

        //TreeList Navbar
        public string BuidNavMenuInItem(int? key = null)
        {
            if (key == null) return "";

            var item = _dataMenu.Where(x => x.MenuId == key).FirstOrDefault();

            var child = MenuTree[key.Value].OrderBy(x => x.Priority).ThenBy(x => x.LevelTree).ToList(); ;
            htmlRaw += item != null && item.ParentId == RootId && child.Count > 0 ? $"<li class=\"dropdown mega-menu mega-menu-wide \"><a class=\"dropdown-toggle\" data-toggle=\"dropdown\">{item.Name}<span class=\"caret\"></span></a><div class=\"dropdown-menu dropdown-content \"><div class=\"dropdown-content-body\"><div class=\"row\">" :
                       item != null && item.ParentId != RootId && child.Count > 0 ? $" <div class=\"col-md-3\" ><span class=\"menu-heading underlined\">{item.Name}</span><ul class=\"menu-list\">" :
                       child.Count == 0 && item.Level == 2 ? $"<div class=\"col-md-3\"><ul class=\"menu-list\"><li><a href=/{item.Path}/{item.MenuId}>{item.Name} </a></li></ul></div>" :
                       child.Count == 0 && item.Level != 2 ? $"<li><a  href=/{item.Path}/{item.MenuId}>{item.Name}</a></li>" : "";


            if (child.Count > 0)
            {
                foreach (var i in child)
                {
                    BuidNavMenuInItem(i.MenuId);

                }

            }

            htmlRaw += item != null && item.ParentId == RootId && child.Count > 0 ? $"</div></div></div></li>" :
                       item != null && item.ParentId != RootId && child.Count > 0 ? $"</ul></div>" : "";


            return htmlRaw;
        }

        // Build fancy tree json data

        private List<MenuFancyTreeItem> data = new List<MenuFancyTreeItem>();
        private void AddItemToJsonTree(List<MenuFancyTreeItem> data, int key, MenuFancyTreeItem item)
        {
            foreach (var elm in data)
            {
                if (elm.key == key)
                {
                    elm.children.Add(item);
                }
                AddItemToJsonTree(elm.children, key, item);
            }
        }
        public class MenuFancyTreeItem
        {
            public string title;
            public int key;
            public int parentId;
            public int priority;
            public string path;
            public string pathTableau;
            public bool selected;
            public bool hideCheckbox;
            public bool folder;
            public List<MenuFancyTreeItem> children;
            public static implicit operator MenuFancyTreeItem(EntityMenuModel model)
            {
                return new MenuFancyTreeItem()
                {
                    title = model.Name,
                    key = model.MenuId,
                    parentId = model.ParentId,
                    priority = model.Priority,
                    path = model.Path,
                    //pathTableau = model.PathTableau,
                    selected = model.Selected,
                    children = new List<MenuFancyTreeItem>()
                };
            }
        }

        public string BuildMenuToJsonStr(int? key = null)
        {
            if (key == null) return "";

            var item = _dataMenu.Where(x => x.MenuId == key).FirstOrDefault();
            var child = MenuTree[key.Value].OrderBy(x => x.Priority).ThenBy(x => x.LevelTree).ToList();

            if (item != null)
            {
                var parentOfItem = _dataMenu.Where(x => x.MenuId == item.ParentId).FirstOrDefault();

                if (parentOfItem == null) data.Add(item);
                else
                {
                    AddItemToJsonTree(data, parentOfItem.MenuId, item);
                }
            }

            if (child.Count > 0)
            {
                foreach (var kid in child)
                {
                    BuildMenuToJsonStr(kid.MenuId);
                }
            }

            return new JavaScriptSerializer().Serialize(data);
        }
    }
}