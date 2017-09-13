
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

function ResetCategory(categoryId) {
    document.location.href = "/Quiz/ResetCategory/" + categoryId;
}

function ResetCategoryValidated(categoryId) {
    document.location.href = "/Quiz/ResetCategory/" + categoryId + "?isValidated=true";
}

function Continue(questionId) {
    document.location.href = "/Quiz/Question/" + questionId;
}

function Logout() {
    document.location.href = "/Members/Logout";
}

function ResetProgress() {
    document.location.href = "/Members/ResetProgress";
}

function ResetProgressValidated() {
    document.location.href = "/Members/ResetProgress?isValidated=true";
}

function GoToQuizOverview() {
    document.location.href = "/Admin/QuizOverview";
}

function Yay() {
    alert("Correct answer!");
}