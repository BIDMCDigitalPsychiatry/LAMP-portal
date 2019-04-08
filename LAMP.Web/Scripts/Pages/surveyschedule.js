var selecteddropdownType = $('#ddlSurveyRepeatId').val();
/**
* Save survey schedule details
* @method savesurveyschedule
*/
function savesurveyschedule() {
    var status = true;
    var selectedType = $('#ddlSurveyRepeatId').val();
    var optionsCount = 0;
    $('#Options').val('');
    if ((selectedType > 4 && selectedType < 11) || selectedType == 12) {
        /*Survey Slot Time Mandatory Validation*/
        if ($('#txtSurveySlotTime').val() == "") {
            $('#SurveySlotTimeValidation').show();
            status = false;
        }
        else {
            $('#SurveySlotTimeValidation').hide();
        }
    }

    /*Survey Dropdown Validation*/
    if ($("#ddlSurveyId option:selected").length == 0) {
        $('#surveyValidation').show();
        status = false;
    }
    else {
        $('#surveyValidation').hide();
    }

    if ((selectedType > 4 && selectedType < 11) || selectedType == 12) {
        /*Schedule Date Picker Validation*/
        if ($("#txtSurveyScheduleDate").val() == "") {
            $('#scheduleDateValidation').show();
            status = false;
        }
        else {
            $('#scheduleDateValidation').hide();
        }
    }


    if (selectedType == "11") {
        var arrayOptions = [];
        $('#optionsDiv > div').each(function () {
            if ($(this).find(".optInput").val() != '') {
                var text = $.trim($(this).find(".optInput").val());
                var optionText = $(this).find(".optInput").val();
                var options = $('#Options').val();
                if (optionText != '') {
                    if (options != "") {
                        options += "," + optionText;
                    }
                    else
                        options += optionText;
                    $('#Options').val(options);
                }
                optionsCount++;
            }
        });
        if (optionsCount > 0) {
            var commastring = $('#Options').val();
            var arry = commastring.split(',');
            for (i = 0; i < arry.length; i++)
                arrayOptions.push(GetUTCTimeForTimeSlotOptions(arry[i]));
            $('#OptionsArray').val(JSON.stringify(arrayOptions));
        }
        if (optionsCount == 0) {
            $('#optionsSpan').html("Specify time options.");
            $('#optionsSpan').show();
            status = false;
        }
        else {
            if (optionsCount < 2) {
                $('#optionsSpan').html("Please add at least 2 time options.");
                $('#optionsSpan').show();
                status = false;
            }
        }
    }

    if (status == false) {
        return false;
    }
    else {
        var time = $('#txtSurveySlotTime').val();
        $('#txtSurveySlotTime').val(GetUTCTimeForTimeSlotOptions(time));
        $('#surveyValidation').hide();
        $('#ScheduleSurveyForm').submit();
    }
}

/**
* Get UTCTime For TimeSlot Options
* @method GetUTCTimeForTimeSlotOptions
* @param {String} slotime - slot time
*/
function GetUTCTimeForTimeSlotOptions(slotime) {
    var today = new Date();
    var date = today.getFullYear() + '/' + (today.getMonth() + 1) + '/' + today.getDate();
    var repeatId = $('#ddlSurveyRepeatId').val();

    if (repeatId == 5 || repeatId == 6 || repeatId == 7 || repeatId == 8 || repeatId == 9 || repeatId == 10 || repeatId == 12) {
        var dateval = $('#txtSurveyScheduleDate').val();
        var result = dateval.split("/");
        date = parseInt(result[2]) + '/' + parseInt(result[1]) + '/' + parseInt(result[0]);
    }

    //********************************
    var time = slotime;
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

    return (surveySlotTime.toUTCString());

}

