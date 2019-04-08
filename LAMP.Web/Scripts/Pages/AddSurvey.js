$(document).ready(function () {
    $('#AnswerTypedropdown').change(function () {
        if ($('#AnswerTypedropdown').val() != "") {
            var selectedType = $('#AnswerTypedropdown').val();

            if (selectedType == "2") {
                $('#optionsDiv').show();
                $('.optionText_0').focus();
            }
            else {
                $('#optionsDiv').hide();
            }
            $('#answertypeValidator').hide();
        }
        else {
            $('#optionsDiv').hide();
        }
    });
    $('#SurveyName').focus();

});

/**
* AddOptionRow - to add options if Answer type is Scroll wheels.
* @method AddOptionRow
* @param {Button} addButton - addButton
* @return {bool} value
*/
function AddOptionRow(addButton) {

    var option = $(addButton).parent().parent().find('#optionTextBox').val();
    if ($.trim(option) == '') {
        $('#optionsSpan').html("Specify Options.");
        $('#optionsSpan').show();
        $(addButton).parent().parent().find('#optionTextBox').focus();
        return false;
    }
    if (option.indexOf(',') > -1) {
        $('#optionsSpan').html("Comma is not allowed.");
        $('#optionsSpan').show();
        return false;
    }
    $(addButton).parent().find('#removeButton').show();
    $(addButton).hide();

    var rowCount = 0;
    $('#optionsDiv > div').each(function () {
        rowCount++;
    });
    console.log(rowCount);
    if (rowCount < 10) {
        var html = '<div class="add-sec" ><input type="text" class="form-control optInput optionText_' + rowCount + '" id="optionTextBox" onkeypress="GetValueKeyPress(this, \'options\')" onkeyup="GetValueKeyPress(this, \'options\')"  maxlength="100"><input type="hidden" id="optionId" class="optionId" value="0"/>';
        html += '<div class="controls"><button type="button" class="glyphicon glyphicon-plus add-icn addButton"  id="addButton" onclick="return AddOptionRow(this)"></button>';
        html += '<button type="button" class="glyphicon glyphicon-remove add-icn removeButton" id="removeButton" onclick="return RemoveOptionRow(this)" style="display:none"></button></div></div>';

        $('#optionsDiv').append(html);
        $('.optionText_' + rowCount).focus();
    }
    return true;
}

/**
* RemoveOptionRow - To remove a row.
* @method RemoveOptionRow
* @param {Button} removeButton - removeButton
* @return {bool} value
*/
function RemoveOptionRow(removeButton) {
    var count = $('.removeButton').length;
    console.log("count:" + count);
    count = count - 1;
    var addButtonCount = $('.addButton:visible').length;
    console.log('addButtonCount:' + addButtonCount);
    $(removeButton).parent().parent().remove();
    if (addButtonCount < 1) {
        var html = '<div class="add-sec" ><input type="text" class="form-control optInput optionText_' + count + '" id="optionTextBox" onkeypress="GetValueKeyPress(this, \'options\')" onkeyup="GetValueKeyPress(this, \'options\')"  maxlength="100"><input type="hidden" id="optionId" class="optionId" value="0"/>';
        html += '<div class="controls"><button type="button" class="glyphicon glyphicon-plus add-icn addButton"  id="addButton" onclick="return AddOptionRow(this)"></button>';
        html += '<button type="button" class="glyphicon glyphicon-remove add-icn removeButton" id="removeButton" onclick="return RemoveOptionRow(this)" style="display:none"></button></div></div>';
    }
    $('#optionsDiv').append(html);
    return true;
}

