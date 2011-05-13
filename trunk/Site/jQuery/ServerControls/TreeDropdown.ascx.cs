using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using jQuery.ServerControls.TreeDropdown;
using Activizr.Logic.Interfaces;
using System.ComponentModel;


namespace jQuery
{
    namespace ServerControls
    {
        namespace TreeDropdown
        {
            [Serializable]
            public class TreeViewNode
            {
                public TreeViewNode (string text, string value)
                {
                    this.value = value;
                    this.text = text;
                }
                public TreeViewNode (string text, string value, string title)
                {
                    this.value = value;
                    this.text = text;
                    this.title = title;
                }
                public const string closedClass = "";
                public const string openClass = "open";
                public const string disabledClass = "disabled";
                public const string selectedClass = "selected";
                public const string partSelectedClass = "partSelected";

                public bool clickable = true;
                public bool selected = false;
                public string value = "";
                public string text = "";
                public string title = "";
                public string className = "";
                public bool expanded = false;
                public bool isleaf { get { return children.Count == 0; } }
                public bool hasSelectedChild = false;
                internal List<TreeViewNode> children = new List<TreeViewNode>();
                public void AddChild (TreeViewNode c)
                {
                    children.Add(c);
                }

                public delegate TreeViewNode doToNodeWithParent (TreeViewNode node, TreeViewNode parentNode);
                public delegate TreeViewNode doToNode (TreeViewNode node);

                public void each (doToNode oper)
                {
                    foreach (TreeViewNode st in this.children)
                    {
                        st.each(oper);
                    }
                }

                public void each (doToNodeWithParent oper)
                {
                    foreach (TreeViewNode st in this.children)
                    {
                        st.each(oper, this);
                    }
                }



                public void each (doToNodeWithParent oper, TreeViewNode parentNode)
                {
                    foreach (TreeViewNode st in this.children)
                    {
                        st.each(oper, this);
                    }
                    oper(this, parentNode);
                }

                public List<TreeViewNode> enabledNodes ()
                {
                    List<TreeViewNode> retVal = new List<TreeViewNode>();
                    each(delegate(TreeViewNode node)
                    {
                        if (node.clickable) retVal.Add(node);
                        return node;
                    });
                    return retVal;
                }

                public void expandTree (bool expand)
                {
                    each(delegate(TreeViewNode node, TreeViewNode parentNode)
                    {
                        node.expanded = expand;
                        return node;
                    });
                }

                public void refreshChildSelectionCount ()
                {
                    each(delegate(TreeViewNode node, TreeViewNode parentNode)
                    {
                        parentNode.hasSelectedChild = parentNode.hasSelectedChild || node.selected || node.hasSelectedChild;
                        return node;
                    });
                }

                public void expandLevels (bool expand, int levels)
                {
                    if (levels == 0) return;
                    --levels;
                    this.expanded = expand;

                    foreach (TreeViewNode st in children)
                    {
                        st.expandLevels(expand, levels);
                    }
                }

                public TreeViewNode findValue (string findvalue)
                {
                    if (value.Trim() == findvalue.Trim())
                    {
                        return this;
                    }
                    foreach (TreeViewNode st in children)
                    {
                        TreeViewNode subnode = st.findValue(findvalue);
                        if (subnode != null) return subnode;
                    }
                    return null;
                }

                public TreeViewNode findText (string findtext)
                {
                    if (text.Trim().ToLower() == findtext.Trim().ToLower())
                    {
                        return this;
                    }
                    foreach (TreeViewNode st in children)
                    {
                        TreeViewNode subnode = st.findText(findtext);
                        if (subnode != null) return subnode;
                    }
                    return null;
                }

                public TreeViewNode expandSubnode (string findValue, bool expand, int additional)
                {

                    if (value == findValue)
                    {
                        this.expandLevels(expand, additional);
                        return this;
                    }

                    TreeViewNode foundChild = null;
                    foreach (TreeViewNode st in children)
                    {

                        foundChild = st.expandSubnode(findValue, expand, additional);
                        if (foundChild != null) break;
                    }
                    if (foundChild != null)
                    {
                        this.expanded = expand;
                        return foundChild;
                    }
                    return null;
                }

                public void pruneToSubtree (int[] values)
                {
                    pruneToValues(values, true);
                }

                public void pruneToSubtree (string[] values)
                {
                    pruneToValues(values, true);
                }

