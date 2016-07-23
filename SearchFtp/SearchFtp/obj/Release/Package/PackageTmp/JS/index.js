function check() {
    var text = document.getElementById("keyword").value;
    if(text== ""){
        alert("不能为空哦！");
        document.getElementById("keyword").focus();
        return false;
    }
    return true;
}