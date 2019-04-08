/**
* Show message in popup
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
    $('#userListForm').trigger('submit');
    return true;
}

/**
* Load user details for edit
* @method UserEditClick
* @param {Number} rowId - User Id
*/
function UserEditClick(rowId) {
    var url = appPath + 'UserAdmin/EditUser/';
    var errorMsg = '';
    $.ajax({
        async: false,
        cache: false,
        type: 'GET',
        url: url,
        data: { userId: rowId },
        success: function (data) {
            window.location.reload(data);
        }
    });

}

/**
* Search Users on Study Id
* @method SearchUserClick
*/
function SearchUserClick() {
    var searchstr = $('#SearchId').val();
    var url = appPath + 'UserAdmin/SearchUsers/';
    var errorMsg = '';
    var data_string = 'searchStr=' + searchstr;
    $.ajax({
        async: false,
        cache: false,
        type: 'GET',
        url: url,
        data: data_string,
        success: function (result) {
            alert('Success: ' + result);
            $('#divUserList').html(result);
        },
        error: function (result) {
            alert('Error' + result.responseText);
        }
    });
}

/**
* Delete a user on user id
* @method DeleteUser
*/
function DeleteUser() {
    var messagehtml = "<div class=\"icon\"><i class=\"fa fa-check-circle\" style=\"cursor:pointer;\" onclick=\"$('#msgsuccess').hide();\"></i></div>";
    messagehtml = messagehtml + "The user has been deleted.<br/>";
    var url = appPath + 'UserAdmin/DeleteUser/';
    $.ajax({
        async: false,
        cache: false,
        type: 'POST',
        url: url,
        data: { userId: $('#currentUserId').val() },
        success: function (result) {
            $('#DeleteUserPopupConfirmation').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $('#msgsuccess').show();
            $("#msgsuccess").html(messagehtml);

            setTimeout(function () {
                $("#msgsuccess").hide();
                window.location = appPath + "UserAdmin/BackToUsers";
            }, 2000);
        },
        error: function (data) {
            alert('Error');
        }
    });
}

/**
* To activate or deactivate a user
* @method ActivateUser
*/
function ActivateUser(url) {
    var url = appPath + 'UserAdmin/ChangeUserStatus/';
    var messagehtml = "<div class=\"icon\"><i class=\"fa fa-check-circle\" style=\"cursor:pointer;\" onclick=\"$('#msgsuccess').hide();\"></i></div>";
    var toDo = $('#userFunctionality').val();
    switch (toDo) {
        case 'Deactivate': status = 'false';
            messagehtml = messagehtml + "The user has been deactivated.<br/>";
            break;
        case 'Activate': status = 'true';
            messagehtml = messagehtml + "The user has been activated successfully.<br/>";
            break;
    }
    $.ajax({
        async: false,
        cache: false,
        type: 'POST',
        url: url,
        data: { userId: $('#currentUserId').val(), Status: status },
        success: function (result) {
            $('#DeleteUserPopupConfirmation').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $('#msgsuccess').show();
            $("#msgsuccess").html(messagehtml);
            setTimeout(function () { $("#msgsuccess").hide(); location.reload(true); }, 2000);
        },
        error: function (data) {
            alert('Unexpected Error');
        }
    });
}

