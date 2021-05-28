$(document).ready(
    function () {

        $(".tree-checkbox-hierarchical").fancytree({
            checkbox: true,
            selectMode: 3,
            select: function (event, data) {
                // A node was activated: display its title:

                GetSelectedTreeNode("readdyTree", "StrAllowedMenus");
                //var node = data.node;
                //if (node.selected) {
                //    var LstSelectedMenu = $("#LstSelectedMenu").val();
                //    LstSelectedMenu += ","+ node.key;
                //    $("#LstSelectedMenu").val(LstSelectedMenu)
                //    alert(LstSelectedMenu);
                //}
                //console.log(node.key);
                //else
                //    console.log("ko duowc chon");
            },


        });


    }
);

$(document).ready(function () {
    ActiveMenu();
});

function GetSelectedTreeNode(treeID,tagInput) {
   // alert(treeID);
    var tree = $("#" + treeID).fancytree("getTree");

    selNodes = tree.getSelectedNodes();
    var lsSelected = "";
    for (i = 0; i < selNodes.length; i++) {
        lsSelected += "," +selNodes[i].key;
    }
    if (lsSelected.length>0)
    lsSelected = lsSelected.substring(1);
    $("#" + tagInput).val(lsSelected);
    


}
function ActiveMenu() {
    $('.level-1').each(function () {
        if ($(this).children('ul').length > 0) {
            $(this).addClass('parent');
        }
    });
    $('li.parent > a').click(function () {
        $(this).parent().toggleClass('active');
    });
    menu();
}
function menu() {
    $('li.currentactive').parentsUntil('div').toggleClass('active');
    $('li.active').parent().css({ "display": "block" });
    $('li.currentactive').parent().css({ "display": "block" });
    $('ul').removeClass('active');
    $('li.currentactive > a').children().css({ "color": "yellow" });
    $('li.currentactive > a:focus').css({ "background-color": "#26A69A" });
}