                public void pruneToValues (int[] values)
                {
                    pruneToValues(values, false);
                }

                public void pruneToValues (string[] values)
                {
                    pruneToValues(values, false);
                }

                protected void pruneToValues (int[] values, bool keepSubtree)
                {
                    List<string> stringvalues = new List<string>();
                    foreach (int val in values) stringvalues.Add("" + val);
                    pruneToValues(stringvalues.ToArray(), keepSubtree);
                }

                protected void pruneToValues (string[] values, bool keepSubtree)
                {
                    Dictionary<string, bool> pruneToValuesDict = new Dictionary<string, bool>();
                    foreach (string s in values)
                        pruneToValuesDict[s] = true;
                    pruneToValuesInner(pruneToValuesDict, keepSubtree);
                }

                protected void pruneToValues (Dictionary<string, bool> pruneToValuesDict, bool keepSubtree)
                {
                    pruneToValuesInner(pruneToValuesDict, keepSubtree);
                }

                protected bool pruneToValuesInner (Dictionary<string, bool> pruneToValuesDict, bool keepSubtree)
                {
                    if (pruneToValuesDict.ContainsKey(this.value) && keepSubtree)
                    { // keep subtree of found
                    }
                    else
                    {
                        List<TreeViewNode> toremove = new List<TreeViewNode>();
                        foreach (TreeViewNode st in children)
                        {
                            if (st.pruneToValuesInner(pruneToValuesDict, keepSubtree))
                                toremove.Add(st);
                        }
                        foreach (TreeViewNode st in toremove)
                        {
                            children.Remove(st);
                        }
                    }
                    if (pruneToValuesDict.ContainsKey(this.value))
                    {
                        this.clickable = true;
                        return false;
                    }
                    else
                    {
                        if (this.children.Count == 0)
                        {
                            return true;
                        }
                        else
                        {
                            this.clickable = false;
                            return false;
                        }
                    }
                }


            }
        }
    }
}


public partial class jQuery_ServerControls_TreeDropdown : System.Web.UI.UserControl
{
    [Browsable(true)]
    public event EventHandler SelectedIndexChanged;

    protected virtual void OnSelectedIndexChanged (EventArgs e)
    {
        if (SelectedIndexChanged != null)
            SelectedIndexChanged(this, e);
    }

    public TreeViewNode Content
    {
        get
        {
            if (ViewState["content"] == null)
            {
                ViewState["content"] = new TreeViewNode("", "");
            }
            return (TreeViewNode)ViewState["content"];
        }
        set { ViewState["content"] = value; }
    }

    [DisplayName("Width (in px), integer")]
    public int Width
    {
        get
        {
            if (ViewState["width"] == null)
            {
                ViewState["width"] = (int)275;
            }
            return (int)ViewState["width"];
        }
        set { ViewState["width"] = value; }
    }

    public int Height
    {
        get
        {
            if (ViewState["height"] == null)
            {
                ViewState["height"] = (int)300;
            }
            return (int)ViewState["height"];
        }
        set { ViewState["height"] = value; }
    }

    public bool MultiSelect
    {
        get
        {
            if (ViewState["multiselect"] == null)
            {
                ViewState["multiselect"] = false;
            }
            return (bool)ViewState["multiselect"];
        }
        set { ViewState["multiselect"] = value; }
    }

    public bool DropDown
    {
        get
        {
            if (ViewState["dropdown"] == null)
            {
                ViewState["dropdown"] = true;
            }
            return (bool)ViewState["dropdown"];
        }
        set { ViewState["dropdown"] = value; }
    }

    public bool CheckBoxes
    {
        get
        {
            if (ViewState["checkboxes"] == null)
            {
                ViewState["checkboxes"] = false;
            }
            return (bool)ViewState["checkboxes"];
        }
        set { ViewState["checkboxes"] = value; }
    }

    public bool AutoPostBack
    {
        get
        {
            if (ViewState["autopostback"] == null)
            {
                ViewState["autopostback"] = false;
            }
            return (bool)ViewState["autopostback"];
        }
        set { ViewState["autopostback"] = value; }
    }

    public string PreviousSelectedValue
    {
        get
        {
            if (ViewState["PreviousSelectedValue"] == null)
            {
                ViewState["PreviousSelectedValue"] = "";
            }
            return (string)ViewState["PreviousSelectedValue"];
        }
        set { ViewState["PreviousSelectedValue"] = value; }
    }

