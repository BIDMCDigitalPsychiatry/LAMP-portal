
/**
* ShowDeleteSurveyConfirmationPopup - To show the delete question confirmation pop up.
* @method ShowDeleteSurveyConfirmationPopup
* @param - {String} surveyId
* @return {bool} value 
*/
function ShowDeleteSurveyConfirmationPopup(surveyId) {
    $('#confirmMessage').text("Are you sure you want to delete this survey permanently?");
   $('#DeleteSurveyPopupConfirmation').modal('show');
   $('#CurrentSurveyId').val(surveyId);
    return false;
}

/**
* DeleteSurvey - To delete the selected survey.
* @method DeleteSurvey
* @param - 
* @return {void} 
*/
function DeleteSurvey() {
    var messagehtml = "<div class=\"icon\"><i class=\"fa fa-check-circle\" style=\"cursor:pointer;\" onclick=\"$('#msgsuccess').hide();\"></i></div>";
    messagehtml = messagehtml + "The survey has been deleted.<br/>";
    var url = appPath + 'Survey/DeleteSurvey/';
    $.ajax({
        async: false,
        cache: false,
        type: 'POST',
        url: url,
        data: { SurveyId: $('#CurrentSurveyId').val() },
        success: function (result) {
            $('#DeleteSurveyPopupConfirmation').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $('#msgsuccess').show();
            $("#msgsuccess").html(messagehtml);
            setTimeout(function () {
                $("#msgsuccess").hide(); $("#loaderImage").hide();
                window.location = appPath + "Survey/Index";
            }, 2000);
            setTimeout(function () {
                $('#loaderImage').show();
            }, 2000);
        },
        error: function (data) {
            alert('Error');
        }
    });
}

/**
* DeleteSurvey - To sort the survey list.
* @method SortSurveyList
* @param - {String} columnName
* @return {void} 
*/
function SortSurveyList(columnName) {
    var previousColumn = $("#SortField").val();
    if (columnName == null || columnName == '') {
        $("#SortField").val("SurveyName")
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