/**
* To show confirmation popup for Delete, Activate and Deactivate
* @method ShowDeleteUserConfirmationPopup
* @param {Number} userId - User Id
* @param {String} toDo - Action
* @return {bool} Status
*/
function ShowDeleteUserConfirmationPopup(userId, toDo) {
    $('#btnDelete').hide();
    $('#btnActivate').hide();
    switch (toDo) {
        case 'Delete': $('#confirmMessage').text("Are you sure you want to delete this user permanently?");
            $('#btnDelete').show();
            break;
        case 'Deactivate': $('#confirmMessage').text("Are you sure you want to deactivate this user?");
            $('#btnActivate').attr('title', 'Deactivate');
            $('#btnActivate').show();
            break;
        case 'Activate': $('#confirmMessage').text("Are you sure you want to activate this user?");
            $('#btnActivate').show();
            break;
    }
    $('#btnActivate').text(toDo.toUpperCase());
    $('#DeleteUserPopupConfirmation').modal('show');
    $('#currentUserId').val(userId);
    $('#userFunctionality').val(toDo);
    return false;
}
//-------------------------Index/Login----------------
/**
* Login Success functionality
* @method onLoginSuccess
* @param {Object} response - Response object
*/
function onLoginSuccess(response) {
    if (response.Errors.length > 0) {
        $("#Password").val("");
        $("#msgDiv").show();
        $("#msgDiv").html("<div class=\"icon\"><i class=\"fa fa-times-circle\" style=\"cursor:pointer;\" onclick=\" $('#msgDiv').hide();\"></i></div>");
        for (var i = 0; i < response.Errors.length; i++) {
            $("#msgDiv").html($("#msgDiv").html() + response.Errors[i].Message + "<br/>");
        }
    }
    else window.location = response.ReturnUrl;
}

/**
* Handle failure
* @method handleFailure
* @param {Object} ajaxContext - ajaxContext object
*/
function handleFailure(ajaxContext) {
    var obj = $.parseJSON(ajaxContext.responseText);
    alert(obj['message']);
}

/**
* Forgot password functionality
* @method forgotPasswordClick
*/
function forgotPasswordClick() {
    $('#loading').show();
    var url = appPath + 'Account/ForgotPassword/';
    var errorMsg = '';
    var t = setTimeout(function () {
        $.ajax({
            async: false,
            cache: false,
            type: 'POST',
            url: url,
            success: function (response) {
                if (response.Errors.length > 0) {
                    $("#msg").show();
                    $("#msgsuccess").hide();
                    $("#msg").html("<div class=\"icon\"><i class=\"fa fa-times-circle\" style=\"cursor:pointer;\" onclick=\"$('#msg').hide();\"></i></div>");
                    for (var i = 0; i < response.Errors.length; i++) {
                        $("#msg").html($("#msg").html() + response.Errors[i].Message + "<br/>");
                    }
                    $('#loading').hide();
                    window.setTimeout(function () {
                        $("#msg").hide();
                    }, 5000);
                }
                else {
                    $("#EmailForResetPassword").val("");
                    $("#msgsuccess").show();
                    $("#msg").hide();
                    $("#msgsuccess").html("<div class=\"icon\"><i class=\"fa fa-check-circle\" style=\"cursor:pointer;\" onclick=\"$('#msgsuccess').hide();\"></i></div>");
                    $("#msgsuccess").html($("#msgsuccess").html() + response.SuccessMessage + "<br/>");
                    $('#loading').hide();
                    window.setTimeout(function () {
                        $("#msgsuccess").hide();
                    }, 5000);
                }
            },
            failure: function (response) {
                $('#loading').hide();
            },
            error: function (response) {
                $('#loading').hide();
            }
        });
    }, 5000);
}

/**
* Close message popup
* @method Close
*/
function Close() {
    $("#EmailForResetPassword").val("");
    $("#msgsuccess").hide();
}
//-------------------------Index/Login----------------


//-------------------------Change password ----------------
/**
* Change password success handling
* @method onLoginSuccessCP
* @param {Object} response - Response object
*/
function onLoginSuccessCP(response) {
    if (response.Errors.length > 0) {
        $("#msg").show();
        $("#msgsuccess").hide();
        $("#msg").html("<div class=\"icon\"><i class=\"fa fa-times-circle\" style=\"cursor:pointer;\" onclick=\"$('#msg').hide();\"></i></div>");
        for (var i = 0; i < response.Errors.length; i++) {
            $("#msg").html($("#msg").html() + response.Errors[i].Message + "<br/>");
        }
        if (response.Errors[0].Key == "2046" || response.Errors[0].Key == "2047" || response.Errors[0].Key == "2048") {
            $("#OldPassword").val('');
            $("#OldPassword").focus();
        }
    } else {
        $("#msgsuccess").show();
        $("#msg").hide();
        $("#msgsuccess").html("<div class=\"icon\"><i class=\"fa fa-check-circle\" style=\"cursor:pointer;\" onclick=\"window.location = response.ReturnUrl;\"></i></div>");
        $("#msgsuccess").html($("#msgsuccess").html() + response.SuccessMessage + "<br/>");
        window.setTimeout(function () {
            $("#msgsuccess").hide();
        }, 5000);
    }
}