    public string NodeTemplate
    {
        get
        {
            if (ViewState["nodeTemplate"] == null)
            {
                ViewState["nodeTemplate"] = "<div{selected}{valueProp}>{checkbox}{text}</div>";
            }
            return (string)ViewState["nodeTemplate"];
        }
        set { ViewState["nodeTemplate"] = value; }
    }

    public string SelectedValue
    {
        get
        {
            if (MultiSelect)
            {
                Dictionary<string, bool> selectedvaluesDict = SelectedValues;
                string[] selectedvalues = new string[selectedvaluesDict.Count];
                selectedvaluesDict.Keys.CopyTo(selectedvalues, 0);
                return string.Join(",", selectedvalues);
            }
            else
                return this.HiddenValue.Value;
        }
        set
        {
            if (this.Content != null && this.Content.children.Count > 0)
            {
                this.value.InnerText = this.HiddenText.Value = "";
                this.HiddenValue.Value = "-1";
                this.Content.each(delegate(TreeViewNode node) { node.selected = false; return node; });
                if (MultiSelect)
                {
                    string[] selectedvalues = value.Split(',');
                    Dictionary<string, bool> selectedValuesDict = new Dictionary<string, bool>();
                    foreach (string val in selectedvalues)
                        selectedValuesDict[val] = true;
                    this.Content.each(delegate(TreeViewNode node)
                    {
                        node.selected = selectedValuesDict.ContainsKey(node.value);
                        if (node.selected)
                            this.Content.expandSubnode(node.value, true, 0);
                        return node;
                    });
                }
                else
                {
                    this.Content.each(delegate(TreeViewNode node)
                    {
                        node.expanded = false;
                        node.selected = false;
                        return node;
                    });
                    TreeViewNode foundChild = this.Content.expandSubnode(value, true, 0);
                    if (foundChild != null)
                    {
                        this.HiddenValue.Value = foundChild.value;
                        foundChild.selected = true;
                        this.value.InnerText = this.HiddenText.Value = foundChild.text;
                    }
                }
            }
        }
    }


    public Dictionary<string, bool> SelectedValues
    {
        get
        {
            Dictionary<string, bool> retVal = new Dictionary<string, bool>();
            if (this.Content != null)
            {
                Content.each(delegate(TreeViewNode node) { if (node.selected) retVal.Add(node.value, true); return node; });
            }

            return retVal;
        }
        set
        {
            if (this.Content != null && this.Content.children.Count > 0)
            {
                this.value.InnerText = this.HiddenText.Value = "";
                this.HiddenValue.Value = "-1";
                this.Content.each(delegate(TreeViewNode node)
                {
                    node.selected = value.ContainsKey(node.value);
                    if (node.selected)
                        this.Content.expandSubnode(node.value, true, 0);
                    return node;
                });

            }
        }
    }

    public string SelectedText
    {
        get { return this.HiddenText.Value; }
        set
        {
            if (this.Content != null && this.Content.children.Count > 0)
            {
                this.value.InnerText = this.HiddenText.Value = "";
                this.HiddenValue.Value = "-1";
                this.Content.each(delegate(TreeViewNode node) { node.selected = false; return node; });
                TreeViewNode foundChild = this.Content.findText(value);
                if (foundChild != null)
                {
                    this.Content.expandSubnode(foundChild.value, true, 0);
                    foundChild.selected = true;
                    this.value.InnerText = this.HiddenText.Value = foundChild.text;
                    this.HiddenValue.Value = foundChild.value;
                }
            }
        }
    }

    string clientClick = "";

    public string ClientClick
    {
        get { return clientClick; }
        set { clientClick = value; }
    }

    string clientController = "";

    public string ClientController
    {
        get { return clientController; }
        set { clientController = value; }
    }


    public void LoadTree (ITreeNodeObject root)
    {
        TreeViewNode node0 = new TreeViewNode(root.Name, "" + root.Identity);

        buildTree(node0, root);
        this.SetContent(node0);
    }

