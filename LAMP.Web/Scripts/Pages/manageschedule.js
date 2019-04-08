/**
* To Show Delete Schedule Survey Popup Confirmation
* @method ShowDeleteScheduleSurveyPopupConfirmation
* @param {Integer} surveyId - surveyId
* @param {String} toDo - toDo
*/
function ShowDeleteScheduleSurveyPopupConfirmation(surveyId, toDo) {
    $('#btnDelete').hide();
    switch (toDo) {
        case 'Delete': $('#confirmMessageSurvey').text("Are you sure you want to delete this survey schedule permanently?");
            $('#btnDelete').show();
            break;
    }
    $('#DeleteScheduleSurveyPopupConfirmation').modal('show');
    $('#currentSurveyId').val(surveyId);
    return false;
}

/**
* To Delete Schedule Survey
* @method DeleteScheduleSurvey
*/
function DeleteScheduleSurvey() {
    var messagehtml = "<div class=\"icon\"><i class=\"fa fa-check-circle\" style=\"cursor:pointer;\" onclick=\"$('#msgsuccessSurvey').hide();\"></i></div>";
    messagehtml = messagehtml + "The survey schedule has been deleted.<br/>";
    var url = appPath + 'UserAdmin/DeleteSurveySchedule/';
    setTimeout(function () {
        $('#loaderImage').show();
    }, 2000);
    $.ajax({
        async: false,
        cache: false,
        type: 'POST',
        url: url,
        data: { surveyId: $('#currentSurveyId').val() },
        success: function (result) {
            $('#DeleteScheduleSurveyPopupConfirmation').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $('#msgsuccessSurvey').show();
            $("#msgsuccessSurvey").html(messagehtml);
            setTimeout(function () {
                $("#msgsuccessSurvey").hide();
                $("#loaderImage").hide();
                window.location = appPath + "UserAdmin/ManageSchedule";
            }, 2000);
        },
        error: function (data) {
        }
    });
}

/**
* To Show Delete Schedule Game Popup Confirmation
* @method ShowDeleteScheduleGamePopupConfirmation
* @param {Integer} cTestId - cTestId
* @param {String} toDo - toDo
*/
function ShowDeleteScheduleGamePopupConfirmation(cTestId, toDo) {
    $('#btnDelete').hide();
    switch (toDo) {
        case 'Delete': $('#confirmMessageGame').text("Are you sure you want to delete this game schedule permanently?");
            $('#btnDelete').show();
            break;
    }
    $('#DeleteGamePopupConfirmation').modal('show');
    $('#currentGameId').val(cTestId);
    return false;
}

/**
* To Delete Schedule Game
* @method DeleteScheduleGame
*/
function DeleteScheduleGame() {
    var messagehtml = "<div class=\"icon\"><i class=\"fa fa-check-circle\" style=\"cursor:pointer;\" onclick=\"$('#msgsuccessgame').hide();\"></i></div>";
    messagehtml = messagehtml + "The game schedule has been deleted.<br/>";
    var url = appPath + 'UserAdmin/DeleteGameSchedule/';
    setTimeout(function () {
        $('#loaderImage').show();
    }, 2000);
    $.ajax({
        async: false,
        cache: false,
        type: 'POST',
        url: url,
        data: { cTestId: $('#currentGameId').val() },
        success: function (result) {
            $('#DeleteGamePopupConfirmation').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $('#msgsuccessgame').show();
            $("#msgsuccessgame").html(messagehtml);
            setTimeout(function () {
                $("#msgsuccessgame").hide();
                $("#loaderImage").hide();
                window.location = appPath + "UserAdmin/ManageSchedule";
            }, 2000);
        },
        error: function (data) {
            alert('Error');
        }
    });
}

/**
* To Show Delete Schedule Batch Popup Confirmation
* @method ShowDeleteScheduleBatchPopupConfirmation
* @param {Integer} batchId - batch Id
* @param {String} toDo - toDo
*/
function ShowDeleteScheduleBatchPopupConfirmation(batchId, toDo) {
    $('#btnDelete').hide();
    switch (toDo) {
        case 'Delete': $('#confirmMessageBatch').text("Are you sure you want to delete this batch schedule permanently?");
            $('#btnDelete').show();
            break;
    }
    $('#DeleteScheduleBatchPopupConfirmation').modal('show');
    $('#currentBatchId').val(batchId);
    return false;
}