/**
* GetAnswerTypeOptions - To validate the form controls and get the option values on submitting the form.
* @method GetAnswerTypeOptions
* @param -
* @return {bool} value
*/
function GetAnswerTypeOptions() {
    var valid = true;
    var selectedType = $('#AnswerTypedropdown').val();
    var optionsCount = 0;
    $('#Options').val('');

    if ($.trim($('#SurveyName').val()) == "") {
        $('#surveyNameValidator').html('Specify Survey Name.');
        $('#surveyNameValidator').show();
        valid = false;
    }
    else if ($.trim($('#SurveyName').val()).length > 60) {
        $('#surveyNameValidator').html('Specify a survey name not exceeding 60 characters.');
        $('#surveyNameValidator').show();
        valid = false;
    }
    else {
        var text = $('#SurveyName').val();
        if (text.match(/[<>]/)) {
            $('#surveyNameValidator').html('Specify Survey Name without HTML tags.');
            $('#surveyNameValidator').show();
            valid = false;
        }
    }

    var surveyId = $('#SurveyId').val();

    if (surveyId == "" || surveyId == "0") {
        if ($.trim($('#QuestionText').val()) == "") {
            $('#questionValidator').html('Specify Question.');
            $('#questionValidator').show();
            valid = false;
        }
        if (selectedType == "") {
            $('#answertypeValidator').show();
            valid = false;
        }
    }
    else {
        if ($('#QuestionText').val() != "") {
            if (selectedType == "") {
                $('#answertypeValidator').show();
                valid = false;
            }
        }
    }

    if ($('#QuestionText').val() != "") {
        if ($.trim($('#QuestionText').val()).length > 100) {
            $('#questionValidator').html('Specify a question text not exceeding 100 characters.');
            $('#questionValidator').show();
            valid = false;
        }
    }

    var text = $('#QuestionText').val();
    if (text.match(/[<>]/)) {
        $('#questionValidator').html('Specify Question Text without HTML tags.');
        $('#questionValidator').show();
        valid = false;
    }

    if (selectedType == "2") {
        $('#optionsDiv > div').each(function () {
            if ($(this).find(".optInput").val() != '') {
                var text = $.trim($(this).find(".optInput").val());
                if (text != '') {
                    if (text.length > 100) {
                        $('#optionsSpan').html('Specify an option not exceeding 100 characters.');
                        $('#optionsSpan').show();
                        valid = false;
                    }
                }
                if (text.match(/[<>]/)) {
                    $('#optionsSpan').html('Specify Options without HTML tags.');
                    $('#optionsSpan').show();
                    valid = false;
                }

                if (text.indexOf(',') > -1) {
                    $('#optionsSpan').html('Comma is not Allowed');
                    $('#optionsSpan').show();
                    valid = false;
                }
                var optionText = $(this).find(".optionId").val() + "_" + $(this).find(".optInput").val();

                var options = $('#Options').val();
                if (optionText != '') {
                    if (options != "")
                        options += "," + optionText;
                    else
                        options += optionText;
                    $('#Options').val(options);
                }
                optionsCount++;
            }

        });

        if (optionsCount == 0) {
            $('#optionsSpan').html("Specify Options.");
            $('#optionsSpan').show();
            valid = false;
        }
        else {
            if (optionsCount < 2) {
                $('#optionsSpan').html("Please add at least 2 options.");
                $('#optionsSpan').show();
                valid = false;
            }
        }
    }

    if (valid == false)
        return false;
    else {
        return true;
    }
}

/**
* EditQuestion - To populate the question text and answer type on clicking the edit button in the questions list.
* @method EditQuestion
* @param - {String} qId, {String} qText, {String} ansType, {String} optionsList
* @return {bool} value
*/
function EditQuestion(qId, qText, ansType, optionsList) {
    $('#QuestionId').val(qId);
    $("#QuestionText").prop("disabled", false);
    $("#AnswerTypedropdown").prop("disabled", false);
    $('#QuestionText').focus();

    var url = appPath + 'Survey/EditSurveyQuestion/';
    $.ajax({
        async: false,
        cache: false,
        type: 'POST',
        url: url,
        data: { QuestionId: qId },
        success: function (result) {

            if (result.Status == 0) {
                $('#QuestionText').val(result.QuestionText);
                $('#AnswerTypedropdown').val(result.AnswerType);
                if (result.AnswerType == 2 && result.Options != "") {

                    var optionsList = result.Options;
                    var optionsArray = [];
                    if (optionsList.indexOf(',') > 0)
                        optionsArray = optionsList.split(',');
                    else
                        optionsArray.push(optionsList);

                    if (optionsArray.length > 0) {
                        $('#optionsDiv').html('');
                        $('#optionsDiv').append('<label>Options</label>');
                        var rowCount = optionsArray.length;

                        for (var i = 0; i < rowCount; i++) {
                            var optionId = optionsArray[i].split("_")[0];
                            var optionText = optionsArray[i].split("_")[1];

                            var html = '<div class="add-sec" ><input type="text" class="form-control optInput optionText_' + i + '" id="optionTextBox" value="' + optionText + '" onkeypress="GetValueKeyPress(this, \'options\')" onkeyup="GetValueKeyPress(this, \'options\')"  maxlength="100"><input type="hidden" id="optionId" class="optionId" value="' + optionId + '"/>';
                            html += '<div class="controls"><button type="button" class="glyphicon glyphicon-plus add-icn" id="addButton" onclick="return AddOptionRow(this)"  style="display:none"></button>';
                            html += '<button type="button" class="glyphicon glyphicon-remove add-icn" id="removeButton" onclick="return RemoveOptionRow(this)"></button></div></div>';
                            $('#optionsDiv').append(html);
                        }
                        if (rowCount < 9) {
                            var html = '<div class="add-sec" ><input type="text" class="form-control optInput optionText_' + rowCount + '" id="optionTextBox" onkeypress="GetValueKeyPress(this, \'options\')" onkeyup="GetValueKeyPress(this, \'options\')"  maxlength="100"><input type="hidden" id="optionId" class="optionId" value="0"/>';
                            html += '<div class="controls"><button type="button" class="glyphicon glyphicon-plus add-icn" id="addButton" onclick="return AddOptionRow(this)"></button>';
                            html += '<button type="button" class="glyphicon glyphicon-remove add-icn" id="removeButton" onclick="return RemoveOptionRow(this)" style="display:none"></button></div></div>';
                            $('#optionsDiv').append(html);
                        }

                        $('.optionText_' + rowCount).focus();
                    }
                    $('#optionsDiv').show();
                }
                else {
                    $('#optionsDiv').hide();
                }
            }
        },
        error: function (data) {
            alert('Error');
        }
    });    
}

