var selecteddropdownType = $('#ddlCognitionTestRepeatId').val();

/**
* To Validate CognitionTime
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
    else {
        return "12:00 AM";
    }
}

/**
* To format date time
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
* To Get UTC Time For TimeSlot Options
* @method GetUTCTimeForTimeSlotOptions
* @param {String} slotime - slotime
*/
function GetUTCTimeForTimeSlotOptions(slotime) {
    var today = new Date();
    var date = today.getFullYear() + '/' + (today.getMonth() + 1) + '/' + today.getDate();
    var repeatId = $('#ddlCognitionTestRepeatId').val();

    if (repeatId == 5 || repeatId == 6 || repeatId == 7 || repeatId == 8 || repeatId == 9 || repeatId == 10 || repeatId == 12) {
        var dateval = $('#txtGameScheduleDate').val();
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
* To save schedule game
* @method saveschedulegame
*/
function saveschedulegame() {
    var status = true;
    var selectedType = $('#ddlCognitionTestRepeatId').val();
    var optionsCount = 0;
    if ((selectedType > 4 && selectedType < 11) || selectedType == 12) {
        /*Game Slot Time Mandatory Validation*/
        if ($('#txtCognitionTestSlotTime').val() == "") {
            $('#CognitionSlotTimeValidation').show();
            status = false;
        }
        else {
            $('#CognitionSlotTimeValidation').hide();
        }
    }

    /*Game Dropdown Validation*/
    if ($("#ddlCognitionTestId option:selected").length == 0) {
        $('#cognitionValidation').show();
        status = false;
    }
    else {
        $("#cognitionValidation").hide();
    }

    /*Game Version Dropdown Validation*/
    if ($('#ddlCognitionVersionId option:selected').length == 0) {
        $('#cognitionVersionValidation').show();
        status = false;
    }
    else {
        $("#cognitionVersionValidation").hide();
    }

    if ((selectedType > 4 && selectedType < 11) || selectedType == 12) {
        /*Schedule Date Picker Validation*/
        if ($("#txtGameScheduleDate").val() == "") {
            $('#scheduleDateValidation').show();
            status = false;
        }
        else {
            $('#scheduleDateValidation').hide();
        }
    }
    /* Options div Validation and binding*/
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
        var time = $('#txtCognitionTestSlotTime').val();
        $('#txtCognitionTestSlotTime').val(GetUTCTimeForTimeSlotOptions(time));

        $('#cognitionValidation').hide();
        $('#scheduleEntryForm').submit();
    }
}

/**
* Load version
* @method versionLoad
* @param {Integer} j 
*/
function versionLoad(j) {
    var ddlSubCategory = $('#ddlCognitionVersionId');
    var $versions = $('#ddlCognitionVersionId');
    var url = '@Url.Action("GetCognitionVersion", "UserAdmin")';
    $.getJSON(url, { cognitionId: j }, function (response) {
        $('#ddlCognitionVersionId  > option').remove();
        if (response != null) {
            if (response.length > 0) {
                $versions.empty();
                for (i = 0; i < response.length; i++) {
                    ddlSubCategory.append(new Option(response[i].Text, response[i].Value));
                }
            }
            else {
                var opt = new Option('(No Versions)', '-1');
                $(opt).attr("selected", "selected");
                $(ddlSubCategory).append(opt);
            }
        }
        ddlSubCategory.selectpicker('refresh');
    });
    $('#ddlCognitionVersionId option:first-child').attr("selected", "selected");
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
* @param {Object} removeButton - removeButton
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
        $('#schedulepick2').datetimepicker({
            format: 'DD/MM/YYYY'
        });
        $('.schedulepick').datetimepicker({
            format: 'LT',
            ignoreReadonly: true
        });
    });
    var txtCognitionTestSlotTime = $('#txtCognitionTestSlotTime').val();
    var AdminCTestSchID = parseInt($('#AdminCTestSchID').val());
    if (AdminCTestSchID > 0) {
        if ($('#txtCognitionTestSlotTime').val() != '' && $('#txtCognitionTestSlotTime').val() != null) {
            var cTestDate = new Date($('#txtCognitionTestSlotTime').val() + ' UTC');
            var cTestSlotTime = formatDate(cTestDate);
            $('#txtCognitionTestSlotTime').val(cTestSlotTime);
        }
        else {
            $('#txtCognitionTestSlotTime').val(GetDefaultSlotime(0));
        }
        if (selecteddropdownType == "2" || selecteddropdownType == "3" || selecteddropdownType == "4" || selecteddropdownType == "11") {
            $("#scheduleDateDiv").hide();
            $("#slotTimeDiv").hide();
        }
    }
    else {
        $('#txtCognitionTestSlotTime').val(GetDefaultSlotime(0));
        if (selecteddropdownType == "2" || selecteddropdownType == "3" || selecteddropdownType == "4" || selecteddropdownType == "11") {
            $("#scheduleDateDiv").hide();
            $("#slotTimeDiv").hide();
        }
    }
    $('#ddlCognitionTestRepeatId').change(function () {
        if ($('#ddlCognitionTestRepeatId').val() != "") {
            var selectedType = $('#ddlCognitionTestRepeatId').val();
            if (selectedType == "11") {
                $('#optionsDiv').show();
                $("#scheduleDateDiv").hide();
                $("#slotTimeDiv").hide();
            }
            else if (selectedType == "2" || selectedType == "3" || selectedType == "4") {
                $('#optionsDiv').hide();
                $("#scheduleDateDiv").hide();
                $("#slotTimeDiv").hide();
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

    $('#Timer2').datetimepicker({
        format: 'LT',
        ignoreReadonly: true
    });
});
