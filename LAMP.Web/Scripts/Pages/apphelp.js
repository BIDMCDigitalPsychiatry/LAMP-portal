/**
* To Show Delete AppHelp Confirmation Popup
* @method ShowDeleteAppHelpConfirmationPopup
* @param {Integer} helpId - userid
* @param {String} toDo - toDo
*/
function ShowDeleteAppHelpConfirmationPopup(helpId, toDo) {
    $('#btnDelete').hide();
    switch (toDo) {
        case 'Delete': $('#confirmMessage').text("Are you sure you want to delete this app help permanently?");
            $('#btnDelete').show();
            break;
    }
    $('#DeleteAppHelpPopupConfirmation').modal('show');
    $('#currentUserId').val(helpId);
    return false;
}

/**
* To Delete App Help
* @method DeleteAppHelp
*/
function DeleteAppHelp() {
    var messagehtml = "<div class=\"icon\"><i class=\"fa fa-check-circle\" style=\"cursor:pointer;\" onclick=\"$('#msgsuccess').hide();\"></i></div>";
    messagehtml = messagehtml + "The app help has been deleted.<br/>";
    var url = appPath + 'UserAdmin/DeleteAppHelp/';
    $.ajax({
        async: false,
        cache: false,
        type: 'POST',
        url: url,
        data: { helpId: $('#currentUserId').val() },
        success: function (result) {
            $('#DeleteAppHelpPopupConfirmation').modal('hide');
            $('body').removeClass('modal-open');
            $('.modal-backdrop').remove();
            $('#msgsuccess').show();
            $("#msgsuccess").html(messagehtml);
            setTimeout(function () { $("#msgsuccess").hide(); location.reload(true); }, 2000);
        },
        error: function (data) {
            alert('Error');
        }
    });
}