/**
* Change password failure handling
* @method handleFailureCP
* @param {Object} ajaxContext - ajaxContext object
*/
function handleFailureCP(ajaxContext) {
    var obj = $.parseJSON(ajaxContext.responseText);
    alert(obj['message']);
}
//-------------------------Change password ----------------
//-------------------------Reset password ----------------
/**
* Reset password success handling
* @method onLoginSuccessRP
* @param {Object} response - response object
*/
function onLoginSuccessRP(response) {
    if (response.Errors.length > 0) {
        $("#msg").show();
        $("#msgsuccess").hide();
        $("#msg").html("<div class=\"icon\"><i class=\"fa fa-times-circle\" style=\"cursor:pointer;\" onclick=\"$('#msg').hide();\"></i></div>");
        for (var i = 0; i < response.Errors.length; i++) {
            $("#msg").html($("#msg").html() + response.Errors[i].Message + "<br/>");
        }
    } else {
        $("#msgsuccess").show();
        $("#msg").hide();
        $("#msgsuccess").html("<div class=\"icon\"><i class=\"fa fa-check-circle\" style=\"cursor:pointer;\" onclick=\"window.location = response.ReturnUrl;\"></i></div>");
        $("#msgsuccess").html($("#msgsuccess").html() + response.SuccessMessage + "<br/>");
        window.setTimeout(function () {
            window.location.href = response.ReturnUrl;
        }, 5000);
    }
}

/**
* Reset password failure handling
* @method handleFailureRP
* @param {Object} ajaxContext - ajaxContext object
*/
function handleFailureRP(ajaxContext) {
    var obj = $.parseJSON(ajaxContext.responseText);
    alert(obj['message']);
}
//-------------------------Reset password ----------------
/**
* Set Offset value
* @method SetOffsetValue
*/
function SetOffsetValue() {
    var d = new Date()
    // we have added (-) to get the actual Timezone Offset
    var timeZoneOffset = -d.getTimezoneOffset();
    var url = appPath + 'UserAdmin/UpdateOffsetValue/';
    $.ajax({
        async: false,
        cache: false,
        type: 'GET',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: url,
        data: { offset: timeZoneOffset },
        success: function (result) {
            //alert('Success: ' + result);
        },
        error: function (result) {
            // alert('Error' + result.responseText);
        }
    });
}
//-------------------------Delete Admin ----------------
/**
* To show confirmation popup for Delete 
* @method ShowDeleteAdminConfirmationPopup
* @param {Number} adminId - Admin Id
* @param {String} toDo - Action
* @return {bool} Status
*/
function ShowDeleteAdminConfirmationPopup(adminId, toDo) {
    $('#btnDelete').hide();
    switch (toDo) {
        case 'Delete': $('#confirmMessageAdmin').text("Are you sure you want to delete this administrator account permanently?");
            $('#btnDelete').show();
            break;
    }
    $('#DeleteAdminPopupConfirmation').modal('show');
    $('#currentUserId').val(adminId);
    return false;
}
/**
* Delete a admin on admin id
* @method DeleteAdmin
*/
function DeleteAdmin() {
    var messagehtml = "<div class=\"icon\"><i class=\"fa fa-check-circle\" style=\"cursor:pointer;\" onclick=\"$('#msgsuccess').hide();\"></i></div>";
    messagehtml = messagehtml + "The administrator has been deleted.<br/>";
    var url = appPath + 'UserAdmin/DeleteAdmin/';
    setTimeout(function () {
        $('#loaderImage').show();
    }, 2000);
    $.ajax({
        async: false,
        cache: false,
        type: 'POST',
        url: url,
        data: { adminId: $('#currentUserId').val() },
        success: function (result) {
            $('#DeleteAdminPopupConfirmation').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $('#msgsuccess').show();
            $("#msgsuccess").html(messagehtml);

            setTimeout(function () {
                $("#msgsuccess").hide();
                $("#loaderImage").hide();
                window.location = appPath + "UserAdmin/BackToAdmins";
            }, 2000);
        },
        error: function (data) {
            alert('Error');
        }
    });
}
//-------------------------Delete Admin ----------------

