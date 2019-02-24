/*
*首页js特效
*菜单切换
*作者：wyd 2018-02-24
*/
$(function(){
	loadMenuTree(true); //加载导航菜单
});

//加载导航菜单
function loadMenuTree() {
    // //发送AJAX请求
    // $.ajax({
    //     type: "post",
    //     url: "http://localhost:1803/async/mng_async.ashx?action=test&time=" + Math.random(),
    //     dataType: "html",
    //     success: function (data, textStatus) {
    //         //将得到的数据插件到页面中
    //         $("#sidebar-nav").html(data);
    //         //初始化导航菜单
    //         initMenuTree();
    //     }
    // });
    initMenuTree();
}

//初始化导航菜单
function initMenuTree() {
    var navObj = $("#main-nav");
    var navGroupObj = $("#sidebar-nav .sidebar-nav-group");
    //先清空NAV菜单内容
    navObj.html('');
    navGroupObj.each(function (i) {
        //添加菜单导航
        var navHtml = $('<li><a data-toggle="tab" href="#"><i class="' + $(this).attr("main-nav-icon") + '"></i><span>' + $(this).attr("main-nav-title") + '</span></a></li>').appendTo(navObj);
        //默认选中第一项
        if (i == 0) {
            $(this).show();
            navHtml.addClass("active");
        }
        //为菜单添加事件
        navHtml.click(function () {
            navGroupObj.hide();
            navGroupObj.eq(navObj.children("li").index($(this))).show();
            navGroupObj.eq(navObj.children("li").index($(this))).children("ul").addClass("in");
        });
    });
    //首先隐藏所有的UL
    navGroupObj.hide();
    navGroupObj.eq(0).show();//展开第一个菜单
    navGroupObj.eq(0).children("ul").addClass("in");//
}