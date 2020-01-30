using System;
using System.Linq;
using System.Net.Http.Formatting;
using Skybrud.Umbraco.Analytics.Models.Config;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;

namespace Skybrud.Umbraco.Analytics.Controllers.Trees {

    //[Tree(Constants.Applications.Settings, "skybrud.analytics", "Skybrud.Analytics", "icon-folder", "icon-folder", sortOrder: 15)]
    //[Tree("sectionAlias", "treeAlias", "icon-folder", "icon-folder", true, 15, "Skybrud", "Skybrud.Analytics")]
    [Tree(Constants.Applications.Settings, "skybrud.analytics", IsSingleNodeTree = true, SortOrder = 15)]
    public class SkybrudAnalyticsTreeController : TreeController {

        protected override TreeNode CreateRootNode(FormDataCollection queryStrings) {

            TreeNode root = CreateTreeNode("-1", "-1", queryStrings, "Skybrud.Analytics", "icon-chart-curve", false);

            root.RoutePath = $"settings/{TreeAlias}/overview";

            return root;

        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings) {

            TreeNodeCollection nodes = new TreeNodeCollection();
            return nodes;

            if (id == "-1") {
                TreeNode node = CreateTreeNode("oauth", "-1", queryStrings, "OAuth", "icon-settings", AnalyticsConfig.Current.GetClients().Any());
                node.RoutePath = $"settings/{TreeAlias}/oauth";
                nodes.Add(node);
            }

            
            if (id == "oauth") {

                foreach (AnalyticsConfigClient client in AnalyticsConfig.Current.GetClients()) {
                    TreeNode node = CreateTreeNode(client.Id, "oauth", queryStrings, client.Name, "icon-target", false);
                    node.RoutePath = $"settings/{TreeAlias}/oauth/" + client.Id;
                    nodes.Add(node);
                }

            }

            return nodes;

        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings) {
            return new MenuItemCollection();
        }

    }

}