function OnSuccess(data) {
    var sel_mode = $('#userTypeDropdownId').val();
    if (sel_mode == "1") {
        $('#DivExport').show();
        var totalRows = $('#UsersTotalRows', $(data)).val();

        $('#headerAdminOrUsers').html('Users <span class="count" id="spanTotalUser">' + totalRows + '</span>');
        $('#searchBox').html('');
        $('#searchBox').html('<input class="form-control" id="txtSearchId" name="UserListViewModel.SearchId" placeholder="Search with ID" tabindex="2" type="text" value="">');
        $('#anchor_add').html('<i class="add"></i> Add New User');
        $('#anchor_add').attr('href', appPath + 'UserAdmin/Edit');
        $('#anchor_add').attr('title', 'Add New User');
    }
    else {
        $('#DivExport').hide();
        var totalRows = $('#AdminTotalRows', $(data)).val();

        $('#headerAdminOrUsers').html('Admin <span class="count" id="spanTotalUser">' + totalRows + '</span>');
        $('#searchBox').html('');
        $('#searchBox').html('<input class="form-control" id="txtSearchId" name="AdminListViewModel.SearchId" placeholder="Search with Name" tabindex="2" type="text" value="">');

        $('#anchor_add').html('<i class="add"></i> Add New Admin');
        $('#anchor_add').attr('href', appPath + 'UserAdmin/CreateAdmin');
        $('#anchor_add').attr('title', 'Add New Admin');
    }
}
function ExportExcel() {
    var status = true;
    var counter = 0;
    var UserIds = [];
    $('#UserList > tbody > tr').each(function () {
        //getting tbody values
        if ($(this).find(".check").is(':checked')) {
            UserIds.push($(this).find("td:eq(2)").html());
            counter++;
        }
    });
    var postUserData = JSON.stringify({ userIds: UserIds });
    if ($("#txtFromDate").val() == "") {
        alert("Please select from date.");
        status = false;
    }
    if ($("#txtToDate").val() == "") {
        alert("Please select to date.");
        status = false;
    }

    var begin = $("#txtFromDate").val();
    var end = $('#txtToDate').val();

    var beginvals = begin.split("/");
    var endvals = end.split("/");
    var beginDate = new Date(beginvals[2], beginvals[1], beginvals[0]);
    var endDate = new Date(endvals[2], endvals[1], endvals[0]);
    if (beginDate > endDate) {
        alert("From date Should be less than To date");
        status = false;
    }
    if (counter <= 0) {
        alert("Please select users.");
        status = false;
    }


    if (status == true) {
        window.location = appPath + "UserAdmin/Export" + "?userIds=" + UserIds + "&fromDate=" + $("#txtFromDate").val() + "&toDate=" + $("#txtToDate").val();
    }
}


/*_______________________________________Protocol Date start_______________________________________*/
/**
* To set the protocol date and time 
* @method SetProtocolDate
* @param {Number} adminId - Admin Id
* @param {String} toDo - Action
* @return {bool} Status
*/

function SetProtocolDate()
{
    if($("#frmProtocol").valid())
    {
        var messagehtml = "<div class=\"icon\"><i class=\"fa fa-check-circle\" style=\"cursor:pointer;\" onclick=\"$('#msgsuccess').hide();\"></i></div>";
        messagehtml = messagehtml + "The Protocol date saved successfully.<br/>";
        $("#loaderImage").show();
        var param = {
            DatePart: $("#DatePart").val(),
            TimePart: GetUTCTimeForTimeSlotOptions($("#TimePart").val()),
            UserId: $("#currentUserId").val(),
        };
        var url = appPath + 'UserAdmin/SetProtocolDate/';
        $.ajax({
            async: true,
            cache: false,
            type: 'POST',
            url: url,
            data: param,
            success: function (result) {
                $('#ProtocolModal').modal('toggle');
                $('#msgsuccess').show();
                $("#msgsuccess").html(messagehtml);
                $("#loaderImage").hide();
                setTimeout(function () {
                    $("#msgsuccess").hide();
                }, 2000);
            },
            error: function (data) {
                alert('Error');
            }
        });
    }
}

