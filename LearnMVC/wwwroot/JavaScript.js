
function DoStuff(id) {
    document.location.href = "/Quiz/Text/" + id;
}

function GoToIndex() {
    document.location.href = "/Members/Index";
}


function Plus(id) {
    document.location.href = "/Quiz/Question/" + id;
}

function SubmitAnswer(id) {
    document.location.href = "/Quiz/Question/" + id;
}


$(document).ready(function () {
    $("input[type='button']").click(function () {
        var radioValue = $("input[name='optradio']:checked").val();
        if (radioValue) {
            alert(radioValue);
        }
    });

});

//$("#sidebarzItem, #sidebarzItemDone").click(function () {
//    alert("yey!");
//    var id = this.CategoryID;
//    window.location.replace("/Quiz/Text/" + id);
//})

//$("#open_same_window").click(function () {
//    //this will find the selected website from the dropdown
//    var go_to_url = $("#websites").find(":selected").val();

//    //this will redirect us in same window
//    
//});