/**
* AddQuestion - To clear the question fields to add a new question to theexisting survey.
* @method AddQuestion
* @param - 
* @return {void} 
*/
function AddQuestion() {
    $("#AnswerTypedropdown").prop("disabled", false);
    $("#QuestionText").prop("disabled", false);
    $('#QuestionText').focus();
    $('#QuestionId').val('');
    $('#Options').val('');
    $("#QuestionText").val('');
    $("#AnswerTypedropdown").val('');

    $('#optionsDiv').html('');
    $('#optionsDiv').append('<label>Options</label>');
    var html = '<div class="add-sec" ><input type="text" class="form-control optInput optionText_0" id="optionTextBox" onkeypress="GetValueKeyPress(this, \'options\')" onkeyup="GetValueKeyPress(this, \'options\')"><input type="hidden" id="optionId" class="optionId" value="0"/>';
    html += '<div class="controls"><button type="button" class="glyphicon glyphicon-plus add-icn" id="addButton" onclick="return AddOptionRow(this)"></button>';
    html += '<button type="button" class="glyphicon glyphicon-remove add-icn" id="removeButton" onclick="return RemoveOptionRow(this)" style="display:none"></button></div></div>';
    $('#optionsDiv').append(html);
}

/**
* Cancel - To go back to survey listing page.
* @method Cancel
* @param - 
* @return {void} 
*/
function Cancel() {
    var url = appPath + 'Survey/Index/';
    window.location.href = url;
}

/**
* GetValueKeyPress - To hide the validator <span> while entering value to the input text box.
* @method GetValueKeyPress
* @param - {TextBox} optionTextBox, {String} control
* @return {void} 
*/
function GetValueKeyPress(optionTextBox, control) {
    var option = $(optionTextBox).val();
    if ($.trim(option) != "") {
        switch (control) {
            case 'options':
                $('#optionsSpan').hide();
                break;
            case 'surveyname':
                $('#surveyNameValidator').hide();
                break;
            case 'question':
                $('#questionValidator').hide();
            default:
        }

    }
}

/**
* ShowDeleteQuestionConfirmationPopup - To show the delete question confirmation pop up.
* @method ShowDeleteQuestionConfirmationPopup
* @param - {String} questionId
* @return {void} 
*/
function ShowDeleteQuestionConfirmationPopup(questionId) {
    $('#confirmMessage').text("Are you sure you want to delete this question permanently?");
    $('#DeleteQuestionPopupConfirmation').modal('show');
    $('#CurrentQuestionId').val(questionId);
    return false;
}

/**
* DeleteQuestion - To delete the selected question.
* @method DeleteQuestion
* @param - 
* @return {void} 
*/
function DeleteQuestion() {
    var messagehtml = "<div class=\"icon\"><i class=\"fa fa-check-circle\" style=\"cursor:pointer;\" onclick=\"$('#msgsuccess').hide();\"></i></div>";

    var url = appPath + 'Survey/DeleteSurveyQuestion/';
    $.ajax({
        async: false,
        cache: false,
        type: 'POST',
        url: url,
        data: { QuestionId: $('#CurrentQuestionId').val() },
        success: function (result) {
            $('#DeleteQuestionPopupConfirmation').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();

            if (result.Status == 0) {
                messagehtml = messagehtml + "The question has been deleted.<br/>";
                $('#msgsuccess').show();
                $("#msgsuccess").html(messagehtml);
                setTimeout(function () { $("#msgsuccess").hide(); location.reload(true); }, 2000);
            }
            else {
                $('#alertMessage').text('This question cannot be deleted as at least one question has to be maintained in the survey.');
                $('#popupAlert').modal('show');
            }
        },
        error: function (data) {
            alert('Error');
        }
    });
}