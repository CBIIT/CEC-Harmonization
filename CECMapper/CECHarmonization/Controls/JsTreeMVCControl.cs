using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

namespace CECHarmonization.Controls
{
    public static class JsTreeMVCControl
    {
        private static System.Text.StringBuilder _htmlStringBuilder = new System.Text.StringBuilder();
        private static MvcHtmlString RenderListItem(CECHarmonization.Models.INodeItem treeItem)
        {
            foreach (var item in ((CECHarmonization.Models.ListItem)treeItem).Nodes)
            {
                var listItem = ((CECHarmonization.Models.ListItem)item);
                var dataJsTree = listItem.DataJsTree;

                _htmlStringBuilder.AppendFormat("<li class='{0}' id='{1}' data-jstree='{6} \"opened\":{2}, \"selected\":{3}, \"disabled\":{4}, \"icon\":\"{5}\" {7}'> {8}",
                    listItem.Class, listItem.Id, dataJsTree.Opened.ToString().ToLower(),
                    dataJsTree.Selected.ToString().ToLower(), dataJsTree.Disabled.ToString().ToLower(),
                    dataJsTree.Icon, "{", "}", listItem.Text);

                if (item.Nodes.Count > 0)
                {
                    _htmlStringBuilder.Append("<ul>");
                    RenderListItem(item);
                    _htmlStringBuilder.Append("</ul>");
                }

                _htmlStringBuilder.Append("</li>");
            }

            return new MvcHtmlString(_htmlStringBuilder.ToString());
        }

        public static MvcHtmlString RenderJsTreeNodes(this HtmlHelper html,
                                                      List<CECHarmonization.Models.TreeNode> treeNodes)
        {
            _htmlStringBuilder = new System.Text.StringBuilder();

            var stringBuilder = new System.Text.StringBuilder();

            foreach (var treeNodeItem in treeNodes)
            {
                foreach (var listItem in treeNodeItem.ListItems)
                {
                    stringBuilder.Append(RenderListItem(listItem));
                }
            }

            return new MvcHtmlString(stringBuilder.ToString());
        }
    }
}