/**
* To Validate Cognition Time
* @method ValidateCognitionTime
* @return {bool} Status
*/
function ValidateCognitionTime() {
    var session = $('#ddlCognitionTestSlotID').val();
    var flag = true;
    var timeSelected = $('#txtCognitionTestSlotTime').val();
    var timeSplit = timeSelected.split(":");
    var hourTime = parseInt(timeSplit[0]);
    if (session == 1) {
        if (timeSelected.indexOf("PM") !== -1) {
            $('#cognitionTimeValidation').html("Select Morning Cognition Slot Time.");
            $('#cognitionTimeValidation').show();
            flag = false;
        }
    }
    else
        if (session == 2) {
            if (timeSelected.indexOf("AM") !== -1) {
                $('#cognitionTimeValidation').html("Select Afternoon Cognition Slot Time.");
                $('#cognitionTimeValidation').show();
                flag = false;
            }
            else if (hourTime < 12 && hourTime >= 5) {
                $('#cognitionTimeValidation').html("The available Afternoon Cognition Slot is between 12:00 PM and 04:59 PM. Please select a time during this period.");
                $('#cognitionTimeValidation').show();
                flag = false;
            }
        }
        else if (session == 3) {
            if (timeSelected.indexOf("AM") !== -1) {
                $('#cognitionTimeValidation').html("Select Evening Cognition Slot Time.");
                $('#cognitionTimeValidation').show();
                flag = false;
            }
            else if (hourTime < 5 || hourTime == 12) {
                $('#cognitionTimeValidation').html("The available Evening Cognition Slot is between 05:00 PM and 11:59 PM. Please select a time during this period. ");
                $('#cognitionTimeValidation').show();
                flag = false;
            }
        }
    return flag;
}

/**
* To Get Default Slotime
* @method GetDefaultSlotime
* @param {String} slot - slot
* @return {String} Time
*/
function GetDefaultSlotime(slot) {
    if (slot == "1") {
        return "12:00 AM";
    } else if (slot == "2") {
        return "12:00 PM";
    }
    else if (slot == "3") {
        return "05:00 PM";
    }
}

/**
* To Validate SurveyTime
* @method ValidateSurveyTime
* @return {bool} status
*/
function ValidateSurveyTime() {
    var session = $('#ddlSurveySlotID').val();
    var flag = true;
    var timeSelected = $('#txtSurveySlotTime').val();
    console.log(timeSelected);
    var timeSplit = timeSelected.split(":");
    var hourTime = parseInt(timeSplit[0]);
    if (session == 1) {
        if (timeSelected.indexOf("PM") !== -1) {
            $('#surveyTimeValidation').html("Select Morning Survey Slot Time.");
            $('#surveyTimeValidation').show();
            flag = false;
        }
    }
    else
        if (session == 2) {
            if (timeSelected.indexOf("AM") !== -1) {
                $('#surveyTimeValidation').html("Select Afternoon Survey Slot Time.");
                $('#surveyTimeValidation').show();
                flag = false;
            }
            else if (hourTime < 12 && hourTime >= 5) {
                $('#surveyTimeValidation').html("The available Afternoon Survey Slot is between 12:00 PM and 04:59 PM. Please select a time during this period.");
                $('#surveyTimeValidation').show();
                flag = false;
            }
        }
        else if (session == 3) {
            if (timeSelected.indexOf("AM") !== -1) {
                $('#surveyTimeValidation').html("Select Evening Survey Slot Time.");
                $('#surveyTimeValidation').show();
                flag = false;
            }
            else if (hourTime < 5 || hourTime == 12) {
                $('#surveyTimeValidation').html("The available Evening Survey Slot is between 05:00 PM and 11:59 PM. Please select a time during this period.");
                $('#surveyTimeValidation').show();
                flag = false;
            }
        }

    return flag;
}

/**
* @method formatDate
* @param {DateTime} date - date
* @return {String} Time
*/
function formatDate(date) {
    // formats a javascript Date object into a 12h AM/PM time string
    var hour = date.getHours();
    var minute = date.getMinutes();
    var amPM = (hour > 11) ? "pm" : "am";
    if (hour > 12) {
        hour -= 12;
    } else if (hour == 0) {
        hour = "12";
    }
    if (minute < 10) {
        minute = "0" + minute;
    }
    return hour + ":" + minute + amPM;
}

