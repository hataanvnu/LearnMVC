
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
    if (what === 1)
        document.location.href = "/Admin/AddCategory";
    else if (what === 2)
        document.location.href = "/Admin/AddQuizUnit";
    else if (what === 3)
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
    alert("CORRECT!");
}

function Next(id) {
    document.location.href = "/Quiz/Text/" + id;
}


$(document).ready(function () {
    $.ajax({
        url: "/Members/GetProgress",
        type: "GET",
        success: function (result) {
            LoadChart(result.doneQuestions, result.totalAmountOfQuestion);
        }
    });


});

function LoadChart(doneQuestions, allQuestions) {

    //Chart JS goes from here ---> SO SUPERAWESOME
    var canvas = document.getElementById("myChart");
    var ctx = canvas.getContext('2d');
    


    // Global Options:
    Chart.defaults.global.defaultFontColor = 'black';
    Chart.defaults.global.defaultFontSize = 16;

    var data = {
        labels: ["Done ", "To Do"],
        datasets: [
            {
                fill: true,
                backgroundColor: [
                    '#fd8be3',
                    'white'],
                data: [doneQuestions, allQuestions], // INSERT PROGRESS HERE
                // Notice the borderColor 
                borderColor: ['black', 'black'],
                borderWidth: [1, 1]
            }
        ]
    };

    // Notice the rotation from the documentation.

    var options = {
        title: {
            display: true,
            text: 'My progress?',
            position: 'top'
        },
        rotation: -0.7 * Math.PI
    };


    // Chart declaration:
    var myBarChart = new Chart(ctx, {
        type: 'pie',
        data: data,
        options: options
    });

} 
