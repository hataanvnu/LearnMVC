
function DoStuff(id) {
    document.location.href = "/Quiz/Text/" + id;
}

function GoToIndex() {
    document.location.href = "/Members/Index";
}


function Plus(id) {
    document.location.href = "/Quiz/Question/" + id;
}

function GoToResult() {
    document.location.href = "/Admin/Resultat";
}

function GoToCreateNew(what) {
    if (what == 1)
        document.location.href = "/Admin/AddCategory";
    else if (what == 2)
        document.location.href = "/Admin/AddQuizUnit";
    else if (what == 3)
        document.location.href = "/Admin/AddQuestion";

}

