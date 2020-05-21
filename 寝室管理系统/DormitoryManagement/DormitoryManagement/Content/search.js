var controllerNamer = "";//用于保存当前所在页
var url = window.location.pathname;
var a = url.split("/");
controllerNamer = a[1];//当前所在页面
var pla = "";
if (controllerNamer == "Dormitory") {
    pla = "请输入寝室编号"
} else if (controllerNamer == "Student") {
    pla = "请输入学生姓名"
} else if (controllerNamer == "Floor") {
    pla = "请输入楼号"
} else if (controllerNamer == "Class") {
    pla = "状态或班级名或班级名加状态或状态加班级名"
} else if (controllerNamer == "ClassRecord") {
    pla = "请输入班级编号或者学生姓名"
} else if (controllerNamer == "StudentRecord") {
    pla = "请输入寝室编号或者学生姓名"
}
$("#Search_input").attr('placeholder', pla);
//实时查询
var vbs = false;
$("#Search_input").on('compositionstart', function () {
    // 输入汉语拼音时锁住搜索框，不进行搜索，或者从汉语拼音转到字母时也可触发
    vbs = true;
});
$("#Search_input").on('compositionend', function () {
    // 结束汉语拼音输入并生成汉字时，解锁搜索框，进行搜索
    vbs = false;
    Conditions = $("#Search_input").val();
    //搜索
    Search(Conditions);
    if (Conditions != "") {
        //联想词
        Lenovo(Conditions);
        //联想词选择功能
        Le_Choose();
    }
});
//$("#Search_input").bind("input propertychange", function () {
//    value = $("#Search_input").val();
//    Search(value);
//});
var Conditions = "";
//联想词选择功能
function Le_Choose() {
    $.when(Le).done(function () {
        $(".Le_ul").children("li").click(function () {
            var val=($(this).text());
            $("#Search_input").val(val);
            Search(val);
            $("#LenovoBox").html("");
        })
    });
};  
$(document).ready(function () {
 
    $(".srh-btn").click(function () {
        $("#Search_input").val("");
        $("#LenovoBox").html("");
    });
    //当搜索框失去焦点
    $("#Search_input").blur(function () {
        Conditions = $("#Search_input").val();//保存搜索内容
        if (Conditions == "" || Conditions == null) {
            Search(Conditions);
            $("#LenovoBox").html("");
        } else {
            //查询
            Search(Conditions);
        }
    });

    //回车键事件
    $(document).keyup(function (event) {
        if (event.keyCode == "13") {//keyCode=13是回车键
            Search(Conditions);
        }

    });
    function change() {
        Conditions = $("#Search_input").val();//保存搜索内容
        if (Conditions != "") {
            //联想词
            Lenovo(Conditions);
            //联想词选择
            Le_Choose();
            
        }
    };
    $("#Search_input").on('input propertychange', change);//当搜索框内值发生改变时

});