    private void buildTree (TreeViewNode node0, ITreeNodeObject g)
    {
        foreach (ITreeNodeObject cg in g.ChildObjects)
        {
            TreeViewNode node = new TreeViewNode(cg.Name, "" + cg.Identity);
            node0.AddChild(node);
            buildTree(node, cg);
        }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
        if (IsPostBack)
        {

            if (MultiSelect)
            {
                string submittedValues = Request["cb" + this.ClientID];
                if (submittedValues != null)
                {
                    Dictionary<string, bool> selectedValuesDict = new Dictionary<string, bool>();
                    string[] splitted = submittedValues.Split(',');
                    foreach (string val in splitted)
                    {
                        selectedValuesDict.Add(val, true);
                    }
                    this.Content.each(delegate(TreeViewNode node)
                    {
                        node.selected = selectedValuesDict.ContainsKey(node.value);
                        if (node.selected)
                            this.Content.expandSubnode(node.value, true, 0);
                        return node;
                    });
                }
            }
            if (PreviousSelectedValue != this.SelectedValue)
            {
                OnSelectedIndexChanged(new EventArgs());
            }
            SelectedValue = SelectedValue;
        }



        string style =
"#" + treeView.ClientID + @"
{
    display:" + (DropDown ? "none" : "") + @";
    position:absolute;
    height:" + (Height) + @"px;
    min-width:" + (Width) + @"px;
    width:auto;
    z-index:99;
    /*overflow-y:scroll;
    overflow-x:hidden;*/
    overflow-y:auto;
    overflow-x:auto;
    border:2px outset silver;
    padding-top:0.2em;
    padding-left:0.2em ;
    padding-right:0px;
    padding-bottom:0.2em ;
    background-color:white;
}
#" + value.ClientID + @"
{
    position:relative;
    padding: 2px;
    float: left;
    display: inline;
    width:" + (Width - 21) + @"px;
    z-index:2;
}
#" + valueFrame.ClientID + @"
{
    position:relative;
    display:" + (DropDown ? "block" : "none") + @";
    height: 18px; 
    width: " + (Width) + @"px; 
    border: 1px solid InactiveCaption;
    padding: 1px 1px 1px 3px;
    cursor:hand;
    background-color:white;
    z-index:1;
    visibility:visible !important;
}

#" + treeView.ClientID + @" .lb
     {
     display:inline;
     }

#" + treeView.ClientID + @" li
     {
     }
#" + treeView.ClientID + @" li .partSelected
     {
        font-style:italic;
     }
#" + treeView.ClientID + @" .cb
     {
     padding:0px;
     margin:-4px 3px 0px 3px;
     " + (CheckBoxes ? "" : "display:none;") + @"
     }
#" + treeView.ClientID + @" .selected
{
    font-weight:bold;
}
#" + treeView.ClientID + @" .disabled
{
    color:silver;
}

.TreeDropdown_openbutton
{
    position:relative;
    z-index:2;
}

";

        HtmlGenericControl StyleTag = new HtmlGenericControl();
        StyleTag.EnableViewState=false;
        StyleTag.Attributes.Add("type", "text/css");
        StyleTag.InnerHtml = style;
        StyleTag.TagName = "style";
        Page.Header.Controls.Add(StyleTag);

        HtmlGenericControl LinkTag = new HtmlGenericControl();
        LinkTag.Attributes.Add("href", "/jQuery/jquery-treeview/jquery.treeview.css");
        LinkTag.Attributes.Add("type", "text/css");
        LinkTag.Attributes.Add("rel", "stylesheet");
        LinkTag.ID = "jquery_treeview_css";
        LinkTag.TagName = "link";
        if (Page.Header.FindControl("jquery_treeview_css") == null)
        {
            Page.Header.Controls.Add(LinkTag);
        }

        ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "jquery.cookie.js", "/jQuery/js/jquery.cookie.js");
        ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "jquery-treeview.js", "/jQuery/jquery-treeview/jquery.treeview.js");
        ScriptManager.RegisterStartupScript(this, this.GetType(), "TreeDropdownScript" + treeView.ClientID,
@"
    <script type='text/javascript'>
        function showTree_" + treeView.ClientID + @"() {
            var jQTree= $('#" + treeView.ClientID + @"');
            jQTree
                .css('left', 0)
                .css('display', 'block');
            $('#" + ImageButton1.ClientID + @"').attr('src','/jQuery/ServerControls/DropDownArrow-n.png');
        }
        
        function hideTree_" + treeView.ClientID + @"() {
            var jQTree= $('#" + treeView.ClientID + @"');
            jQTree.css('display', 'none');
            $('#" + ImageButton1.ClientID + @"').attr('src','/jQuery/ServerControls/DropDownArrow-s.png');"
         + (AutoPostBack ?
                "if(jQTree.data('dirty')==true){" + Page.ClientScript.GetPostBackEventReference(this, "SelectedIndex") + ";}"
              : "") + @"
        }
        
        function toggleTree_" + treeView.ClientID + @"(elem) {
            var jQTree= $('#" + treeView.ClientID + @"');
            if (jQTree.css('display')=='none')
                showTree_" + treeView.ClientID + @"()
            else
                hideTree_" + treeView.ClientID + @"()
        }
        
        function " + this.ID + @"_nodeClicked(value,elem,target)
        {
            if (value > '')
            {
            $('#" + treeView.ClientID + @"').data('dirty',true);
         " + (MultiSelect ? @"
                $('#" + HiddenValue.ClientID + @"').val('');
                $('#" + value.ClientID + @"').text('');
                $('#" + HiddenText.ClientID + @"').val('');
                    
                var cb=$(elem).find('input').get(0)
                if (target != cb)
                    cb.checked=!cb.checked;
                if (cb.checked){
                    $(elem).addClass('" + TreeViewNode.selectedClass + @"');
                }
                else {
                    $(elem).removeClass('" + TreeViewNode.selectedClass + @"');
                }  
                var clientClick=true;
                    " + (this.clientClick != ""
                        ? "clientClick=" + this.clientClick + @"(value,elem,target);"
                        : "")
              : @"
                    $('#" + treeView.ClientID + @" .lb').removeClass('" + TreeViewNode.selectedClass + @"');
                    $(elem).addClass('" + TreeViewNode.selectedClass + @"');
                    $('#" + HiddenValue.ClientID + @"').val(value);
                    $('#" + value.ClientID + @"').text($(elem).text());
                    $('#" + HiddenText.ClientID + @"').val($(elem).text());
                    var clientClick=true;
                    " + (this.clientClick != ""
                        ? "clientClick=" + this.clientClick + @"(value,elem,target);"
                        : "")
                 + @"
                    if (clientClick)
                    { "
                    + (DropDown && MultiSelect == false
                        ? @"setTimeout(hideTree_" + treeView.ClientID + @",100);"
                        : "")
                 + @"}
                 "
                 )
                 + this.ID + @"_adjustParents(elem)
            }
        }
        
        function " + this.ID + @"_adjustParents(elem)
        {
            var $parent= $(elem).parents('li:first');
            if ($parent.length > 0) {
                var selChildren =$parent.children('ul').first().children('li').children('.selected, .partSelected').length;
                if (selChildren > 0)
                    $parent.children('.lb:first').addClass('partSelected');
                else   
                    $parent.children('.lb:first').removeClass('partSelected');
                " + this.ID + @"_adjustParents($parent.get(0))
            }
        }
        
        $(function() {
            setTimeout(function(){
            $('#" + treeView.ClientID + @"').data('dirty',false);
            var  t1=new Date().valueOf();
            $('#" + this.ClientID + @"_tree').treeview({
		        collapsed: true,
                animated: 'fast',          
                prerendered: true,          
                persist: 'cookie'
                " + (this.ClientController != "" ? ",control:" + this.ClientController : "") + @"

            }); 
            var t2=new Date().valueOf();  
            //top.status += ' " + this.ID + @"='+(t2-t1)  
            
            $('#" + ImageButton1.ClientID + @"').click( function(evt){
                    toggleTree_" + treeView.ClientID + @"(this);
                    evt.stopPropagation();
            });
            
            $('#" + valueFrame.ClientID + @"').click( function(){ showTree_" + treeView.ClientID + @"(this);} );

            $('#" + treeView.ClientID + @" .lb').hover(function() { $(this).addClass('hover'); }, function() { $(this).removeClass('hover'); });

            $('#" + treeView.ClientID + @"').click(function(evt){
                if ( $(evt.target).is('#" + treeView.ClientID + @"'))
                    return;
                if ( $(evt.target).is('.hitarea'))
                    return;
                var liElem=$(evt.target).closest('li');
                var elem = $(liElem).children('.lb')[0];
                " + this.ID + @"_nodeClicked($(liElem).children('.lb:first').attr('val'),elem,evt.target);
            });
            $('#" + valueFrame.ClientID + @":parent *').css('visibility','visible');
            },200);
            $('#" + valueFrame.ClientID + @":parent *').css('visibility','visible');
            var oldFocus=window.focus;
            window.focus=function() { 
                $('#" + valueFrame.ClientID + @":parent *').css('visibility','hidden');
                $('#" + valueFrame.ClientID + @":parent *').css('visibility','visible');
                if (oldFocus)
                    return oldFocus();
                }

        })
	</script>", false);
        this.value.InnerText = this.HiddenText.Value;

    }

    public void SetValue (string value, string text)
    {
        this.SelectedValue = value;
        this.value.InnerText = this.HiddenText.Value = text;
    }

    public void SetContent (TreeViewNode pContent)
    {
        Content = pContent;
    }

    protected void Page_PreRender (object sender, EventArgs e)
    {
        TreeViewNode tempRoot = new TreeViewNode("", "");
        tempRoot.children.Add(Content);
        tempRoot.refreshChildSelectionCount();
        treeView.InnerHtml = RenderContent(tempRoot.children, tempRoot);
        PreviousSelectedValue = SelectedValue;
    }

    private string RenderContent (List<TreeViewNode> content, TreeViewNode parentNode)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<ul");
        if (this.Content == content[0])
            sb.Append(" id='" + this.ClientID + "_tree'");
        else
        {
            if (!parentNode.expanded)
            {
                sb.Append(" style='display: none;'");
            }
        }
        sb.AppendLine(">");

        List<TreeViewNode>.Enumerator enumer = content.GetEnumerator();
        TreeViewNode node = null;
        if (enumer.MoveNext())
            node = enumer.Current;
        while (node != null)
        {
            TreeViewNode nextNode = null;
            if (enumer.MoveNext())
                nextNode = enumer.Current;


            string elemClass = "";

            elemClass += " lb";

            if (node.isleaf)
                elemClass += " leaf";

            if (node.clickable == false)
                elemClass = " " + TreeViewNode.disabledClass;

            if (node.selected)
                elemClass += " " + TreeViewNode.selectedClass;

            if (node.hasSelectedChild)
                elemClass += " " + TreeViewNode.partSelectedClass;

            elemClass = elemClass.Trim();

            string hitarea = "";

            if (node.isleaf)
            {
                if (nextNode == null)
                    node.className += " last";
            }
            else
            {
                if (node.expanded)
                {
                    node.className += " collapsable";
                    if (nextNode == null)
                    {
                        node.className += " lastCollapsable";
                        hitarea = "<div class='hitarea collapsable-hitarea lastCollapsable-hitarea'></div>";
                    }
                    else
                        hitarea = "<div class='hitarea collapsable-hitarea'></div>";
                }
                else
                {
                    node.className += " expandable";
                    if (nextNode == null)
                    {
                        node.className += " lastExpandable";
                        hitarea = "<div class='hitarea expandable-hitarea lastExpandable-hitarea'></div>";
                    }
                    else
                        hitarea = "<div class='hitarea expandable-hitarea'></div>";
                }
            }


            string checkbox = "";
            if (MultiSelect)
                checkbox = "<input type='checkbox' class='cb' name='cb" + this.ClientID + "'" + (node.selected ? " checked=checked" : "") + (node.clickable ? "" : " disabled=disabled") + " value='{value}' />";
            string className = (node.className != "" ? " class='" + node.className.Trim() + "'" : "");
            string valueProp = (node.clickable ? " val='{value}'" : "");
            string selected = (elemClass != "" ? " class='" + elemClass + "'" : "");
            selected += (node.title != "" ? " title='" + node.title + "'" : "");
            string nodeTag = "<li{className}>" + hitarea + NodeTemplate;
            nodeTag = nodeTag
                .Replace("{checkbox}", checkbox)
                .Replace("{className}", className)
                .Replace("{valueProp}", valueProp)
                .Replace("{selected}", selected)
                .Replace("{value}", node.value)
                .Replace("{text}", node.text);

            sb.Append(nodeTag);

            if (node.children.Count > 0)
                sb.Append(RenderContent(node.children, node));
            sb.AppendLine("</li>");

            node = nextNode;
        }
        sb.AppendLine("</ul>");
        return sb.ToString();
    }

}