/**
* To Delete Schedule Batch
* @method DeleteScheduleBatch
*/
function DeleteScheduleBatch() {
    var messagehtml = "<div class=\"icon\"><i class=\"fa fa-check-circle\" style=\"cursor:pointer;\" onclick=\"$('#msgsuccessBatch').hide();\"></i></div>";
    messagehtml = messagehtml + "The batch schedule has been deleted.<br/>";
    var url = appPath + 'UserAdmin/DeleteBatchSchedule/';
    $('#loaderImage').show();
    $.ajax({
        async: false,
        cache: false,
        type: 'POST',
        url: url,
        data: { batchId: $('#currentBatchId').val() },
        success: function (result) {
            $('#DeleteScheduleBatchPopupConfirmation').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $('#msgsuccessBatch').show();
            $("#loaderImage").hide();
            $("#msgsuccessBatch").html(messagehtml);
            setTimeout(function () {
                $("#msgsuccessBatch").hide();
                window.location = appPath + "UserAdmin/ManageSchedule";
            }, 2000);
        },
        error: function (data) {
            $("#loaderImage").hide();            
        }
    });
}

/**
* @method SuccessPaginationUser
*/
function SuccessPaginationUser() {
    $("#loaderImage").hide();
    $('body,html').animate({
        scrollTop: 0
    }, 300);
}

/**
* @method BeginPaginaionUser
*/
function BeginPaginaionUser() {
    $('#loaderImage').show();
}

/**
* @method OnBeginAjaxCall_User
*/
function OnBeginAjaxCall_User() {
    $('#loaderImage').show();
}

/**
* @method OnSuccessAjaxCall_User
*/
function OnSuccessAjaxCall_User() {
    $("#loaderImage").hide();
}

/**
* @method OnFailureAjaxCall_User
* @param {Object} data - data
*/
function OnFailureAjaxCall_User(data) {
    $("#loaderImage").hide();
}

/**
* To Show Alert
* @method ShowAlert
* @param {String} message - message
*/
function ShowAlert(message) {
    $('#alertMessage').text(message);
    $('#popupAlert').modal('show');
}

/**
* Sort User list
* @method SortUserList
* @param {String} columnName - columnName
* @return {bool} value
*/
function SortUserList(columnName) {
    var previousColumn = $("#SortField").val();
    if (columnName == null || columnName == '') {
        $("#SortField").val("StudyId")
    }
    else {
        $("#SortField").val(columnName);
    }
    // Set sort order
    if ($("#SortOrder").val() == 'asc') {
        $("#SortOrder").val("desc");
    }
    else if ($("#SortOrder").val() == 'desc') {
        $("#SortOrder").val("asc");
    }
    else {
        $("#SortOrder").val("asc"); // Default is set to asscending order
    }

    $("#CurrentPage").val(1);
    $('#surveyListForm').trigger('submit');
    return true;
}

/**
* Sort Game List
* @method SortGameList
* @param {String} columnName - columnName
*/
function SortGameList(columnName) {
    var previousColumn = $("#SortFieldGame").val();
    if (columnName == null || columnName == '') {
        $("#SortFieldGame").val("GameName")
    }
    else {
        $("#SortFieldGame").val(columnName);
    }
    // Set sort order
    if ($("#SortOrderGame").val() == 'asc') {
        $("#SortOrderGame").val("desc");
    }
    else if ($("#SortOrderGame").val() == 'desc') {
        $("#SortOrderGame").val("asc");
    }
    else {
        $("#SortOrderGame").val("asc"); // Default is set to asscending order
    }

    $("#CurrentPage").val(1);
    $('#gameListForm').trigger('submit');
    return true;
}

/**
* Sort Schedule Batch List
* @method SortScheduleBatchList
* @param {String} columnName - columnName
*/
function SortScheduleBatchList(columnName) {
    var previousColumn = $("#SortFieldGame").val();
    if (columnName == null || columnName == '') {
        $("#SortFieldGame").val("BatchName")
    }
    else {
        $("#SortFieldGame").val(columnName);
    }
    // Set sort order
    if ($("#SortOrderGame").val() == 'asc') {
        $("#SortOrderGame").val("desc");
    }
    else if ($("#SortOrderGame").val() == 'desc') {
        $("#SortOrderGame").val("asc");
    }
    else {
        $("#SortOrderGame").val("asc"); // Default is set to asscending order
    }

    $("#CurrentPage").val(1);
    $('#ScheduleBatchListForm').trigger('submit');
    return true;
}