/**
* To save schedule details
* @method saveschedule
*/
function saveschedule() {
    var status = true;
    if ($('#txtSurveySlotTime').val() == "")
    {
        $('#SurveySlotTimeValidation').show();
        status = false;
    }
    else
    {
        $('#SurveySlotTimeValidation').hide();
    }
    if ($('#txtCognitionTestSlotTime').val() == "") {
        $('#CognitionSlotTimeValidation').show();
        status = false;
    }
    else {
        $('#CognitionSlotTimeValidation').hide();
    }
    
    if (!ValidateCognitionTime())
        status = false;
    else {
        $('#cognitionTimeValidation').hide();
    }
    if (!ValidateSurveyTime())
        status = false;
    else {
        $('#surveyTimeValidation').hide();
    }
    if ($("#ddlSurveyId option:selected").length == 0) {
        $('#surveyValidation').show();
        status = false;
    }
    else {
        $('#surveyValidation').hide();
    }
    if ($("#ddlCognitionTestId option:selected").length == 0) {
        $('#cognitionValidation').show();
        status = false;
    }
    else {
        $("#cognitionValidation").hide();
    }
    if (status == false) {
        return false;
    }
    else {
        var today = new Date();
        var date = today.getFullYear() + '/' + (today.getMonth() + 1) + '/' + today.getDate();
        var time = $('#txtSurveySlotTime').val();
        var hours = Number(time.match(/^(\d+)/)[1]);
        var minutes = Number(time.match(/:(\d+)/)[1]);
        var AMPM = time.match(/\s(.*)$/)[1];
        if (AMPM == "PM" && hours < 12) hours = hours + 12;
        if (AMPM == "AM" && hours == 12) hours = hours - 12;
        var sHours = hours.toString();
        var sMinutes = minutes.toString();
        if (hours < 10) sHours = "0" + sHours;
        if (minutes < 10) sMinutes = "0" + sMinutes;
        var slotTime = sHours + ":" + sMinutes;
              
        var dateTime = date + ' ' + slotTime;
        var surveySlotTime = new Date(dateTime);
        $('#txtSurveySlotTime').val(surveySlotTime.toUTCString());
        var time = $('#txtCognitionTestSlotTime').val();
        var hours = Number(time.match(/^(\d+)/)[1]);
        var minutes = Number(time.match(/:(\d+)/)[1]);
        var AMPM = time.match(/\s(.*)$/)[1];
        if (AMPM == "PM" && hours < 12) hours = hours + 12;
        if (AMPM == "AM" && hours == 12) hours = hours - 12;
        var sHours = hours.toString();
        var sMinutes = minutes.toString();
        if (hours < 10) sHours = "0" + sHours;
        if (minutes < 10) sMinutes = "0" + sMinutes;
        var slotTime = sHours + ":" + sMinutes;

        var dateTime = date + ' ' + slotTime;
        var surveySlotTime = new Date(dateTime);
        $('#txtCognitionTestSlotTime').val(surveySlotTime.toUTCString());

        $('#surveyValidation').hide();
        $('#cognitionValidation').hide();
        var selectedSurveyId = [];
        var selectedCognitionTestId = [];
        $('#ddlSurveyId :selected').each(function (i, sel) {
            selectedSurveyId.push($(sel).val());
        });
        $('#ddlCognitionTestId :selected').each(function (i, sel) {
            selectedCognitionTestId.push($(sel).val());
        });
        $('#SurveyArrayString').val(selectedSurveyId);
        $('#CTesTArrayString').val(selectedCognitionTestId);
        console.log("submission area");
        $('#scheduleEntryForm').submit();
    }
}

$(document).ready(function () {
    $('#ddlSurveySlotID').change(function () {
        $('#txtSurveySlotTime').val(GetDefaultSlotime($('#ddlSurveySlotID').val()));
    });
    $('#ddlCognitionTestSlotID').change(function () {

        $('#txtCognitionTestSlotTime').val(GetDefaultSlotime($('#ddlCognitionTestSlotID').val()));
    });

    var userSettingId = parseInt($('#UserSettingID').val());
    if (userSettingId > 0) {
        if ($('#txtSurveySlotTime').val() != '') {
            var surveyDate = new Date($('#txtSurveySlotTime').val() + ' UTC');
            var surveySlotTime = formatDate(surveyDate);
            $('#txtSurveySlotTime').val(surveySlotTime);
        }
        else {
            $('#txtSurveySlotTime').val(GetDefaultSlotime($('#ddlSurveySlotID').val()));
        }

        if ($('#txtCognitionTestSlotTime').val() != '') {
            var cTestDate = new Date($('#txtCognitionTestSlotTime').val() + ' UTC');
            var cTestSlotTime = formatDate(cTestDate);
            $('#txtCognitionTestSlotTime').val(cTestSlotTime);
        }
        else {
            $('#txtCognitionTestSlotTime').val(GetDefaultSlotime($('#ddlCognitionTestSlotID').val()));
        }
    }

    $('#Timer1,#Timer2').datetimepicker({
        format: 'LT',
        ignoreReadonly: true
    });
});