/**
* Get Default Slot time
* @method GetDefaultSlotime
* @param {String} slot - slot
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
    else {
        return "12:00 AM";
    }
}

/**
* To format Date
* @method formatDate
* @param {DateTime} date - date
* @return {String} time
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
* To Validate SurveyTime
* @method ValidateSurveyTime
*/
function ValidateSurveyTime() {
    var session = $('#ddlSurveySlotID').val();
    var flag = true;
    var timeSelected = $('#txtSurveySlotTime').val();
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
* @method AddOptionRow
* @param {Object} addButton - addButton
*/
function AddOptionRow(addButton) {

    var option = $(addButton).parent().parent().find('#optionTextBox').val();
    if ($.trim(option) == '') {
        $('#optionsSpan').html("Specify time option.");
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
        var html = '<div class="add-sec"><div class="input-group date schedulepickadd" id="schedulepick"><input readonly type="text" class="calendarInput form-control optInput optionText_' + rowCount + '" id="optionTextBox"><input type="hidden" id="optionId" class="optionId" value="0" />';
        html += '<span class="input-group-addon"><span class="glyphicon glyphicon-time"></span></span></div>';
        html += '<div class="controls"><button type="button" class="glyphicon glyphicon-plus add-icn addButton" id="addButton" onclick="return AddOptionRow(this)"></button>';
        html += '<button type="button"class="glyphicon glyphicon-remove add-icn" id="removeButton" onclick="return RemoveOptionRow(this)" style="display:none"></button></div></div>';

        $('#optionsDiv').append(html);
        $('.schedulepickadd').datetimepicker({
            format: 'LT',
            ignoreReadonly: true
        });
    }
    return true;
}

/**
* @method RemoveOptionRow
* @param {Button} removeButton - removeButton
*/
function RemoveOptionRow(removeButton) {
    var count = $('.removeButton').length;
    count = count - 1;
    var addButtonCount = $('.addButton:visible').length;
    console.log("Count:" + addButtonCount);
    $(removeButton).parent().parent().remove();
    if (addButtonCount < 1) {
        var html = '<div class="add-sec"><div class="input-group date schedulepick0 schedulepickadd" id="schedulepick"><input type="text" readonly class="form-control optInput optionText_' + count + '" id="optionTextBox"><input type="hidden" id="optionId" class="optionId" value="0" />';
        html += '<span class="input-group-addon"><span class="glyphicon glyphicon-time"></span></span></div>';
        html += '<div class="controls"><button type="button" class="glyphicon glyphicon-plus add-icn addButton" id="addButton" onclick="return AddOptionRow(this)"></button>';
        html += '<button type="button" class="glyphicon glyphicon-remove add-icn removeButton" id="removeButton" onclick="return RemoveOptionRow(this)" style="display:none"></button></div></div>';
    }
    $('#optionsDiv').append(html);
    $('.schedulepickadd').datetimepicker({
        format: 'LT',
        ignoreReadonly: true
    });
    return true;
}

$(document).ready(function () {
    $(function () {
        $('#schedulepick1').datetimepicker({
            format: 'DD/MM/YYYY'
        });
        $('.schedulepick').datetimepicker({
            format: 'LT',
            ignoreReadonly: true
        });
    });

    var scheduledDateString = $('#SurveyScheduleDateValue').val();
    var txtSurveySlotTime = $('#txtSurveySlotTime').val();
    $('#txtSurveySlotTime').val(GetDefaultSlotime(0));
    var AdminSurveySchID = parseInt($('#AdminSurveySchID').val());
    if (AdminSurveySchID > 0) {
        if (txtSurveySlotTime != '') {
            var surveyDate = new Date(txtSurveySlotTime + ' UTC');
            var surveySlotTime = formatDate(surveyDate);
            $('#txtSurveySlotTime').val(surveySlotTime);
        }
        else {
            $('#txtSurveySlotTime').val(GetDefaultSlotime(0));
        }
        if (selecteddropdownType == "2" || selecteddropdownType == "3" || selecteddropdownType == "4" || selecteddropdownType == "11") {
            $("#scheduleDateDiv").hide();
            $("#slotTimeDiv").hide();
        }
    }
    else {
        if (selecteddropdownType == "2" || selecteddropdownType == "3" || selecteddropdownType == "4" || selecteddropdownType == "11") {
            $("#scheduleDateDiv").hide();
            $("#slotTimeDiv").hide();
        }
    }

    $('#ddlSurveyRepeatId').change(function () {
        if ($('#ddlSurveyRepeatId').val() != "") {
            var selectedType = $('#ddlSurveyRepeatId').val();
            if (selectedType == "11") {
                $('#optionsDiv').show();
                $("#scheduleDateDiv").hide();
                $("#slotTimeDiv").hide();
            }
            else if (selectedType == "2" || selectedType == "3" || selectedType == "4") {
                $("#scheduleDateDiv").hide();
                $("#slotTimeDiv").hide();
                $('#optionsDiv').hide();
            }
            else {
                $('#optionsDiv').hide();
                $("#scheduleDateDiv").show();
                $("#slotTimeDiv").show();
            }
        }
        else {
            $('#optionsDiv').hide();
        }
    });

    $('#Timer1').datetimepicker({
        format: 'LT',
        ignoreReadonly: true
    });
});