function GetProtocolDate(UserId) {
       $("#loaderImage").show();
        var param = {
            UserId: UserId,
        };
        var url = appPath + 'UserAdmin/getProtocolDate/';
       
        $.ajax({
            async: true,
            cache: false,
            type: 'POST',
            url: url,
            data: param,
            success: function (result) {
                ClearProtocolFields();
                if (result != null && result.TimePart != null) {
                    ConvertToLocalTimeOption(result.TimePart);
                }
                $("#loaderImage").hide();
            },
            error: function (data) {
                alert('Error');
            }
        });
    }

function ClearProtocolFields()
{
    $("#frmProtocol").validate().resetForm();
    $(".field-validation-error").find('span').remove();
    $("#DatePart").val("");
    $("#TimePart").val("");
}
function SetcurrentUserId(Userid) {
    $('#currentUserId').val(Userid);
    GetProtocolDate(Userid);
}
function GetUTCTimeForTimeSlotOptions(slotime) {
    var today = new Date();
    var date = today.getFullYear() + '/' + (today.getMonth() + 1) + '/' + today.getDate();
     var dateval = $('#DatePart').val();
     var result = dateval.split("/");
     date = parseInt(result[2]) + '/' + parseInt(result[1]) + '/' + parseInt(result[0]);
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

function ConvertToLocalTimeOption( dateTimeString) {
    var ProtocolDate = new Date((parseFloat(dateTimeString)));
    var ProtocoTimestr = formatDate(ProtocolDate, "TIME");
    var ProtocoDatestr = formatDate(ProtocolDate, "DATE");
    $("#DatePart").val(ProtocoDatestr);
    $("#TimePart").val(ProtocoTimestr);
}
function formatDate(date,Type) {
    // formats a javascript Date object into a 12h AM/PM time string
    if (Type == "TIME")
    {
        var hour = date.getHours();
        var minute = date.getMinutes();
        var amPM = (hour > 11) ? "PM" : "AM";
        if (hour > 12) {
            hour -= 12;
        } else if (hour == 0) {
            hour = "12";
        }
        if (minute < 10) {
            minute = "0" + minute;
        }
        return hour + ":" + minute + " "+amPM;
    }
    else if (Type == "DATE")
    {
        var month = date.getMonth()+1;
        return ((date.getDate()) + "/" + (month<10?("0"+month):month) + "/" + (date.getFullYear()));

    }
   
}

/*_______________________________________Protocol Date End_______________________________________*/

function SuccessPaginationAdmin() {
    $("#loaderImage").hide();
    $('body,html').animate({
        scrollTop: 0
    }, 300);
}
function BeginPaginaionAdmin() {
    $('#loaderImage').show();
}

function SuccessPaginationUser() {
    $("#loaderImage").hide();
    $('body,html').animate({
        scrollTop: 0
    }, 300);
    $('#checkAll').prop('checked', false);
    $('#txtFromDate').val('');
    $('#txtToDate').val('');
}
function BeginPaginaionUser() {
    $('#loaderImage').show();
}

function OnBeginAjaxCall_User() {
    $('#loaderImage').show();
}
function OnSuccessAjaxCall_User() {
    $("#loaderImage").hide();
}
function OnFailureAjaxCall_User(data) {
    $("#loaderImage").hide();
}
function OnBeginAjaxCall_Admin() {
    $('#loaderImage').show();
}
function OnSuccessAjaxCall_Admin() {
    $("#loaderImage").hide();
}
function OnFailureAjaxCall_Admin(data) {
    $("#loaderImage").hide();
}